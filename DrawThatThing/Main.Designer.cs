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
			this.txtActions = new System.Windows.Forms.TextBox();
			this.btnPlay = new System.Windows.Forms.Button();
			this.workerClickAround = new System.ComponentModel.BackgroundWorker();
			this.btnLoadImage = new System.Windows.Forms.Button();
			this.dlgImportImage = new System.Windows.Forms.OpenFileDialog();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.intRedMin = new System.Windows.Forms.NumericUpDown();
			this.intRedMax = new System.Windows.Forms.NumericUpDown();
			this.intGreenMax = new System.Windows.Forms.NumericUpDown();
			this.intGreenMin = new System.Windows.Forms.NumericUpDown();
			this.intBlueMax = new System.Windows.Forms.NumericUpDown();
			this.intBlueMin = new System.Windows.Forms.NumericUpDown();
			this.chkRedActive = new System.Windows.Forms.CheckBox();
			this.chkGreenActive = new System.Windows.Forms.CheckBox();
			this.chkBlueActive = new System.Windows.Forms.CheckBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.intMaxLightness = new System.Windows.Forms.NumericUpDown();
			this.pctPreview = new System.Windows.Forms.PictureBox();
			this.btnLoadBitmap = new System.Windows.Forms.Button();
			this.label8 = new System.Windows.Forms.Label();
			this.intWaitBetween = new System.Windows.Forms.NumericUpDown();
			this.label9 = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.intPreviewWidth = new System.Windows.Forms.NumericUpDown();
			this.intPreviewHeight = new System.Windows.Forms.NumericUpDown();
			this.label10 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.intRedMin)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.intRedMax)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.intGreenMax)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.intGreenMin)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.intBlueMax)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.intBlueMin)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.intMaxLightness)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pctPreview)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.intWaitBetween)).BeginInit();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.intPreviewWidth)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.intPreviewHeight)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// txtMousePositionY
			// 
			this.txtMousePositionY.Location = new System.Drawing.Point(704, 20);
			this.txtMousePositionY.Name = "txtMousePositionY";
			this.txtMousePositionY.Size = new System.Drawing.Size(79, 20);
			this.txtMousePositionY.TabIndex = 0;
			this.txtMousePositionY.Text = "0";
			// 
			// txtMousePositionX
			// 
			this.txtMousePositionX.Location = new System.Drawing.Point(704, 46);
			this.txtMousePositionX.Name = "txtMousePositionX";
			this.txtMousePositionX.Size = new System.Drawing.Size(79, 20);
			this.txtMousePositionX.TabIndex = 1;
			this.txtMousePositionX.Text = "0";
			// 
			// lblX
			// 
			this.lblX.AutoSize = true;
			this.lblX.Location = new System.Drawing.Point(684, 49);
			this.lblX.Name = "lblX";
			this.lblX.Size = new System.Drawing.Size(14, 13);
			this.lblX.TabIndex = 2;
			this.lblX.Text = "X";
			// 
			// lblY
			// 
			this.lblY.AutoSize = true;
			this.lblY.Location = new System.Drawing.Point(683, 23);
			this.lblY.Name = "lblY";
			this.lblY.Size = new System.Drawing.Size(14, 13);
			this.lblY.TabIndex = 2;
			this.lblY.Text = "Y";
			// 
			// btnGetPosition
			// 
			this.btnGetPosition.Location = new System.Drawing.Point(687, 72);
			this.btnGetPosition.Name = "btnGetPosition";
			this.btnGetPosition.Size = new System.Drawing.Size(96, 44);
			this.btnGetPosition.TabIndex = 3;
			this.btnGetPosition.Text = "Set Start Pos";
			this.btnGetPosition.UseVisualStyleBackColor = true;
			this.btnGetPosition.Click += new System.EventHandler(this.btnGetPosition_Click);
			// 
			// txtActions
			// 
			this.txtActions.AcceptsReturn = true;
			this.txtActions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtActions.Location = new System.Drawing.Point(7, 145);
			this.txtActions.MaxLength = 600000;
			this.txtActions.Multiline = true;
			this.txtActions.Name = "txtActions";
			this.txtActions.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtActions.Size = new System.Drawing.Size(317, 236);
			this.txtActions.TabIndex = 4;
			this.txtActions.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// btnPlay
			// 
			this.btnPlay.BackColor = System.Drawing.Color.Green;
			this.btnPlay.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnPlay.ForeColor = System.Drawing.Color.White;
			this.btnPlay.Location = new System.Drawing.Point(687, 371);
			this.btnPlay.Name = "btnPlay";
			this.btnPlay.Size = new System.Drawing.Size(96, 43);
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
			this.btnLoadImage.Location = new System.Drawing.Point(223, 102);
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
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(4, 38);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(27, 13);
			this.label1.TabIndex = 8;
			this.label1.Text = "Red";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(4, 64);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(36, 13);
			this.label2.TabIndex = 11;
			this.label2.Text = "Green";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(4, 90);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(28, 13);
			this.label3.TabIndex = 14;
			this.label3.Text = "Blue";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(55, 15);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(27, 13);
			this.label4.TabIndex = 16;
			this.label4.Text = "MIN";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(113, 15);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(30, 13);
			this.label5.TabIndex = 16;
			this.label5.Text = "MAX";
			// 
			// intRedMin
			// 
			this.intRedMin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.intRedMin.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.intRedMin.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this.intRedMin.Location = new System.Drawing.Point(46, 34);
			this.intRedMin.Maximum = new decimal(new int[] {
            256,
            0,
            0,
            0});
			this.intRedMin.Name = "intRedMin";
			this.intRedMin.Size = new System.Drawing.Size(53, 23);
			this.intRedMin.TabIndex = 5;
			this.intRedMin.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
			// 
			// intRedMax
			// 
			this.intRedMax.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.intRedMax.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.intRedMax.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this.intRedMax.Location = new System.Drawing.Point(106, 34);
			this.intRedMax.Maximum = new decimal(new int[] {
            256,
            0,
            0,
            0});
			this.intRedMax.Name = "intRedMax";
			this.intRedMax.Size = new System.Drawing.Size(53, 23);
			this.intRedMax.TabIndex = 5;
			this.intRedMax.Value = new decimal(new int[] {
            256,
            0,
            0,
            0});
			// 
			// intGreenMax
			// 
			this.intGreenMax.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.intGreenMax.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.intGreenMax.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this.intGreenMax.Location = new System.Drawing.Point(106, 60);
			this.intGreenMax.Maximum = new decimal(new int[] {
            256,
            0,
            0,
            0});
			this.intGreenMax.Name = "intGreenMax";
			this.intGreenMax.Size = new System.Drawing.Size(53, 23);
			this.intGreenMax.TabIndex = 18;
			this.intGreenMax.Value = new decimal(new int[] {
            256,
            0,
            0,
            0});
			// 
			// intGreenMin
			// 
			this.intGreenMin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.intGreenMin.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.intGreenMin.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this.intGreenMin.Location = new System.Drawing.Point(46, 60);
			this.intGreenMin.Maximum = new decimal(new int[] {
            256,
            0,
            0,
            0});
			this.intGreenMin.Name = "intGreenMin";
			this.intGreenMin.Size = new System.Drawing.Size(53, 23);
			this.intGreenMin.TabIndex = 17;
			this.intGreenMin.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
			// 
			// intBlueMax
			// 
			this.intBlueMax.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.intBlueMax.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.intBlueMax.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this.intBlueMax.Location = new System.Drawing.Point(106, 86);
			this.intBlueMax.Maximum = new decimal(new int[] {
            256,
            0,
            0,
            0});
			this.intBlueMax.Name = "intBlueMax";
			this.intBlueMax.Size = new System.Drawing.Size(53, 23);
			this.intBlueMax.TabIndex = 20;
			this.intBlueMax.Value = new decimal(new int[] {
            256,
            0,
            0,
            0});
			// 
			// intBlueMin
			// 
			this.intBlueMin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.intBlueMin.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.intBlueMin.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this.intBlueMin.Location = new System.Drawing.Point(46, 86);
			this.intBlueMin.Maximum = new decimal(new int[] {
            256,
            0,
            0,
            0});
			this.intBlueMin.Name = "intBlueMin";
			this.intBlueMin.Size = new System.Drawing.Size(53, 23);
			this.intBlueMin.TabIndex = 19;
			this.intBlueMin.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
			// 
			// chkRedActive
			// 
			this.chkRedActive.AutoSize = true;
			this.chkRedActive.BackColor = System.Drawing.Color.Silver;
			this.chkRedActive.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.chkRedActive.ForeColor = System.Drawing.Color.White;
			this.chkRedActive.Location = new System.Drawing.Point(182, 37);
			this.chkRedActive.Name = "chkRedActive";
			this.chkRedActive.Size = new System.Drawing.Size(12, 11);
			this.chkRedActive.TabIndex = 21;
			this.chkRedActive.UseVisualStyleBackColor = false;
			// 
			// chkGreenActive
			// 
			this.chkGreenActive.AutoSize = true;
			this.chkGreenActive.BackColor = System.Drawing.Color.Silver;
			this.chkGreenActive.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.chkGreenActive.ForeColor = System.Drawing.Color.White;
			this.chkGreenActive.Location = new System.Drawing.Point(182, 63);
			this.chkGreenActive.Name = "chkGreenActive";
			this.chkGreenActive.Size = new System.Drawing.Size(12, 11);
			this.chkGreenActive.TabIndex = 21;
			this.chkGreenActive.UseVisualStyleBackColor = false;
			// 
			// chkBlueActive
			// 
			this.chkBlueActive.AutoSize = true;
			this.chkBlueActive.BackColor = System.Drawing.Color.Silver;
			this.chkBlueActive.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.chkBlueActive.ForeColor = System.Drawing.Color.White;
			this.chkBlueActive.Location = new System.Drawing.Point(182, 89);
			this.chkBlueActive.Name = "chkBlueActive";
			this.chkBlueActive.Size = new System.Drawing.Size(12, 11);
			this.chkBlueActive.TabIndex = 21;
			this.chkBlueActive.UseVisualStyleBackColor = false;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(168, 15);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(45, 13);
			this.label6.TabIndex = 16;
			this.label6.Text = "ACTIVE";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(4, 122);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(78, 13);
			this.label7.TabIndex = 14;
			this.label7.Text = "Max Lightness ";
			// 
			// intMaxLightness
			// 
			this.intMaxLightness.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this.intMaxLightness.Location = new System.Drawing.Point(88, 119);
			this.intMaxLightness.Maximum = new decimal(new int[] {
            768,
            0,
            0,
            0});
			this.intMaxLightness.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this.intMaxLightness.Name = "intMaxLightness";
			this.intMaxLightness.Size = new System.Drawing.Size(125, 20);
			this.intMaxLightness.TabIndex = 20;
			this.intMaxLightness.Value = new decimal(new int[] {
            600,
            0,
            0,
            0});
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
			// btnLoadBitmap
			// 
			this.btnLoadBitmap.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
			this.btnLoadBitmap.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnLoadBitmap.ForeColor = System.Drawing.Color.White;
			this.btnLoadBitmap.Location = new System.Drawing.Point(7, 387);
			this.btnLoadBitmap.Name = "btnLoadBitmap";
			this.btnLoadBitmap.Size = new System.Drawing.Size(316, 27);
			this.btnLoadBitmap.TabIndex = 23;
			this.btnLoadBitmap.Text = "Show Preview >>";
			this.btnLoadBitmap.UseVisualStyleBackColor = false;
			this.btnLoadBitmap.Click += new System.EventHandler(this.btnLoadBitmap_Click);
			// 
			// label8
			// 
			this.label8.BackColor = System.Drawing.Color.White;
			this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label8.Location = new System.Drawing.Point(330, 6);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(347, 35);
			this.label8.TabIndex = 24;
			this.label8.Text = "Result Preview";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// intWaitBetween
			// 
			this.intWaitBetween.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.intWaitBetween.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this.intWaitBetween.Location = new System.Drawing.Point(687, 346);
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
			this.intWaitBetween.Size = new System.Drawing.Size(96, 23);
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
			this.label9.Location = new System.Drawing.Point(683, 330);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(96, 13);
			this.label9.TabIndex = 25;
			this.label9.Text = "Wait Between (ms)";
			// 
			// panel1
			// 
			this.panel1.AutoScroll = true;
			this.panel1.Controls.Add(this.pctPreview);
			this.panel1.Location = new System.Drawing.Point(334, 38);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(343, 376);
			this.panel1.TabIndex = 26;
			// 
			// intPreviewWidth
			// 
			this.intPreviewWidth.Location = new System.Drawing.Point(270, 38);
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
			this.intPreviewHeight.Location = new System.Drawing.Point(270, 64);
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
			this.label10.Location = new System.Drawing.Point(220, 41);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(35, 13);
			this.label10.TabIndex = 28;
			this.label10.Text = "Width";
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(220, 67);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(38, 13);
			this.label11.TabIndex = 28;
			this.label11.Text = "Height";
			// 
			// pictureBox1
			// 
			this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pictureBox1.Location = new System.Drawing.Point(7, 420);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(772, 368);
			this.pictureBox1.TabIndex = 29;
			this.pictureBox1.TabStop = false;
			// 
			// Main
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.ClientSize = new System.Drawing.Size(795, 800);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.intPreviewHeight);
			this.Controls.Add(this.intPreviewWidth);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.btnLoadBitmap);
			this.Controls.Add(this.chkBlueActive);
			this.Controls.Add(this.chkGreenActive);
			this.Controls.Add(this.chkRedActive);
			this.Controls.Add(this.intMaxLightness);
			this.Controls.Add(this.intWaitBetween);
			this.Controls.Add(this.intBlueMax);
			this.Controls.Add(this.intBlueMin);
			this.Controls.Add(this.intGreenMax);
			this.Controls.Add(this.intGreenMin);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.intRedMax);
			this.Controls.Add(this.intRedMin);
			this.Controls.Add(this.txtActions);
			this.Controls.Add(this.btnLoadImage);
			this.Controls.Add(this.btnPlay);
			this.Controls.Add(this.btnGetPosition);
			this.Controls.Add(this.lblY);
			this.Controls.Add(this.lblX);
			this.Controls.Add(this.txtMousePositionX);
			this.Controls.Add(this.txtMousePositionY);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Main";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "Main";
			((System.ComponentModel.ISupportInitialize)(this.intRedMin)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.intRedMax)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.intGreenMax)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.intGreenMin)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.intBlueMax)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.intBlueMin)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.intMaxLightness)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pctPreview)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.intWaitBetween)).EndInit();
			this.panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.intPreviewWidth)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.intPreviewHeight)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox txtMousePositionY;
		private System.Windows.Forms.TextBox txtMousePositionX;
		private System.Windows.Forms.Label lblX;
		private System.Windows.Forms.Label lblY;
		private System.Windows.Forms.Button btnGetPosition;
		private System.Windows.Forms.TextBox txtActions;
		private System.Windows.Forms.Button btnPlay;
		private System.ComponentModel.BackgroundWorker workerClickAround;
		private System.Windows.Forms.Button btnLoadImage;
		private System.Windows.Forms.OpenFileDialog dlgImportImage;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.NumericUpDown intRedMin;
		private System.Windows.Forms.NumericUpDown intRedMax;
		private System.Windows.Forms.NumericUpDown intGreenMax;
		private System.Windows.Forms.NumericUpDown intGreenMin;
		private System.Windows.Forms.NumericUpDown intBlueMax;
		private System.Windows.Forms.NumericUpDown intBlueMin;
		private System.Windows.Forms.CheckBox chkRedActive;
		private System.Windows.Forms.CheckBox chkGreenActive;
		private System.Windows.Forms.CheckBox chkBlueActive;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.NumericUpDown intMaxLightness;
		private System.Windows.Forms.PictureBox pctPreview;
		private System.Windows.Forms.Button btnLoadBitmap;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.NumericUpDown intWaitBetween;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.NumericUpDown intPreviewWidth;
		private System.Windows.Forms.NumericUpDown intPreviewHeight;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.PictureBox pictureBox1;
	}
}

