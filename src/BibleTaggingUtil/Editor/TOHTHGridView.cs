using BibleTaggingUtil.BibleVersions;
using BibleTaggingUtil.Settings;
using BibleTaggingUtil.Strongs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BibleTaggingUtil.Editor
{
    internal class TOHTHGridView : DataGridView
    {
        public ReferenceVersionTAGNT BibleNT { get; set; }
        public ReferenceVersionTAHOT BibleOT { get; set; }
        public string SearchTag { get; internal set; }

        private BindingSource myGridBinder = new BindingSource();

        public TOHTHGridView()
        {
            // Enable double buffering
            this.DoubleBuffered = true;

            // Force control to redraw when resized and reduce background flickering
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint, true);
        }

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
                                e.RowIndex == this.Rows.Count - 1 &&
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
            Stopwatch stopwatch = Stopwatch.StartNew();

            var cm = System.Reflection.MethodBase.GetCurrentMethod();
            var name = cm.DeclaringType.FullName + "." + cm.Name;
            Tracing.TraceEntry(name);

            //this.Rows.Clear();
            //if (DataSource is BindingSource bs)
            //{
            //    if (bs.DataSource is DataTable dt)
            //        dt.Rows.Clear();
            //}

            if (DataSource is DataTable dt)
            {
                dt.Rows.Clear();
            }
            else
            {
                this.Rows.Clear();
            }
            if (verseWords == null)
                return;

            List<string> words = new List<string>();
            List<string> hebrew = new List<string>();
            List<string> transliteration = new List<string>();
            List<StrongsCluster> tags = new List<StrongsCluster>();
            List<string> morphology = new List<string>();
            List<string> rootStrongs = new List<string>();
            List<string> wordType = new List<string>();
            List<string> altVerseNumber = new List<string>();
            List<string> altStrongs = new List<string>();
            List<string> wordNumber = new List<string>();
            List<string> meaningVar = new List<string>();
            List<string> lexicalForm = new List<string>();
            List<string> gloss = new List<string>();
            StrongsCluster tagLable = new StrongsCluster("TAG");
            try
            {
                words.Add("ENG");               // English translation in/ beginning
                hebrew.Add("HEB");              // Hebrew בְּ/רֵאשִׁ֖ית
                altVerseNumber.Add("ALT");      // Alternate verse number
                wordNumber.Add("W #");          // Word number in TAHOT
                wordType.Add("TYP");            // Type L=Leningrad text; Q=Qere scribal corrections; K=original text; R=Restored text
                lexicalForm.Add("LEX");         // lexical form in Heberew (from Expanded Strong tags: H9003=ב=in/{H7225G=רֵאשִׁית=: beginning»first:1_beginning}) 
                gloss.Add("GLS");               // English gloss (from Expanded Strong tags: H9003=ב=in/{H7225G=רֵאשִׁית=: beginning»first:1_beginning})
                morphology.Add("GMR");          // Grammar HR/Ncfsa
                meaningVar.Add("VAR");          // Meaning Variants
                transliteration.Add("XLT");     // Transliteration be./re.Shit
                altStrongs.Add("A_S");          // Alternative Strong
                rootStrongs.Add("STG");         // dStrong H9003/{H7225G}
                tags.Add(tagLable);
                bool extended = Properties.ReferenceBibles.Default.ExtendedTaggingOT;
                for (int i = 0; i < verseWords.Count; i++)
                {
                    VerseWord verseWord = verseWords[i];
                    if (extended)
                    {
                        Tracing.TraceInfo(name, $"Extended Processing word {i + 1}/{verseWords.Count}: {verseWord.Hebrew} with {verseWord.TahotSubWords.Count} sub-words");
                        foreach (var sw in verseWord.TahotSubWords)
                        {
                            words.Add(sw.English);
                            hebrew.Add(sw.Hebrew);
                            morphology.Add(sw.Morphology);
                            transliteration.Add(sw.Transliteration);
                            rootStrongs.Add(sw.RootStrongs);
                            altStrongs.Add(sw.AltStrongs);
                            wordType.Add(sw.WordType);
                            altVerseNumber.Add(sw.AltWordNumber);
                            wordNumber.Add(sw.WordNumber);
                            meaningVar.Add(sw.MeaningVariant);
                            lexicalForm.Add(sw.LexicalForm);
                            gloss.Add(sw.Gloss);
                            tags.Add(sw.Tag);
                        }

                    }
                    else
                    {
                        Tracing.TraceInfo(name, $"Processing word {i + 1}/{verseWords.Count}: {verseWord.Hebrew} without sub-word expansion");
                        words.Add(verseWord.Word);
                        hebrew.Add(verseWord.Hebrew);
                        morphology.Add(verseWord.Morphology);
                        transliteration.Add(verseWord.Transliteration);
                        rootStrongs.Add(verseWord.RootStrong);
                        altStrongs.Add(verseWord.AltStrongs);
                        wordType.Add(verseWord.WordType);
                        altVerseNumber.Add(verseWord.AltVerseNumber);
                        wordNumber.Add(verseWord.WordNumber);
                        meaningVar.Add(verseWord.MeaningVar);
                        lexicalForm.Add(verseWord.DictForm);
                        gloss.Add(verseWord.DictGloss);
                        tags.Add(verseWord.Strong);
                    }
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
                    */
                    ;
                }

                Tracing.TraceInfo(name, "Finished processing words. Total columns to display: " + words.Count);
                var first = stopwatch.Elapsed;
                stopwatch.Restart();
                var table = BuildTable(
                    extended,
                    words,
                    hebrew,
                    altVerseNumber, // remove when extended to speed-up painting
                    wordNumber,
                    wordType,       // remove when extended to speed-up painting
                    lexicalForm,
                    gloss,
                    morphology,
                    //int morphRow = 7; // becomes 5 when extended
                    meaningVar,     // remove when extended to speed-up painting
                    transliteration,// remove when extended to speed-up painting
                    altStrongs,     // remove when extended to speed-up painting
                    rootStrongs,
                    tags);


                Tracing.TraceInfo(name, "Adding rows to grid. This may take some time for verses with many words and extended tagging enabled");
               
                ColumnWidthMode columnWidthMode = (ColumnWidthMode)Properties.ReferenceBibles.Default.ColumnWidthMode;
                int columnWidth = Properties.ReferenceBibles.Default.FixedColumnWidth;

                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

                var second = stopwatch.Elapsed;
                stopwatch.Restart();

                SuspendLayout();
                DataBindings.Clear();
                DataSource = null;
                Columns.Clear();    // if this line is commented out, then, for Gen 1:2: populating the grid goes from 3ms to 7ms

                //DataSource = myGridBinder;  // this approach slow down populating the grid
                //myGridBinder.DataSource = table;

                DataSource = table;
                if(columnWidthMode == ColumnWidthMode.Fixed)
                {
                    // for Gen 1:2: the following loop adds 3 ms
                    foreach (DataGridViewColumn col in Columns)
                    {
                        col.Width = columnWidth; // Default width in pixels
                    }
                }

                Tracing.TraceInfo(name, "Finished adding rows to grid. Starting to auto size columns");
                if (columnWidthMode == ColumnWidthMode.Auto)
                {
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
                    Tracing.TraceInfo(name, "Finished auto sizing columns");
                } 
                ResumeLayout();
                int typeRow = 4; // Type L=Leningrad text; Q=Qere scribal corrections; K=original text; R=Restored text
                int morphRow = 7;
                //if(extended)
                //morphRow = 5;

                var third = stopwatch.Elapsed;
                stopwatch.Restart();

                Tracing.TraceInfo(name, "Finished adding rows to grid. Starting to color code special words and search tags");
                //this.Rows[1].DefaultCellStyle.ForeColor = Color.Black;
                //this.Rows[this.RowCount - 1].DefaultCellStyle.ForeColor = Color.Black;
                this.DefaultCellStyle.ForeColor = Color.Black;
                for (int i = 0; i < words.Count; i++)
                {
                    string word = (string)this.Rows[1].Cells[i].Value;
                    StrongsCluster tag = (StrongsCluster)this.Rows[this.RowCount - 1].Cells[i].Value;
                    if (word.Contains("יהוה") || tag.ToString().Contains("3069") || tag.ToString().Contains("3068"))
                    {
                        this.Rows[1].Cells[i].Style.ForeColor = Color.Red;
                        this.Rows[this.RowCount - 1].Cells[i].Style.ForeColor = Color.Red;
                    }
        //            else
        //            {
        //                this.Rows[1].Cells[i].Style.ForeColor = Color.Black;
        //                this.Rows[this.RowCount - 1].Cells[i].Style.ForeColor = Color.Black;
        //            }
                    if (SearchTag != null && tag.ToString().Contains(SearchTag))
                    {
                        this.Rows[this.RowCount - 1].Cells[i].Style.ForeColor = Color.Maroon;
                        if (RowCount > 2)
                            this.Rows[this.RowCount - 2].Cells[i].Style.BackColor = Color.Yellow;
                    }

                    // higlight columns wher the type does not have 'L'
                    string type = (string)this.Rows[typeRow].Cells[i].Value;
                    if (i> 0 && type.Contains("X"))
                    {
                        for (int j = 0; j < this.RowCount; j++)
                        {
                            this.Rows[j].Cells[i].Style.BackColor = Color.LightGray;
                        }
                    }

                }

                // Color code repeated words with same strongs number and different morphology
                Tracing.TraceInfo(name, "Starting to color code repeated words with same strongs number and different morphology");
                int colourIndex = 0;
                List<Color> colours = new List<Color>() { Color.Green, Color.Blue, Color.Red, Color.Brown, Color.DarkOrange, Color.Maroon };
                Dictionary<string, int> repeated = new Dictionary<string, int>();
                for (int i = 1; i < words.Count; i++)
                {
                    StrongsCluster tag = (StrongsCluster)this.Rows[this.RowCount - 1].Cells[i].Value;
                    string morph = (string)this.Rows[morphRow].Cells[i].Value;
                    for (int j = i + 1; j < words.Count; j++)
                    {
                        StrongsCluster tag2 = (StrongsCluster)this.Rows[this.RowCount - 1].Cells[j].Value;
                        string morph2 = (string)this.Rows[morphRow].Cells[j].Value;
                        if (tag.ToStringS() == tag2.ToStringS() && morph != morph2 && !tag.IsTagLable)
                        {
                            if (!repeated.ContainsKey(tag.ToStringS()))
                            {
                                repeated.Add(tag.ToStringS(), colourIndex);
                                colourIndex = (colourIndex + 1) % colours.Count;
                            }
                            Color foreColor = colours[repeated[tag.ToStringS()]];
                            this.Rows[morphRow].Cells[i].Style.ForeColor = foreColor;
                            this.Rows[morphRow].Cells[i].Style.BackColor = Color.LightGray;
                            this.Rows[morphRow].Cells[j].Style.ForeColor = foreColor;
                            this.Rows[morphRow].Cells[j].Style.BackColor = Color.LightGray;
                        }
                    }
                }
                var fourth = stopwatch.Elapsed;
                stopwatch.Stop();
                Tracing.TraceExit(name);
            }
            catch (Exception ex)
            {
                Tracing.TraceException(name, ex.Message);
            }


            this.ClearSelection();

            this.Rows[0].ReadOnly = true;
            this.Rows[1].ReadOnly = true;

        }
        private DataTable BuildTable(
                    bool extended,
                    List<string> words,
                    List<string> hebrew,
                    List<string> altVerseNumber,
                    List<string> wordNumber,
                    List<string> wordType,
                    List<string> lexicalForm,
                    List<string> gloss,
                    List<string> morphology,
                    //int morphRow = 7;
                    List<string> meaningVar,
                    List<string> transliteration,
                    List<string> altStrongs,
                    List<string> rootStrongs,
                    List<StrongsCluster> tags)
        {
            var cm = System.Reflection.MethodBase.GetCurrentMethod();
            var name = cm.DeclaringType.FullName + "." + cm.Name;
            Tracing.TraceEntry(name);

            var table = new DataTable();
            int columnCount = words.Count;
            // Create columns
            for (int c = 0; c < columnCount; c++)
                table.Columns.Add("", typeof(object));   // no names

            // Add each array as a row
            table.Rows.Add(words.ToArray());
            table.Rows.Add(hebrew.ToArray());
            //if(!extended)
                table.Rows.Add(altVerseNumber.ToArray());
            table.Rows.Add(wordNumber.ToArray());
            //if (!extended)
                table.Rows.Add(wordType.ToArray());
            table.Rows.Add(lexicalForm.ToArray());
            table.Rows.Add(gloss.ToArray());
            table.Rows.Add(morphology.ToArray());
            //if (!extended)
                table.Rows.Add(meaningVar.ToArray());
            //if (!extended)
                table.Rows.Add(transliteration.ToArray());
            //if (!extended)
                table.Rows.Add(altStrongs.ToArray());
            table.Rows.Add(rootStrongs.ToArray());
            table.Rows.Add(tags.ToArray());

            Tracing.TraceExit(name);
            return table;
        }

        protected override void OnDataBindingComplete(DataGridViewBindingCompleteEventArgs e)
        {
            //Columns.Cast<DataGridViewColumn>().ToList().ForEach(c => c.Width = 200);
            base.OnDataBindingComplete(e);
        }

        protected override void OnCellFormatting(DataGridViewCellFormattingEventArgs e)
        {
            //if (this.Rows.Count > 1 && this[0, 1].Value.ToString() == "GRK" )
            {
                if (this[0, e.RowIndex].Value.ToString() == "GMR")
                {
                    for (int i = 1; i < this.ColumnCount; i++)
                    {
                        DataGridViewCell cell = this.Rows[e.RowIndex].Cells[i];
                        cell.ToolTipText = GetMorphologyDetails(this[0, 1].Value.ToString(), cell.Value.ToString());
                    }
                }
            }

            //base.OnCellFormatting(e);
        }

        private Morphology.NT morphNT = new Morphology.NT();
        private Morphology.OT morphOT = new Morphology.OT();
        private string GetMorphologyDetails(string lang, string morph)
        {
            string result = string.Empty;
            if (lang == "GRK")
            {
                if (morph.Contains("/"))
                {
                    string[] parts = morph.Split('/');

                    foreach (string m in parts)
                    {
                        result += m.Trim() + ":\r\n" + morphNT.GetMorphologyDetails(m.Trim()) + "\r\n";
                    }
                }
                else
                {
                    result = morphNT.GetMorphologyDetails(morph);
                }
            }
            else
            {
                try
                {
                    result = morphOT.GetMorphologyDetails(morph);
                }
                catch (Exception ex)
                {
                    int x = 0;
                }
            }

            return result;
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
            List<string> conjoin = new List<string>();
            List<string> transliteration = new List<string>();
            List<StrongsCluster> tags = new List<StrongsCluster>();
            List<StrongsCluster> dTags = new List<StrongsCluster>();
            List<string> morphology = new List<string>();
            List<string> rootStrongs = new List<string>();
            List<string> altStrongs = new List<string>();
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
                conjoin.Add("CNJ");
                altVerseNumber.Add("ALT");
                varUsed.Add("VAR");
                wordNumber.Add("W #");
                wordType.Add("TYP");
                morphology.Add("GMR");
                transliteration.Add("XLT");
                altStrongs.Add("A_S");
                rootStrongs.Add("STG");
                tags.Add(tagLable);
                dTags.Add(tagLable);

                for (int i = 0; i < verseWords.Count; i++)
                {
                    VerseWord verseWord = verseWords[i];
                    if (verseWord.Reference.Contains("5:48") && i == 9)
                    {
                        int x = 0;
                    }
                    words.Add(verseWord.Word);
                    greek.Add(verseWord.Greek);
                    conjoin.Add(verseWord.ConjoinWord);
                    dictForm.Add(verseWord.DictForm);
                    dictGloss.Add(verseWord.DictGloss);
                    morphology.Add(verseWord.Morphology);
                    transliteration.Add(verseWord.Transliteration);
                    rootStrongs.Add(verseWord.RootStrong);
                    altStrongs.Add(verseWord.AltStrongs);
                    wordType.Add(verseWord.WordType);
                    altVerseNumber.Add(verseWord.AltVerseNumber);
                    varUsed.Add(verseWord.VarUsed ? "*****" : "");
                    wordNumber.Add(verseWord.WordNumber);
                    tags.Add(verseWord.Strong);
                    dTags.Add(verseWord.dStrong);

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
                //this.Rows.Add(empty.ToArray());
                this.Rows.Add(dictGloss.ToArray());
                this.Rows.Add(dictForm.ToArray());
                this.Rows.Add(altVerseNumber.ToArray());
                this.Rows.Add(varUsed.ToArray());
                this.Rows.Add(wordNumber.ToArray());
                this.Rows.Add(conjoin.ToArray());
                this.Rows.Add(wordType.ToArray());
                int typeRow = 8;
                this.Rows.Add(morphology.ToArray());
                int morphRow = 9;
                this.Rows.Add(transliteration.ToArray());
                this.Rows.Add(altStrongs.ToArray());
                this.Rows.Add(rootStrongs.ToArray());
                if (Properties.ReferenceBibles.Default.dStrongs)
                    this.Rows.Add(dTags.ToArray());
                else
                    this.Rows.Add(tags.ToArray());

                this.ClearSelection();

                // Gray out columns that are not tagged with a K strongs number
                for (int i = 1; i < words.Count; i++)
                {
                    string type = (string)this.Rows[typeRow].Cells[i].Value;
                    if (!type.ToUpper().Contains("K"))
                    {
                        for (int j = 0; j < this.RowCount; j++)
                        {
                            this.Rows[j].Cells[i].Style.BackColor = Color.LightGray;
                        }
                    }

                }

                // Color code repeated words with same strongs number and different morphology
                int colourIndex = 0;
                List<Color> colours = new List<Color>() { Color.Green, Color.Blue, Color.Red, Color.Brown, Color.DarkOrange, Color.Maroon };
                Dictionary<string, int> repeated = new Dictionary<string, int>();
                for (int i = 1; i < words.Count; i++)
                {
                    StrongsCluster tag = (StrongsCluster)this.Rows[this.RowCount - 1].Cells[i].Value;
                    string morph = (string)this.Rows[morphRow].Cells[i].Value;
                    for (int j = i + 1; j < words.Count; j++)
                    {
                        StrongsCluster tag2 = (StrongsCluster)this.Rows[this.RowCount - 1].Cells[j].Value;
                        string morph2 = (string)this.Rows[morphRow].Cells[j].Value;
                        if (tag.ToStringS() == tag2.ToStringS() && morph != morph2 && !tag.IsTagLable)
                        {
                            if (!repeated.ContainsKey(tag.ToStringS()))
                            {
                                repeated.Add(tag.ToStringS(), colourIndex);
                                colourIndex = (colourIndex + 1) % colours.Count;
                            }
                            Color foreColor = colours[repeated[tag.ToStringS()]];
                            this.Rows[morphRow].Cells[i].Style.ForeColor = foreColor;
                            this.Rows[morphRow].Cells[i].Style.BackColor = Color.LightGray;
                            this.Rows[morphRow].Cells[j].Style.ForeColor = foreColor;
                            this.Rows[morphRow].Cells[j].Style.BackColor = Color.LightGray;
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

        /// <summary>
        /// Highlights unused strongs numbers in dgvTarget tags row
        /// </summary>
        /// <param name="dgvTarget"></param>
        /// <exception cref="NotImplementedException"></exception>
        internal void HighlightUnusedStrongs(TargetGridView dgvTarget)
        {
            if (this.Rows.Count == 0 || dgvTarget.Rows.Count == 0)
                return;
            int tagsRowIndex = this.Rows.Count - 1;
            int targetTagsRowIndex = dgvTarget.Rows.Count - 1;
            for (int i = 1; i < this.ColumnCount; i++)
            {
                StrongsCluster tag = (StrongsCluster)this.Rows[tagsRowIndex].Cells[i].Value;
                bool used = false;
                for (int j = 0; j < dgvTarget.ColumnCount; j++)
                {
                    StrongsCluster targetTag = (StrongsCluster)dgvTarget.Rows[targetTagsRowIndex].Cells[j].Value;
                    if (targetTag.ToStringS().Contains(tag.ToStringS()) && !tag.IsTagLable)
                    {
                        used = true;
                        break;
                    }
                }
                if (!used && !tag.IsTagLable)
                {
                    this.Rows[tagsRowIndex].Cells[i].Style.BackColor = Color.LightPink;
                }
            }
        }

        /// <summary>
        /// Highlights strongs numbers overused by dgvTarget tags row
        /// </summary>
        /// <param name="dgvTarget"></param>
        /// <exception cref="NotImplementedException"></exception>
        internal void HighlightOverusedStrongs(TargetGridView dgvTarget)
        {
            if (this.Rows.Count == 0 || dgvTarget.Rows.Count == 0)
                return;
            int tagsRowIndex = this.Rows.Count - 1;
            int targetTagsRowIndex = dgvTarget.Rows.Count - 1;

            // In this grid, a strongs number may be used multiple times
            // we want to ensure the number of times a strongs number is used in dgvTarget does not exceed the number of times it appears in this grid
            // if it does, we highlight the excess usage in this and dgvTarget

            // Count the occurrences of each strongs number in this grid
            Dictionary<string, int> tagCounts = new Dictionary<string, int>();
            for (int i = 1; i < this.ColumnCount; i++)
            {
                StrongsCluster tag = (StrongsCluster)this.Rows[tagsRowIndex].Cells[i].Value;
                if (!tag.IsTagLable)
                {
                    string tagStr = tag.ToStringS();
                    if (!tagCounts.ContainsKey(tagStr))
                        tagCounts.Add(tagStr, 0);
                    tagCounts[tagStr]++;
                }
            }
            // Count the occurrences of each strongs number in dgvTarget
            Dictionary<string, List<int>> targetTagCounts = new Dictionary<string, List<int>>();
            for (int j = 0; j < dgvTarget.ColumnCount; j++)
            {
                StrongsCluster targetTag = (StrongsCluster)dgvTarget.Rows[targetTagsRowIndex].Cells[j].Value;
                if (!targetTag.IsTagLable)
                {
                    string tagStr = targetTag.ToStringS();
                    // tagStr may contain multiple strongs numbers separated by space, we need to count each of them separately
                    string[] tagStrs = tagStr.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string ts in tagStrs)
                    {
                         if (!targetTagCounts.ContainsKey(ts))
                            targetTagCounts.Add(ts, new List<int>());
                        targetTagCounts[ts].Add(j);
                    }
                }
            }
            // Highlight excess usage
            foreach (var kvp in targetTagCounts)
            {
                string tagStr = kvp.Key;
                if (string.IsNullOrEmpty(tagStr))
                    continue;
                List<int> targetColumns = kvp.Value;
                int countInThis = tagCounts.ContainsKey(tagStr) ? tagCounts[tagStr] : 0;
                if (targetColumns.Count > countInThis)
                {
                    // Highlight excess usage in dgvTarget
                    for (int i = 0 /*countInThis*/; i < targetColumns.Count; i++)
                    {
                        int colIndex = targetColumns[i];
                        dgvTarget.Rows[targetTagsRowIndex].Cells[colIndex].Style.BackColor = Color.LightSalmon;
                    }
                    // Highlight excess usage in this grid
                    int excessCount = targetColumns.Count - countInThis;
                    for (int i = 1; i < this.ColumnCount && excessCount > 0; i++)
                    {
                        StrongsCluster tag = (StrongsCluster)this.Rows[tagsRowIndex].Cells[i].Value;
                        if (tag.ToStringS() == tagStr && !tag.IsTagLable)
                        {
                            this.Rows[tagsRowIndex].Cells[i].Style.BackColor = Color.LightSalmon;
                            //excessCount--;
                        }
                    }
                }
            }




        }
    }
}
