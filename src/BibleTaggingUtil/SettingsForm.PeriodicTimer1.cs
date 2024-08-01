using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibleTaggingUtil
{
    public partial class SettingsForm
    {

        public bool PeriodicSaveEnabled
        {
            get { return cbPeriodicSave.Checked; }
            set { cbPeriodicSave.Checked = value; }
        }

        public int SavePeriod
        {
            get
            {
                return (int)nudSavePeriod.Value;
            }
            set
            {
                nudSavePeriod.Value = value;
            }
        }

        private void cbPeriodicSave_CheckedChanged(object sender, EventArgs e)
        {
            nudSavePeriod.Enabled = cbPeriodicSave.Checked;
        }

    }
}
