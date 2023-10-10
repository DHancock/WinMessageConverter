using System.Diagnostics;

namespace Test_WinForms;

internal static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        ViewTraceListener traceListener = new();
        Trace.Listeners.Add(traceListener);

        ApplicationConfiguration.Initialize();
        Application.Run(new Form1());

        Trace.Listeners.Remove(traceListener);
    }
}
