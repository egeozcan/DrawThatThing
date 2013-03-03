using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace DrawThatThing
{
	using System.Collections.Generic;
	using System.Drawing;
	using System.Drawing.Drawing2D;
	using System.Globalization;
	using System.Runtime.InteropServices;

	using LowLevelTools;

	public partial class Main : Form
	{
		[DllImport("user32.dll")]
		public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
		[DllImport("user32.dll")]
		public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

		private IEnumerable<MouseAction> _actions; 

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
			if (_actions == null)
			{
				return;
			}
			workerClickAround.RunWorkerAsync(
				new ClickerArguments
					{
						mouseActions = this._actions,
						offset = new Point(txtMousePositionX.Text.AsInt(), txtMousePositionY.Text.AsInt())
					});
		}

		private struct ClickerArguments
		{
			public IEnumerable<MouseAction> mouseActions;
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
			var reader = new BitmapReader();
			reader.Read(this.dlgImportImage.FileName);
			using (var bitmap = new Bitmap(this.dlgImportImage.FileName))
			{
				this.intPreviewHeight.Value = bitmap.Height;
				this.intPreviewWidth.Value = bitmap.Width;
			}
			this._actions = (new ColoredBitmapReader(this.dlgImportImage.FileName)).getDrawInstructions(new[]
			    {
			        new ColorSpot
			        	{
			        		color = Color.Black,
							point = new Point(1025, -1883)
			        	},
					new ColorSpot
						{
							color = Color.Red,
							point = new Point(1039, -1850)
						}
			    });
			dlgImportImage.FileName = null;
			this.updatePreview();
		}

		private void updatePreview()
		{
			var bitmap = new Bitmap((int)this.intPreviewWidth.Value, (int)this.intPreviewHeight.Value);
			Pen blackPen = new Pen(Color.Black, 3);
			using (var graphics = Graphics.FromImage(bitmap))
			{
				foreach (var action in _actions)
				{
					Point lastP = Point.Empty;
					foreach (var point in action._points.Select(x => x.Point.HasValue ? x.Point.Value : Point.Empty))
					{
						if (lastP == Point.Empty)
						{
							lastP = point;
						}
						graphics.DrawLine(blackPen, lastP, point);
						lastP = point;
					}
				}
			}
			pctPreview.Width = bitmap.Width;
			pctPreview.Height = bitmap.Height;
			pctPreview.Image = bitmap;
		}
	}
}