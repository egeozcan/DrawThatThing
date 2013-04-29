namespace DrawThatThing
{
	partial class DrawThatThing
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DrawThatThing));
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
			this.txtMousePositionY = new System.Windows.Forms.TextBox();
			this.txtMousePositionX = new System.Windows.Forms.TextBox();
			this.lblX = new System.Windows.Forms.Label();
			this.btnPlay = new System.Windows.Forms.Button();
			this.workerClickAround = new System.ComponentModel.BackgroundWorker();
			this.btnLoadImage = new System.Windows.Forms.Button();
			this.dlgImportImage = new System.Windows.Forms.OpenFileDialog();
			this.pctPreview = new System.Windows.Forms.PictureBox();
			this.panelPreview = new System.Windows.Forms.Panel();
			this.panelPreviewInner = new System.Windows.Forms.Panel();
			this.panelPlay = new System.Windows.Forms.Panel();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.lblStopMouseShortcut = new System.Windows.Forms.Label();
			this.lblStartPositionShortcut = new System.Windows.Forms.Label();
			this.lblpickColorShortcut = new System.Windows.Forms.Label();
			this.lblStopMouseTitle = new System.Windows.Forms.Label();
			this.lblStartPositionTitle = new System.Windows.Forms.Label();
			this.lblPickColorTitle = new System.Windows.Forms.Label();
			this.intPreviewWidth = new System.Windows.Forms.NumericUpDown();
			this.intPreviewHeight = new System.Windows.Forms.NumericUpDown();
			this.label10 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.lblColorOptions = new System.Windows.Forms.Label();
			this.dataGridColors = new System.Windows.Forms.DataGridView();
			this.PosX = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.PosY = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.RGB = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.IsBackground = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.btnExport = new System.Windows.Forms.Button();
			this.btnImport = new System.Windows.Forms.Button();
			this.dlgExportPixels = new System.Windows.Forms.SaveFileDialog();
			this.dlgImportPixels = new System.Windows.Forms.OpenFileDialog();
			this.workerCalculate = new System.ComponentModel.BackgroundWorker();
			this.cmbParser = new System.Windows.Forms.ComboBox();
			this.label7 = new System.Windows.Forms.Label();
			this.dataGridSettings = new System.Windows.Forms.DataGridView();
			this.settingName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.label8 = new System.Windows.Forms.Label();
			this.btnReparse = new System.Windows.Forms.Button();
			this.btnClearUnusedColors = new System.Windows.Forms.Button();
			this.label9 = new System.Windows.Forms.Label();
			this.cmbBrushSelector = new System.Windows.Forms.ComboBox();
			this.panelBottom = new System.Windows.Forms.Panel();
			this.panelMiddleRight = new System.Windows.Forms.Panel();
			this.panel6 = new System.Windows.Forms.Panel();
			this.panelTop = new System.Windows.Forms.Panel();
			this.panelMiddleLeft = new System.Windows.Forms.Panel();
			this.panelMiddle = new System.Windows.Forms.Panel();
			this.panelMiddleMiddle = new System.Windows.Forms.Panel();
			this.panelDebug = new System.Windows.Forms.Panel();
			this.txtDebugRoutes = new System.Windows.Forms.TextBox();
			this.btnDebugAddPoint = new System.Windows.Forms.Button();
			this.btnDebugPlay = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.pctPreview)).BeginInit();
			this.panelPreview.SuspendLayout();
			this.panelPreviewInner.SuspendLayout();
			this.panelPlay.SuspendLayout();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.intPreviewWidth)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.intPreviewHeight)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dataGridColors)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dataGridSettings)).BeginInit();
			this.panelBottom.SuspendLayout();
			this.panelMiddleRight.SuspendLayout();
			this.panel6.SuspendLayout();
			this.panelTop.SuspendLayout();
			this.panelMiddleLeft.SuspendLayout();
			this.panelMiddle.SuspendLayout();
			this.panelDebug.SuspendLayout();
			this.SuspendLayout();
			// 
			// txtMousePositionY
			// 
			this.txtMousePositionY.Location = new System.Drawing.Point(135, 20);
			this.txtMousePositionY.Name = "txtMousePositionY";
			this.txtMousePositionY.Size = new System.Drawing.Size(79, 20);
			this.txtMousePositionY.TabIndex = 0;
			this.txtMousePositionY.Text = "0";
			// 
			// txtMousePositionX
			// 
			this.txtMousePositionX.Location = new System.Drawing.Point(46, 20);
			this.txtMousePositionX.Name = "txtMousePositionX";
			this.txtMousePositionX.Size = new System.Drawing.Size(79, 20);
			this.txtMousePositionX.TabIndex = 1;
			this.txtMousePositionX.Text = "0";
			// 
			// lblX
			// 
			this.lblX.AutoSize = true;
			this.lblX.Location = new System.Drawing.Point(12, 23);
			this.lblX.Name = "lblX";
			this.lblX.Size = new System.Drawing.Size(24, 13);
			this.lblX.TabIndex = 2;
			this.lblX.Text = "X,Y";
			// 
			// btnPlay
			// 
			this.btnPlay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnPlay.BackColor = System.Drawing.Color.Green;
			this.btnPlay.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnPlay.ForeColor = System.Drawing.Color.White;
			this.btnPlay.Location = new System.Drawing.Point(560, 13);
			this.btnPlay.Name = "btnPlay";
			this.btnPlay.Size = new System.Drawing.Size(107, 42);
			this.btnPlay.TabIndex = 3;
			this.btnPlay.Text = "PLAY >>";
			this.btnPlay.UseVisualStyleBackColor = false;
			this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
			// 
			// workerClickAround
			// 
			this.workerClickAround.WorkerSupportsCancellation = true;
			this.workerClickAround.DoWork += new System.ComponentModel.DoWorkEventHandler(this.workerClickAround_DoWork);
			// 
			// btnLoadImage
			// 
			this.btnLoadImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnLoadImage.BackColor = System.Drawing.Color.Yellow;
			this.btnLoadImage.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnLoadImage.ForeColor = System.Drawing.Color.Black;
			this.btnLoadImage.Location = new System.Drawing.Point(571, 3);
			this.btnLoadImage.Name = "btnLoadImage";
			this.btnLoadImage.Size = new System.Drawing.Size(101, 37);
			this.btnLoadImage.TabIndex = 3;
			this.btnLoadImage.Text = "Parse Image";
			this.btnLoadImage.UseVisualStyleBackColor = false;
			this.btnLoadImage.Click += new System.EventHandler(this.btnLoadImage_Click);
			// 
			// dlgImportImage
			// 
			this.dlgImportImage.Filter = "JPG File|*.jpg|PNG Files|*.png";
			// 
			// pctPreview
			// 
			this.pctPreview.BackColor = System.Drawing.Color.Transparent;
			this.pctPreview.Location = new System.Drawing.Point(0, 0);
			this.pctPreview.Name = "pctPreview";
			this.pctPreview.Size = new System.Drawing.Size(50, 50);
			this.pctPreview.TabIndex = 22;
			this.pctPreview.TabStop = false;
			// 
			// panelPreview
			// 
			this.panelPreview.BackColor = System.Drawing.Color.White;
			this.panelPreview.Controls.Add(this.panelPreviewInner);
			this.panelPreview.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelPreview.Location = new System.Drawing.Point(0, 0);
			this.panelPreview.Name = "panelPreview";
			this.panelPreview.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
			this.panelPreview.Size = new System.Drawing.Size(672, 239);
			this.panelPreview.TabIndex = 26;
			// 
			// panelPreviewInner
			// 
			this.panelPreviewInner.AutoScroll = true;
			this.panelPreviewInner.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panelPreviewInner.BackgroundImage")));
			this.panelPreviewInner.Controls.Add(this.pctPreview);
			this.panelPreviewInner.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelPreviewInner.Location = new System.Drawing.Point(0, 5);
			this.panelPreviewInner.Name = "panelPreviewInner";
			this.panelPreviewInner.Size = new System.Drawing.Size(672, 234);
			this.panelPreviewInner.TabIndex = 23;
			// 
			// panelPlay
			// 
			this.panelPlay.Controls.Add(this.groupBox1);
			this.panelPlay.Controls.Add(this.btnPlay);
			this.panelPlay.Controls.Add(this.lblStopMouseShortcut);
			this.panelPlay.Controls.Add(this.lblStartPositionShortcut);
			this.panelPlay.Controls.Add(this.lblpickColorShortcut);
			this.panelPlay.Controls.Add(this.lblStopMouseTitle);
			this.panelPlay.Controls.Add(this.lblStartPositionTitle);
			this.panelPlay.Controls.Add(this.lblPickColorTitle);
			this.panelPlay.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panelPlay.Location = new System.Drawing.Point(0, 239);
			this.panelPlay.Name = "panelPlay";
			this.panelPlay.Size = new System.Drawing.Size(672, 60);
			this.panelPlay.TabIndex = 23;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.txtMousePositionY);
			this.groupBox1.Controls.Add(this.txtMousePositionX);
			this.groupBox1.Controls.Add(this.lblX);
			this.groupBox1.Location = new System.Drawing.Point(7, 2);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(225, 53);
			this.groupBox1.TabIndex = 29;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Mouse Start Position";
			// 
			// lblStopMouseShortcut
			// 
			this.lblStopMouseShortcut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblStopMouseShortcut.AutoSize = true;
			this.lblStopMouseShortcut.Location = new System.Drawing.Point(476, 15);
			this.lblStopMouseShortcut.Name = "lblStopMouseShortcut";
			this.lblStopMouseShortcut.Size = new System.Drawing.Size(71, 13);
			this.lblStopMouseShortcut.TabIndex = 36;
			this.lblStopMouseShortcut.Text = "Shift + Alt + C";
			this.lblStopMouseShortcut.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblStartPositionShortcut
			// 
			this.lblStartPositionShortcut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblStartPositionShortcut.AutoSize = true;
			this.lblStartPositionShortcut.Location = new System.Drawing.Point(476, 29);
			this.lblStartPositionShortcut.Name = "lblStartPositionShortcut";
			this.lblStartPositionShortcut.Size = new System.Drawing.Size(71, 13);
			this.lblStartPositionShortcut.TabIndex = 37;
			this.lblStartPositionShortcut.Text = "Shift + Alt + S";
			this.lblStartPositionShortcut.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblpickColorShortcut
			// 
			this.lblpickColorShortcut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblpickColorShortcut.AutoSize = true;
			this.lblpickColorShortcut.Location = new System.Drawing.Point(476, 44);
			this.lblpickColorShortcut.Name = "lblpickColorShortcut";
			this.lblpickColorShortcut.Size = new System.Drawing.Size(71, 13);
			this.lblpickColorShortcut.TabIndex = 38;
			this.lblpickColorShortcut.Text = "Shift + Alt + A";
			this.lblpickColorShortcut.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblStopMouseTitle
			// 
			this.lblStopMouseTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblStopMouseTitle.AutoSize = true;
			this.lblStopMouseTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblStopMouseTitle.Location = new System.Drawing.Point(402, 13);
			this.lblStopMouseTitle.Name = "lblStopMouseTitle";
			this.lblStopMouseTitle.Size = new System.Drawing.Size(73, 17);
			this.lblStopMouseTitle.TabIndex = 39;
			this.lblStopMouseTitle.Text = "Stop mouse :";
			this.lblStopMouseTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lblStopMouseTitle.UseCompatibleTextRendering = true;
			// 
			// lblStartPositionTitle
			// 
			this.lblStartPositionTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblStartPositionTitle.AutoSize = true;
			this.lblStartPositionTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblStartPositionTitle.Location = new System.Drawing.Point(313, 29);
			this.lblStartPositionTitle.Name = "lblStartPositionTitle";
			this.lblStartPositionTitle.Size = new System.Drawing.Size(165, 13);
			this.lblStartPositionTitle.TabIndex = 40;
			this.lblStartPositionTitle.Text = "Set start position to cursor :";
			this.lblStartPositionTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblPickColorTitle
			// 
			this.lblPickColorTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblPickColorTitle.AutoSize = true;
			this.lblPickColorTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblPickColorTitle.Location = new System.Drawing.Point(407, 44);
			this.lblPickColorTitle.Name = "lblPickColorTitle";
			this.lblPickColorTitle.Size = new System.Drawing.Size(72, 13);
			this.lblPickColorTitle.TabIndex = 41;
			this.lblPickColorTitle.Text = "Pick color :";
			this.lblPickColorTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// intPreviewWidth
			// 
			this.intPreviewWidth.Enabled = false;
			this.intPreviewWidth.Location = new System.Drawing.Point(295, 4);
			this.intPreviewWidth.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
			this.intPreviewWidth.Name = "intPreviewWidth";
			this.intPreviewWidth.Size = new System.Drawing.Size(52, 20);
			this.intPreviewWidth.TabIndex = 27;
			// 
			// intPreviewHeight
			// 
			this.intPreviewHeight.Enabled = false;
			this.intPreviewHeight.Location = new System.Drawing.Point(295, 26);
			this.intPreviewHeight.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
			this.intPreviewHeight.Name = "intPreviewHeight";
			this.intPreviewHeight.Size = new System.Drawing.Size(52, 20);
			this.intPreviewHeight.TabIndex = 27;
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(256, 8);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(35, 13);
			this.label10.TabIndex = 28;
			this.label10.Text = "Width";
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(253, 30);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(38, 13);
			this.label11.TabIndex = 28;
			this.label11.Text = "Height";
			// 
			// lblColorOptions
			// 
			this.lblColorOptions.AutoSize = true;
			this.lblColorOptions.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblColorOptions.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblColorOptions.Location = new System.Drawing.Point(3, 0);
			this.lblColorOptions.Name = "lblColorOptions";
			this.lblColorOptions.Size = new System.Drawing.Size(80, 13);
			this.lblColorOptions.TabIndex = 31;
			this.lblColorOptions.Text = "Color Palette";
			// 
			// dataGridColors
			// 
			this.dataGridColors.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.dataGridColors.BackgroundColor = System.Drawing.Color.White;
			this.dataGridColors.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
			this.dataGridColors.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridColors.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.PosX,
            this.PosY,
            this.RGB,
            this.IsBackground});
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.25F);
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.dataGridColors.DefaultCellStyle = dataGridViewCellStyle1;
			this.dataGridColors.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGridColors.EnableHeadersVisualStyles = false;
			this.dataGridColors.Location = new System.Drawing.Point(3, 13);
			this.dataGridColors.Name = "dataGridColors";
			this.dataGridColors.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dataGridColors.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
			this.dataGridColors.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
			this.dataGridColors.Size = new System.Drawing.Size(382, 107);
			this.dataGridColors.TabIndex = 34;
			this.dataGridColors.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridColors_CellEndEdit);
			this.dataGridColors.CellLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridColors_CellLeave);
			this.dataGridColors.RowLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridColors_RowLeave);
			// 
			// PosX
			// 
			this.PosX.HeaderText = "X";
			this.PosX.Name = "PosX";
			// 
			// PosY
			// 
			this.PosY.HeaderText = "Y";
			this.PosY.Name = "PosY";
			// 
			// RGB
			// 
			this.RGB.HeaderText = "RGB";
			this.RGB.Name = "RGB";
			// 
			// IsBackground
			// 
			this.IsBackground.HeaderText = "BG Color";
			this.IsBackground.Name = "IsBackground";
			// 
			// btnExport
			// 
			this.btnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnExport.BackColor = System.Drawing.Color.White;
			this.btnExport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnExport.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnExport.Location = new System.Drawing.Point(211, 13);
			this.btnExport.Name = "btnExport";
			this.btnExport.Size = new System.Drawing.Size(82, 27);
			this.btnExport.TabIndex = 3;
			this.btnExport.Text = "Export";
			this.btnExport.UseVisualStyleBackColor = false;
			this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
			// 
			// btnImport
			// 
			this.btnImport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnImport.BackColor = System.Drawing.Color.White;
			this.btnImport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnImport.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnImport.Location = new System.Drawing.Point(300, 13);
			this.btnImport.Name = "btnImport";
			this.btnImport.Size = new System.Drawing.Size(82, 27);
			this.btnImport.TabIndex = 3;
			this.btnImport.Text = "Import";
			this.btnImport.UseVisualStyleBackColor = false;
			this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
			// 
			// dlgExportPixels
			// 
			this.dlgExportPixels.DefaultExt = "csv";
			// 
			// dlgImportPixels
			// 
			this.dlgImportPixels.DefaultExt = "csv";
			this.dlgImportPixels.FileName = "openFileDialog1";
			this.dlgImportPixels.Filter = "CSV Files|*.csv";
			// 
			// workerCalculate
			// 
			this.workerCalculate.DoWork += new System.ComponentModel.DoWorkEventHandler(this.workerCalculate_DoWork);
			this.workerCalculate.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.workerCalculate_RunWorkerCompleted);
			// 
			// cmbParser
			// 
			this.cmbParser.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbParser.FormattingEnabled = true;
			this.cmbParser.Location = new System.Drawing.Point(89, 3);
			this.cmbParser.Name = "cmbParser";
			this.cmbParser.Size = new System.Drawing.Size(148, 21);
			this.cmbParser.TabIndex = 42;
			this.cmbParser.SelectedIndexChanged += new System.EventHandler(this.cmbParser_SelectedIndexChanged);
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(4, 7);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(37, 13);
			this.label7.TabIndex = 43;
			this.label7.Text = "Parser";
			// 
			// dataGridSettings
			// 
			this.dataGridSettings.AllowUserToAddRows = false;
			this.dataGridSettings.AllowUserToDeleteRows = false;
			this.dataGridSettings.AllowUserToResizeRows = false;
			this.dataGridSettings.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.dataGridSettings.BackgroundColor = System.Drawing.Color.White;
			this.dataGridSettings.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
			this.dataGridSettings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridSettings.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.settingName,
            this.Value});
			dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.25F);
			dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.dataGridSettings.DefaultCellStyle = dataGridViewCellStyle3;
			this.dataGridSettings.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGridSettings.EnableHeadersVisualStyles = false;
			this.dataGridSettings.Location = new System.Drawing.Point(0, 13);
			this.dataGridSettings.Name = "dataGridSettings";
			this.dataGridSettings.RowHeadersVisible = false;
			this.dataGridSettings.Size = new System.Drawing.Size(229, 147);
			this.dataGridSettings.TabIndex = 44;
			this.dataGridSettings.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridSettings_CellEndEdit);
			// 
			// settingName
			// 
			this.settingName.HeaderText = "Name";
			this.settingName.Name = "settingName";
			this.settingName.ReadOnly = true;
			// 
			// Value
			// 
			this.Value.HeaderText = "Value";
			this.Value.Name = "Value";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Dock = System.Windows.Forms.DockStyle.Top;
			this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label8.Location = new System.Drawing.Point(0, 0);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(93, 13);
			this.label8.TabIndex = 45;
			this.label8.Text = "Parser Settings";
			// 
			// btnReparse
			// 
			this.btnReparse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnReparse.BackColor = System.Drawing.Color.RoyalBlue;
			this.btnReparse.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnReparse.ForeColor = System.Drawing.Color.White;
			this.btnReparse.Location = new System.Drawing.Point(464, 3);
			this.btnReparse.Name = "btnReparse";
			this.btnReparse.Size = new System.Drawing.Size(101, 37);
			this.btnReparse.TabIndex = 3;
			this.btnReparse.Text = "Refresh";
			this.btnReparse.UseVisualStyleBackColor = false;
			this.btnReparse.Visible = false;
			this.btnReparse.Click += new System.EventHandler(this.btnReparse_Click);
			// 
			// btnClearUnusedColors
			// 
			this.btnClearUnusedColors.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnClearUnusedColors.BackColor = System.Drawing.Color.Red;
			this.btnClearUnusedColors.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnClearUnusedColors.ForeColor = System.Drawing.Color.White;
			this.btnClearUnusedColors.Location = new System.Drawing.Point(357, 3);
			this.btnClearUnusedColors.Name = "btnClearUnusedColors";
			this.btnClearUnusedColors.Size = new System.Drawing.Size(101, 37);
			this.btnClearUnusedColors.TabIndex = 3;
			this.btnClearUnusedColors.Text = "Clear Unused Colors";
			this.btnClearUnusedColors.UseVisualStyleBackColor = false;
			this.btnClearUnusedColors.Visible = false;
			this.btnClearUnusedColors.Click += new System.EventHandler(this.btnClearUnusedColors_Click);
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(4, 30);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(76, 13);
			this.label9.TabIndex = 47;
			this.label9.Text = "Brush Selector";
			// 
			// cmbBrushSelector
			// 
			this.cmbBrushSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbBrushSelector.FormattingEnabled = true;
			this.cmbBrushSelector.Location = new System.Drawing.Point(89, 26);
			this.cmbBrushSelector.Name = "cmbBrushSelector";
			this.cmbBrushSelector.Size = new System.Drawing.Size(148, 21);
			this.cmbBrushSelector.TabIndex = 46;
			// 
			// panelBottom
			// 
			this.panelBottom.Controls.Add(this.panelPreview);
			this.panelBottom.Controls.Add(this.panelPlay);
			this.panelBottom.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelBottom.Location = new System.Drawing.Point(10, 351);
			this.panelBottom.Name = "panelBottom";
			this.panelBottom.Size = new System.Drawing.Size(672, 299);
			this.panelBottom.TabIndex = 48;
			// 
			// panelMiddleRight
			// 
			this.panelMiddleRight.Controls.Add(this.dataGridColors);
			this.panelMiddleRight.Controls.Add(this.panel6);
			this.panelMiddleRight.Controls.Add(this.lblColorOptions);
			this.panelMiddleRight.Dock = System.Windows.Forms.DockStyle.Right;
			this.panelMiddleRight.Location = new System.Drawing.Point(287, 0);
			this.panelMiddleRight.Name = "panelMiddleRight";
			this.panelMiddleRight.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.panelMiddleRight.Size = new System.Drawing.Size(385, 160);
			this.panelMiddleRight.TabIndex = 49;
			// 
			// panel6
			// 
			this.panel6.Controls.Add(this.btnExport);
			this.panel6.Controls.Add(this.btnImport);
			this.panel6.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel6.Location = new System.Drawing.Point(3, 120);
			this.panel6.Name = "panel6";
			this.panel6.Size = new System.Drawing.Size(382, 40);
			this.panel6.TabIndex = 35;
			// 
			// panelTop
			// 
			this.panelTop.Controls.Add(this.btnLoadImage);
			this.panelTop.Controls.Add(this.btnReparse);
			this.panelTop.Controls.Add(this.btnClearUnusedColors);
			this.panelTop.Controls.Add(this.label9);
			this.panelTop.Controls.Add(this.intPreviewWidth);
			this.panelTop.Controls.Add(this.cmbBrushSelector);
			this.panelTop.Controls.Add(this.intPreviewHeight);
			this.panelTop.Controls.Add(this.label7);
			this.panelTop.Controls.Add(this.label10);
			this.panelTop.Controls.Add(this.cmbParser);
			this.panelTop.Controls.Add(this.label11);
			this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
			this.panelTop.Location = new System.Drawing.Point(10, 10);
			this.panelTop.Name = "panelTop";
			this.panelTop.Size = new System.Drawing.Size(672, 58);
			this.panelTop.TabIndex = 50;
			// 
			// panelMiddleLeft
			// 
			this.panelMiddleLeft.Controls.Add(this.dataGridSettings);
			this.panelMiddleLeft.Controls.Add(this.label8);
			this.panelMiddleLeft.Dock = System.Windows.Forms.DockStyle.Left;
			this.panelMiddleLeft.Location = new System.Drawing.Point(0, 0);
			this.panelMiddleLeft.Name = "panelMiddleLeft";
			this.panelMiddleLeft.Padding = new System.Windows.Forms.Padding(0, 0, 3, 0);
			this.panelMiddleLeft.Size = new System.Drawing.Size(232, 160);
			this.panelMiddleLeft.TabIndex = 51;
			// 
			// panelMiddle
			// 
			this.panelMiddle.BackColor = System.Drawing.Color.White;
			this.panelMiddle.Controls.Add(this.panelMiddleMiddle);
			this.panelMiddle.Controls.Add(this.panelMiddleLeft);
			this.panelMiddle.Controls.Add(this.panelMiddleRight);
			this.panelMiddle.Dock = System.Windows.Forms.DockStyle.Top;
			this.panelMiddle.Location = new System.Drawing.Point(10, 68);
			this.panelMiddle.Name = "panelMiddle";
			this.panelMiddle.Size = new System.Drawing.Size(672, 160);
			this.panelMiddle.TabIndex = 52;
			// 
			// panelMiddleMiddle
			// 
			this.panelMiddleMiddle.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelMiddleMiddle.Location = new System.Drawing.Point(232, 0);
			this.panelMiddleMiddle.Name = "panelMiddleMiddle";
			this.panelMiddleMiddle.Size = new System.Drawing.Size(55, 160);
			this.panelMiddleMiddle.TabIndex = 46;
			// 
			// panelDebug
			// 
			this.panelDebug.Controls.Add(this.txtDebugRoutes);
			this.panelDebug.Controls.Add(this.btnDebugAddPoint);
			this.panelDebug.Controls.Add(this.btnDebugPlay);
			this.panelDebug.Dock = System.Windows.Forms.DockStyle.Top;
			this.panelDebug.Location = new System.Drawing.Point(10, 228);
			this.panelDebug.Name = "panelDebug";
			this.panelDebug.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
			this.panelDebug.Size = new System.Drawing.Size(672, 123);
			this.panelDebug.TabIndex = 53;
			this.panelDebug.Visible = false;
			// 
			// txtDebugRoutes
			// 
			this.txtDebugRoutes.Dock = System.Windows.Forms.DockStyle.Right;
			this.txtDebugRoutes.Location = new System.Drawing.Point(122, 5);
			this.txtDebugRoutes.Multiline = true;
			this.txtDebugRoutes.Name = "txtDebugRoutes";
			this.txtDebugRoutes.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtDebugRoutes.Size = new System.Drawing.Size(443, 113);
			this.txtDebugRoutes.TabIndex = 45;
			// 
			// btnDebugAddPoint
			// 
			this.btnDebugAddPoint.BackColor = System.Drawing.Color.White;
			this.btnDebugAddPoint.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnDebugAddPoint.Location = new System.Drawing.Point(7, 6);
			this.btnDebugAddPoint.Name = "btnDebugAddPoint";
			this.btnDebugAddPoint.Size = new System.Drawing.Size(109, 30);
			this.btnDebugAddPoint.TabIndex = 44;
			this.btnDebugAddPoint.Text = "Add Point";
			this.btnDebugAddPoint.UseVisualStyleBackColor = false;
			this.btnDebugAddPoint.Click += new System.EventHandler(this.btnDebugAddPoint_Click);
			// 
			// btnDebugPlay
			// 
			this.btnDebugPlay.BackColor = System.Drawing.Color.White;
			this.btnDebugPlay.Dock = System.Windows.Forms.DockStyle.Right;
			this.btnDebugPlay.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnDebugPlay.Location = new System.Drawing.Point(565, 5);
			this.btnDebugPlay.Name = "btnDebugPlay";
			this.btnDebugPlay.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
			this.btnDebugPlay.Size = new System.Drawing.Size(107, 113);
			this.btnDebugPlay.TabIndex = 42;
			this.btnDebugPlay.Text = "TEST >>";
			this.btnDebugPlay.UseVisualStyleBackColor = false;
			this.btnDebugPlay.Click += new System.EventHandler(this.btnDebugPlay_Click);
			// 
			// DrawThatThing
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.ClientSize = new System.Drawing.Size(692, 660);
			this.Controls.Add(this.panelBottom);
			this.Controls.Add(this.panelDebug);
			this.Controls.Add(this.panelMiddle);
			this.Controls.Add(this.panelTop);
			this.MinimumSize = new System.Drawing.Size(700, 333);
			this.Name = "DrawThatThing";
			this.Padding = new System.Windows.Forms.Padding(10);
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "DrawThatThing";
			((System.ComponentModel.ISupportInitialize)(this.pctPreview)).EndInit();
			this.panelPreview.ResumeLayout(false);
			this.panelPreviewInner.ResumeLayout(false);
			this.panelPlay.ResumeLayout(false);
			this.panelPlay.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.intPreviewWidth)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.intPreviewHeight)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dataGridColors)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dataGridSettings)).EndInit();
			this.panelBottom.ResumeLayout(false);
			this.panelMiddleRight.ResumeLayout(false);
			this.panelMiddleRight.PerformLayout();
			this.panel6.ResumeLayout(false);
			this.panelTop.ResumeLayout(false);
			this.panelTop.PerformLayout();
			this.panelMiddleLeft.ResumeLayout(false);
			this.panelMiddleLeft.PerformLayout();
			this.panelMiddle.ResumeLayout(false);
			this.panelDebug.ResumeLayout(false);
			this.panelDebug.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TextBox txtMousePositionY;
		private System.Windows.Forms.TextBox txtMousePositionX;
		private System.Windows.Forms.Label lblX;
		private System.Windows.Forms.Button btnPlay;
		private System.ComponentModel.BackgroundWorker workerClickAround;
		private System.Windows.Forms.Button btnLoadImage;
		private System.Windows.Forms.OpenFileDialog dlgImportImage;
		private System.Windows.Forms.PictureBox pctPreview;
		private System.Windows.Forms.Panel panelPreview;
		private System.Windows.Forms.NumericUpDown intPreviewWidth;
		private System.Windows.Forms.NumericUpDown intPreviewHeight;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label lblColorOptions;
		private System.Windows.Forms.DataGridView dataGridColors;
		private System.Windows.Forms.Button btnExport;
		private System.Windows.Forms.Button btnImport;
		private System.Windows.Forms.SaveFileDialog dlgExportPixels;
        private System.Windows.Forms.OpenFileDialog dlgImportPixels;
		private System.ComponentModel.BackgroundWorker workerCalculate;
		private System.Windows.Forms.Label lblStopMouseShortcut;
		private System.Windows.Forms.Label lblStartPositionShortcut;
		private System.Windows.Forms.Label lblpickColorShortcut;
		private System.Windows.Forms.Label lblPickColorTitle;
		private System.Windows.Forms.Label lblStartPositionTitle;
		private System.Windows.Forms.Label lblStopMouseTitle;
        private System.Windows.Forms.ComboBox cmbParser;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.DataGridView dataGridSettings;
		private System.Windows.Forms.DataGridViewTextBoxColumn settingName;
		private System.Windows.Forms.DataGridViewTextBoxColumn Value;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Button btnReparse;
		private System.Windows.Forms.Button btnClearUnusedColors;
		private System.Windows.Forms.DataGridViewTextBoxColumn PosX;
		private System.Windows.Forms.DataGridViewTextBoxColumn PosY;
		private System.Windows.Forms.DataGridViewTextBoxColumn RGB;
		private System.Windows.Forms.DataGridViewCheckBoxColumn IsBackground;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.ComboBox cmbBrushSelector;
		private System.Windows.Forms.Panel panelPlay;
		private System.Windows.Forms.Panel panelBottom;
		private System.Windows.Forms.Panel panelMiddleRight;
		private System.Windows.Forms.Panel panel6;
		private System.Windows.Forms.Panel panelTop;
		private System.Windows.Forms.Panel panelMiddleLeft;
		private System.Windows.Forms.Panel panelMiddle;
		private System.Windows.Forms.Panel panelMiddleMiddle;
		private System.Windows.Forms.Panel panelPreviewInner;
		private System.Windows.Forms.Panel panelDebug;
		private System.Windows.Forms.Button btnDebugPlay;
		private System.Windows.Forms.TextBox txtDebugRoutes;
		private System.Windows.Forms.Button btnDebugAddPoint;
	}
}

