# WinMessageConverter

 A .Net utility library that converts cryptic system window message codes to their string equivalents. Allows for simpler interpretation of debug trace output.

![screenshot](https://github.com/DHancock/WinMessageConverter/assets/28826959/8c10d0a4-62e0-4534-a3dc-ea67fb4097cd)

This code is also available as a [nuget package](https://www.nuget.org/packages/AssyntSoftware.WinMessageConverter)

 ## API

The library consists of a single static class [WinMsg](WinMessageConverter/WinMsg.cs) containing four functions.

````c#
namespace AssyntSoftware.WinMessageConverter;

public static class WinMsg
{
    public static bool TryGetMessage(uint msg, out string? text)
    {
        return LazyProvider.MessageDictionary.TryGetValue(msg, out text);
    }

    public static string Format(uint msg)
    {
        if (TryGetMessage(msg, out string? text))
            return text!;

        return $"0x{msg:X4}";
    }

    // parameter types suitable for CsWin32 window subclass functions (WinAppSdk)
    public static string Format(uint msg, nuint wParam, nint lParam)
    {
        return $"{Format(msg)} wParam: 0x{wParam:X4} lParam: 0x{lParam:X4}";
    }

    // parameter types suitable for WPF and WinForms applications
    public static string Format(int msg, IntPtr wParam, IntPtr lParam)
    {
        return Format((uint)msg, (nuint)(nint)wParam, lParam);
    }
}
````
<br/>

> [!NOTE]
> Several system message codes are not defined in WinUser.h so can't be converted.<br/>
> Commonly encountered codes are, but not limited to: 0x0060, 0x0093, 0x0094 
<br/>

## Typical usage for WinAppSdk projects
````c#
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
    }

    private LRESULT NewSubWindowProc(HWND hWnd, uint uMsg, WPARAM wParam, LPARAM lParam, nuint uIdSubclass, nuint dwRefData)
    {
        Debug.WriteLine(WinMsg.Format(uMsg, wParam, lParam));
        return PInvoke.DefSubclassProc(hWnd, uMsg, wParam, lParam);
    }
````
Further worked examples for WPF and WinForms are contained in the [TestSolution](TestSolution)
