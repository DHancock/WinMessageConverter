using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Test_WPF;

internal partial class ViewTraceListener : TraceListener
{
    private readonly Lock lockObject;
    private readonly StringBuilder store;
    private TextBox? consumer;
    private readonly DispatcherTimer dispatcherTimer;

    public ViewTraceListener() : base(nameof(ViewTraceListener))
    {
        lockObject = new Lock();

        dispatcherTimer = new DispatcherTimer();
        dispatcherTimer.Interval = TimeSpan.FromMilliseconds(100);
        dispatcherTimer.Tick += DispatcherTimer_Tick;

        store = new StringBuilder(1000);
    }

    private void DispatcherTimer_Tick(object? sender, object e)
    {
        lock (lockObject)
        {
            if (consumer is not null)
            {
                int start = consumer.SelectionStart;
                int length = consumer.SelectionLength;

                consumer.AppendText(store.ToString());
                consumer.ScrollToEnd();

                consumer.SelectionStart = start;
                consumer.SelectionLength = length;

                store.Clear();
                dispatcherTimer.Stop();
            }
        }
    }

    public void RegisterConsumer(TextBox textBox)
    {
        lock (lockObject)
        {
            Debug.Assert(consumer is null);
            Debug.Assert(textBox.IsLoaded);

            consumer = textBox;
        }
    }

    public override bool IsThreadSafe => true;

    public override void Write(string? message)
    {
        lock (lockObject)
        {
            try
            {
                store.Append(message);

                if (!dispatcherTimer.IsEnabled && (consumer is not null))
                {
                    dispatcherTimer.Start();
                }
            }
            catch
            {
            }
        }
    }

    public override void WriteLine(string? message) => Write(message + Environment.NewLine);
}
