using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace BibleTaggingUtil.BibleVersions
{
    public class ReferenceVersionTOTHT : BibleVersion
    {

        public ReferenceVersionTOTHT(BibleTaggingForm container) : base(container, 23145) { }

        /// <summary>
        /// Verse words dictionary
        /// key: the word number in the verse
        /// Value: a populated VerseWord instance
        /// </summary>
        private Verse verseWords = null;
        private string currentVerseRef = string.Empty;

        /// <summary>
        /// Bible Dictionary
        /// Key: verse reference (xxx c:v) xxx = book name, c = chapter number, v = verse number
        /// </summary>
        // private Dictionary<string, Dictionary<int, VerseWord>> bible = new Dictionary<string, Dictionary<int, VerseWord>>();

        string textFilePath = string.Empty;

        
        enum STATE
        {
            START,
            STRONG,
            HEBREW,
            ENGLISH
        }
        private void SaveUpdates()
        {
            using (StreamWriter outputFile = new StreamWriter(@"C:\temp\toth.txt"))
            {

                for (int i = 0; i < bible.Keys.Count; i++)
                {
                    string[] keys = bible.Keys.ToArray();
                    string verRef = keys[i];


                    try
                    {
                        Verse words = bible[verRef];
                        VerseWord vw = words[0];
                        string verse = vw.Word + " [" + vw.Hebrew + "]";
                        for (int j = 0; j < vw.Strong.Length; j++)
                        {
                            verse += (" <" + vw.Strong[j]) + ">";
                        }
                        for (int k = 1; k < words.Count; k++)
                        {
                            verse += " " + (words[k].Word + " [" + words[k].Hebrew + "]");
                            for (int j = 0; j < words[k].Strong.Length; j++)
                            {
                                verse += (" <" + words[k].Strong[j]) + ">";
                            }
                        }
                        string line = string.Format("{0:s} {1:s}", verRef, verse);
                        outputFile.WriteLine(line);
                    }
                    catch (Exception ex)
                    {
                        Tracing.TraceException(MethodBase.GetCurrentMethod().Name, ex.Message);
                    }


                }


                //for (int i = 0; i < targetVersion.Keys.Count; i++)
                //{
                //    string key = targetVersion.Keys.ToArray()[i];
                //    string line = string.Format("{0:s} {1:s}", key, targetVersion[key]); 
                //    outputFile.WriteLine(line); 
                //}

            }


        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="wordLine"></param>
        override protected void ParseLine(string wordLine)
        {
            ParseLine2(wordLine);
            return;

            if (string.IsNullOrEmpty(wordLine))
                return;

            Match match = Regex.Match(wordLine, @"^\d\d_\w\w\w\.\d+\.\d+#\d+\.?\w?\t");
            if (!match.Success)
                return;

            /*
             * [0]	"00 Ref in Heb"	                "01_Gen.001.001#01"
             * [1]	"KJV ref : source"	            "Gen.001.001#01 : WLT"
             * [2]	"Accents=Pointed+Consonants"	"בְּ/רֵאשִׁ֖ית=בְּרֵאשִׁית+בראשית"
             * [3]	"BSB translation"	            " In the beginning "
             * [4]	"SIL translation"	            "in/ beginning"
             * [5]	"Extended Strongs "	            "H9003#1=ב=in/HN{H7225G|H7225G«H7225#4=רֵאשִׁית=: beginning»first|1_beginning}"
             * [6]	"Alt Strongs of OS or SIL"	    ""
             * [7]	"ETCBC Morph=STEP morph"	    "A 5/¦0¦--¦0¦--¦--¦-¦1¦1¦2¦ 2 = HR/Ncfsa"
             * [8]	"ETCBC Syntax"	                " 2¦ 5¦i¦. ¦.¦Time¦. ¦. = "
             * [9]	"Westminster morph"	            "He Hn-asf"
             * [10]	"OpenScripture morph"	        "Pp/ncfsa"
             * [11]	"Cp with BHS"	                "=בְּרֵאשִׂ֖ית״בְּרֵאשִׁ֖ית״״בְּרֵאשִׁ֖ית"
             * [12]	"Cant from OS"	                "1.0"
             * [13]	"BSB Transliteration\r\n"	    "be·re·Shit"
             */
            string[] lineParts = wordLine.Split('\t');

            // we only consider WLT words
            if (!lineParts[1].EndsWith("WLT"))
                return;

            string verseRef = string.Empty;

            // get verse reference
            // Gen.001.001#01 : WLT
            match = Regex.Match(lineParts[1], @"(\w\w\w)\.(\d+)\.(\d+)#(\d+)\w? : WLT");
            if (!match.Success)
            {
                Console.WriteLine(wordLine);
            }

            verseRef = string.Format("{0} {1}:{2}", match.Groups[1].Value,
                                                    match.Groups[2].Value.TrimStart('0'),
                                                    match.Groups[3].Value.TrimStart('0'));

            if (string.IsNullOrEmpty(currentVerseRef))
            {
                // very first verse
                currentVerseRef = verseRef;
                verseWords = new Verse();
            }

            if (verseRef != currentVerseRef)
            {
                // we are moving to a new verse
                // save the completed verse
                //bible.Add(currentVerseRef, verseWords);
                currentVerseRef = verseRef;
                verseWords = new Verse();

                currentVerseCount++;
                container.UpdateProgress("Loading " + bibleName, (100 * currentVerseCount) / totalVerses);

            }

            // 1. get Hebrew word parts
            string hebrewWord = string.Empty;
            int equalSign = lineParts[2].IndexOf('=');
            if (equalSign != -1)
            {
                hebrewWord = lineParts[2].Substring(0, equalSign);
            }
            string[] hebrewWordParts = hebrewWord.Split('/');
            string[] extendedStrongParts = lineParts[5].Split('/');

            for (int i = 0; i < extendedStrongParts.Length; i++)
            {

                //if (extendedStrongParts[i].Contains("9016"))
                //    continue;
                string englishWord = string.Empty;
                string[] strongRefs = null;
                string hebrew = string.Empty;
                string transliteration = string.Empty;
                List<string> strongList = new List<string>();
                string part = string.Empty;

                string st = extendedStrongParts[i].Trim();
                if (string.IsNullOrEmpty(st)) continue;

                int oc = st.IndexOf("{");
                if (oc != -1)
                {
                    int cc = st.IndexOf("}");
                    if (cc != -1)
                    {
                        st = st.Substring(oc+1, cc-oc-1);
                    }
                    else
                    {
                        Tracing.TraceException(MethodBase.GetCurrentMethod().Name, "No closing curly bracket: " + st);
                        continue;
                    }

                }

                // H6440H|H6440L«H6440#4=פָּנֶה=: (PERSON_eg_'his_face'='him')[face]»face|6_(PERSON_eg_'his_face'='him')[face]
                // uStrong|dStrong«eStrong=HEBREW=Gloss»meaning|1_submeaning
                int oshv = st.IndexOf("«");
                if (oshv != -1)
                {
                    int cshv = st.IndexOf("»");
                    if (cshv != -1)
                    {
                        st = st.Substring(oshv + 1, cshv - oshv - 1);
                    }
                    else
                    {
                        st = st.Substring(oshv + 1);
                    }
                }


                string[] strings = st.Split('=');
                if (strings.Length < 3 && strings.Length > 4)
                {
                    Tracing.TraceException(MethodBase.GetCurrentMethod().Name, "Ext strongs does not have three or four parts: " + st);
                    continue;
                }
                hebrew = strings[1];
                englishWord = strings[2];
                if (strings.Length == 4)
                    englishWord += strings[3];

                // H6440#4
                int hash = strings[0].IndexOf('#');
                if (hash == -1) {
                    Tracing.TraceException(MethodBase.GetCurrentMethod().Name, "estrong does not contain a '#': " + st);
                    continue;
                }
 
                strongList.Add( strings[0].Substring(1,4));

                transliteration = lineParts[13];

                strongRefs = strongList.ToArray();
                try
                {
                    int wordNumber = verseWords.Count;
                    if (englishWord.ToLower() != "verseend" && strongRefs[0] != "9001" && strongRefs[0] != "9014" && strongRefs[0] != "9015")
                    {
                        verseWords[wordNumber] = new VerseWord(hebrew, englishWord, strongRefs, transliteration, currentVerseRef);
                        if (bible.ContainsKey(currentVerseRef))
                            bible[currentVerseRef] = verseWords;
                        else
                            bible.Add(currentVerseRef, verseWords);
                    }
                }
                catch (Exception ex)
                {
                    Tracing.TraceException(MethodBase.GetCurrentMethod().Name, ex.Message);
                }

            }

        }

        protected void ParseLine2(string wordLine)
        {
            if (string.IsNullOrEmpty(wordLine))
                return;

            /*
             * Gen.1.1-01	Gen.1.1-01	בְּרֵאשִׁית	בְּ/רֵאשִׁ֖ית	HR/Ncfsa	H9003=ב=in/H7225=רֵאשִׁית=first_§1_beginning
             * 
             * [0]	"00 Ref in Heb"	                "Gen.1.1-01"
             * [1]	"KJV ref : source"	            "Gen.1.1-01"
             * [2]	"Pointed"                   	"בְּרֵאשִׁית"
             * [3]	"Accented"	                    "בְּ/רֵאשִׁ֖ית"
             * [4]	"Morphology"	                "HR/Ncfsa"
             * [5]	"Extended Strongs "	            "H9003=ב=in/H7225=רֵאשִׁית=first_§1_beginning"
             */

            Match match = Regex.Match(wordLine, @"^\w\w\w\.\d+\.\d+\-\d+\.?\w?\t");
            if (!match.Success)
                return;

            string[] lineParts = wordLine.Split('\t');

            string verseRef = string.Empty;

            // get verse reference
            // Gen.1.1-01
            // word number may be followed by
            // "k" for Ketiv or "q" for Qere or "x" for proposed missing word
            // we only use k (the version found in the main text)
            if (lineParts[1].EndsWith('q') || lineParts[1].EndsWith('x'))
                return;
            match = Regex.Match(lineParts[1], @"(\w\w\w)\.(\d+)\.(\d+)\-(\d+)\w?");
            if (!match.Success)
            {
                Console.WriteLine(wordLine);
            }

            verseRef = string.Format("{0} {1}:{2}", match.Groups[1].Value,
                                                    match.Groups[2].Value.TrimStart('0'),
                                                    match.Groups[3].Value.TrimStart('0'));

            if (string.IsNullOrEmpty(currentVerseRef))
            {
                // very first verse
                currentVerseRef = verseRef;
                verseWords = new Verse();
            }

            if (verseRef != currentVerseRef)
            {
                // we are moving to a new verse
                // save the completed verse
                //bible.Add(currentVerseRef, verseWords);
                currentVerseRef = verseRef;
                verseWords = new Verse();

                currentVerseCount++;
                container.UpdateProgress("Loading " + bibleName, (100 * currentVerseCount) / totalVerses);

            }

            // 1. get Hebrew word parts
            // we use the Accented
            string hebrewWord = lineParts[3];
            string[] hebrewWordParts = hebrewWord.Split('/');
            string[] extendedStrongParts = lineParts[5].Split('/');

            for (int i = 0; i < extendedStrongParts.Length; i++)
            {
                string englishWord = string.Empty;
                string hebrew = string.Empty;
                string[] strongRefs = null;

                List<string> strongList = new List<string>();
                string part = string.Empty;

                string st = extendedStrongParts[i].Trim();
                if (string.IsNullOrEmpty(st)) continue;

                string[] strings = st.Split('=');
                if (strings.Length < 3 && strings.Length > 4)
                {
                    Tracing.TraceException(MethodBase.GetCurrentMethod().Name, "Ext strongs does not have three or four parts: " + st);
                    continue;
                }
                hebrew = strings[1];
                englishWord = strings[2];
                int at = englishWord.IndexOf('@');
                if (at > 0)
                    englishWord = englishWord.Substring(0, at);
                //if (strings.Length == 4)
                //    englishWord += strings[3];

                strongList.Add(strings[0].Substring(1, 4));

                strongRefs = strongList.ToArray();
                try
                {
                    int wordNumber = verseWords.Count;
                    if (englishWord.ToLower() != "verseend" && strongRefs[0] != "9001" && strongRefs[0] != "9014" && strongRefs[0] != "9015")
                    {
                        verseWords[wordNumber] = new VerseWord(hebrew, englishWord, strongRefs, "", currentVerseRef);
                        if (bible.ContainsKey(currentVerseRef))
                            bible[currentVerseRef] = verseWords;
                        else
                            bible.Add(currentVerseRef, verseWords);
                    }
                }
                catch (Exception ex)
                {
                    Tracing.TraceException(MethodBase.GetCurrentMethod().Name, ex.Message);
                }

            }

        }
    }
}
        
