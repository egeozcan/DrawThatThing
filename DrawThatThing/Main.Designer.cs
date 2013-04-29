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
			this.panel1 = new System.Windows.Forms.Panel();
			this.intPreviewWidth = new System.Windows.Forms.NumericUpDown();
			this.intPreviewHeight = new System.Windows.Forms.NumericUpDown();
			this.label10 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.lblColorOptions = new System.Windows.Forms.Label();
			this.dataGridColors = new System.Windows.Forms.DataGridView();
			this.btnExport = new System.Windows.Forms.Button();
			this.btnImport = new System.Windows.Forms.Button();
			this.dlgExportPixels = new System.Windows.Forms.SaveFileDialog();
			this.dlgImportPixels = new System.Windows.Forms.OpenFileDialog();
			this.workerCalculate = new System.ComponentModel.BackgroundWorker();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.cmbParser = new System.Windows.Forms.ComboBox();
			this.label7 = new System.Windows.Forms.Label();
			this.dataGridSettings = new System.Windows.Forms.DataGridView();
			this.settingName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.label8 = new System.Windows.Forms.Label();
			this.btnReparse = new System.Windows.Forms.Button();
			this.btnClearUnusedColors = new System.Windows.Forms.Button();
			this.PosX = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.PosY = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.RGB = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.IsBackground = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			((System.ComponentModel.ISupportInitialize)(this.pctPreview)).BeginInit();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.intPreviewWidth)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.intPreviewHeight)).BeginInit();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridColors)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dataGridSettings)).BeginInit();
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
			this.btnPlay.BackColor = System.Drawing.Color.Green;
			this.btnPlay.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnPlay.ForeColor = System.Drawing.Color.White;
			this.btnPlay.Location = new System.Drawing.Point(516, 765);
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
			this.btnLoadImage.BackColor = System.Drawing.Color.Yellow;
			this.btnLoadImage.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnLoadImage.ForeColor = System.Drawing.Color.Black;
			this.btnLoadImage.Location = new System.Drawing.Point(519, 10);
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
			this.pctPreview.BackColor = System.Drawing.Color.White;
			this.pctPreview.Location = new System.Drawing.Point(0, 0);
			this.pctPreview.Name = "pctPreview";
			this.pctPreview.Size = new System.Drawing.Size(0, 0);
			this.pctPreview.TabIndex = 22;
			this.pctPreview.TabStop = false;
			// 
			// panel1
			// 
			this.panel1.AutoScroll = true;
			this.panel1.BackColor = System.Drawing.Color.WhiteSmoke;
			this.panel1.Controls.Add(this.pctPreview);
			this.panel1.Location = new System.Drawing.Point(12, 319);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(610, 429);
			this.panel1.TabIndex = 26;
			// 
			// intPreviewWidth
			// 
			this.intPreviewWidth.Enabled = false;
			this.intPreviewWidth.Location = new System.Drawing.Point(207, 8);
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
			this.intPreviewHeight.Location = new System.Drawing.Point(207, 30);
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
			this.label10.Location = new System.Drawing.Point(164, 11);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(35, 13);
			this.label10.TabIndex = 28;
			this.label10.Text = "Width";
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(161, 33);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(38, 13);
			this.label11.TabIndex = 28;
			this.label11.Text = "Height";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.txtMousePositionY);
			this.groupBox1.Controls.Add(this.txtMousePositionX);
			this.groupBox1.Controls.Add(this.lblX);
			this.groupBox1.Location = new System.Drawing.Point(11, 754);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(225, 53);
			this.groupBox1.TabIndex = 29;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Mouse Start Position";
			// 
			// lblColorOptions
			// 
			this.lblColorOptions.AutoSize = true;
			this.lblColorOptions.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblColorOptions.Location = new System.Drawing.Point(14, 58);
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
			this.dataGridColors.EnableHeadersVisualStyles = false;
			this.dataGridColors.Location = new System.Drawing.Point(17, 74);
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
			this.dataGridColors.Size = new System.Drawing.Size(345, 211);
			this.dataGridColors.TabIndex = 34;
			this.dataGridColors.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridColors_CellEndEdit);
			this.dataGridColors.CellLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridColors_CellLeave);
			this.dataGridColors.RowLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridColors_RowLeave);
			// 
			// btnExport
			// 
			this.btnExport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnExport.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnExport.Location = new System.Drawing.Point(192, 291);
			this.btnExport.Name = "btnExport";
			this.btnExport.Size = new System.Drawing.Size(82, 22);
			this.btnExport.TabIndex = 3;
			this.btnExport.Text = "Export";
			this.btnExport.UseVisualStyleBackColor = true;
			this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
			// 
			// btnImport
			// 
			this.btnImport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnImport.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnImport.Location = new System.Drawing.Point(280, 291);
			this.btnImport.Name = "btnImport";
			this.btnImport.Size = new System.Drawing.Size(82, 22);
			this.btnImport.TabIndex = 3;
			this.btnImport.Text = "Import";
			this.btnImport.UseVisualStyleBackColor = true;
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
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(437, 765);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(71, 13);
			this.label1.TabIndex = 36;
			this.label1.Text = "Shift + Alt + C";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(437, 779);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(71, 13);
			this.label2.TabIndex = 37;
			this.label2.Text = "Shift + Alt + S";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(437, 794);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(71, 13);
			this.label3.TabIndex = 38;
			this.label3.Text = "Shift + Alt + A";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.Location = new System.Drawing.Point(368, 794);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(72, 13);
			this.label4.TabIndex = 41;
			this.label4.Text = "Pick color :";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label5.Location = new System.Drawing.Point(274, 779);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(165, 13);
			this.label5.TabIndex = 40;
			this.label5.Text = "Set start position to cursor :";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label6
			// 
			this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label6.AutoSize = true;
			this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label6.Location = new System.Drawing.Point(363, 763);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(73, 17);
			this.label6.TabIndex = 39;
			this.label6.Text = "Stop mouse :";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.label6.UseCompatibleTextRendering = true;
			// 
			// cmbParser
			// 
			this.cmbParser.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbParser.FormattingEnabled = true;
			this.cmbParser.Location = new System.Drawing.Point(18, 29);
			this.cmbParser.Name = "cmbParser";
			this.cmbParser.Size = new System.Drawing.Size(121, 21);
			this.cmbParser.TabIndex = 42;
			this.cmbParser.SelectedIndexChanged += new System.EventHandler(this.cmbParser_SelectedIndexChanged);
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(15, 15);
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
			this.dataGridSettings.EnableHeadersVisualStyles = false;
			this.dataGridSettings.Location = new System.Drawing.Point(379, 74);
			this.dataGridSettings.Name = "dataGridSettings";
			this.dataGridSettings.RowHeadersVisible = false;
			this.dataGridSettings.Size = new System.Drawing.Size(243, 239);
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
			this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label8.Location = new System.Drawing.Point(375, 57);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(93, 13);
			this.label8.TabIndex = 45;
			this.label8.Text = "Parser Settings";
			// 
			// btnReparse
			// 
			this.btnReparse.BackColor = System.Drawing.Color.RoyalBlue;
			this.btnReparse.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnReparse.ForeColor = System.Drawing.Color.White;
			this.btnReparse.Location = new System.Drawing.Point(412, 10);
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
			this.btnClearUnusedColors.BackColor = System.Drawing.Color.Red;
			this.btnClearUnusedColors.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnClearUnusedColors.ForeColor = System.Drawing.Color.White;
			this.btnClearUnusedColors.Location = new System.Drawing.Point(305, 10);
			this.btnClearUnusedColors.Name = "btnClearUnusedColors";
			this.btnClearUnusedColors.Size = new System.Drawing.Size(101, 37);
			this.btnClearUnusedColors.TabIndex = 3;
			this.btnClearUnusedColors.Text = "Clear Unused Colors";
			this.btnClearUnusedColors.UseVisualStyleBackColor = false;
			this.btnClearUnusedColors.Visible = false;
			this.btnClearUnusedColors.Click += new System.EventHandler(this.btnClearUnusedColors_Click);
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
			// DrawThatThing
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.ClientSize = new System.Drawing.Size(633, 817);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.dataGridSettings);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.cmbParser);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnImport);
			this.Controls.Add(this.btnExport);
			this.Controls.Add(this.dataGridColors);
			this.Controls.Add(this.lblColorOptions);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.intPreviewHeight);
			this.Controls.Add(this.intPreviewWidth);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.btnClearUnusedColors);
			this.Controls.Add(this.btnReparse);
			this.Controls.Add(this.btnLoadImage);
			this.Controls.Add(this.btnPlay);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DrawThatThing";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "DrawThatThing";
			((System.ComponentModel.ISupportInitialize)(this.pctPreview)).EndInit();
			this.panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.intPreviewWidth)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.intPreviewHeight)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridColors)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dataGridSettings)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

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
		private System.Windows.Forms.Panel panel1;
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
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
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
	}
}

