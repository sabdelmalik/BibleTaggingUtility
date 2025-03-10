﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

using BibleTaggingUtil;

namespace SM.Bible.Formats.USFM
{
    public class USFM_Generator
    {
        Dictionary<string, Dictionary<int, USFM_Entry>> bible = new Dictionary<string, Dictionary<int, USFM_Entry>>();
        Dictionary<string, EntryMap> verseMap = new Dictionary<string, EntryMap>();
        string bibleVplFile = string.Empty;
        BibleTaggingForm parent;

        Dictionary<string, string> usfmConf;
        string taggedFolder = string.Empty;
        string VersionTitle = string.Empty;
        string usfmRefFolder = string.Empty;

        TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

        public USFM_Generator(BibleTaggingForm parent, ConfigurationHolder config )
        {
            this.parent = parent;

            this.usfmConf = config.USFM;
        }


        //****
        string currentFileName = string.Empty;
        string currentBook = string.Empty;
        int currentChapter = 0;

        int lineIndex = 0;
        //****
         public void Generate()
        {
            ReadUSFM();

            foreach (string key in parent.Target.Bible.Keys)
            {
                string reference = key;
                int idx = reference.IndexOf(' ');
                string altName = reference.Substring(0, idx);
                string usbName = string.Empty;
                int bookIndex = Array.IndexOf(Constants.osisAltNames, textInfo.ToTitleCase(altName));
                if (Constants.osisAltNames.Contains(altName, StringComparer.OrdinalIgnoreCase))
                    usbName = Constants.ubsNames[bookIndex];
                else
                {
                    Console.WriteLine("Error parsing " +  key);
                }

                reference = reference.Replace(altName, usbName).ToUpper();

                EntryMap map = verseMap[reference];
                USFM_Entry entry = bible[usbName.ToUpper()][map.Index];
                string usfmText = entry.Text;

                if(usbName.ToUpper() == "1CO" )// && entry.Marker == "\\c" && entry.Number == 16)
                {
                    int x = 0;
                }
                entry.Text = MergeVerse(parent.Target.Bible[key], usfmText, bookIndex);

            }
            SaveUSFMBible();
        }

        private string MergeVerse(Verse verseWords, string usfm, int bookIndex)
        {
            string result = string.Empty;
            List<string> words = new List<string>();
            List<string> tags = new List<string>();

            String prfx = (bookIndex > 38) ? "G" : "H";
            for (int i = 0; i < verseWords.Count; i++)
            {
                words.Add(verseWords[i].Word);
                tags.Add(verseWords[i].Strong.ToString());
/*                string tmpTag = string.Empty;
                for (int j = 0; j < verseWords[i].Strong.Length; j++)
                    if(!string.IsNullOrEmpty(verseWords[i].Strong[j]))
                       tmpTag += prfx + verseWords[i].Strong[j] + " ";
                tags.Add(tmpTag.Trim());
*/
            }

//            for (int i = 0; i < tags.Count; i++)
//            {
//                tags[i] = tags[i].Replace("<", prfx).Replace(">", "");
//            }

            // \v 1 \w فِي الْبَدْءِ|strong="H7225"\w* \w خَلَقَ|strong="H1254"\w* \w اللهُ|strong="H0430"\w* \w السَّمَاوَاتِ|strong="H8064"\w* \w وَالأَرْضَ|strong="H0776"\w*. 
            for (int i = 0; i < words.Count; i++)
            {
                if (i >= tags.Count || string.IsNullOrEmpty(tags[i]))
                    result += string.Format(" \\w {0}\\w*", words[i]);
                else
                    result += string.Format(" \\w {0}|strong=\"{1}\"\\w*", words[i], tags[i]);
            }
    
            return result.Trim();
        }

        private void ReadUSFM()
        {
            string targetBiblesFolder = BibleTaggingUtil.Properties.TargetBibles.Default.TargetBiblesFolder;
            string targetBible = BibleTaggingUtil.Properties.TargetBibles.Default.TargetBible;
            string biblesFolder = Path.Combine(targetBiblesFolder, targetBible);

            string usfmRefFolder = Path.Combine(biblesFolder, usfmConf[UsfmConstants.usfmRefFolder]);
            string[] files = Directory.GetFiles(usfmRefFolder, "*.usfm");
            foreach (string file in files)
            {
                ParseBook(file);
            }
        }

        private void ParseBook(string file)
        {
            currentFileName = Path.GetFileName(file);
            Dictionary<int, USFM_Entry> book = new Dictionary<int, USFM_Entry>();

            using (StreamReader sr = new StreamReader(file))
            {
                while (!sr.EndOfStream)
                {
                    string? line = sr.ReadLine();
                    if (line is not null)
                    {
                        if (file.Contains("20-PSAarb-vd") && line.Contains("c 119"))
                        {
                            int x = 0;
                        }

                        ParseLine(line.Trim(), book);
                    }
                }

              if (!string.IsNullOrEmpty(currentBook) && !bible.ContainsKey(currentBook))
                    bible[currentBook] = book;
            }
        }

