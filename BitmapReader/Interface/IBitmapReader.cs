using System.Collections.Generic;
using BitmapReader.Classes;

namespace BitmapReader.Interface
{
	public interface IBitmapReader
	{
		IEnumerable<MouseDragAction> GetDrawInstructions(
			List<ColorSpot> colorPalette, IDictionary<string, string> options = null);
	}
}