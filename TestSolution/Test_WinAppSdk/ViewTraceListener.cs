using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

using System;
using System.Diagnostics;
using System.Text;

namespace Test_WinAppSdk;

internal partial class ViewTraceListener : TraceListener
{
    private readonly object lockObject = new();
    private readonly StringBuilder store;
    private TextBox? consumer;
    private ScrollViewer? scrollViewer;
    private bool viewUpdateRequired;
    private readonly DispatcherTimer dispatcherTimer;

    public ViewTraceListener() : base(nameof(ViewTraceListener))
    {
        dispatcherTimer = new();
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
                if (store.Length > 0)
                {
                    int start = consumer.SelectionStart;
                    int length = consumer.SelectionLength;

                    consumer.Text += store.ToString();

                    consumer.SelectionStart = start;
                    consumer.SelectionLength = length;

                    store.Clear();
                    viewUpdateRequired = true;
                }

                if (viewUpdateRequired && (scrollViewer is not null))
                {
                    viewUpdateRequired = !scrollViewer.ChangeView(0.0, scrollViewer.ExtentHeight, 1.0f);

                    if (!viewUpdateRequired)
                    {
                        dispatcherTimer.Stop();
                    }
                }
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

            scrollViewer = FindChild<ScrollViewer>(consumer);
        }
    }

    private static T? FindChild<T>(DependencyObject parent) where T : FrameworkElement
    {
        int count = VisualTreeHelper.GetChildrenCount(parent);

        for (int index = 0; index < count; index++)
        {
            DependencyObject child = VisualTreeHelper.GetChild(parent, index);

            if (child is T target)
            {
                return target;
            }

            T? result = FindChild<T>(child);

            if (result is not null)
            {
                return result;
            }
        }

        return null;
    }

    public override bool IsThreadSafe { get; } = true;

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
