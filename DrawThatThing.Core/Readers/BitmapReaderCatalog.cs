using System.Reflection;
using System.Runtime.ExceptionServices;
using DrawThatThing.Core.Attributes;
using DrawThatThing.Core.Interfaces;

namespace DrawThatThing.Core.Readers;

/// <summary>
/// The bitmap readers ("parsers") the user can choose from: the built-in ones plus any
/// <see cref="IBitmapReader"/> implementations found in DLLs inside the "Plugins" folder.
/// </summary>
public sealed class BitmapReaderCatalog
{
    private readonly Dictionary<string, Type> _readers = new(StringComparer.Ordinal);

    public IReadOnlyCollection<string> Names => _readers.Keys;

    public static BitmapReaderCatalog CreateDefault(string? pluginDirectory = null)
    {
        var catalog = new BitmapReaderCatalog();
        catalog.Add(typeof(AbstractReader));
        catalog.Add(typeof(DetailedReader));
        catalog.Add(typeof(LinearReader));
        catalog.Add(typeof(PointReader));
        if (pluginDirectory != null)
        {
            catalog.LoadPlugins(pluginDirectory);
        }
        return catalog;
    }

    public void Add(Type readerType, string? name = null)
    {
        name ??= readerType.Name;
        _readers.TryAdd(name, readerType);
    }

    public void LoadPlugins(string directory)
    {
        if (!Directory.Exists(directory))
        {
            return;
        }

        foreach (var pluginFile in Directory.GetFiles(directory, "*.dll").OrderBy(f => f, StringComparer.OrdinalIgnoreCase))
        {
            Type[] types;
            try
            {
                var assembly = Assembly.LoadFrom(pluginFile);
                // A plugin's build output usually contains a copy of this library; its readers are already built in.
                if (assembly == typeof(IBitmapReader).Assembly)
                {
                    continue;
                }
                types = assembly.GetTypes();
            }
            catch (Exception)
            {
                continue;
            }

            foreach (var type in types.Where(IsReaderType))
            {
                Add(type, Path.GetFileName(pluginFile));
            }
        }
    }

    public IEnumerable<DefaultSettingAttribute> GetDefaultSettings(string name)
    {
        return _readers.TryGetValue(name, out var type)
            ? type.GetCustomAttributes<DefaultSettingAttribute>(true)
            : [];
    }

    public IBitmapReader Create(string name, string imagePath)
    {
        if (!_readers.TryGetValue(name, out var type))
        {
            throw new InvalidOperationException($"Unknown parser \"{name}\".");
        }
        try
        {
            return (IBitmapReader)Activator.CreateInstance(type, imagePath)!;
        }
        catch (TargetInvocationException ex) when (ex.InnerException != null)
        {
            // Show the parser's own error message rather than "Exception has been thrown by the target of an invocation".
            ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            throw;
        }
    }

    private static bool IsReaderType(Type type)
    {
        return typeof(IBitmapReader).IsAssignableFrom(type)
               && type is { IsAbstract: false, IsInterface: false }
               && type.GetConstructor([typeof(string)]) != null;
    }
}
