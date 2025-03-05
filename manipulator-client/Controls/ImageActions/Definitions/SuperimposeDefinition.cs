using Maynard.ImageManipulator.Client.Utilities;
using Maynard.Imaging.Extensions;

namespace Maynard.ImageManipulator.Client.Controls.ImageActions.Definitions;

public class SuperimposeDefinition() : ActionDefinition("Superimpose Original", ActionDescriptions.SUPERIMPOSE)
{
    public override string LoadingMessage => "Superimposing original image...";
    public override Picture Process(Picture image) => throw new NotImplementedException();
    public Picture Process(Picture picture, Picture other) => picture.Superimpose(other);
}