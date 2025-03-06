using System.Text.RegularExpressions;
using Maynard.ImageManipulator.Client.Controls.ImageActions.Definitions;
using Maynard.ImageManipulator.Client.Controls.Panels;

namespace Maynard.ImageManipulator.Client.Utilities;

public static partial class TransformationPersistenceManager
{
    private const string PREFIX = "CustomTransformation";
    private const int BUILT_IN_START = 1;
    public const int CUSTOM_START = 100;
    private const char SEPARATOR = '\uf8ff';

    private static string GenerateKey(int index) => $"{PREFIX}{index}";
    private static int GetNextKey()
    {
        int next = CUSTOM_START;
        while (Preferences.ContainsKey(GenerateKey(next)))
            next++;
        return next;
    }

    public static void Delete(string key)
    {
        int number = int.Parse(IntegerRegex().Match(key).Value);
        while (Preferences.ContainsKey(GenerateKey(number + 1)))
            Preferences.Set(GenerateKey(number), Preferences.Get(GenerateKey(number++ + 1), null));
        Preferences.Remove(GenerateKey(number));
    }

    public static CustomTransformationData[] List()
    {
        List<CustomTransformationData> output = new();
        output.AddRange(Scan(BUILT_IN_START));
        output.AddRange(Scan(CUSTOM_START));
        return output.ToArray();
    }

    private static CustomTransformationData[] Scan(int start)
    {
        List<CustomTransformationData> output = new();
        string key;
        while (Preferences.ContainsKey(key = GenerateKey(start)))
        {
            string data = Preferences.Get(key, null);
            if (string.IsNullOrWhiteSpace(data))
                break;
            string[] parts = data.Split(SEPARATOR);
            
            output.Add(new()
            {
                Key = key,
                Index = start++,
                Description = parts.First(),
                Actions = ActionDefinition.Describe(parts.Last())
            });
        }

        return output.ToArray();
    }
    
    public static void Save(string description)
    {
        string key = GenerateKey(GetNextKey());
        string transformation = ActionPanel.Instance.TransformationString;
        if (!string.IsNullOrWhiteSpace(transformation))
            Preferences.Set(key, $"{description}{SEPARATOR}{transformation}");
    }

    public static string Load(string key)
    {
        if (!Preferences.ContainsKey(key))
            return null;
        string serialized = Preferences.Get(key, null);
        if (string.IsNullOrWhiteSpace(serialized))
            return null;
        string transformation = serialized[(serialized.IndexOf(SEPARATOR) + 1)..];
        ActionPanel.Instance.Load(transformation);
        ActionPanel.Instance.Focus();
        return transformation;
    }

 
    [GeneratedRegex(@"\d+$")]
    private static partial Regex IntegerRegex();
}

public class CustomTransformationData
{
    public bool IsBuiltIn => Index < TransformationPersistenceManager.CUSTOM_START;
    public string Key { get; init; }
    public int Index { get; init; }
    public string Description { get; init; }
    public string[] Actions { get; set; }
}
