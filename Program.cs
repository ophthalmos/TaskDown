namespace TaskDown;

internal static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main()
    {
        using Mutex singleMutex = new(true, "{4A41FEF9-5916-419F-BE03-A72162E7A84C}", out var isNewInstance);
        if (isNewInstance)
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new FrmTaskDown());
        }
        else { NativeMethods.PostMessage(NativeMethods.HWND_BROADCAST, NativeMethods.WM_SHOWTASKDOWN, IntPtr.Zero, IntPtr.Zero); } // ShutDown_Dialog()
    }
}