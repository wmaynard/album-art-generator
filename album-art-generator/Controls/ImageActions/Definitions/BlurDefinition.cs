using Maynard.ImageManipulator.Client.Controls.ImageActions.Buttons;
using Maynard.ImageManipulator.Client.Extensions;
using Maynard.ImageManipulator.Client.Utilities;

namespace Maynard.ImageManipulator.Client.Controls.ImageActions.Definitions;

public class BlurDefinition() : ActionDefinitionWithSlider(ActionDescriptions.TITLE_BLUR, ActionDescriptions.BLUR, minimum: 1, maximum: 20)
{
    public override string LoadingMessage => $"Blurring image with strength {Strength}...";

    public override Picture Process(Picture picture) => picture.Blur(Strength);
    public override Func<Picture, string, Picture> GenerateDelegate() => (picture, _) => picture.Blur(Strength);
    public override object[] ConfigurableValues => [Strength];
    protected override void Deserialize(string[] values) => StrengthSlider.Value = int.Parse(values[0]);
}