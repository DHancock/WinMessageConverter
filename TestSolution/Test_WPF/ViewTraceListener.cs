using System;
using System.Diagnostics;
using System.Text;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Test_WPF;

internal class ViewTraceListener : TraceListener
{
    private readonly object lockObject = new();
    private StringBuilder? store;
    private TextBox? consumer;

    public ViewTraceListener() : base(nameof(ViewTraceListener))
    {
    }

    public void RegisterConsumer(TextBox textBox)
    {
        lock (lockObject)
        {
            Debug.Assert(consumer is null && textBox.IsInitialized);

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
                consumer.Dispatcher.BeginInvoke(() =>
                {
                    consumer.Text += message;
                },
                DispatcherPriority.Background);
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
