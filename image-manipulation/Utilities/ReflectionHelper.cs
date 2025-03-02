using System.Reflection;
using System.Text.RegularExpressions;
using Maynard.Imaging.Extensions;

namespace Maynard.Imaging.Utilities;

public static partial class ReflectionHelper
{
    public static string[] GetActionNames()
    {
        List<string> actions = ["--- Select an action ---"];
        actions.AddRange(Assembly
            .GetAssembly(typeof(BlurExtension))
            ?.GetTypes()
            .Where(type => type.IsPublic && type.Namespace == "Maynard.Imaging.Extensions")
            .Select(type => type.Name.Replace("Extension", ""))
            .Select(name => CapitalSpacingRegex().Replace(name, " $1"))
            ?? []
        );

        return actions.ToArray();
    }

    [GeneratedRegex("(?<=.)([A-Z])")]
    private static partial Regex CapitalSpacingRegex();
}