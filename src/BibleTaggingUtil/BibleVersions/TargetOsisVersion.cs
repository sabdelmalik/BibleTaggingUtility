using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

using BibleTaggingUtil.OsisXml;

namespace BibleTaggingUtil.BibleVersions
{
    /// <summary>
    /// 1. Read all the contents of the OSIS xml as a string
    /// 2. Use regex to get offset of each book and create a book/offset map
    /// 3. GetVerse() takes a verse reference (e.g. Rev.19.14) and uses regex to find the offset and size
    ///    of the verse. (regex starts its search from the offset found in the offsets map)
    /// 4. ParseVerse() takes the verse text enclsed between sID and eID and loads it into a XmlDocument.
    ///    This is because the verse content is well formed xml. This makes it easy to extract the verse's 
    ///    words and Strong's tags.
    /// 5. Once the tags has been update, the verse is reconstructed and is replaced in the text
    /// 6. Finally the text is saved.
    /// </summary>
    public class TargetOsisVersion : BibleVersion
    {
        /// <summary>
        /// The complete OSIS XML file is read into this string
        /// </summary>
        private string osisDoc = string.Empty;

        private Dictionary<string, string> osisDocSegments = new Dictionary<string, string>();
        
        /// <summary>
        /// Key:    Bible book name 
        /// value:  Offset of the bookwithin the osisDoc string
        /// The offsets speeds up the regex search for verses
        /// </summary>
//        private Dictionary<string, int> bookOffsets = new Dictionary<string, int>();
        private Dictionary<string, Dictionary<string, OsisVerse>> bookTemp = new Dictionary<string, Dictionary<string, OsisVerse>>();

        /// <summary>
        /// Key:    Verse reference in the format Gen 1:1 
        /// value:  The parsed OSIS content of the verse
        private Dictionary<string, OsisVerse> osisBible = new Dictionary<string, OsisVerse>();

        public TargetOsisVersion(BibleTaggingForm container) : base(container, 31104) { }


