using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;

using WeifenLuo.WinFormsUI.Docking;
using System.Diagnostics;
using System.Threading;

using BibleTaggingUtil.Editor;
using SM.Bible.Formats.USFM;
using SM.Bible.Formats.USFM2OSIS;
using System.Reflection;

using BibleTaggingUtil.BibleVersions;
using System.Linq.Expressions;
using static System.Net.WebRequestMethods;
using Microsoft.VisualBasic.Logging;
using System.Xml.Linq;
using System.Text;
using System.Text.Json;
using BibleTaggingUtil.TranslationTags;
using BibleTaggingUtil.Settings;
using BibleTaggingUtil.Versification;
using BibleTaggingUtil.Restore;
using BibleTaggingUtil.Strongs;
using System.Drawing.Text;
using System.Text.RegularExpressions;

namespace BibleTaggingUtil
{
    public partial class BibleTaggingForm : Form
    {
        private const bool dev = false;

        private BrowserPanel browserPanel;
        private EditorPanel editorPanel;
        private VerseSelectionPanel verseSelectionPanel;
        private ProgressForm progressForm = null;

        private TargetVersion target;
        private TargetOsisVersion osisTarget;
        private ReferenceTopVersion referenceTopVersion;
        private ReferenceVersionTAHOT referenceTOTHT;
        private ReferenceVersionTAGNT referenceTAGNT;

        private string execFolder = string.Empty;
        private string crosswirePath = string.Empty;

        private bool m_bSaveLayout = true;
        private DeserializeDockContent m_deserializeDockContent;

        private TranslationTagsEx translationTags = null;

        // to save updated dictionary
        // https://stackoverflow.com/questions/36333567/saving-a-dictionaryint-object-in-c-sharp-serialization

        private ConfigurationHolder config;
        private SettingsForm settingsForm = new SettingsForm();
        private RestoreTarget restoreTarget = null;

        public BibleTaggingForm()
        {
            InitializeComponent();

            dockPanel.DocumentStyle = DocumentStyle.DockingWindow;
            m_deserializeDockContent = new DeserializeDockContent(GetContentFromPersistString);

            this.Resize += BibleTaggingForm_Resize;

            // allow key press detection
            this.KeyPreview = true;
            // handel keypress
            this.KeyDown += BibleTaggingForm_KeyDown;
#if DEBUG
            generateSWORDFilesToolStripMenuItem.Visible = false;
#else
            //saveHebrewToolStripMenuItem.Visible = false;
            //saveKJVPlainToolStripMenuItem.Visible = false;
            usfmToolStripMenuItem.Visible = false;
            oSISToolStripMenuItem.Visible = false;
#endif

            target = new TargetVersion(this);
            osisTarget = new TargetOsisVersion(this);
            referenceTopVersion = new ReferenceTopVersion(this);
            referenceTOTHT = new ReferenceVersionTAHOT(this);
            referenceTAGNT = new ReferenceVersionTAGNT(this);
        }

        #region Form Events

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BibleTaggingForm_Load(object sender, EventArgs e)
        {
            bool workingOnNIV = false;

            settingsForm.TargetAboutToChange += SettingsForm_TargetAboutToChange;

            Assembly assembly = Assembly.GetExecutingAssembly();
            AssemblyName assemblyName = assembly.GetName();
            Version version = assemblyName.Version;
            this.Text = "Bible Tagging " + version.ToString();

            var cm = System.Reflection.MethodBase.GetCurrentMethod();
            var name = cm.DeclaringType.FullName + "." + cm.Name;

            execFolder = Path.GetDirectoryName(assembly.Location);
            Tracing.InitialiseTrace(execFolder);

            Tracing.TraceEntry(name);

            string configFile = Path.Combine(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath), "DockPanel.config");

            #region WinFormUI setup
            this.dockPanel.Theme = this.vS2013BlueTheme1;

            if (System.IO.File.Exists(configFile))
                dockPanel.LoadFromXml(configFile, m_deserializeDockContent);

            EnableRestoreMenu();

            browserPanel = new BrowserPanel(); //CreateNewDocument();
            browserPanel.Text = "Blue Letter Bible Lexicon";
            browserPanel.TabText = browserPanel.Text;
            if (dockPanel.DocumentStyle == DocumentStyle.SystemMdi)
            {
                browserPanel.MdiParent = this;
                browserPanel.Show(dockPanel, DockState.DockRight);
            }
            else
                browserPanel.Show(dockPanel, DockState.DockRight);
            browserPanel.CloseButtonVisible = false;
            browserPanel.DockState = (DockState)Properties.MainSettings.Default.BrowserPanelDockState;
            Tracing.TraceInfo(name, "browserPanel initialised");

            browserPanel.LexiconWebsite = "https://www.blueletterbible.org/lexicon/{0}/kjv/wlc/0-1/";
            browserPanel.NavigateToTag("h1");
            //browserPanel.NavigateTo("https://www.blueletterbible.org/lexicon/h1/kjv/wlc/0-1/");

            //progressForm = new ProgressForm(this);

            verseSelectionPanel = new VerseSelectionPanel();
            verseSelectionPanel.Text = "Verse Selection";
            verseSelectionPanel.TabText = verseSelectionPanel.Text;


            editorPanel = new EditorPanel(this, browserPanel, verseSelectionPanel);
            editorPanel.Text = "Verse Editor";
            editorPanel.TabText = editorPanel.Text;

            if (dockPanel.DocumentStyle == DocumentStyle.SystemMdi)
            {
                editorPanel.MdiParent = this;
                editorPanel.Show(dockPanel, DockState.Document);
            }
            else
                editorPanel.Show(dockPanel, DockState.Document);

            editorPanel.CloseButton = false;
            editorPanel.CloseButtonVisible = false;
            Tracing.TraceInfo(name, "editorPanel initialised");

            if (dockPanel.DocumentStyle == DocumentStyle.SystemMdi)
            {
                verseSelectionPanel.MdiParent = this;
                verseSelectionPanel.Show(dockPanel, DockState.DockLeft);
            }
            else
                verseSelectionPanel.Show(dockPanel, DockState.DockLeft);

            verseSelectionPanel.CloseButtonVisible = false;
            verseSelectionPanel.DockState = (DockState)Properties.MainSettings.Default.VersePanelDockState;
            Tracing.TraceInfo(name, "verseSelectionPanel initialised");

