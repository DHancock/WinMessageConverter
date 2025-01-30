using AssyntSoftware.WinMessageConverter;

using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Shell;
using Windows.Win32.UI.WindowsAndMessaging;

using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Test_WinAppSdk;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
    private readonly SUBCLASSPROC subClassDelegate;
    private readonly HWND hWnd;

    public MainWindow()
    {
        this.InitializeComponent();

        hWnd = (HWND)WindowNative.GetWindowHandle(this);
        subClassDelegate = new SUBCLASSPROC(NewSubWindowProc);

        if (!PInvoke.SetWindowSubclass(hWnd, subClassDelegate, 0, 0))
            throw new Win32Exception(Marshal.GetLastPInvokeError());

        RegisterConsumer();
    }

    private void RegisterConsumer()
    {
        TraceCounsumer.Loaded += (s, e) =>
        {
            foreach (TraceListener listener in Trace.Listeners)
            {
                if (listener is ViewTraceListener viewTraceListener)
                {
                    viewTraceListener.RegisterConsumer(TraceCounsumer);
                    return;
                }
            }

            TraceCounsumer.Text = "failed to find trace listener";
        };
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        TraceCounsumer.Text = string.Empty;
    }

    private LRESULT NewSubWindowProc(HWND hWnd, uint uMsg, WPARAM wParam, LPARAM lParam, nuint uIdSubclass, nuint dwRefData)
    {
        Trace.WriteLine(WinMsg.Format(uMsg, wParam, lParam));
        
        if (uMsg == PInvoke.WM_GETMINMAXINFO)
        {
            const double MinWidth = 660;
            const double MinHeight = 500;

            MINMAXINFO minMaxInfo = Marshal.PtrToStructure<MINMAXINFO>(lParam);
            double scaleFactor = GetScaleFactor();
            minMaxInfo.ptMinTrackSize.X = Math.Max(ConvertToDeviceSize(MinWidth, scaleFactor), minMaxInfo.ptMinTrackSize.X);
            minMaxInfo.ptMinTrackSize.Y = Math.Max(ConvertToDeviceSize(MinHeight, scaleFactor), minMaxInfo.ptMinTrackSize.Y);
            Marshal.StructureToPtr(minMaxInfo, lParam, true);
        }

        return PInvoke.DefSubclassProc(hWnd, uMsg, wParam, lParam);
    }

    private static int ConvertToDeviceSize(double value, double scalefactor) => Convert.ToInt32(Math.Clamp(value * scalefactor, 0, short.MaxValue));

    private double GetScaleFactor()
    {
        if ((Content is not null) && (Content.XamlRoot is not null))
            return Content.XamlRoot.RasterizationScale;

        double dpi = PInvoke.GetDpiForWindow(hWnd);
        return dpi / 96.0;
    }
}
