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

using BitmapReader.Classes;
using BitmapReader.Extensions;
using BitmapReader.Interface;
using DrawThatThing.Internal;
using Point = BitmapReader.Classes.Point;

namespace DrawThatThing
{
	using BitmapReader.Attributes;
	using BitmapReader.Win32;

	using Point = Point;

	public partial class DrawThatThing : Form
	{
		private const string BitmapreaderInterface = "BitmapReader.Interface.IBitmapReader";
		private IEnumerable<MouseDragAction> _actions;
		private readonly Dictionary<string, Type> _plugins = new Dictionary<string, Type>();

		public DrawThatThing()
		{
			InitializeComponent();
			//Shift + Alt + C
			RegisterHotKey(Handle, 0, 5, 'C');
			//Shift + Alt + S
			RegisterHotKey(Handle, 1, 5, 'S');
			//Shift + Alt + A
			RegisterHotKey(Handle, 2, 5, 'A');

			var dir = Path.Combine(Environment.CurrentDirectory, "Plugins");
			var pluginFiles = Directory.GetFiles(dir, "*.dll");
			foreach (var pluginFile in pluginFiles)
			{
				var pluginCandidate = System.Reflection.Assembly.LoadFrom(pluginFile);
				var pluginCandidateTypes = pluginCandidate.GetTypes();
				foreach (var pluginCandidateType in pluginCandidateTypes)
				{
					if (pluginCandidateType.GetInterface(BitmapreaderInterface) == null)
					{
						continue;
					}
					var pluginName = Path.GetFileName(pluginFile);
					if (pluginName != null && !String.IsNullOrWhiteSpace(pluginFile) && !_plugins.ContainsKey(pluginName))
					{
						_plugins.Add(pluginName, pluginCandidateType);                            
					}
				}
			}
			foreach (var plugin in _plugins)
			{
				cmbParser.Items.Add(plugin.Key);
			}
			if (cmbParser.Items.Count > 0)
			{
				cmbParser.SelectedIndex = 0;
			}
		}

		[DllImport("user32.dll")]
		public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);

		[DllImport("user32.dll")]
		public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

