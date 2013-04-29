using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoodleOrDieBrushChanger
{
	using BitmapReader.Classes;
	using BitmapReader.Interface;

	public class DoodleOrDieBrushChanger : IBrushChanger
	{
		public IEnumerable<Brush> GetBrushes()
		{
			throw new NotImplementedException();
		}
		public MouseDragAction SelectBrush(Brush brush)
		{
			throw new NotImplementedException();
		}
	}
}
