using Maynard.ImageManipulator.Client.Utilities;

namespace Maynard.ImageManipulator.Client.Controls;

public abstract class ActionButton : Panel
{
    public string Name { get; init; }
    
    protected Button AddButton { get; set; }
    private Label Description { get; set; }
    
    public EventHandler<ActionButtonEventArgs> Clicked { get; set; }
    public ActionButton(string name, string description)
    {
        TitleLabel.IsVisible = false;
        Border.Stroke = WdmBrushes.YELLOW;
        Border.MinimumHeightRequest = 0;
        Border.StrokeThickness = 1;
        AddButton = new()
        {
            Text = name
        };
        AddButton.Clicked += OnClick;
        Description = new()
        {
            Text = description
        };

        Padding = new(5);
        Stack.Add(AddButton);
        Stack.Add(new EmptySpace(30));
        Stack.Add(Description);
    }

    protected abstract void OnClick(object sender, EventArgs args);
}

public class ActionButtonEventArgs(ActionDefinition control)
{
    public ActionDefinition Control { get; set; } = control;
}



public abstract class ActionDefinition : Panel
{
    private Label Description { get; set; }
    public EventHandler EffectUpdated { get; set; }
    public ActionDefinition(string name, string description)
    {
        Border.Stroke = WdmBrushes.YELLOW;
        Border.StrokeThickness = 1;
        Border.MinimumHeightRequest = 0;
        TitleLabel.Text = name;
        Description = new()
        {
            Text = description
        };
        
        Stack.Add(Description);
    }
}

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
        
        Stack.Add(StrengthLabel);
        Stack.Add(StrengthSlider);
    }
}
public class BlurButton : ActionButton
{
    public BlurButton() : base("Blur", ActionDescriptions.BLUR)
    {
    }

    protected override void OnClick(object sender, EventArgs args)
    {
        Clicked?.Invoke(this, new(new BlurDefinition()));
    }
}

// Blur
// Crop To Square
// Dim
// Resize
// ScaleToMaxDimension
// Spotify
// Superimpose
// Upscale