		protected override void WndProc(ref Message m)
		{
			if (m.Msg == 0x0312)
			{
				HandleHotkey(m.WParam.ToInt32());
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
			using (var bitmap = new Bitmap(1, 1))
			{
				using (Graphics g = Graphics.FromImage(bitmap))
				{
					g.CopyFromScreen(new System.Drawing.Point(mouse.X, mouse.Y), new System.Drawing.Point(0, 0), bitmap.Size);
					Color color = bitmap.GetPixel(0, 0);
					var i = dataGridColors.Rows.Add(mouse.X, mouse.Y, color.ToHEX());
					UpdateCellColor(i);
				}
			}
		}

		private void UpdateCellColor(int rowIndex)
		{
			var cell = this.dataGridColors.Rows[rowIndex].Cells[this.dataGridColors.Rows[rowIndex].Cells.Count - 1];
			cell.Style = new DataGridViewCellStyle
			{
				BackColor = (cell.Value as String ?? "").ToColor()
			};
		}

		private void HandleHotkey(int id)
		{
			switch (id)
			{
				case 0:
					if (workerClickAround.IsBusy)
					{
						workerClickAround.CancelAsync();
					}
					break;
				case 1:
					SetControlTextToMousePosition(txtMousePositionX, txtMousePositionY);
					break;
				case 2:
					AddCurrentMousePositionToPalette();
					break;
			}
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
						MouseActions = _actions,
						Offset = new Point(txtMousePositionX.Text.ToInt(), txtMousePositionY.Text.ToInt())
					});
		}

		private void btnExport_Click(object sender, EventArgs e)
		{
			dlgExportPixels.ShowDialog();
			string fileName = dlgExportPixels.FileName;
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
			string fileName = dlgImportPixels.FileName;
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
			for (int i = 0; i < dataGridColors.Rows.Count; i++)
			{
				if (dataGridColors.Rows[i].IsNewRow)
				{
					continue;
				}
				dataGridColors.Rows.RemoveAt(i);
			}
			foreach (object[] value in values)
			{
				dataGridColors.Rows.Add(value);
			}
		}

		private void btnLoadImage_Click(object sender, EventArgs e)
		{
			dlgImportImage.ShowDialog();
			if (!dlgImportImage.CheckFileExists)
			{
				return;
			}
			string imagepath = dlgImportImage.FileName;
			if (String.IsNullOrWhiteSpace(imagepath))
			{
				return;
			}
			btnLoadImage.Enabled = false;
			btnLoadImage.Text = "Loading...";
			var spots = new ColorSpot[dataGridColors.Rows.Count];
			for (int i = 0; i < dataGridColors.Rows.Count; i++)
			{
				DataGridViewRow row = dataGridColors.Rows[i];
				spots[i] = new ColorSpot
					{
						Color = (row.Cells[2].Value as String ?? "").ToColor(),
						Point =
							new Point((row.Cells[0].Value ?? "").ToString().ToInt(),
												   (row.Cells[1].Value ?? "").ToString().ToInt())
					};
			}
			List<ColorSpot> colorPalette = spots.Where(x => !x.Color.IsEmpty).ToList();
			var parserOptions =
				this.dataGridSettings.Rows.Cast<DataGridViewRow>().ToDictionary(
					row => (row.Cells[0].Value ?? "").ToString(), row => (row.Cells[1].Value ?? "").ToString());
			workerCalculate.RunWorkerAsync(
				new CalculatorArguments
					{
						Parser = _plugins[cmbParser.Text],
						ColorPalette = colorPalette,
						Imagepath = imagepath,
						ParserOptions = parserOptions
					});
			dlgImportImage.FileName = null;
		}

		private void btnPlay_Click(object sender, EventArgs e)
		{
			StartClicking();
		}

		private void UpdateOptions()
		{
			dataGridSettings.Rows.Clear();
			var currentPlugin = _plugins[cmbParser.Text];
			var attributes = currentPlugin.GetCustomAttributes(true).Select(x => x as DefaultSettingAttribute);
			foreach (DefaultSettingAttribute setting in attributes.Where(setting => setting != null))
			{
				this.dataGridSettings.Rows.Add(setting.Name, setting.DefaultValue);
			}
		}

		private void UpdatePreview()
		{
			var bitmap = new Bitmap((int) intPreviewWidth.Value, (int) intPreviewHeight.Value);
			var pen = new Pen(Color.Black, 1);
			using (Graphics graphics = Graphics.FromImage(bitmap))
			{
				foreach (MouseDragAction action in _actions)
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
			pctPreview.Width = bitmap.Width;
			pctPreview.Height = bitmap.Height;
			pctPreview.Image = bitmap;
		}

		private void workerCalculate_DoWork(object sender, DoWorkEventArgs e)
		{
			var args = e.Argument as CalculatorArguments;
			var results = new CalculatorResults();
			if (args == null)
			{
				return;
			}
			using (var bitmap = new Bitmap(args.Imagepath))
			{
				results.ImageHeight = bitmap.Height;
				results.ImageWidth = bitmap.Width;
			}
			var reader = (IBitmapReader)Activator.CreateInstance(args.Parser, new object[]{ args.Imagepath });
			try
			{
				results.Actions = reader.GetDrawInstructions(args.ColorPalette, args.ParserOptions).ToList();
			}
			catch (Exception err)
			{
				results.Error = err;
			}
			e.Result = results;
		}

		private void workerCalculate_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			btnLoadImage.Enabled = true;
			btnLoadImage.Text = "Parse Image";
			var results = e.Result as CalculatorResults;
			if (results == null)
			{
				return;
			}
			if (results.Error != null)
			{
				MessageBox.Show(results.Error.Message);
				return;
			}
			_actions = results.Actions;
			intPreviewHeight.Value = results.ImageHeight;
			intPreviewWidth.Value = results.ImageWidth;
			UpdatePreview();
		}

		private void workerClickAround_DoWork(object sender, DoWorkEventArgs e)
		{
			var args = e.Argument as ClickerArguments;
			if (args == null)
			{
				return;
			}
			foreach (
				var action in
					args.MouseActions.TakeWhile(coordinate => !workerClickAround.CancellationPending))
			{
				foreach (var b in action.Play(args.Offset).Where(b => workerClickAround.CancellationPending))
				{
					break;
				}
			}
		}

		private void dataGridColors_RowLeave(object sender, DataGridViewCellEventArgs e)
		{
			UpdateCellColor(e.RowIndex);
		}

		private void dataGridColors_CellLeave(object sender, DataGridViewCellEventArgs e)
		{
			UpdateCellColor(e.RowIndex);
		}

		private void dataGridColors_CellEndEdit(object sender, DataGridViewCellEventArgs e)
		{
			UpdateCellColor(e.RowIndex);
		}

		private void cmbParser_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateOptions();
		}
	}
}