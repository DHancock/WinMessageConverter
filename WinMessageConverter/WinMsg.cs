using System;

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