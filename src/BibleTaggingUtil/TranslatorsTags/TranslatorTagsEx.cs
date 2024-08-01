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
        private string TranslationTagsFolderPath = string.Empty;
        private string booksFolderPath = string.Empty;

        BibleTaggingForm container;
        public TranslationTagsEx(BibleTaggingForm bibleTaggingForm)
        {
            container = bibleTaggingForm;

            otVerseMap = VerseMappings.OldTestament;
            ntVerseMap = VerseMappings.NewTestament;

            // Set up individual books folder if does not exist
            TranslationTagsFolderPath = Properties.TranslationTags.Default.TranslationTagsFolder;
            booksFolderPath = Path.Combine(TranslationTagsFolderPath, "Books");
            if (!Directory.Exists(booksFolderPath))
            {
                Directory.CreateDirectory(booksFolderPath);
            }
        }

        public void Export(BibleVersion target, ReferenceVersionTAHOT tahot, ReferenceVersionTAGNT tagnt)
        {
            Prepare(target, tahot, tagnt);
        }

        int totalVerseCount = 0;
        int verseCounter = 0;

        private void Prepare(BibleVersion target, ReferenceVersionTAHOT tahot, ReferenceVersionTAGNT tagnt)
        {
            otWords.Clear();
            otMissedWords.Clear();
            otErrors.Clear();

            ntWords.Clear();
            ntErrors.Clear();

            if (!string.IsNullOrEmpty(Properties.TranslationTags.Default.RepeatedWordFileName))
            {
                totalVerseCount = 2 * target.Bible.Count;
                verseCounter = 0;
                container.WaitCursorControl(true);

                string repeatedPath = Path.Combine(TranslationTagsFolderPath, Properties.TranslationTags.Default.RepeatedWordFileName);
                repeatedPath = Path.ChangeExtension(repeatedPath, ".txt");
                using (StreamWriter sw = new StreamWriter(repeatedPath))
                {
                    foreach ((string verseRef, Verse verse) in target.Bible)
                    {
                        container.UpdateProgress("Processing xlation Tags", (100 * verseCounter++) / totalVerseCount);

                        //string repeatedPath = Path.Combine(TranslationTagsFolderPath, Properties.TranslationTags.Default.RepeatedWordFileName);
                        string book = string.Empty;

                        int idx = verseRef.IndexOf(' ');
                        if (idx != -1)
                        {
                            book = verseRef.Substring(0, idx);
                        }

                        int bookIndex = target.GetBookIndex(book);

                        #region Repeted word code
                        try
                        {

                            // Repeated Hebrew word for intensification. Sometimes this can also mean "every" (e.g. "every morning" for "morning morning" or "every day" for "day day", "every man" for "man man").
                            if (bookIndex < 5)
                            {
                                string lastWord = string.Empty;
                                for (int i = 0; i < verse.Count; i++)
                                {
                                    VerseWord word = verse[i];
                                    if (word.Strong.Count > 1)
                                    {
                                        foreach (StrongsNumber s in word.Strong.Strongs)
                                        {
                                            if (word.Strong.Strongs.Count(item => item.ToStringS() == s.ToStringS()) > 1)
                                            {
                                                // we have a word to report
                                                string aWord = word.Word;
                                                string strong = word.Strong.ToString();
                                                if (aWord != lastWord)
                                                {
                                                    lastWord = aWord;
                                                    sw.WriteLine(string.Format("{0}\t{1}\t{2}", verseRef, aWord, strong));
                                                }
                                            }
                                        }

                                    }
                                }

                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Repeted Words Errors Found", "TT Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                        }
                        #endregion temp code

                        if (bookIndex < 39)
                        {
                            // OT
                            ProcessHebrewVerse(verse, bookIndex, book, verseRef, tahot);
                        }
                        else
                        {
                            // NT
                            ProcessGreekVerse(verse, bookIndex, book, verseRef, tagnt);
                        }
                        string x = verseRef;
                        Verse y = verse;

                    }
                }
            }

            if (otWords.Count > 0)
            {
                if (otErrors.Count > 0)
                {
                    if (!string.IsNullOrEmpty(Properties.TranslationTags.Default.ErrorsFileName))
                    {
                        string errorsFile = Path.Combine(TranslationTagsFolderPath, Properties.TranslationTags.Default.ErrorsFileName + "_OT");
                        errorsFile = Path.ChangeExtension(errorsFile, ".txt");
                        OutputErrors(errorsFile, otErrors);
                    }
                    MessageBox.Show("OT Errors Found", "TT Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }
                else
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(Properties.TranslationTags.Default.OutputFileName))
                        {
                            //              OutputTranslatorTagsOT(@"C:\temp\TTAraSVD - Translation Tags etc. for Arabic SVD OT - STEPBible.org CC BY_1_1.txt", otWords, publicDomain);
                            OutputTranslatorTagsOT(otWords, Properties.TranslationTags.Default.PublicDomain);
                            MessageBox.Show("OT Success", "TT Success", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("OT Write failed", "TT Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }

                    foreach ((string verseRef, Verse hebrewVerse) in tahot.Bible)
                    {
                        int idx = verseRef.IndexOf(' ');
                        string book = string.Empty;
                        if (idx != -1)
                        {
                            book = verseRef.Substring(0, idx);
                        }

                        int bookIndex = tahot.GetBookIndex(book);
                        string bookName = target.GetBookNameFromIndex(bookIndex);
                        string reference = verseRef.Replace(book, bookName);

                        for (int i = 0; i < hebrewVerse.Count; i++)
                        {
                            bool found = false;
                            if (otWords.ContainsKey(reference))
                            {
                                List<TranslatorWord> updatedVerse = otWords[reference];
                                foreach (TranslatorWord w in updatedVerse)
                                {
                                    if (w.OtiginalWords != null)
                                    {
                                        foreach (OriginalWordDetails ow in w.OtiginalWords)
                                        {
                                            if (ow.AncientWordIndex == i)
                                            {
                                                found = true;
                                                break;
                                            }
                                        }
                                        if (found)
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                int e = 0;
                            }
                            if (!found)
                            {
                                if (otMissedWords.ContainsKey(reference))
                                {
                                    otMissedWords[reference].Add(i);
                                }
                                else
                                {
                                    otMissedWords[reference] = new List<int> { i };
                                }
                            }
                        }
                    }

                }

                if (!string.IsNullOrEmpty(Properties.TranslationTags.Default.MissingWordsFileName))
                {
                    string missedWordsFile = Path.Combine(TranslationTagsFolderPath, Properties.TranslationTags.Default.MissingWordsFileName + "_OT");
                    missedWordsFile = Path.ChangeExtension(missedWordsFile, ".txt");
                    using (StreamWriter sw = new StreamWriter(missedWordsFile))
                    {
                        foreach ((string reference, List<int> missed) in otMissedWords)
                        {
                            sw.WriteLine(string.Format("{0}\t{1}", reference, string.Join(",", missed)));
                        }
                    }
                }
            }

            if (ntWords.Count > 0)
            {
                if (ntErrors.Count > 0)
                {
                    if (!string.IsNullOrEmpty(Properties.TranslationTags.Default.ErrorsFileName))
                    {
                        string errorsFile = Path.Combine(TranslationTagsFolderPath, Properties.TranslationTags.Default.ErrorsFileName + "_NT");
                        errorsFile = Path.ChangeExtension(errorsFile, ".txt");
                        OutputErrors(errorsFile, ntErrors);
                    }
                    MessageBox.Show("NT Errors Found", "TT Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }
                else
                {
                    try
                    {
                        //OutputTranslatorTagsNT(@"C:\temp\TTAraSVD - Translation Tags etc. for Arabic SVD NT - STEPBible.org CC BY_1_1.txt", ntWords);
                        OutputTranslatorTagsNT(ntWords, Properties.TranslationTags.Default.PublicDomain);
                        //OutputTranslatorTags(@"C:\temp\TTESV - Translators Tags for ESV.txt", otWords);
                        MessageBox.Show("NT Success", "TT Success", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("NT Write failed", "TT Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                }
            }
            container.WaitCursorControl(false);
        }

        private void OutputErrors(string errorFile, List<string> errors)
        {
            using (StreamWriter sw = new StreamWriter(errorFile))
            {
                int i = 0;
                while (true)
                {
                    if (i >= errors.Count) break;

                    sw.Write(errors[i]);
                    sw.Write("\t");
                    if (i + 50 < errors.Count) sw.Write(errors[i + 50]);
                    sw.Write("\t");
                    if (i + 100 < errors.Count) sw.Write(errors[i + 100]);
                    sw.WriteLine();
                    i++;
                    if (i % 50 == 0)
                    {
                        i += 100;
                        sw.WriteLine();
                    }
                }
            }
        }
        private void WritePreamble(StreamWriter sw, string preamble)
        {
            string created = string.Format("Created {0} {1}.", 
                DateTime.Now.ToString("yyyy-MM-dd"), 
                ConfigurationHolder.Instance.OSIS[OsisConstants.creator_name]); 
            preamble = preamble.Replace("{Created}", created);

            string version = string.Format("Version {0}:", Properties.TranslationTags.Default.Version);
            preamble = preamble.Replace("{Version}", version);

            sw.WriteLine(preamble);
        }

    }



 }