            #endregion WinFormUI setup



            while (!Properties.ReferenceBibles.Default.Configured)
            {
                GetSettings(startup: true);
                if (Properties.ReferenceBibles.Default.Configured)
                    break;
                DialogResult result = MessageBox.Show("Incomplete Settings! \r\n Do you want to retry settings", "Settings", MessageBoxButtons.YesNo);
                if (result == DialogResult.No)
                {
                    Application.Exit();
                    return;
                }
            }

            crosswirePath = Path.Combine(execFolder, "Crosswire");
            //refFolder = Path.Combine(execFolder, "ReferenceBibles");

            //tagntPath = Path.Combine(refFolder, ntReference);
            //tothtPath = Path.Combine(refFolder, otReference);

            editorPanel.TAGNT = referenceTAGNT;
            editorPanel.TAHOT = referenceTOTHT;
            editorPanel.TopVersion = referenceTopVersion;
            editorPanel.TargetVersion = target;
            editorPanel.TargetOsisVersion = osisTarget;

            Application.ThreadException += Application_ThreadException;

            new Thread(() =>
            {
                try
                {
                    LoadBibles();
                }
                catch (Exception ex)
                {
                    var cm = System.Reflection.MethodBase.GetCurrentMethod();
                    var name = cm.DeclaringType.FullName + "." + cm.Name;
                    Tracing.TraceException(name, ex.Message);
                    HandleException(ex);
                }
            }).Start();
        }

        private void SettingsForm_TargetAboutToChange(object sender, EventArgs e)
        {
            target.SaveUpdates();
        }

