using BibleTaggingUtil.OsisXml;
using BibleTaggingUtil.Strongs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace BibleTaggingUtil.BibleVersions
{
    public abstract partial class BibleVersion
    {

        protected BibleTaggingForm container;

        /// <summary>
        /// Bible Dictionary
        /// Key: verse reference (xxx c:v) xxx = OSIS book name, v = verse number
        /// </summary>
        protected Dictionary<string, Verse> bible = new Dictionary<string, Verse>();

        /// <summary>
        /// Bible Dictionary
        /// Key: UBS book name
        /// value: loaded Bible book name
        /// </summary>
        protected Dictionary<string, string> bookNames = new Dictionary<string, string>();

        protected List<string> bookNamesList = new List<string>();

        private const string referencePattern1 = @"^([0-9A-Za-z]+)\s([0-9]+):([0-9]+)\s*(.*)";
        private const string referencePattern2 = @"^[0-9]+_([0-9A-Za-z]+)\.([0-9]+)\.([0-9]+)\s*(.*)";
        private const string referencePattern3 = @"^([0-9A-Za-z]{3})\.([0-9]+)\.([0-9]+)\s*(.*)";
        private string textReferencePattern = string.Empty;

        protected string bibleName = string.Empty;
        protected int totalVerses = 0;
        protected int currentVerseCount;

        public BibleVersion(BibleTaggingForm container, int totalVerses)
        {
            this.container = container;
            this.totalVerses = totalVerses;
        }

        public Dictionary<string, Verse> Bible { get { return bible; } }

        public string BibleName { get { return bibleName; } set { bibleName = value;} }

        public int LastVerse(string book, int chapter)
        {
           return container.VerseSelectionPanel.LastVerse(book, chapter);
        }

        public virtual bool LoadBibleFile(string textFilePath, bool newBible, bool more)
        {
            var cm = System.Reflection.MethodBase.GetCurrentMethod();
            var name = cm.DeclaringType.FullName + "." + cm.Name;
            Tracing.TraceInfo(name, $"Entry");

            if (newBible)
            {
                bible.Clear();
                bookNames.Clear();
                bookNamesList.Clear();
                currentVerseCount = 0;
            }
            string ext = Path.GetExtension(textFilePath); 
            if (ext == ".xml" && this is ReferenceTopVersion)
                return LoadOsisBibleFileInternal(textFilePath, more);
            else
                return LoadBibleFileInternal(textFilePath, more);
        }

        protected virtual bool LoadBibleFileInternal(string textFilePath, bool more)
        {
            Tracing.TraceEntry(MethodBase.GetCurrentMethod().Name, textFilePath, more);
            bool result = false;
            bool individual = Properties.TargetBibles.Default.IndividualBooks;

            // We fix files only for TargetVersion because we don't want to modify the reference versions. The reference versions are used for comparison and we want to keep them intact.
            if (this is TargetVersion && !FixFileIfCorrupt(textFilePath))
            {
                return false;
            }

            if (File.Exists(textFilePath))
            {
                result = true;
                using (var fileStream = new FileStream(textFilePath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        while (reader.Peek() >= 0)
                        {
                            var line = reader.ReadLine().Trim(' ');
                            if (line.StartsWith('#'))
                                continue;
                            if (!string.IsNullOrEmpty(line))
                            {
                                if (string.IsNullOrEmpty(textReferencePattern))
                                {
                                    //if (!SetSearchPattern(line, out textReferencePattern))
                                    //    continue;
                                    SetSearchPattern(line, out textReferencePattern);
                                }
                                if (!string.IsNullOrEmpty(textReferencePattern))
                                {
                                    AddBookName(line);
                                }
                                if (line.StartsWith("John 9:3"))
                                {
                                    int x = 0;
                                }
                                ParseLine(line);
                            }
                                       }

                    }
                }

            }

            if(!more && !(new int[] {66, 39, 27 }).Contains(bookNamesList.Count))
            {
                if(!individual && this is TargetVersion)
                {
                    Tracing.TraceError(MethodBase.GetCurrentMethod().Name, string.Format("{0}:Book Names Count = {1}. Was expecting 66, 39 or 27",
                                        Path.GetFileName(textFilePath), bookNamesList.Count));
                    return false;
                }
            }

            bookNames.Clear();
            if (individual && this is TargetVersion)
            {
                Tracing.TraceInfo(MethodBase.GetCurrentMethod().Name, string.Format("Individual books detected. Book Names Count = {0}", bookNamesList.Count));
                for (int i = 0; i < bookNamesList.Count; i++)
                {
                    int idx = Utils.GetBookIndexFromBook(bookNamesList[i]);
                    bookNames.Add(Constants.ubsNames.Keys.ToArray()[idx], bookNamesList[i]);
                }
            }
            else
            {
                if (bookNamesList.Count == 66 || bookNamesList.Count == 39)
                {
                    for (int i = 0; i < bookNamesList.Count; i++)
                    {
                        bookNames.Add(Constants.ubsNames.Keys.ToArray()[i], bookNamesList[i]);
                    }
                }
                else if (bookNamesList.Count == 27)
                {
                    for (int i = 0; i < bookNamesList.Count; i++)
                    {
                        bookNames.Add(Constants.ubsNames.Keys.ToArray()[i + 39], bookNamesList[i]);
                    }
                }
            }
            return result;
        }

    }
}
