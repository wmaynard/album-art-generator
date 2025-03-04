namespace Maynard.ImageManipulator.Client.Utilities;

public static class Tooltip
{
    public static void Set(BindableObject view, string text) => SemanticProperties.SetDescription(view, text);
}