
using System;

namespace BibleTaggingUtil.Settings
{
    partial class SettingsForm
    {
        private void InitializeOsisGenerationTab()
        {
            checkBoxUseDisambiguatedStrong.Checked = Properties.OsisFileGeneration.Default.UseDisambiguatedStrong;
            tbHebTags2Exclude.Text = Properties.OsisFileGeneration.Default.HebrewTagsToExclude;
            tbGrkTags2Exclude.Text = Properties.OsisFileGeneration.Default.GreekTagsToExclude;
        }

        private void checkBoxUseDisambiguatedStrong_CheckedChanged(object sender, EventArgs e)
        {
            Properties.OsisFileGeneration.Default.UseDisambiguatedStrong = checkBoxUseDisambiguatedStrong.Checked;
        }


    }
}