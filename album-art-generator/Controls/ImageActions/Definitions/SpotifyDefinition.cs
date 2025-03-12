using Maynard.ImageManipulator.Client.Extensions;
using Maynard.ImageManipulator.Client.Utilities;

namespace Maynard.ImageManipulator.Client.Controls.ImageActions.Definitions;

public class SpotifyDefinition : ActionDefinitionWithSlider
{
    private SliderWithLabels VignetteSlider { get; set; }
    public SpotifyDefinition() : base(ActionDescriptions.TITLE_SPOTIFY, ActionDescriptions.SPOTIFY, minimum: 2, maximum: 100)
    {
        StrengthLabel.Text = "Tile Size (pixels)";
        VignetteSlider = new()
        {
            Minimum = VignetteDefinition.MINIMUM,
            Maximum = VignetteDefinition.MAXIMUM,
            Title = "Vignette Strength",
            Value = (int)(VignetteDefinition.MAXIMUM / 2)
        };
        VignetteSlider.ValueChanged += ValueChanged;
        
        Stack.Add(VignetteSlider);
    }
    private void ValueChanged(object sender, EventArgs e) => EffectUpdated?.Invoke(this, EventArgs.Empty);
    public override string LoadingMessage => "Slicing image into tiles...";
    public override Picture Process(Picture picture) => picture.Spotify(Strength, (int)VignetteSlider.Value);
    public override Func<Picture, string, Picture> GenerateDelegate() => (picture, _) => picture.Spotify(Strength, (int)VignetteSlider.Value);
    public override object[] ConfigurableValues => [Strength, VignetteSlider.Value];

    protected override void Deserialize(string[] values)
    {
        StrengthSlider.Value = int.Parse(values[0]);
        VignetteSlider.Value = int.Parse(values[1]);
    }
}