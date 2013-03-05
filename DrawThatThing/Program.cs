using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DrawThatThing
{
	static class Program
	{
		/// <summary>
		/// The main entry Point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new DrawThatThing());
		}
	}
}
