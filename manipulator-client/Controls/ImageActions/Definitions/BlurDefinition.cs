using Maynard.ImageManipulator.Client.Controls.ImageActions.Buttons;
using Maynard.ImageManipulator.Client.Utilities;
using Maynard.Imaging.Extensions;

namespace Maynard.ImageManipulator.Client.Controls.ImageActions.Definitions;

public class BlurDefinition() : ActionDefinitionWithSlider("Blur", ActionDescriptions.BLUR, minimum: 1, maximum: 20)
{
    public override string LoadingMessage => $"Blurring image with strength {Strength}...";

    public override Picture Process(Picture picture) => picture.Blur(Strength);
}