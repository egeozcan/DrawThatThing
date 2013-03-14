namespace DrawThatThing.Internal
{
	using System.Collections.Generic;

	using BitmapReader;
	using BitmapReader.Classes;

	internal class ClickerArguments
	{
		public IEnumerable<MouseDragAction> MouseActions;
		public Point Offset;
	}
}