using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Configuration.Internal;
using System.Windows.Forms;
using System.Reflection;
using BibleTaggingUtil.BibleVersions;

namespace BibleTaggingUtil
{
    public class OSIS_Generator
    {
        Dictionary<string, string> osisConf = new Dictionary<string, string>();
        string bibleVplFile = string.Empty;

        public OSIS_Generator(ConfigurationHolder config )
        {
            // bible-vpl-file=AraSVD_2023_01_08_17_56.txt
            this.osisConf = config.OSIS;
            string taggedFolder = Path.GetDirectoryName(config.TaggedBible);
            String[] existingTagged = Directory.GetFiles(taggedFolder, "*.*");
            if(existingTagged.Length != 1)
            {
                MessageBox.Show("Was expectin one file to convert - OSIS generation Failed");
                return;
            }
            bibleVplFile = existingTagged[0];
        }


        /// <summary>");
        /// 
        /// </summary>");
        /// <param name="vplFile">");Verse perline file</param>");
        public void Generate(TargetVersion targetVersion, bool publicDomain = false)
        {
            //string bibleVplFile = string.Empty;
            string otVplFile = string.Empty;
            string ntVplFile = string.Empty;
            string outputFile = string.Empty;
            bool forInjeel = false;

            try
            {
                string targetBiblesFolder = Properties.TargetBibles.Default.TargetBiblesFolder;
                string targetBible = Properties.TargetBibles.Default.TargetBible;
                string biblesFolder = Path.Combine(targetBiblesFolder, targetBible);

                //                if (osisConf.ContainsKey(OsisConstants.bible_vpl_file))
                //                    bibleVplFile = osisConf[OsisConstants.bible_vpl_file];

                if (osisConf.ContainsKey(OsisConstants.forInjeel))
                {
                    string temp = osisConf[OsisConstants.forInjeel];
                    forInjeel = (temp.ToLower() == "true");
                }

                if (osisConf.ContainsKey(OsisConstants.ot_vpl_file))
                    otVplFile = osisConf[OsisConstants.ot_vpl_file];

                if (osisConf.ContainsKey(OsisConstants.nt_vpl_file))
                    ntVplFile = osisConf[OsisConstants.nt_vpl_file];

                if (osisConf.ContainsKey(OsisConstants.output_file))
                {
                    if (publicDomain)
                        outputFile = osisConf[OsisConstants.output_file_publicDomaind];
                    else
                        outputFile = osisConf[OsisConstants.output_file];
                }


                if (string.IsNullOrEmpty(outputFile))
                    throw new Exception("Configuration must contain an entry for output-file or output-file");

                if (string.IsNullOrEmpty(otVplFile) && string.IsNullOrEmpty(ntVplFile))
                    throw new Exception("Configuration must contain atleast one entry of ot-vpl-file and nt-vpl-file");

                if (!string.IsNullOrEmpty(bibleVplFile))
                {
                    // we have a complete bible file that we need to split into ot and nt
                    if (string.IsNullOrEmpty(otVplFile) || string.IsNullOrEmpty(ntVplFile))
                        throw new Exception("Configuration must contain ot-vpl-file and nt-vpl-file defined because bible-vpl=file is defined");

                    string otFilePath = Path.Combine(biblesFolder, otVplFile);
                    if (File.Exists(otFilePath))
                    {
                        string buFileName = string.Format("{0}_{1}.txt", Path.GetFileNameWithoutExtension(otVplFile), DateTime.Now.ToString("yyyy_MM_dd_HH_mm"));
                        File.Move(otFilePath, Path.Combine(biblesFolder, buFileName));
                    }
                    string ntFilePath = Path.Combine(biblesFolder, ntVplFile);
                    if (File.Exists(ntFilePath))
                    {
                        string buFileName = string.Format("{0}_{1}.txt", Path.GetFileNameWithoutExtension(ntVplFile), DateTime.Now.ToString("yyyy_MM_dd_HH_mm"));
                        File.Move(ntFilePath, Path.Combine(biblesFolder, buFileName));
                    }

                    using (StreamReader sr = new StreamReader(Path.Combine(biblesFolder, bibleVplFile)))
                    using (StreamWriter otWriter = new StreamWriter(otFilePath))
                    using (StreamWriter ntWriter = new StreamWriter(ntFilePath))
                    {
                        while (!sr.EndOfStream)
                        {
                            string line = sr.ReadLine();
                            int space = line.IndexOf(' ');
                            string bookName = line.Substring(0, space);
                            // int bookIndex = Array.IndexOf(OsisConstants.osisAltNames, bookName);
                            int bookIndex = targetVersion.GetBookIndex(bookName);
                            if (bookIndex < 0)
                            {
                                throw new Exception("book name not found");
                            }
                            if (bookIndex > 38)
                            {
                                ntWriter.WriteLine(line);
                            }
                            else
                            {
                                otWriter.WriteLine(line);
                            }


                        }

                    }

                }

                string osisFile = Path.Combine(biblesFolder, outputFile);
                if (File.Exists(osisFile))
                    File.Delete(osisFile);

                using (StreamWriter sw = new StreamWriter(osisFile))
                {
                    sw.WriteLine("<?xml version='1.0' encoding='UTF-8'?>");
                    WriteOsisStartTag(sw);
                    WriteOsisTextStartTag(sw);

                    WriteHeader(sw, publicDomain);

                    if (!string.IsNullOrEmpty(otVplFile))
                    {
                        using (StreamReader sr = new StreamReader(Path.Combine(biblesFolder, otVplFile)))
                        {

                            WriteBible(sr, sw, "H", targetVersion, forInjeel, publicDomain);
                        }
                    }


                    if (!string.IsNullOrEmpty(ntVplFile))
                    {
                        using (StreamReader sr = new StreamReader(Path.Combine(biblesFolder, ntVplFile)))
                        {

                            WriteBible(sr, sw, "G", targetVersion, forInjeel, publicDomain);
                        }
                    }

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

        private void WriteBible(StreamReader sr, StreamWriter sw, string strongPrefix, TargetVersion targetVersion, bool forInjeel, bool publicDomain = false)
        {
            sw.WriteLine("<div type=\"bookGroup\">");

            WriteBibleBooks(sr, sw, strongPrefix, targetVersion, forInjeel, publicDomain);

            sw.WriteLine("</div>");

            currentBook = string.Empty;
            currentChapter = string.Empty;
        }

        string currentBook = string.Empty;
        string currentChapter = string.Empty;
        private void WriteBibleBooks(StreamReader sr, StreamWriter sw, string strongPrefix, TargetVersion targetVersion, bool forInjeel, bool publicDomain = false)
        {
            try
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    //if (publicDomain)
                    //    line = Utils.RemoveDiacritics(line);

                    int bs = line.IndexOf(' ');
                    string bookName = line.Substring(0, bs);
                    string bookOsisName = OsisConstants.osisNames[targetVersion.GetBookIndex(bookName)];    //Array.IndexOf(OsisConstants.osisAltNames, bookName)];
                    int idx1 = line.IndexOf(' ');
                    int idx2 = line.IndexOf(':', idx1 + 1);
                    int idx3 = line.IndexOf(' ', idx2 + 1);
                    string chapter = line.Substring(idx1 + 1, idx2 - idx1 - 1);
                    string verse = line.Substring(idx2 + 1, idx3 - idx2 - 1);
                    string verseText = line.Substring(idx3 + 1);

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
                            if (strongPrefix == "G" && osisConf[OsisConstants.osisIDWork].ToLower().Contains("ara"))
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

                    if (currentChapter != chapter)
                    {
                        // this is a new chapter
                        if (!string.IsNullOrEmpty(currentChapter))
                        {
                            // we are at the end of a chapter
                            sw.WriteLine("</chapter>");
                        }
                        currentChapter = chapter;
                        // start tag for the new chapter
                        sw.WriteLine(string.Format("<chapter osisID='{0}.{1}'>", currentBook, currentChapter));
                    }
                    // Handle Psalm headers - enclose in <hebrewTitle> tag for Arbic Bible
                    if(osisConf[OsisConstants.osisIDWork].ToLower().Contains("ara") && 
                        (currentBook == "Ps" && verse == "1"))
                    {
                        int astrisk = verseText.IndexOf(" * ");
                        if (astrisk > 1)
                        {
                            #region extract title without tags
                            string title = string.Empty;
                            string temp = verseText.Substring(0, astrisk + 1);
                            string title1 = GetTaggedVerse(temp, strongPrefix);
                            int startOfWord = 0;
                            int startOfTag = 0;
                            while (true)
                            {
                                startOfTag = temp.IndexOf(" <", startOfWord);
                                if (startOfTag != startOfWord)
                                {
                                    if (startOfTag == -1)
                                    {
                                        if (startOfWord < temp.Length - 1)
                                            title = temp.Substring(startOfWord);
                                        break;
                                    }
                                    string segment = temp.Substring(startOfWord, startOfTag - startOfWord);
                                    title += segment;

                                }

                                startOfWord = temp.IndexOf(">", startOfTag) + 1;


                            }

                            #endregion
                            sw.WriteLine(string.Format("<title canonical=\"true\" type=\"psalm\">{0}</title>", title1));

                            int astrisk2 = verseText.IndexOf(" * <>");
                            if (astrisk2 > 1)
                                verseText = verseText.Substring(astrisk2 + 6);
                            else
                                verseText = verseText.Substring(astrisk + 4);
                        }

                    }

                    if(forInjeel)
                    {
                        if (currentBook == "Ezra" && currentChapter == "5" && verse == "6")
                        {
                            previousText = verseText;
                            continue;
                        }
                        else if (currentBook == "Ezra" && currentChapter == "5" && verse == "7")
                        {
                            // need to split, combine, save, the the remainaing text
                            string endtext = "هكَذَا: <1836>";
                            int idx = verseText.IndexOf(endtext);
                            string verse6 = previousText + " " + verseText.Substring(0, idx+endtext.Length);
                            sw.WriteLine(string.Format("<verse osisID='{0}.{1}.{2}'>{3}</verse>", currentBook, currentChapter, "6", GetTaggedVerse(verse6, strongPrefix)));
                            previousText = string.Empty;
                            verseText = verseText.Substring(idx+endtext.Length).Trim();
                        }
                        else if (currentBook == "Dan" && currentChapter == "2" && verse == "14")
                        {
                            previousText = verseText;
                            continue;
                        }
                        else if (currentBook == "Dan" && currentChapter == "2" && verse == "15")
                        {
                            // need to split, combine, save, the the remainaing text
                            string endtext = "الْمَلِكِ: <4430>";
                            int idx = verseText.IndexOf(endtext);
                            string verse14 = previousText + " " + verseText.Substring(0, idx + endtext.Length);
                            sw.WriteLine(string.Format("<verse osisID='{0}.{1}.{2}'>{3}</verse>", currentBook, currentChapter, "14", GetTaggedVerse(verse14, strongPrefix)));
                            previousText = string.Empty;
                            verseText = verseText.Substring(idx + endtext.Length).Trim();
                        }
                        else if (currentBook == "1Tim" && currentChapter == "6" && verse == "21")
                        {
                            verseText = verseText.Replace(" ٢٢ <>", "");
                        }
                        else if (currentBook == "3John" && currentChapter == "1" && verse == "14")
                        {
                            previousText = verseText;
                            continue;
                        }
                        else if (currentBook == "3John" && currentChapter == "1" && verse == "15")
                        {
                            verseText = previousText + " " + verseText;
                            previousText = string.Empty;
                            verse = "14";
                        }


                    }

                    sw.WriteLine(string.Format("<verse osisID='{0}.{1}.{2}'>{3}</verse>", currentBook, currentChapter, verse, GetTaggedVerse(verseText, strongPrefix)));
                }
                // end tags for the lastchapter / last book
                sw.WriteLine("</chapter>");
                sw.WriteLine("</div>");
            }
            catch (Exception ex)
            {
                var cm = System.Reflection.MethodBase.GetCurrentMethod();
                var name = cm.DeclaringType.FullName + "." + cm.Name;
                Tracing.TraceException(name, ex.Message);
                throw;
            }

        }

        string previousText = string.Empty;
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
                                if(tags[0] != "<>" && !tags[0].Contains("???"))
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

    }
}
