using Maynard.ImageManipulator.Client.Extensions;
using Maynard.ImageManipulator.Client.Utilities;
using SixLabors.ImageSharp.PixelFormats;

namespace Maynard.ImageManipulator.Client.Controls.ImageActions.Definitions;

public class SuperimposeDefinition() : ActionDefinition("Superimpose Original", ActionDescriptions.SUPERIMPOSE)
{
    public override string LoadingMessage => "Superimposing original image...";
    public override Picture Process(Picture image) => throw new NotImplementedException();

    public Picture Process(Picture picture, Picture other)
    {
        other = other.ScaleToDimension(picture.MaximumDimension());
        return picture.Superimpose(other);
    }
    public override Func<Picture, string, Picture> GenerateDelegate() => (picture, filename) =>
    {
        Picture original = SixLabors.ImageSharp.Image.Load<Rgba32>(filename);
        original = original.ScaleToDimension(picture.MaximumDimension());
        return picture.Superimpose(original);
    };
    public override object[] ConfigurableValues => null;
    protected override void Deserialize(string[] values) { }
}