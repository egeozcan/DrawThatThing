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
			this.intExpand = new System.Windows.Forms.NumericUpDown();
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
			((System.ComponentModel.ISupportInitialize)(this.intExpand)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.intRedMin)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.intRedMax)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.intGreenMax)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.intGreenMin)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.intBlueMax)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.intBlueMin)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.intMaxLightness)).BeginInit();
			this.SuspendLayout();
			// 
			// txtMousePositionY
			// 
			this.txtMousePositionY.Location = new System.Drawing.Point(289, 12);
			this.txtMousePositionY.Name = "txtMousePositionY";
			this.txtMousePositionY.Size = new System.Drawing.Size(79, 20);
			this.txtMousePositionY.TabIndex = 0;
			this.txtMousePositionY.Text = "0";
			// 
			// txtMousePositionX
			// 
			this.txtMousePositionX.Location = new System.Drawing.Point(289, 38);
			this.txtMousePositionX.Name = "txtMousePositionX";
			this.txtMousePositionX.Size = new System.Drawing.Size(79, 20);
			this.txtMousePositionX.TabIndex = 1;
			this.txtMousePositionX.Text = "0";
			// 
			// lblX
			// 
			this.lblX.AutoSize = true;
			this.lblX.Location = new System.Drawing.Point(269, 41);
			this.lblX.Name = "lblX";
			this.lblX.Size = new System.Drawing.Size(14, 13);
			this.lblX.TabIndex = 2;
			this.lblX.Text = "X";
			// 
			// lblY
			// 
			this.lblY.AutoSize = true;
			this.lblY.Location = new System.Drawing.Point(268, 15);
			this.lblY.Name = "lblY";
			this.lblY.Size = new System.Drawing.Size(14, 13);
			this.lblY.TabIndex = 2;
			this.lblY.Text = "Y";
			// 
			// btnGetPosition
			// 
			this.btnGetPosition.Location = new System.Drawing.Point(272, 64);
			this.btnGetPosition.Name = "btnGetPosition";
			this.btnGetPosition.Size = new System.Drawing.Size(96, 44);
			this.btnGetPosition.TabIndex = 3;
			this.btnGetPosition.Text = "Set Start Pos";
			this.btnGetPosition.UseVisualStyleBackColor = true;
			this.btnGetPosition.Click += new System.EventHandler(this.btnGetPosition_Click);
			// 
			// txtActions
			// 
			this.txtActions.Location = new System.Drawing.Point(198, 12);
			this.txtActions.Multiline = true;
			this.txtActions.Name = "txtActions";
			this.txtActions.Size = new System.Drawing.Size(64, 394);
			this.txtActions.TabIndex = 4;
			// 
			// btnPlay
			// 
			this.btnPlay.BackColor = System.Drawing.Color.Green;
			this.btnPlay.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnPlay.ForeColor = System.Drawing.Color.White;
			this.btnPlay.Location = new System.Drawing.Point(272, 362);
			this.btnPlay.Name = "btnPlay";
			this.btnPlay.Size = new System.Drawing.Size(96, 44);
			this.btnPlay.TabIndex = 3;
			this.btnPlay.Text = "PLAY";
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
			this.btnLoadImage.Location = new System.Drawing.Point(7, 152);
			this.btnLoadImage.Name = "btnLoadImage";
			this.btnLoadImage.Size = new System.Drawing.Size(185, 44);
			this.btnLoadImage.TabIndex = 3;
			this.btnLoadImage.Text = "LOAD";
			this.btnLoadImage.UseVisualStyleBackColor = false;
			this.btnLoadImage.Click += new System.EventHandler(this.btnLoadImage_Click);
			// 
			// dlgImportImage
			// 
			this.dlgImportImage.Filter = "JPG File|*.jpg|PNG Files|*.png";
			// 
			// intExpand
			// 
			this.intExpand.DecimalPlaces = 1;
			this.intExpand.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
			this.intExpand.Location = new System.Drawing.Point(272, 124);
			this.intExpand.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.intExpand.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
			this.intExpand.Name = "intExpand";
			this.intExpand.Size = new System.Drawing.Size(96, 20);
			this.intExpand.TabIndex = 5;
			this.intExpand.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
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
			this.label5.Location = new System.Drawing.Point(102, 15);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(30, 13);
			this.label5.TabIndex = 16;
			this.label5.Text = "MAX";
			// 
			// intRedMin
			// 
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
			this.intRedMin.Size = new System.Drawing.Size(47, 20);
			this.intRedMin.TabIndex = 5;
			this.intRedMin.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
			// 
			// intRedMax
			// 
			this.intRedMax.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this.intRedMax.Location = new System.Drawing.Point(97, 34);
			this.intRedMax.Maximum = new decimal(new int[] {
            256,
            0,
            0,
            0});
			this.intRedMax.Name = "intRedMax";
			this.intRedMax.Size = new System.Drawing.Size(47, 20);
			this.intRedMax.TabIndex = 5;
			this.intRedMax.Value = new decimal(new int[] {
            256,
            0,
            0,
            0});
			// 
			// intGreenMax
			// 
			this.intGreenMax.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this.intGreenMax.Location = new System.Drawing.Point(97, 60);
			this.intGreenMax.Maximum = new decimal(new int[] {
            256,
            0,
            0,
            0});
			this.intGreenMax.Name = "intGreenMax";
			this.intGreenMax.Size = new System.Drawing.Size(47, 20);
			this.intGreenMax.TabIndex = 18;
			this.intGreenMax.Value = new decimal(new int[] {
            256,
            0,
            0,
            0});
			// 
			// intGreenMin
			// 
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
			this.intGreenMin.Size = new System.Drawing.Size(47, 20);
			this.intGreenMin.TabIndex = 17;
			this.intGreenMin.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
			// 
			// intBlueMax
			// 
			this.intBlueMax.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this.intBlueMax.Location = new System.Drawing.Point(97, 86);
			this.intBlueMax.Maximum = new decimal(new int[] {
            256,
            0,
            0,
            0});
			this.intBlueMax.Name = "intBlueMax";
			this.intBlueMax.Size = new System.Drawing.Size(47, 20);
			this.intBlueMax.TabIndex = 20;
			this.intBlueMax.Value = new decimal(new int[] {
            256,
            0,
            0,
            0});
			// 
			// intBlueMin
			// 
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
			this.intBlueMin.Size = new System.Drawing.Size(47, 20);
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
			this.chkRedActive.Location = new System.Drawing.Point(161, 37);
			this.chkRedActive.Name = "chkRedActive";
			this.chkRedActive.Size = new System.Drawing.Size(15, 14);
			this.chkRedActive.TabIndex = 21;
			this.chkRedActive.UseVisualStyleBackColor = true;
			// 
			// chkGreenActive
			// 
			this.chkGreenActive.AutoSize = true;
			this.chkGreenActive.Location = new System.Drawing.Point(161, 63);
			this.chkGreenActive.Name = "chkGreenActive";
			this.chkGreenActive.Size = new System.Drawing.Size(15, 14);
			this.chkGreenActive.TabIndex = 21;
			this.chkGreenActive.UseVisualStyleBackColor = true;
			// 
			// chkBlueActive
			// 
			this.chkBlueActive.AutoSize = true;
			this.chkBlueActive.Location = new System.Drawing.Point(161, 89);
			this.chkBlueActive.Name = "chkBlueActive";
			this.chkBlueActive.Size = new System.Drawing.Size(15, 14);
			this.chkBlueActive.TabIndex = 21;
			this.chkBlueActive.UseVisualStyleBackColor = true;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(147, 15);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(45, 13);
			this.label6.TabIndex = 16;
			this.label6.Text = "ACTIVE";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(4, 124);
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
			this.intMaxLightness.Size = new System.Drawing.Size(104, 20);
			this.intMaxLightness.TabIndex = 20;
			this.intMaxLightness.Value = new decimal(new int[] {
            600,
            0,
            0,
            0});
			// 
			// Main
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(378, 422);
			this.Controls.Add(this.chkBlueActive);
			this.Controls.Add(this.chkGreenActive);
			this.Controls.Add(this.chkRedActive);
			this.Controls.Add(this.intMaxLightness);
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
			this.Controls.Add(this.intExpand);
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
			((System.ComponentModel.ISupportInitialize)(this.intExpand)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.intRedMin)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.intRedMax)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.intGreenMax)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.intGreenMin)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.intBlueMax)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.intBlueMin)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.intMaxLightness)).EndInit();
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
		private System.Windows.Forms.NumericUpDown intExpand;
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
	}
}

