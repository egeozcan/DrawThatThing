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
			this.txtMousePositionY = new System.Windows.Forms.TextBox();
			this.txtMousePositionX = new System.Windows.Forms.TextBox();
			this.lblX = new System.Windows.Forms.Label();
			this.btnGetPosition = new System.Windows.Forms.Button();
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
			this.PosX = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.PosY = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.RGB = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.btnBringUp = new System.Windows.Forms.Button();
			this.btnExport = new System.Windows.Forms.Button();
			this.btnImport = new System.Windows.Forms.Button();
			this.dlgExportPixels = new System.Windows.Forms.SaveFileDialog();
			this.dlgImportPixels = new System.Windows.Forms.OpenFileDialog();
			this.chkUseAlternativeParser = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.pctPreview)).BeginInit();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.intPreviewWidth)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.intPreviewHeight)).BeginInit();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridColors)).BeginInit();
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
			// btnGetPosition
			// 
			this.btnGetPosition.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnGetPosition.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnGetPosition.Location = new System.Drawing.Point(230, 15);
			this.btnGetPosition.Name = "btnGetPosition";
			this.btnGetPosition.Size = new System.Drawing.Size(101, 30);
			this.btnGetPosition.TabIndex = 3;
			this.btnGetPosition.Text = "Set to cursor pos";
			this.btnGetPosition.UseVisualStyleBackColor = true;
			this.btnGetPosition.Click += new System.EventHandler(this.btnGetPosition_Click);
			// 
			// btnPlay
			// 
			this.btnPlay.BackColor = System.Drawing.Color.Green;
			this.btnPlay.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnPlay.ForeColor = System.Drawing.Color.White;
			this.btnPlay.Location = new System.Drawing.Point(676, 374);
			this.btnPlay.Name = "btnPlay";
			this.btnPlay.Size = new System.Drawing.Size(107, 39);
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
			this.btnLoadImage.Location = new System.Drawing.Point(438, 12);
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
			this.pctPreview.Size = new System.Drawing.Size(222, 231);
			this.pctPreview.TabIndex = 22;
			this.pctPreview.TabStop = false;
			// 
			// panel1
			// 
			this.panel1.AutoScroll = true;
			this.panel1.Controls.Add(this.pctPreview);
			this.panel1.Location = new System.Drawing.Point(12, 12);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(420, 400);
			this.panel1.TabIndex = 26;
			// 
			// intPreviewWidth
			// 
			this.intPreviewWidth.Location = new System.Drawing.Point(727, 11);
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
			this.intPreviewHeight.Location = new System.Drawing.Point(727, 43);
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
			this.label10.Location = new System.Drawing.Point(684, 14);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(35, 13);
			this.label10.TabIndex = 28;
			this.label10.Text = "Width";
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(681, 46);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(38, 13);
			this.label11.TabIndex = 28;
			this.label11.Text = "Height";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.btnGetPosition);
			this.groupBox1.Controls.Add(this.txtMousePositionY);
			this.groupBox1.Controls.Add(this.txtMousePositionX);
			this.groupBox1.Controls.Add(this.lblX);
			this.groupBox1.Location = new System.Drawing.Point(438, 319);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(345, 51);
			this.groupBox1.TabIndex = 29;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Mouse Start Position";
			// 
			// lblColorOptions
			// 
			this.lblColorOptions.AutoSize = true;
			this.lblColorOptions.Location = new System.Drawing.Point(459, 58);
			this.lblColorOptions.Name = "lblColorOptions";
			this.lblColorOptions.Size = new System.Drawing.Size(70, 13);
			this.lblColorOptions.TabIndex = 31;
			this.lblColorOptions.Text = "Color Options";
			// 
			// dataGridColors
			// 
			this.dataGridColors.BackgroundColor = System.Drawing.Color.White;
			this.dataGridColors.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
			this.dataGridColors.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridColors.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.PosX,
            this.PosY,
            this.RGB});
			this.dataGridColors.Location = new System.Drawing.Point(438, 74);
			this.dataGridColors.MultiSelect = false;
			this.dataGridColors.Name = "dataGridColors";
			this.dataGridColors.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
			this.dataGridColors.Size = new System.Drawing.Size(345, 211);
			this.dataGridColors.TabIndex = 34;
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
			// btnBringUp
			// 
			this.btnBringUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnBringUp.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnBringUp.Location = new System.Drawing.Point(551, 291);
			this.btnBringUp.Name = "btnBringUp";
			this.btnBringUp.Size = new System.Drawing.Size(30, 22);
			this.btnBringUp.TabIndex = 3;
			this.btnBringUp.Text = "^^";
			this.btnBringUp.UseVisualStyleBackColor = true;
			this.btnBringUp.Click += new System.EventHandler(this.btnBringUp_Click);
			// 
			// btnExport
			// 
			this.btnExport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnExport.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnExport.Location = new System.Drawing.Point(587, 291);
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
			this.btnImport.Location = new System.Drawing.Point(675, 291);
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
			// chkUseAlternativeParser
			// 
			this.chkUseAlternativeParser.AutoSize = true;
			this.chkUseAlternativeParser.Location = new System.Drawing.Point(552, 11);
			this.chkUseAlternativeParser.Name = "chkUseAlternativeParser";
			this.chkUseAlternativeParser.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.chkUseAlternativeParser.Size = new System.Drawing.Size(109, 17);
			this.chkUseAlternativeParser.TabIndex = 35;
			this.chkUseAlternativeParser.Text = "Alternative Parser";
			this.chkUseAlternativeParser.UseVisualStyleBackColor = true;
			// 
			// DrawThatThing
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.ClientSize = new System.Drawing.Size(795, 424);
			this.Controls.Add(this.chkUseAlternativeParser);
			this.Controls.Add(this.btnImport);
			this.Controls.Add(this.btnExport);
			this.Controls.Add(this.btnBringUp);
			this.Controls.Add(this.dataGridColors);
			this.Controls.Add(this.lblColorOptions);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.intPreviewHeight);
			this.Controls.Add(this.intPreviewWidth);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.btnLoadImage);
			this.Controls.Add(this.btnPlay);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DrawThatThing";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "Main";
			((System.ComponentModel.ISupportInitialize)(this.pctPreview)).EndInit();
			this.panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.intPreviewWidth)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.intPreviewHeight)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridColors)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox txtMousePositionY;
		private System.Windows.Forms.TextBox txtMousePositionX;
		private System.Windows.Forms.Label lblX;
		private System.Windows.Forms.Button btnGetPosition;
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
		private System.Windows.Forms.DataGridViewTextBoxColumn PosX;
		private System.Windows.Forms.DataGridViewTextBoxColumn PosY;
		private System.Windows.Forms.DataGridViewTextBoxColumn RGB;
		private System.Windows.Forms.Button btnBringUp;
		private System.Windows.Forms.Button btnExport;
		private System.Windows.Forms.Button btnImport;
		private System.Windows.Forms.SaveFileDialog dlgExportPixels;
		private System.Windows.Forms.OpenFileDialog dlgImportPixels;
		private System.Windows.Forms.CheckBox chkUseAlternativeParser;
	}
}

