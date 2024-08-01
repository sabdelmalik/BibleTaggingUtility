using BibleTaggingUtil.BibleVersions;
using BibleTaggingUtil.Strongs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BibleTaggingUtil.Editor
{
    internal class TOHTHGridView : DataGridView
    {
        public ReferenceVersionTAGNT BibleNT { get; set; }
        public ReferenceVersionTAHOT BibleOT { get; set; }
        public string SearchTag { get; internal set; }

        protected override void OnCellEnter(DataGridViewCellEventArgs e)
        {
            // Ignore this event
            //base.OnCellEnter(e);
        }
        protected override void OnCellMouseDown(DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Left &&
                                e.Clicks == 1 &&
                                e.RowIndex >= this.Rows.Count - 2 &&
                                e.ColumnIndex > 0 &&
                                this.SelectedCells.Count > 0)
                {
                    StrongsCluster tag = new StrongsCluster();
                    int rowIndex = SelectedCells[0].RowIndex;

                    if (this.SelectedCells.Count > 1)
                    {
                        bool sameRow = true;
                        foreach (DataGridViewCell cell in this.SelectedCells)
                        {

                            if ((cell.RowIndex != rowIndex))
                            {
                                sameRow = false;
                                break;
                            }
                        }

                        if (sameRow)
                        {
                            //bool mergeOk = true; // we only drag adjacent cells
                            int count = this.SelectedCells.Count;
                            int colIndex = SelectedCells[count - 1].ColumnIndex;
                            tag = (StrongsCluster)this[colIndex, rowIndex].Value;
                            for (int i = count - 2; i >= 0; i--)
                            {
                                tag += (StrongsCluster)this[SelectedCells[i].ColumnIndex, rowIndex].Value;
                                //if (Math.Abs(SelectedCells[i].ColumnIndex - colIndex) != 1)
                                //{
                                //    //mergeOk = false;
                                //    tag = new StrongsCluster(new string[] { "" });
                                //    break;
                                //}
                                colIndex = SelectedCells[i].ColumnIndex;
                            }
                        }


                    }
                    else
                    {
                        tag = this.Rows[rowIndex].Cells[e.ColumnIndex].Value as StrongsCluster;
                    }
                    DragData data = new DragData(1, e.ColumnIndex, tag, this);
                    this.DoDragDrop(data, DragDropEffects.Copy);
                }
            }
            catch (Exception ex)
            {
                var cm = System.Reflection.MethodBase.GetCurrentMethod();
                var name = cm.DeclaringType.FullName + "." + cm.Name;
                Tracing.TraceException(name, ex.Message);
            }

            base.OnCellMouseDown(e);
        }

        public void Update(Verse verseWords, BibleTestament testament)
        {
            if (testament == BibleTestament.OT)
                UpdateOT(verseWords);
            else
                UpdateNT(verseWords);
        }
        private void UpdateOT(Verse verseWords)
        {
            this.Rows.Clear();
            if (verseWords == null)
                return;

            List<string> words = new List<string>();
            List<string> hebrew = new List<string>();
            List<string> transliteration = new List<string>();
            List<StrongsCluster> tags = new List<StrongsCluster>();
            List<string> morphology = new List<string>();
            List<string> rootStrongs = new List<string>();
            List<string> wordType =   new List<string>();
            List<string> altVerseNumber = new List<string>();
            List<string> wordNumber = new List<string>();
            List<string> meaningVar = new List<string>();
            StrongsCluster tagLable = new StrongsCluster("TAG");
            try
            {
                words.Add("ENG");
                hebrew.Add("HEB");
                altVerseNumber.Add("ALT");
                wordNumber.Add("W #");
                wordType.Add("TYP");
                morphology.Add("GMR");
                meaningVar.Add("VAR");
                transliteration.Add("XLT");
                rootStrongs.Add("STG");
                tags.Add(tagLable);

                for (int i = 0; i < verseWords.Count; i++)
                {
                    VerseWord verseWord = verseWords[i];
                    words.Add(verseWord.Word);
                    hebrew.Add(verseWord.Hebrew);
                    morphology.Add(verseWord.Morphology);
                    transliteration.Add(verseWord.Transliteration);
                    rootStrongs.Add(verseWord.RootStrong);
                    wordType.Add(verseWord.WordType);
                    altVerseNumber.Add(verseWord.AltVerseNumber);
                    wordNumber.Add(verseWord.WordNumber);
                    meaningVar.Add(verseWord.MeaningVar);
                    tags.Add(verseWord.Strong);
/*
                    if (verseWord.Strong.Count > 0)
                    {
                        string s = String.Empty;
                        bool E = (verseWord.Hebrew.Trim() == "אֱלֹהִים");
                        bool Y = (verseWord.Hebrew.Trim() == "יהוה");
                        bool strongIsE = (verseWord.Strong[0].Number == 430);
                        bool strongIsY = ((verseWord.Strong[0].Number == 3068) || (verseWord.Strong[0].Number == 3069));

                        if (E || Y)
                        {
                            // special treatment for אֱלֹהִים & יהוה
                            if ((E && strongIsE) || (Y && strongIsY))
                                //s = "<" + verseWord.Strong[0] + ">";
                                s= verseWord.Strong[0].ToStringEx();
                        }
                        else
                        {
                            //s = "<" + verseWord.Strong[0] + ">";
                            s = verseWord.Strong[0].ToStringEx();
                        }

                        if (verseWord.Strong.Count > 1)
                        {
                            for (int j = 1; j < verseWord.Strong.Count; j++)
                            {
                                strongIsE = (verseWord.Strong[j].Number == 430);
                                strongIsY = ((verseWord.Strong[j].Number == 3068) || (verseWord.Strong[j].Number == 3069));
                                if (E || Y)
                                {
                                    // special treatment for אֱלֹהִים & יהוה
                                    if ((E && strongIsE) || (Y && strongIsY))
                                    {
                                        if (!string.IsNullOrEmpty(s))
                                            s += " ";
                                        //s += "<" + verseWord.Strong[j] + ">";
                                        s = verseWord.Strong[j].ToStringEx();
                                    }
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(s))
                                        s += " ";
                                    //s += "<" + verseWord.Strong[j] + ">";
                                    s = verseWord.Strong[j].ToStringEx();
                                }
                            }
                        }
                        tags.Add(s.Trim());
                    }
                    else
                        tags.Add("")
*/;
                }

                //this.ColumnCount = verseWords.Count;
                this.ColumnCount = words.Count;

                this.Rows.Add(words.ToArray());
                this.Rows.Add(hebrew.ToArray());
                this.Rows.Add(altVerseNumber.ToArray());
                this.Rows.Add(wordNumber.ToArray());
                this.Rows.Add(wordType.ToArray());
                this.Rows.Add(morphology.ToArray());
                this.Rows.Add(meaningVar.ToArray());
                this.Rows.Add(transliteration.ToArray());
                this.Rows.Add(rootStrongs.ToArray());
                this.Rows.Add(tags.ToArray());

                for (int i = 0; i < words.Count; i++)
                {
                    string word = (string)this.Rows[1].Cells[i].Value;
                    StrongsCluster tag = (StrongsCluster)this.Rows[this.RowCount-1].Cells[i].Value;
                    if (word.Contains("יהוה") || tag.ToString().Contains("3069") || tag.ToString().Contains("3068"))
                    {
                        this.Rows[1].Cells[i].Style.ForeColor = Color.Red;
                        this.Rows[this.RowCount - 1].Cells[i].Style.ForeColor = Color.Red;
                    }
                    else
                    {
                        this.Rows[1].Cells[i].Style.ForeColor = Color.Black;
                        this.Rows[this.RowCount - 1].Cells[i].Style.ForeColor = Color.Black;
                    }
                    if (SearchTag!=null && tag.ToString().Contains(SearchTag))
                    {
                        this.Rows[this.RowCount - 1].Cells[i].Style.ForeColor = Color.Maroon;
                        if(RowCount > 2)
                            this.Rows[this.RowCount - 2].Cells[i].Style.BackColor = Color.Yellow;
                    }

                }
            }
            catch (Exception ex)
            {
                var cm = System.Reflection.MethodBase.GetCurrentMethod();
                var name = cm.DeclaringType.FullName + "." + cm.Name;
                Tracing.TraceException(name, ex.Message);
            }

            this.ClearSelection();

            this.Rows[0].ReadOnly = true;
            this.Rows[1].ReadOnly = true;

        }

        protected override void OnCellFormatting(DataGridViewCellFormattingEventArgs e)
        {
            if (this[0,e.RowIndex].Value.ToString() == "GMR")
            {
                for (int i = 1; i < this.ColumnCount; i++)
                {
                    DataGridViewCell cell = this.Rows[e.RowIndex].Cells[i];
                    cell.ToolTipText = GetMorphologyDetails(cell.Value.ToString());
                }
            }
            //base.OnCellFormatting(e);
        }

        private Morphology.NT morfNT = new Morphology.NT();
        private string GetMorphologyDetails(string morf)
        {
            return morfNT.GetMorphologyDetails(morf);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="verseWords"></param>
        private void UpdateNT(Verse verseWords)
        {
            this.Rows.Clear();
            if (verseWords == null)
                return;

            List<string> words = new List<string>();
            List<string> greek = new List<string>();
            List<string> dictForm = new List<string>();
            List<string> dictGloss = new List<string>();
            List<string> transliteration = new List<string>();
            List<StrongsCluster> tags = new List<StrongsCluster>();
            List<string> morphology = new List<string>();
            List<string> rootStrongs = new List<string>();
            List<string> wordType = new List<string>();
            List<string> altVerseNumber = new List<string>();
            List<string> varUsed = new List<string>();
            List<string> wordNumber = new List<string>();
            StrongsCluster tagLable = new StrongsCluster("TAG");

            try
            {
                words.Add("ENG");
                greek.Add("GRK");
                dictForm.Add("LEX");
                dictGloss.Add("GLS");
                altVerseNumber.Add("ALT");
                varUsed.Add("VAR");
                wordNumber.Add("W #");
                wordType.Add("TYP");
                morphology.Add("GMR");
                transliteration.Add("XLT");
                rootStrongs.Add("STG");
                tags.Add(tagLable);

                for (int i = 0; i < verseWords.Count; i++)
                {
                    VerseWord verseWord = verseWords[i];
                    words.Add(verseWord.Word);
                    greek.Add(verseWord.Greek);
                    dictForm.Add(verseWord.DictForm);
                    dictGloss.Add(verseWord.DictGloss);
                    morphology.Add(verseWord.Morphology);
                    transliteration.Add(verseWord.Transliteration);
                    rootStrongs.Add(verseWord.RootStrong);
                    wordType.Add(verseWord.WordType);
                    altVerseNumber.Add(verseWord.AltVerseNumber);
                    varUsed.Add(verseWord.VarUsed ? "*****" : "");
                    wordNumber.Add(verseWord.WordNumber);
                    tags.Add(verseWord.Strong);

                    /*                   string strng = string.Empty;
                                       foreach (string s in verseWord.Strong)
                                       {
                                           strng += "<" + s + "> ";
                                       }
                                       tags.Add(strng.Trim());
                    */
                    //tags.Add(verseWord.Strong.ToStringEx());
                }

                //this.ColumnCount = verseWords.Count;
                this.ColumnCount = words.Count;
                List<string> empty = new List<string>(ColumnCount);
                empty.AddRange(Enumerable.Repeat("", ColumnCount));

                this.Rows.Add(words.ToArray());
                this.Rows.Add(greek.ToArray());
                this.Rows.Add(empty.ToArray());
                this.Rows.Add(dictGloss.ToArray());
                this.Rows.Add(dictForm.ToArray());
                this.Rows.Add(altVerseNumber.ToArray());
                this.Rows.Add(varUsed.ToArray());
                this.Rows.Add(wordNumber.ToArray());
                this.Rows.Add(wordType.ToArray());
                int typeRow = 8;
                this.Rows.Add(morphology.ToArray());
                this.Rows.Add(transliteration.ToArray());
                this.Rows.Add(rootStrongs.ToArray());
                this.Rows.Add(tags.ToArray());

                this.ClearSelection();

                for (int i = 1; i < words.Count; i++)
                {
                    string type = (string)this.Rows[typeRow].Cells[i].Value;
                    if (!type.ToUpper().Contains("K"))
                    {
                        for (int j = 0; j < this.RowCount; j++) { 
                            this.Rows[j].Cells[i].Style.BackColor = Color.LightGray;
                        }
                    }
 
                }


                this.Rows[0].ReadOnly = true;
                this.Rows[1].ReadOnly = true;
            }
            catch (Exception ex)
            {
                var cm = System.Reflection.MethodBase.GetCurrentMethod();
                var name = cm.DeclaringType.FullName + "." + cm.Name;
                Tracing.TraceException(name, ex.Message);
            }
        }
    }
}
