using BibleTaggingUtil.BibleVersions;
using BibleTaggingUtil.Strongs;
using BibleTaggingUtil.TranslatorsTags;
using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BibleTaggingUtil.TranslationTags
{
    public partial class TranslationTagsEx
    {
        private Dictionary<string, List<TranslatorWord>> ntWords = new Dictionary<string, List<TranslatorWord>>();
        private List<string> ntErrors = new List<string>();

        /// <summary>
        /// key: verse Ref
        /// Value: int array :  startWord, length, grkVerse#, grkStartWord, grkLength
        /// </summary>
        private Dictionary<string, List<int[]>> ntVerseMap;
        private void ProcessGreekVerse(Verse verse, int bookIndex, string book, string verseRef, ReferenceVersionTAGNT tagnt)
        {
            string bookName = tagnt.GetBookNameFromIndex(bookIndex);
            string originalRef = verseRef.Replace(book, bookName);
            int colon = originalRef.IndexOf(':');
            string verseNum = originalRef.Substring(colon);
            if(verseRef == "Mar 7:20")
            {
                int x = 0;
            }

            if (ntVerseMap.ContainsKey(verseRef))
            {
                int len = 0;
                try
                {
                    foreach (int[] map in ntVerseMap[verseRef])
                    {
                        //  ch  v  s  l  v  s    
                        int chapterNum = map[0];
                        int verseNumbr = map[1];
                        int start = map[2];
                        if (map[3] == -1)
                            len = verse.Count - start;
                        else
                            len = map[3];
                        //len += map[3];
                        int mapVerse = map[4];
                        int mapStart = map[5];
                        int mapLen = map[6];

                        int sp = verseRef.IndexOf(' ');
                        string bk = verseRef.Substring(0, sp);
                        int grkChapter = chapterNum;
                        if(mapVerse == 0)
                        {
                            // we are mapping to the last verse in the previous chapter
                            if (grkChapter > 1) grkChapter--;
                            mapVerse = tagnt.LastVerse(bookName, grkChapter);
                        }

                        int bkIndex = container.Target.GetBookIndex(bk);
                        string grkBk = tagnt.GetBookNameFromIndex(bkIndex);
                        string grkRef = string.Format("{0} {1}:{2}", grkBk, grkChapter, mapVerse);

                        // get the greek verse
                        Verse greekVerse = tagnt.Bible[grkRef];
                        Verse greekVerseSec = greekVerse.SubVerse(mapStart, mapLen);
                        if (map[3] == 0) // unmapped section of reconstructed words
                            ProcessVerseNT(verseRef, originalRef, verse, start, 0, greekVerseSec);
                        else
                            ProcessVerseNT(verseRef, originalRef, verse, start, len, greekVerseSec);
                    }

                }
                catch (Exception ex)
                {
                    var cm = System.Reflection.MethodBase.GetCurrentMethod();
                    var name = cm.DeclaringType.FullName + "." + cm.Name;
                    Tracing.TraceException(name, ex.Message);
                }
            }
            else if (tagnt.Bible.ContainsKey(originalRef))
            {
                // get the Greek verse
                Verse greekVerse = tagnt.Bible[originalRef];
                ProcessVerseNT(verseRef, originalRef, verse, 0, verse.Count, greekVerse); ;
            }
        }
      //private void ProcessVerseNT(string verseRef, string originalRef, Verse verse, int startIndex, int end, Verse greekVerse)
        private void ProcessVerseNT(string verseRef, string originalRef, Verse verse, int startIndex, int count, Verse greekVerse)
        {
            Dictionary<string, int> usedStrongs = new Dictionary<string, int>();

            List<TranslatorWord> newVerse = new List<TranslatorWord>();

            if (verseRef == "Mar 7:37")
            {
                int x = 0;
            }

            #region set repeated word indices and counts
            Dictionary<string, List<VerseWord>> temp = new Dictionary<string, List<VerseWord>>();
            for (int i = 0; i < greekVerse.Count; i++)
            {
                VerseWord vw1 = greekVerse[i];
                if (vw1.Strong.Count != 1) continue; // for now, handle only single strong's.

                string strongString = vw1.StrongStringS;
                if (temp.ContainsKey(strongString)) continue; // we already processed this number

                for (int j = i + 1; j < greekVerse.Count; j++)
                {
                    VerseWord vw2 = greekVerse[j];
                    if (vw2.Strong.Count != 1) continue; // for now, handle only single strong's.

                    if (strongString == vw2.StrongStringS)
                    {
                        if (temp.ContainsKey(strongString))
                        {
                            if (vw2.Strong[0].Occurance == 0)
                            {
                                vw2.Strong[0].Occurance = temp[strongString].Count + 1;
                            }
                            else if (vw2.Strong[0].Occurance != temp[strongString].Count + 1)
                            {
                                int w = 0;
                                vw2.Strong[0].Occurance = temp[strongString].Count + 1;
                            }

                            temp[strongString].Add(vw2);
                        }
                        else
                        {
                            // first match
                            temp[strongString] = new List<VerseWord>();
                            if (vw1.Strong[0].Occurance == 0)
                            {
                                vw1.Strong[0].Occurance = 1;
                            }
                            else if (vw1.Strong[0].Occurance != 1)
                            {
                                int w = 0;
                                vw1.Strong[0].Occurance = 1;
                            }
                            if (vw2.Strong[0].Occurance == 0)
                            {
                                vw2.Strong[0].Occurance = 2;
                            }
                            else if (vw2.Strong[0].Occurance != 2)
                            {
                                int w = 0;
                                vw2.Strong[0].Occurance = 2;
                            }
                            temp[strongString].Add(vw1);
                            temp[strongString].Add(vw2);
                        }
                    }
                }
            }
            foreach (List<VerseWord> list in temp.Values)
            {
                foreach (VerseWord w in list)
                {
                    w.Strong[0].CountInVerse = list.Count;
                }
            }
            #endregion set repeated word indices and counts

            try
            {

                // key: target word index
                // val: greek word index
                Dictionary<int, List<int>> target2GreekMap = new Dictionary<int, List<int>>();

                int lastGrkIndex = -1;
                for (int i = startIndex; i < startIndex + count; i++)
                {
                    List<OriginalWordDetails> ow = new List<OriginalWordDetails>();

                    if (verse[i].Strong == null || verse[i].Strong.Count == 0 ||
                        (verse[i].Strong.Count == 1 && verse[i].Strong[0].IsEmpty))
                    {
                        ow = null;
                    }
                    else
                    {
                        bool comb = false;
                        foreach (StrongsNumber strong in verse[i].Strong.Strongs)
                        {
                            if (string.IsNullOrEmpty(strong.ToString()))
                                continue;
                            //VerseWord gw = greekVerse.GetWordFromStrong(strong.ToStringS(), lastGrkIndex, comb); // offset);
                            List<VerseWord> gwList = greekVerse.GetWordListFromStrong(strong);//, lastGrkIndex, comb); // offset);
                            VerseWord gw = null;
                            if (gwList.Count == 0)
                            {
/*                                ntErrors.Add(string.Format("Empty gwList:  {0}\t{1}-{2}\t{3}",
                                   verseRef,
                                   verse[i].WordIndex,
                                   verse[i].Word,
                                   strong));*/
                            }
                            else if (gwList.Count > 1)
                            {

                                if (target2GreekMap.Count == 0)
                                {
                                    gw = gwList[0];
                                }
                                else
                                {
                                    int lastAraWordIdx = target2GreekMap.Keys.ToArray()[target2GreekMap.Keys.Count - 1];
                                    lastGrkIndex = 0;
                                    foreach (int hi in target2GreekMap[lastAraWordIdx])
                                    {
                                        if (hi > lastGrkIndex) lastGrkIndex = hi;
                                    }


                                    List<VerseWord> unusedGrkWds = new List<VerseWord>();
                                    bool found = false;
                                    for (int j = 0; j < gwList.Count; j++)
                                    {
                                        gw = gwList[j];
                                        foreach (List<int> ints in target2GreekMap.Values)
                                        {
                                            if (ints.Contains(gw.WordIndex))
                                            {
                                                found = true;
                                                break;
                                            }
                                        }
                                        if (!found)
                                        {
                                            unusedGrkWds.Add(gw);
                                        }
                                        found = false;
                                    }

                                    // decide which Greek word to use.
                                    if (unusedGrkWds.Count > 0)
                                    {
                                        // Start with the first occurrence
                                        gw = unusedGrkWds[0];

                                        if (unusedGrkWds.Count > 1)
                                        {
                                            // if אֹת֖ H0853 or H0176 א֚וֹ or H0413 אֶל choose closest to last
                                            if (gw.Strong.Strongs.Count == 1 &&
                                                (gw.Strong.Strongs[0].ToString().Contains("H0853")) ||
                                                (gw.Strong.Strongs[0].ToString().Contains("H0176")) ||
                                                 (gw.Strong.Strongs[0].ToString().Contains("H0413")))
                                            {
                                                int span = Math.Abs(lastGrkIndex - gw.WordIndex);
                                                // get the first in the list
                                                // choose closest to last
                                                for (int c = 1; c < unusedGrkWds.Count; c++)
                                                {
                                                    int sp = Math.Abs(lastGrkIndex - unusedGrkWds[c].WordIndex);
                                                    if (sp < span)
                                                    {
                                                        span = sp;
                                                        gw = unusedGrkWds[c];
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                gw = gwList[0];
                            }
                            if (gw == null)
                            {
                                ntErrors.Add(string.Format("gw is null: {0}\t{1}-{2}\t{3}",
                                   verseRef,
                                   verse[i].WordIndex + 1,
                                   verse[i].Word,
                                   strong));

                            }
                            else
                            {
                                if (comb)
                                    lastGrkIndex = Math.Min(lastGrkIndex, gw.WordIndex);
                                else
                                    lastGrkIndex = gw.WordIndex;

                                comb = true; // in case next strong is still within the same target word
                                usedStrongs[strong.ToStringS()] = gw.WordIndex + 1;
                                if (target2GreekMap.ContainsKey(i))
                                {
                                    target2GreekMap[i].Add(gw.WordIndex);
                                }
                                else
                                {
                                    target2GreekMap[i] = new List<int> { gw.WordIndex };
                                }
                                OriginalWordDetails wd = new OriginalWordDetails(gw.RootStrong, gw.Morphology, gw.Greek, gw.WordIndex, gw.Transliteration, gw.Word, gw.WordNumber, gw.WordType, gw.AltVerseNumber, gw.Reference,dictForm:gw.DictForm, dictGloss:gw.DictGloss);
                                bool add = true;
                                if(ow.Count > 0)
                                {
                                    foreach (OriginalWordDetails owd in ow)
                                    {
                                        if (owd.AncientWordNumber == wd.AncientWordNumber &&
                                            owd.AncientWordIndex == wd.AncientWordIndex && 
                                            owd.Strongs == wd.Strongs)
                                        {
                                            add = false; break;
                                        }
                                    }
                                }
                                if(add)
                                    ow.Add(wd);
                            }
                        }
                    }
                    TranslatorWord aw = new TranslatorWord(verse[i].Word, ow);
                    aw.TargetWordIndex = newVerse.Count;
                    newVerse.Add(aw);
                }

                // Now map contains all Target word indices and corresponding Greek word indices
                // if a Greek index is missing
                // we find the target word preceding 

                SortedDictionary<int, int> missedWords = new SortedDictionary<int, int>();
                int lastGreekIndex = -1;
                int indexOffset = 1;
                bool doContinue = false;

                Dictionary<int, int> reverseMap = new Dictionary<int, int>();
                Dictionary<int, List<int>> correctedMap = new Dictionary<int, List<int>>();

                // fill in the Target Words missing from map
                // Target words with no greek words, are assigned greek word numbers beyond the greekVerse.Count
                int missingTargetWords = 0;
                if (count > 0)
                {
                    //int targetWordCount = count;
                    missingTargetWords = count - target2GreekMap.Count;
                    if (missingTargetWords > 0)
                    {
                        int lastGreekWord = greekVerse.Count - 1;
                        for (int i = startIndex; i < startIndex + count; i++)
                        {
                            if (target2GreekMap.ContainsKey(i))
                            {
                                correctedMap[i] = target2GreekMap[i];
                            }
                            else
                            {
                                correctedMap[i] = new List<int> { ++lastGreekWord };
                            }
                        }
                    }
                    else
                    {
                        correctedMap = target2GreekMap;
                    }
                }

                // initialise reverse map
                // making room at the end for the unmapped Target words
                for (int i = 0; i < (greekVerse.Count + missingTargetWords); i++)
                {
                    reverseMap[i] = -1;
                }

                int restoredAllowance = 100;  // alowance for restored Greek words (RHW) e.g. if 3 RHW follow word index 5, they will be numbered 501, 502, 503
                // assign target indices to greek indeces
                foreach ((int mapKey, List<int> mapValues) in correctedMap)
                {
                    foreach (int idx in mapValues)
                    {
                        reverseMap[idx] = (mapKey + 1) * restoredAllowance; // allow for in-between Greek and avoid 0 index
                    }
                }

                // fill in the gaps
                int lastAraIndex = 0;
                List<int> seenAraIndeces = new List<int>();
                foreach (int hIndex in reverseMap.Keys)
                {
                    if (reverseMap[hIndex] == -1)
                    {
                        reverseMap[hIndex] = ++lastAraIndex;
                    }
                    else
                    {
                        if (!seenAraIndeces.Contains(reverseMap[hIndex]))
                        {
                            // this code is to deal with an Target word with multiple Greek words with gaps in btween
                            lastAraIndex = reverseMap[hIndex];
                            seenAraIndeces.Add(lastAraIndex);
                        }
                    }
                }

                SortedDictionary<int, List<int>> newMap = new SortedDictionary<int, List<int>>();
                foreach ((int hIndex, int aIndex) in reverseMap)
                {
                    if (newMap.ContainsKey(aIndex))
                    {
                        newMap[aIndex].Add(hIndex);
                    }
                    else
                    {
                        if (hIndex < greekVerse.Count)
                            newMap[aIndex] = new List<int> { hIndex };
                        else
                            newMap[aIndex] = new List<int> { };
                    }
                }

                List<TranslatorWord> updatedVerse = new List<TranslatorWord>();
                foreach ((int aIndex, List<int> hIndex) in newMap)
                {
                    if ((aIndex % restoredAllowance) == 0)
                    {
                        int index = (aIndex / restoredAllowance) - 1;
                        int adjIndex = index - startIndex;
                        if (hIndex.Count > 0)
                            updatedVerse.Add(newVerse[adjIndex]);
                        else
                        {
                            TranslatorWord unmapped = new TranslatorWord(verse[index].Word, null);
                            unmapped.TargetWordIndex = adjIndex;
                            updatedVerse.Add(unmapped);
                        }
                    }
                    else
                    {
                        if (hIndex.Count != 1)
                        {
                            int n = 0;
                        }
                        VerseWord gw = greekVerse[hIndex[0]];
                        List<OriginalWordDetails> ow = new List<OriginalWordDetails>();
                        ow.Add(new OriginalWordDetails(gw.RootStrong, gw.Morphology, gw.Greek, gw.WordIndex, gw.Transliteration, gw.Word, gw.WordNumber, gw.WordType, gw.AltVerseNumber, gw.Reference));
                        updatedVerse.Add(new TranslatorWord(string.Empty, ow));

                    }
                }

                if (ntWords.ContainsKey(originalRef))
                {
                    int offset = 0;
                    // find last Target word index
                    for (int i = ntWords[originalRef].Count - 1; i >= 0; i--)
                    {
                        TranslatorWord word = ntWords[originalRef][i];
                        if (string.IsNullOrEmpty(word.Word)) continue;
                        offset = word.TargetWordIndex + 1;
                        break;
                    }
                    //append this verse to previous verse
                    foreach (TranslatorWord w in updatedVerse)
                    {
                        w.TargetWordIndex += offset;
                        ntWords[originalRef].Add(w);
                    }
                }
                else
                {
                    ntWords[originalRef] = updatedVerse;
                }


            }
            catch (Exception ex)
            {
                var cm = System.Reflection.MethodBase.GetCurrentMethod();
                var name = cm.DeclaringType.FullName + "." + cm.Name;
                Tracing.TraceException(name, ex.Message);
            }
        }

        private void OutputTranslatorTagsNT(Dictionary<string, List<TranslatorWord>> words, bool publicDomain)
        {
            string version = Properties.TranslationTags.Default.Version.Replace(".", "_");

            StreamWriter swR = null;
            StreamWriter sw = null;

            bool perBook = Properties.TranslationTags.Default.FilesPerBook;
            bool preambleWritten = false;
            string lastWrittenBook = string.Empty;
            int bookNum = 0;

            foreach ((string verseRef, List<TranslatorWord> verseWords) in words)
            {
                if (verseRef == "1Th 1:3")
                {
                    int x = 0;
                }

                container.UpdateProgress("Processing xlation Tags", (100 * verseCounter++) / totalVerseCount);

                if (perBook)
                {
                    int space = verseRef.IndexOf(' ');
                    string book = verseRef.Substring(0, space);
                    if (string.IsNullOrEmpty(lastWrittenBook))
                    {
                        // this is the first time through
                        bookNum = 40;
                        lastWrittenBook = book;
                    }
                    else if (book != lastWrittenBook)
                    {
                        // we are starting a new book
                        // flush and close for last book
                        if (sw != null) { sw.Flush(); sw.Close(); sw = null; }
                        if (swR != null) { swR.Flush(); swR.Close(); swR = null; }
                        preambleWritten = false;
                        lastWrittenBook = book;
                        bookNum++;
                    }
                    if (sw == null)
                    {
                        string output = Path.Combine(booksFolderPath,
                                     string.Format("NT_{0:d2}_{1}_{2}_{3}.txt", bookNum -39, book, Properties.TranslationTags.Default.OutputFileName, version));
                        sw = new StreamWriter(output, false);
                    }

                    if (swR == null)
                    {
                        if (!string.IsNullOrEmpty(Properties.TranslationTags.Default.ForReviewFileName))
                        {
                            string forReview = Path.Combine(booksFolderPath,
                                string.Format("NT_{0:d2}_{1}_{2}.txt", bookNum -39, book, Properties.TranslationTags.Default.ForReviewFileName));
                            swR = new StreamWriter(forReview, false);
                        }
                    }
                }
                else
                {
                    if (sw == null)
                    {
                        string output = Path.Combine(TranslationTagsFolderPath,
                                     string.Format("{0}_NT_{1}.txt", Properties.TranslationTags.Default.OutputFileName, version));
                        sw = new StreamWriter(output, false);
                    }
                    if (swR == null)
                    {
                        if (!string.IsNullOrEmpty(Properties.TranslationTags.Default.ForReviewFileName))
                        {
                            string forReview = Path.Combine(TranslationTagsFolderPath,
                            string.Format("{0}-NT.txt", Properties.TranslationTags.Default.ForReviewFileName));
                            swR = new StreamWriter(forReview, false);
                        }
                    }
                }

                if (publicDomain && !preambleWritten)
                {
                    string preamble = Properties.Resources.TTArabicSVDPreamble_NT;

                    WritePreamble(sw, preamble);
                    preambleWritten = true;
                }
                try
                {
                    string Strongs = "Strongs";
                    string Morphology = "Grammar";
                    string AncientWordVerse = "Grk w#";
                    string AncientWordNumber = string.Empty;
                    string AncientWord = "Greek";
                    string Transliteration = "Transliteration";
                    string WordByWord = "Word-by-word";
                    string Lexicon = "Lexicon";
                    string Gloss = "Gloss";

                    // Table columns after the Arabic word
                    // in the order to be displayed
                    // key = title
                    // val = variable
                    Dictionary<string, string> outputTable = new Dictionary<string, string>();
                    outputTable.Add(Strongs, "");
                    outputTable.Add(AncientWord, "");
                    outputTable.Add(Lexicon, "");
                    outputTable.Add(Gloss, "");
                    outputTable.Add(Transliteration, "");
                    outputTable.Add(Morphology, "");
                    outputTable.Add(AncientWordVerse, "");
                    //outputTable.Add(WordByWord, "");

                    sw.WriteLine(verseRef.Replace(" ", ".").Replace(":", "."));
                    sw.WriteLine("========>>>");
                    //sw.WriteLine("Ara W#\tArabic\tStrongs\tGrammar\tGrk w#\tGreek\tTransliteration\tWord-by-word\tLexicon\tGloss");
                    //sw.WriteLine("Ara W#\tArabic\tStrongs\tGrammar\tGrk w#\tGreek\tTransliteration\tLexicon\tGloss");
                    //sw.WriteLine("ESV W#\tArabic\tStrongs\tGrammar\tGrk w#\tGreek\tTransliteration\tWord-by-word");
                    string header = "Ara W#\tArabic";
                    string[] headerX = outputTable.Keys.ToArray();
                    for (int i = 0; i < headerX.Length; i++)
                    {
                        header += "\t" + headerX[i];
                    }
                    sw.WriteLine(header);

                    int arabicIndexAdjust = 1;
                    foreach (TranslatorWord word in verseWords)
                    {
                        string araWordNo = string.Empty;
                        if (!string.IsNullOrEmpty(word.Word))
                        {
                            string[] parts = word.Word.Split(new char[] { ' ' });
                            if (parts.Length == 1)
                            {
                                if (parts[0] == "-" || parts[0] == "\u00ad")
                                    arabicIndexAdjust--;
                                else
                                    araWordNo = string.Format("#{0:d2}", word.TargetWordIndex + arabicIndexAdjust);
                            }
                            else
                            {
                                araWordNo = string.Format("#{0:d2} - #{1:d2}", word.TargetWordIndex + arabicIndexAdjust,
                                                                   word.TargetWordIndex + arabicIndexAdjust + parts.Length - 1);
                                arabicIndexAdjust += (parts.Length - 1);
                            }
                        }

                        string line = string.Empty;
                        if (word.OtiginalWords == null)
                        {
                            line = string.Format("{0}\t{1}", //\t\t\t\t\t\t\t",
                                    araWordNo,
                                    word.Word);
                            for (int i = 0; i < outputTable.Count; i++)
                                line += "\t";
                            string reviewLine = string.Format("{0}\t{1}", verseRef, word.Word);
                            swR.WriteLine(reviewLine);
                        }
                        else
                        {
                            // concatenate all Greek words
                            //Strongs = string.Empty;
                            //Morphology = string.Empty;
                            //AncientWordVerse = string.Empty;
                            //AncientWordNumber = string.Empty;
                            //AncientWord = string.Empty;
                            //Transliteration = string.Empty;
                            //WordByWord = string.Empty;
                            //Lexicon = string.Empty;
                            //Gloss = string.Empty;

                            foreach (string key in outputTable.Keys)
                            {
                                outputTable[key] = string.Empty;
                            }


                            foreach (OriginalWordDetails owd in word.OtiginalWords)
                            {
                                string ancientVerseNumber = owd.AncientWordVerse;
                                if (!string.IsNullOrEmpty(ancientVerseNumber))
                                {
                                    string ancientVerseref = owd.AncientWordReference;
                                    int sp = ancientVerseref.IndexOf(' ');
                                    if (sp >= 0)
                                    {
                                        ancientVerseNumber = string.Format("[{0}]", ancientVerseref.Substring(sp + 1));
                                    }
                                }
                                // if empty, and verse number is different from current verse number
                                // construct verse number
                                string chV = verseRef.Substring(verseRef.IndexOf(' ') + 1).Replace(":", ".");
                                if (string.IsNullOrEmpty(ancientVerseNumber))
                                {
                                    string grkChV = owd.AncientWordReference;
                                    grkChV = grkChV.Substring(verseRef.IndexOf(' ') + 1).Replace(":", ".");
                                    if (grkChV != chV)
                                        ancientVerseNumber = string.Format("({0})", grkChV);
                                }
                                else
                                {
                                    if (ancientVerseNumber.Trim(new char[] { '(', ')' }) == chV)
                                        ancientVerseNumber = string.Empty;
                                }

                                //Strongs += owd.Strongs + "; ";
                                //Morphology += owd.Morphology + "; ";
                                ////                                AncientWordVerse += ancientVerseNumber + "; ";
                                ////                                AncientWordNumber += owd.AncientWordNumber + "; ";
                                //AncientWordVerse += ancientVerseNumber + "#" + owd.AncientWordNumber + "; ";
                                //AncientWord += owd.AncientWord + "; ";
                                //Transliteration += owd.Transliteration + "; ";
                                //WordByWord += owd.WordByWord + "; ";
                                //Lexicon += owd.DictForm + "; ";
                                //Gloss += owd.DictGloss+ "; ";
                                if(outputTable.ContainsKey(Strongs)) outputTable[Strongs] += owd.Strongs + "; ";
                                if (outputTable.ContainsKey(Morphology)) outputTable[Morphology] += owd.Morphology + "; ";
                                if (outputTable.ContainsKey(AncientWordVerse)) outputTable[AncientWordVerse] += ancientVerseNumber + "#" + owd.AncientWordNumber + "; ";
                                if (outputTable.ContainsKey(AncientWord)) outputTable[AncientWord] += owd.AncientWord + "; ";
                                if (outputTable.ContainsKey(Transliteration)) outputTable[Transliteration] += owd.Transliteration + "; ";
                                if (outputTable.ContainsKey(WordByWord)) outputTable[WordByWord] += owd.WordByWord + "; ";
                                if (outputTable.ContainsKey(Lexicon)) outputTable[Lexicon] += owd.DictForm + "; ";
                                if (outputTable.ContainsKey(Gloss)) outputTable[Gloss] += owd.DictGloss + "; ";


                            }

                            //line = string.Format("{0}\t{1}\t{2}\t{3}\t{4}#{5}\t{6}\t{7}\t{8}",
                            //line = string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}",
                            //line = string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}",
                            //    araWordNo,
                            //    word.Word,
                            //    Strongs.TrimEnd(new char[] { ';', ' ' }),
                            //    Morphology.TrimEnd(new char[] { ';', ' ' }),
                            //    AncientWordVerse.TrimEnd(new char[] { ';', ' ' }),
                            //    //AncientWordNumber.TrimEnd(new char[] { ';', ' ' }),
                            //    AncientWord.TrimEnd(new char[] { ';', ' ' }),
                            //    Transliteration.TrimEnd(new char[] { ';', ' ' }),
                            //    //WordByWord.TrimEnd(new char[] { ';', ' ' }),
                            //    Lexicon.TrimEnd(new char[] { ';', ' ' }),
                            //    Gloss.TrimEnd(new char[] { ';', ' ' }));

                            line = string.Format("{0}\t{1}", araWordNo, word.Word);

                            for (int i = 0; i < outputTable.Count; i++)
                            {
                                line += "\t" + outputTable[headerX[i]].TrimEnd(new char[] { ';', ' ' });
                            }

                            if (string.IsNullOrEmpty(araWordNo))
                            {
                                //if (!Strongs.Contains("G3588") && !Strongs.Contains("G1161")) //τὸν, δὲ
                                //{
                                //    string reviewLine = string.Format("{0}\t#{1}:{2}",
                                //        verseRef,
                                //        AncientWordNumber.TrimEnd(new char[] { ';', ' ' }),
                                //        AncientWord.TrimEnd(new char[] { ';', ' ' }));
                                //    swR.WriteLine(reviewLine);
                                //}
                                if (!outputTable[Strongs].Contains("G3588") && !outputTable[Strongs].Contains("G1161")) //τὸν, δὲ
                                {
                                    string reviewLine = string.Format("{0}\t#{1}:{2}",
                                        verseRef,
                                        outputTable[AncientWordVerse].TrimEnd(new char[] { ';', ' ' }),
                                        outputTable[AncientWord].TrimEnd(new char[] { ';', ' ' }));
                                    swR.WriteLine(reviewLine);
                                }
                            }
                        }
                        if (publicDomain)
                            sw.WriteLine(line); // Utils.RemoveDiacritics(line));
                        else
                            sw.WriteLine(line);
                    }
                    sw.WriteLine("<<<========");
                    sw.WriteLine();
                }
                catch (Exception ex)
                {
                    var cm = System.Reflection.MethodBase.GetCurrentMethod();
                    var name = cm.DeclaringType.FullName + "." + cm.Name;
                    Tracing.TraceException(name, ex.Message);
                }
            }
            if (sw != null)
            {
                sw.Flush();
                sw.Close();
                sw = null;
            }
            if (swR != null)
            {
                swR.Flush();
                swR.Close();
                swR = null;
            }

        }

        [Obsolete]
        private void OutputTranslatorTagsNT1(string destination, Dictionary<string, List<TranslatorWord>> words)
        {
            using (StreamWriter sw = new StreamWriter(destination, false))
            {
                foreach ((string verseRef, List<TranslatorWord> verseWords) in words)
                {
                    if (verseRef == "Psa 3:2")
                    {
                        int z = 0;
                    }
                    try
                    {
                        sw.WriteLine(verseRef.Replace(" ", ".").Replace(":", "."));
                        sw.WriteLine("========>>>");
                        sw.WriteLine("Ara W#\tTarget\tStrongs\tGrammar\tGrk w#\tGreek\tTransliteration\tWord-by-word");
                        //sw.WriteLine("ESV W#\tTarget\tStrongs\tGrammar\tGrk w#\tGreek\tTransliteration\tWord-by-word");
                        int targetIndexAdjust = 1;
                        foreach (TranslatorWord word in verseWords)
                        {
                            string tgtWordNo = string.Empty;
                            if (!string.IsNullOrEmpty(word.Word))
                            {
                                string[] parts = word.Word.Split(new char[] { ' ' });
                                if (parts.Length == 1)
                                {
                                    tgtWordNo = string.Format("#{0:d2}", word.TargetWordIndex + targetIndexAdjust);
                                }
                                else
                                {
                                    tgtWordNo = string.Format("#{0:d2} - #{1:d2}", word.TargetWordIndex + targetIndexAdjust,
                                                                       word.TargetWordIndex + targetIndexAdjust + parts.Length - 1);
                                    targetIndexAdjust += (parts.Length - 1);
                                }
                            }

                            string line = string.Empty;
                            if (word.OtiginalWords == null)
                            {
                                line = string.Format("{0}\t{1}\t\t\t\t\t\t",
                                        tgtWordNo,
                                        word.Word);

                            }
                            else
                            {
                                // concatenate all Greek words
                                string Strongs = string.Empty;
                                string Morphology = string.Empty;
                                string AncientWordVerse = string.Empty;
                                string AncientWordNumber = string.Empty;
                                string AncientWord = string.Empty;
                                string Transliteration = string.Empty;
                                string WordByWord = string.Empty;

                                foreach (OriginalWordDetails owd in word.OtiginalWords)
                                {
                                    string ancientVerseNumber = owd.AncientWordVerse;
                                    // if empty, and verse number is different from current verse number
                                    // construct verse number
                                    string chV = verseRef.Substring(verseRef.IndexOf(' ') + 1).Replace(":", ".");
                                    if (string.IsNullOrEmpty(ancientVerseNumber))
                                    {
                                        string grkChV = owd.AncientWordReference;
                                        grkChV = grkChV.Substring(verseRef.IndexOf(' ') + 1).Replace(":", ".");
                                        if (grkChV != chV)
                                            ancientVerseNumber = string.Format("({0})", grkChV);
                                    }
                                    else
                                    {
                                        if (ancientVerseNumber.Trim(new char[] { '(', ')' }) == chV)
                                            ancientVerseNumber = string.Empty;
                                    }

                                    Strongs += owd.Strongs + "; ";
                                    Morphology += owd.Morphology + "; ";
                                    AncientWordVerse += ancientVerseNumber + "; ";
                                    AncientWordNumber += owd.AncientWordNumber + "; ";
                                    AncientWord += owd.AncientWord + "; ";
                                    Transliteration += owd.Transliteration + "; ";
                                    WordByWord += owd.WordByWord + "; ";
                                }

                                line = string.Format("{0}\t{1}\t{2}\t{3}\t{4}#{5}\t{6}\t{7}\t{8}",
                                    tgtWordNo,
                                    word.Word,
                                    Strongs.TrimEnd(new char[] { ';', ' ' }),
                                    Morphology.TrimEnd(new char[] { ';', ' ' }),
                                    AncientWordVerse.TrimEnd(new char[] { ';', ' ' }),
                                    AncientWordNumber.TrimEnd(new char[] { ';', ' ' }),
                                    AncientWord.TrimEnd(new char[] { ';', ' ' }),
                                    Transliteration.TrimEnd(new char[] { ';', ' ' }),
                                    WordByWord);
                            }
                            sw.WriteLine(line);
                        }
                        sw.WriteLine();
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
    }



 }
