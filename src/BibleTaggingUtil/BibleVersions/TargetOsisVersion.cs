﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    internal class TargetOsisVersion : BibleVersion
    {
        /// <summary>
        /// The complete OSIS XML file is read into this string
        /// </summary>
        string osisDoc = string.Empty;

        /// <summary>
        /// Key:    Bible book name 
        /// value:  Offset of the bookwithin the osisDoc string
        /// The offsets speeds up the regex search for verses
        /// </summary>
        private Dictionary<string, int> bookOffsets = new Dictionary<string, int>();

        /// <summary>
        /// Key:    Verse reference in the format Gen 1:1 
        /// value:  The parsed OSIS content of the verse
        protected Dictionary<string, OsisVerse> osisBible = new Dictionary<string, OsisVerse>();

        public TargetOsisVersion(BibleTaggingForm container) : base(container, 31104) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        internal void Read(string fileName)
        {
            // Read the complete OSIS XML file
            osisDoc = File.ReadAllText(fileName);

            // Build the book's offsets map
            BuildBookOffsets();

            // Create an OSIS Bible Map
            foreach (string book in bookOffsets.Keys)
            {
                Regex regex = new Regex(
                    string.Format(@"<verse\s*sID\=""(.*)""\s*osisID\=""{0}\.([0-9]+)\.([0-9]+)""\s*/>", book));

                MatchCollection VerseMatches = regex.Matches(osisDoc, bookOffsets[book]);
                if (VerseMatches.Count > 0)
                {
                    foreach (Match VerseMatch in VerseMatches)
                    {
                        OsisVerse? osisVerse = GetVersesTags(book, VerseMatch);
                        if (osisVerse != null)
                        {
                            osisBible.Add(osisVerse.VerseRefX, osisVerse);
                            if (osisVerse.VerseRef == "Gen.9.26")
                            {
                                int x = 0;
                            }
                            Verse verseWords = osisVerse.GetVerseWords();
                            bible.Add(osisVerse.VerseRefX, verseWords);
                        }
                    }
                }
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
                VerseMatch = regex.Match(osisDoc, startIndex);
                if (VerseMatch.Success)
                {
                    endIndex = VerseMatch.Index;
                    eID = VerseMatch.Groups[1].Value;
                }

                string verseXml = string.Empty;
                if (endIndex > startIndex)
                    verseXml = osisDoc.Substring(startIndex, endIndex - startIndex);

                result = new OsisVerse(verseRef, startIndex, sID, verseXml, eID);
            }
            catch (Exception ex)
            {
                //Trace(string.Format("{0},Error,{1},{2}", sID, ++errorCounter,ex.Message), Color.Red);
                //result = string.Format("{0},Error,{1},{2}", sID, ++errorCounter, ex.Message);
            }

            return result;
        }


        internal OsisVerse GetVerse(string verseRef)
        {
            OsisVerse verseXML = null;

            //string verse = @"<verse sID=""Gen.1.26"" osisID=""Gen.1.26"" />Then <w lemma='strong:H0430' src='024'>God</w> <w lemma='strong:H0559' src='014'>said</w>, <hi type=""normal"" /><note osisID=""Gen.1.26!crossReference.o"" osisRef=""Gen.1.26"" n=""o"" type=""crossReference""><reference osisRef=""Gen.3.22"">ch. 3:22</reference>; <reference osisRef=""Gen.11.7"">11:7</reference>; <reference osisRef=""Isa.6.8"">Isa. 6:8</reference></note><q marker=""“"" level=""1"" sID=""01001026.1"" />Let us <w lemma='strong:H6213!a' src='034'>make</w> <w lemma='strong:H0120' src='044'>man</w><note n=""1"" osisID=""Gen.1.26!note.1"" osisRef=""Gen.1.26"" subType=""x-name-meaning"" type=""explanation"">The Hebrew word for <catchWord><hi type=""italic"">man</hi></catchWord> (<hi type=""italic"">adam</hi>) is the generic term for mankind and becomes the proper name <hi type=""italic"">Adam</hi></note> in our <w lemma='strong:H6754 strong:H9003' src='054 051'>image</w>, <hi type=""normal"" /><note osisID=""Gen.1.26!crossReference.p"" osisRef=""Gen.1.26"" n=""p"" type=""crossReference""><reference osisRef=""Gen.5.1"">ch. 5:1</reference>; <reference osisRef=""Gen.9.6"">9:6</reference>; <reference osisRef=""1Cor.11.7"">1 Cor. 11:7</reference>; <reference osisRef=""Eph.4.24"">Eph. 4:24</reference>; <reference osisRef=""Col.3.10"">Col. 3:10</reference>; <reference osisRef=""Jas.3.9"">James 3:9</reference></note>after our <w lemma='strong:H1823 strong:H9025 strong:H9004' src='064 056 061'>likeness</w>. And <note osisID=""Gen.1.26!crossReference.q"" osisRef=""Gen.1.26"" n=""q"" type=""crossReference""><reference osisRef=""Gen.9.2"">ch. 9:2</reference>; <reference osisRef=""Ps.8.6-Ps.8.8"">Ps. 8:6-8</reference>; <reference osisRef=""Jas.3.7"">James 3:7</reference></note>let them have <w lemma='strong:H7287!a strong:H9025' src='074 066'>dominion</w> over the <w lemma='strong:H1710 strong:H9003' src='084 081'>fish</w> of the <w lemma='strong:H3220 strong:H9009' src='094 091'>sea</w> and over the <w lemma='strong:H5775 strong:H9002 strong:H9003' src='104 100 101'>birds</w> of the <w lemma='strong:H8064 strong:H9009' src='114 111'>heavens</w> and over the <w lemma='strong:H0929 strong:H9002 strong:H9003' src='124 120 121'>livestock</w> and over <w lemma='strong:H3605 strong:H9002 strong:H9003' src='134 130 131'>all</w> the <w lemma='strong:H0776 strong:H9009' src='144 141'>earth</w> and over <w lemma='strong:H3605 strong:H9002 strong:H9003' src='154 150 151'>every</w> <w lemma='strong:H7431 strong:H9009' src='164 161'>creeping thing</w> that <w lemma='strong:H7430 strong:H9009' src='174 171'>creeps</w> on the <w lemma='strong:H0776 strong:H5921!a strong:H9009' src='194 184 191'>earth</w>.<q marker=""”"" level=""1"" eID=""01001026.1"" /><verse eID=""Gen.1.26"" />";
            verseRef = "Rev.19.14";

            int bookStart = -1;
            Match m = Regex.Match(verseRef, @"([1-9A-Za-z]*)\.(\d{1,3})\.(\d{1,3})");
            if (m != null)
            {
                string book = m.Groups[1].Value;
                string chapter = m.Groups[2].Value;
                string verse = m.Groups[3].Value;

                bookStart = bookOffsets[book];
            }


            string pattern1 = string.Format(
                @"<verse\s*sID\=""(.*)""\s*osisID\=""{0}""\s*/>", verseRef);

            var regex = new Regex(pattern1);
            int startIndex = 0;
            int endIndex = 0;
            string sID = string.Empty;
            string eID = string.Empty;

            Match match = regex.Match(osisDoc, bookStart);
            if (match.Success)
            {
                startIndex = match.Index + match.Groups[0].Length;
                sID = match.Groups[1].Value;
            }

            string pattern2 = string.Format(
                @"<verse\s*eID\=""{0}""\s*/>", sID);
            regex = new Regex(pattern2);
            match = regex.Match(osisDoc, startIndex);
            if (match.Success)
            {
                endIndex = match.Index;
                eID = match.Groups[1].Value;
            }

            string osisVerse = string.Empty;
            if (endIndex > startIndex)
            {
                osisVerse = osisDoc.Substring(startIndex, endIndex - startIndex);
                verseXML = new OsisVerse(verseRef, osisVerse.Length, sID, osisVerse, eID);
            }

            return verseXML;
            //osisDoc = osisDoc.Remove(startIndex, osisVerse.Length).Insert(startIndex, osisVerse);

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
            if (osisDoc != null)
            {
                foreach (OsisVerse osisVerse in osisBible.Values)
                {
                    if (osisVerse.Dirty)
                    {
                        osisDoc = osisDoc.Remove(osisVerse.StartIndex, osisVerse.Length)
                                         .Insert(osisVerse.StartIndex, osisVerse.VerseComplteXml);
                    }
                }
                File.WriteAllText(fileName, osisDoc);
            }
        }

        private void BuildBookOffsets()
        {
            bookOffsets.Clear();
            while (true)
            {
                // <div osisID="Gen" type="book">
                Regex regex = new Regex(@"<div\s*osisID\=""([1-9A-Za-z]*)""\s*type\=""book"">");
                MatchCollection matches = regex.Matches(osisDoc);
                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                    {
                        int offset = match.Index;
                        string book = match.Groups[1].Value;
                        bookOffsets[book] = offset;
                    }
                    break;
                }
            }
        }

    }
}