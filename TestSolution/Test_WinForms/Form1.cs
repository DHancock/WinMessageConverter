using AssyntSoftware.WinMessageConverter;

using System.Diagnostics;

namespace Test_WinForms;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        foreach (TraceListener listener in Trace.Listeners)
        {
            if (listener is ViewTraceListener viewTraceListener)
            {
                viewTraceListener.RegisterConsumer(textBox);
                return;
            }
        }

        textBox.Text = "failed to find trace listener";
    }

    protected override void WndProc(ref Message m)
    {
        // avoids excessive amounts of spam...
        const int WM_NCHITTEST = 0x0084;
        const int WM_CTLCOLORSTATIC = 0x0138;
        const int WM_MOUSEMOVE = 0x0200;
        const int WM_NCMOUSEMOV = 0x00A0;
        const int WM_SETCURSOR = 0x0020;

        if ((m.Msg != WM_NCHITTEST) && (m.Msg != WM_CTLCOLORSTATIC) && (m.Msg != WM_MOUSEMOVE) && (m.Msg != WM_NCMOUSEMOV) && (m.Msg != WM_SETCURSOR))
            Trace.WriteLine(WinMsg.Format(m.Msg, m.WParam, m.LParam));

        base.WndProc(ref m);
    }

    private void ClearButton_Click(object sender, EventArgs e)
    {
        textBox.Text = string.Empty;
    }
}