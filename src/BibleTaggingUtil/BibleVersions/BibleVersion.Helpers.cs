using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BibleTaggingUtil.BibleVersions
{
    public abstract partial class BibleVersion
    {
        public List<string> UbsBookNames
        {
            get
            {
                return bookNames.Keys.ToList();
            }
        }
        public int BookCount
        {
            get
            {
                return bookNamesList.Count;
            }
        }

        public List<string> BookNames
        {
            get
            {
                return bookNamesList;
            }
        }

        public string this[string ubsName]
        {
            get
            {
                string bookName = string.Empty;
                try
                {
                    if (bookNames.Count > 0)
                        bookName = bookNames[ubsName];
                }
                catch (Exception ex)
                {
                    var cm = System.Reflection.MethodBase.GetCurrentMethod();
                    var name = cm.DeclaringType.FullName + "." + cm.Name;
                    Tracing.TraceException(name, ex.Message);
                }
                return bookName;

            }
        }

        private bool AddBookName(string line)
        {
            if (line.ToLower().Contains("gen"))
            {
                int o = 0;
            }
            Match mTx = Regex.Match(line, textReferencePattern);
            if (!mTx.Success)
            {
                //Tracing.TraceError(MethodBase.GetCurrentMethod().Name, "Could not detect text reference: " + line);
                return false;
            }
            if (line.ToLower().Contains("gen"))
            {
                int o = 0;
            }

            String book = mTx.Groups[1].Value;
            if (!bookNamesList.Contains(book))
                bookNamesList.Add(book);

            return true;
        }

        private bool SetSearchPattern(string line, out string referancePattern)
        {

            Match mTx = Regex.Match(line, referencePattern1);
            if (mTx.Success)
            {
                referancePattern = referencePattern1;
                return true;
            }

            mTx = Regex.Match(line, referencePattern2);
            if (mTx.Success)
            {
                referancePattern = referencePattern2;
                return true;
            }

            mTx = Regex.Match(line, referencePattern3);
            if (mTx.Success)
            {
                referancePattern = referencePattern3;
                return true;
            }

            //Tracing.TraceError(MethodBase.GetCurrentMethod().Name, "Could not detect reference pattern: " + line);
            referancePattern = string.Empty;
            return false;
        }


        public int GetBookIndex(string bookName)
        {
            int index = -1;
            if (bookNamesList.Contains(bookName))
            {
                index = Array.IndexOf(bookNamesList.ToArray(), bookName);

                if (bookNamesList.Count == 27)
                {
                    // we have NT books only
                    index += 39;

                }
            }
            return index;
        }


        public string GetBookNameFromIndex(int index)
        {
            string bookName = string.Empty;

            bool individual = Properties.TargetBibles.Default.IndividualBooks;
            if (individual)
            {
                foreach (string book in bookNamesList)
                {
                    if (Utils.GetBookIndexFromBook(book) == index)
                    {
                        bookName = book;
                        break;
                    }
                }
            }
            else if (index < 66)
            {
                if (index < bookNamesList.Count)
                    bookName = bookNamesList[index];
                else if (bookNamesList.Count == 27)
                    bookName = bookNamesList[index - 39];
            }
            return bookName;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="reference">reference using UBS book name</param>
        /// <returns></returns>
        public string GetCorrectReference(string reference)
        {
            string correctReference = string.Empty;
            bool individual = Properties.TargetBibles.Default.IndividualBooks;

            int space = reference.IndexOf(' ');
            string book = reference.Substring(0, space);
            string cv = reference.Substring(space + 1);
            int offset = 0;
            if (bookNamesList.Count == 27)
            {
                // we have NT books only
                offset = 39;

            }

            string correctBook = string.Empty;
            if (individual)
            {
                int index = Utils.GetBookIndexFromBook(book);
                foreach (string b in bookNamesList)
                {
                    if (index == Utils.GetBookIndexFromBook(b))
                    {
                        correctBook = b;
                        break;
                    }
                }
            }
            else
            {
                int index = Array.IndexOf(Constants.ubsNames.Keys.ToArray(), book) - offset;
                correctBook = bookNamesList[index];
            }
            correctReference = string.Format("{0} {1}", correctBook, cv);
            return correctReference;
        }

    }
}
