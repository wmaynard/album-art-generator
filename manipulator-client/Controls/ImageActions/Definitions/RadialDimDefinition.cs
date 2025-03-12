using Maynard.ImageManipulator.Client.Extensions;
using Maynard.ImageManipulator.Client.Utilities;

namespace Maynard.ImageManipulator.Client.Controls.ImageActions.Definitions;

public class RadialDimDefinition() : ActionDefinitionWithSlider(ActionDescriptions.TITLE_DIM, ActionDescriptions.DIM, minimum: MINIMUM, maximum: MAXIMUM)
{
    public const int MINIMUM = 1;
    public const int MAXIMUM = 200;
    public override string LoadingMessage => $"Dimming edges with strength {Strength}...";

    public override Picture Process(Picture picture) => picture.Dim(Strength);
    public override Func<Picture, string, Picture> GenerateDelegate() => (picture, _) => picture.Dim(Strength);
    public override object[] ConfigurableValues => [Strength];
    protected override void Deserialize(string[] values) => StrengthSlider.Value = int.Parse(values[0]);
}