using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;

namespace TaskDown;
internal static class Utilities
{
    internal static string Truncate(string text, int maxLength)
    {
        if (string.IsNullOrEmpty(text) || text.Length <= maxLength) { return text; }
        else { return text[..(maxLength - 1)] + "…"; }
    }

    internal static void HelpMsgTaskDlg(IntPtr hwnd, Icon? icon)
    {
        var foot = "              © " + GetBuildDate().ToString("yyyy") + " Wilhelm Happe, Version " + Assembly.GetExecutingAssembly().GetName().Version?.ToString() + " (" + GetBuildDate().ToString("d") + ")";
        var msg = "Das Programm stellt Tastenkombinationen zur" + Environment.NewLine +
            "Verfügung, mit denen sich Apps und Windows" + Environment.NewLine +
            "fix beenden lassen." + Environment.NewLine +
            "Der Name TaskDown spielt unter anderem auf" + Environment.NewLine +
            "die intensive Verwendung des TaskDialogs an.";
        TaskDialog.ShowDialog(hwnd, new TaskDialogPage() { Caption = "Über " + Application.ProductName, Heading = "TaskDown", Text = msg, Icon = icon == null ? null : new TaskDialogIcon(icon), AllowCancel = true, Buttons = { TaskDialogButton.OK }, Footnote = foot });
    }

    internal static void ErrorMsgTaskDlg(IntPtr hwnd, string message, TaskDialogIcon? taskDialogIcon = null)
    {
        taskDialogIcon ??= TaskDialogIcon.Error;
        TaskDialog.ShowDialog(hwnd, new TaskDialogPage() { Caption = Application.ProductName, SizeToContent = true, Text = message, Icon = taskDialogIcon, AllowCancel = true, Buttons = { TaskDialogButton.OK } });
    }

    internal static DateTime GetBuildDate()
    { //s. <SourceRevisionId>build$([System.DateTime]::UtcNow.ToString("yyyyMMddHHmmss"))</SourceRevisionId> in ClipMenu.csproj
        const string BuildVersionMetadataPrefix = "+build";
        var attribute = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>();
        if (attribute?.InformationalVersion != null)
        {
            var value = attribute.InformationalVersion;
            var index = value.IndexOf(BuildVersionMetadataPrefix);
            if (index > 0)
            {
                value = value[(index + BuildVersionMetadataPrefix.Length)..];
                if (DateTime.TryParseExact(value, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result)) { return result; }
            }
        }
        return default;
    }

    internal static bool IsAutoStartEnabled(string taskName)
    {
        ProcessStartInfo start = new()
        {
            FileName = "schtasks.exe", // Specify exe name.
            UseShellExecute = false,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden,
            Arguments = "/query /TN \"" + taskName + "\"",
            RedirectStandardOutput = true
        };
        using var process = Process.Start(start);
        if (process == null) { return false; }
        using var reader = process.StandardOutput;
        var stdout = reader.ReadToEnd();
        if (stdout.Contains(taskName)) { return true; }
        else { return false; }
    }

    internal static void SetAutoStart(string appName, string assemblyLocation)
    {
        new Process()
        {
            StartInfo = {
                  UseShellExecute = false,
                  FileName = "SCHTASKS.exe",
                  RedirectStandardError = true,
                  RedirectStandardOutput = true,
                  CreateNoWindow = true,
                  WindowStyle = ProcessWindowStyle.Hidden, // /delay <delaytime> mmmm:ss format - cave: problem initializing saveScriptPath path
                  Arguments = string.Format(@"/Create /F /RL HIGHEST /SC ONLOGON /DELAY 0000:30 /TN " + appName + " /TR \"'" + assemblyLocation + "'\"")
               }
        }.Start();
    }

    internal static void UnSetAutoStart(string taskName)
    {
        new Process()
        {
            StartInfo = {
                  UseShellExecute = false,
                  FileName = "SCHTASKS.exe",
                  RedirectStandardError = true,
                  RedirectStandardOutput = true,
                  CreateNoWindow = true,
                  WindowStyle = ProcessWindowStyle.Hidden,
                  Arguments = string.Format(@"/Delete /F /TN " + taskName)
               }
        }.Start();
    }


    public static async Task StartProcess(ProcessStartInfo processStartInfo)  // int delay = 0
    {
        //await Task.Delay(delay);
        var p = Process.Start(processStartInfo);
        if (p == null) { return; }
        await p.WaitForExitAsync().ConfigureAwait(false);
    }

    public static void SplashWait(int waitingTime = 1000)
    {
        var img = Properties.Resources.SplashText;
        using var window = new Form
        {
            FormBorderStyle = FormBorderStyle.None,
            Size = img.Size,
            BackgroundImage = img,
            BackgroundImageLayout = ImageLayout.None,
            StartPosition = FormStartPosition.CenterScreen,
            ShowInTaskbar = false
        };
        window.Load += (sender, e) => window.Size = img.Size;  // Workaround: verstehe nicht, warum die Höhe ungewollt geändert wird (22 => 39)
        window.Show();
        Thread.Sleep(waitingTime);
        window.Close();
    }
}
