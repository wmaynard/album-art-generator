using Maynard.ImageManipulator.Client.Utilities;

namespace Maynard.ImageManipulator.Client.Controls.ImageActions.Definitions;

public class BlurDefinition : ActionDefinition
{
    private Label StrengthLabel { get; set; }
    private SliderWithLabels StrengthSlider { get; set; }
    
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
}