using AssyntSoftware.WinMessageConverter;

using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;

namespace Test_WPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        Loaded += (s, e) =>
        {
            foreach (TraceListener listener in Trace.Listeners)
            {
                if (listener is ViewTraceListener viewTraceListener)
                {
                    viewTraceListener.RegisterConsumer(TraceTextBox);
                    return;
                }
            }

            TraceTextBox.Text = "Failed to find trace listener.";
        };

        ComponentDispatcher.ThreadFilterMessage += ProcessMessage;
    }

    private void ProcessMessage(ref MSG msg, ref bool handled)
    {
        // avoids excessive amounts of spam...
        const int WM_USER = 0x0400;
        const int WM_TIMER = 0x0113;
        const int WM_MOUSEMOVE = 0x0200;
        const int WM_NCMOUSEMOV = 0x00A0;
        const int Unknown = 0x0060; // seems to be related to autoscrolling the text in a TextBox

        switch (msg.message)
        {
            case > WM_USER: break;
            case WM_TIMER: break;
            case WM_MOUSEMOVE: break;
            case WM_NCMOUSEMOV: break;
            case Unknown: break;
            default:
            {
                Trace.WriteLine(WinMsg.Format(msg.message, msg.wParam, msg.lParam));
                break;
            }
        }
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        TraceTextBox.Text = string.Empty;
    }
}
