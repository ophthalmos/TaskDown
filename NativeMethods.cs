using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;

namespace TaskDown;
internal static class NativeMethods
{
    internal const int SW_RESTORE = 9;
    internal const int SW_SHOW = 5;

    internal const int DWMWA_CLOAKED = 14;
    internal const int GWL_EXSTYLE = (-20);
    internal const int GWL_STYLE = (-16);
    internal const int GW_OWNER = 4;
    internal const uint WS_MINIMIZE = 0x20000000;
    internal const uint WS_EX_TOOLWINDOW = 0x00000080;
    internal const uint WS_EX_APPWINDOW = 0x40000; // provides a taskbar button for a window that would otherwise lack one
    internal const int SW_SHOWNOACTIVATE = 4; // similar to SW_SHOWNORMAL, except that the window is not activated.

    //internal const uint EVENT_SYSTEM_FOREGROUND = 0x0003;
    //internal const uint WINEVENT_OUTOFCONTEXT = 0;
    //internal const int WINEVENT_SKIPOWNPROCESS = 2;
    //internal const int EVENT_SYSTEM_MINIMIZEEND = 0x0017;

    internal const int SC_CLOSE = 0xF060;
    internal const int WM_HOTKEY = 0x312;
    internal const int HOTKEY_ID0 = 0x0311;
    internal const int HOTKEY_ID1 = 0x0312;
    internal const int HOTKEY_ID2 = 0x0313;
    internal const int WM_SYSCOMMAND = 0x0112;
    internal const int HWND_BROADCAST = 0xffff;
    internal const int SC_MONITORPOWER = 0xF170;

    internal enum Modifiers : uint
    {
        Alt = 0x0001, Control = 0x0002, Shift = 0x0004, Win = 0x0008
    }

    internal enum PowerState
    {
        Suspend = 0, Hibernate = 1
    }

    internal static readonly uint WM_SHOWTASKDOWN = RegisterWindowMessage("WM_SHOWTASKDOWN");


    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern uint GetDoubleClickTime();

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern uint RegisterWindowMessage(string lpString);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    internal static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool ExitWindowsEx(uint uFlags, uint dwReason);

    [DllImport("powrprof.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool SetSuspendState(PowerState state, bool hibernate, bool force);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    [DllImport("user32.dll")]
    internal static extern int ShowWindow(IntPtr hWnd, uint Msg);

    [DllImport("user32.dll")]
    internal static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    internal static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    internal static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    //[DllImport("user32.dll", SetLastError = true)]
    //internal static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hModWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

    //[DllImport("user32.dll", SetLastError = true)]
    //internal static extern int UnhookWinEvent(IntPtr hWinEventHook);

    //internal delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

    //[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
    //private static extern int GetWindowTextLength(IntPtr hWnd);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool IsIconic(IntPtr hWnd);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

    [DllImport("user32.dll", SetLastError = true)]
    internal static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

    //private static bool ClassName_Shell(IntPtr hwnd)
    //{
    //    StringBuilder lpClassName = new(256);
    //    return GetClassName(hwnd, lpClassName, lpClassName.Capacity) != 0
    //        && string.CompareOrdinal(lpClassName.ToString(), "Shell_TrayWnd") == 0
    //        || string.CompareOrdinal(lpClassName.ToString(), "Progman") == 0
    //        || string.CompareOrdinal(lpClassName.ToString(), "WorkerW") == 0;
    //}

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    internal static void CloseTaskDialogIfOpen()
    {
        var hWnd1 = FindWindow("#32770", "TaskDown - Aktionen");
        var hWnd2 = FindWindow("#32770", "TaskDown - Geöffnete Apps");
        if (hWnd1 != IntPtr.Zero) { SendMessage(hWnd1, WM_SYSCOMMAND, SC_CLOSE, 0); }
        else if (hWnd2 != IntPtr.Zero) { SendMessage(hWnd2, WM_SYSCOMMAND, SC_CLOSE, 0); }
    }

    internal static string WinGetTitle(IntPtr hwnd)
    {
        StringBuilder lpString = new(256);
        return GetWindowText(hwnd, lpString, lpString.Capacity) > 0 ? lpString.ToString() : string.Empty;
    }
    internal static string WinGetClass(IntPtr hwnd)
    {
        StringBuilder lpClassName = new(256);
        return GetClassName(hwnd, lpClassName, lpClassName.Capacity) > 0 ? lpClassName.ToString() : string.Empty;
    }

    //private static Process GetProcessFromHandle(IntPtr hWnd)
    //{
    //    uint processId;
    //    _ = GetWindowThreadProcessId(hWnd, out processId);
    //    return Process.GetProcessById((int)processId);
    //}

    //internal static void WinEventProc(IntPtr _1, uint eventType, IntPtr hwnd, int idObject, int idChild, uint _2, uint _3)
    //{ //https://devblogs.microsoft.com/oldnewthing/20130930-00/?p=3083 // if (hwnd && idObject == OBJID_WINDOW && idChild == CHILDID_SELF && event == EVENT_SYSTEM_FOREGROUND)
    //    List<IntPtr> formHandles = [];
    //    formHandles.AddRange(from Form form in Application.OpenForms select form.Handle);
    //    if (hwnd != IntPtr.Zero && idObject == 0x00000000 && idChild == 0 && eventType == EVENT_SYSTEM_FOREGROUND &&
    //        !ClassName_Shell(hwnd) && !formHandles.Contains(hwnd))
    //    {
    //        var process = GetProcessFromHandle(hwnd);
    //        //if (process.ProcessName == "devenv") { return; }
    //        if (process != null)
    //        {
    //            for (var x = processes.Count - 1; x > -1; x--) // iterating forward leads to error
    //            {
    //                if (process.Id == processes[x].Id) { processes.Remove(processes[x]); }
    //            }
    //            processes.Insert(0, process);
    //        }
    //    } //Restore from Minimize wird problemlos erfasst
    //}

    //internal static void TidyUpProcessList()
    //{
    //    for (var x = processes.Count - 1; x > -1; x--) //  iterating backwards. Alternatively: foreach (var pid in items.ToList())
    //    {
    //        if (processes[x].HasExited || !IsWindow(processes[x].MainWindowHandle) || string.IsNullOrEmpty(WinGetClass(processes[x].MainWindowHandle))) { processes.Remove(processes[x]); }
    //    }
    //}

    public delegate bool EnumWindowsCallback(int hWnd, int lParam);


    [DllImport("user32.dll")]
    public static extern int EnumWindows(EnumWindowsCallback lpEnumFunc, int lParam);

    [DllImport("user32.dll")]
    public static extern IntPtr GetLastActivePopup(IntPtr hWnd);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
    public static extern IntPtr GetWindow(IntPtr hWnd, int flags);

    [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
    public static extern int GetWindowLongPtr32(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
    public static extern int GetWindowLongPtr64(IntPtr hWnd, int nIndex);

    public static int GetWindowLongPtr(IntPtr hWnd, int nIndex)
    {// This static method is required because Win32 does not support GetWindowLongPtr directly
        return IntPtr.Size == 8 ? GetWindowLongPtr64(hWnd, nIndex) : GetWindowLongPtr32(hWnd, nIndex);
    }

    [DllImport("DwmApi.dll")]
    public static extern int DwmGetWindowAttribute(IntPtr hwnd, int dwAttributeToGet, out int pvAttributeValue, int cbAttribute);

}
