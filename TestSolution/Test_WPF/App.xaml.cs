using System.Diagnostics;
using System.Windows;

namespace Test_WPF;

public partial class App : Application
{
    private readonly ViewTraceListener traceListener = new();

    protected override void OnStartup(StartupEventArgs e)
    {
        Trace.Listeners.Add(traceListener);
        base.OnStartup(e);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);
        Trace.Listeners.Remove(traceListener);
    }
}