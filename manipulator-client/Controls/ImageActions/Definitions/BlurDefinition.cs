using Maynard.ImageManipulator.Client.Utilities;
using Maynard.Imaging.Extensions;

namespace Maynard.ImageManipulator.Client.Controls.ImageActions.Definitions;

public class BlurDefinition : ActionDefinition
{
    public override string LoadingMessage => $"Blurring image with strength {Strength}...";
    private Label StrengthLabel { get; set; }
    private SliderWithLabels StrengthSlider { get; set; }
    public int Strength => (int)StrengthSlider.Value;
    
    public BlurDefinition() : base("Blur", ActionDescriptions.BLUR)
    {
        StrengthLabel = new()
        {
            Text = "Effect Strength"
        };
        StrengthSlider = new()
        {
            Minimum = 1,
            Maximum = 20,
            Value = 1, 
            ThumbColor = WdmColors.BLUE,
            MinimumTrackColor = WdmColors.GREEN,
            MaximumTrackColor = WdmColors.WHITE,
        };
        StrengthSlider.ValueChanged += OnUpdated;
        
        Stack.Add(StrengthLabel);
        Stack.Add(StrengthSlider);
    }

    public override Picture Process(Picture picture)
    {
        return picture.Blur(Strength);
    }
}