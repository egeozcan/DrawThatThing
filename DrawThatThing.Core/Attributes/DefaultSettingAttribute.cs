using System.Globalization;

namespace DrawThatThing.Core.Attributes;

/// <summary>
/// Declares a setting (and its default value) that a bitmap reader understands.
/// The settings are listed in the "Parser Settings" grid when the reader is selected.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DefaultSettingAttribute : Attribute
{
    public string Name { get; }
    public string DefaultValue { get; }

    public DefaultSettingAttribute(string name, object defaultValue)
    {
        Name = name;
        DefaultValue = Convert.ToString(defaultValue, CultureInfo.InvariantCulture) ?? string.Empty;
    }
}
