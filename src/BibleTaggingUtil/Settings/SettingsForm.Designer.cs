
namespace BibleTaggingUtil.Settings
{
    partial class SettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            cbPeriodicSave = new System.Windows.Forms.CheckBox();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            btnCancel = new System.Windows.Forms.Button();
            btnOK = new System.Windows.Forms.Button();
            nudSavePeriod = new System.Windows.Forms.NumericUpDown();
            tabControl1 = new System.Windows.Forms.TabControl();
            targetBible = new System.Windows.Forms.TabPage();
            checkBoxRTL = new System.Windows.Forms.CheckBox();
            cbTargetBibles = new System.Windows.Forms.ComboBox();
            label14 = new System.Windows.Forms.Label();
            tbTargetBiblesFolder = new System.Windows.Forms.TextBox();
            label13 = new System.Windows.Forms.Label();
            button1 = new System.Windows.Forms.Button();
            referenceBibles = new System.Windows.Forms.TabPage();
            checkbNtRefSkip = new System.Windows.Forms.CheckBox();
            checkbOtRefSkip = new System.Windows.Forms.CheckBox();
            checkbTopRefSkip = new System.Windows.Forms.CheckBox();
            cbMainNT = new System.Windows.Forms.ComboBox();
            cbMainOT = new System.Windows.Forms.ComboBox();
            cbTopReference = new System.Windows.Forms.ComboBox();
            label5 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            translationTags = new System.Windows.Forms.TabPage();
            checkbPublicDomain = new System.Windows.Forms.CheckBox();
            checkbAppendTimestamp = new System.Windows.Forms.CheckBox();
            checkbFilesPerBook = new System.Windows.Forms.CheckBox();
            tbRepeatedWord = new System.Windows.Forms.TextBox();
            label12 = new System.Windows.Forms.Label();
            tbForReviewFileName = new System.Windows.Forms.TextBox();
            label10 = new System.Windows.Forms.Label();
            tbMissingWordsFileName = new System.Windows.Forms.TextBox();
            label9 = new System.Windows.Forms.Label();
            tbErrorsFileName = new System.Windows.Forms.TextBox();
            label8 = new System.Windows.Forms.Label();
            tbOutputFileName = new System.Windows.Forms.TextBox();
            label7 = new System.Windows.Forms.Label();
            tbVersion = new System.Windows.Forms.TextBox();
            tbTtFolder = new System.Windows.Forms.TextBox();
            label11 = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            btnTTFolder = new System.Windows.Forms.Button();
            osisGeneration = new System.Windows.Forms.TabPage();
            checkBoxUseDisambiguatedStrong = new System.Windows.Forms.CheckBox();
            periodicSave = new System.Windows.Forms.TabPage();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            panel1 = new System.Windows.Forms.Panel();
            translationTagsFolderDialog = new System.Windows.Forms.FolderBrowserDialog();
            ttRefBibleSkip = new System.Windows.Forms.ToolTip(components);
            targetBiblesFolderDialog = new System.Windows.Forms.FolderBrowserDialog();
            label15 = new System.Windows.Forms.Label();
            cbVersification = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)nudSavePeriod).BeginInit();
            tabControl1.SuspendLayout();
            targetBible.SuspendLayout();
            referenceBibles.SuspendLayout();
            translationTags.SuspendLayout();
            osisGeneration.SuspendLayout();
            periodicSave.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // cbPeriodicSave
            // 
            cbPeriodicSave.AutoSize = true;
            cbPeriodicSave.Location = new System.Drawing.Point(2, 32);
            cbPeriodicSave.Name = "cbPeriodicSave";
            cbPeriodicSave.Size = new System.Drawing.Size(119, 24);
            cbPeriodicSave.TabIndex = 0;
            cbPeriodicSave.Text = "Periodic Save";
            cbPeriodicSave.UseVisualStyleBackColor = true;
            cbPeriodicSave.CheckedChanged += cbPeriodicSave_CheckedChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(28, 71);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(44, 20);
            label1.TabIndex = 1;
            label1.Text = "Evrey";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(168, 71);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(61, 20);
            label2.TabIndex = 3;
            label2.Text = "Minutes";
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            btnCancel.Location = new System.Drawing.Point(482, 18);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(94, 35);
            btnCancel.TabIndex = 4;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            btnOK.BackColor = System.Drawing.Color.PaleGreen;
            btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            btnOK.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnOK.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            btnOK.Location = new System.Drawing.Point(146, 18);
            btnOK.Name = "btnOK";
            btnOK.Size = new System.Drawing.Size(94, 35);
            btnOK.TabIndex = 4;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = false;
            btnOK.Click += btnOK_Click;
            // 
            // nudSavePeriod
            // 
            nudSavePeriod.Enabled = false;
            nudSavePeriod.Location = new System.Drawing.Point(89, 69);
            nudSavePeriod.Maximum = new decimal(new int[] { 60, 0, 0, 0 });
            nudSavePeriod.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudSavePeriod.Name = "nudSavePeriod";
            nudSavePeriod.Size = new System.Drawing.Size(58, 27);
            nudSavePeriod.TabIndex = 5;
            nudSavePeriod.Value = new decimal(new int[] { 10, 0, 0, 0 });
            // 
            // tabControl1
            // 
            tabControl1.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            tabControl1.Controls.Add(targetBible);
            tabControl1.Controls.Add(referenceBibles);
            tabControl1.Controls.Add(translationTags);
            tabControl1.Controls.Add(osisGeneration);
            tabControl1.Controls.Add(periodicSave);
            tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            tabControl1.Location = new System.Drawing.Point(3, 3);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(793, 474);
            tabControl1.TabIndex = 6;
            // 
            // targetBible
            // 
            targetBible.Controls.Add(cbVersification);
            targetBible.Controls.Add(label15);
            targetBible.Controls.Add(checkBoxRTL);
            targetBible.Controls.Add(cbTargetBibles);
            targetBible.Controls.Add(label14);
            targetBible.Controls.Add(tbTargetBiblesFolder);
            targetBible.Controls.Add(label13);
            targetBible.Controls.Add(button1);
            targetBible.Location = new System.Drawing.Point(4, 29);
            targetBible.Name = "targetBible";
            targetBible.Size = new System.Drawing.Size(785, 441);
            targetBible.TabIndex = 4;
            targetBible.Text = "Target Bible";
            targetBible.UseVisualStyleBackColor = true;
            // 
            // checkBoxRTL
            // 
            checkBoxRTL.AutoSize = true;
            checkBoxRTL.Location = new System.Drawing.Point(208, 145);
            checkBoxRTL.Name = "checkBoxRTL";
            checkBoxRTL.Size = new System.Drawing.Size(146, 24);
            checkBoxRTL.TabIndex = 13;
            checkBoxRTL.Text = "Right To Left Text";
            checkBoxRTL.UseVisualStyleBackColor = true;
            checkBoxRTL.CheckedChanged += checkBoxRTL_CheckedChanged;
            // 
            // cbTargetBibles
            // 
            cbTargetBibles.FormattingEnabled = true;
            cbTargetBibles.Location = new System.Drawing.Point(208, 89);
            cbTargetBibles.Name = "cbTargetBibles";
            cbTargetBibles.Size = new System.Drawing.Size(483, 28);
            cbTargetBibles.TabIndex = 12;
            cbTargetBibles.SelectedIndexChanged += cbTargetBibles_SelectedIndexChanged;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new System.Drawing.Point(18, 92);
            label14.Name = "label14";
            label14.Size = new System.Drawing.Size(88, 20);
            label14.TabIndex = 11;
            label14.Text = "Target Bible";
            // 
            // tbTargetBiblesFolder
            // 
            tbTargetBiblesFolder.Location = new System.Drawing.Point(208, 37);
            tbTargetBiblesFolder.Name = "tbTargetBiblesFolder";
            tbTargetBiblesFolder.Size = new System.Drawing.Size(483, 27);
            tbTargetBiblesFolder.TabIndex = 9;
            tbTargetBiblesFolder.Text = "Target Bibles Folder";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new System.Drawing.Point(18, 39);
            label13.Name = "label13";
            label13.Size = new System.Drawing.Size(140, 20);
            label13.TabIndex = 8;
            label13.Text = "Target Bibles Folder";
            // 
            // button1
            // 
            button1.BackColor = System.Drawing.SystemColors.ButtonFace;
            button1.BackgroundImage = Properties.Resources.ellipsisTX;
            button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            button1.Font = new System.Drawing.Font("Wingdings", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            button1.Location = new System.Drawing.Point(729, 35);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(33, 31);
            button1.TabIndex = 10;
            button1.Text = "Target Bibles Folder";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // referenceBibles
            // 
            referenceBibles.Controls.Add(checkbNtRefSkip);
            referenceBibles.Controls.Add(checkbOtRefSkip);
            referenceBibles.Controls.Add(checkbTopRefSkip);
            referenceBibles.Controls.Add(cbMainNT);
            referenceBibles.Controls.Add(cbMainOT);
            referenceBibles.Controls.Add(cbTopReference);
            referenceBibles.Controls.Add(label5);
            referenceBibles.Controls.Add(label4);
            referenceBibles.Controls.Add(label3);
            referenceBibles.Location = new System.Drawing.Point(4, 29);
            referenceBibles.Name = "referenceBibles";
            referenceBibles.Size = new System.Drawing.Size(785, 441);
            referenceBibles.TabIndex = 2;
            referenceBibles.Text = "Reference Bibles";
            referenceBibles.UseVisualStyleBackColor = true;
            // 
            // checkbNtRefSkip
            // 
            checkbNtRefSkip.AutoSize = true;
            checkbNtRefSkip.Location = new System.Drawing.Point(567, 141);
            checkbNtRefSkip.Name = "checkbNtRefSkip";
            checkbNtRefSkip.Size = new System.Drawing.Size(59, 24);
            checkbNtRefSkip.TabIndex = 2;
            checkbNtRefSkip.Text = "Skip";
            checkbNtRefSkip.UseVisualStyleBackColor = true;
            checkbNtRefSkip.CheckedChanged += CheckbNtRefSkip_CheckedChanged;
            // 
            // checkbOtRefSkip
            // 
            checkbOtRefSkip.AutoSize = true;
            checkbOtRefSkip.Location = new System.Drawing.Point(567, 102);
            checkbOtRefSkip.Name = "checkbOtRefSkip";
            checkbOtRefSkip.Size = new System.Drawing.Size(59, 24);
            checkbOtRefSkip.TabIndex = 2;
            checkbOtRefSkip.Text = "Skip";
            checkbOtRefSkip.UseVisualStyleBackColor = true;
            checkbOtRefSkip.CheckedChanged += CheckbOtRefSkip_CheckedChanged;
            // 
            // checkbTopRefSkip
            // 
            checkbTopRefSkip.AutoSize = true;
            checkbTopRefSkip.Location = new System.Drawing.Point(567, 58);
            checkbTopRefSkip.Name = "checkbTopRefSkip";
            checkbTopRefSkip.Size = new System.Drawing.Size(59, 24);
            checkbTopRefSkip.TabIndex = 2;
            checkbTopRefSkip.Text = "Skip";
            ttRefBibleSkip.SetToolTip(checkbTopRefSkip, "Takes effect after restart");
            checkbTopRefSkip.UseVisualStyleBackColor = true;
            checkbTopRefSkip.CheckedChanged += CheckbTopRefSkip_CheckedChanged;
            // 
            // cbMainNT
            // 
            cbMainNT.AccessibleDescription = "";
            cbMainNT.FormattingEnabled = true;
            cbMainNT.Location = new System.Drawing.Point(268, 142);
            cbMainNT.Name = "cbMainNT";
            cbMainNT.Size = new System.Drawing.Size(272, 28);
            cbMainNT.TabIndex = 1;
            cbMainNT.SelectedIndexChanged += cbMainNT_SelectedIndexChanged;
            // 
            // cbMainOT
            // 
            cbMainOT.FormattingEnabled = true;
            cbMainOT.Location = new System.Drawing.Point(268, 98);
            cbMainOT.Name = "cbMainOT";
            cbMainOT.Size = new System.Drawing.Size(272, 28);
            cbMainOT.TabIndex = 1;
            cbMainOT.SelectedIndexChanged += cbMainOT_SelectedIndexChanged;
            // 
            // cbTopReference
            // 
            cbTopReference.FormattingEnabled = true;
            cbTopReference.Location = new System.Drawing.Point(268, 54);
            cbTopReference.Name = "cbTopReference";
            cbTopReference.Size = new System.Drawing.Size(272, 28);
            cbTopReference.TabIndex = 1;
            cbTopReference.SelectedIndexChanged += cbTopReference_SelectedIndexChanged;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(69, 145);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(177, 20);
            label5.TabIndex = 0;
            label5.Text = "Main NT  Reference Bible";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(69, 98);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(176, 20);
            label4.TabIndex = 0;
            label4.Text = "Main OT  Reference Bible";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(69, 53);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(142, 20);
            label3.TabIndex = 0;
            label3.Text = "Top Reference Bible";
            // 
            // translationTags
            // 
            translationTags.Controls.Add(checkbPublicDomain);
            translationTags.Controls.Add(checkbAppendTimestamp);
            translationTags.Controls.Add(checkbFilesPerBook);
            translationTags.Controls.Add(tbRepeatedWord);
            translationTags.Controls.Add(label12);
            translationTags.Controls.Add(tbForReviewFileName);
            translationTags.Controls.Add(label10);
            translationTags.Controls.Add(tbMissingWordsFileName);
            translationTags.Controls.Add(label9);
            translationTags.Controls.Add(tbErrorsFileName);
            translationTags.Controls.Add(label8);
            translationTags.Controls.Add(tbOutputFileName);
            translationTags.Controls.Add(label7);
            translationTags.Controls.Add(tbVersion);
            translationTags.Controls.Add(tbTtFolder);
            translationTags.Controls.Add(label11);
            translationTags.Controls.Add(label6);
            translationTags.Controls.Add(btnTTFolder);
            translationTags.Location = new System.Drawing.Point(4, 29);
            translationTags.Name = "translationTags";
            translationTags.Padding = new System.Windows.Forms.Padding(3);
            translationTags.Size = new System.Drawing.Size(785, 441);
            translationTags.TabIndex = 1;
            translationTags.Text = "Translation Tags";
            translationTags.UseVisualStyleBackColor = true;
            // 
            // checkbPublicDomain
            // 
            checkbPublicDomain.AutoSize = true;
            checkbPublicDomain.Location = new System.Drawing.Point(558, 392);
            checkbPublicDomain.Name = "checkbPublicDomain";
            checkbPublicDomain.Size = new System.Drawing.Size(128, 24);
            checkbPublicDomain.TabIndex = 9;
            checkbPublicDomain.Text = "Public Domain";
            checkbPublicDomain.UseVisualStyleBackColor = true;
            checkbPublicDomain.CheckedChanged += checkbPublicDomain_CheckedChanged;
            // 
            // checkbAppendTimestamp
            // 
            checkbAppendTimestamp.AutoSize = true;
            checkbAppendTimestamp.Location = new System.Drawing.Point(231, 392);
            checkbAppendTimestamp.Name = "checkbAppendTimestamp";
            checkbAppendTimestamp.Size = new System.Drawing.Size(253, 24);
            checkbAppendTimestamp.TabIndex = 8;
            checkbAppendTimestamp.Text = "Append Timestamp To File Name";
            checkbAppendTimestamp.UseVisualStyleBackColor = true;
            checkbAppendTimestamp.CheckedChanged += checkbAppendTimestamp_CheckedChanged;
            // 
            // checkbFilesPerBook
            // 
            checkbFilesPerBook.AutoSize = true;
            checkbFilesPerBook.Location = new System.Drawing.Point(13, 392);
            checkbFilesPerBook.Name = "checkbFilesPerBook";
            checkbFilesPerBook.Size = new System.Drawing.Size(186, 24);
            checkbFilesPerBook.TabIndex = 8;
            checkbFilesPerBook.Text = "Generate Files Per Book";
            checkbFilesPerBook.UseVisualStyleBackColor = true;
            checkbFilesPerBook.CheckedChanged += checkbFilesPerBook_CheckedChanged;
            // 
            // tbRepeatedWord
            // 
            tbRepeatedWord.Location = new System.Drawing.Point(203, 331);
            tbRepeatedWord.Name = "tbRepeatedWord";
            tbRepeatedWord.Size = new System.Drawing.Size(483, 27);
            tbRepeatedWord.TabIndex = 6;
            tbRepeatedWord.TextChanged += TbRepeatedWord_TextChanged;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new System.Drawing.Point(13, 333);
            label12.Name = "label12";
            label12.Size = new System.Drawing.Size(184, 20);
            label12.TabIndex = 5;
            label12.Text = "Repeated Word File Name";
            // 
            // tbForReviewFileName
            // 
            tbForReviewFileName.Location = new System.Drawing.Point(203, 274);
            tbForReviewFileName.Name = "tbForReviewFileName";
            tbForReviewFileName.Size = new System.Drawing.Size(483, 27);
            tbForReviewFileName.TabIndex = 6;
            tbForReviewFileName.TextChanged += TbForReviewFileName_TextChanged;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new System.Drawing.Point(13, 276);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(152, 20);
            label10.TabIndex = 5;
            label10.Text = "For Review File Name";
            // 
            // tbMissingWordsFileName
            // 
            tbMissingWordsFileName.Location = new System.Drawing.Point(203, 221);
            tbMissingWordsFileName.Name = "tbMissingWordsFileName";
            tbMissingWordsFileName.Size = new System.Drawing.Size(483, 27);
            tbMissingWordsFileName.TabIndex = 6;
            tbMissingWordsFileName.TextChanged += TbMissingWordsFileName_TextChanged;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new System.Drawing.Point(13, 223);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(176, 20);
            label9.TabIndex = 5;
            label9.Text = "Missing Words File Name";
            // 
            // tbErrorsFileName
            // 
            tbErrorsFileName.Location = new System.Drawing.Point(203, 164);
            tbErrorsFileName.Name = "tbErrorsFileName";
            tbErrorsFileName.Size = new System.Drawing.Size(483, 27);
            tbErrorsFileName.TabIndex = 6;
            tbErrorsFileName.TextChanged += TbErrorsFileName_TextChanged;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(13, 166);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(118, 20);
            label8.TabIndex = 5;
            label8.Text = "Errors File Name";
            // 
            // tbOutputFileName
            // 
            tbOutputFileName.Location = new System.Drawing.Point(203, 108);
            tbOutputFileName.Name = "tbOutputFileName";
            tbOutputFileName.Size = new System.Drawing.Size(483, 27);
            tbOutputFileName.TabIndex = 6;
            tbOutputFileName.TextChanged += TbOutputFileName_TextChanged;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(13, 110);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(126, 20);
            label7.TabIndex = 5;
            label7.Text = "Output File Name";
            // 
            // tbVersion
            // 
            tbVersion.Location = new System.Drawing.Point(203, 12);
            tbVersion.Name = "tbVersion";
            tbVersion.Size = new System.Drawing.Size(140, 27);
            tbVersion.TabIndex = 6;
            tbVersion.TextChanged += TbVersion_TextChanged;
            // 
            // tbTtFolder
            // 
            tbTtFolder.Location = new System.Drawing.Point(203, 60);
            tbTtFolder.Name = "tbTtFolder";
            tbTtFolder.Size = new System.Drawing.Size(483, 27);
            tbTtFolder.TabIndex = 6;
            tbTtFolder.TextChanged += tbTtFolder_TextChanged;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new System.Drawing.Point(13, 15);
            label11.Name = "label11";
            label11.Size = new System.Drawing.Size(57, 20);
            label11.TabIndex = 5;
            label11.Text = "Version";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(13, 62);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(160, 20);
            label6.TabIndex = 5;
            label6.Text = "Translation Tags Folder";
            // 
            // btnTTFolder
            // 
            btnTTFolder.BackColor = System.Drawing.SystemColors.ButtonFace;
            btnTTFolder.BackgroundImage = Properties.Resources.ellipsisTX;
            btnTTFolder.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            btnTTFolder.Font = new System.Drawing.Font("Wingdings", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            btnTTFolder.Location = new System.Drawing.Point(724, 58);
            btnTTFolder.Name = "btnTTFolder";
            btnTTFolder.Size = new System.Drawing.Size(33, 31);
            btnTTFolder.TabIndex = 7;
            btnTTFolder.UseVisualStyleBackColor = false;
            btnTTFolder.Click += btnTTFolder_Click;
            // 
            // osisGeneration
            // 
            osisGeneration.Controls.Add(checkBoxUseDisambiguatedStrong);
            osisGeneration.Location = new System.Drawing.Point(4, 29);
            osisGeneration.Name = "osisGeneration";
            osisGeneration.Size = new System.Drawing.Size(785, 441);
            osisGeneration.TabIndex = 3;
            osisGeneration.Text = "OSIS Generation";
            osisGeneration.UseVisualStyleBackColor = true;
            // 
            // checkBoxUseDisambiguatedStrong
            // 
            checkBoxUseDisambiguatedStrong.AutoSize = true;
            checkBoxUseDisambiguatedStrong.BackColor = System.Drawing.Color.WhiteSmoke;
            checkBoxUseDisambiguatedStrong.Location = new System.Drawing.Point(66, 51);
            checkBoxUseDisambiguatedStrong.Name = "checkBoxUseDisambiguatedStrong";
            checkBoxUseDisambiguatedStrong.Size = new System.Drawing.Size(209, 24);
            checkBoxUseDisambiguatedStrong.TabIndex = 0;
            checkBoxUseDisambiguatedStrong.Text = "Use Disambiguated Strong";
            checkBoxUseDisambiguatedStrong.UseVisualStyleBackColor = false;
            checkBoxUseDisambiguatedStrong.CheckedChanged += checkBoxUseDisambiguatedStrong_CheckedChanged;
            // 
            // periodicSave
            // 
            periodicSave.Controls.Add(nudSavePeriod);
            periodicSave.Controls.Add(cbPeriodicSave);
            periodicSave.Controls.Add(label1);
            periodicSave.Controls.Add(label2);
            periodicSave.Location = new System.Drawing.Point(4, 29);
            periodicSave.Name = "periodicSave";
            periodicSave.Padding = new System.Windows.Forms.Padding(3);
            periodicSave.Size = new System.Drawing.Size(785, 441);
            periodicSave.TabIndex = 0;
            periodicSave.Text = "Periodic Save";
            periodicSave.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(tabControl1, 0, 0);
            tableLayoutPanel1.Controls.Add(panel1, 0, 1);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 85.409256F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.5907469F));
            tableLayoutPanel1.Size = new System.Drawing.Size(799, 562);
            tableLayoutPanel1.TabIndex = 7;
            // 
            // panel1
            // 
            panel1.Controls.Add(btnCancel);
            panel1.Controls.Add(btnOK);
            panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            panel1.Location = new System.Drawing.Point(3, 483);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(793, 76);
            panel1.TabIndex = 7;
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Location = new System.Drawing.Point(18, 192);
            label15.Name = "label15";
            label15.Size = new System.Drawing.Size(90, 20);
            label15.TabIndex = 14;
            label15.Text = "Versification";
            // 
            // cbVersification
            // 
            cbVersification.FormattingEnabled = true;
            cbVersification.Location = new System.Drawing.Point(208, 192);
            cbVersification.Name = "cbVersification";
            cbVersification.Size = new System.Drawing.Size(483, 28);
            cbVersification.TabIndex = 15;
            cbVersification.SelectedIndexChanged += cbVersification_SelectedIndexChanged;
            // 
            // SettingsForm
            // 
            AcceptButton = btnOK;
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new System.Drawing.Size(799, 562);
            Controls.Add(tableLayoutPanel1);
            Name = "SettingsForm";
            Text = "SettingsForm";
            Load += SettingsForm_Load;
            ((System.ComponentModel.ISupportInitialize)nudSavePeriod).EndInit();
            tabControl1.ResumeLayout(false);
            targetBible.ResumeLayout(false);
            targetBible.PerformLayout();
            referenceBibles.ResumeLayout(false);
            referenceBibles.PerformLayout();
            translationTags.ResumeLayout(false);
            translationTags.PerformLayout();
            osisGeneration.ResumeLayout(false);
            osisGeneration.PerformLayout();
            periodicSave.ResumeLayout(false);
            periodicSave.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.CheckBox cbPeriodicSave;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.NumericUpDown nudSavePeriod;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage periodicSave;
        private System.Windows.Forms.TabPage translationTags;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TabPage referenceBibles;
        private System.Windows.Forms.ComboBox cbMainNT;
        private System.Windows.Forms.ComboBox cbMainOT;
        private System.Windows.Forms.ComboBox cbTopReference;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbTtFolder;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnTTFolder;
        private System.Windows.Forms.FolderBrowserDialog translationTagsFolderDialog;
        private System.Windows.Forms.TextBox tbForReviewFileName;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox tbMissingWordsFileName;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox tbErrorsFileName;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbOutputFileName;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox checkbFilesPerBook;
        private System.Windows.Forms.CheckBox checkbAppendTimestamp;
        private System.Windows.Forms.TextBox tbVersion;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckBox checkbTopRefSkip;
        private System.Windows.Forms.ToolTip ttRefBibleSkip;
        private System.Windows.Forms.CheckBox checkbNtRefSkip;
        private System.Windows.Forms.CheckBox checkbOtRefSkip;
        private System.Windows.Forms.TextBox tbRepeatedWord;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.CheckBox checkbPublicDomain;
        private System.Windows.Forms.TabPage osisGeneration;
        private System.Windows.Forms.CheckBox checkBoxUseDisambiguatedStrong;
        private System.Windows.Forms.TabPage targetBible;
        private System.Windows.Forms.TextBox tbTargetBiblesFolder;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.FolderBrowserDialog targetBiblesFolderDialog;
        private System.Windows.Forms.ComboBox cbTargetBibles;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.CheckBox checkBoxRTL;
        private System.Windows.Forms.ComboBox cbVersification;
        private System.Windows.Forms.Label label15;
    }
}