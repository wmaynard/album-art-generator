using Maynard.ImageManipulator.Client.Utilities;
using Maynard.Imaging.Extensions;

namespace Maynard.ImageManipulator.Client.Controls.ImageActions.Definitions;

public class RadialDimDefinition() : ActionDefinitionWithSlider("Dim Edges", ActionDescriptions.DIM, minimum: 1, maximum: 20)
{
    public override string LoadingMessage => $"Dimming edges with strength {Strength}...";

    public override Picture Process(Picture picture) => picture.Dim(Strength);
    public override Func<Picture, string, Picture> GenerateDelegate() => (picture, _) => picture.Dim(Strength);
    public override object[] ConfigurableValues => [Strength];
    protected override void Deserialize(string[] values) => StrengthSlider.Value = int.Parse(values[0]);
}