namespace DrawThatThing.Internal
{
	using System;
	using System.Collections.Generic;

	using Helpers.Classes;

	internal class CalculatorResults
	{
		public List<MouseDragAction> actions;
		public Exception error;
		public bool useAlternativeParser;
		public int imageWidth;
		public int imageHeight;
	}
}