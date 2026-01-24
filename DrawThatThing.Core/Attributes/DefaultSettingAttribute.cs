namespace DrawThatThing.Core.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DefaultSettingAttribute : Attribute
{
    public string Name { get; }
    public string DefaultValue { get; }

    public DefaultSettingAttribute(string name, string defaultValue)
    {
        Name = name;
        DefaultValue = defaultValue;
    }
}
