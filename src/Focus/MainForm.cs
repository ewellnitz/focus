using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Focus
{
    public partial class MainForm : Form
    {
        private System.Windows.Forms.Timer _timer = new System.Windows.Forms.Timer();
        private Collection<ProgramDuration> _programDurations = new Collection<ProgramDuration>();
        private BindingSource _programDurationsBindingSource = new BindingSource();
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetForegroundWindow();
        public MainForm()
        {
            InitializeComponent();
            _timer.Tick += _timer_Tick;
            _timer.Interval = 10000;
            _programDurationsBindingSource.DataSource = _programDurations;
            dataGridView1.DataSource = _programDurationsBindingSource;
        }

        public string GetActiveProcess()
        {
            var activatedHandle = GetForegroundWindow();
            var activeProcess = Process.GetProcesses().FirstOrDefault(p => p.MainWindowHandle == activatedHandle);
            return activeProcess?.ProcessName ?? "";
        }

        private void _timer_Tick(object? sender, EventArgs e)
        {
            string activeProcess = GetActiveProcess();
            var programDuration = _programDurations.FirstOrDefault(p => p.ProgramName == activeProcess);
            if (programDuration == null)
            {
                programDuration = new ProgramDuration() { Duration = TimeSpan.Zero, ProgramName = activeProcess };
                _programDurations.Add(programDuration);
            }
            programDuration.Duration += TimeSpan.FromMilliseconds(_timer.Interval);
            _programDurationsBindingSource.ResetBindings(false);

            /*this.Cursor = new Cursor(Cursor.Current.Handle);
            var oldPos = Cursor.Position;
            Cursor.Position = new Point(Cursor.Position.X - 10, Cursor.Position.Y - 10);
            System.Threading.Thread.Sleep(10);
            Cursor.Position = oldPos;*/

            SendKeys.Send("a");
        }

        private void startStopButton_Click(object sender, EventArgs e)
        {
            if (_timer.Enabled)
            {
                _timer.Stop();
                startStopButton.Text = "&Start";
            }
            else
            {
                _timer.Start();
                startStopButton.Text = "&Stop";
            }
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            _programDurations.Clear();
            _programDurationsBindingSource.ResetBindings(false);
        }
    }
}