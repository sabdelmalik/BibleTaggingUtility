using BibleTaggingUtil.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BibleTaggingUtil.BibleVersions
{
    public class TargetVersion : BibleVersion
    {
        System.Timers.Timer saveTimer = null;

        public TargetVersion(BibleTaggingForm container) : base(container, 31104) { }

        public void SaveUpdates()
        {
            Tracing.TraceEntry(MethodBase.GetCurrentMethod().Name);

            try
            {
                if (saveTimer != null && saveTimer.Enabled)
                {
                    saveTimer.Stop();
                    saveTimer.Start();
                }

                lock (this)
                {
                    if (!container.EditorPanel.TargetDirty)
                        return;

                    container.WaitCursorControl(true);
                    container.EditorPanel.TargetDirty = false;
                    container.EditorPanel.SaveCurrentVerse();

                    if (bible.Count > 0)
                    {
                        bool individual = Properties.TargetBibles.Default.IndividualBooks;
                        string currentbook = Properties.TargetBibles.Default.CurrentBook;

                        string taggedFolder = string.Empty;
                        if (individual)
                        {
                            if (string.IsNullOrEmpty(currentbook))
                            {
                                throw new Exception("No current book");
                                //string biblesFolder = Properties.TargetBibles.Default.TargetBiblesFolder;
                                //string target = Properties.TargetBibles.Default.TargetBible;
                                //taggedFolder = Path.Combine(biblesFolder, target);
                                //taggedFolder = Path.Combine(taggedFolder, "taggedX");
                            }
                            else
                            {
                                taggedFolder = Path.GetDirectoryName(currentbook);
                            }
                        }
                        else
                        {
                            // construct Updates fileName
                            taggedFolder = Path.GetDirectoryName(container.Config.TaggedBible);
                        }
                        string oldTaggedFolder = Path.Combine(taggedFolder, "OldTagged");
                        if (!Directory.Exists(oldTaggedFolder))
                            Directory.CreateDirectory(oldTaggedFolder);

                        string updatesFilePath = string.Empty;
                        if (individual)
                        {
                            // move current book to old folder
                            string src = currentbook;
                            string fName = Path.GetFileName(currentbook);
                            string dst = Path.Combine(oldTaggedFolder, fName);
                            File.Move(src, dst);

                            // construct the updated file name
                            string bareFilename = Path.GetFileNameWithoutExtension(currentbook);
                            int speratorIndex = bareFilename.LastIndexOf("___");
                            if(speratorIndex > 0)
                                bareFilename = bareFilename.Substring(0, speratorIndex);
                            string updatesFileName = $"{bareFilename}___{DateTime.Now.ToString("yyyy_MM_dd_HH_mm")}.txt";
                            updatesFilePath = Path.Combine(taggedFolder, updatesFileName);
                            Properties.TargetBibles.Default.CurrentBook = updatesFilePath;
                            Properties.TargetBibles.Default.Save();
                        }
                        else
                        {
                            // move existing tagged files to the old folder
                            String[] existingTagged = Directory.GetFiles(taggedFolder, "*.*");
                            foreach (String existingTaggedItem in existingTagged)
                            {
                                string fName = Path.GetFileName(existingTaggedItem);
                                string src = Path.Combine(taggedFolder, fName);
                                string dst = Path.Combine(oldTaggedFolder, fName);
                                if (System.IO.File.Exists(dst))
                                    System.IO.File.Delete(src);
                                else
                                    System.IO.File.Move(src, dst);
                            }

                            string baseName = Path.GetFileNameWithoutExtension(container.Config.TaggedBible);
                            string updatesFileName = string.Format("{0:s}_{1:s}.txt", baseName, DateTime.Now.ToString("yyyy_MM_dd_HH_mm"));
                            updatesFilePath = Path.Combine(taggedFolder, updatesFileName);
                        }
                        using (StreamWriter outputFile = new StreamWriter(updatesFilePath))
                        {
                            foreach (string verseRef in  container.Target.Bible.Keys)
                            {
                                string line = string.Format("{0:s} {1:s}", verseRef, Utils.GetVerseText(container.Target.Bible[verseRef], true));
                                outputFile.WriteLine(line);
                            }

                        }

                    }

                    Properties.MainSettings.Default.LastBook = container.VerseSelectionPanel.CurrentBook;
                    Properties.MainSettings.Default.LastChapter = container.VerseSelectionPanel.CurrentChapter;
                    Properties.MainSettings.Default.LastVerse = container.VerseSelectionPanel.CurrentVerse;
                    Properties.MainSettings.Default.Save();
                    container.WaitCursorControl(false);
                }

            }
            catch(Exception ex)
            {
                var cm = System.Reflection.MethodBase.GetCurrentMethod();
                var name = cm.DeclaringType.FullName + "." + cm.Name;
                Tracing.TraceException(name, ex.Message);
            }
            container.WaitCursorControl(false);

        }

        #region Priodic Save
        public void ActivatePeriodicTimer()
        {
            if (container.InvokeRequired)
            {
                container.Invoke(new Action(() =>
                {
                    ActivatePeriodicTimer();
                }));
            }
            else
            {
                int priodicSaveTime = Properties.MainSettings.Default.PeriodicSaveTime;

                if (saveTimer == null)
                {
                    saveTimer = new System.Timers.Timer();
                }
                if (priodicSaveTime > 0)
                {
                    saveTimer.Interval = priodicSaveTime * 60000;
                    saveTimer.AutoReset = true;
                    saveTimer.Elapsed -= SaveTimer_Elapsed; // just in case we were subscribed before
                    saveTimer.Elapsed += SaveTimer_Elapsed; // we only want one subscription

                    saveTimer.Enabled = true;
                    saveTimer.Start();
                }
                else
                {
                    if (saveTimer != null)
                    {
                        saveTimer.Stop();
                        saveTimer.Enabled = false;
                        saveTimer.Elapsed -= SaveTimer_Elapsed;
                    }
                }
            }
        }


        private void SaveTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            PeriodicSave();
        }

        private void PeriodicSave()
        {
            if (container.InvokeRequired)
            {
                container.Invoke(new Action(() =>
                {
                    PeriodicSave();
                }));
            }
            else
            {
                SaveUpdates();
            }
        }

        #endregion Priodic Save
    }
}
