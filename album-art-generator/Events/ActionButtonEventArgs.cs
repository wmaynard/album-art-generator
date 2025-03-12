using Maynard.ImageManipulator.Client.Controls.ImageActions.Definitions;

namespace Maynard.ImageManipulator.Client.Events;

public class ActionButtonEventArgs(ActionDefinition control)
{
    public ActionDefinition Control { get; set; } = control;
}