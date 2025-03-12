using Maynard.ImageManipulator.Client.Controls;
using Maynard.ImageManipulator.Client.Enums;

namespace Maynard.ImageManipulator.Client.Events;

public class ButtonClickEventArgs(ButtonType type)
{
    public ButtonType Type { get; set; } = type;
}