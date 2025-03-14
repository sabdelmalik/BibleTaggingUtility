﻿using BibleTaggingUtil.Strongs;
using System.Windows.Forms;

namespace BibleTaggingUtil.Editor
{
    public class DragData
    {
        public DragData(int rowIndex, int columnIndex, StrongsCluster tag, DataGridView source)
        {
            Tag = tag;
            Source = source;
            ColumnIndex = columnIndex;
            RowIndex = rowIndex;
        }

        //public DragData(int rowIndex, int columnIndex, string text, DataGridView source)
        //{
        //    Text = text;
        //    Source = source;
        //    ColumnIndex = columnIndex;
        //    RowIndex = rowIndex;
        //}

        public string Text { get; private set; }
        public StrongsCluster Tag { get; private set; }
        public DataGridView Source { get; private set; }
        public int ColumnIndex { get; private set; }
        public int RowIndex { get; private set; }

    }

}
