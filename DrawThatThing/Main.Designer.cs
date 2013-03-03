namespace DrawThatThing
{
	partial class Main
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
			this.lblY = new System.Windows.Forms.Label();
			this.btnGetPosition = new System.Windows.Forms.Button();
			this.btnPlay = new System.Windows.Forms.Button();
			this.workerClickAround = new System.ComponentModel.BackgroundWorker();
			this.btnLoadImage = new System.Windows.Forms.Button();
			this.dlgImportImage = new System.Windows.Forms.OpenFileDialog();
			this.pctPreview = new System.Windows.Forms.PictureBox();
			this.intWaitBetween = new System.Windows.Forms.NumericUpDown();
			this.label9 = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.intPreviewWidth = new System.Windows.Forms.NumericUpDown();
			this.intPreviewHeight = new System.Windows.Forms.NumericUpDown();
			this.label10 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			((System.ComponentModel.ISupportInitialize)(this.pctPreview)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.intWaitBetween)).BeginInit();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.intPreviewWidth)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.intPreviewHeight)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// txtMousePositionY
			// 
			this.txtMousePositionY.Location = new System.Drawing.Point(33, 21);
			this.txtMousePositionY.Name = "txtMousePositionY";
			this.txtMousePositionY.Size = new System.Drawing.Size(79, 20);
			this.txtMousePositionY.TabIndex = 0;
			this.txtMousePositionY.Text = "0";
			// 
			// txtMousePositionX
			// 
			this.txtMousePositionX.Location = new System.Drawing.Point(33, 47);
			this.txtMousePositionX.Name = "txtMousePositionX";
			this.txtMousePositionX.Size = new System.Drawing.Size(79, 20);
			this.txtMousePositionX.TabIndex = 1;
			this.txtMousePositionX.Text = "0";
			// 
			// lblX
			// 
			this.lblX.AutoSize = true;
			this.lblX.Location = new System.Drawing.Point(13, 50);
			this.lblX.Name = "lblX";
			this.lblX.Size = new System.Drawing.Size(14, 13);
			this.lblX.TabIndex = 2;
			this.lblX.Text = "X";
			// 
			// lblY
			// 
			this.lblY.AutoSize = true;
			this.lblY.Location = new System.Drawing.Point(12, 24);
			this.lblY.Name = "lblY";
			this.lblY.Size = new System.Drawing.Size(14, 13);
			this.lblY.TabIndex = 2;
			this.lblY.Text = "Y";
			// 
			// btnGetPosition
			// 
			this.btnGetPosition.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnGetPosition.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnGetPosition.Location = new System.Drawing.Point(118, 21);
			this.btnGetPosition.Name = "btnGetPosition";
			this.btnGetPosition.Size = new System.Drawing.Size(75, 46);
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
			this.btnPlay.Location = new System.Drawing.Point(676, 102);
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
			this.btnLoadImage.Location = new System.Drawing.Point(12, 15);
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
			// intWaitBetween
			// 
			this.intWaitBetween.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.intWaitBetween.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this.intWaitBetween.Location = new System.Drawing.Point(570, 118);
			this.intWaitBetween.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
			this.intWaitBetween.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.intWaitBetween.Name = "intWaitBetween";
			this.intWaitBetween.Size = new System.Drawing.Size(91, 23);
			this.intWaitBetween.TabIndex = 20;
			this.intWaitBetween.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(568, 100);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(96, 13);
			this.label9.TabIndex = 25;
			this.label9.Text = "Wait Between (ms)";
			// 
			// panel1
			// 
			this.panel1.AutoScroll = true;
			this.panel1.Controls.Add(this.pctPreview);
			this.panel1.Location = new System.Drawing.Point(7, 147);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(776, 267);
			this.panel1.TabIndex = 26;
			// 
			// intPreviewWidth
			// 
			this.intPreviewWidth.Location = new System.Drawing.Point(61, 67);
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
			this.intPreviewHeight.Location = new System.Drawing.Point(61, 93);
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
			this.label10.Location = new System.Drawing.Point(11, 70);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(35, 13);
			this.label10.TabIndex = 28;
			this.label10.Text = "Width";
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(11, 96);
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
			this.groupBox1.Controls.Add(this.lblY);
			this.groupBox1.Location = new System.Drawing.Point(570, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(213, 81);
			this.groupBox1.TabIndex = 29;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Mouse Start Position";
			// 
			// Main
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.ClientSize = new System.Drawing.Size(795, 424);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.intWaitBetween);
			this.Controls.Add(this.label9);
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
			this.Name = "Main";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "Main";
			((System.ComponentModel.ISupportInitialize)(this.pctPreview)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.intWaitBetween)).EndInit();
			this.panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.intPreviewWidth)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.intPreviewHeight)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox txtMousePositionY;
		private System.Windows.Forms.TextBox txtMousePositionX;
		private System.Windows.Forms.Label lblX;
		private System.Windows.Forms.Label lblY;
		private System.Windows.Forms.Button btnGetPosition;
		private System.Windows.Forms.Button btnPlay;
		private System.ComponentModel.BackgroundWorker workerClickAround;
		private System.Windows.Forms.Button btnLoadImage;
		private System.Windows.Forms.OpenFileDialog dlgImportImage;
		private System.Windows.Forms.PictureBox pctPreview;
		private System.Windows.Forms.NumericUpDown intWaitBetween;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.NumericUpDown intPreviewWidth;
		private System.Windows.Forms.NumericUpDown intPreviewHeight;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.GroupBox groupBox1;
	}
}

