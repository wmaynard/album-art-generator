using Maynard.ImageManipulator.Client.Utilities;

namespace Maynard.ImageManipulator.Client.Controls;

public class EmptySpace : BoxView
{
    public EmptySpace(int width, int height, Color color = null)
    {
        if (width > 0)
            WidthRequest = width;
        if (height > 0)
            HeightRequest = height;
        BackgroundColor = color ?? WdmColors.BLACK;
    }

    public EmptySpace(int height) : this(0, height) { }
}