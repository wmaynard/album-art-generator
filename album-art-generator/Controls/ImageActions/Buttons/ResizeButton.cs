using Maynard.ImageManipulator.Client.Controls.ImageActions.Definitions;
using Maynard.ImageManipulator.Client.Utilities;

namespace Maynard.ImageManipulator.Client.Controls.ImageActions.Buttons;

public class ResizeButton() : ActionButton(ActionDescriptions.TITLE_RESIZE, ActionDescriptions.RESIZE)
{
    protected override void OnClick(object sender, EventArgs args) => Clicked?.Invoke(this, new(new ResizeDefinition()));
}