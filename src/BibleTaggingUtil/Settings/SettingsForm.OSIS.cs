
using System;

namespace BibleTaggingUtil.Settings
{
    partial class SettingsForm
    {
        private void checkBoxUseDisambiguatedStrong_CheckedChanged(object sender, EventArgs e)
        {
            Properties.OsisFileGeneration.Default.UseDisambiguatedStrong = checkBoxUseDisambiguatedStrong.Checked;
        }

    }
}