using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BibleTaggingUtil.Strongs
{
    /// <summary>
    /// 
    /// </summary>
    public class StrongsNumber : ICloneable
    {
        public string Prefix { get; }

        /// <summary>
        /// Numeric value of Strong Number
        /// </summary>
        public int Number { get; }

        /// <summary>
        /// Disambiguated character
        /// </summary>
        public string Disambiguation { get; }

        /// <summary>
        /// Occurrence in the verse (A or a: first B or b:Second ....)
        /// </summary>
        public int Occurance { get; set; }
        public int CountInVerse { get; set; }

        private string occurance;

        public StrongsNumber(string st) 
        {
            Prefix = string.Empty;
            Number = 0;
            Disambiguation = string.Empty;
            Occurance = 0;

            if(string.IsNullOrEmpty(st)) { return; }

            // get rid of enclosing angle brackets
            string temp = st.Replace("<", "").Replace(">", "").Trim();
            if (string.IsNullOrEmpty(temp)) { return; }


            string pattern = @"([GH])([\d]{1,5})([a-zA-Z]{0,2})[_]{0,1}([a-zA-Z]{0,1})";
            Match m = Regex.Match(temp, pattern);
            if (m != null && m.Groups.Count > 2)
            {
                Prefix = m.Groups[1].Value;
                Number = int.Parse(m.Groups[2].Value);
                Disambiguation = m.Groups[3].Value;
                occurance = m.Groups[4].Value;
                if(string.IsNullOrEmpty(occurance)) { Occurance = 0; }
                else
                {
                    occurance = occurance.ToUpper();
                    Occurance = occurance[0] - 0x40;
                }
            }
        }

        public void SetOccurance(int occ)
        {
            if(occ > 0)
            {
                Occurance = occ;
                occurance = ((char)(occ + 0x40)).ToString();
            }
        }

        public bool IsEmpty
        {
            get { return Prefix == string.Empty; }
        }

        public override string ToString()
        {
            string result = string.Empty;
            if (Prefix != string.Empty)
            {
                if (string.IsNullOrEmpty(occurance))
                    result = string.Format("{0}{1:d4}{2}", Prefix, Number, Disambiguation, occurance);
                else
                    result = string.Format("{0}{1:d4}{2}_{3}", Prefix, Number, Disambiguation, occurance);
            }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Simple Strong's number</returns>
        public string ToStringS()
        {
            string result = string.Empty;
            if (Prefix != string.Empty)
            {
                result = string.Format("{0}{1:d4}", Prefix, Number);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Disambiguated Strong's number</returns>
        public string ToStringD()
        {
            string result = string.Empty;
            if (Prefix != string.Empty)
            {
                result = string.Format("{0}{1:d4}{2}", Prefix, Number, Disambiguation);
            }
            return result;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }


}
