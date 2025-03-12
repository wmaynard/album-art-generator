using Maynard.ImageManipulator.Client.Extensions;
using Maynard.ImageManipulator.Client.Utilities;

namespace Maynard.ImageManipulator.Client.Controls.ImageActions.Definitions;

public class CropToSquareDefinition() : ActionDefinition(ActionDescriptions.TITLE_CROP_TO_SQUARE, ActionDescriptions.CROP_TO_SQUARE)
{
    public override string LoadingMessage => "Cropping to square dimensions...";
    public override Picture Process(Picture picture) => picture.CropToSquare();
    public override Func<Picture, string, Picture> GenerateDelegate() => (picture, _) => picture.CropToSquare();

    public override object[] ConfigurableValues => null;
    protected override void Deserialize(string[] values) { }
}