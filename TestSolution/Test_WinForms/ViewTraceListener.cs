using System.Diagnostics;
using System.Text;

namespace Test_WinForms;

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
            Debug.Assert(consumer is null && textBox.Created);

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
                consumer.BeginInvoke(() =>
                {
                    consumer.Text += message;
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
