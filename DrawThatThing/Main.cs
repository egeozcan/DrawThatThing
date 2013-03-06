using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace DrawThatThing
{
	using System.Collections.Generic;
	using System.Drawing;
	using System.Drawing.Imaging;
	using System.Globalization;
	using System.IO;
	using System.Runtime.InteropServices;
	using System.Text;

	using LowLevelTools;

	public partial class DrawThatThing : Form
	{
		[DllImport("user32.dll")]
		public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
		[DllImport("user32.dll")]
		public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

		private IEnumerable<MouseDragAction> _actions;
		private readonly ColorSpot[] _builtinColorSpots = new[]
			{
				new ColorSpot
					{
						color = Color.Black,
						point = new Point(-1884,1025)
					},
				new ColorSpot
					{
						color = Color.Red,
						point = new Point(-1850,1038)
					},
				new ColorSpot
					{
						color = Color.Blue,
						point = new Point(-1788,1037)
					},
				new ColorSpot
					{
						color = Color.Green,
						point = new Point(-1819,1021)
					},
				new ColorSpot
					{
						color = Color.White,
						point = new Point(0,0)
					},
				new ColorSpot
					{
						color = Color.FromArgb(190, 169, 16),
						point = new Point(-1673,1039)
					}
			};

		public DrawThatThing()
		{
			InitializeComponent();
			RegisterHotKey(this.Handle, this.GetType().GetHashCode(), 5, 'C');
			this.Text = "DrawThatThing";
			foreach (var colorSpot in _builtinColorSpots)
			{
				dataGridColors.Rows.Add(colorSpot.point.X.ToString(CultureInfo.InvariantCulture), colorSpot.point.Y.ToString(CultureInfo.InvariantCulture), colorSpot.color.ToHEX());				
			}
		}

		private void btnGetPosition_Click(object sender, EventArgs e)
		{
			SetControlTextToMousePosition(this.txtMousePositionX, this.txtMousePositionY);
		}

		private static void SetControlTextToMousePosition(Control boxX, Control boxY)
		{
			var cursorPosition = MouseOperations.GetCursorPosition();
			boxX.Text = cursorPosition.X.ToString(CultureInfo.InvariantCulture);
			boxY.Text = cursorPosition.Y.ToString(CultureInfo.InvariantCulture);
		}

		protected override void WndProc(ref Message m)
		{
			if (m.Msg == 0x0312)
			{
				this.HandleHotkey();
			}
			base.WndProc(ref m);
		}

		private void HandleHotkey()
		{
			if (workerClickAround.IsBusy)
			{
				workerClickAround.CancelAsync();
			}
		}

		private void btnPlay_Click(object sender, EventArgs e)
		{
			StartClicking();
		}

		private void StartClicking()
		{
			if (_actions == null)
			{
				return;
			}
			workerClickAround.RunWorkerAsync(
				new ClickerArguments
					{
						mouseActions = this._actions,
						offset = new Point(txtMousePositionX.Text.ToInt(), txtMousePositionY.Text.ToInt())
					});
		}

		private struct ClickerArguments
		{
			public IEnumerable<MouseDragAction> mouseActions;
			public Point offset;
		}

		private void workerClickAround_DoWork(object sender, DoWorkEventArgs e)
		{
			var args = (ClickerArguments)e.Argument;
			foreach (var action in args.mouseActions.TakeWhile(coordinate => !this.workerClickAround.CancellationPending))
			{
				action.Play(args.offset);
			}
		}

		private void btnLoadImage_Click(object sender, EventArgs e)
		{
			dlgImportImage.ShowDialog();
			if (!this.dlgImportImage.CheckFileExists)
			{
				return;
			}
			using (var bitmap = new Bitmap(this.dlgImportImage.FileName))
			{
				this.intPreviewHeight.Value = bitmap.Height;
				this.intPreviewWidth.Value = bitmap.Width;
			}
			ColorSpot[] spots = new ColorSpot[dataGridColors.Rows.Count];
			for (int i = 0; i < dataGridColors.Rows.Count; i++)
			{
				var row = dataGridColors.Rows[i];
				spots[i] = new ColorSpot
					{
						color = (row.Cells[2].Value as String ?? "").ToColor(),
						point = new Point((row.Cells[0].Value as String ?? "").ToInt(), (row.Cells[1].Value as String ?? "").ToInt())
					};
			}
			var notEmptyColors = spots.Where(x => !x.color.IsEmpty).ToArray();
			this._actions = new ColoredBitmapReader(this.dlgImportImage.FileName).getDrawInstructions(notEmptyColors);
			dlgImportImage.FileName = null;
			this.updatePreview();
		}

		private void updatePreview()
		{
			var bitmap = new Bitmap((int)this.intPreviewWidth.Value, (int)this.intPreviewHeight.Value);
			Pen pen = new Pen(Color.Black, 1);
			using (var graphics = Graphics.FromImage(bitmap))
			{
				foreach (var action in _actions)
				{
					var cPen = action.Color.IsEmpty ? pen : new Pen(action.Color, 1);
					pen = cPen;
					Point[] points = action.Points;
					Point lastP = points[0];
					foreach (var point in points)
					{
						graphics.DrawLine(cPen, lastP, point);
						lastP = point;
					}
				}
			}
			pctPreview.Width = bitmap.Width;
			pctPreview.Height = bitmap.Height;
			pctPreview.Image = bitmap;
		}

		private void btnBringUp_Click(object sender, EventArgs e)
		{
			if (dataGridColors.SelectedRows.Count > 0)
			{
				var selectedRow = dataGridColors.SelectedRows[0];
				selectedRow.SetValues(txtMousePositionX.Text, txtMousePositionY.Text);
				if (dataGridColors.Rows.Count > selectedRow.Index + 1)
				{
					dataGridColors.Rows[selectedRow.Index + 1].Selected = true;
				}
			}
		}

		private void btnExport_Click(object sender, EventArgs e)
		{
			dlgExportPixels.ShowDialog();
			var fileName = dlgExportPixels.FileName;
			if (String.IsNullOrWhiteSpace(fileName))
			{
				return;
			}
			var sb = new StringBuilder();
			sb.AppendLine("X;Y;RGB");
			foreach (DataGridViewRow row in dataGridColors.Rows)
			{
				sb.AppendLine(String.Format("{0};{1};{2}", row.Cells[0].Value, row.Cells[1].Value, row.Cells[2].Value));
			}
			File.WriteAllText(fileName, sb.ToString());
		}

		private void btnImport_Click(object sender, EventArgs e)
		{
			dlgImportPixels.ShowDialog();
			var fileName = dlgImportPixels.FileName;
			if (String.IsNullOrWhiteSpace(fileName))
			{
				return;
			}
			IEnumerable<string[]> values;
			try
			{
				values = File.ReadAllLines(fileName).Skip(1).Select(l => l.Split(";".ToCharArray()));
			}
			catch (Exception)
			{
				return;
			}
			if (!values.Any())
			{
				return;
			}
			for (int i = 0; i < dataGridColors.Rows.Count; i++)
			{
				if (dataGridColors.Rows[i].IsNewRow)
				{
					continue;
				}
				dataGridColors.Rows.RemoveAt(i);
			}
			foreach (var value in values)
			{
				dataGridColors.Rows.Add(value);
			}
		}
	}
}