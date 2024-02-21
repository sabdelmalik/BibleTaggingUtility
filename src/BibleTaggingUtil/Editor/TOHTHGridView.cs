using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BibleTaggingUtil.Editor
{
    internal class TOHTHGridView : DataGridView
    {

        protected override void OnCellMouseDown(DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && e.Clicks == 1)
            {
                string text = string.Empty;
                if (e.RowIndex >= 0)
                {
                    if (this.SelectedCells.Count > 1 && Control.ModifierKeys != Keys.Control)
                    {
                        bool sameRow = true;
                        foreach (DataGridViewCell cell in this.SelectedCells)
                        {
                            // we only merge in the top row
                            if (cell.RowIndex != this.RowCount - 1)
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
                            text = (string)this[colIndex, this.RowCount - 1].Value;
                            for (int i = count - 2; i >= 0 ; i--)
                            {
                                text += (" " + this[SelectedCells[i].ColumnIndex, this.RowCount - 1].Value);
                                if (Math.Abs(SelectedCells[i].ColumnIndex - colIndex) != 1)
                                {
                                    //mergeOk = false;
                                    text = string.Empty;
                                    break;
                                }
                                colIndex = SelectedCells[i].ColumnIndex;
                            }
                        }


                    }
                    else
                    {
                        text = ((String)this.Rows[this.RowCount - 1].Cells[e.ColumnIndex].Value).Trim();
                    }
                    DragData data = new DragData(1, e.ColumnIndex, text, this);
                    if (!string.IsNullOrEmpty(text))
                    {
                        this.DoDragDrop(data, DragDropEffects.Copy);
                    }
                }
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
            List<string> tags = new List<string>();
            List<string> morphology = new List<string>();
            List<string> rootStrongs = new List<string>();
            List<string> wordType =   new List<string>();
            List<string> altVerseNumber = new List<string>();
            List<string> wordNumber = new List<string>();
            List<string> meaningVar = new List<string>();

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
                tags.Add("TAG");

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

                    if (verseWord.Strong.Length > 0)
                    {
                        string s = String.Empty;
                        bool E = (verseWord.Hebrew.Trim() == "אֱלֹהִים");
                        bool Y = (verseWord.Hebrew.Trim() == "יהוה");
                        bool strongIsE = (verseWord.Strong[0].Trim() == "0430");
                        bool strongIsY = ((verseWord.Strong[0].Trim() == "3068") || (verseWord.Strong[0].Trim() == "3069"));

                        if (E || Y)
                        {
                            // special treatment for אֱלֹהִים & יהוה
                            if ((E && strongIsE) || (Y && strongIsY))
                                s = "<" + verseWord.Strong[0] + ">";
                        }
                        else
                        {
                            s = "<" + verseWord.Strong[0] + ">";
                        }

                        if (verseWord.Strong.Length > 1)
                        {
                            for (int j = 1; j < verseWord.Strong.Length; j++)
                            {
                                strongIsE = (verseWord.Strong[j].Trim() == "0430");
                                strongIsY = ((verseWord.Strong[j].Trim() == "3068") || (verseWord.Strong[j].Trim() == "3069"));
                                if (E || Y)
                                {
                                    // special treatment for אֱלֹהִים & יהוה
                                    if ((E && strongIsE) || (Y && strongIsY))
                                    {
                                        if (!string.IsNullOrEmpty(s))
                                            s += " ";
                                        s += "<" + verseWord.Strong[j] + ">";
                                    }
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(s))
                                        s += " ";
                                    s += "<" + verseWord.Strong[j] + ">";
                                }
                            }
                        }
                        tags.Add(s.Trim());
                    }
                    else
                        tags.Add("");
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
                    string strong = (string)this.Rows[this.RowCount-1].Cells[i].Value;
                    if (word.Contains("יהוה") || strong.Contains("3069") || strong.Contains("3068"))
                    {
                        this.Rows[1].Cells[i].Style.ForeColor = Color.Red;
                        this.Rows[this.RowCount - 1].Cells[i].Style.ForeColor = Color.Red;
                    }
                    else
                    {
                        this.Rows[1].Cells[i].Style.ForeColor = Color.Black;
                        this.Rows[this.RowCount - 1].Cells[i].Style.ForeColor = Color.Black;
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
            List<string> transliteration = new List<string>();
            List<string> tags = new List<string>();
            List<string> morphology = new List<string>();
            List<string> rootStrongs = new List<string>();
            List<string> wordType = new List<string>();
            List<string> altVerseNumber = new List<string>();
            List<string> wordNumber = new List<string>();

            try
            {
                words.Add("ENG");
                greek.Add("GRK");
                altVerseNumber.Add("ALT");
                wordNumber.Add("W #");
                wordType.Add("TYP");
                morphology.Add("GMR");
                transliteration.Add("XLT");
                rootStrongs.Add("STG");
                tags.Add("TAG");

                for (int i = 0; i < verseWords.Count; i++)
                {
                    VerseWord verseWord = verseWords[i];
                    words.Add(verseWord.Word);
                    greek.Add(verseWord.Greek);
                    morphology.Add(verseWord.Morphology);
                    transliteration.Add(verseWord.Transliteration);
                    rootStrongs.Add(verseWord.RootStrong);
                    wordType.Add(verseWord.WordType);
                    altVerseNumber.Add(verseWord.AltVerseNumber);
                    wordNumber.Add(verseWord.WordNumber);

                    string strng = string.Empty;
                    foreach (string s in verseWord.Strong)
                    {
                        strng += "<" + s + "> ";
                    }
                    tags.Add(strng.Trim());
                }

                //this.ColumnCount = verseWords.Count;
                this.ColumnCount = words.Count;

                this.Rows.Add(words.ToArray());
                this.Rows.Add(greek.ToArray());
                this.Rows.Add(altVerseNumber.ToArray());
                this.Rows.Add(wordNumber.ToArray());
                this.Rows.Add(wordType.ToArray());
                this.Rows.Add(morphology.ToArray());
                this.Rows.Add(transliteration.ToArray());
                this.Rows.Add(rootStrongs.ToArray());
                this.Rows.Add(tags.ToArray());

                this.ClearSelection();

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
