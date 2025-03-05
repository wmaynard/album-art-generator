using Maynard.ImageManipulator.Client.Utilities;
using Maynard.Imaging.Extensions;

namespace Maynard.ImageManipulator.Client.Controls.ImageActions.Definitions;

public class CropToSquareDefinition() : ActionDefinition("Crop to Square", ActionDescriptions.CROP_TO_SQUARE)
{
    public override string LoadingMessage => "Cropping to square dimensions...";
    public override Picture Process(Picture picture) => picture.CropToSquare();
}