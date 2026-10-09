using BibleTaggingUtil.OsisXml;
using BibleTaggingUtil.Strongs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace BibleTaggingUtil.BibleVersions
{
    public abstract partial class BibleVersion
    {
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
                container.UpdateProgress("Loading " + bibleName, 0);
                string osisDoc = string.Empty;
                using (StreamReader sr = new StreamReader(filePath))
                {
                    osisDoc = sr.ReadToEnd();
                }
                osisDoc = osisDoc.Replace("\r\n", "").Replace("\n", "").Replace("<p>", "").Replace("</p>", "");

                osisDoc = RemoveOddTags(osisDoc, "p");
                osisDoc = RemoveOddTags(osisDoc, "q");
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

                        if (xmlNode.Name == "l")
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
                                new StrongsCluster(
                                    OsisUtils.Instance.GetStrongsFromLemma(xmlNode.Attributes["lemma"].Value).ToArray()),
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
