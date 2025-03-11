using Maynard.ImageManipulator.Client.Controls.ImageActions.Definitions;
using Maynard.ImageManipulator.Client.Utilities;

namespace Maynard.ImageManipulator.Client.Controls.ImageActions.Buttons;

public class ScaleToMaxButton() : ActionButton(ActionDescriptions.TITLE_SCALE_TO_MAX_DIMENSION, ActionDescriptions.SCALE_TO_MAX_DIMENSION)
{
    protected override void OnClick(object sender, EventArgs args) => Clicked?.Invoke(this, new(new ScaleToMaxDefinition()));
}