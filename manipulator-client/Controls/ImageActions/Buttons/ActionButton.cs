using Maynard.ImageManipulator.Client.Events;
using Maynard.ImageManipulator.Client.Utilities;

namespace Maynard.ImageManipulator.Client.Controls.ImageActions.Buttons;

public abstract class ActionButton : Panel
{
    public string EffectName { get; init; }
    protected Button AddButton { get; set; }
    private Label Description { get; set; }
    
    public EventHandler<ActionButtonEventArgs> Clicked { get; set; }
    protected ActionButton(string name, string description)
    {
        TitleLabel.IsVisible = false;
        Border.Stroke = WdmBrushes.YELLOW;
        Border.MinimumHeightRequest = 0;
        Border.StrokeThickness = 1;
        AddButton = new()
        {
            Text = EffectName = name
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