        protected override bool LoadBibleFileInternal(string textFilePath, bool more)
        {
            var cm = System.Reflection.MethodBase.GetCurrentMethod();
            var name = cm.DeclaringType.FullName + "." + cm.Name;
            Tracing.TraceEntry(name, textFilePath, more);

            bool result = false;
            if (File.Exists(textFilePath))
            {
                try
                {
                    //ThreadPool.SetMinThreads(8, 8);

                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    LoadT(textFilePath);
                    stopwatch.Stop();
                    long elapsed_time = stopwatch.ElapsedMilliseconds;

                    foreach (OsisVerse osisVerse in osisBible.Values)
                    {
                        Verse verseWords = osisVerse.GetVerseWords();
                        bible.Add(osisVerse.VerseRefX, verseWords);
                    }


                    //bookNamesList = bookOffsets.Keys.ToList();
                    bookNamesList = new List<string>();
                    bookNamesList.AddRange(osisDocSegments.Keys.Skip(1));

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

                    result = true;
                }
                catch (Exception ex)
                {
                    Tracing.TraceException(name, ex.Message);
                    throw;
                }
            }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        private void Load(string fileName)
        {
            // Read the complete OSIS XML file
            //osisDoc = File.ReadAllText(fileName);
            using(StreamReader sr = new StreamReader(fileName))
            {
                osisDoc = sr.ReadToEnd();
            }

            // Build the book's offsets map
            //            BuildBookOffsets();
            BuildOsisSegments();

            // Create an OSIS Bible Map
            //foreach (string book in bookOffsets.Keys)
            foreach (string book in osisDocSegments.Keys)
            {
                Regex regex = new Regex(
                    string.Format(@"<verse\s*sID\=""(.*)""\s*osisID\=""{0}\.([0-9]+)\.([0-9]+)""\s*/>", book));

//                MatchCollection VerseMatches = regex.Matches(osisDoc, bookOffsets[book]);
                MatchCollection VerseMatches = regex.Matches(osisDocSegments[book]);
                if (VerseMatches.Count > 0)
                {
                    foreach (Match VerseMatch in VerseMatches)
                    {
                        OsisVerse? osisVerse = GetVersesTags(book, VerseMatch);
                        if (osisVerse != null)
                        {
                            osisBible.Add(osisVerse.VerseRefX, osisVerse);
                            //Verse verseWords = osisVerse.GetVerseWords();
                            //bible.Add(osisVerse.VerseRefX, verseWords);
                        }
                    }
                }
            }
        }

        private void LoadT(string fileName)
        {
            try
            {
                // Read the complete OSIS XML file
                //osisDoc = File.ReadAllText(fileName);
                using (StreamReader sr = new StreamReader(fileName))
                {
                    osisDoc = sr.ReadToEnd();
                }

                // Build the book's offsets map
                //BuildBookOffsets();
                BuildOsisSegments();

                bookCount = 0;
                container.UpdateProgress("Loading " + bibleName, (100 * bookCount) / 66);
                // Create an OSIS Bible Map
                //foreach (string book in bookOffsets.Keys)
                //{
                //    //new Thread(() => { LoadBook(book); }).Start();
                //    ThreadPool.QueueUserWorkItem(LoadBook, book);
                //}
                //while (bookCount < bookOffsets.Keys.Count) ;

                foreach (string book in osisDocSegments.Keys.Skip(1))
                {
                    //new Thread(() => { LoadBook(book); }).Start();
                    ThreadPool.QueueUserWorkItem(LoadBookFromSegments, book);
                }
                while (bookCount < osisDocSegments.Keys.Count-1) ;

                foreach(string book in osisDocSegments.Keys.Skip(1))
                {
                    osisBible = osisBible.Concat(bookTemp[book]).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                }
            }
            catch(Exception ex)
            {
                var cm = System.Reflection.MethodBase.GetCurrentMethod();
                var name = cm.DeclaringType.FullName + "." + cm.Name;
                Tracing.TraceException(name, ex.Message);
                throw;
            }
        }

        object lockingObject = new object();
        int bookCount = 0;
        private void BookLoaded(string book, Dictionary<string, OsisVerse> localBible)
        {
            lock (lockingObject)
            {
                bookCount++;
                container.UpdateProgress("Loading " + bibleName, (100 * bookCount) / 66);
                bookTemp[book] = localBible;
            }
        }
        //private void LoadBook(Object threadContext) //string book)
        //{
        //    string book = (string)threadContext;

        //    Dictionary<string, OsisVerse> localBible = new Dictionary<string, OsisVerse>();
        //    try
        //    {
        //        Regex regex = new Regex(
        //            string.Format(@"<verse\s*sID\=""(.*)""\s*osisID\=""{0}\.([0-9]+)\.([0-9]+)""\s*/>", book));

        //        MatchCollection VerseMatches = regex.Matches(osisDoc, bookOffsets[book]);
        //        if (VerseMatches.Count > 0)
        //        {
        //            foreach (Match VerseMatch in VerseMatches)
        //            {
        //                OsisVerse? osisVerse = GetVersesTags(book, VerseMatch);
        //                if (osisVerse != null)
        //                {
        //                    //lock (this)
        //                    {
        //                        localBible.Add(osisVerse.VerseRefX, osisVerse);
        //                    }
        //                    // Verse verseWords = osisVerse.GetVerseWords();
        //                    // bible.Add(osisVerse.VerseRefX, verseWords);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        var cm = System.Reflection.MethodBase.GetCurrentMethod();
        //        var name = cm.DeclaringType.FullName + "." + cm.Name;
        //        Tracing.TraceException(name, ex.Message);
        //    }
        //    finally
        //    {
        //        BookLoaded(book, localBible);
        //    }
        //}
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
                        OsisVerse? osisVerse = GetVersesTags(book, VerseMatch);
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

        private OsisVerse? GetVersesTags(string book, Match VerseMatch)
        {
            OsisVerse result = null;

            int startIndex = VerseMatch.Index + VerseMatch.Groups[0].Length;
            string sID = VerseMatch.Groups[1].Value;
            string eID = string.Empty;
            string verseRef = string.Format("{0}.{1}.{2}", book, VerseMatch.Groups[2].Value, VerseMatch.Groups[3].Value);
            string verseXML = string.Empty;
            int endIndex = 0;

            if (verseRef == "Gen.2.7")
            {
                int x = 0;
            }

            try
            {
                Regex regex = new Regex(string.Format(@"<verse\s*eID\=""{0}""\s*/>", sID));
//                VerseMatch = regex.Match(osisDoc, startIndex);
                VerseMatch = regex.Match(osisDocSegments[book]);
                if (VerseMatch.Success)
                {
                    endIndex = VerseMatch.Index;
                    eID = VerseMatch.Groups[1].Value;
                }

                string verseXml = string.Empty;
                if (endIndex > startIndex)
                    verseXml = osisDocSegments[book].Substring(startIndex, endIndex - startIndex);
                //                    verseXml = osisDoc.Substring(startIndex, endIndex - startIndex);

                result = new OsisVerse(verseRef, startIndex, sID, verseXml, eID);
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

        internal OsisVerse GetVerse(string verseRef)
        {
            OsisVerse verseXML = null;

            //string verse = @"<verse sID=""Gen.1.26"" osisID=""Gen.1.26"" />Then <w lemma='strong:H0430' src='024'>God</w> <w lemma='strong:H0559' src='014'>said</w>, <hi type=""normal"" /><note osisID=""Gen.1.26!crossReference.o"" osisRef=""Gen.1.26"" n=""o"" type=""crossReference""><reference osisRef=""Gen.3.22"">ch. 3:22</reference>; <reference osisRef=""Gen.11.7"">11:7</reference>; <reference osisRef=""Isa.6.8"">Isa. 6:8</reference></note><q marker=""“"" level=""1"" sID=""01001026.1"" />Let us <w lemma='strong:H6213!a' src='034'>make</w> <w lemma='strong:H0120' src='044'>man</w><note n=""1"" osisID=""Gen.1.26!note.1"" osisRef=""Gen.1.26"" subType=""x-name-meaning"" type=""explanation"">The Hebrew word for <catchWord><hi type=""italic"">man</hi></catchWord> (<hi type=""italic"">adam</hi>) is the generic term for mankind and becomes the proper name <hi type=""italic"">Adam</hi></note> in our <w lemma='strong:H6754 strong:H9003' src='054 051'>image</w>, <hi type=""normal"" /><note osisID=""Gen.1.26!crossReference.p"" osisRef=""Gen.1.26"" n=""p"" type=""crossReference""><reference osisRef=""Gen.5.1"">ch. 5:1</reference>; <reference osisRef=""Gen.9.6"">9:6</reference>; <reference osisRef=""1Cor.11.7"">1 Cor. 11:7</reference>; <reference osisRef=""Eph.4.24"">Eph. 4:24</reference>; <reference osisRef=""Col.3.10"">Col. 3:10</reference>; <reference osisRef=""Jas.3.9"">James 3:9</reference></note>after our <w lemma='strong:H1823 strong:H9025 strong:H9004' src='064 056 061'>likeness</w>. And <note osisID=""Gen.1.26!crossReference.q"" osisRef=""Gen.1.26"" n=""q"" type=""crossReference""><reference osisRef=""Gen.9.2"">ch. 9:2</reference>; <reference osisRef=""Ps.8.6-Ps.8.8"">Ps. 8:6-8</reference>; <reference osisRef=""Jas.3.7"">James 3:7</reference></note>let them have <w lemma='strong:H7287!a strong:H9025' src='074 066'>dominion</w> over the <w lemma='strong:H1710 strong:H9003' src='084 081'>fish</w> of the <w lemma='strong:H3220 strong:H9009' src='094 091'>sea</w> and over the <w lemma='strong:H5775 strong:H9002 strong:H9003' src='104 100 101'>birds</w> of the <w lemma='strong:H8064 strong:H9009' src='114 111'>heavens</w> and over the <w lemma='strong:H0929 strong:H9002 strong:H9003' src='124 120 121'>livestock</w> and over <w lemma='strong:H3605 strong:H9002 strong:H9003' src='134 130 131'>all</w> the <w lemma='strong:H0776 strong:H9009' src='144 141'>earth</w> and over <w lemma='strong:H3605 strong:H9002 strong:H9003' src='154 150 151'>every</w> <w lemma='strong:H7431 strong:H9009' src='164 161'>creeping thing</w> that <w lemma='strong:H7430 strong:H9009' src='174 171'>creeps</w> on the <w lemma='strong:H0776 strong:H5921!a strong:H9009' src='194 184 191'>earth</w>.<q marker=""”"" level=""1"" eID=""01001026.1"" /><verse eID=""Gen.1.26"" />";
            verseRef = "Rev.19.14";

            int bookStart = -1;
            string book = string.Empty;
            Match m = Regex.Match(verseRef, @"([1-9A-Za-z]*)\.(\d{1,3})\.(\d{1,3})");
            if (m != null)
            {
                book = m.Groups[1].Value;
                string chapter = m.Groups[2].Value;
                string verse = m.Groups[3].Value;

                //bookStart = bookOffsets[book];
            }


            string pattern1 = string.Format(
                @"<verse\s*sID\=""(.*)""\s*osisID\=""{0}""\s*/>", verseRef);

            var regex = new Regex(pattern1);
            int startIndex = 0;
            int endIndex = 0;
            string sID = string.Empty;
            string eID = string.Empty;

            //Match match = regex.Match(osisDoc, bookStart);
            Match match = regex.Match(osisDocSegments[book]);
            if (match.Success)
            {
                startIndex = match.Index + match.Groups[0].Length;
                sID = match.Groups[1].Value;
            }

            string pattern2 = string.Format(
                @"<verse\s*eID\=""{0}""\s*/>", sID);
            regex = new Regex(pattern2);
            //match = regex.Match(osisDoc, startIndex);
            match = regex.Match(osisDocSegments[book]);
            if (match.Success)
            {
                endIndex = match.Index;
                eID = match.Groups[1].Value;
            }

            string osisVerse = string.Empty;
            if (endIndex > startIndex)
            {
                //osisVerse = osisDoc.Substring(startIndex, endIndex - startIndex);
                osisVerse = osisDocSegments[book].Substring(startIndex, endIndex - startIndex);
                verseXML = new OsisVerse(verseRef, osisVerse.Length, sID, osisVerse, eID);
            }

            return verseXML;
        }

        private void ParseVerse(string verse)
        {
            //string verse = @"<w lemma='strong:G2532'>And</w> the <w lemma='strong:G4753'>armies</w> <w lemma='strong:G3588 strong:G1722'>of</w> <w lemma='strong:G3772'>heaven</w>, <hi type=""normal"" /><note osisID=""Rev.19.14!crossReference.s"" osisRef=""Rev.19.14"" n=""s"" type=""crossReference""><reference osisRef=""Rev.3.4"">ch. 3:4</reference>; <reference osisRef=""Rev.7.9"">7:9</reference></note><w lemma='strong:G1746'>arrayed</w> in <w lemma='strong:G1039'>fine linen</w>, <w lemma='strong:G3022'>white</w> <w lemma='strong:G2532'>and</w> <w lemma='strong:G2513'>pure</w>, <hi type=""normal"" /><note osisID=""Rev.19.14!crossReference.t"" osisRef=""Rev.19.14"" n=""t"" type=""crossReference"">[<reference osisRef=""Rev.14.20"">ch. 14:20</reference>]</note>were <w lemma='strong:G0190'>following</w> <w lemma='strong:G0846'>him</w> <w lemma='strong:G1909'>on</w> <w lemma='strong:G3022'>white</w> <w lemma='strong:G2462'>horses</w>.";
            string header = @"<?xml version=""1.0"" encoding=""UTF-8""?><osis>";
            string trailer = @"</osis>";

            string verseXML = header + verse + trailer;

            XmlDocument document = new XmlDocument();
            document.LoadXml(verseXML);
            //XPathNavigator? nav = document.CreateNavigator();

            XmlElement root = document.DocumentElement;
            foreach (XmlNode node in root.ChildNodes)
            {
                string name = node.Name;
                List<string> strongs = new List<string>();
                if (name == "#text")
                {
                    string word = node.InnerText;
                }
                else if (name == "w")
                {
                    string word = node.InnerText;
                    XmlAttributeCollection? attrib = node.Attributes;
                    if (attrib != null)
                    {
                        foreach (XmlAttribute attr in attrib)
                        {
                            if (attr.Name == "lemma")
                            {
                                strongs = GetStrongs(attr.Value);
                            }
                        }
                    }

                }

            }
        }

        private List<string> GetStrongs(string value)
        {
            List<string> strings = new List<string>();
            MatchCollection matches = Regex.Matches(value, @"strong:([GH]\d\d\d\d)");
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    strings.Add(match.Groups[1].Value);
                }
            }

            return strings;
        }

        internal void Save(string fileName)
        {
            var cm = System.Reflection.MethodBase.GetCurrentMethod();
            var name = cm.DeclaringType.FullName + "." + cm.Name;
            try
            {
                Tracing.TraceEntry(name);
                lock (this)
                {
                    if (!container.EditorPanel.TargetDirty)
                        return;

                    container.WaitCursorControl(true);
                    container.EditorPanel.TargetDirty = false;
                    container.EditorPanel.SaveCurrentVerse();

                    // 1.Update osisBible' dirty fkag from Bible
                    foreach (string verseRef in Bible.Keys)
                    {
                        Verse v = Bible[verseRef];
                        if (v.Dirty)
                        {
                            //osisBible[verseRef].UpdateVerse(v);
                            osisBible[verseRef].Dirty = true;
                        }
                    }

                    // 2. Update osisDoc from osisBible
                    foreach (OsisVerse osisVerse in osisBible.Values)
                    {
                        if (osisVerse.Dirty)
                        {
                            osisDocSegments[osisVerse.Book] = osisDocSegments[osisVerse.Book]
                                             .Remove(osisVerse.StartIndex, osisVerse.Length)
                                             .Insert(osisVerse.StartIndex, osisVerse.VerseCompleteXml);

                            int diff = osisVerse.VerseCompleteXml.Length - osisVerse.Length;
                            if (diff != 0)
                            {
                                osisVerse.UpdateVerseXml(osisVerse.VerseCompleteXml);
                                UpdateIndices(osisVerse.Book);
                            }
                        }
                    }

                    // rebuild Document
                    osisDoc = string.Empty;
                    foreach(string seg  in osisDocSegments.Values)
                    {
                        osisDoc += seg;
                    }

                    // construce Updates fileName
                    string taggedFolder = Path.GetDirectoryName(container.Config.TaggedBible);
                    string oldTaggedFolder = Path.Combine(taggedFolder, "OldTagged");
                    if (!Directory.Exists(oldTaggedFolder))
                        Directory.CreateDirectory(oldTaggedFolder);

                    // move existing tagged files to the old folder
                    String[] existingTagged = Directory.GetFiles(taggedFolder, "*.*");
                    foreach (String existingTaggedItem in existingTagged)
                    {
                        string fName = Path.GetFileName(existingTaggedItem);
                        string src = Path.Combine(taggedFolder, fName);
                        string dst = Path.Combine(oldTaggedFolder, fName);
                        if (System.IO.File.Exists(dst))
                            System.IO.File.Delete(src);
                        else
                            System.IO.File.Move(src, dst);
                    }

                    string baseName = Path.GetFileNameWithoutExtension(container.Config.TaggedBible);
                    string updatesFileName = string.Format("{0:s}_{1:s}.xml", baseName, DateTime.Now.ToString("yyyy_MM_dd_HH_mm"));

                    File.WriteAllText(Path.Combine(taggedFolder, updatesFileName), osisDoc);
                }
            }
            catch (Exception ex)
            {
                Tracing.TraceException(name, ex.Message);
                throw;
            }
            container.WaitCursorControl(false);
        }

        private void UpdateIndices(string book)
        {
            try
            {
                Regex regex = new Regex(
                    string.Format(@"<verse\s*sID\=""(.*)""\s*osisID\=""{0}\.([0-9]+)\.([0-9]+)""\s*/>", book));

                MatchCollection VerseMatches = regex.Matches(osisDocSegments[book]);
                if (VerseMatches.Count > 0)
                {
                    foreach (Match VerseMatch in VerseMatches)
                    {
                        int startIndex = VerseMatch.Index + VerseMatch.Groups[0].Length;
                        string verseRefX = string.Format("{0} {1}:{2}", book, VerseMatch.Groups[2].Value, VerseMatch.Groups[3].Value);

                        OsisVerse? osisVerse = osisBible[verseRefX];
                        osisVerse.UpdateVerseStartIndex(startIndex);
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

        //private void UpdateBook(Object threadContext) //string book)
        //{
        //    string book = (string)threadContext;

        //    try
        //    {
        //        Regex regex = new Regex(
        //            string.Format(@"<verse\s*sID\=""(.*)""\s*osisID\=""{0}\.([0-9]+)\.([0-9]+)""\s*/>", book));

        //        MatchCollection VerseMatches = regex.Matches(osisDocSegments[book]);
        //        if (VerseMatches.Count > 0)
        //        {
        //            foreach (Match VerseMatch in VerseMatches)
        //            {
        //                int startIndex = VerseMatch.Index + VerseMatch.Groups[0].Length;
        //                string verseRefX = string.Format("{0} {1}:{2}", book, VerseMatch.Groups[2].Value, VerseMatch.Groups[3].Value);

        //                OsisVerse? osisVerse = osisBible[verseRefX];
        //                osisVerse.UpdateVerseStartIndex(startIndex);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        var cm = System.Reflection.MethodBase.GetCurrentMethod();
        //        var name = cm.DeclaringType.FullName + "." + cm.Name;
        //        Tracing.TraceException(name, ex.Message);
        //    }
        //    finally
        //    {
        //        BookUpdated();
        //    }
        //}


        //private void BuildBookOffsets()
        //{
        //    bookOffsets.Clear();
        //    while (true)
        //    {
        //        // <div osisID="Gen" type="book">
        //        Regex regex = new Regex(@"<div\s*osisID\=""([1-9A-Za-z]*)""\s*type\=""book"">");
        //        MatchCollection matches = regex.Matches(osisDoc);
        //        if (matches.Count > 0)
        //        {
        //            foreach (Match match in matches)
        //            {
        //                int offset = match.Index;
        //                string book = match.Groups[1].Value;
        //                bookOffsets[book] = offset;
        //                bookTemp[book] = null;
        //            }
        //            break;
        //        }
        //    }
        //}
        private void BuildOsisSegments()
        {
            osisDocSegments.Clear();
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
                            osisDocSegments.Add(book, osisDoc.Substring(start, offset-start));
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

    }
}
