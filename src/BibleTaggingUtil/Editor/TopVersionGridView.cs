﻿using BibleTaggingUtil.BibleVersions;
using BibleTaggingUtil.Strongs;
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
    internal class TopVersionGridView : DataGridView
    {
        public ReferenceTopVersion Bible { get; set; }

        protected override void OnCellMouseDown(DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && e.Clicks == 1)
            {
                if (e.RowIndex >= 0)
                {
                    StrongsCluster tag = (StrongsCluster)this.Rows[1].Cells[e.ColumnIndex].Value;
                    if (tag != null)
                    {
                        DragData data = new DragData(1, e.ColumnIndex, tag, this);
                        this.DoDragDrop(data, DragDropEffects.Copy);
                    }
                }
            }
            base.OnCellMouseDown(e);
        }


        /// <summary>
        /// Populates the reference verse data grid view with the 
        /// verse words and tags
        /// </summary>
        /// <param name="verse">The reference verse including tags</param>
        public void Update(Verse verse)
        {
            this.Rows.Clear();
            if (verse == null)
                return;

            string[] verseWords = new string[verse.Count];
            StrongsCluster[] verseTags = new StrongsCluster[verse.Count];

            try
            {
                for (int i = 0; i < verse.Count; i++)
                {
                    verseWords[i] = verse[i].Word;
                    verseTags[i] = verse[i].Strong;
                   
                    // string thisTag = verse[i].Strong.ToString();//string.Empty;

                    /*                for (int j = 0; j < verse[i].Strong.Count; j++)
                                    {
                                        if (verse[i].Strong[j].Contains('('))
                                            thisTag += verse[i].Strong[j] + " ";
                                        else
                                            thisTag += "<" + verse[i].Strong[j] + "> ";
                                    }

                                    thisTag = thisTag.Trim().
                                            Replace(",", "").
                                            Replace(".", "").
                                            Replace(";", "").
                                            Replace(":", "");
                    */
                    //if (thisTag == "<>")
                    //    thisTag = string.Empty;
                    //verseTags[i] = thisTag;
                }


                this.ColumnCount = verseWords.Length;
                this.Rows.Add(verseWords);
                this.Rows.Add(verseTags);

                for (int i = 0; i < verseTags.Length; i++)
                {
                    StrongsCluster tag = (StrongsCluster)this.Rows[1].Cells[i].Value;
                    if (tag == null)
                        continue;
                    if (tag.ToString().Contains("0410>"))
                    {
                        this.Rows[1].Cells[i].Style.BackColor = Color.Yellow;
                    }
                    
                    if (Properties.ReferenceBibles.Default.TopRightToLeft)
                        this.Columns[i].DisplayIndex = verseWords.Length - i - 1;
                    else
                        this.Columns[i].DisplayIndex = i;
                }


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
