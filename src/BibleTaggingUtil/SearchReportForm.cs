using BibleTaggingUtil.BibleVersions;
using BibleTaggingUtil.Editor;
using BibleTaggingUtil.Strongs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BibleTaggingUtil
{
    internal partial class SearchReportForm : Form
    {
        Dictionary<string, VerseWord> topMap = new Dictionary<string, VerseWord>();
        Dictionary<string, VerseWord> tahotMap = new Dictionary<string, VerseWord>();
        Dictionary<string, VerseWord> tagntMap = new Dictionary<string, VerseWord>();
        Dictionary<string, VerseWord> targetMap = new Dictionary<string, VerseWord>();
        
        ReferenceTopVersion top;
        ReferenceVersionTAHOT tahot;
        ReferenceVersionTAGNT tagnt;
        TargetVersion target;
        public SearchReportForm(TargetVersion target, ReferenceVersionTAHOT tahot, ReferenceVersionTAGNT tagnt, ReferenceTopVersion top)
        {
            InitializeComponent();

            this.top = top;
            this.tahot = tahot;
            this.tagnt = tagnt;
            this.target = target;
        }

        private void SearchReportForm_Load(object sender, EventArgs e)
        {
            checkBoxTop.Text = top.BibleName;
            checkBoxTAHOT.Text = tahot.BibleName;
            checkBoxTAGNT.Text = tagnt.BibleName;
            checkBoxTarget.Text = target.BibleName;
        }

        private void btnTTFolder_Click(object sender, EventArgs e)
        {
            DialogResult result = saveFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                tbOtputPath.Text = saveFileDialog1.FileName;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (checkBoxTop.Checked)
                Search(top, topMap);

            if (checkBoxTAHOT.Checked)
                Search(tahot, tahotMap);
            if (checkBoxTAGNT.Checked)
                Search(tagnt, tagntMap);
            if (checkBoxTop.Checked)
                Search(top, topMap);

            if(!string.IsNullOrEmpty(tbOtputPath.Text))
            {
                using(StreamWriter sw = new StreamWriter(tbOtputPath.Text))
                {
                    foreach((string reference, VerseWord word) in tahotMap)
                    {
                        sw.WriteLine(string.Format("{0}\t{1}", reference, word.WordNumber));
                    }
                }
            }
        }

        private void Search(BibleVersion bibleVersion, Dictionary<string, VerseWord> outMap)
        {
            string[] tags = new string[] { "H5921A", "H3651C" };
            foreach ((string reference, Verse verse) in bibleVersion.Bible) 
            { 
                for(int i = 0; i< verse.Count-1; i++) 
                {
                    VerseWord word1 = verse[i];
                    VerseWord word2 = verse[i+1];
                    if (word1.ToString().Contains(tags[0]) && word2.ToString().Contains(tags[1]))
                    {
                        outMap[reference] = word1;
                        break;
                    }
                }
            }
        }
    }
}