        private void ParseLine(string line, Dictionary<int, USFM_Entry> book)
        {
            if (string.IsNullOrEmpty(line))
                return;
            USFM_Entry entry = new USFM_Entry();

            string marker = line;
            int space1 = line.IndexOf(' ');
            if (space1 > -1)
            {
                marker = line.Substring(0, space1);
            }
            entry.Marker = marker;
            int space2 = -1;
            switch (marker)
            {
                case @"\id":
                    if (line.Length > space1 + 1)
                    {
                        string bookLine = line.Substring(space1 + 1).Trim();
                        currentBook = bookLine;
                        string otherBookInfo = string.Empty;
                        space2 = bookLine.IndexOf(' ');
                        if (space2 > -1)
                        {
                            currentBook = bookLine.Substring(0, space2);
                            otherBookInfo = bookLine.Substring(space2 + 1).Trim();
                        }
                        else
                        {
                            otherBookInfo = string.Format("{0}, {1}", currentFileName, VersionTitle);
                        }
                        entry.Code = currentBook;
                        entry.Text = otherBookInfo;
                    }
                    break;

                case @"\usfm":
                case @"\ide":
                case @"\h":
                case @"\qa":
                case @"\cl":
                case @"\p":
                    if (space1 >= 0 && line.Length > space1 + 1)
                        entry.Text = line.Substring(space1 + 1);
                    break;
                case @"\c":
                    if (line.Length > 3)
                    {
                        int.TryParse(line.Substring(3), out currentChapter);
                        entry.Number = currentChapter;
                    }
                    break;
                case @"\v":
                    space2 = line.IndexOf(' ', 3);
                    if (space2 > -1)
                    {
                        int verseNum = 0;
                        bool success = int.TryParse(line.Substring(3, space2 - 3), out verseNum);
                        entry.Number = verseNum;
                        entry.Text = line.Substring(space2 + 1);
                        string verseRef = string.Format("{0} {1}:{2}", currentBook, currentChapter, verseNum);
                        EntryMap map = new EntryMap();
                        map.Book = currentBook;
                        map.Index = lineIndex;
                        verseMap[verseRef] = map;
                    }
                    break;
                default:
                    if (marker.StartsWith(@"\toc") ||
                        marker.StartsWith(@"\mt") ||
                        marker.StartsWith(@"\s"))
                    {
                        if (line.Length > space1 + 1)
                            entry.Text = line.Substring(space1 + 1);
                    }
                    break;
            }

            book[lineIndex++] = entry;
        }

        public void SaveUSFMBible()
        {
            string targetBiblesFolder = BibleTaggingUtil.Properties.TargetBibles.Default.TargetBiblesFolder;
            string targetBible = BibleTaggingUtil.Properties.TargetBibles.Default.TargetBible;
            string biblesFolder = Path.Combine(targetBiblesFolder, targetBible);
            string usfmOutFolder = Path.Combine(biblesFolder, usfmConf[UsfmConstants.usfmOutFolder]);
            if (!Directory.Exists(usfmOutFolder))
            {
                Directory.CreateDirectory(usfmOutFolder);
            }

            foreach(Dictionary<int, USFM_Entry> book in bible.Values)
            {
                string fileName = string.Empty;

                List<USFM_Entry> entries = book.Values.ToList();

                USFM_Entry id = entries[0];
                TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                string code = textInfo.ToTitleCase(id.Code.ToLower());
                  
                if (Constants.ubsNames.Contains(code, StringComparer.OrdinalIgnoreCase))
                {
                    string fileNamePrefix = Constants.usfmFileNamePrefixes[Array.IndexOf(Constants.ubsNames, textInfo.ToTitleCase(code))];
                    fileName = string.Format("{0}{1}-{2}.{3}",
                                        fileNamePrefix.ToUpper(),
                                        usfmConf[UsfmConstants.usfmLang].ToLower(),
                                        usfmConf[UsfmConstants.usfmId].ToLower(),
                                        usfmConf[UsfmConstants.fileExtension]);
                }

                id.Text = fileName + ", " + usfmConf[UsfmConstants.versionTitle];

                if(string.IsNullOrEmpty(fileName))
                {
                    Console.WriteLine("Could not identify file name: id = " + id.Code);
                    continue;
                }

                string filePath = Path.Combine(usfmOutFolder, fileName);
                if (File.Exists(filePath))
                    File.Delete(filePath);

                using (StreamWriter sw = new StreamWriter(new FileStream(filePath, FileMode.Create), Encoding.UTF8))
                {
                    for(int idx = 0; idx < entries.Count; idx++)
                    {
                        string line = entries[idx].Marker;
                        if (entries[idx].HasNumber)
                            line += (" " + entries[idx].Number.ToString());
                        if (entries[idx].HasCode)
                            line += (" " + entries[idx].Code);
                        if (entries[idx].HasText)
                            line += (" " + entries[idx].Text);

                        sw.WriteLine(line);
                    }

                }
             }
        }
    }
}

