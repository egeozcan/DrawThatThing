using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace DrawThatThing
{
	using System.Collections.Generic;
	using System.Drawing;
	using System.Globalization;
	using System.Runtime.InteropServices;
	using System.Threading;

	using LowLevelTools;

	public partial class Main : Form
	{
		[DllImport("user32.dll")]
		public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
		[DllImport("user32.dll")]
		public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

		public Main()
		{
			InitializeComponent();
			RegisterHotKey(this.Handle, this.GetType().GetHashCode(), 5, 'C');
		}

		private void btnGetPosition_Click(object sender, EventArgs e)
		{
			var cursorPosition = MouseOperations.GetCursorPosition();
			txtMousePositionX.Text = cursorPosition.X.ToString(CultureInfo.InvariantCulture);
			txtMousePositionY.Text = cursorPosition.Y.ToString(CultureInfo.InvariantCulture);
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
			workerClickAround.RunWorkerAsync(
				new ClickerArguments
					{
						coordinateString = this.txtActions.Text,
						xStart = int.Parse(this.txtMousePositionX.Text),
						yStart = int.Parse(this.txtMousePositionY.Text),
						expand = intExpand.Value,
						sleepBetween = (int)intWaitBetween.Value
					});
		}

		private struct ClickerArguments
		{
			public int xStart;
			public int yStart;
			public decimal expand;
			public string coordinateString;
			public int sleepBetween;
		}

		private void workerClickAround_DoWork(object sender, DoWorkEventArgs e)
		{
			var args = (ClickerArguments)e.Argument;
			var xStart = args.xStart;
			var yStart = args.yStart;
			var sleepBetween = args.sleepBetween;
			var coordinates = GetCoordinates(args.coordinateString);
			foreach (var coordinate in coordinates.TakeWhile(coordinate => !this.workerClickAround.CancellationPending))
			{
				try
				{
					MouseOperations.SetCursorPosition(
						(int)(xStart + args.expand * coordinate[1]),
						(int)(yStart + args.expand * coordinate[0]));
					MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftDown);
					if (coordinate.Length == 4)
					{
						Thread.Sleep(sleepBetween);
						MouseOperations.SetCursorPosition(
							(int)(xStart + args.expand * coordinate[3]),
							(int)(yStart + args.expand * coordinate[2]));
					}
					Thread.Sleep(10);
					MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp);
					Thread.Sleep(sleepBetween);
				}
				catch (Exception)
				{
					break;
				}
			}
		}
		private static IEnumerable<int[]> GetCoordinates(String coordinateString)
		{
			if (String.IsNullOrEmpty(coordinateString))
			{
				return new int[][] { };
			}
			try
			{
				return coordinateString
					.Split('\n')
					.AsParallel()
					.Select(line => line.Split(' ').AsParallel().Select(int.Parse).ToArray())
					.ToArray();
			}
			catch (Exception)
			{
				MessageBox.Show("An error occured when parsing.");
				return new int[][] { };
			}
			
		}

		private void btnLoadImage_Click(object sender, EventArgs e)
		{
			dlgImportImage.ShowDialog();
			if (!this.dlgImportImage.CheckFileExists)
			{
				return;
			}
			var reader = new BitmapReader();
			reader.Read(this.dlgImportImage.FileName);
			var args = this.CreateAcceptanceArgs();
			var filteredPixels = reader.getFilteredPixels(args);
			this.txtActions.Text = String.Join(Environment.NewLine, filteredPixels.Select(x => x.ToString()));
			dlgImportImage.FileName = null;
			this.updatePreview();
		}

		private BitmapReader.PixelAcceptanceArgs CreateAcceptanceArgs()
		{
			return new BitmapReader.PixelAcceptanceArgs
				{
					RedEnabled = this.chkRedActive.Checked,
					RedMax = (int)this.intRedMax.Value,
					RedMin = (int)this.intRedMin.Value,
					BlueEnabled = this.chkBlueActive.Checked,
					BlueMax = (int)this.intBlueMax.Value,
					BlueMin = (int)this.intBlueMin.Value,
					GreenEnabled = this.chkGreenActive.Checked,
					GreenMax = (int)this.intGreenMax.Value,
					GreenMin = (int)this.intGreenMin.Value,
					MaxLight = (int)this.intMaxLightness.Value
				};
		}

		private void btnLoadBitmap_Click(object sender, EventArgs e)
		{
			this.updatePreview();
		}

		private void updatePreview()
		{
			var bitmap = new Bitmap(pctPreview.Width, pctPreview.Height);
			Pen blackPen = new Pen(Color.Black, 3);
			using (var graphics = Graphics.FromImage(bitmap))
			{
				foreach (var c in GetCoordinates(txtActions.Text))
				{
					if (c.Length == 4)
					{
						graphics.DrawLine(blackPen, c[0], c[1], c[2], c[3]);
					}
					else
					{
						graphics.DrawLine(blackPen, c[0], c[1], c[0], c[1]);
					}
				}
			}
			pctPreview.Image = bitmap;
		}
	}
}