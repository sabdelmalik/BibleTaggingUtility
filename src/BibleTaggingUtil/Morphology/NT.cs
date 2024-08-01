using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BibleTaggingUtil.Morphology
{
    internal class NT
    {
        private Dictionary<string, string> partsOfSpeach = new Dictionary<string, string>()
            {
                {"A", "Adjective"},
                {"ADV", "Adverb"},
                {"C", "Reciprocal pronoun"},
                {"COND", "Conditional particle or conjunction"},
                {"CONJ", "Conjunction"},
                {"D", "Demonstrative pronoun"},
                {"F", "Reflexive pronoun"},
                {"HEB", "Hebrew transliterated word"},
                {"I", "Interrogative pronoun"},
                {"INJ", "Interjection"},
                {"K", "Correlative pronoun"},
                {"N", "Noun"},
                {"NUI", "Adjective - Indeclinable Numeral"},
                {"P", "Personal pronoun"},
                {"PREP", "Preposition"},
                {"PRT", "Particle"},
                {"Q", "Correlative or Interrogative pronoun"},
                {"R", "Relative pronoun"},
                {"S", "Posessive pronoun"},
                {"T", "Article"},
                {"ARAF", "Aramaic transliterated word"},
                {"ARAM", "Aramaic transliterated word"},
                {"ARAN", "Aramaic transliterated word"},
                {"V", "Verb"},
                {"X", "Indefinite pronoun"}
            };

        private Dictionary<char, string> tense = new Dictionary<char, string>()
        {
            {'A', "Aorist"},
            {'F', "Future"},
            {'I', "Imperfect"},
            {'L', "Pluperfect"},
            {'P', "Present"},
            {'R', "Perfect"},
            {'Y', "Pluperfect"},
            {'X', "indefinite tense"}
        };

        private Dictionary<char, string> voice = new Dictionary<char, string>()
        {
            {'A', "Active"},
            {'D', "Middle Deponent"},
            {'E', "Either Middle or Passive"},
            {'M', "Middle"},
            {'N', "Middle or Passive Deponent"},
            {'O', "Passive Deponent"},
            {'P', "Passive"},
            {'Q', "impersonal active"},
            {'X', "indefinite voice"}
        };

        private Dictionary<char, string> mood = new Dictionary<char, string>()
        {
            {'M', "Imperative"},
            {'I', "Indicative"},
            {'N', "Infinitive"},
            {'O', "Optative"},
            {'P', "Participle"},
            {'S', "Subjunctiv"}
        };

        private Dictionary<char, string> morfCase = new Dictionary<char, string>()
        {
            {'A', "Accusative"},
            {'D', "Dative"},
            {'G', "Genitive"},
            {'N', "Nominative"},
            {'V', "Vocative"}
        };

        private Dictionary<char, string> gender = new Dictionary<char, string>()
        {
            {'M', "Masculine"},
            {'F', "Feminine"},
            {'N', "Neuter"}
        };
        private Dictionary<char, string> person = new Dictionary<char, string>()
        {
            {'1', "First"},
            {'2', "Second"},
            {'3', "Third"}
        };

        private Dictionary<char, string> number = new Dictionary<char, string>()
        {
            {'S', "Singular"},
            {'P', "Plural"}
        };

        public NT()
        {
            //partsOfSpeach = new Dictionary<string, string>()
            //{
            //    {"A", "Adjective"},
            //    {"ADV", "Adverb"},
            //    {"CONJ", "Conjunction"},
            //    {"N", "Noun"},
            //    {"P", "Pronoun"},
            //    {"PREP", "Preposition"},
            //    {"PRT", "Particle"},
            //    {"T", "Article"},
            //    {"V", "Verb"}
            //};
        }
        public string GetMorphologyDetails(string morf)
        {
            string result = morf;
            if (morf != null)
            {
                string[] morfParts = morf.Split(new char[] { '-' });
                result = partsOfSpeach[morfParts[0]];
                if (morfParts.Length > 1)
                {
                    result += "\r\n";
                    switch (morfParts[0])
                    {
                        case "V":
                            result += ParseVerbMorf(morfParts);
                            break;

                        case "A":
                        case "N":
                        case "T":
                        case "P":
                            if (morfParts[1].Length == 3)
                            {
                                bool specific = true;
                                switch (morfParts[1][0])
                                {
                                    case '1':
                                        result += "1st ";
                                        break;
                                    case '2':
                                        result += "2nd ";
                                        break;
                                    case '3':
                                        result += "3rd ";
                                        break;
                                    default:
                                        specific = false;
                                        break;

                                }
                                result += specific? 
                                    ParseNounMorf(morfParts[1].Substring(1)) : 
                                    ParseNounMorf(morfParts[1]);
                            }
                            break;

                        case "F":
                            if (morfParts[1].Length == 4)
                            {
                                switch (morfParts[1][0])
                                {
                                    case '1':
                                        result += "1st ";
                                        break;
                                    case '2':
                                        result += "2nd ";
                                        break;
                                    case '3':
                                        result += "3rd ";
                                        break;
                                }
                                result += ParseNounMorf(morfParts[1].Substring(1));
                            }
                            break;

                        case "S":
                            if (morfParts[1].Length == 5)
                            {
                                string pre = morfParts[1].Substring(0,2);
                                switch (pre)
                                {
                                    case "1S":
                                        result += "1st Singular ";
                                        break;
                                    case "2S":
                                        result += "2nd Singular ";
                                        break;
                                    case "1P":
                                        result += "1st Plural ";
                                        break;
                                    case "2P":
                                        result += "2nd Plural ";
                                        break;
                                }
                                result += ParseNounMorf(morfParts[1].Substring(2));
                            }
                            break;

                        default:
                            result += "\r\n" + morf;
                            break;

                    }

                    
                }
            }

            return result;

        }

        private string ParseVerbMorf(string[] morfParts)
        {
            string result = string.Empty;

            string verbMorf1 = morfParts[1];
            string aorist = string.Empty;
            if(verbMorf1.Length == 4)
            {
                switch (verbMorf1[0])
                {
                    case '1':
                        aorist += "1st ";
                        break;
                    case '2':
                        aorist += "2nd ";
                        break;
                    case '3':
                        aorist += "3rd ";
                        break;
                }
                verbMorf1 = verbMorf1.Substring(1);
            }
            if (verbMorf1.Length == 3) 
            {
                result += "Tense: " + aorist + tense[verbMorf1[0]] + "\r\n";
                result += "Voice: " + voice[verbMorf1[1]] + "\r\n";
                result += "Mood: " + mood[verbMorf1[2]] + "\r\n";

                if(morfParts.Length == 3)
                {
                    string verbMorf2 = morfParts[2];

                    if (verbMorf1[2] == 'P' && verbMorf2.Length == 3) 
                    {
                        result += ParseNounMorf(verbMorf2);
                    }
                    else if(verbMorf2.Length == 2) 
                    {
                        result += "Person: " + person[verbMorf2[0]] + "\r\n";
                        result += "Number: " + number[verbMorf2[1]] + "\r\n";
                    }
                }
            }
            


            return result;
        }

        private string ParseNounMorf(string morfString)
        {
            string result = string.Empty;
            if (morfString == "NUI")
                result = "Indeclinable Numeral";
            else if (morfString == "PRI")
                result = "Indeclinable Proper Noun";

            else if (morfString.Length == 3)
            {
                result += "Case: " + morfCase[morfString[0]] + "\r\n";
                result += "Number: " + number[morfString[1]] + "\r\n";
                result += "Gender: " + gender[morfString[2]] + "\r\n";
            }
            else if (morfString.Length == 2)
            {
                result += "Case: " + morfCase[morfString[0]] + "\r\n";
                result += "Number: " + number[morfString[1]] + "\r\n";
            }

                return result;
        }
    }
}
