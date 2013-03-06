namespace DrawThatThing.Internal
{
	using System.Collections.Generic;

	using BitmapReader;
	using BitmapReader.Classes;

	internal class ClickerArguments
	{
		public IEnumerable<MouseDragAction> mouseActions;
		public Point offset;
	}
}