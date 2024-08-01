using BibleTaggingUtil.Strongs;
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

        // dictionary for words to ignore the TR meaningVar
        // key ref
        // val word #
        Dictionary<string, int[]> ignoreTR = new Dictionary<string, int[]>()
        {
            { "Mrk 4:21", new int[]{22} },
            { "Mrk 9:40", new int[]{5,7} },
            { "Luk 5:1", new int[]{8} },
            { "Luk 5:25", new int[]{7} },
            { "Luk 6:8", new int[]{10} },
            { "Luk 6:9", new int[]{8} },
            { "Luk 6:3", new int[]{13} },
            { "Luk 7:33", new int[]{5} },
            { "Luk 8:6", new int[]{2} },
            { "Luk 13:24", new int[]{5} },
            { "Luk 17:2", new int[]{3,4} },
            { "Jhn 12:2", new int[]{17} },
            { "Jhn 18:30", new int[]{9} },
            { "Jhn 18:34", new int[]{5} },
            { "Act 16:17", new int[]{19} },
            { "Act 17:27", new int[]{2} },
            { "Act 21:8", new int[]{8} },
            { "Act 23:16", new int[]{7,8} },
            { "Act 24:13", new int[]{0,3} },
            { "Rom 12:11", new int[]{8} },
            { "2Co 11:10", new int[]{10} },
            { "Php 3:13", new int[]{3} },
            { "Php 4:3", new int[]{0} },
            { "1Ti 6:17", new int[]{15} },
            { "1Pe 1:4", new int[]{11} },
            { "2Pe 2:12", new int[]{5} },
            { "Jud 1:24", new int[]{4} },
            { "Rev 7:4", new int[]{9} },
            { "Rev 7:5", new int[]{ 3, 9, 15 } }, 
            { "Rev 7:6", new int[]{ 3, 9, 15 } }, 
            { "Rev 7:7", new int[]{ 3, 9, 15 } }, 
            { "Rev 7:8", new int[]{ 3, 9, 15 } }, 
            { "Rev 11:11", new int[]{ 13 } },
            { "Rev 12:7", new int[]{ 14 } },
            { "Rev 13:18", new int[]{ 24 } } 
        };

        override protected void ParseLine(string line)
        {
            try
            {
                ParseLine4(line);
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

                    if(verseRef == "Heb 10:34")
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
                    string dictForm = dictParts[0];
                    string dictGloss = dictParts.Length > 1 ? dictParts[1] : string.Empty;

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
                    //if(currentVerseRef == "Php 4:3" && wordNumber == 0)
                    if (ignoreTR.ContainsKey(currentVerseRef) && ignoreTR[currentVerseRef].Contains(wordNumber))
                    {
                        meaningVar = string.Empty;
                    }
                    bible[currentVerseRef][wordNumber] = new VerseWord(greek, english, new StrongsCluster(strongsList), transliteration, currentVerseRef, grammar, dStrong, wordType, altVerseNum, wordNum, meaningVar: meaningVar, dictForm:dictForm,dictGloss:dictGloss);
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
