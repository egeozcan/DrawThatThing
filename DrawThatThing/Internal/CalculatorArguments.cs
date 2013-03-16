using System;

namespace DrawThatThing.Internal
{
	using System.Collections.Generic;

	using BitmapReader.Classes;

	internal class CalculatorArguments
	{
	    public Type Parser;
		public IDictionary<string, string> ParserOptions; 
		public List<ColorSpot> ColorPalette;
		public string Imagepath;
	}
}