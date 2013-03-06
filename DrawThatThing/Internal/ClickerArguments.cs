namespace DrawThatThing.Internal
{
	using System.Collections.Generic;

	using Helpers;
	using Helpers.Classes;

	internal class ClickerArguments
	{
		public IEnumerable<MouseDragAction> mouseActions;
		public Point offset;
	}
}