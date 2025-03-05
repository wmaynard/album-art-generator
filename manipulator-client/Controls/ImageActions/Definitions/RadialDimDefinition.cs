using Maynard.ImageManipulator.Client.Utilities;
using Maynard.Imaging.Extensions;

namespace Maynard.ImageManipulator.Client.Controls.ImageActions.Definitions;

public class RadialDimDefinition() : ActionDefinitionWithSlider("Dim Edges", ActionDescriptions.DIM, minimum: 1, maximum: 20)
{
    public override string LoadingMessage => $"Dimming edges with strength {Strength}...";

    public override Picture Process(Picture picture) => picture.Dim(Strength);
}