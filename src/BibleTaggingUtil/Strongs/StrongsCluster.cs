using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibleTaggingUtil.Strongs
{
    public class StrongsCluster : IEnumerable
    {
        private List<StrongsNumber> strongs;

        private bool tagLable = false;
        private string lable = string.Empty;

        public bool IsTagLable { get { return tagLable; } }
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

        public StrongsCluster(string lable)
        {
            tagLable = true;
            this.lable = lable;
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
            if (a.IsTagLable)
                return b;
            if (b.IsTagLable)
                return a;
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
                if(index < 0 || index >= strongs.Count)
                    throw new ArgumentOutOfRangeException("index");
                return strongs[index];
            }
            set
            {
                if (index < 0 || index >= strongs.Count)
                    throw new ArgumentOutOfRangeException("index");
                strongs[index] = value;
            }
        }

        public void DeleteAt(int index)
        {
            if (index < 0 || index >= strongs.Count)
                throw new ArgumentOutOfRangeException("index");
            if (strongs.Count == 1)
                strongs[0] = new StrongsNumber("");
            else
                strongs.RemoveAt(index);
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

        public override string ToString()
        {
            string result = string.Empty;

            if (tagLable) { result = lable; }
            else
            {
                foreach (StrongsNumber num in strongs)
                {
                    result += num.ToString() + " ";
                }
            }

            return result.Trim();
        }

        public string ToStringD()
        {
            string result = string.Empty;

            foreach(StrongsNumber num in strongs)
            {
                result += num.ToStringD() + " ";
            }

            return result.Trim();
        }
        #region IEnumerable

        private class StrongsEnumerator : IEnumerator
        {
            private List<StrongsNumber> strongs;
            int position = -1;

            //constructor
            public StrongsEnumerator(List<StrongsNumber> strongs)
            {
                this.strongs = strongs;
            }
            private IEnumerator getEnumerator()
            {
                return (IEnumerator)this;
            }
            //IEnumerator
            public bool MoveNext()
            {
                position++;
                return (position < strongs.Count);
            }
            //IEnumerator
            public void Reset()
            {
                position = -1;
            }
            //IEnumerator
            public object Current
            {
                get
                {
                    try
                    {
                        return strongs[position];
                    }
                    catch (IndexOutOfRangeException)
                    {
                        throw new InvalidOperationException();
                    }
                }
            }
        }  //end nested class

        public IEnumerator GetEnumerator()
        {
            return new StrongsEnumerator(strongs);
        }

        #endregion IEnumerator and IEnumerable


    }
}
