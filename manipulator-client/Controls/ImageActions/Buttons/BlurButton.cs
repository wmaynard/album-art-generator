using Maynard.ImageManipulator.Client.Controls.ImageActions.Definitions;
using Maynard.ImageManipulator.Client.Utilities;

namespace Maynard.ImageManipulator.Client.Controls.ImageActions.Buttons;

public class BlurButton() : ActionButton(ActionDescriptions.TITLE_BLUR, ActionDescriptions.BLUR)
{
    protected override void OnClick(object sender, EventArgs args) => Clicked?.Invoke(this, new(new BlurDefinition()));
}