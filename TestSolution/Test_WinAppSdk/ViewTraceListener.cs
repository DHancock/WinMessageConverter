using Microsoft.UI.Dispatching;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Controls;

using System;
using System.Diagnostics;
using System.Text;

namespace Test_WinAppSdk;

internal class ViewTraceListener : TraceListener
{
    private readonly object lockObject = new();
    private StringBuilder? store;
    private RichEditBox? consumer;

    public ViewTraceListener() : base(nameof(ViewTraceListener))
    {
    }

    public void RegisterConsumer(RichEditBox textBox)
    {
        lock (lockObject)
        {
            Debug.Assert(consumer is null && textBox.IsLoaded);

            consumer = textBox;

            if (store is not null)
            {
                if (store.Length > 0)
                    WriteInternal(store.ToString());

                store = null;
            }
        }
    }

    public override bool IsThreadSafe { get; } = true;

    private void WriteInternal(string message)
    {
        const int cMaxStoreLength = 1024 * 10;

        try
        {
            if (consumer is null)
            {
                store ??= new StringBuilder();

                store.Append(message);

                if (store.Length > cMaxStoreLength)
                    store.Remove(0, cMaxStoreLength / 2);
            }
            else
            {
                consumer?.DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Low, () =>
                {
                    consumer.Document.GetText(TextGetOptions.UseObjectText, out string existing);
                    consumer.Document.SetText(TextSetOptions.None, existing + message);
                });
            }
        }
        catch
        {
        }
    }

    public override void Write(string? message)
    {
        if (message is not null)
        {
            lock (lockObject)
            {
                WriteInternal(message);
            }
        }
    }

    public override void WriteLine(string? message) => Write(message + Environment.NewLine);
}
