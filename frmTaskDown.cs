using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.Win32;

namespace TaskDown;

public partial class FrmTaskDown : Form
{
    private readonly ArrayList windowHandles = [];
    private static readonly string appName = "TaskDown";
    private static readonly string assLctn = Path.Combine(AppContext.BaseDirectory, "TaskDown.exe");  // EXE-Pfad
    private readonly string saveScriptPath = Path.Combine(AppContext.BaseDirectory, "TaskDown.ps1");
    private static readonly List<Tuple<Process, IntPtr>> processes = [];
    private bool isTaskDialogOpen = false;
    private bool isDoubleClicked = false;

    public FrmTaskDown()
    {
        InitializeComponent();
        textBox.Text = Path.Exists(saveScriptPath) ? saveScriptPath : string.Empty;
    }

    private void FrmTaskDown_Load(object sender, EventArgs e)
    {
        if (!NativeMethods.RegisterHotKey(Handle, NativeMethods.HOTKEY_ID0, (uint)NativeMethods.Modifiers.Win, (uint)Keys.End))
        {
            Utilities.ErrorMsgTaskDlg(Handle, "Win+Ende konnte nicht registriert werden.\nWahrscheinlich wird die Tastenkombination\nbereits von einer anderen App benutzt.");
        }
        if (NativeMethods.RegisterHotKey(Handle, NativeMethods.HOTKEY_ID1, (uint)(NativeMethods.Modifiers.Control | NativeMethods.Modifiers.Win), (uint)Keys.End) == false)
        {
            Utilities.ErrorMsgTaskDlg(Handle, "Strg+Win+Ende konnte nicht registriert werden.\nWahrscheinlich wird die Tastenkombination\nbereits von einer anderen App benutzt.");
        }
        if (NativeMethods.RegisterHotKey(Handle, NativeMethods.HOTKEY_ID2, (uint)(NativeMethods.Modifiers.Shift | NativeMethods.Modifiers.Control | NativeMethods.Modifiers.Win), (uint)Keys.End) == false)
        {
            Utilities.ErrorMsgTaskDlg(Handle, "Shift+Strg+Win+Ende konnte nicht registriert werden.\nWahrscheinlich wird die Tastenkombination\nbereits von einer anderen App benutzt.");
        }
        if (Utilities.IsAutoStartEnabled(appName)) { ckbAutoStart.Checked = true; }

        var args = Environment.GetCommandLineArgs();
        for (var i = 0; i < args.Length; i++)
        {
            if (args[i].StartsWith('/'))
            {
                var option = args[i][1..];
                switch (option.ToLowerInvariant())
                {
                    case "shutdown":
                        ShutDown_Dialog(); ;
                        break;
                    case "tasklist":
                        TaskList_Dialog();
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private void FrmTaskDown_Shown(object sender, EventArgs e)
    {
        Hide();
        Opacity = 100;
    }

    private void ShowHideToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (Visible) { Hide(); }
        else
        {
            Show();
            TopMost = true; // make our form jump to the top of everything
            BringToFront();
            Activate();
        }
    }

    private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
    {
        Close();
        Application.Exit();
    }

    private void FrmTaskDown_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (e.CloseReason == CloseReason.UserClosing && (ModifierKeys & Keys.Shift) != Keys.Shift) // Anwendung kann nur über «Beenden» in cntxtMenuTNA beendet werden!
        {
            e.Cancel = true; // das Schließen des Formulars verhindern
            Hide();
        }
        else
        {
            NativeMethods.UnregisterHotKey(Handle, NativeMethods.HOTKEY_ID0);
            NativeMethods.UnregisterHotKey(Handle, NativeMethods.HOTKEY_ID1);
            NativeMethods.UnregisterHotKey(Handle, NativeMethods.HOTKEY_ID2);
        }
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        switch (keyData)
        {
            case Keys.Escape | Keys.Shift:
            case Keys.F4 | Keys.Alt:
                {
                    Application.Exit();
                    return true;
                }
            case Keys.Escape: { Hide(); return true; }
        }
        return base.ProcessCmdKey(ref msg, keyData);
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == NativeMethods.WM_HOTKEY && m.WParam == NativeMethods.HOTKEY_ID0)
        {
            if (!isTaskDialogOpen)
            {
                if (Visible) { Hide(); }
                else { ShutDown_Dialog(); }
            }
        }
        else if (m.Msg == NativeMethods.WM_HOTKEY && m.WParam == NativeMethods.HOTKEY_ID1)
        {
            if (isTaskDialogOpen || Visible) { Hide(); return; }
            if (AltTabList() > 0 && processes[0] != null)
            {
                var activeWindowHandle = processes[0].Item1.MainWindowHandle;
                if (activeWindowHandle == IntPtr.Zero) { activeWindowHandle = processes[0].Item2; }
                if (activeWindowHandle != IntPtr.Zero && activeWindowHandle == NativeMethods.GetForegroundWindow())
                {
                    if (!NativeMethods.PostMessage(activeWindowHandle, NativeMethods.WM_SYSCOMMAND, NativeMethods.SC_CLOSE, IntPtr.Zero))
                    {
                        BeginInvoke(new Action(Console.Beep));
                    }
                }
                else { BeginInvoke(new Action(() => TaskList_Dialog())); }
            }
            else { BeginInvoke(new Action(() => ShutDown_Dialog())); }
        }
        else if (m.Msg == NativeMethods.WM_HOTKEY && m.WParam == NativeMethods.HOTKEY_ID2)
        {
            if (isTaskDialogOpen || Visible) { Hide(); return; }
            if (AltTabList() > 0 && processes[0] != null)
            {
                var namedProcesses = Process.GetProcessesByName((string?)processes[0].Item1.ProcessName);
                foreach (var namedProcess in namedProcesses)
                {
                    var currWindowHandle = namedProcess.MainWindowHandle;
                    if (currWindowHandle != IntPtr.Zero)
                    {
                        if (!NativeMethods.PostMessage(currWindowHandle, NativeMethods.WM_SYSCOMMAND, NativeMethods.SC_CLOSE, IntPtr.Zero))
                        {
                            BeginInvoke(new Action(Console.Beep));
                        }
                    }
                }
            }
            else { BeginInvoke(new Action(() => ShutDown_Dialog())); }
        }
        else if (m.Msg == NativeMethods.WM_SHOWTASKDOWN) { ShutDown_Dialog(); } // another instance is started
        else { base.WndProc(ref m); }
    }

    private void TaskList_Dialog(bool showShutDown = false)
    {
        if (AltTabList() == 0)
        {
            ShutDown_Dialog();
            return;
        }

        isTaskDialogOpen = true;
        var btnCancel = TaskDialogButton.No; // Cancel oder Abort stehen vor Continue - lässt sich nicht ändern
        var btnCloseAll = TaskDialogButton.Yes; // Es können nicht gleichzeitig benutzerdefinierte Schaltflächen und Befehlslinks angezeigt werden."
        var bar = processes.Count == 3 ? "alle drei" : processes.Count == 2 ? "beide" : "ALLE " + processes.Count;
        bar = processes.Count == 1 ? "wird die App geschlossen!" : "werden " + bar + " Apps geschlossen!";
        var page = new TaskDialogPage()
        {
            Caption = appName + " - Geöffnete Apps",
            Heading = processes.Count > 1 ? "Möchten Sie alle Apps schließen?" : "Möchten Sie die App schließen?",
            Text = "Klicken Sie " + (processes.Count > 1 ? "eine" : "die") + " Schaltfläche, um die App in den Vordergrund zu holen.",
            Icon = TaskDialogIcon.ShieldWarningYellowBar,
            AllowCancel = true,
            Buttons = { btnCloseAll, btnCancel },
            DefaultButton = btnCloseAll,
            Footnote = new TaskDialogFootnote() { Text = "Wenn sie »Ja« wählen " + bar },
            Verification = new TaskDialogVerificationCheckBox()
            {
                Text = "Aktionen-Dialog danach anzeigen",
                Checked = showShutDown
            }
        };
        foreach (var process in processes)
        {
            var title = process.Item1.MainWindowTitle;
            if (string.IsNullOrEmpty(title)) { title = NativeMethods.WinGetTitle(process.Item2); }
            if (string.IsNullOrEmpty(title)) { title = "N.N."; }
            var linkButton = new TaskDialogCommandLinkButton(Utilities.Truncate(title, 40))
            {
                ShowShieldIcon = false,
                DescriptionText = process.Item1.MainModule != null ? Path.GetFileName(process.Item1.MainModule.FileName) : process.Item1.ProcessName,
                Tag = processes.IndexOf(process)
            };
            page.Buttons.Add(linkButton);
        }
        var result = TaskDialog.ShowDialog(this, page);
        if (result.Tag is not null) { ShowInForeground(result.Tag); }
        else if (result == btnCloseAll)
        {
            foreach (var process in processes)
            {
                var activeWindowHandle = process.Item1.MainWindowHandle;
                if (activeWindowHandle == IntPtr.Zero) { activeWindowHandle = process.Item2; }
                if (activeWindowHandle != IntPtr.Zero) { NativeMethods.PostMessage(activeWindowHandle, NativeMethods.WM_SYSCOMMAND, NativeMethods.SC_CLOSE, IntPtr.Zero); }
            }
            Application.DoEvents();
            Thread.Sleep(1000);
            if (page.Verification.Checked && AltTabList() == 0) { BeginInvoke(new Action(() => ShutDown_Dialog())); } // BeginInvoke(new Action(ShutDown_Dialog)) <= wenn ohne Argumente 
        }
        else if (result == btnCancel && showShutDown && page.Verification.Checked) { BeginInvoke(new Action(() => ShutDown_Dialog(false))); } //new Action(() => TaskList_Dialog()))
        isTaskDialogOpen = false;
    }

    private void ShutDown_Dialog(bool checkApps = true)
    {
        isTaskDialogOpen = true;
        var page = new TaskDialogPage()
        {
            Caption = appName + " - Aktionen",
            Icon = TaskDialogIcon.ShieldBlueBar,
            Heading = "Wählen Sie eine Aktion:",
            AllowCancel = true,
            AllowMinimize = true, // A modeless dialog can be minimizable.
            Verification = new TaskDialogVerificationCheckBox()
            {
                Text = "Vor dem Shutdown geöffnete Apps ggf. anzeigen",
                Checked = checkApps
            },
        };
        page.Buttons.Add(new TaskDialogCommandLinkButton("&Herunterfahren", "Alle Apps werden geschlossen und der PC wird ausgeschaltet.") { Tag = "shutdown" });
        using (var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Power"))
        {
            if (key != null)
            {
                var value = key.GetValue("HibernateEnabledDefault");
                if (value != null && (int)value == 1)
                {
                    page.Buttons.Add(new TaskDialogCommandLinkButton("&Ruhezustand", "Apps bleiben geöffnet. Der Zustand wird auf der SSD gespeichert.") { Tag = "hibernate" });
                }
            }
        }
        page.Buttons.Add(new TaskDialogCommandLinkButton("&Energie sparen", "Apps bleiben geöffnet. Der Zustand wird im RAM gespeichert.") { Tag = "sleep" });
        page.Buttons.Add(new TaskDialogCommandLinkButton("&Monitor ausschalten", "Apps bleiben geöffnet. Der PC wird nicht gesperrt.") { Tag = "monitor" });
        page.Buttons.Add(new TaskDialogCommandLinkButton("&Sperren", "Apps bleiben geöffnet. Der Sperrbildschirm wird aktiviert.") { Tag = "lock" });
        page.Buttons.Add(new TaskDialogCommandLinkButton("&Abmelden", "Alle Apps werden geschlossen. Der Benutzer wird abgemeldet.") { Tag = "logoff" });
        page.Buttons.Add(new TaskDialogCommandLinkButton("&Neu starten", "Alle Apps werden geschlossen und der PC startet neu.") { Tag = "restart" });
        page.Buttons.Add(new TaskDialogCommandLinkButton("&Firmware", "Der PC fährt herunter und bootet in die BIOS-Einstellungen.") { Tag = "firmware" });
        if (Path.Exists(textBox.Text) && Path.GetExtension(textBox.Text) == ".ps1")
        {
            page.Buttons.Add(new TaskDialogCommandLinkButton("&Datensicherung", textBox.Text) { Tag = "save" });
        }
        page.Buttons.Add(TaskDialogButton.Cancel);
        var result = TaskDialog.ShowDialog(this, page);
        if (result.Tag is not null) { Execute((string)result.Tag, page.Verification.Checked); }
        isTaskDialogOpen = false;
    }

    private static void ShowInForeground(object pid)
    {
        var foo = processes[(int)pid];
        if (foo != null)
        {
            var activeWindowHandle = foo.Item1.MainWindowHandle;
            if (activeWindowHandle == IntPtr.Zero) { activeWindowHandle = foo.Item2; }
            if (activeWindowHandle != IntPtr.Zero)
            {
                NativeMethods.SetForegroundWindow(activeWindowHandle);
                if (NativeMethods.IsIconic(activeWindowHandle)) { _ = NativeMethods.ShowWindow(activeWindowHandle, NativeMethods.SW_RESTORE); }
                else { _ = NativeMethods.ShowWindow(activeWindowHandle, NativeMethods.SW_SHOW); }
            }
        }
    }

    private void FrmTaskDown_HelpButtonClicked(object sender, System.ComponentModel.CancelEventArgs e)
    {
        e.Cancel = true;
        Utilities.HelpMsgTaskDlg(Handle, Icon);
    }

    private void FrmTaskDown_HelpRequested(object sender, HelpEventArgs hlpevent)
    {
        hlpevent.Handled = true;
        Utilities.HelpMsgTaskDlg(Handle, Icon);
    }

    private void FrmTaskDown_VisibleChanged(object sender, EventArgs e)
    {
        if (Visible)
        {
            listView.Items.Clear();
            AltTabList();
            for (var i = 0; i < processes.Count; i++)
            {
                var item = new ListViewItem(processes[i].Item1.Id.ToString());
                item.SubItems.Add(processes[i].Item1.ProcessName);
                item.SubItems.Add(NativeMethods.WinGetClass(processes[i].Item1.MainWindowHandle));
                item.SubItems.Add(NativeMethods.WinGetTitle(processes[i].Item1.MainWindowHandle));
                listView.Items.Add(item);
            }
        }
    }

    private async void Execute(string? itemName, bool checkApps)
    {
        var filename = string.Empty;
        var arguments = string.Empty;
        var shellExecute = false; // Default in .NET-Core
        switch (itemName)
        {
            case "save":
                if (checkApps && AltTabList() > 0) { TaskList_Dialog(true); }
                else
                {
                    Utilities.SplashWait();  // default 1000 ms
                    //Invoke(static () => { var newForm = new FrmSplashWait(); newForm.Show(); });
                    //await Task.Delay(1000);

                    filename = "powershell.exe";
                    arguments = $"-NoProfile -ExecutionPolicy RemoteSigned -file \"{textBox.Text}\"";
                }
                break;
            case "shutdown":
                if (checkApps && AltTabList() > 0) { TaskList_Dialog(true); }
                else
                {
                    filename = "shutdown.exe";
                    arguments = "-s /t 4 /c \"Windows wird heruntergefahren\"";
                }
                break;
            case "restart":
                if (checkApps && AltTabList() > 0) { TaskList_Dialog(true); }
                else
                {
                    filename = "shutdown.exe";
                    arguments = "-r /t 5 /c \"Windows wird neu gestartet\"";
                }
                break;
            case "monitor":  // Monitor abschalten
                Utilities.SplashWait();  // default 1000 ms
                NativeMethods.SendMessage(Handle, NativeMethods.WM_SYSCOMMAND, NativeMethods.SC_MONITORPOWER, 2);  // 1=Standby, 2=Off
                break;
            case "logoff":  // Abmelden
                if (checkApps && AltTabList() > 0) { TaskList_Dialog(true); }
                else
                {
                    if (!NativeMethods.ExitWindowsEx(0, 0)) { Console.Beep(); }  // vorher Monitor abzuschalten funkt nicht
                }
                break;
            case "lock":
                filename = "Rundll32.exe";
                arguments = "User32.dll, LockWorkStation";
                break;
            case "firmware":
                if (checkApps && AltTabList() > 0) { TaskList_Dialog(true); }
                else
                {
                    filename = "shutdown.exe";
                    arguments = "-r -fw /t 6 /c \"Neustart zur Firmware-Benutzeroberfläche\"";
                }
                break;
            case "hibernate":
                if (!Application.SetSuspendState(PowerState.Hibernate, false, true)) { Console.Beep(); }
                break;
            case "sleep":  // Energie sparen
                //SplashForm sleepSplash = new("Bitte warten"); // using geht nur mit ShowDialog
                //_ = NativeMethods.ShowWindow(sleepSplash.Handle, NativeMethods.SW_SHOWNOACTIVATE); // ohne TopMost!
                //Thread.Sleep(1000);  // Give user a chance to release keys (in case their release would wake up the monitor again).
                //sleepSplash.Dispose();
                Utilities.SplashWait();  // default 1000 ms
                if (!NativeMethods.SetSuspendState(NativeMethods.PowerState.Suspend, false, true)) { Console.Beep(); }
                break;
        }
        if (!string.IsNullOrEmpty(filename))
        {
            //if (taskDelay > 0)
            //{
            //    SplashForm frmSplash = new("Bitte warten", taskDelay - 100); // using geht nur mit ShowDialog
            //    _ = NativeMethods.ShowWindow(frmSplash.Handle, NativeMethods.SW_SHOWNOACTIVATE); // ohne TopMost!
            //}
            var processInfo = new ProcessStartInfo { FileName = filename, Arguments = arguments, UseShellExecute = shellExecute };
            await Utilities.StartProcess(processInfo); // , taskDelay
            //Process.Start(new ProcessStartInfo { FileName = filename, Arguments = arguments, UseShellExecute = shellExecute });
        }
        //if (itemName is "shutdown" or "restart" or "firmware") { Application.Exit(); } // besser nicht - z.B. bei Abbruch aus irgendwelchen Gründen
    }

    private int AltTabList()
    {
        windowHandles.Clear();
        processes.Clear();
        _ = NativeMethods.EnumWindows(new NativeMethods.EnumWindowsCallback(MyCallback), 0);

        foreach (int i in windowHandles)
        {
            try
            {
                var indexHwnd = new IntPtr(i);
                if (NativeMethods.IsWindowVisible(indexHwnd) && indexHwnd != Handle)
                {
                    var ownerHwnd = indexHwnd;
                    do
                    {
                        ownerHwnd = NativeMethods.GetWindow(ownerHwnd, NativeMethods.GW_OWNER);
                    } while (!IntPtr.Zero.Equals(NativeMethods.GetWindow(ownerHwnd, NativeMethods.GW_OWNER)));
                    ownerHwnd = ownerHwnd != IntPtr.Zero ? ownerHwnd : indexHwnd;
                    if (NativeMethods.GetLastActivePopup(ownerHwnd) == indexHwnd)
                    {
                        var es = NativeMethods.GetWindowLongPtr(indexHwnd, NativeMethods.GWL_EXSTYLE);
                        if ((!(((es & NativeMethods.WS_EX_TOOLWINDOW) == NativeMethods.WS_EX_TOOLWINDOW) && ((es & NativeMethods.WS_EX_APPWINDOW) != NativeMethods.WS_EX_APPWINDOW))) && !IsInvisibleWin10BackgroundAppWindow(indexHwnd))
                        {
                            //var name = new StringBuilder(255);
                            //_ = NativeMethods.GetWindowText(i, name, 255);
                            try
                            {// 32-Bit-Prozesse
                                uint pid = 0;
                                _ = NativeMethods.GetWindowThreadProcessId(indexHwnd, out pid);
                                var p = Process.GetProcessById((int)pid);
                                //for (var x = processes.Count - 1; x > -1; x--) // iterating forward leads to error
                                //{
                                //    if (p.Id == processes[x].Id) { processes.Remove(processes[x]); }
                                //}
                                //processes.Add(p);
                                processes.Add(new Tuple<Process, IntPtr>(p, indexHwnd));
                            }
                            catch { } // 32-Bit-Prozess kann nicht auf 64-Bit Prozesse zugreifen
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); return 0; }
        }
        return processes.Count;
    }

    private bool MyCallback(int hWnd, int lParam)
    {
        windowHandles.Add(hWnd);
        return true;
    }

    private static bool IsInvisibleWin10BackgroundAppWindow(IntPtr hWindow)
    {
        int cloakedVal;
        var hr = NativeMethods.DwmGetWindowAttribute(hWindow, NativeMethods.DWMWA_CLOAKED, out cloakedVal, sizeof(int));
        if (hr != 0) { cloakedVal = 0; }  // returns S_OK (which is zero) on success. Otherwise, it returns an HRESULT error code
        return cloakedVal != 0;
    }

    private void Button_Click(object sender, EventArgs e)
    {
        var msg = @"Die im nachfolgenden Dialog getroffene Auswahl
wird derzeit bei einem Neustart nicht übernommen.
Um ein Sicherungsskript dauerhaft zu verwenden, 
müssen sie eine PowerShell-Datei mit dem Namen 
»TaskDown.ps1« ins Programmverzeichnis legen.";
        Utilities.ErrorMsgTaskDlg(Handle, msg, TaskDialogIcon.Information);
        openFileDialog.InitialDirectory = assLctn;
        var result = openFileDialog.ShowDialog(); // Show the dialog.
        if (result == DialogResult.OK) { textBox.Text = openFileDialog.FileName; }
    }

    private void ListView_MouseDoubleClick(object sender, MouseEventArgs e)
    {
        if (listView.SelectedItems != null)
        {
            var activeWindowHandle = Process.GetProcessById(Convert.ToInt32(listView.SelectedItems[0].Text)).MainWindowHandle;
            if (activeWindowHandle != IntPtr.Zero)
            {
                NativeMethods.SetForegroundWindow(activeWindowHandle);
                if (NativeMethods.IsIconic(activeWindowHandle)) { _ = NativeMethods.ShowWindow(activeWindowHandle, NativeMethods.SW_RESTORE); }
                else { _ = NativeMethods.ShowWindow(activeWindowHandle, NativeMethods.SW_SHOW); }
                Hide();
            }
        }
    }

    private void LinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        try
        {
            Process.Start(new ProcessStartInfo("taskschd.msc") { UseShellExecute = true });
        }
        catch (Exception ex) when (ex is Win32Exception or InvalidOperationException) { Utilities.ErrorMsgTaskDlg(Handle, ex.Message); }
    }

    private void CkbAutoStart_CheckedChanged(object sender, EventArgs e)
    {
        if (ckbAutoStart.Focused)
        {
            if (ckbAutoStart.Checked) { Utilities.SetAutoStart(appName, assLctn); }
            else { Utilities.UnSetAutoStart(appName); }
        }
    }

    private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
    {
        Utilities.HelpMsgTaskDlg(Handle, Icon);
    }

    private void NotifyMenuStrip_Opening(object sender, CancelEventArgs e)
    {
        showHideToolStripMenuItem.Text = Visible ? "Hide" : "Show";
    }

    private void FrmTaskDown_Deactivate(object sender, EventArgs e)
    {
        Hide();
    }

    private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        ShutDown_Dialog();
    }

    private void LinkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        TaskList_Dialog();
    }

    private void LinkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        Hide();
    }

    private async void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
    {
        if (isDoubleClicked) { return; }
        if (e.Button == MouseButtons.Left)
        {
            isDoubleClicked = false;
            await Task.Run(() => Utilities.SplashWait((int)NativeMethods.GetDoubleClickTime())); 
            if (isDoubleClicked) { return; }
            else if (!isTaskDialogOpen) { ShutDown_Dialog(); }
            else { NativeMethods.CloseTaskDialogIfOpen(); }
        }
    }

    private async void NotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
    {
        isDoubleClicked = true;
        if (Visible) { Hide(); }
        else if (!isTaskDialogOpen)
        {
            Show(); TopMost = true; // make our form jump to the top of everything
            BringToFront();
            Activate();
        }
        else { NativeMethods.CloseTaskDialogIfOpen(); }
        await Task.Delay((int)NativeMethods.GetDoubleClickTime());
        isDoubleClicked = false;
    }
}
