using BibleTaggingUtil.OsisXml;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms.Design;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace BibleTaggingUtil.BibleVersions
{
    public abstract class BibleVersion
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

        public string BibleName { set { bibleName = value;} }

        public virtual bool LoadBibleFile(string textFilePath, bool newBible, bool more)
        {
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
                                if(line.StartsWith("3Jo 1:15"))
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
                Tracing.TraceError(MethodBase.GetCurrentMethod().Name, string.Format("{0}:Book Names Count = {1}. Was expecting 66, 39 or 27",
                                        Path.GetFileName(textFilePath), bookNamesList.Count));
                return false;
            }


            if (bookNamesList.Count == 66 || bookNamesList.Count == 39)
            {
                for (int i = 0; i < bookNamesList.Count; i++)
                {
                    bookNames.Add(Constants.ubsNames[i], bookNamesList[i]);
                }
            }
            else if (bookNamesList.Count == 27)
            {
               for (int i = 0; i < bookNamesList.Count; i++)
                {
                    bookNames.Add(Constants.ubsNames[i+39], bookNamesList[i]);
                }
            }
            return result;
        }

        public int BookCount
        {
            get
            {
                return bookNamesList.Count;
            }
        }
        public Dictionary<string, Verse> Bible { get { return bible; } }

        public string this[string ubsName]
        {
            get
            {
                string bookName = string.Empty;
                try
                {
                    if(bookNames.Count > 0)
                    bookName = bookNames[ubsName];
                }
                catch(Exception ex)
                {
                    var cm = System.Reflection.MethodBase.GetCurrentMethod();
                    var name = cm.DeclaringType.FullName + "." + cm.Name;
                    Tracing.TraceException(name, ex.Message);
                }
                return bookName;

            }
        }

        private bool AddBookName(string line)
        {
            if(line.ToLower().Contains("gen"))
            {
                int o = 0;
            }
            Match mTx = Regex.Match(line, textReferencePattern);
            if (!mTx.Success)
            {
                //Tracing.TraceError(MethodBase.GetCurrentMethod().Name, "Could not detect text reference: " + line);
                return false;
            }

            String book = mTx.Groups[1].Value;
            if (!bookNamesList.Contains(book))
                bookNamesList.Add(book);

            return true;
        }

        private bool SetSearchPattern(string line, out string referancePattern)
        {

            Match mTx = Regex.Match(line, referencePattern1);
            if (mTx.Success)
            {
                referancePattern = referencePattern1;
                return true;
            }

            mTx = Regex.Match(line, referencePattern2);
            if (mTx.Success)
            {
                referancePattern = referencePattern2;
                return true;
            }

            mTx = Regex.Match(line, referencePattern3);
            if (mTx.Success)
            {
                referancePattern = referencePattern3;
                return true;
            }

            //Tracing.TraceError(MethodBase.GetCurrentMethod().Name, "Could not detect reference pattern: " + line);
            referancePattern = string.Empty;
            return false;
        }

        protected virtual void ParseLine(string line)
        {

            Match mTx = Regex.Match(line, @"^([0-9A-Za-z]+)\s([0-9]+):([0-9]+)\s*(.*)");

            /*            // find spcae between book and Chapter
                        int spaceB = line.IndexOf(' ');
                        // find spcae between Verse number and Text
                        int spaceV = line.IndexOf(' ', spaceB + 1);
                        if (spaceV == -1)
                        {
                            throw new Exception(string.Format("Ill formed verse line!"));
                        }

                        string book = line.Substring(0, spaceB);
                        string reference = line.Substring(0, spaceV);
                        string verse = line.Substring(spaceV + 1);*/

            string book = mTx.Groups[1].Value;
            string chapter = mTx.Groups[2].Value;
            string verseNo = mTx.Groups[3].Value;
            string verse = mTx.Groups[4].Value;
            string reference = string.Format("{0} {1}:{2}", book, chapter, verseNo);
            if(reference == "Jhn 1:1")
            { 
                int x = 0;
            }


            BibleTestament testament = Utils.GetTestament(reference);

            string[] verseParts = verse.Split(' ');
            List<string> words = new List<string>();
            List<string> tags= new List<string>();
            string tempWord = string.Empty;
            string tmpTag = string.Empty;
            for (int i = 0; i < verseParts.Length; i++)
            {
                string versePart = verseParts[i].Trim();
                if (string.IsNullOrEmpty(versePart))
                    continue; // some extra space
                if (i == 0 || versePart[0] != '<' ) // add i == 0 test because a verse can not start with a tag.
                {
                    if (!string.IsNullOrEmpty(tmpTag))
                        tags.Add(tmpTag);
                    tmpTag = string.Empty;
                    tempWord += (string.IsNullOrEmpty(tempWord)) ? verseParts[i] : (" " + verseParts[i]);
                    if (i == verseParts.Length - 1)
                    {
                        // last word
                        words.Add(tempWord);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(tempWord))
                        words.Add(tempWord);
                    tempWord = string.Empty;
                    if (verseParts[i] == "<>")
                    {
                        tmpTag = "<>";
                    }
                    else
                    {
                        tmpTag += (string.IsNullOrEmpty(tmpTag)) ? verseParts[i] : (" " + verseParts[i]);
                        tmpTag = tmpTag.Replace(".", "").Replace("?", "").Trim();
                        if (!string.IsNullOrEmpty(tmpTag))
                        {
                            string[] pts = tmpTag.Split(' ');
                            tmpTag= string.Empty;
                            foreach (string t in pts)
                            {
                                if (t.StartsWith('<'))
                                {
                                    int x = t.IndexOf(">");
                                    if (x > 0)
                                    {
                                        string t1 = t.Substring(1, x - 1);
                                        int len = t1.Length;
                                        if (len == 0) continue;
                                        char last = t1[len - 1];
                                        if (char.IsDigit(last))
                                        {
                                            string t2 = "0000" + t1;
                                            tmpTag += "<" + t2.Substring(t2.Length - 4) + "> ";
                                        }
                                        else
                                        {
                                            string t2 = "0000" + t1;
                                            tmpTag += "<" + t2.Substring(t2.Length - 5) + "> ";
                                        }
                                    }
                                 }
                                else
                                    tmpTag += t + " ";
                            }
                        }
                        tmpTag = tmpTag.Trim();
                        if (i == verseParts.Length - 1)
                        {
                            // last word
                            if (tmpTag.EndsWith('.'))
                                tmpTag.Remove(tmpTag.Length - 1, 1);
                            tags.Add(tmpTag);
                        }
                    }
                }
            }

            if(words.Count == (tags.Count + 1)) // last word was not tagged
                tags.Add(string.Empty);
            string[] vWords = words.ToArray();
            string[] vTags = tags.ToArray();
            if(vWords.Length != vTags.Length)
            {
                throw new Exception(string.Format("Word Count = {0}, Tags Count {1}", vWords, vTags.Length));
            }

            // remove <> from tags
            for (int i = 0; i < vTags.Length; i++)
                vTags[i] = vTags[i].Replace("<", "").Replace(">", "");

            Verse verseWords = new Verse();
            for (int i = 0; i < vWords.Length; i++)
            {
                string[] splitTags = vTags[i].Split(' ');
                verseWords[i] = new VerseWord(vWords[i], splitTags, reference);
            }

            bible.Add(reference, verseWords);
            currentVerseCount++;
            container.UpdateProgress("Loading " + bibleName, (100 * currentVerseCount) / totalVerses);

        }

        public int GetBookIndex(string bookName)
        {
            int index = -1;
            if (bookNamesList.Contains(bookName))
            {
                index = Array.IndexOf(bookNamesList.ToArray(), bookName);

                if (bookNamesList.Count == 27)
                {
                    // we have NT books only
                    index += 39;

                }
            }
            return index;
        }

        public string GetCorrectReference(string reference)
        {
            string correctReference = string.Empty;

            int space = reference.IndexOf(' ');
            string book = reference.Substring(0, space);
            string cv = reference.Substring(space + 1);
            int offset = 0;
            if(bookNamesList.Count == 27)
            {
                // we have NT books only
                offset = 39;

            }
            string correctBook = bookNamesList[Array.IndexOf(Constants.ubsNames, book) -  offset];

            correctReference = string.Format("{0} {1}", correctBook, cv);
            return correctReference;
        }

        #region OSIS Top Version
        /// <summary>
        /// key: book name
        /// value: osis segment representing this book
        /// the first segment is the osis header
        /// </summary>
        private Dictionary<string, string> osisDocSegments = new Dictionary<string, string>();

        /// <summary>
        /// Key:    book name 
        /// Value: Dictionary with:
        ///         Key:    verse reference 
        ///         value:  Verse
        /// </summary>
        private Dictionary<string, Dictionary<string, OsisVerse>> bookTemp = new Dictionary<string, Dictionary<string, OsisVerse>>();

        private int bookCount = 0;
        private object lockingObject = new object();

        /// <summary>
        /// 1. Read all the contents of the OSIS xml as a string
        /// 2. Use regex to get offset of each book and create a book/offset map
        /// 3. GetVerse() takes a verse reference (e.g. Rev.19.14) and uses regex to find the offset and size
        ///    of the verse. (regex starts its search from the offset found in the offsets map)
        /// 4. ParseVerse() takes the verse text enclosed between sID and eID and loads it into a XmlDocument.
        ///    This is because the verse content is well formed xml. This makes it easy to extract the verse's 
        ///    words and Strong's tags.
        /// </summary>
        /// <param name="textFilePath"></param>
        /// <param name="more"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private bool LoadOsisBibleFileInternal(string filePath, bool more)
        {
            bool result = true;
            string currentReference = string.Empty;
            string currentSID = string.Empty;
            Verse verse = null;

            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            try
            {
                container.UpdateProgress("Loading " + bibleName,0);
                string osisDoc = string.Empty;
                using (StreamReader sr = new StreamReader(filePath))
                {
                    osisDoc = sr.ReadToEnd();
                }
                osisDoc = osisDoc.Replace("\r\n", "").Replace("\n", "").Replace("<p>","").Replace("</p>", "");

                osisDoc = RemoveOddTags(osisDoc, "p");
                osisDoc = RemoveOddTags(osisDoc, "lg");
                //osisDoc = RemoveOddTags(osisDoc, "div");
                osisDoc = RemoveOddTags(osisDoc, "l");

                XmlDocument doc = new XmlDocument();
                doc.PreserveWhitespace = false;
                doc.LoadXml(osisDoc);
                XmlElement root = doc.DocumentElement;

                var verses = root.SelectNodes("//verse[@sID]");
                int totalVerses = verses.Count;
                int verseCounter = 0;

                foreach (XmlNode node in verses)
                {
                    container.UpdateProgress("Loading " + bibleName, (100 * verseCounter) / totalVerses);

                    XmlNode xmlNode = node;
                    int wordIndex = 0;
                    while (true)
                    {
                        XmlNode temp1;

                        if (xmlNode.Name == "verse")
                        {
                            if (xmlNode.Attributes["sID"] != null)
                            {
                                verse = new Verse();
                                wordIndex = 0;
                                currentSID = xmlNode.Attributes["sID"].Value;
                                currentReference = OsisUtils.Instance.ChangeReferenceToLocalFormat(xmlNode.Attributes["osisID"].Value);
                            }
                            else if (xmlNode.Attributes["eID"] != null)
                            {
                                string eID = currentSID = xmlNode.Attributes["eID"].Value;
                                if (eID == currentSID)
                                {
                                    bible[currentReference] = verse;
                                    int idx = currentReference.IndexOf(" ");
                                    if (idx != -1)
                                    {
                                        string book = currentReference.Substring(0, idx);
                                        if (!bookNamesList.Contains(book))
                                            bookNamesList.Add(book);
                                    }
                                    verseCounter++;
                                    break;
                                }
                                else
                                {
                                    // error
                                    result = false;
                                }
                            }
                            temp1 = xmlNode.NextSibling;
                            if (temp1 == null)
                            {
                                result = false;
                            }
                            else xmlNode = temp1;

                            continue;
                        }

                        if(xmlNode.Name == "l")
                        {
                            temp1 = xmlNode.NextSibling;
                            if (temp1 == null)
                            {
                                result = false;
                            }
                            else xmlNode = temp1;

                            continue;
                        }
                        else if (xmlNode.Name == "#text")
                        {
                            verse[wordIndex++] = new VerseWord(xmlNode.InnerText, "", currentReference);
                            temp1 = xmlNode.NextSibling;
                            if (temp1 == null)
                            {
                                result = false;
                            }
                            else xmlNode = temp1;

                            continue;
                        }
                        else if (xmlNode.Name == "w")
                        {
                            verse[wordIndex++] = new VerseWord(xmlNode.InnerText,
                                OsisUtils.Instance.GetStrongsFromLemma(xmlNode.Attributes["lemma"].Value).ToArray(),
                                currentReference);

                            temp1 = xmlNode.NextSibling;
                            if (temp1 == null)
                            {
                                result = false;
                            }
                            else xmlNode = temp1;

                            continue;
                        }

                        temp1 = xmlNode.NextSibling;
                        if (temp1 == null)
                        {
                            result = false;
                        }
                        else xmlNode = temp1;

                        continue;
                    }
                }

            }
            catch (Exception ex)
            {
                var cm = System.Reflection.MethodBase.GetCurrentMethod();
                var name = cm.DeclaringType.FullName + "." + cm.Name;
                Tracing.TraceException(name, ex.Message);
                throw;
            }

            if (!more && !(new int[] { 66, 39, 27 }).Contains(bookNamesList.Count))
            {
                Tracing.TraceError(MethodBase.GetCurrentMethod().Name, string.Format("{0}:Book Names Count = {1}. Was expecting 66, 39 or 27",
                                        Path.GetFileName(filePath), bookNamesList.Count));
                return false;
            }


            if (bookNamesList.Count == 66 || bookNamesList.Count == 39)
            {
                for (int i = 0; i < bookNamesList.Count; i++)
                {
                    bookNames.Add(Constants.ubsNames[i], bookNamesList[i]);
                }
            }
            else if (bookNamesList.Count == 27)
            {
                for (int i = 0; i < bookNamesList.Count; i++)
                {
                    bookNames.Add(Constants.ubsNames[i + 39], bookNamesList[i]);
                }
            }


            //var bibleJson = JsonSerializer.Serialize(bible, new JsonSerializerOptions
            //{
            //    Converters = { new JsonBibleConverter() },
            //    WriteIndented = true,
            //});

            //var bibleJson = JsonSerializer.Serialize(bible, new JsonSerializerOptions
            //{
            //    WriteIndented = true,
            //});

            //sw.Stop();
            //long leg1 = sw.ElapsedMilliseconds;
            //sw.Restart();

            //FileStream s = new FileStream(@"C:\temp\Bible.ser",FileMode.Create);
            //BinaryFormatter b = new BinaryFormatter();
            //b.Serialize(s, bible);
            //s.Close();


            //JsonBibleConverter conv = new JsonBibleConverter();
            //string bibleJson = conv.Write(bible);
            //File.WriteAllText(@"C:\temp\Bible.Json", bibleJson);
            //sw.Stop();
            //long leg2 = sw.ElapsedMilliseconds;
            //sw.Restart();

            //FileStream s1 = new FileStream(@"C:\temp\Bible.ser", FileMode.Open);
            //BinaryFormatter b1 = new BinaryFormatter();
            //bible = (Dictionary<string, Verse>)b1.Deserialize(s1);

            //conv.Read(@"C:\temp\Bible.Json", bible, bookNames, bookNamesList);
            //sw.Stop();
            //long leg3 = sw.ElapsedMilliseconds;

            return result;
        }

        private string RemoveOddTags(string VerseXml, string tag)
        {
            string sPattern = @"(<" + tag + @"[^>]*>)";
            if (tag == "div" || tag == "l")
                sPattern = string.Format(@"(<{0}\s[^>]+>)", tag);

            string ePattern = @"(</" + tag + @"[^>]*>)";
            string result = string.Empty;

            int startIndex = 0;
            try
            {
                result = Regex.Replace(VerseXml, sPattern, "");
                result = Regex.Replace(result, ePattern, "");
            }
            catch (Exception ex)
            {
                var cm = System.Reflection.MethodBase.GetCurrentMethod();
                var name = cm.DeclaringType.FullName + "." + cm.Name;
                Tracing.TraceException(name, ex.Message);
                throw;
            }
            return result;
        }

        private void ProcessNodeL()
        {

        }

        private void BookLoaded(string book, Dictionary<string, OsisVerse> localBible)
        {
            lock (lockingObject)
            {
                bookCount++;
                container.UpdateProgress("Loading " + bibleName, (100 * bookCount) / 66);
                bookTemp[book] = localBible;
            }
        }

        private void LoadBookFromSegments(Object threadContext) //string book)
        {
            string book = (string)threadContext;

            Dictionary<string, OsisVerse> localBible = new Dictionary<string, OsisVerse>();
            try
            {
                Regex regex = new Regex(
                    string.Format(@"<verse\s*sID\=""(.*)""\s*osisID\=""{0}\.([0-9]+)\.([0-9]+)""\s*/>", book));

                MatchCollection VerseMatches = regex.Matches(osisDocSegments[book]);
                if (VerseMatches.Count > 0)
                {
                    foreach (Match VerseMatch in VerseMatches)
                    {
                        OsisVerse? osisVerse = OsisUtils.Instance.GetVersesTags(book, osisDocSegments[book], VerseMatch);
                        if (osisVerse != null)
                        {
                            //lock (this)
                            {
                                localBible.Add(osisVerse.VerseRefX, osisVerse);
                            }
                            // Verse verseWords = osisVerse.GetVerseWords();
                            // bible.Add(osisVerse.VerseRefX, verseWords);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var cm = System.Reflection.MethodBase.GetCurrentMethod();
                var name = cm.DeclaringType.FullName + "." + cm.Name;
                Tracing.TraceException(name, ex.Message);
                throw;
            }
            finally
            {
                BookLoaded(book, localBible);
            }
        }

        private void BuildOsisSegments(string osisDoc)
        {
            osisDocSegments.Clear();
            try
            {
                while (true)
                {
                    // <div osisID="Gen" type="book">
                    Regex regex = new Regex(@"<div\s*osisID\=""([1-9A-Za-z]*)""\s*type\=""book"">");
                    MatchCollection matches = regex.Matches(osisDoc);
                    if (matches.Count > 0)
                    {
                        string book = "header";
                        int start = 0;

                        foreach (Match match in matches)
                        {
                            int offset = match.Index;
                            try
                            {
                                osisDocSegments.Add(book, osisDoc.Substring(start, offset - start));
                            }
                            catch (Exception ex)
                            {
                                var cm = System.Reflection.MethodBase.GetCurrentMethod();
                                var name = cm.DeclaringType.FullName + "." + cm.Name;
                                Tracing.TraceException(name, ex.Message);
                                throw;
                            }

                            start = offset;
                            book = match.Groups[1].Value;
                        }
                        osisDocSegments.Add(book, osisDoc.Substring(start));
                        break;
                    }
                    else
                    {
                        throw new Exception("Could not identify any book in the document");
                    }
                }
            }
            catch (Exception ex)
            {
                var cm = System.Reflection.MethodBase.GetCurrentMethod();
                var name = cm.DeclaringType.FullName + "." + cm.Name;
                Tracing.TraceException(name, ex.Message);
                throw;
            }
        }

        #endregion OSIS Top Version



    }
}
