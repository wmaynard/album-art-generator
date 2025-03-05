using Maynard.ImageManipulator.Client.Utilities;
using Maynard.Imaging.Extensions;

namespace Maynard.ImageManipulator.Client.Controls.ImageActions.Definitions;

public class SpotifyDefinition : ActionDefinitionWithSlider
{
    public SpotifyDefinition() : base("Spot Effect", ActionDescriptions.SPOTIFY, minimum: 1, maximum: 100)
        => StrengthLabel.Text = "Tile Size (pixels)";

    public override string LoadingMessage => "Slicing image into tiles...";
    public override Picture Process(Picture picture) => picture.Spotify(Strength);
}