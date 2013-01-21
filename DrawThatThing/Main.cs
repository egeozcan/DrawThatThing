using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace DrawThatThing
{
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
						expand = intExpand.Value
					});
		}

		private struct ClickerArguments
		{
			public int xStart;
			public int yStart;
			public decimal expand;
			public string coordinateString;
		}

		private void workerClickAround_DoWork(object sender, DoWorkEventArgs e)
		{
			var args = (ClickerArguments)e.Argument;
			var xStart = args.xStart;
			var yStart = args.yStart;
			foreach (var coordinates in
				args.coordinateString.Split('\n').Select(line => line.Split(' ')).TakeWhile(
					coordinates => !this.workerClickAround.CancellationPending))
			{
				try
				{
					MouseOperations.SetCursorPosition(
						(int)(xStart + args.expand * int.Parse(coordinates[1])),
						(int)(yStart + args.expand * int.Parse(coordinates[0])));
					MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftDown);
					if (coordinates.Length == 4)
					{
						Thread.Sleep(10);
						MouseOperations.SetCursorPosition(
							(int)(xStart + args.expand * int.Parse(coordinates[3])),
							(int)(yStart + args.expand * int.Parse(coordinates[2])));
						Thread.Sleep(10);
					}
					MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp);
					Thread.Sleep(10);
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
			var args = new BitmapReader.PixelAcceptanceArgs
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
			this.txtActions.Text = reader.getNonWhite(args);
			dlgImportImage.FileName = null;
		}
	}
}