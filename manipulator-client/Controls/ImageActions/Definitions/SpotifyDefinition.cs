using Maynard.ImageManipulator.Client.Extensions;
using Maynard.ImageManipulator.Client.Utilities;

namespace Maynard.ImageManipulator.Client.Controls.ImageActions.Definitions;

public class SpotifyDefinition : ActionDefinitionWithSlider
{
    private SliderWithLabels DimSlider { get; set; }
    public SpotifyDefinition() : base(ActionDescriptions.TITLE_SPOTIFY, ActionDescriptions.SPOTIFY, minimum: 2, maximum: 100)
    {
        StrengthLabel.Text = "Tile Size (pixels)";
        DimSlider = new()
        {
            Minimum = RadialDimDefinition.MINIMUM,
            Maximum = RadialDimDefinition.MAXIMUM,
            Title = "Dim Strength",
            Value = (int)(RadialDimDefinition.MAXIMUM / 2)
        };
        DimSlider.ValueChanged += ValueChanged;
        
        Stack.Add(DimSlider);
    }
    private void ValueChanged(object sender, EventArgs e) => EffectUpdated?.Invoke(this, EventArgs.Empty);
    public override string LoadingMessage => "Slicing image into tiles...";
    public override Picture Process(Picture picture) => picture.Spotify(Strength, (int)DimSlider.Value);
    public override Func<Picture, string, Picture> GenerateDelegate() => (picture, _) => picture.Spotify(Strength, (int)DimSlider.Value);
    public override object[] ConfigurableValues => [Strength, DimSlider.Value];

    protected override void Deserialize(string[] values)
    {
        StrengthSlider.Value = int.Parse(values[0]);
        DimSlider.Value = int.Parse(values[1]);
    }
}