﻿using BibleTaggingUtil.OsisXml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibleTaggingUtil
{
    public class ConfigurationHolder
    {
        private const string biblesConfigFile = "BiblesConfig.txt";

        private enum ParseState
        {
            NONE,
            TAGGING,
            OSIS,
            USFM,
            USFM2OSIS
        }
        private static ConfigurationHolder instance = null;
        private static readonly object lockObj = new object();

        public static ConfigurationHolder Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObj)
                    {
                        if (instance == null)
                        {
                            instance = new ConfigurationHolder();
                        }
                    }
                }
                return instance;
            }
        }


        private ConfigurationHolder()
        {
            HebrewReferences = new List<string>();
            GreekReferences = new List<string>();
            OSIS = new Dictionary<string, string>();
            USFM = new Dictionary<string, string>();
            USFM2OSIS = new Dictionary<string, string>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="biblesFolder"></param>
        /// <returns></returns>
        public string ReadBiblesConfig(string biblesFolder)
        {
            string configFilePath = Path.Combine(biblesFolder, biblesConfigFile);
            if (!File.Exists(configFilePath))
                return string.Format("File not found: {0}", configFilePath);

            //Properties.MainSettings.Default.TargetTextDirection = "LTR";

            ParseState state = ParseState.NONE;

            HebrewReferences.Clear();
            GreekReferences.Clear();
            OSIS.Clear();
            USFM.Clear();
            USFM2OSIS.Clear();

            string templine = string.Empty;

            using (StreamReader sr = new StreamReader(configFilePath))
            {
                bool osis = false;
                while (sr.Peek() >= 0)
                {
                    string line = sr.ReadLine().Trim();
                    if (string.IsNullOrEmpty(line))
                        continue;

                    if(line.StartsWith('['))
                    {
                        int end= line.IndexOf(']');
                        if (end == -1)
                            continue;
                        string section = line.Substring(1, end - 1);
                        switch (section.ToLower())
                        {
                            case "tagging":
                                state= ParseState.TAGGING; break;
                            case "osis":
                                state= ParseState.OSIS; break;
                            case "usfm":
                                state = ParseState.USFM; break;
                            case "usfm2osis":
                                state = ParseState.USFM2OSIS; break;
                        }
                        continue;
                    }
                    else
                    {
                        if (line.EndsWith("\\"))
                        {
                            templine += "\n" + line.TrimEnd('\\');
                            continue;
                        }
                        line = templine + "\n" + line;
                        line = line.TrimStart('\n');
                        templine = string.Empty;
                    }

                    int equal = line.IndexOf("=");
                    if (equal == -1) continue;

                    string[] parts = new string[2];
                    parts[0] = line.Substring(0, equal).Trim();
                    parts[1] = line.Substring(equal + 1).Trim();



                    if (state == ParseState.TAGGING)
                    {
                        switch (parts[0].Trim().ToLower())
                        {
                            case "untaggedbible":
                                UnTaggedBible = Path.Combine(biblesFolder, parts[1].Trim());
                                break;
                            case "taggedbible":
                                TaggedBible = Path.Combine(biblesFolder, "tagged\\" + parts[1].Trim());
                                break;
                            case "kjv":
                                KJV = Path.Combine(biblesFolder, parts[1].Trim());
                                break;
                            case "topreferenceversion":
                                TopReferenceVersion = parts[1].Trim();
                                break;
                            case "hebrewreferences":
                                {
                                    string[] hebParts = parts[1].Split(',');
                                    for (int i = 0; i < hebParts.Length; i++)
                                    {
                                        HebrewReferences.Add(Path.Combine(biblesFolder, hebParts[i]));
                                    }
                                }
                                break;
                            case "greekreferences":
                                {
                                    string[] grkParts = parts[1].Split(',');
                                    for (int i = 0; i < grkParts.Length; i++)
                                    {
                                        GreekReferences.Add(Path.Combine(biblesFolder, grkParts[i]));
                                    }
                                }
                                break;
                            //case "targettextdirection":
                            //    Properties.MainSettings.Default.TargetTextDirection = parts[1].Trim();
                            //    break;
                            case "osis":
                                if(parts[1].Trim().ToLower() == "true") osis = true;
                                break;
                        }
                    }

                    else if(state == ParseState.OSIS)
                    {
                        OSIS[parts[0]] = parts[1];
                    }

                    else if (state == ParseState.USFM)
                    {
                        USFM[parts[0]] = parts[1];
                    }

                   else if (state == ParseState.USFM2OSIS)
                    {
                        USFM2OSIS[parts[0]] = parts[1];
                    }

                }

                Properties.MainSettings.Default.Osis = osis;
            }
            return string.Empty;
        }

        public string UnTaggedBible { get; private set; }
        public string TaggedBible { get; private set; }
        public string TopReferenceVersion { get; private set; }

        public string KJV { get; private set; }
        public List<string> HebrewReferences { get; private set; }
        public List<string> GreekReferences { get; private set; }

        public Dictionary<string, string> OSIS { get; private set; }
        public Dictionary<string, string> USFM { get; private set; }
        public Dictionary<string, string> USFM2OSIS { get; private set; }


    }


}
