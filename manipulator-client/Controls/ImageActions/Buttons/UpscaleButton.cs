using Maynard.ImageManipulator.Client.Controls.ImageActions.Definitions;
using Maynard.ImageManipulator.Client.Utilities;

namespace Maynard.ImageManipulator.Client.Controls.ImageActions.Buttons;

public class UpscaleButton() : ActionButton("Upscale", ActionDescriptions.UPSCALE)
{
    protected override void OnClick(object sender, EventArgs args) => Clicked?.Invoke(this, new(new UpscaleDefinition()));
}