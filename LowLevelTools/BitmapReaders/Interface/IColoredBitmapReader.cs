namespace BitmapReader.BitmapReaders.Interface
{
	using System.Collections.Generic;

	using Classes;

	public interface IColoredBitmapReader
	{
		IEnumerable<MouseDragAction> getDrawInstructions(List<ColorSpot> ColorPalette);
	}
}