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

	using Helpers;
	using Helpers.BitmapReaders;
	using Helpers.BitmapReaders.Interface;
	using Helpers.Classes;
	using Helpers.Extensions;

	using Point = Helpers.Point;

	public partial class DrawThatThing : Form
	{
		#region Constants and Fields

		private readonly ColorSpot[] _builtinColorSpots = new[]
			{
				new ColorSpot { Color = Color.Black, Point = new Point(-1884, 1025) },
				new ColorSpot { Color = Color.Red, Point = new Point(-1850, 1038) },
				new ColorSpot { Color = Color.Blue, Point = new Point(-1788, 1037) },
				new ColorSpot { Color = Color.Green, Point = new Point(-1819, 1021) },
				new ColorSpot { Color = Color.White, Point = new Point(0, 0) },
				new ColorSpot { Color = Color.FromArgb(190, 169, 16), Point = new Point(-1673, 1039) }
			};
		private IEnumerable<MouseDragAction> _actions;

		#endregion

		#region Constructors and Destructors

		public DrawThatThing()
		{
			this.InitializeComponent();
			RegisterHotKey(this.Handle, this.GetType().GetHashCode(), 5, 'C');
			foreach (ColorSpot colorSpot in this._builtinColorSpots)
			{
				this.dataGridColors.Rows.Add(
					colorSpot.Point.X.ToString(CultureInfo.InvariantCulture),
					colorSpot.Point.Y.ToString(CultureInfo.InvariantCulture),
					colorSpot.Color.ToHEX());
			}
		}

		#endregion

		#region Public Methods

		[DllImport("user32.dll")]
		public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
		[DllImport("user32.dll")]
		public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

		#endregion

		#region Methods

		protected override void WndProc(ref Message m)
		{
			if (m.Msg == 0x0312)
			{
				this.HandleHotkey();
			}
			base.WndProc(ref m);
		}
		private static void SetControlTextToMousePosition(Control boxX, Control boxY)
		{
			MouseOperations.MousePoint cursorPosition = MouseOperations.GetCursorPosition();
			boxX.Text = cursorPosition.X.ToString(CultureInfo.InvariantCulture);
			boxY.Text = cursorPosition.Y.ToString(CultureInfo.InvariantCulture);
		}

		private void HandleHotkey()
		{
			if (this.workerClickAround.IsBusy)
			{
				this.workerClickAround.CancelAsync();
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

		private void btnBringUp_Click(object sender, EventArgs e)
		{
			if (this.dataGridColors.SelectedRows.Count > 0)
			{
				DataGridViewRow selectedRow = this.dataGridColors.SelectedRows[0];
				selectedRow.SetValues(this.txtMousePositionX.Text, this.txtMousePositionY.Text);
				if (this.dataGridColors.Rows.Count > selectedRow.Index + 1)
				{
					this.dataGridColors.Rows[selectedRow.Index + 1].Selected = true;
				}
			}
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
		private void btnGetPosition_Click(object sender, EventArgs e)
		{
			SetControlTextToMousePosition(this.txtMousePositionX, this.txtMousePositionY);
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
			using (var bitmap = new Bitmap(this.dlgImportImage.FileName))
			{
				this.intPreviewHeight.Value = bitmap.Height;
				this.intPreviewWidth.Value = bitmap.Width;
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
			List<ColorSpot> notEmptyColors = spots.Where(x => !x.Color.IsEmpty).ToList();

			IColoredBitmapReader reader;
			if (chkUseAlternativeParser.Checked)
			{
				reader = new AlternativeColoredBitmapReader(this.dlgImportImage.FileName);
			}
			else
			{
				reader = new ColoredBitmapReader(this.dlgImportImage.FileName);				
			}
			this._actions = reader.getDrawInstructions(notEmptyColors);
			this.dlgImportImage.FileName = null;
			this.updatePreview();
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
		private void workerClickAround_DoWork(object sender, DoWorkEventArgs e)
		{
			var args = (ClickerArguments)e.Argument;
			foreach (
				MouseDragAction action in args.mouseActions.TakeWhile(coordinate => !this.workerClickAround.CancellationPending))
			{
				action.Play(args.offset);
			}
		}

		#endregion

		private struct ClickerArguments
		{
			public IEnumerable<MouseDragAction> mouseActions;
			public Point offset;
		}
	}
}