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
                {"CONJ", "Conjunction"},
                {"N", "Noun"},
                {"P", "Pronoun"},
                {"PREP", "Preposition"},
                {"PRT", "Particle"},
                {"T", "Article"},
                {"V", "Verb"}
            };

        private Dictionary<char, string> tense = new Dictionary<char, string>()
        {
            {'A', "Aorist"},
            {'F', "Future"},
            {'I', "Imperfect"},
            {'P', "Present"},
            {'X', "Perfect"},
            {'Y', "Pluperfect"}
        };

        private Dictionary<char, string> voice = new Dictionary<char, string>()
        {
            {'A', "Active"},
            {'M', "Middle"},
            {'P', "Passive"}
        };

        private Dictionary<char, string> mood = new Dictionary<char, string>()
        {
            {'M', "Imperative"},
            {'I', "Indicative"},
            {'N', "Infinitive"},
            {'P', "Participle"},
            {'S', "Subjunctiv"}
        };

        private Dictionary<char, string> morfCase = new Dictionary<char, string>()
        {
            {'A', "Accusative"},
            {'D', "Dative"},
            {'G', "Genitive"},
            {'N', "Nominative"}
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
                                result += ParseNounMorf(morfParts[1]);
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
                aorist += verbMorf1[0] + " ";
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

            if (morfString.Length == 3)
            {
                result += "Case: " + morfCase[morfString[0]] + "\r\n";
                result += "Number: " + number[morfString[1]] + "\r\n";
                result += "Gender: " + gender[morfString[2]] + "\r\n";
            }

                return result;
        }
    }
}
