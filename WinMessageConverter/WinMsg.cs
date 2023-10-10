using System;

namespace AssyntSoftware.WinMessageConverter;

public static class WinMsg
{
    public static bool TryGetMessage(uint msg, out string? output)
    {
        return LazyProvider.MessageDictionary.TryGetValue(msg, out output);
    }

    public static string Format(uint msg)
    {
        if (TryGetMessage(msg, out string? output))
            return output!;

        return $"0x{msg:X4}";
    }

    public static string Format(uint msg, nuint wParam, nint lParam)
    {
        return $"{Format(msg)} wParam: 0x{wParam:X4} lParam: 0x{lParam:X4}";
    }

    public static string Format(int msg, IntPtr wParam, IntPtr lParam)
    {
        return Format((uint)msg, (nuint)(nint)wParam, lParam);
    }
}