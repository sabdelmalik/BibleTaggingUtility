using BibleTaggingUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace BibleTaggingUtil.OsisXml
{
    internal class OsisTag
    {
        private int level = 0;
        private int index = 0;
        private VerseTagType tagType;
        private XmlNode tagXml;
        private VerseWord? verseWord = null;
        private List<OsisTag> osisTags = new List<OsisTag>();

        public OsisTag(int level, int index, VerseTagType tagType, XmlNode tagXML, string verseRef)
        {
            this.tagType = tagType;
            tagXml = tagXML;
            this.level = level;
            this.index = index;

            switch (tagType)
            {
                case VerseTagType.text:
                    CreateUntaggedWord(index, tagXML, verseRef);
                    break;
                case VerseTagType.w:
                    CreateTaggedWord(index, tagXML, verseRef);
                    break;
                case VerseTagType.note:
                case VerseTagType.milestone:
                case VerseTagType.b:
                case VerseTagType.UNKNOWN:
                    break;

                default:
                    {
                        osisTags = OsisUtils.Instance.GetOsisTags(tagXML, verseRef, ++level);
                        //foreach (OsisTag tag in osisTags)
                        //{
                        //    new OsisTag(++level, index, tag.tagType, tag.tagXml, verseRef); ;
                        //}
                    }
                    break;
            }
        }

        private void CreateUntaggedWord(int index, XmlNode tagXML, string verseRef)
        {
            verseWord = new VerseWord(tagXML.InnerText, new string[0], verseRef);
            verseWord.OsisTagIndex = index;
            verseWord.OsisTagLevel = level;
        }
        private void CreateTaggedWord(int index, XmlNode tagXML, string verseRef)
        {
            string word = tagXML.InnerText;

            List<string> strongs = new List<string>();

            XmlAttributeCollection? attrib = tagXML.Attributes;
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

            verseWord = new VerseWord(tagXML.InnerText, strongs.ToArray(), verseRef);
            verseWord.OsisTagIndex = index;
            verseWord.OsisTagLevel = level;
        }

        private List<string> GetStrongs(string value)
        {
            List<string> strings = new List<string>();
            MatchCollection matches = Regex.Matches(value, @"strong:[GH](\d\d\d\d)");
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    strings.Add(match.Groups[1].Value);
                }
            }

            return strings;
        }

        public List<VerseWord> GetVerseWords()
        {
            List<VerseWord> verseWords = new List<VerseWord>();
            switch (tagType)
            {
                case VerseTagType.text:
                case VerseTagType.w:
                    verseWords.Add(verseWord);
                    break;
                default:
                    {
                        if (osisTags.Count > 0)
                        {
                            foreach (OsisTag tag in osisTags)
                            {
                                List < VerseWord > verseWords1 = tag.GetVerseWords();
                                if(verseWords1.Count > 0)
                                    verseWords.AddRange(verseWords1);
                            }
                        }
                    }
                    break;
            }
            return verseWords;
        }
        public override string ToString()
        {
            string result = string.Empty;
            switch(tagType)
            {
                case VerseTagType.text:
                    if (verseWord != null)
                        result = verseWord.Word;
                    break;
                case VerseTagType.w:
                    string lemmaValue = string.Empty;
                    if (verseWord != null)
                    {
                        foreach(string s in verseWord.Strong)
                        {
                            lemmaValue += string.Format("strong:{0}{1} ", verseWord.Testament== BibleTestament.OT? "H":"G", s);
                        }
                        lemmaValue = lemmaValue.Trim();
                        XmlNode tagXml1 = tagXml.Clone();
                        if (tagXml1 != null)
                        {
                            ((XmlElement)tagXml1).SetAttribute("lemma", lemmaValue); // Set to new value.
                            result = tagXml1.OuterXml;
                        }
                    }
                    break;
                case VerseTagType.note:
                case VerseTagType.milestone:
                case VerseTagType.b:
                    result = tagXml.OuterXml;
                    break;

                case VerseTagType.UNKNOWN:
                    string temp = tagXml.OuterXml;
                    string marker = tagXml.InnerXml;
                    int idx = temp.IndexOf(marker);
                    switch(marker)
                    {
                        case "$start$":
                            result= temp.Substring(idx+marker.Length);
                            break;
                        case "$end$":
                            result= temp.Substring(0,idx);
                            break;
                        default:
                            result = tagXml.OuterXml;
                            break;
                    }
                    break;

                default:
                    {
                        string result1 = string.Empty;
                        if (osisTags.Count > 0)
                        {
                            foreach (OsisTag tag in osisTags)
                            {
                                result1 += tag.ToString();
                            }
                            XmlNode tagXmlTemp = tagXml.Clone();
                            tagXmlTemp.InnerXml = result1;
                            result = tagXmlTemp.OuterXml;
                            if (tagType == VerseTagType.l)
                            {
                                int i = result.IndexOf("$end$");
                                if (i > 0)
                                    result = result.Remove(i, "$end$</l>".Length);
                                i = result.IndexOf("$start$");
                                if (i > 0)
                                {
                                    int x = result.LastIndexOf("<l", i);
                                    result = result.Remove(x, i - x + "$start$".Length);
                                }
                            }

                        }
                        else
                            result = tagXml.OuterXml;
                    }
                    break;
            }
            return result;
        }

        internal void Update(VerseWord verseWord)
        {
            if(level == verseWord.OsisTagLevel)
            {

            }
        }

        public XmlNode TagNode
        {
            get
            {
                return tagXml;
            }
        }


    }
}
