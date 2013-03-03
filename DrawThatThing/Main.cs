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
						mouseActions = c,
						offset = 
					});
		}

		private struct ClickerArguments
		{
			public List<MouseAction> mouseActions;
			public Point offset;
		}

		private void workerClickAround_DoWork(object sender, DoWorkEventArgs e)
		{
			var args = (ClickerArguments)e.Argument;
			var xStart = args.xStart;
			var yStart = args.yStart;
			var sleepBetween = args.sleepBetween;
			foreach (var coordinate in coordinates.TakeWhile(coordinate => !this.workerClickAround.CancellationPending))
			{
				try
				{
					MouseOperations.SetCursorPosition(
						xStart + coordinate[0],
						yStart + coordinate[1]);
					MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftDown);
					if (coordinate.Length == 4)
					{
						Thread.Sleep(sleepBetween);
						MouseOperations.SetCursorPosition(
							xStart + coordinate[2],
							yStart + coordinate[3]);
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

		private void btnLoadImage_Click(object sender, EventArgs e)
		{
			dlgImportImage.ShowDialog();
			if (!this.dlgImportImage.CheckFileExists)
			{
				return;
			}
			var reader = new BitmapReader();
			reader.Read(this.dlgImportImage.FileName);
			using (var bitmap = new Bitmap(this.dlgImportImage.FileName))
			{
				this.intPreviewHeight.Value = bitmap.Height;
				this.intPreviewWidth.Value = bitmap.Width;
			}
			var args = this.CreateAcceptanceArgs();
			var filteredPixels = reader.getFilteredPixels(args);
			//(new ColoredBitmapReader(this.dlgImportImage.FileName)).getDrawInstructions(args, new[]
			//    {
			//        new ColorSpot() 
			//    });
			this.txtActions.Text = String.Join(Environment.NewLine, filteredPixels.Select(x => x.ToString()));
			dlgImportImage.FileName = null;
			this.updatePreview();
		}

		private PixelAcceptanceArgs CreateAcceptanceArgs()
		{
			return new PixelAcceptanceArgs
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
			var bitmap = new Bitmap((int)this.intPreviewWidth.Value, (int)this.intPreviewHeight.Value);
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
			pctPreview.Width = bitmap.Width;
			Console.WriteLine(pctPreview.Width);
			Console.WriteLine(pctPreview.Height);
			Console.WriteLine(bitmap.Width);
			Console.WriteLine(bitmap.Height);
			pctPreview.Height = bitmap.Height;
			pctPreview.Image = bitmap;
		}
	}
}