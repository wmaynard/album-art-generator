using Maynard.ImageManipulator.Client.Utilities;
using Maynard.Imaging.Extensions;

namespace Maynard.ImageManipulator.Client.Controls.ImageActions.Definitions;

public class ScaleToMaxDefinition() : ActionDefinition("Scale to Max", ActionDescriptions.SCALE_TO_MAX_DIMENSION)
{
    public override string LoadingMessage => "Scaling an image to its max dimension...";
    public override Picture Process(Picture image) => image.ScaleToMaxDimension();
}