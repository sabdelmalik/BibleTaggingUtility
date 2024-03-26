
namespace BibleTaggingUtil.Editor
{
    partial class EditorPanel
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
            System.Windows.Forms.Panel panel1;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditorPanel));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            checkBsStrongHighlight = new System.Windows.Forms.CheckBox();
            picRefresh = new System.Windows.Forms.PictureBox();
            picRedo = new System.Windows.Forms.PictureBox();
            picFindTagForward = new System.Windows.Forms.PictureBox();
            picDecreaseFont = new System.Windows.Forms.PictureBox();
            picIncreaseFont = new System.Windows.Forms.PictureBox();
            picEnableEdit = new System.Windows.Forms.PictureBox();
            picUndo = new System.Windows.Forms.PictureBox();
            picPrevVerse = new System.Windows.Forms.PictureBox();
            picForward = new System.Windows.Forms.PictureBox();
            picBack = new System.Windows.Forms.PictureBox();
            picNextVerse = new System.Windows.Forms.PictureBox();
            picSave = new System.Windows.Forms.PictureBox();
            cbTagToFind = new System.Windows.Forms.ComboBox();
            tbCurrentReference = new System.Windows.Forms.TextBox();
            splitContainerMainEditor = new System.Windows.Forms.SplitContainer();
            dgvTopVersion = new TopVersionGridView();
            tbTopVersion = new System.Windows.Forms.TextBox();
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            dgvTOTHT = new TOHTHGridView();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            tbTH = new System.Windows.Forms.TextBox();
            tbTH_Next = new System.Windows.Forms.TextBox();
            tbTH_Previous = new System.Windows.Forms.TextBox();
            dgvTarget = new TargetGridView();
            tbTarget = new System.Windows.Forms.TextBox();
            toolTip1 = new System.Windows.Forms.ToolTip(components);
            panel1 = new System.Windows.Forms.Panel();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picRefresh).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picRedo).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picFindTagForward).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picDecreaseFont).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picIncreaseFont).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picEnableEdit).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picUndo).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picPrevVerse).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picForward).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picBack).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picNextVerse).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picSave).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainerMainEditor).BeginInit();
            splitContainerMainEditor.Panel1.SuspendLayout();
            splitContainerMainEditor.Panel2.SuspendLayout();
            splitContainerMainEditor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvTopVersion).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvTOTHT).BeginInit();
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvTarget).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(checkBsStrongHighlight);
            panel1.Controls.Add(picRefresh);
            panel1.Controls.Add(picRedo);
            panel1.Controls.Add(picFindTagForward);
            panel1.Controls.Add(picDecreaseFont);
            panel1.Controls.Add(picIncreaseFont);
            panel1.Controls.Add(picEnableEdit);
            panel1.Controls.Add(picUndo);
            panel1.Controls.Add(picPrevVerse);
            panel1.Controls.Add(picForward);
            panel1.Controls.Add(picBack);
            panel1.Controls.Add(picNextVerse);
            panel1.Controls.Add(picSave);
            panel1.Controls.Add(cbTagToFind);
            panel1.Controls.Add(tbCurrentReference);
            panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            panel1.Location = new System.Drawing.Point(0, 788);
            panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(1299, 72);
            panel1.TabIndex = 8;
            // 
            // checkBsStrongHighlight
            // 
            checkBsStrongHighlight.AutoSize = true;
            checkBsStrongHighlight.Location = new System.Drawing.Point(867, 26);
            checkBsStrongHighlight.Name = "checkBsStrongHighlight";
            checkBsStrongHighlight.Size = new System.Drawing.Size(170, 24);
            checkBsStrongHighlight.TabIndex = 23;
            checkBsStrongHighlight.Text = "sStrong for Highlight";
            checkBsStrongHighlight.UseVisualStyleBackColor = true;
            checkBsStrongHighlight.CheckedChanged += checkBsStrongHighlight_CheckedChanged;
            // 
            // picRefresh
            // 
            picRefresh.Image = (System.Drawing.Image)resources.GetObject("picRefresh.Image");
            picRefresh.Location = new System.Drawing.Point(1051, 18);
            picRefresh.Name = "picRefresh";
            picRefresh.Size = new System.Drawing.Size(40, 40);
            picRefresh.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            picRefresh.TabIndex = 22;
            picRefresh.TabStop = false;
            toolTip1.SetToolTip(picRefresh, "Refresh");
            picRefresh.Click += picRefresh_Click;
            // 
            // picRedo
            // 
            picRedo.Image = (System.Drawing.Image)resources.GetObject("picRedo.Image");
            picRedo.Location = new System.Drawing.Point(1171, 18);
            picRedo.Name = "picRedo";
            picRedo.Size = new System.Drawing.Size(40, 40);
            picRedo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            picRedo.TabIndex = 21;
            picRedo.TabStop = false;
            toolTip1.SetToolTip(picRedo, "Redo");
            picRedo.Click += picRedo_Click;
            // 
            // picFindTagForward
            // 
            picFindTagForward.Image = (System.Drawing.Image)resources.GetObject("picFindTagForward.Image");
            picFindTagForward.Location = new System.Drawing.Point(674, 18);
            picFindTagForward.Name = "picFindTagForward";
            picFindTagForward.Size = new System.Drawing.Size(40, 40);
            picFindTagForward.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            picFindTagForward.TabIndex = 20;
            picFindTagForward.TabStop = false;
            toolTip1.SetToolTip(picFindTagForward, "Find Tag Forward");
            picFindTagForward.Click += picFindTagForward_Click;
            // 
            // picDecreaseFont
            // 
            picDecreaseFont.Image = (System.Drawing.Image)resources.GetObject("picDecreaseFont.Image");
            picDecreaseFont.Location = new System.Drawing.Point(617, 18);
            picDecreaseFont.Name = "picDecreaseFont";
            picDecreaseFont.Size = new System.Drawing.Size(40, 40);
            picDecreaseFont.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            picDecreaseFont.TabIndex = 19;
            picDecreaseFont.TabStop = false;
            toolTip1.SetToolTip(picDecreaseFont, "Decrease Font Size");
            picDecreaseFont.Click += picDecreaseFont_Click;
            // 
            // picIncreaseFont
            // 
            picIncreaseFont.Image = (System.Drawing.Image)resources.GetObject("picIncreaseFont.Image");
            picIncreaseFont.Location = new System.Drawing.Point(571, 18);
            picIncreaseFont.Name = "picIncreaseFont";
            picIncreaseFont.Size = new System.Drawing.Size(40, 40);
            picIncreaseFont.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            picIncreaseFont.TabIndex = 18;
            picIncreaseFont.TabStop = false;
            toolTip1.SetToolTip(picIncreaseFont, "Increase Font Size");
            picIncreaseFont.Click += picIncreaseFont_Click;
            // 
            // picEnableEdit
            // 
            picEnableEdit.Image = (System.Drawing.Image)resources.GetObject("picEnableEdit.Image");
            picEnableEdit.Location = new System.Drawing.Point(506, 18);
            picEnableEdit.Name = "picEnableEdit";
            picEnableEdit.Size = new System.Drawing.Size(40, 40);
            picEnableEdit.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            picEnableEdit.TabIndex = 17;
            picEnableEdit.TabStop = false;
            toolTip1.SetToolTip(picEnableEdit, "Enable Target Editing ");
            picEnableEdit.Click += picEnableEdit_Click;
            // 
            // picUndo
            // 
            picUndo.Image = (System.Drawing.Image)resources.GetObject("picUndo.Image");
            picUndo.Location = new System.Drawing.Point(1113, 18);
            picUndo.Name = "picUndo";
            picUndo.Size = new System.Drawing.Size(40, 40);
            picUndo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            picUndo.TabIndex = 16;
            picUndo.TabStop = false;
            toolTip1.SetToolTip(picUndo, "Undo");
            picUndo.Click += picUndo_Click;
            // 
            // picPrevVerse
            // 
            picPrevVerse.Image = (System.Drawing.Image)resources.GetObject("picPrevVerse.Image");
            picPrevVerse.Location = new System.Drawing.Point(173, 18);
            picPrevVerse.Name = "picPrevVerse";
            picPrevVerse.Size = new System.Drawing.Size(40, 40);
            picPrevVerse.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            picPrevVerse.TabIndex = 15;
            picPrevVerse.TabStop = false;
            toolTip1.SetToolTip(picPrevVerse, "Previous Verse");
            picPrevVerse.Click += picPrevVerse_Click;
            // 
            // picForward
            // 
            picForward.Image = (System.Drawing.Image)resources.GetObject("picForward.Image");
            picForward.Location = new System.Drawing.Point(49, 23);
            picForward.Name = "picForward";
            picForward.Size = new System.Drawing.Size(30, 30);
            picForward.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            picForward.TabIndex = 14;
            picForward.TabStop = false;
            toolTip1.SetToolTip(picForward, "Forward if Back was used");
            picForward.Click += picForward_Click;
            // 
            // picBack
            // 
            picBack.Image = (System.Drawing.Image)resources.GetObject("picBack.Image");
            picBack.Location = new System.Drawing.Point(3, 23);
            picBack.Name = "picBack";
            picBack.Size = new System.Drawing.Size(30, 30);
            picBack.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            picBack.TabIndex = 14;
            picBack.TabStop = false;
            toolTip1.SetToolTip(picBack, "Back to last viewed verse");
            picBack.Click += picBack_Click;
            // 
            // picNextVerse
            // 
            picNextVerse.Image = (System.Drawing.Image)resources.GetObject("picNextVerse.Image");
            picNextVerse.Location = new System.Drawing.Point(429, 18);
            picNextVerse.Name = "picNextVerse";
            picNextVerse.Size = new System.Drawing.Size(40, 40);
            picNextVerse.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            picNextVerse.TabIndex = 14;
            picNextVerse.TabStop = false;
            toolTip1.SetToolTip(picNextVerse, "Next Verse");
            picNextVerse.Click += picNextVerse_Click;
            // 
            // picSave
            // 
            picSave.Image = (System.Drawing.Image)resources.GetObject("picSave.Image");
            picSave.Location = new System.Drawing.Point(97, 18);
            picSave.Name = "picSave";
            picSave.Size = new System.Drawing.Size(40, 40);
            picSave.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            picSave.TabIndex = 13;
            picSave.TabStop = false;
            toolTip1.SetToolTip(picSave, "Save");
            picSave.Click += picSave_Click;
            // 
            // cbTagToFind
            // 
            cbTagToFind.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            cbTagToFind.FormattingEnabled = true;
            cbTagToFind.Items.AddRange(new object[] { "???", "<blank>" });
            cbTagToFind.Location = new System.Drawing.Point(720, 24);
            cbTagToFind.Name = "cbTagToFind";
            cbTagToFind.Size = new System.Drawing.Size(127, 28);
            cbTagToFind.TabIndex = 12;
            cbTagToFind.Text = "???";
            cbTagToFind.SelectedIndexChanged += cbTagToFind_SelectedIndexChanged;
            // 
            // tbCurrentReference
            // 
            tbCurrentReference.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            tbCurrentReference.ForeColor = System.Drawing.Color.DarkRed;
            tbCurrentReference.Location = new System.Drawing.Point(219, 23);
            tbCurrentReference.Name = "tbCurrentReference";
            tbCurrentReference.ReadOnly = true;
            tbCurrentReference.Size = new System.Drawing.Size(204, 30);
            tbCurrentReference.TabIndex = 1;
            tbCurrentReference.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            tbCurrentReference.TextChanged += tbCurrentReference_TextChanged;
            // 
            // splitContainerMainEditor
            // 
            splitContainerMainEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainerMainEditor.Location = new System.Drawing.Point(0, 0);
            splitContainerMainEditor.Name = "splitContainerMainEditor";
            splitContainerMainEditor.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerMainEditor.Panel1
            // 
            splitContainerMainEditor.Panel1.Controls.Add(dgvTopVersion);
            splitContainerMainEditor.Panel1.Controls.Add(tbTopVersion);
            // 
            // splitContainerMainEditor.Panel2
            // 
            splitContainerMainEditor.Panel2.Controls.Add(splitContainer1);
            splitContainerMainEditor.Size = new System.Drawing.Size(1299, 788);
            splitContainerMainEditor.SplitterDistance = 187;
            splitContainerMainEditor.TabIndex = 4;
            // 
            // dgvTopVersion
            // 
            dgvTopVersion.AllowDrop = true;
            dgvTopVersion.AllowUserToAddRows = false;
            dgvTopVersion.AllowUserToDeleteRows = false;
            dgvTopVersion.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            dgvTopVersion.BackgroundColor = System.Drawing.SystemColors.ControlLight;
            dgvTopVersion.Bible = null;
            dgvTopVersion.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvTopVersion.ColumnHeadersVisible = false;
            dgvTopVersion.Cursor = System.Windows.Forms.Cursors.Hand;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            dgvTopVersion.DefaultCellStyle = dataGridViewCellStyle4;
            dgvTopVersion.Dock = System.Windows.Forms.DockStyle.Fill;
            dgvTopVersion.GridColor = System.Drawing.SystemColors.ControlText;
            dgvTopVersion.Location = new System.Drawing.Point(0, 31);
            dgvTopVersion.Name = "dgvTopVersion";
            dgvTopVersion.ReadOnly = true;
            dgvTopVersion.RowHeadersVisible = false;
            dgvTopVersion.RowHeadersWidth = 51;
            dgvTopVersion.RowTemplate.Height = 29;
            dgvTopVersion.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            dgvTopVersion.ShowCellToolTips = false;
            dgvTopVersion.Size = new System.Drawing.Size(1299, 156);
            dgvTopVersion.TabIndex = 4;
            // 
            // tbTopVersion
            // 
            tbTopVersion.BackColor = System.Drawing.SystemColors.MenuHighlight;
            tbTopVersion.Dock = System.Windows.Forms.DockStyle.Top;
            tbTopVersion.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            tbTopVersion.ForeColor = System.Drawing.SystemColors.HighlightText;
            tbTopVersion.Location = new System.Drawing.Point(0, 0);
            tbTopVersion.Name = "tbTopVersion";
            tbTopVersion.ReadOnly = true;
            tbTopVersion.Size = new System.Drawing.Size(1299, 31);
            tbTopVersion.TabIndex = 3;
            tbTopVersion.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer1.Location = new System.Drawing.Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(dgvTOTHT);
            splitContainer1.Panel1.Controls.Add(tableLayoutPanel1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(dgvTarget);
            splitContainer1.Panel2.Controls.Add(tbTarget);
            splitContainer1.Size = new System.Drawing.Size(1299, 597);
            splitContainer1.SplitterDistance = 382;
            splitContainer1.TabIndex = 3;
            // 
            // dgvTOTHT
            // 
            dgvTOTHT.AllowDrop = true;
            dgvTOTHT.AllowUserToAddRows = false;
            dgvTOTHT.AllowUserToDeleteRows = false;
            dgvTOTHT.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            dgvTOTHT.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            dgvTOTHT.BackgroundColor = System.Drawing.SystemColors.ControlLight;
            dgvTOTHT.BibleNT = null;
            dgvTOTHT.BibleOT = null;
            dgvTOTHT.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvTOTHT.ColumnHeadersVisible = false;
            dgvTOTHT.Cursor = System.Windows.Forms.Cursors.Hand;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            dgvTOTHT.DefaultCellStyle = dataGridViewCellStyle5;
            dgvTOTHT.Dock = System.Windows.Forms.DockStyle.Fill;
            dgvTOTHT.GridColor = System.Drawing.SystemColors.ControlText;
            dgvTOTHT.Location = new System.Drawing.Point(0, 60);
            dgvTOTHT.Name = "dgvTOTHT";
            dgvTOTHT.ReadOnly = true;
            dgvTOTHT.RowHeadersVisible = false;
            dgvTOTHT.RowHeadersWidth = 51;
            dgvTOTHT.RowTemplate.Height = 29;
            dgvTOTHT.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            dgvTOTHT.Size = new System.Drawing.Size(1299, 322);
            dgvTOTHT.TabIndex = 4;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            tableLayoutPanel1.Controls.Add(tbTH, 1, 0);
            tableLayoutPanel1.Controls.Add(tbTH_Next, 2, 0);
            tableLayoutPanel1.Controls.Add(tbTH_Previous, 0, 0);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new System.Drawing.Size(1299, 60);
            tableLayoutPanel1.TabIndex = 6;
            // 
            // tbTH
            // 
            tbTH.BackColor = System.Drawing.SystemColors.MenuHighlight;
            tbTH.Dock = System.Windows.Forms.DockStyle.Fill;
            tbTH.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            tbTH.ForeColor = System.Drawing.SystemColors.HighlightText;
            tbTH.Location = new System.Drawing.Point(392, 3);
            tbTH.Name = "tbTH";
            tbTH.ReadOnly = true;
            tbTH.Size = new System.Drawing.Size(513, 31);
            tbTH.TabIndex = 5;
            tbTH.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbTH_Next
            // 
            tbTH_Next.BackColor = System.Drawing.SystemColors.MenuHighlight;
            tbTH_Next.Dock = System.Windows.Forms.DockStyle.Fill;
            tbTH_Next.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            tbTH_Next.ForeColor = System.Drawing.SystemColors.HighlightText;
            tbTH_Next.Location = new System.Drawing.Point(911, 3);
            tbTH_Next.Name = "tbTH_Next";
            tbTH_Next.ReadOnly = true;
            tbTH_Next.Size = new System.Drawing.Size(385, 31);
            tbTH_Next.TabIndex = 5;
            tbTH_Next.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbTH_Previous
            // 
            tbTH_Previous.BackColor = System.Drawing.SystemColors.MenuHighlight;
            tbTH_Previous.Dock = System.Windows.Forms.DockStyle.Fill;
            tbTH_Previous.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            tbTH_Previous.ForeColor = System.Drawing.SystemColors.HighlightText;
            tbTH_Previous.Location = new System.Drawing.Point(3, 3);
            tbTH_Previous.Name = "tbTH_Previous";
            tbTH_Previous.ReadOnly = true;
            tbTH_Previous.Size = new System.Drawing.Size(383, 31);
            tbTH_Previous.TabIndex = 5;
            tbTH_Previous.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // dgvTarget
            // 
            dgvTarget.AllowDrop = true;
            dgvTarget.AllowUserToAddRows = false;
            dgvTarget.AllowUserToDeleteRows = false;
            dgvTarget.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            dgvTarget.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            dgvTarget.BackgroundColor = System.Drawing.SystemColors.ControlLight;
            dgvTarget.Bible = null;
            dgvTarget.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvTarget.ColumnHeadersVisible = false;
            dgvTarget.CurrentVerse = null;
            dgvTarget.CurrentVerseReferece = null;
            dgvTarget.Cursor = System.Windows.Forms.Cursors.Hand;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            dgvTarget.DefaultCellStyle = dataGridViewCellStyle6;
            dgvTarget.Dock = System.Windows.Forms.DockStyle.Fill;
            dgvTarget.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnF2;
            dgvTarget.GridColor = System.Drawing.SystemColors.ControlText;
            dgvTarget.Location = new System.Drawing.Point(0, 31);
            dgvTarget.Name = "dgvTarget";
            dgvTarget.RowHeadersVisible = false;
            dgvTarget.RowHeadersWidth = 51;
            dgvTarget.RowTemplate.Height = 29;
            dgvTarget.SearchTag = null;
            dgvTarget.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            dgvTarget.ShowCellToolTips = false;
            dgvTarget.Size = new System.Drawing.Size(1299, 180);
            dgvTarget.TabIndex = 2;
            // 
            // tbTarget
            // 
            tbTarget.BackColor = System.Drawing.SystemColors.MenuHighlight;
            tbTarget.Dock = System.Windows.Forms.DockStyle.Top;
            tbTarget.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            tbTarget.ForeColor = System.Drawing.SystemColors.HighlightText;
            tbTarget.Location = new System.Drawing.Point(0, 0);
            tbTarget.Name = "tbTarget";
            tbTarget.ReadOnly = true;
            tbTarget.Size = new System.Drawing.Size(1299, 31);
            tbTarget.TabIndex = 6;
            tbTarget.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // EditorPanel
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1299, 860);
            Controls.Add(splitContainerMainEditor);
            Controls.Add(panel1);
            Name = "EditorPanel";
            Text = "EdirorPanel";
            Load += EditorPanel_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picRefresh).EndInit();
            ((System.ComponentModel.ISupportInitialize)picRedo).EndInit();
            ((System.ComponentModel.ISupportInitialize)picFindTagForward).EndInit();
            ((System.ComponentModel.ISupportInitialize)picDecreaseFont).EndInit();
            ((System.ComponentModel.ISupportInitialize)picIncreaseFont).EndInit();
            ((System.ComponentModel.ISupportInitialize)picEnableEdit).EndInit();
            ((System.ComponentModel.ISupportInitialize)picUndo).EndInit();
            ((System.ComponentModel.ISupportInitialize)picPrevVerse).EndInit();
            ((System.ComponentModel.ISupportInitialize)picForward).EndInit();
            ((System.ComponentModel.ISupportInitialize)picBack).EndInit();
            ((System.ComponentModel.ISupportInitialize)picNextVerse).EndInit();
            ((System.ComponentModel.ISupportInitialize)picSave).EndInit();
            splitContainerMainEditor.Panel1.ResumeLayout(false);
            splitContainerMainEditor.Panel1.PerformLayout();
            splitContainerMainEditor.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerMainEditor).EndInit();
            splitContainerMainEditor.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvTopVersion).EndInit();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvTOTHT).EndInit();
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvTarget).EndInit();
            ResumeLayout(false);
        }


        #endregion
        private System.Windows.Forms.TextBox tbCurrentReference;
        private System.Windows.Forms.PictureBox picPrevVerse;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.SplitContainer splitContainerMainEditor;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private TargetGridView dgvTarget;
        private TopVersionGridView dgvTopVersion;
        private TOHTHGridView dgvTOTHT;
        private System.Windows.Forms.ComboBox cbTagToFind;
        private System.Windows.Forms.PictureBox picNextVerse;
        private System.Windows.Forms.PictureBox picSave;
        private System.Windows.Forms.PictureBox picUndo;
        private System.Windows.Forms.PictureBox picEnableEdit;
        private System.Windows.Forms.PictureBox picDecreaseFont;
        private System.Windows.Forms.PictureBox picIncreaseFont;
        private System.Windows.Forms.PictureBox picFindTagForward;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.PictureBox picRedo;
        private System.Windows.Forms.TextBox tbTopVersion;
        private System.Windows.Forms.TextBox tbTH;
        private System.Windows.Forms.TextBox tbTarget;
        private System.Windows.Forms.PictureBox picRefresh;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox tbTH_Next;
        private System.Windows.Forms.TextBox tbTH_Previous;
        private System.Windows.Forms.CheckBox checkBsStrongHighlight;
        private System.Windows.Forms.PictureBox picBack;
        private System.Windows.Forms.PictureBox picForward;
    }
}