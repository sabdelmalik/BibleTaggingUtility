using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.AxHost;

namespace BibleTaggingUtil.BibleVersions
{
    internal enum ParseState
    {
        Initial,
        HeaderFound,
        RefLineFound,
        WordFound,
        RefLineContFound,
        WordContFound,
        WordsHeaderFound

    }

    public class ReferenceVersionTAGNT : BibleVersion
    {
        /// <summary>
        /// Verse words dictionary
        /// key: the word number in the verse
        /// Value: a populated VerseWord instance
        /// </summary>
        private Verse verseWords = null;
        private string currentVerseRef = string.Empty;

        string textFilePath = string.Empty;

        ParseState pState = ParseState.Initial;

        public ReferenceVersionTAGNT(BibleTaggingForm container) : base(container, 7959) { }

        string verseReference = string.Empty;
        string verseRef = string.Empty;
        string bookName = string.Empty;

        int verseWordCount = 0;
        int strongsCount = 0;
        int wordsLineCounter = 0;
        List<string> strongList = new List<string>();

        override protected void ParseLine(string line)
        {
            try
            {
                ParseLine4(line);
                return;

                line = line.Trim();
                switch (pState)
                {
                    case ParseState.Initial:
                        if (line.Contains("Reference + word\tWord #"))
                            pState = ParseState.HeaderFound;
                        break;
                    case ParseState.HeaderFound:
                        if (line.StartsWith("#"))
                        {
                            string[] lineparts = line.Substring(2).Split('\t');
                            verseReference = lineparts[0];
                            string[] refParts = verseReference.Split('.');
                            bookName = refParts[0];
                            //                        if (bookName == "Mrk")
                            //                            bookName = "Mar";
                            //                        else if (bookName == "Jhn")
                            //                            bookName = "Joh";
                            verseRef = string.Format("{0} {1}:{2}", bookName, refParts[1], refParts[2]);

                            verseWordCount = lineparts.Length - 1;
                            if (verseReference == "Tit.2.7")
                            {
                                int x = 0;
                            }
                            for (int i = 1; i < lineparts.Length; i++)
                            {
                                if (string.IsNullOrEmpty(lineparts[i].Trim(' ')) || char.IsAscii(lineparts[i].Trim(' ')[0]))
                                    verseWordCount--;
                            }
                            pState = ParseState.RefLineFound;
                        }
                        break;
                    case ParseState.RefLineFound:
                        if (line.StartsWith("#_Word=Grammar"))
                        {
                            verseWords = new Verse();

                            // strong Numbers
                            string[] lineparts = line.Substring(2).Split('\t');
                            strongsCount = lineparts.Length - 1;
                            for (int i = 1; i < lineparts.Length; i++)
                            {
                                if (!lineparts[i].Trim().StartsWith("G"))
                                    strongsCount--;
                                else
                                {
                                    string[] s = lineparts[i].Split('=');
                                    strongList.Add(s[0].Substring(1));
                                }
                            }

                            if (verseWordCount != strongsCount)
                            {
                                throw new Exception("word count mismatch");
                            }
                            pState = ParseState.WordFound;
                        }
                        break;
                    case ParseState.WordFound:
                        if (line.StartsWith("#_" + verseReference))
                        {
                            string[] lineparts = line.Substring(2).Split('\t');
                            verseWordCount += lineparts.Length - 1;
                            for (int i = 1; i < lineparts.Length; i++)
                            {
                                if (char.IsAscii(lineparts[i].Trim()[0]))
                                    verseWordCount--;
                            }
                            pState = ParseState.RefLineContFound;
                        }
                        else if (line.StartsWith("#_REFERENCE"))
                        {
                            pState = ParseState.WordsHeaderFound;
                        }
                        break;
                    case ParseState.RefLineContFound:
                        if (line.StartsWith("#_Word=Grammar"))
                        {
                            // strong Numbers
                            string[] lineparts = line.Substring(2).Split('\t');
                            strongsCount += lineparts.Length - 1;
                            for (int i = 1; i < lineparts.Length; i++)
                            {
                                if (!lineparts[i].StartsWith("G"))
                                    strongsCount--;
                                else
                                {
                                    string[] s = lineparts[i].Split('=');
                                    strongList.Add(s[0].Substring(1));
                                }
                            }
                            if (verseWordCount != strongsCount)
                            {
                                throw new Exception("word count mismatch");
                            }
                            pState = ParseState.WordFound;
                        }
                        break;
                    case ParseState.WordsHeaderFound:
                        if (string.IsNullOrEmpty(line))
                        {
                            if (verseWordCount != wordsLineCounter)
                            {
                                throw new Exception("word lines count mismatch");
                            }

                            string strongsline = strongList[0];
                            for (int i = 1; i < strongList.Count; i++)
                            {
                                strongsline += " " + strongList[i];
                            }

                            //sw.WriteLine(string.Format("{0} {1}:{2} {3}", bookName, refParts[1], refParts[2], strongsline));
                            bible[verseRef] = verseWords;

                            currentVerseCount++;
                            container.UpdateProgress("Loading " + bibleName, (100 * currentVerseCount) / totalVerses);

                            verseReference = string.Empty;
                            verseWordCount = 0;
                            strongsCount = 0;
                            wordsLineCounter = 0;
                            verseWords = null;
                            strongList.Clear();

                            pState = ParseState.HeaderFound;
                        }
                        else
                        {
                            string[] lineParts = line.Split('\t');
                            string strong = lineParts[4].Trim().Substring(1);
                            string[] strongA = new string[1];
                            strongA[0] = strong;
                            string greek = lineParts[2].Trim();
                            string english = lineParts[3].Trim();
                            string morphology = lineParts[5].Trim();

                            verseWords[wordsLineCounter++] = new VerseWord(greek, english, strongA, string.Empty, verseRef, morphology);
                        }
                        break;
                }
            }
            catch(Exception ex)
            {
                var cm = System.Reflection.MethodBase.GetCurrentMethod();
                var name = cm.DeclaringType.FullName + "." + cm.Name;
                Tracing.TraceException(name, ex.Message);
            }
        }

