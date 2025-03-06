using System.Text.RegularExpressions;
using Maynard.ImageManipulator.Client.Controls.ImageActions.Definitions;
using Maynard.ImageManipulator.Client.Controls.Panels;

namespace Maynard.ImageManipulator.Client.Utilities;

public static partial class TransformationPersistenceManager
{
    private const string PREFIX = "CustomTransformation";
    private const int BUILT_IN_START = 1;
    private const int START = 100;
    private const char SEPARATOR = '\uf8ff';

    private static string GenerateKey(int index) => $"{PREFIX}{index}";
    private static int GetNextKey()
    {
        int next = START;
        while (Preferences.ContainsKey(GenerateKey(next)))
            next++;
        return next;
    }

    public static void Delete(string key)
    {
        int number = int.Parse(IntegerRegex().Match(key).Value);
        string current = null;
        string next = null;
        while (Preferences.ContainsKey(GenerateKey(number + 1)))
        {
            current = GenerateKey(number);
            next = GenerateKey(number + 1);
            Preferences.Set(current, Preferences.Get(next, null));
            number++;
        }
        Preferences.Remove(current ?? key);
    }

    public static CustomTransformationData[] List()
    {
        List<CustomTransformationData> output = new();
        output.AddRange(Scan(BUILT_IN_START));
        output.AddRange(Scan(START));
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
                Index = start,
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

    public class CustomTransformationData
    {
        public bool IsBuiltIn => Index < START;
        public string Key { get; init; }
        public int Index { get; init; }
        public string Description { get; init; }
        public string[] Actions { get; set; }
    }

    [GeneratedRegex(@"\d+$")]
    private static partial Regex IntegerRegex();
}

