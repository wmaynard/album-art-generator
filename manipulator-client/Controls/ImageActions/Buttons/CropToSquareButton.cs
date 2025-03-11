using Maynard.ImageManipulator.Client.Controls.ImageActions.Definitions;
using Maynard.ImageManipulator.Client.Utilities;

namespace Maynard.ImageManipulator.Client.Controls.ImageActions.Buttons;

public class CropToSquareButton() : ActionButton(ActionDescriptions.TITLE_CROP_TO_SQUARE, ActionDescriptions.CROP_TO_SQUARE)
{
    protected override void OnClick(object sender, EventArgs args) => Clicked?.Invoke(this, new(new CropToSquareDefinition()));
}