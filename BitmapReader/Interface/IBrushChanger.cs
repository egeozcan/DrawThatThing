using System.Collections.Generic;
using BitmapReader.Classes;

namespace BitmapReader.Interface
{
	public interface IBrushChanger
	{
		IEnumerable<Brush> GetBrushes();
		MouseDragAction SelectBrush(Brush brush);
	}
}