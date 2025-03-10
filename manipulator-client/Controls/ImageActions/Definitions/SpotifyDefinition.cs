using Maynard.ImageManipulator.Client.Extensions;
using Maynard.ImageManipulator.Client.Utilities;

namespace Maynard.ImageManipulator.Client.Controls.ImageActions.Definitions;

public class SpotifyDefinition : ActionDefinitionWithSlider
{
    public SpotifyDefinition() : base("Spot Effect", ActionDescriptions.SPOTIFY, minimum: 1, maximum: 100)
        => StrengthLabel.Text = "Tile Size (pixels)";

    public override string LoadingMessage => "Slicing image into tiles...";
    public override Picture Process(Picture picture) => picture.Spotify(Strength);
    public override Func<Picture, string, Picture> GenerateDelegate() => (picture, _) => picture.Spotify(Strength);
    public override object[] ConfigurableValues => [Strength];
    protected override void Deserialize(string[] values) { StrengthSlider.Value = int.Parse(values[0]); }
}