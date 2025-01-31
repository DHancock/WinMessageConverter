using System.Diagnostics;
using System.Text;

namespace Test_WinForms;

internal partial class ViewTraceListener : TraceListener
{
    private readonly Lock lockObject;
    private readonly StringBuilder store;
    private TextBox? consumer;
    private readonly System.Windows.Forms.Timer timer;

    public ViewTraceListener() : base(nameof(ViewTraceListener))
    {
        lockObject = new Lock();

        timer = new System.Windows.Forms.Timer();
        timer.Interval = 100;
        timer.Tick += Timer_Tick;

        store = new StringBuilder(1000);
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        lock (lockObject)
        {
            if (consumer is not null)
            {
                int start = consumer.SelectionStart;
                int length = consumer.SelectionLength;

                consumer.AppendText(store.ToString());

                consumer.SelectionStart = start;
                consumer.SelectionLength = length;

                store.Clear();
                timer.Stop();
            }
        }
    }

    public void RegisterConsumer(TextBox textBox)
    {
        lock (lockObject)
        {
            Debug.Assert(consumer is null);
            Debug.Assert(textBox.IsHandleCreated);

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

                if (!timer.Enabled && (consumer is not null))
                {
                    timer.Start();
                }
            }
            catch
            {
            }
        }
    }

    public override void WriteLine(string? message) => Write(message + Environment.NewLine);
}
