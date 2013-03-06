namespace DrawThatThing
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Globalization;
	using System.IO;
	using System.Linq;
	using System.Runtime.InteropServices;
	using System.Text;
	using System.Windows.Forms;

	using BitmapReader;
	using BitmapReader.BitmapReaders;
	using BitmapReader.BitmapReaders.Interface;
	using BitmapReader.Classes;
	using BitmapReader.Extensions;

	using Internal;

	using Point = BitmapReader.Point;

	public partial class DrawThatThing : Form
	{
		private IEnumerable<MouseDragAction> _actions;

		public DrawThatThing()
		{
			this.InitializeComponent();
			//Shift + Alt + C
			RegisterHotKey(this.Handle, 0, 5, 'C');
			//Shift + Alt + P
			RegisterHotKey(this.Handle, 1, 5, 'S');
			//Shift + Alt + S
			RegisterHotKey(this.Handle, 2, 5, 'P');
		}

		[DllImport("user32.dll")]
		public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
		[DllImport("user32.dll")]
		public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

		protected override void WndProc(ref Message m)
		{
			if (m.Msg == 0x0312)
			{
				this.HandleHotkey(m.WParam.ToInt32());
			}
			base.WndProc(ref m);
		}
		private static void SetControlTextToMousePosition(Control boxX, Control boxY)
		{
			MouseOperations.MousePoint cursorPosition = MouseOperations.GetCursorPosition();
			boxX.Text = cursorPosition.X.ToString(CultureInfo.InvariantCulture);
			boxY.Text = cursorPosition.Y.ToString(CultureInfo.InvariantCulture);
		}

		private void AddCurrentMousePositionToPalette()
		{
			MouseOperations.MousePoint mouse = MouseOperations.GetCursorPosition();
			using (Bitmap bitmap = new Bitmap(1, 1))
			{
				using (Graphics g = Graphics.FromImage(bitmap))
				{
					g.CopyFromScreen(new System.Drawing.Point(mouse.X, mouse.Y), new System.Drawing.Point(0, 0), bitmap.Size);
					var color = bitmap.GetPixel(0, 0);
					dataGridColors.Rows.Add(mouse.X, mouse.Y, color.ToHEX());
				}
			}
		}
		private void HandleHotkey(int id)
		{
			
				switch (id)
				{
					case 0:
						if (this.workerClickAround.IsBusy)
						{
							this.workerClickAround.CancelAsync();
						}
						break;
					case 1:
						SetControlTextToMousePosition(this.txtMousePositionX, this.txtMousePositionY);
						break;
					case 2:
						this.AddCurrentMousePositionToPalette();
						break;
				}
		}

		private void StartClicking()
		{
			if (this._actions == null)
			{
				return;
			}
			this.workerClickAround.RunWorkerAsync(
				new ClickerArguments
					{
						mouseActions = this._actions,
						offset = new Point(this.txtMousePositionX.Text.ToInt(), this.txtMousePositionY.Text.ToInt())
					});
		}

		private void btnExport_Click(object sender, EventArgs e)
		{
			this.dlgExportPixels.ShowDialog();
			string fileName = this.dlgExportPixels.FileName;
			if (String.IsNullOrWhiteSpace(fileName))
			{
				return;
			}
			var sb = new StringBuilder();
			sb.AppendLine("X;Y;RGB");
			foreach (DataGridViewRow row in this.dataGridColors.Rows)
			{
				sb.AppendLine(String.Format("{0};{1};{2}", row.Cells[0].Value, row.Cells[1].Value, row.Cells[2].Value));
			}
			File.WriteAllText(fileName, sb.ToString());
		}

		private void btnImport_Click(object sender, EventArgs e)
		{
			this.dlgImportPixels.ShowDialog();
			string fileName = this.dlgImportPixels.FileName;
			if (String.IsNullOrWhiteSpace(fileName))
			{
				return;
			}
			IEnumerable<string[]> values;
			try
			{
				values = File.ReadAllLines(fileName).Skip(1).Select(l => l.Split(";".ToCharArray())).ToList();
			}
			catch (Exception)
			{
				return;
			}
			if (!values.Any())
			{
				return;
			}
			for (int i = 0; i < this.dataGridColors.Rows.Count; i++)
			{
				if (this.dataGridColors.Rows[i].IsNewRow)
				{
					continue;
				}
				this.dataGridColors.Rows.RemoveAt(i);
			}
			foreach (object[] value in values)
			{
				this.dataGridColors.Rows.Add(value);
			}
		}

		private void btnLoadImage_Click(object sender, EventArgs e)
		{
			this.dlgImportImage.ShowDialog();
			if (!this.dlgImportImage.CheckFileExists)
			{
				return;
			}
			string imagepath = this.dlgImportImage.FileName;
			if (String.IsNullOrWhiteSpace(imagepath))
			{
				return;
			}
			var spots = new ColorSpot[this.dataGridColors.Rows.Count];
			for (int i = 0; i < this.dataGridColors.Rows.Count; i++)
			{
				DataGridViewRow row = this.dataGridColors.Rows[i];
				spots[i] = new ColorSpot
					{
						Color = (row.Cells[2].Value as String ?? "").ToColor(),
						Point = new Point((row.Cells[0].Value as String ?? "").ToInt(), (row.Cells[1].Value as String ?? "").ToInt())
					};
			}
			List<ColorSpot> colorPalette = spots.Where(x => !x.Color.IsEmpty).ToList();
			this.workerCalculate.RunWorkerAsync(
				new CalculatorArguments
					{ useAlternativeParser = this.chkUseAlternativeParser.Checked, ColorPalette = colorPalette, imagepath = imagepath });
			this.dlgImportImage.FileName = null;
		}

		private void btnPlay_Click(object sender, EventArgs e)
		{
			this.StartClicking();
		}

		private void updatePreview()
		{
			var bitmap = new Bitmap((int)this.intPreviewWidth.Value, (int)this.intPreviewHeight.Value);
			var pen = new Pen(Color.Black, 1);
			using (Graphics graphics = Graphics.FromImage(bitmap))
			{
				foreach (MouseDragAction action in this._actions)
				{
					Pen cPen = action.Color.IsEmpty ? pen : new Pen(action.Color, 1);
					pen = cPen;
					List<Point> points = action.Points;
					Point lastP = points[0];
					foreach (Point point in points)
					{
						graphics.DrawLine(cPen, lastP.ToStandardPoint(), point.ToStandardPoint());
						lastP = point;
					}
				}
			}
			this.pctPreview.Width = bitmap.Width;
			this.pctPreview.Height = bitmap.Height;
			this.pctPreview.Image = bitmap;
		}

		private void workerCalculate_DoWork(object sender, DoWorkEventArgs e)
		{
			var args = e.Argument as CalculatorArguments;
			var results = new CalculatorResults();
			if (args == null)
			{
				return;
			}
			using (var bitmap = new Bitmap(args.imagepath))
			{
				results.imageHeight = bitmap.Height;
				results.imageWidth = bitmap.Width;
			}
			IColoredBitmapReader reader;
			if (args.useAlternativeParser)
			{
				reader = new AlternativeColoredBitmapReader(args.imagepath);
			}
			else
			{
				reader = new ColoredBitmapReader(args.imagepath);
			}
			try
			{
				results.actions = reader.getDrawInstructions(args.ColorPalette).ToList();
			}
			catch (Exception err)
			{
				results.error = err;
			}
			e.Result = results;
		}

		private void workerCalculate_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			var results = e.Result as CalculatorResults;
			if (results == null)
			{
				return;
			}
			if (results.error != null)
			{
				MessageBox.Show(results.error.Message);
				return;
			}
			this._actions = results.actions;
			this.intPreviewHeight.Value = results.imageHeight;
			this.intPreviewWidth.Value = results.imageWidth;
			this.updatePreview();
		}
		private void workerClickAround_DoWork(object sender, DoWorkEventArgs e)
		{
			var args = e.Argument as ClickerArguments;
			if (args == null)
			{
				return;
			}
			foreach (
				MouseDragAction action in args.mouseActions.TakeWhile(coordinate => !this.workerClickAround.CancellationPending))
			{
				action.Play(args.offset);
			}
		}
	}
}