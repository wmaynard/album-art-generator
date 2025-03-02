using System.Reflection;

namespace Maynard.Imaging.Utilities;

public static class ReflectionHelper
{
    public static string[] GetActionNames()
    {
        return
        [
            "--- Select an action ---",
            "Blur",
            "Crop to Square",
            "Dim",
            "Resize",
            "Scale to Max Dimension",
            "Spotify",
            "Superimpose",
            "Upscale"
        ];
    }
}