        protected void ParseLine4(string line)
        {
            if (string.IsNullOrEmpty(line))
                return;

            /*
             * Heb (&Eng) Ref & Type	Hebrew	Transliteration	English translation	dStrongs = Lexical = Gloss	Grammar	Meaning Variants	Spelling Variants	Conjoin word	sStrong+Instance	Alt Strongs
             * 
             * Gen.1.1#01=M +T	בְּ/רֵאשִׁ֖ית	be./re.Shit	in/ beginning	H9003=ב=in / {H7225G=רֵאשִׁית=: beginning»first:1_beginning}	HR/Ncfsa			H7225		Gen.1.1-01	Gen.1.1-01	בְּרֵאשִׁית	בְּ/רֵאשִׁ֖ית	HR/Ncfsa	H9003=ב=in/H7225=רֵאשִׁית=first_§1_beginning
             * 
             * [0]	Ref in Heb (Eng) & Type         "Psa.31.24(31.25)#02=L"
             * [1]	Hebrew	                        "וְ/יַאֲמֵ֣ץ"
             * [2]  Transliteration                 "ve./ya.'a.Metz"
             * [3]	Translation                     "so/ may it show strength"
             * [4]	dStrongs                        "H9002/{H0553}"
             * [5]  Grammar                         "HC/Vhj3ms"
             * [6]  Meaning Variants                ""
             * [7]  Spelling Variants               ""
             * [8]  Root dStrong+Instance           "H0553"
             * [9]  Alternative Strongs+Instance    ""
             * [10] Conjoin word                    ""
             * [11]  Expanded Strong tags           "H9002=ו=and/{H0553=אָמֵץ=to strengthen}"
             */

            //            string regexPattern = @"([1-9a-zA-Z]+)\.([0-9]{1,3})\.([0-9]{1,3})\({0,1}[0-9.]*\){0,1}\#([0-9]{0,3})\=([()LQKR])\t([^\t]*)\t([^\t]*)\t([^\t]*)\t([^\t]*)\t([^\t]*)\t([^\t]*)\t([^\t]*)\t([^\t]*)\t([^\t]*)\t([^\t]*)\t([^\t]*)\t";
            //string regexPattern = @"([1-9a-zA-Z]+)\.([0-9]{1,3})\.([0-9]{1,3})\[{0,1}[0-9.]*\]{0,1}\#([0-9]{1,3})\=([()NKO{1,6})\t([^\t]*)\t([^\t]*)\t([^\t]*)\t([^\t]*)\t([^\t]*)\t([^\t]*)\t([^\t]*)\t([^\t]*)\t([^\t]*)\t([^\t]*)\t([^\t]*)\t([^\t]*)";
            string regexPattern = @"([1-9a-zA-Z]+)\.([0-9]{1,3})\.([0-9]{1,3})([[{(]{0,1}[0-9.]*[]})]{0,1})\#([0-9]{1,3})\=([()a-zA-Z]{1,8})\t([^\t]*)\t([^\t]*)\t([^\t]*)\t([^\t]*)\t([^\t]*)\t([^\t]*)\t([^\t]*)\t([^\t]*)\t([^\t]*)\t([^\t]*)\t([^\t]*)"; //\t([^\t]*)";

            try
            {
                line = line.Trim();
                Match match = Regex.Match(line, regexPattern);
                if (match.Success)
                {
                    /*
                     * Slashes (/) separate segments of words in Strongs, Morph, Hebrew, Transliteration and Word-by-word English, 
                     * and (\) separates punctuation in Strongs and Hebrew. 
                     * Strong numbers have the main word in {curly} brackets 
                     * Transliteration is preceded by the Hebrew word number, and marks stress with an upper case, 
                     * Translation has unspoken words in <angled> brackets and added words in [square] brackets
                     */
                    string bookName = match.Groups[1].Value;
                    string chapterNum = match.Groups[2].Value;
                    string verseNum = match.Groups[3].Value;
                    string altVerseNum = match.Groups[4].Value;
                    string wordNum = match.Groups[5].Value;
                    string wordType = match.Groups[6].Value;
                    string greekRaw = match.Groups[7].Value;
                    string english = match.Groups[8].Value;
                    string dStrongGrammer = match.Groups[9].Value;
                    string dictionaryFormGloss = match.Groups[10].Value;
                    string editions = match.Groups[11].Value;
                    string meaningVar = match.Groups[12].Value;
                    string spellingVar = match.Groups[13].Value;
                    string spanish = match.Groups[14].Value;
                    string submeaning = match.Groups[15].Value;
                    string cojoinWord = match.Groups[16].Value;
                    string sStrong = match.Groups[17].Value;
                    string altStrong = match.Groups[18].Value;

                    string verseRef = string.Format("{0} {1}:{2}", bookName,
                            chapterNum.TrimStart('0'),
                            verseNum.TrimStart('0'));

                    if(verseRef == "1Jn 2:14")
                    {
                        int x = 0;
                    }
                    string greek = string.Empty;
                    string transliteration = string.Empty;
                    Match m = Regex.Match(greekRaw, @"([^(]+)\(([^)]+)\)");
                    if (m.Success)
                    {
                        greek = m.Groups[1].Value;
                        transliteration = m.Groups[2].Value;
                    }

                    string dStrong = string.Empty;
                    string grammar = string.Empty;
                    string[] dstrongGrammarParts = dStrongGrammer.Split('+');
                    foreach (string dstrongGrammarPart in dstrongGrammarParts)
                    {
                        string[] dstrongParts = dstrongGrammarPart.Trim().Split('=');
                        dStrong += dstrongParts[0].Trim() + "/";
                        grammar = (dstrongParts.Length > 1 ? dstrongParts[1] : string.Empty) + "/";
                    }
                    dStrong = dStrong.Trim('/');
                    grammar = grammar.Trim('/');

                    string[] strongsList = sStrong.Split(',');
                    for(int i = 0; i < strongsList.Length; i++)
                        strongsList[i] = strongsList[i].Trim();

                    string[] dictParts = dictionaryFormGloss.Split('=');
                    string dictionaryForm = dictParts[0];
                    string gloss = dictParts.Length > 1 ? dictParts[1] : string.Empty;

                    if (string.IsNullOrEmpty(currentVerseRef))
                    {
                        // very first verse
                        currentVerseRef = verseRef;
                        bible[currentVerseRef] = new Verse();
                    }

                    if (verseRef != currentVerseRef)
                    {
                        // we are moving to a new verse
                        // save the completed verse
                        //bible.Add(currentVerseRef, verseWords);
                        currentVerseRef = verseRef;
                        bible[currentVerseRef] = new Verse();

                        currentVerseCount++;
                        container.UpdateProgress("Loading " + bibleName, (100 * currentVerseCount) / totalVerses);
                    }

                    int wordNumber = bible[currentVerseRef].Count;
                    bible[currentVerseRef][wordNumber] = new VerseWord(greek, english, strongsList, transliteration, currentVerseRef, grammar, dStrong, wordType, altVerseNum, wordNum);
                }
                else // not a word line
                    return;


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
