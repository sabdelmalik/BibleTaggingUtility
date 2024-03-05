using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibleTaggingUtil.Strongs
{
    public class StrongsCluster
    {
        private List<StrongsNumber> strongs;

        public StrongsCluster() { strongs = new List<StrongsNumber>(); }
        public StrongsCluster(List<StrongsNumber> strongsNums)
        {
            strongs = new List<StrongsNumber>();
            foreach (StrongsNumber number in strongsNums)
            {
                strongs.Add((StrongsNumber) number.Clone());
            }
        }

        public StrongsCluster(string[] strongsNums)
        {
            strongs = new List<StrongsNumber>();
            foreach (string number in strongsNums)
            {
                strongs.Add(new StrongsNumber(number));
            }
        }


        public List<StrongsNumber> Strongs
        { get { return strongs; } }
        
        public bool Contains(StrongsNumber strongsNum)
        {
            bool result = false;

            foreach (StrongsNumber number in strongs)
            {
                if (strongsNum.ToString() == number.ToString())
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        public static StrongsCluster operator +(StrongsCluster a, StrongsCluster b)
        {
            StrongsCluster strongsCluster = new StrongsCluster(a.Strongs);

            foreach (StrongsNumber number in b.Strongs)
            {
                if(/*strongsCluster.Count > 0 && */!number.IsEmpty && !strongsCluster.Contains(number))
                strongsCluster.Add((StrongsNumber)number.Clone());
            }

            return strongsCluster;
        }

        public int Count
        {  get { return strongs.Count; } }

        public void Add(StrongsNumber strongsNumber)
        { strongs.Add(strongsNumber);}

        public void Add(string strongsNumbers)
        {
            string[] numbers = strongsNumbers.Split(' ');
            foreach (string num in numbers)
            {
                strongs.Add(new StrongsNumber(num));
            }
        }

        public StrongsCluster AddRange(string[] numbers)
        {
            foreach (string num in numbers)
            {
                strongs.Add(new StrongsNumber(num));
            }

            return this;    
        }

        public void Set(string strongsNumbers)
        {
            strongs.Clear();
            if (strongsNumbers != null)
            {
                string[] numbers = strongsNumbers.Split(' ');
                foreach (string num in numbers)
                {
                    strongs.Add(new StrongsNumber(num));
                }
            }
        }

        /// <summary>
        /// puts or removes the word at a specific index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public StrongsNumber this[int index]
        {
            get
            {
                return strongs[index];
            }
            set
            {
                strongs[index] = value;
            }
        }

        public string ToStringBracketed()
        {
            string result = string.Empty;

            foreach (StrongsNumber num in strongs)
            {
                result += ("<" + num.ToString() + "> ");
            }

            return result.Trim();
        }


        public string ToStringS()
        {
            string result = string.Empty;

            foreach (StrongsNumber num in strongs)
            {
                result += num.ToStringS() + " ";
            }

            return result.Trim();
        }

        public string ToStringEx()
        {
            string result = string.Empty;

            foreach (StrongsNumber num in strongs)
            {
                result += num.ToStringEx() + " ";
            }

            return result.Trim();
        }

        public override string ToString()
        {
            string result = string.Empty;

            foreach(StrongsNumber num in strongs)
            {
                result += num.ToString() + " ";
            }

            return result.Trim();
        }

    }
}
