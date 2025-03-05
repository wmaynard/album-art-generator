using Maynard.ImageManipulator.Client.Utilities;

namespace Maynard.ImageManipulator.Client.Controls.ImageActions.Definitions;

public abstract class ActionDefinitionWithSlider : ActionDefinition
{
    protected Label StrengthLabel { get; set; }
    protected SliderWithLabels StrengthSlider { get; set; }
    public int Strength => (int)StrengthSlider.Value;

    public ActionDefinitionWithSlider(string name, string description, int minimum, int maximum) : base(name, description)
    {
        StrengthLabel = new()
        {
            Text = "Effect Strength"
        };
        StrengthSlider = new()
        {
            Minimum = minimum,
            Maximum = maximum,
            Value = minimum,
            ThumbColor = WdmColors.BLUE,
            MinimumTrackColor = WdmColors.GREEN,
            MaximumTrackColor = WdmColors.WHITE,
        };
        StrengthSlider.ValueChanged += OnUpdated;
        
        Stack.Add(StrengthLabel);
        Stack.Add(StrengthSlider);
    }
}