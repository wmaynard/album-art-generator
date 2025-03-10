using Maynard.ImageManipulator.Client.Extensions;
using Maynard.ImageManipulator.Client.Utilities;

namespace Maynard.ImageManipulator.Client.Controls.ImageActions.Definitions;

public class ScaleToMaxDefinition() : ActionDefinition("Scale to Max", ActionDescriptions.SCALE_TO_MAX_DIMENSION)
{
    public override string LoadingMessage => "Scaling an image to its max dimension...";
    public override Picture Process(Picture picture) => picture.ScaleToMaxDimension();
    public override Func<Picture, string, Picture> GenerateDelegate() => (picture, _) => picture.ScaleToMaxDimension();
    public override object[] ConfigurableValues => null;
    protected override void Deserialize(string[] values) { }
}