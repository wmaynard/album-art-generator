using Maynard.ImageManipulator.Client.Controls.ImageActions.Definitions;
using Maynard.ImageManipulator.Client.Utilities;

namespace Maynard.ImageManipulator.Client.Controls.ImageActions.Buttons;

public class DodgeButton() : ActionButton(ActionDescriptions.TITLE_DODGE, ActionDescriptions.DODGE)
{
    protected override void OnClick(object sender, EventArgs args) => Clicked?.Invoke(this, new(new RadialDodgeDefinition()));
}