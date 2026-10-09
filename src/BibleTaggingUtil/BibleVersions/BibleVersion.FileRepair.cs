using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BibleTaggingUtil.BibleVersions
{
    public abstract partial class BibleVersion
    {
        /// <summary>
        /// This method checks if the text file is corrupt 
        /// by detecting excess lines with duplicate verse references.
        /// </summary>
        /// <param name="textFilePath"></param>
        /// <returns>True if the file was fixed or not corrupt, false if the file is corrupt and could not be fixed.</returns>
        private bool FixFileIfCorrupt(string textFilePath)
        {
            bool result = true;
            bool excessLineDetected = false;
            List<string> referenceTracker = new List<string>();
            try
            {
                StringBuilder sb = new StringBuilder();
                string[] lines = File.ReadAllLines(textFilePath);
                foreach (string line in lines)
                {
                    Match mTx = Regex.Match(line, @"^([0-9A-Za-z]+)\s([0-9]+):([0-9]+)\s*(.*)");
                    if (mTx.Success)
                    {
                        string chapter = mTx.Groups[2].Value.Trim();
                        string verseNo = mTx.Groups[3].Value.Trim();
                        String book = mTx.Groups[1].Value;
                        string referenceA = string.Format("{0} {1}:{2}", book, chapter, verseNo);
                        // ensure we are using ubs book names consistently
                        string reference = Utils.GetUbsReference(referenceA);
                        if (string.IsNullOrEmpty(reference))
                        {
                            Tracing.TraceError(MethodBase.GetCurrentMethod().Name, "Could not detect ubs reference: " + referenceA);
                            return false;
                        }
                        if (!referenceTracker.Contains(reference))
                        {
                            referenceTracker.Add(reference);
                            sb.AppendLine(line);
                        }
                        else
                        {
                            excessLineDetected = true;
                        }
                    }
                    else
                    {
                        // this is not supposed to happen.
                        // we should trace the offending line 
                        // and return false to indicate that the file is corrupt and needs to be fixed manually.
                        Tracing.TraceError(MethodBase.GetCurrentMethod().Name, "Could not detect verse reference: " + line);
                        return false;
                    }

                }
                // if we detected excess lines, then write the fixed file back to disk
                if (excessLineDetected && sb.Length > 0)
                {
                    // 1. Backup the original file
                    //    the backup file will be saved to .\OldTagged folder appending the word Bad to the file Name
                    string backupFolder = Path.Combine(Path.GetDirectoryName(textFilePath), "OldTagged");
                    if (!Directory.Exists(backupFolder))
                        Directory.CreateDirectory(backupFolder);
                    // Create a backup file name
                    string backupFileName = Path.Combine(backupFolder, Path.GetFileNameWithoutExtension(textFilePath) + "_Bad" + Path.GetExtension(textFilePath));

                    File.Move(textFilePath, backupFileName, true);

                    // 2. Write the fixed file back to disk
                    File.WriteAllText(textFilePath, sb.ToString());
                }
            }
            catch (Exception ex)
            {
                var cm = System.Reflection.MethodBase.GetCurrentMethod();
                var name = cm.DeclaringType.FullName + "." + cm.Name;
                Tracing.TraceException(name, ex.Message);
                throw;
            }
            return result;
        }
    }
}