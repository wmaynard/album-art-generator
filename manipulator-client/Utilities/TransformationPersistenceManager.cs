using System.Text.RegularExpressions;
using Maynard.ImageManipulator.Client.Controls.ImageActions.Definitions;
using Maynard.ImageManipulator.Client.Controls.Panels;
using Maynard.ImageManipulator.Client.Data;
using Maynard.ImageManipulator.Client.Pages;

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

    public async static Task<string> Load(string key)
    {
        if (!Preferences.ContainsKey(key))
            return null;
        string serialized = Preferences.Get(key, null);
        if (string.IsNullOrWhiteSpace(serialized))
            return null;
        string transformation = serialized[(serialized.IndexOf(SEPARATOR) + 1)..];

        
        // IView page = ActionPanel.Instance;
        // while (page != null && page is not ContentPage)
        //     page = (IView)page.Parent;
        //
        // if (page == null || page is not ContentPage target)
        // {
        //     Log.Error("Unable to navigate to transformation page");
        //     return transformation;
        // }

        await Gui.Update(async() => await Shell.Current.GoToAsync(new (nameof(TransformationPage)), true));
        ActionPanel.Instance.Load(transformation);
        
        return transformation;
    }
 
    [GeneratedRegex(@"\d+$")]
    private static partial Regex IntegerRegex();
}


