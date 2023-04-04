using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BibleTaggingUtil
{
    public partial class ProgressForm : Form
    {
        BibleTaggingForm container;

        System.Timers.Timer timer = null;
        public ProgressForm()
        {
            InitializeComponent();
        }

        public ProgressForm(BibleTaggingForm container)
        {
            InitializeComponent();
            this.container = container;
        }

        public string Label { set { UpdateLabel(value); } }

        public void UpdateLabel(string text)
        {
            if (InvokeRequired)
            {
                try
                {
                    // Call this same method but append THREAD2 to the text
                    Action safeWrite = delegate { UpdateLabel(text); };
                    Invoke(safeWrite);
                }
                catch (Exception ex) { }
            }
            else
            { 
                label.Text = text;
            }
        }

        public int Progress
        {
            set
            {
                if(value < 0)
                {
                    if(timer == null)
                    {
                        timer = new System.Timers.Timer();
                        timer.Enabled = false;
                        timer.AutoReset = true;
                        timer.Interval= 100;
                        timer.Elapsed += Timer_Elapsed;
                    }
                    timer.Elapsed -= Timer_Elapsed;
                    timer.Elapsed += Timer_Elapsed;
                    timedProgress = 0;
                    timer.Start();
                }
                else
                {
                    if (timer != null)
                    {
                        timer.Elapsed -= Timer_Elapsed;
                        timer.Stop();
                    }
                    UpdateProgressBar(value);
                }
            }
        }

        int timedProgress = 0;
        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            timedProgress += 5;
            if (timedProgress > 100)
                timedProgress= 0;
            UpdateProgressBar(timedProgress);
        }

        public void UpdateProgressBar(int progress)
        {
            if (InvokeRequired)
            {
                try
                {
                    // Call this same method but append THREAD2 to the text
                    Action safeWrite = delegate { UpdateProgressBar(progress); };
                    Invoke(safeWrite);
                }
                catch (Exception ex) { }
            }
            else
            {
                progressBar.Value = progress;
            }
        }


        public void Clear()
        {
            UpdateProgressBar(0);
            UpdateLabel(string.Empty);
        }

        private void ProgressForm_Load(object sender, EventArgs e)
        {
            //label.Location = new Point(
            //        this.Location.X + (this.Width / 2) - (label.Width / 2),
            //        this.Location.Y + (this.Height / 2) - (label.Height / 2));
        }
    }
}
