using Maynard.ImageManipulator.Client.Controls.ImageActions.Definitions;
using Maynard.ImageManipulator.Client.Utilities;

namespace Maynard.ImageManipulator.Client.Controls.ImageActions.Buttons;

public class BlurButton() : ActionButton("Blur", ActionDescriptions.BLUR)
{
    protected override void OnClick(object sender, EventArgs args) => Clicked?.Invoke(this, new(new BlurDefinition()));
}