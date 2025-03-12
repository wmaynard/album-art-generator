using Maynard.ImageManipulator.Client.Controls.ImageActions.Definitions;
using Maynard.ImageManipulator.Client.Utilities;

namespace Maynard.ImageManipulator.Client.Controls.ImageActions.Buttons;

public class VignetteButton() : ActionButton(ActionDescriptions.TITLE_VIGNETTE, ActionDescriptions.VIGNETTE)
{
    protected override void OnClick(object sender, EventArgs args) => Clicked?.Invoke(this, new(new VignetteDefinition()));
}