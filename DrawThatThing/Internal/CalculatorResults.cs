namespace DrawThatThing.Internal
{
	using System;
	using System.Collections.Generic;

	using BitmapReader.Classes;

	internal class CalculatorResults
	{
		public List<MouseDragAction> Actions;
		public Exception Error;
		public bool UseAlternativeParser;
		public int ImageWidth;
		public int ImageHeight;
	}
}