        private void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            HandleException(e.Exception);
        }

        public void HandleException(Exception ex)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => { HandleException(ex); }));
            }
            else
            {
                string caption = "Exception";
                string text = "An exception occurred!\r\nThe application will terminate.\r\nDo you want to Save before termination?";
                DialogResult result = MessageBox.Show(text, caption, MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    if (Properties.MainSettings.Default.Osis)
                        OsisTarget.Save("");
                    else
                        Target.SaveUpdates();

                }
                WaitCursorControl(false);
                //this.Close();
                Application.Exit();
            }
        }

        private void BibleTaggingForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.S && e.Modifiers == Keys.Control)
            {
                target.SaveUpdates();
            }
        }

        private void BibleTaggingForm_Resize(object sender, EventArgs e)
        {
            if (progressForm == null)
                return;

            progressForm.Location = new Point(
                this.Location.X + (this.Width / 2) - (progressForm.Width / 2),
                this.Location.Y + (this.Height / 2) - (progressForm.Height / 2));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BibleTaggingForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (MessageBox.Show("This will close down the application. Confirm?", "Close Bible Edit", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    e.Cancel = true;
                    this.Activate();
                }
                else
                {

                    // Save the Verses Updates

                    target.SaveUpdates();
                    Properties.MainSettings.Default.VersePanelDockState = (int)verseSelectionPanel.DockState;
                    Properties.MainSettings.Default.BrowserPanelDockState = (int)browserPanel.DockState;
                    Properties.MainSettings.Default.Save();

                    string configFile = Path.Combine(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath), "DockPanel.config");
                    if (m_bSaveLayout)
                        dockPanel.SaveAsXml(configFile);
                    else if (System.IO.File.Exists(configFile))
                        System.IO.File.Delete(configFile);

                    e.Cancel = false;
                }
            }
            catch (Exception ex)
            {
                var cm = System.Reflection.MethodBase.GetCurrentMethod();
                var name = cm.DeclaringType.FullName + "." + cm.Name;
                Tracing.TraceException(name, ex.Message);
            }
        }

        #endregion Form Events

        private void CloseForm()
        {
            if (InvokeRequired)
            {
                // Call this same method but append THREAD2 to the text
                Action safeWrite = delegate { CloseForm(); };
                Invoke(safeWrite);
            }
            else
            {
                this.Close();
            }
        }

        /// <summary>
        /// Loading Bibles Thread
        /// </summary>
        private void LoadBibles()
        {
            Tracing.TraceEntry(MethodBase.GetCurrentMethod().Name);

            string taggedFolder = string.Empty;

            string targetBiblesFolder = Properties.TargetBibles.Default.TargetBiblesFolder;
            string targetBible = Properties.TargetBibles.Default.TargetBible;
            string biblesFolder = Path.Combine(targetBiblesFolder, targetBible);

            try
            {
                while (true)
                {
                    //if (string.IsNullOrEmpty(biblesFolder) && !Directory.Exists(biblesFolder))
                    //{
                    //    GetBiblesFolder();
                    //    biblesFolder = Properties.MainSettings.Default.BiblesFolder;
                    //    if (string.IsNullOrEmpty(biblesFolder))
                    //    {
                    //        CloseForm();
                    //        return;
                    //    }
                    //}

                    // Load configuration
                    config = ConfigurationHolder.Instance;
                    string confResult = config.ReadBiblesConfig(biblesFolder);
                    //if (!string.IsNullOrEmpty(confResult))
                    //{
                    //    MessageBox.Show(confResult);
                    //    GetBiblesFolder();
                    //    biblesFolder = Properties.MainSettings.Default.BiblesFolder;
                    //    if (string.IsNullOrEmpty(biblesFolder))
                    //    {
                    //        CloseForm();
                    //        return;
                    //    }
                    //    else
                    //    {
                    //        confResult = config.ReadBiblesConfig(biblesFolder);
                    //        if (!string.IsNullOrEmpty(confResult))
                    //        {
                    //            MessageBox.Show(confResult);
                    //            CloseForm();
                    //            return;
                    //        }
                    //    }
                    //}

                    taggedFolder = Path.GetDirectoryName(config.TaggedBible);

                    if (!Directory.Exists(taggedFolder))
                    {
                        DialogResult res = ShowMessageBox("Tagged folder does not exist\r\nSelect another Bible Folder?", "Error!", MessageBoxButtons.YesNo);
                        if (res == DialogResult.Yes)
                        {
                            GetSettings(startup: false);
                            //biblesFolder = string.Empty;
                        }
                        else
                        {
                            CloseForm();
                            return;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                /*
                if (string.IsNullOrEmpty(config.TopReferenceVersion))
                {
                    topReferencePath = Path.Combine(refFolder, "KJV");
                }
                else
                {
                    topReferencePath = Path.Combine(refFolder, config.TopReferenceVersion);
                    if (!Directory.Exists(topReferencePath))
                    {
                        topReferencePath = Path.Combine(refFolder, "KJV");
                    }
                }
                */


                editorPanel.TargetBibleName = targetBible;
                target.BibleName = targetBible;
                osisTarget.BibleName = targetBible;
            }
            catch (Exception ex)
            {
                var cm = System.Reflection.MethodBase.GetCurrentMethod();
                var name = cm.DeclaringType.FullName + "." + cm.Name;
                Tracing.TraceException(name, ex.Message);
            }


            while (true)
            {
                if (string.IsNullOrEmpty(Properties.TargetBibles.Default.TargetBible) ||
                    string.IsNullOrEmpty(Properties.TargetBibles.Default.TargetBiblesFolder))
                {
                    DialogResult res = ShowMessageBox("At least one Target must be selected\r\nYes: to set the target Bible\r\nNo: to quit.",
                        "Target Missing", MessageBoxButtons.YesNo);
                    if (res == DialogResult.No)
                    {
                        Application.Exit();
                    }
                    GetSettings(startup: true);
                }
                else { break; }
            }

            LoadTarget();

            StartGui();

            WaitCursorControl(true);

            this.Closing -= BibleTaggingForm_Closing;
            this.Closing += BibleTaggingForm_Closing;

            if (!Properties.ReferenceBibles.Default.TopRefSkip)
            {
                string refBibleName = Properties.ReferenceBibles.Default.TopReference; //Path.GetFileName(settingsForm.ReferenceTopVersionPath);
                referenceTopVersion.BibleName = refBibleName;
                editorPanel.TopReferenceBibleName = refBibleName;
                if (!LoadReferenceFiles(settingsForm.ReferenceTopVersionPath, referenceTopVersion)) { CloseForm(); return; }
            }

            if (!Properties.ReferenceBibles.Default.OtRefSkip)
            {
                referenceTOTHT.BibleName = Properties.ReferenceBibles.Default.TAOTReference;//otReference;

                if (!LoadReferenceFiles(settingsForm.ReferenceTAOTPath, referenceTOTHT)) { CloseForm(); return; }
            }

            if (!Properties.ReferenceBibles.Default.NtRefSkip)
            {
                referenceTAGNT.BibleName = Properties.ReferenceBibles.Default.TANTReference; //ntReference;
                if (!LoadReferenceFiles(settingsForm.ReferenceTANTPath, referenceTAGNT)) { CloseForm(); return; }

                #region fix strongs multiple occurance suffix
                Dictionary<string, string> refs = new Dictionary<string, string>(); 
                foreach (( string refrence, Verse ver) in referenceTAGNT.Bible)
                {
                    foreach (VerseWord word in ver)
                    {
                        if(!word.VarCorrected && word.VarUsed && word.Strong.Count == 1)
                        {
                            StrongsNumber sn = word.Strong[0];
                            int count = 0;
                            foreach (VerseWord w in ver)
                            {
                                if(w.Strong.Count ==  1 && w.Strong[0].Number == sn.Number)
                                    count++;
                            }
                            if (count > 1)
                            {
                                int currentPos = 1;
                                for (int i = 0; i < ver.Count; i++)
                                {
                                    VerseWord w = ver[i];
                                    if (w.Strong.Count == 1)
                                    {
                                        if (w.Strong.Count == 1 && w.Strong[0].Number == sn.Number)
                                        {
                                            w.Strong[0].SetOccurance(currentPos++);
                                            w.VarCorrected = true;
                                            if (refs.ContainsKey(refrence))
                                            {
                                                if (!refs[refrence].Contains(w.StrongStringS))
                                                {
                                                    refs[refrence] += string.Format("\t({0}) {1}", count, w.StrongStringS);
                                                }
                                            }
                                            else
                                            {
                                                refs[refrence] = string.Format("({0}) {1}", count, w.StrongStringS);
                                            }
                                        }
                                    } else
                                    {
                                        if (w.Strong.ToStringD().Contains(sn.ToStringD()))
                                        {
                                            for(int j = 0; j < w.Strong.Count; j++)
                                            {
                                                StrongsNumber s = w.Strong[j];
                                                if (s.Number == sn.Number)
                                                {
                                                    s.SetOccurance(currentPos++);
                                                    w.VarCorrected = true;
                                                    if (refs.ContainsKey(refrence))
                                                    {
                                                        if (!refs[refrence].Contains(w.StrongStringS))
                                                        {
                                                            refs[refrence] += string.Format("\t({0}) {1}", count, w.StrongStringS);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        refs[refrence] = string.Format("({0}) {1}", count, w.StrongStringS);
                                                    }

                                                }
                                            }
                                        }

                                    }

                                }
                            }
                        }
                    }
                }
                //string path = @"c:\tmp\corrections.txt";
                //using (StreamWriter sw = new StreamWriter(path))
                //{
                //    foreach ((string r, string v) in refs)
                //    {
                //        sw.WriteLine(r + "\t" + v);
                //    }
                //}
                #endregion fix strongs multiple occurance suffix
            }

            target.ActivatePeriodicTimer();

            #region change to allow display greek/hebrew words in the target
            AddAncientWords();
            #endregion change to allow display greek/hebrew words in the target


            editorPanel.ClearNavStack();

            WaitCursorControl(false);
        }

        private void AddAncientWords()
        {
            if (target != null && target.Bible != null && target.Bible.Count > 0)
            {
                foreach ((string reference, Verse v) in target.Bible)
                {
                    int idx = reference.IndexOf(' ');
                    string bk = reference.Substring(0, idx);

                    int bkIdx = target.GetBookIndex(bk);
                    if (bkIdx > 38 && referenceTAGNT != null && referenceTAGNT.Bible != null && referenceTAGNT.Bible.Count > 0)
                    {
                        string ntbk = referenceTAGNT.GetBookNameFromIndex(bkIdx);
                        string ntCorrectReference = $"{ntbk}{reference.Substring(idx)}";
                        if (referenceTAGNT.Bible.ContainsKey(ntCorrectReference))
                        {
                            v.AncientVerse = referenceTAGNT.Bible[ntCorrectReference];
                        }
                    }
                    else if (referenceTOTHT != null && referenceTOTHT.Bible != null && referenceTOTHT.Bible.Count > 0)
                    {
                        string otbk = referenceTOTHT.GetBookNameFromIndex(bkIdx);
                        string otCorrectReference = $"{otbk}{reference.Substring(idx)}";

                        if (referenceTOTHT.Bible.ContainsKey(otCorrectReference))
                        {
                            v.AncientVerse = referenceTOTHT.Bible[otCorrectReference];
                        }
                    }
                }
                UpdateTargetGrid();
            }
        }
        private void UpdateTargetGrid()
        {
            if (InvokeRequired)
            {
                Action safeUpdate = delegate { UpdateTargetGrid(); };
                Invoke(safeUpdate);
            }
            else
            {
                editorPanel.RefreshGrid(false);
            }
        }
        private bool LoadReferenceFiles(string folderPath, BibleVersion reference)
        {
            bool result = false;

            string[] files = Directory.GetFiles(folderPath);
            Dictionary<int, string> fileDict = new Dictionary<int, string>();
            if (files.Length == 1)
            {
                fileDict.Add(0, files[0]);
            }
            else
            {
                foreach (string file in files)
                {
                    int key = 0;
                    string fileName = Path.GetFileName(file);
                    int idx = fileName.IndexOf('.');
                    if (!int.TryParse(fileName.Substring(0, idx), out key))
                    {
                        MessageBox.Show("File name in wrong format: " + fileName);
                    }
                    fileDict.Add(key - 1, file);
                }
            }
            for (int i = 0; i < fileDict.Keys.Count; i++)
            {
                result = reference.LoadBibleFile(fileDict[i], i == 0, i != (files.Length - 1));
            }
            if (!result)
            {
                string refName = Path.GetFileName(folderPath);
                MessageBox.Show("Loading " + refName + " failed");
            }
            else
                StartGui();

            return result;
        }

        private void StartGui()
        {
            Tracing.TraceEntry(MethodBase.GetCurrentMethod().Name);

            if (InvokeRequired)
            {
                // Call this same method 
                Action safeStart = delegate { StartGui(); };
                Invoke(safeStart);
            }
            else
            {
                if(Properties.MainSettings.Default.LastBook < 0)
                { Properties.MainSettings.Default.LastBook = 0; }
                verseSelectionPanel.CurrentBook = Properties.MainSettings.Default.LastBook;


                if (Properties.MainSettings.Default.LastChapter < 0)
                { Properties.MainSettings.Default.LastChapter = 0; }
                verseSelectionPanel.CurrentChapter = Properties.MainSettings.Default.LastChapter;


                if (Properties.MainSettings.Default.LastVerse < 0)
                { Properties.MainSettings.Default.LastVerse = 0; }
                verseSelectionPanel.CurrentVerse = Properties.MainSettings.Default.LastVerse;
                verseSelectionPanel.FireVerseChanged();
                editorPanel.TargetDirty = false;
            }
        }

        public void WaitCursorControl(bool wait)
        {
            //Tracing.TraceEntry(MethodBase.GetCurrentMethod().Name);

            if (InvokeRequired)
            {
                // Call this same method but append THREAD2 to the text
                Action safeWrite = delegate { WaitCursorControl(wait); };
                Invoke(safeWrite);
            }
            else
            {
                if (wait)
                {
                    if (progressForm == null)
                        progressForm = new ProgressForm(this);
                    this.Cursor = Cursors.WaitCursor;
                    //waitCursorAnimation.Visible = true;
                    //aitCursorAnimation.BringToFront();
                    progressForm.Clear();
                    progressForm.TopMost = true;
                    progressForm.Visible = true;
                    //progressForm.Location = new Point((this.Width / 2) - (progressForm.Width / 2),
                    //                                  (this.Height / 2) - (progressForm.Height / 2));

                    progressForm.Show();

                    progressForm.Location = new Point(
                        this.Location.X + (this.Width / 2) - (progressForm.Width / 2),
                        this.Location.Y + (this.Height / 2) - (progressForm.Height / 2));


                    menuStrip1.Enabled = false;
                    verseSelectionPanel.Enabled = false;
                    editorPanel.Enabled = false;
                }
                else
                {
                    if (progressForm == null)
                        return;
                    this.Cursor = Cursors.Default;
                    //waitCursorAnimation.Visible = false;
                    progressForm.Progress = 0;
                    progressForm.Visible = false;
                    progressForm.Dispose();
                    progressForm = null;

                    menuStrip1.Enabled = true;
                    verseSelectionPanel.Enabled = true;
                    editorPanel.Enabled = true;
                }
            }
        }

        public void UpdateProgress(string label, int progress)
        {
            if (InvokeRequired)
            {
                try
                {
                    // Call this same method but append THREAD2 to the text
                    Action safeWrite = delegate { UpdateProgress(label, progress); };
                    Invoke(safeWrite);
                }
                catch (Exception ex) { }
            }
            else
            {
                progressForm.UpdateLabel(label);
                progressForm.Progress = progress;
            }
        }

        private DialogResult ShowMessageBox(string text, string caption, MessageBoxButtons buttons)
        {
            DialogResult result = DialogResult.OK;

            if (InvokeRequired)
            {
                // Call this same method but append THREAD2 to the text
                //Action safeWrite = delegate { ShowMessageBox(text, caption, buttons); };
                result = (DialogResult)Invoke(new Func<DialogResult>(() => ShowMessageBox(text, caption, buttons)));
            }
            else
            {
                result = MessageBox.Show(text, caption, buttons);
            }
            return result;
        }

        #region public properties

        public ConfigurationHolder Config { get { return config; } }
        public EditorPanel EditorPanel { get { return editorPanel; } }
        public VerseSelectionPanel VerseSelectionPanel { get { return verseSelectionPanel; } }
        public TargetVersion Target { get { return target; } }
        public TargetOsisVersion OsisTarget { get { return osisTarget; } }
        public ReferenceTopVersion TopVersion { get { return referenceTopVersion; } }
        public ReferenceVersionTAHOT TOTHT { get { return referenceTOTHT; } }
        public ReferenceVersionTAGNT TAGNT { get { return referenceTAGNT; } }

        #endregion public properties

        #region WinFormUI
        private IDockContent FindDocument(string text)
        {
            if (dockPanel.DocumentStyle == DocumentStyle.SystemMdi)
            {
                foreach (Form form in MdiChildren)
                    if (form.Text == text)
                        return form as IDockContent;

                return null;
            }
            else
            {
                foreach (IDockContent content in dockPanel.Documents)
                    if (content.DockHandler.TabText == text)
                        return content;

                return null;
            }
        }

        private BrowserPanel CreateNewDocument()
        {
            BrowserPanel dummyDoc = new BrowserPanel();

            int count = 1;
            string text = $"Document{count}";
            while (FindDocument(text) != null)
            {
                count++;
                text = $"Document{count}";
            }

            dummyDoc.Text = text;
            return dummyDoc;
        }

        private IDockContent GetContentFromPersistString(string persistString)
        {
            // DummyDoc overrides GetPersistString to add extra information into persistString.
            // Any DockContent may override this value to add any needed information for deserialization.

            string[] parsedStrings = persistString.Split(new char[] { ',' });
            if (parsedStrings.Length != 3)
                return null;

            if (parsedStrings[0] != typeof(DummyDoc).ToString())
                return null;

            DummyDoc dummyDoc = new DummyDoc();
            if (parsedStrings[1] != string.Empty)
                dummyDoc.FileName = parsedStrings[1];
            if (parsedStrings[2] != string.Empty)
                dummyDoc.Text = parsedStrings[2];

            return dummyDoc;
        }

        #endregion WinFormUI


        #region Bible File Loading

        /// <summary>
        /// 
        /// </summary>
        [Obsolete]
        private void GetBiblesFolder()
        {
            Tracing.TraceEntry(MethodBase.GetCurrentMethod().Name);

            if (InvokeRequired)
            {
                Action safeWrite = delegate { GetBiblesFolder(); };
                Invoke(safeWrite);
            }
            else
            {
                string folderPath = string.Empty;
                DialogResult res = folderBrowserDialog1.ShowDialog(this);
                if (res == DialogResult.OK)
                {
                    folderPath = folderBrowserDialog1.SelectedPath;
                }
                //Properties.MainSettings.Default.BiblesFolder = folderPath;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public string GetBibleFilePath(string directory, string title)
        {
            Tracing.TraceEntry(MethodBase.GetCurrentMethod().Name, directory, title);

            string biblePath = string.Empty;

            openFileDialog.Title = title;
            openFileDialog.InitialDirectory = directory;
            openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";

            openFileDialog.RestoreDirectory = false;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                //Read the contents of the file into a stream
                biblePath = openFileDialog.FileName;
            }

            Tracing.TraceExit(MethodBase.GetCurrentMethod().Name, biblePath);
            return biblePath;
        }

        #endregion Bible File Loading

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GetSettings(startup: false);
        }

        private void GetSettings(bool startup = false)
        {
            int priodicSaveTime = Properties.MainSettings.Default.PeriodicSaveTime;
            if (priodicSaveTime > 0)
            {
                settingsForm.PeriodicSaveEnabled = true;
                settingsForm.SavePeriod = priodicSaveTime;
            }
            else
            {
                settingsForm.PeriodicSaveEnabled = false;
            }
            DialogResult result = settingsForm.ShowDialog();
            if (result == DialogResult.OK)
            {
                if (settingsForm.PeriodicSaveEnabled)
                {
                    Properties.MainSettings.Default.PeriodicSaveTime = settingsForm.SavePeriod;
                }
                else
                {
                    Properties.MainSettings.Default.PeriodicSaveTime = 0;
                }
                target.ActivatePeriodicTimer();

                if (!startup)
                {
                    new Thread(() =>
                    {
                        try
                        {
                            UpdateBibles(settingsForm.ChangedFlags);
                        }
                        catch (Exception ex)
                        {
                            var cm = System.Reflection.MethodBase.GetCurrentMethod();
                            var name = cm.DeclaringType.FullName + "." + cm.Name;
                            Tracing.TraceException(name, ex.Message);
                            HandleException(ex);
                        }
                    }).Start();
                }
            }
        }

        private void LoadTarget()
        {
            WaitCursorControl(true);
            string targetBibleName = Properties.TargetBibles.Default.TargetBible;
            string targetBiblesFolder = Properties.TargetBibles.Default.TargetBiblesFolder;
            target.BibleName = targetBibleName;
            editorPanel.TargetBibleName = targetBibleName;
            osisTarget.BibleName = targetBibleName;

            editorPanel.ClearCurrentVerse();

            string bibleFolder = Path.Combine(targetBiblesFolder, targetBibleName);
            config.ReadBiblesConfig(bibleFolder);
            
            string taggedFolder = Path.Combine(bibleFolder, "tagged");
            string[] files = Directory.GetFiles(taggedFolder);
            if (files.Length > 0)
            {
                string ext = Path.GetExtension(files[0]);
                Properties.MainSettings.Default.Osis = false;

                //               if (Properties.MainSettings.Default.Osis)
                if (ext.ToLower() == ".xml")
                {
                    Properties.MainSettings.Default.Osis = true;

                    generateOSISToolStripMenuItem.Visible = false;
                    usfmToolStripMenuItem.Visible = false;
                    oSISToolStripMenuItem.Visible = true;
                    generateSWORDFilesToolStripMenuItem.Visible = false;
                    //saveHebrewToolStripMenuItem.Visible = false;
                    //saveKJVPlainToolStripMenuItem.Visible = false;

                    osisTarget.LoadBibleFile(files[0], true, false);
                }
                else
                    target.LoadBibleFile(files[0], true, false);

                AddAncientWords();

                VerseSelectionPanel.SetBookCount(target.BookCount);
            }

            WaitCursorControl(false);
        }
        private void UpdateBibles(SettingsFlags flags)
        {
            if (flags.TopRefChanged && !Properties.ReferenceBibles.Default.TopRefSkip)
            {
                WaitCursorControl(true);
                string refBibleName = Path.GetFileName(settingsForm.ReferenceTopVersionPath);
                referenceTopVersion.BibleName = refBibleName;
                editorPanel.TopReferenceBibleName = refBibleName;
                if (!LoadReferenceFiles(settingsForm.ReferenceTopVersionPath, referenceTopVersion)) { CloseForm(); return; }
                WaitCursorControl(false);
            }
            if (flags.MainOtChanged && !Properties.ReferenceBibles.Default.OtRefSkip)
            {
                WaitCursorControl(true);

                referenceTOTHT.BibleName = Properties.ReferenceBibles.Default.TAOTReference;//otReference;
                if (!LoadReferenceFiles(settingsForm.ReferenceTAOTPath, referenceTOTHT)) { CloseForm(); return; }

                WaitCursorControl(false);
            }

            if (flags.MainNtChanged && !Properties.ReferenceBibles.Default.NtRefSkip)
            {
                WaitCursorControl(true);
                
                referenceTAGNT.BibleName = Properties.ReferenceBibles.Default.TANTReference; //ntReference;
                if (!LoadReferenceFiles(settingsForm.ReferenceTANTPath, referenceTAGNT)) { CloseForm(); return; }
                
                WaitCursorControl(false);
            }

            if (flags.TargetBibleChanged || flags.VersificaltionChanged)
            {
                verseSelectionPanel.SetVersification();
                string currentRef = editorPanel.CurrentVerseRef;
                LoadTarget();
                verseSelectionPanel.GotoVerse(currentRef);
            }
        }

        List <string> foundArabicWords = new List <string>();
        public void FindVerse(BibleVersion version, string tag, bool singleTag=false)
        {
            try
            {
                string newRef = editorPanel.CurrentVerseRef;
                while (true)
                {
                    newRef = verseSelectionPanel.GetNextRef(newRef);
                    if (newRef == "Rev 22:21")
                    {
                        verseSelectionPanel.GotoVerse(newRef);
                        break;
                    }

                    string text = string.Empty;

                    try
                    {
                        string bookName = newRef.Substring(0, 3);
                        string targetRef = newRef.Replace(bookName, Target[bookName]);
                        text = Utils.GetVerseText(version.Bible[targetRef], true);
                    }
                    catch (Exception ex)
                    {
                        var cm = System.Reflection.MethodBase.GetCurrentMethod();
                        var name = cm.DeclaringType.FullName + "." + cm.Name;
                        Tracing.TraceException(name, ex.Message);
                    }

                    if ((string.IsNullOrEmpty(tag) || tag.ToLower() == "<blank>"))
                    {
                        if (text.Contains("<>"))
                        {
                            verseSelectionPanel.GotoVerse(newRef);
                            break;

                        }
                    }
                    else if (singleTag)
                    {
                        // first a quick check
                        if (!text.Contains(tag))
                            continue;

                        string pattern1 = $@"([ \u0600-\u06FF]+)\s(<{tag}>)\s([.,\u0600-\u06FF]+)";

                        Match m = Regex.Match(text, pattern1);
                        if (m.Success)
                        {
                            string word = Utils.RemoveDiacritics(m.Groups[1].Value);
                            if (foundArabicWords.Contains(word))
                                continue;
                            foundArabicWords.Add(word);
                            verseSelectionPanel.GotoVerse(newRef);
                            break;
                        }

                    }
                    else if (!singleTag && text.Contains(tag))
                    {
                        foundArabicWords.Clear();
                        verseSelectionPanel.GotoVerse(newRef);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                var cm = System.Reflection.MethodBase.GetCurrentMethod();
                var name = cm.DeclaringType.FullName + "." + cm.Name;
                Tracing.TraceException(name, ex.Message);
            }
        }


        /// <summary>
        /// finds the tag that is repeated in two consecutive cells
        /// </summary>
        public void FindRepetitive()
        {
            try
            {
                string newRef = editorPanel.CurrentVerseRef;
                bool more = true;
                while (more)
                {
                    newRef = verseSelectionPanel.GetNextRef(newRef);
                    if (newRef == "Rev 22:21")
                    {
                        // reached the end
                        // We may want to go back to Gen 1:1???
                        verseSelectionPanel.GotoVerse(newRef);
                        break;
                    }

                    string text = string.Empty;

                    try
                    {
                        string bookName = newRef.Substring(0, 3);
                        string targetRef = newRef.Replace(bookName, Target[bookName]);
                        Verse v = Target.Bible[targetRef];
                        for (int i = 0; i < (v.Count - 1); i++)
                        {
                            if (v[i].Strong[0] == v[i + 1].Strong[0])
                            {
                                verseSelectionPanel.GotoVerse(newRef);
                                more = false;
                                break;
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        var cm = System.Reflection.MethodBase.GetCurrentMethod();
                        var name = cm.DeclaringType.FullName + "." + cm.Name;
                        Tracing.TraceException(name, ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                var cm = System.Reflection.MethodBase.GetCurrentMethod();
                var name = cm.DeclaringType.FullName + "." + cm.Name;
                Tracing.TraceException(name, ex.Message);
            }
        }

        #region Generate SWORD Files Main Menu
        private void generateSWORDFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Thread(
                () =>
                {
                    try
                    {
                        WaitCursorControl(true);
                        GenerateOsisXML();
                        WaitCursorControl(false);
                        RunOsis2mod(config.OSIS[OsisConstants.output_file], config.OSIS[OsisConstants.osisIDWork]);
                    }
                    catch (Exception ex)
                    {
                        var cm = System.Reflection.MethodBase.GetCurrentMethod();
                        var name = cm.DeclaringType.FullName + "." + cm.Name;
                        Tracing.TraceException(name, ex.Message);
                        HandleException(ex);
                    }
                }).Start();
        }
        #endregion Generate SWORD Files Main Menu

        #region OSIS Menue
        private void generateOSISToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Thread(
        () =>
        {
            try
            {
                WaitCursorControl(true);
                UpdateProgress("Generating OSIS File", -1);
                GenerateOsisXML();
                WaitCursorControl(false);
            }
            catch (Exception ex)
            {
                var cm = System.Reflection.MethodBase.GetCurrentMethod();
                var name = cm.DeclaringType.FullName + "." + cm.Name;
                Tracing.TraceException(name, ex.Message);
                HandleException(ex);
            }
        }).Start();
        }

        private void GenerateOsisXML()
        {
            OsisGenerator generator = new OsisGenerator(config);
            string n = Properties.TargetBibles.Default.TargetBible;
            if (Properties.TargetBibles.Default.TargetBible.ToLower().Contains("ara"))
            {
                generator.Generate(target, true);
                generator.Generate(target, true, true); // for Injeel
            }
            else
                generator.Generate(target, false);
        }

        private void generateSWORDFilesOsisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            target.SaveUpdates();

            if (Properties.MainSettings.Default.Osis)
            {
                // copy last changes to the output OSIS xml
                string targetBiblesFolder = BibleTaggingUtil.Properties.TargetBibles.Default.TargetBiblesFolder;
                string targetBible = BibleTaggingUtil.Properties.TargetBibles.Default.TargetBible;
                string biblesFolder = Path.Combine(targetBiblesFolder, targetBible);

                string taggedFolder = Path.Combine(biblesFolder, "tagged");
                string LastChanged = Directory.GetFiles(taggedFolder)[0];
                string outputXml = Path.Combine(biblesFolder, config.OSIS[OsisConstants.output_file]);
                System.IO.File.Copy(LastChanged, outputXml, true);
            }

            new Thread(
                () =>
                {
                    try
                    {
                        string outputFile = config.OSIS[OsisConstants.output_file];
                        string outExt = Path.GetExtension(outputFile);
                        string outName = Path.GetFileNameWithoutExtension(outputFile);
                        if (config.OSIS.ContainsKey(OsisConstants.revision))
                        {
                            outName += "-" + config.OSIS[OsisConstants.revision];
                        }
                        outputFile = string.Format("{0}.{1}", outName, outExt);

                        //RunOsis2mod(config.OSIS[OsisConstants.output_file], config.OSIS[OsisConstants.osisIDWork]);
                        RunOsis2mod(outputFile, config.OSIS[OsisConstants.osisIDWork]);
                    }
                    catch (Exception ex)
                    {
                        var cm = System.Reflection.MethodBase.GetCurrentMethod();
                        var name = cm.DeclaringType.FullName + "." + cm.Name;
                        Tracing.TraceException(name, ex.Message);
                        HandleException(ex);
                    }
                }).Start();
        }

        #endregion OSIS Menue


        #region USFM Menu
        private void generateUSFMFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            target.SaveUpdates();
            new Thread(
                () =>
                {
                    try
                    {
                        WaitCursorControl(true);
                        USFM_Generator generator = new USFM_Generator(this, config);
                        generator.Generate();
                        WaitCursorControl(false);
                    }
                    catch (Exception ex)
                    {
                        var cm = System.Reflection.MethodBase.GetCurrentMethod();
                        var name = cm.DeclaringType.FullName + "." + cm.Name;
                        Tracing.TraceException(name, ex.Message);
                        HandleException(ex);
                    }
                }).Start();
        }

        private void convertUSFMToOSISToolStripMenuItem_Click(object sender, EventArgs e)
        {
            USFM2OSIS usfm2osis = new USFM2OSIS(this, config);
            new Thread(
               () =>
               {
                   try
                   {
                       WaitCursorControl(true);
                       usfm2osis.Convert();
                       WaitCursorControl(false);
                   }
                   catch (Exception ex)
                   {
                       var cm = System.Reflection.MethodBase.GetCurrentMethod();
                       var name = cm.DeclaringType.FullName + "." + cm.Name;
                       Tracing.TraceException(name, ex.Message);
                       HandleException(ex);
                   }
               }).Start();
        }

        private void generateSWORDFilesUsfmToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                try
                {
                    RunOsis2mod(config.USFM2OSIS[Usfm2OsisConstants.outputFileName], config.USFM2OSIS[Usfm2OsisConstants.osisIDWork]);
                }
                catch (Exception ex)
                {
                    var cm = System.Reflection.MethodBase.GetCurrentMethod();
                    var name = cm.DeclaringType.FullName + "." + cm.Name;
                    Tracing.TraceException(name, ex.Message);
                    HandleException(ex);
                }
            }).Start();

        }
        #endregion USFM Menu

        #region usfm2osis
        private void RunOsis2mod(string sourceFileName, string targetFolderName)
        {
            string executable = string.Empty;
            string targetFolder = string.Empty;
            string xmlFile = string.Empty;

            try
            {
                WaitCursorControl(true);
                UpdateProgress("Creating SWORD Module", -1);
                string targetBiblesFolder = BibleTaggingUtil.Properties.TargetBibles.Default.TargetBiblesFolder;
                string targetBible = BibleTaggingUtil.Properties.TargetBibles.Default.TargetBible;
                string biblesFolder = Path.Combine(targetBiblesFolder, targetBible);

                string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string modulesFolder = Path.Combine(appData, "Sword\\modules\\texts\\ztext");
                string backupFolderName = string.Format("{0:s}_{1:s}", targetFolderName, DateTime.Now.ToString("yyyy_MM_dd_HH_mm"));
                string backupPath = Path.Combine(biblesFolder, backupFolderName);
                targetFolder = Path.Combine(modulesFolder, targetFolderName);
                if (Directory.Exists(targetFolder))
                {
                    // backup currentModule
                    if (Directory.Exists(backupPath))
                    {
                        DialogResult res = ShowMessageBox("Overwrite old backup", "Do you want to overwrite existing Backup folder\r\n" + backupPath, MessageBoxButtons.YesNo);
                        if (res == DialogResult.Yes)
                        {
                            Directory.Delete(backupPath, true);
                        }
                        else
                        {
                            return;
                        }
                    }
                    Directory.CreateDirectory(backupPath);

                    // copy targetFolder to backupPath
                    var allFiles = Directory.GetFiles(targetFolder, "*.*");
                    foreach (string file in allFiles)
                    {
                        System.IO.File.Copy(file, file.Replace(targetFolder, backupPath));
                    }
                }


                executable = Path.Combine(crosswirePath, "osis2mod.exe");
                xmlFile = Path.Combine(biblesFolder, sourceFileName);
                if (!Directory.Exists(targetFolder))
                {
                    Directory.CreateDirectory(targetFolder);
                }
                else
                {
                    string[] files = Directory.GetFiles(targetFolder);
                    foreach (string file in files)
                    {
                        System.IO.File.Delete(file);
                    }
                }

            }
            catch (Exception ex)
            {
                var cm = System.Reflection.MethodBase.GetCurrentMethod();
                var name = cm.DeclaringType.FullName + "." + cm.Name;
                Tracing.TraceException(name, ex.Message);
            }


            Process process = new Process();
            StringBuilder outputStringBuilder = new StringBuilder();
            StringBuilder errorStringBuilder = new StringBuilder();
            try
            {
                process.StartInfo.FileName = executable;
                process.StartInfo.WorkingDirectory = crosswirePath;
                string versification = config.OSIS[OsisConstants.versification];
                if (string.IsNullOrEmpty(versification))
                    versification = "KJV";
                process.StartInfo.Arguments = string.Format("{0} {1} -v {2} -b 4 -z", targetFolder, xmlFile, versification);

                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;

                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;

                process.EnableRaisingEvents = false;

                process.OutputDataReceived += (sender, eventArgs) => outputStringBuilder.AppendLine(eventArgs.Data);
                process.ErrorDataReceived += (sender, eventArgs) => errorStringBuilder.AppendLine(eventArgs.Data);

                Tracing.TraceInfo(MethodBase.GetCurrentMethod().Name, "osis2mod arguments = " + process.StartInfo.Arguments);

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                var processExited = process.WaitForExit(240000);
                process.CancelOutputRead();

                int secondCounter = 0;
                while (errorStringBuilder.Length == 0)
                {
                    Thread.Sleep(1000);
                    if (++secondCounter > 15)
                        break;
                }
                string output = outputStringBuilder.ToString();
                string error = errorStringBuilder.ToString();

                WaitCursorControl(false);
                if (processExited == false) // we timed out...
                {
                    process.Kill();
                    MessageBox.Show("Module Generation Timed out!");
                }
                else
                {
                    if (process.ExitCode == 0)
                    {
                        Tracing.TraceInfo(MethodBase.GetCurrentMethod().Name, error);
                        MessageBox.Show("Module Generation Completed Successfully!");
                    }
                    else
                    {
                        Tracing.TraceError(MethodBase.GetCurrentMethod().Name, error);
                        MessageBox.Show("Module Generation failed! " + process.ExitCode.ToString());
                    }
                }

            }
            catch (Exception ex)
            {
                var cm = System.Reflection.MethodBase.GetCurrentMethod();
                var name = cm.DeclaringType.FullName + "." + cm.Name;
                Tracing.TraceException(name, ex.Message);
            }
            finally
            {
                process.Close();
            }
            WaitCursorControl(false);
        }
        #endregion usfm2osis

        private void saveUpdatedTartgetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            target.SaveUpdates();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm aboutForm = new AboutForm();

            aboutForm.ShowDialog();

        }

        private void reloadTargetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                try
                {
                    string currentRef = editorPanel.CurrentVerseRef;
                    LoadTarget();
                    verseSelectionPanel.GotoVerse(currentRef);
                }
                catch (Exception ex)
                {
                    var cm = System.Reflection.MethodBase.GetCurrentMethod();
                    var name = cm.DeclaringType.FullName + "." + cm.Name;
                    Tracing.TraceException(name, ex.Message);
                    HandleException(ex);
                }
            }).Start();
            return;
            string taggedFolder = Path.GetDirectoryName(config.TaggedBible);
            string taggedFolderParent = Path.GetDirectoryName(taggedFolder);
            string bibleName = Path.GetFileName(taggedFolderParent);
            string[] files = Directory.GetFiles(taggedFolder);
            if (files.Length > 0)
            {
                WaitCursorControl(true);
                target.BibleName = bibleName;
                target.LoadBibleFile(files[0], true, false);
                WaitCursorControl(false);
                VerseSelectionPanel.SetBookCount(target.BookCount);
            }
        }

        internal void EnableSaveButton(bool v)
        {
            editorPanel.EnableSaveButton(v);
        }

        private void exportTranslatorTagsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                try
                {
                    ExportTranslatorTags();
                }
                catch (Exception ex)
                {
                    var cm = System.Reflection.MethodBase.GetCurrentMethod();
                    var name = cm.DeclaringType.FullName + "." + cm.Name;
                    Tracing.TraceException(name, ex.Message);
                    HandleException(ex);
                }
            }).Start();

        }
        private void ExportTranslatorTags()
        {
            if (translationTags == null)
                translationTags = new TranslationTagsEx(this);

            if (Properties.MainSettings.Default.Osis)
                translationTags.Export(OsisTarget, TOTHT, TAGNT);
            else
                translationTags.Export(Target, TOTHT, TAGNT);
        }


        private void serachReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SearchReportForm search = new SearchReportForm(Target, TOTHT, TAGNT, TopVersion);
            search.ShowDialog();
        }

        private void tAHOTEnglishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = @"c:\temp\tahot_eng.txt";
            using (StreamWriter sw = new StreamWriter(path))
            {
                foreach ((string reference, Verse verse) in referenceTOTHT.Bible)
                {
                    string originalRef = referenceTOTHT.GetCorrectReference(reference);
                    string verseReference = originalRef.Replace(" ", ".").Replace(":", ".");


                    foreach (VerseWord word in verse)
                    {
                        string line = string.Format("{0}#{1}\t{2}", verseReference, word.WordNumber, word.Word);
                        sw.WriteLine(line);

                    }
                }
            }
        }

        private void EnableRestoreMenu()
        {
            string targetBibleFolder = Path.Combine(Properties.TargetBibles.Default.TargetBiblesFolder,
                                        Properties.TargetBibles.Default.TargetBible);
            string taggedFolder = Path.Combine(targetBibleFolder, "tagged");
            string oldTaggedFolder = Path.Combine(taggedFolder, "OldTagged");
            if (Directory.Exists(oldTaggedFolder) && Directory.GetFiles(oldTaggedFolder).Length > 0)
            {
                restoreToolStripMenuItem.Enabled = true;
            }
            else
            { restoreToolStripMenuItem.Enabled = false; }
        }


        private void restoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (restoreTarget == null)
                restoreTarget = new RestoreTarget();

            DialogResult result = restoreTarget.ShowDialog();
            if(result == DialogResult.OK)
            {
                new Thread(() =>
                {
                    try
                    {
                        string currentRef = editorPanel.CurrentVerseRef;
                        LoadTarget();
                        verseSelectionPanel.GotoVerse(currentRef);
                    }
                    catch (Exception ex)
                    {
                        var cm = System.Reflection.MethodBase.GetCurrentMethod();
                        var name = cm.DeclaringType.FullName + "." + cm.Name;
                        Tracing.TraceException(name, ex.Message);
                        HandleException(ex);
                    }
                }).Start();
            }

            restoreTarget.Dispose();
            restoreTarget = null;

        }
    }
}
