using Maynard.ImageManipulator.Client.Controls.ImageActions.Definitions;
using Maynard.ImageManipulator.Client.Utilities;

namespace Maynard.ImageManipulator.Client.Controls.ImageActions.Buttons;

public class BlurButton() : ActionButton("Blur", ActionDescriptions.BLUR)
{
    protected override void OnClick(object sender, EventArgs args) => Clicked?.Invoke(this, new(new BlurDefinition()));
}

public class CropToSquareButton() : ActionButton("Crop to Sqaure", ActionDescriptions.CROP_TO_SQUARE)
{
    protected override void OnClick(object sender, EventArgs args) => Clicked?.Invoke(this, new(new BlurDefinition()));
}

public class RadialDimButton() : ActionButton("Dim Edges", ActionDescriptions.DIM)
{
    protected override void OnClick(object sender, EventArgs args) => Clicked?.Invoke(this, new(new BlurDefinition()));
}

public class ResizeButton() : ActionButton("Resize", ActionDescriptions.RESIZE)
{
    protected override void OnClick(object sender, EventArgs args) => Clicked?.Invoke(this, new(new BlurDefinition()));
}

public class ScaleToMaxButton() : ActionButton("Scale to Max Dimension", ActionDescriptions.SCALE_TO_MAX_DIMENSION)
{
    protected override void OnClick(object sender, EventArgs args) => Clicked?.Invoke(this, new(new BlurDefinition()));
}

public class SpotifyButton() : ActionButton("Spot Effect", ActionDescriptions.SPOTIFY)
{
    protected override void OnClick(object sender, EventArgs args) => Clicked?.Invoke(this, new(new BlurDefinition()));
}

public class SuperimposeButton() : ActionButton("Superimpose Original", ActionDescriptions.SUPERIMPOSE)
{
    protected override void OnClick(object sender, EventArgs args) => Clicked?.Invoke(this, new(new BlurDefinition()));
}

public class UpscaleButton() : ActionButton("Upscale", ActionDescriptions.UPSCALE)
{
    protected override void OnClick(object sender, EventArgs args) => Clicked?.Invoke(this, new(new BlurDefinition()));
}