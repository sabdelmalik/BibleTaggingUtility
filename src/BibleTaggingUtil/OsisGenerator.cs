using BibleTaggingUtil.BibleVersions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BibleTaggingUtil
{
    internal class OsisGenerator
    {
        Dictionary<string, string> osisConf = new Dictionary<string, string>();

        public OsisGenerator(ConfigurationHolder config) 
        {
            this.osisConf = config.OSIS;
        }

        public void Generate(TargetVersion targetVersion, bool publicDomain = false)
        {
            string outputFile = string.Empty;
            bool forInjeel = false;

            try
            {
                string targetBiblesFolder = BibleTaggingUtil.Properties.TargetBibles.Default.TargetBiblesFolder;
                string targetBible = BibleTaggingUtil.Properties.TargetBibles.Default.TargetBible;
                string biblesFolder = Path.Combine(targetBiblesFolder, targetBible);

                //                if (osisConf.ContainsKey(OsisConstants.bible_vpl_file))
                //                    bibleVplFile = osisConf[OsisConstants.bible_vpl_file];

                if (osisConf.ContainsKey(OsisConstants.forInjeel))
                {
                    string temp = osisConf[OsisConstants.forInjeel];
                    forInjeel = (temp.ToLower() == "true");
                }


                if (osisConf.ContainsKey(OsisConstants.output_file))
                {
                    if (publicDomain)
                        outputFile = osisConf[OsisConstants.output_file_publicDomaind];
                    else
                        outputFile = osisConf[OsisConstants.output_file];
                }


                if (string.IsNullOrEmpty(outputFile))
                    throw new Exception("Configuration must contain an entry for output-file or output-file");

                string osisFile = Path.Combine(biblesFolder, outputFile);
                if (File.Exists(osisFile))
                    File.Delete(osisFile);

                using (StreamWriter sw = new StreamWriter(osisFile))
                {
                    sw.WriteLine("<?xml version='1.0' encoding='UTF-8'?>");
                    WriteOsisStartTag(sw);
                    WriteOsisTextStartTag(sw);
                    WriteHeader(sw, publicDomain);

                    WriteBible(sw, targetVersion, forInjeel, publicDomain);

                    WriteOsisTextEndTag(sw);
                    WriteOsisEndTag(sw);
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

        string currentBook = string.Empty;
        string currentChapter = string.Empty;
        bool oldTestament = true;
        
        private void WriteBible(StreamWriter sw, TargetVersion targetVersion, bool forInjeel, bool publicDomain = false)
        {
            string refPattern = @"^([1-9a-zA-Z]{1,6})\s([0-9]{1,3})\:([0-9]{1,3})";
            bool inOT = true;
            Verse previousVerse = new Verse();

            sw.WriteLine("<div type=\"bookGroup\">");
            foreach (string reference in targetVersion.Bible.Keys)
            {
                Verse verse = targetVersion.Bible[reference];

                string bookName = string.Empty;
                string chapterNum = string.Empty;
                string verseNum = string.Empty;

                Match m = Regex.Match(reference, refPattern);
                if (m.Success)
                {
                    bookName = m.Groups[1].Value;
                    chapterNum = m.Groups[2].Value;
                    verseNum = m.Groups[3].Value;
                }

                int currentBookIndex = targetVersion.GetBookIndex(bookName);

                if(currentBookIndex == 39 && inOT)
                {
                    // We've just started the NT
                    inOT = false;
                    sw.WriteLine("</div>");
                    sw.WriteLine("<div type=\"bookGroup\">");
                }
                string bookOsisName = OsisConstants.osisNames[currentBookIndex];

                if (currentBook != bookOsisName)
                {
                    // a new book

                    if (!string.IsNullOrEmpty(currentBook))
                    {
                        if (!string.IsNullOrEmpty(currentChapter))
                        {
                            // we are at the end of a chapter
                            sw.WriteLine("</chapter>");
                            currentChapter = string.Empty;
                        }
                        // and we are at the end of a book
                        if (currentBookIndex >= 39 && osisConf[OsisConstants.osisIDWork].ToLower().Contains("ara"))
                        {
                            string colophonStart = "<div type=\"colophon\">";
                            if (currentBook == "Rom")
                            {
                                sw.WriteLine(colophonStart);
                                sw.WriteLine("كُتِبَتْ إِلَى أَهْلِ رُومِيَةَ مِنْ كُورِنْثُوسَ عَلَى يَدِ فِيبِي خَادِمَةِ كَنِيسَةِ كَنْخَرِيَا");
                                sw.WriteLine("</div>");
                            }
                            else if (currentBook == "Eph")
                            {
                                sw.WriteLine(colophonStart);
                                sw.WriteLine("كُتِبَتْ إِلَى أَهْلِ أَفَسُسَ مِنْ رُومِيَةَ عَلَى يَدِ تِيخِيكُسَ");
                                sw.WriteLine("</div>");
                            }
                            else if (currentBook == "Phil")
                            {
                                sw.WriteLine(colophonStart);
                                sw.WriteLine("كُتِبَتْ إِلَى أَهْلِ فِيلِبِّي مِنْ رُومِيَةَ عَلَى يَدِ أَبَفْرُودِتُسَ");
                                sw.WriteLine("</div>");
                            }
                            else if (currentBook == "Col")
                            {
                                sw.WriteLine(colophonStart);
                                sw.WriteLine("كُتِبَتْ إِلَى أَهْلِ كُولُوسِّي مِنْ رُومِيَةَ بِيَدِ تِيخِيكُسَ وأُنِسِيمُسَ");
                                sw.WriteLine("</div>");
                            }
                            else if (currentBook == "Phlm")
                            {
                                sw.WriteLine(colophonStart);
                                sw.WriteLine("إِلَى فِلِيمُونَ كُتِبَتْ مِنْ رُومِيَةَ عَلَى يَدِ أُنِسِيمُسَ اٌلْخَادِمِ");
                                sw.WriteLine("</div>");
                            }
                        }
                        sw.WriteLine("</div>");
                    }
                    currentBook = bookOsisName;
                    // start tag for the new book
                    sw.WriteLine(string.Format("<div type='book' osisID='{0}'>", currentBook));
                }

                if (currentChapter != chapterNum)
                {
                    // this is a new chapter
                    if (!string.IsNullOrEmpty(currentChapter))
                    {
                        // we are at the end of a chapter
                        sw.WriteLine("</chapter>");
                    }
                    currentChapter = chapterNum;
                    // start tag for the new chapter
                    sw.WriteLine(string.Format("<chapter osisID='{0}.{1}'>", currentBook, currentChapter));
                }


                // Handle Psalm headers - enclose in <hebrewTitle> tag for Arbic Bible
                if (osisConf[OsisConstants.osisIDWork].ToLower().Contains("ara") &&
                    (currentBook == "Ps" && verseNum == "1"))
                {
                    if (verse.HasPsalmTitle)
                    {
                        sw.WriteLine(string.Format("<title canonical=\"true\" type=\"psalm\">{0}</title>", verse.OsisPsalmTitleData));
                    }
                }

                string verseText = verse.OsisVerseData;
                if (forInjeel)
                {
                    // Injeel
                    // Ezra 5:6 ٦ صُورَةُ الرِّسَالَةِ الَّتِي أَرْسَلَهَا تَتْنَايُ وَالِي عَبْرِ النَّهْرِ وَشَتَرْبُوزْنَايُ وَرُفَقَاؤُهُمَا الأَفَرْسَكِيِّينَ الَّذِينَ فِي عَبْرِ النَّهْرِ إِلَى دَارِيُوسَ الْمَلِكِ. أَرْسَلُوا إِلَيْهِ رِسَالَةً وَكَانَ مَكْتُوبًا فِيهَا هكَذَا:
                    // Ezra 5:7 «لِدَارِيُوسَ الْمَلِكِ كُلُّ سَلاَمٍ.
                    // SVD
                    // Ezra 5:6 صُورَةُ الرِّسَالَةِ الَّتِي أَرْسَلَهَا تَتْنَايُ وَالِي عَبْرِ النَّهْرِ وَشَتَرْبُوزْنَايُ وَرُفَقَاؤُهُمَا الأَفَرْسَكِيِّينَ الَّذِينَ فِي عَبْرِ النَّهْرِ إِلَى دَارِيُوسَ الْمَلِكِ.
                    // Ezra 5:7 أَرْسَلُوا إِلَيْهِ رِسَالَةً وَكَانَ مَكْتُوبًا فِيهَا هكَذَا: «لِدَارِيُوسَ الْمَلِكِ كُلُّ سَلاَمٍ.
                    if (currentBook == "Ezra" && currentChapter == "5" && verseNum == "6")
                    {
                        previousVerse = verse;
                        continue;
                    }
                    else if (currentBook == "Ezra" && currentChapter == "5" && verseNum == "7")
                    {
                        // need to split, combine, save, the the remainaing text
                        string endtext = "هكَذَا:"; // هكَذَا: <1836>";
                        int idx = verse.IndexOf(endtext);

                        Verse verse6 = previousVerse + verse.SubVerse(0, idx + 1);

                        sw.WriteLine(string.Format("<verse osisID='{0}.{1}.{2}'>{3}</verse>", currentBook, currentChapter, "6", verse6.OsisVerseData));
                        previousVerse = new Verse();
                        verseText = verse.SubVerse(idx + 1).OsisVerseData;
                    }
                    else if (currentBook == "Dan" && currentChapter == "2" && verseNum == "14")
                    {
                        previousVerse = verse;
                        continue;
                    }
                    else if (currentBook == "Dan" && currentChapter == "2" && verseNum == "15")
                    {
                        // need to split, combine, save, the the remainaing text
                        string endtext = "الْمَلِكِ:"; // الْمَلِكِ: <4430>";
                        int idx = verse.IndexOf(endtext);

                        Verse verse14 = previousVerse + verse.SubVerse(0, idx + 1);

                        sw.WriteLine(string.Format("<verse osisID='{0}.{1}.{2}'>{3}</verse>", currentBook, currentChapter, "14", verse14.OsisVerseData));
                        previousVerse = new Verse();
                        verseText = verse.SubVerse(idx + 1).OsisVerseData;
                    }
                    else if (currentBook == "1Tim" && currentChapter == "6" && verseNum == "21")
                    {

                        int idx = verse.IndexOf("٢٢");
                        verseText = (verse.SubVerse(0, idx) + verse.SubVerse(idx+1)).OsisVerseData;
                    }
                    else if (currentBook == "3John" && currentChapter == "1" && verseNum == "14")
                    {
                        previousVerse = verse;
                        continue;
                    }
                    else if (currentBook == "3John" && currentChapter == "1" && verseNum == "15")
                    {
                        verseText = (previousVerse + verse).OsisVerseData;
                        previousVerse = new Verse();
                        verseNum = "14";
                    }


                }

                sw.WriteLine(string.Format("<verse osisID='{0}.{1}.{2}'>{3}</verse>", currentBook, currentChapter, verseNum, verseText));


            }
            sw.WriteLine("</div>");

            currentBook = string.Empty;
            currentChapter = string.Empty;
        }

        private string GetTaggedVerse(string verseText, string strongPrefix)
        {
            string result = string.Empty;

            string text = verseText.Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Trim();
            string[] verseParts = text.Split(' ');
            string word = string.Empty;
            //string tag = string.Empty;
            List<string> words = new List<string>();
            List<string> tags = new List<string>();

            try
            {
                for (int i = 0; i < verseParts.Length; i++)
                {
                    string part = verseParts[i].Trim();
                    if (part[0] == '<')
                    {
                        try
                        {
                            // this is a tag
                            int idx = part.IndexOf('>');
                            string tag = part.Substring(1, idx - 1);
                            if (string.IsNullOrEmpty(tag) || tag.Contains("?"))
                                tag = "";
                            else
                            {
                                switch (tag.Length)
                                {
                                    case 1: tag = "000" + tag; break;
                                    case 2: tag = "00" + tag; break;
                                    case 3: tag = "0" + tag; break;
                                }
                            }
                            if (tag == "0000")
                                tag = "";
                            tags.Add(tag);
                        }
                        catch (Exception ex)
                        {
                            var cm = System.Reflection.MethodBase.GetCurrentMethod();
                            var name = cm.DeclaringType.FullName + "." + cm.Name;
                            Tracing.TraceException(name, ex.Message);
                        }


                    }
                    else
                    {
                        // this is a new word
                        if (tags.Count > 0)
                        {
                            if (tags.Count == 1 && tags[0] == "" || tags[0].Contains("???"))
                                words.Add(string.Format("<w>{0}</w>", word));
                            else
                            {
                                string strongStr = string.Empty;
                                if (tags[0] != "<>" && !tags[0].Contains("???"))
                                    strongStr = string.Format("strong:{0}{1}", strongPrefix, tags[0]);
                                for (int j = 1; j < tags.Count; j++)
                                {
                                    if (tags[j] != "<>" && !tags[j].Contains("???"))
                                        strongStr += string.Format(" strong:{0}{1}", strongPrefix, tags[j]);
                                }
                                if (string.IsNullOrEmpty(strongStr))
                                    words.Add(string.Format("<w>{0}</w>", word));
                                else
                                {
                                    //if((strongStr.Contains("H3068") || strongStr.Contains("H3069")) &&
                                    //        osisConf[OsisConstants.osisIDWork].ToLower().Contains("ara"))
                                    //    words.Add(string.Format("<hi type=\"bold\"><hi type=\"italic\"><w lemma=\"{0}\">{1}</w></hi></hi>", strongStr, word));
                                    //else
                                    words.Add(string.Format("<w lemma=\"{0}\">{1}</w>", strongStr, word));
                                }

                            }
                            word = string.Empty;
                            tags.Clear();
                        }
                        word += (string.IsNullOrEmpty(word) ? part : " " + part);
                    }
                }
                if (!string.IsNullOrEmpty(word))
                {
                    if (tags.Count == 0 || (tags.Count == 1 && tags[0] == ""))
                        words.Add(string.Format("<w>{0}</w>", word));
                    else
                    {
                        string strongStr = string.Format("strong:{0}{1}", strongPrefix, tags[0]);
                        for (int j = 1; j < tags.Count; j++)
                        {
                            strongStr += string.Format(" strong:{0}{1}", strongPrefix, tags[j]);
                        }
                        //if ((strongStr.Contains("H3068") || strongStr.Contains("H3069")) &&
                        //        osisConf[OsisConstants.osisIDWork].ToLower().Contains("ara"))
                        //    words.Add(string.Format("<hi type=\"bold\"><hi type=\"italic\"><w lemma=\"{0}\">{1}</w></hi></hi>", strongStr, word));
                        //else
                        words.Add(string.Format("<w lemma=\"{0}\">{1}</w>", strongStr, word));
                    }
                }

                for (int i = 0; i < words.Count; i++)
                {
                    result += (string.IsNullOrEmpty(result) ? words[i] : " " + words[i]);
                }

                return result;
            }
            catch (Exception ex)
            {
                var cm = System.Reflection.MethodBase.GetCurrentMethod();
                var name = cm.DeclaringType.FullName + "." + cm.Name;
                Tracing.TraceException(name, ex.Message);
                throw;
            }
        }

        #region osis file Helper methods
        private void WriteOsisStartTag(StreamWriter sw)
        {
            sw.WriteLine("<osis xmlns=\"http://www.bibletechnologies.net/2003/OSIS/namespace\"");
            sw.WriteLine("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"");
            sw.WriteLine("xsi:schemaLocation=\"http://www.bibletechnologies.net/2003/OSIS/namespace http://www.bibletechnologies.net/osisCore.2.1.1.xsd\">");
        }

        private void WriteOsisEndTag(StreamWriter sw)
        {
            sw.WriteLine("</osis>");
        }

        private void WriteOsisTextStartTag(StreamWriter sw)
        {
            try
            {
                sw.WriteLine(string.Format("<osisText osisIDWork=\"{0}\" osisRefWork=\"{1}\" xml:lang=\"{2}\" canonical=\"true\">",
                    osisConf[OsisConstants.osisIDWork],
                    osisConf[OsisConstants.osisRefWork],
                    osisConf[OsisConstants.language]));
            }
            catch (Exception ex)
            {
                var cm = System.Reflection.MethodBase.GetCurrentMethod();
                var name = cm.DeclaringType.FullName + "." + cm.Name;
                Tracing.TraceException(name, ex.Message);
                throw;
            }
        }
        private void WriteOsisTextEndTag(StreamWriter sw)
        {
            sw.WriteLine("</osisText>");
        }


        private void WriteHeader(StreamWriter sw, bool publicDomain = false)
        {
            try
            {
                sw.WriteLine(String.Format("<header>"));

                sw.WriteLine("<revisionDesc>");
                sw.WriteLine(String.Format("<date>{0}</date>", DateTime.Now.ToString("yyyy-MM-dd")));

                sw.WriteLine("<p>OSIS 2.1.1 version</p>");
                if (osisConf.ContainsKey(OsisConstants.revision))
                    sw.WriteLine(String.Format("<p>{0}</p>", osisConf[OsisConstants.revision]));

                sw.WriteLine("</revisionDesc>");

                sw.WriteLine(String.Format("<work osisWork=\"{0}\">", osisConf[OsisConstants.osisIDWork]));
                // title
                // contributor
                if (osisConf.ContainsKey(OsisConstants.contributor_role))
                    if (osisConf.ContainsKey(OsisConstants.contributor_name))
                        sw.WriteLine(String.Format("<contributor role=\"{0}\">{1}</contributor>", osisConf[OsisConstants.contributor_role], osisConf[OsisConstants.contributor_name]));
                    else
                        sw.WriteLine(String.Format("<contributor>{0}</contributor>", osisConf[OsisConstants.contributor_name]));
                // creator
                if (osisConf.ContainsKey(OsisConstants.creator_role))
                    if (osisConf.ContainsKey(OsisConstants.creator_name))
                        sw.WriteLine(String.Format("<creator role=\"{0}\">{1}</creator>", osisConf[OsisConstants.creator_name], osisConf[OsisConstants.creator_name]));
                    else
                        sw.WriteLine(String.Format("<creator role=\"{0}\">{1}</creator>", osisConf[OsisConstants.creator_name], osisConf[OsisConstants.creator_name]));
                // subject
                if (osisConf.ContainsKey(OsisConstants.subject))
                    sw.WriteLine(String.Format("<subject>{0}</subject>", osisConf[OsisConstants.subject]));
                // date
                sw.WriteLine(String.Format("<date>{0}</date>", DateTime.Now.ToString("yyyy-MM-dd")));
                // type
                if (osisConf.ContainsKey(OsisConstants.type))
                    sw.WriteLine(String.Format("<type type=\"OSIS\">{0}</type>", osisConf[OsisConstants.type]));
                // identifier
                if (osisConf.ContainsKey(OsisConstants.identifier))
                    sw.WriteLine(String.Format("<identifier type=\"OSIS\">{0}</identifier>", osisConf[OsisConstants.identifier]));
                // description
                if (publicDomain)
                    if (osisConf.ContainsKey(OsisConstants.description_pd))
                        sw.WriteLine(String.Format("<description>{0}</description>", osisConf[OsisConstants.description_pd]));
                    else
                    if (osisConf.ContainsKey(OsisConstants.description))
                        sw.WriteLine(String.Format("<description>{0}</description>", osisConf[OsisConstants.description_pd]));
                // language
                if (osisConf.ContainsKey(OsisConstants.language) && osisConf.ContainsKey(OsisConstants.language_type))
                    sw.WriteLine(String.Format("<language type=\"{0}\">{1}</language>", osisConf[OsisConstants.language_type], osisConf[OsisConstants.language]));
                // rights
                if (publicDomain)
                    if (osisConf.ContainsKey(OsisConstants.rights_pd))
                        sw.WriteLine(String.Format("<rights type=\"x-copyright\">{0}</rights>", osisConf[OsisConstants.rights_pd]));
                if (osisConf.ContainsKey(OsisConstants.rights))
                    sw.WriteLine(String.Format("<rights type=\"x-copyright\">{0}</rights>", osisConf[OsisConstants.rights]));
                // refSystem
                if (osisConf.ContainsKey(OsisConstants.refSystem))
                    sw.WriteLine(String.Format("<refSystem>{0}</refSystem>", osisConf[OsisConstants.refSystem]));

                sw.WriteLine(String.Format("</work>"));

                sw.WriteLine("<work osisWork=\"strong\">");
                sw.WriteLine("<refSystem>Dict.Strongs</refSystem>");
                sw.WriteLine("</work>");

                sw.WriteLine(String.Format("</header>"));
            }
            catch (Exception ex)
            {
                var cm = System.Reflection.MethodBase.GetCurrentMethod();
                var name = cm.DeclaringType.FullName + "." + cm.Name;
                Tracing.TraceException(name, ex.Message);
                throw;
            }
        }

        #endregion osis file Helper methods

    }
}
