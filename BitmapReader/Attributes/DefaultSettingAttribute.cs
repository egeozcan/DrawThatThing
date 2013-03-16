using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitmapReader.Attributes
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class DefaultSettingAttribute : Attribute
	{
		private readonly string _name;
		private readonly object _defaultValue;

		public string Name
		{
			get
			{
				return this._name;
			}
		}

		public object DefaultValue
		{
			get
			{
				return this._defaultValue;
			}
		}

		public DefaultSettingAttribute(string name, object defaultValue)
		{
			this._defaultValue = defaultValue;
			this._name = name;
		}
	}
}
