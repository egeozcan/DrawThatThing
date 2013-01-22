using System;
using System.Text;
using System.Drawing;
//using Emgu.CV;

namespace LowLevelTools
{
	using System.Collections.Generic;
	using System.Runtime.InteropServices;

	//using Emgu.CV.CvEnum;
	//using Emgu.CV.Structure;

	public class BitmapReader
	{
		private string _bitmapPath;

		public void Read(String path)
		{
			_bitmapPath = path;
		}

		//public MouseAction[] getAbstractLines()
		//{
		//    if (String.IsNullOrEmpty(_bitmapPath))
		//    {
		//        return null;
		//    }
		//    var image = new Image<Bgr, byte>(_bitmapPath);
		//    var memStorage = CvInvoke.cvCreateMemStorage(0);
		//    var lines = CvInvoke.cvHoughLines2(
		//        image, memStorage, HOUGH_TYPE.CV_HOUGH_PROBABILISTIC, 1.0, 1.0, 10, 5, 3.0);
		//    MCvSeq lineSeq = (MCvSeq)Marshal.PtrToStructure(lines, typeof(MCvSeq));
		//    MouseAction[] mouseActions = new MouseAction[lineSeq.total];
		//    for (int i = 0; i < lineSeq.total; i++)
		//    {
		//        int[] val = new int[4];
		//        Marshal.Copy(CvInvoke.cvGetSeqElem(lines, i), val, 0, 4);
		//        mouseActions[i] = new MouseAction();
		//        mouseActions[i].setDrag(val[0], val[1], val[2], val[3]);
		//        //mouseActions[i] = new LineSegment2D(
		//        //    new Point(val[0], val[1]),
		//        //    new Point(val[2], val[3]));
		//    }
		//    return mouseActions;
		//}

		public struct PixelAcceptanceArgs
		{
			public int RedMin;
			public int RedMax;
			public int BlueMin;
			public int BlueMax;
			public int GreenMin;
			public int GreenMax;
			public bool RedEnabled;
			public bool GreenEnabled;
			public bool BlueEnabled;
			public int MaxLight;
		}

		public MouseAction[] getFilteredPixels(PixelAcceptanceArgs args)
		{
			if (String.IsNullOrEmpty(_bitmapPath))
			{
				return null;
			}
			using (var bitmap = new Bitmap(_bitmapPath))
			{
				Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

				System.Drawing.Imaging.BitmapData bmpData =
					bitmap.LockBits(rect,
						System.Drawing.Imaging.ImageLockMode.ReadOnly,
						bitmap.PixelFormat);

				IntPtr ptr = bmpData.Scan0;

				int bytes = bmpData.Stride * bitmap.Height;
				byte[] rgbValues = new byte[bytes];

				Marshal.Copy(ptr, rgbValues, 0, bytes);

				int red;
				int green;
				int blue;
				var output = new List<MouseAction>();
				// y1 x1 y2 x2
				int[] lc = new[] { 0, 0, 0, 0 };

				for (int x = 0; x < bitmap.Width; x++)
				{
					bool lineInProgress = false;
					for (int y = 0; y < bitmap.Height; y++)
					{
						//See the link above for an explanation 
						//of this calculation (assumes 24bppRgb format)
						int position = (y * bmpData.Stride) + (x * 3);
						blue = rgbValues[position];
						green = rgbValues[position + 1];
						red = rgbValues[position + 2];
						if (pixelFits(red, green, blue, args))
						{
							if (lineInProgress)
							{
								lc[2] = y;
								lc[3] = x;
							}
							else
							{
								lineInProgress = true;
								lc[0] = lc[2] = y;
								lc[1] = lc[3] = x;
							}
						}
						else if (lineInProgress)
						{
							noteCoordinates(lc, output);
							lineInProgress = false;
						}
					}
					if (lineInProgress)
					{
						noteCoordinates(lc, output);
					}
				}
				bitmap.UnlockBits(bmpData);
				return output.ToArray();
			}
		}

		private static void noteCoordinates(int[] lc, List<MouseAction> output)
		{
			var action = new MouseAction();
			if (lc[1] != lc[3] || lc[0] != lc[2])
			{
				action.setDrag(lc[0], lc[1], lc[2], lc[3]);
			}
			else
			{
				action.setClick(lc[0], lc[1]);
			}
			output.Add(action);
		}

		private static bool pixelFits(int red, int green, int blue, PixelAcceptanceArgs args)
		{
			if (args.RedEnabled && (red < args.RedMin || red > args.RedMax))
			{
				return false;
			}
			if (args.GreenEnabled && (green < args.GreenMin || green > args.GreenMax))
			{
				return false;
			}
			if (args.BlueEnabled && (blue < args.BlueMin || blue > args.BlueMax))
			{
				return false;
			}
			return red + green + blue <= args.MaxLight;
		}
	}
}
