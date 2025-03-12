using Maynard.ImageManipulator.Client.Controls.ImageActions.Definitions;
using Maynard.ImageManipulator.Client.Utilities;

namespace Maynard.ImageManipulator.Client.Controls.ImageActions.Buttons;

public class WhiteVignetteButton() : ActionButton(ActionDescriptions.TITLE_WHITE_VIGNETTE, ActionDescriptions.WHITE_VIGNETTE)
{
    protected override void OnClick(object sender, EventArgs args) => Clicked?.Invoke(this, new(new WhiteVignetteDefinition()));
}