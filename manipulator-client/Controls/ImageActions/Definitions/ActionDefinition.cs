using Maynard.ImageManipulator.Client.Controls.Panels;
using Maynard.ImageManipulator.Client.Enums;
using Maynard.ImageManipulator.Client.Events;
using Maynard.ImageManipulator.Client.Utilities;

namespace Maynard.ImageManipulator.Client.Controls.ImageActions.Definitions;

public abstract class ActionDefinition : Panel
{
    public abstract string LoadingMessage { get; }
    public string EffectName { get; init; }
    private Grid HeaderGrid { get; set; }
    private Button RemoveButton { get; set; }
    private Button MoveUpButton { get; set; }
    private Button MoveDownButton { get; set; }
    private new Label TitleLabel { get; set; }
    private Label Description { get; set; }
    public EventHandler EffectUpdated { get; set; }
    public EventHandler<ButtonClickEventArgs> ButtonClicked { get; set; }
    protected ActionDefinition(string name, string description)
    {
        HeaderGrid = new()
        {
            Padding = 5,
            ColumnDefinitions = new()
            {
                new() { Width = new(1, GridUnitType.Star) },
                new() { Width = new(1, GridUnitType.Star) },
                new() { Width = new(1, GridUnitType.Star) },
                new() { Width = new(8, GridUnitType.Star) }
            }
        };
        RemoveButton = new() { Text = "X" };
        MoveUpButton = new() { Text = "\u2191" };
        MoveDownButton = new() { Text = "\u2193" };
        RemoveButton.Clicked += OnClicked;
        MoveUpButton.Clicked += OnClicked;
        MoveDownButton.Clicked += OnClicked;
        
        Border.Stroke = WdmBrushes.YELLOW;
        Border.StrokeThickness = 1;
        Border.MinimumHeightRequest = 0;
        base.TitleLabel.IsVisible = false;
        TitleLabel = new()
        {
            Text = EffectName = name,
            HorizontalTextAlignment = TextAlignment.End
        };
        Description = new()
        {
            Text = description
        };
        
        HeaderGrid.Add(RemoveButton, column: 0, row: 0);
        HeaderGrid.Add(MoveUpButton, column: 1, row: 0);
        HeaderGrid.Add(MoveDownButton, column: 2, row: 0);
        HeaderGrid.Add(TitleLabel, column: 3, row: 0);
        Stack.Add(HeaderGrid);
        Stack.Add(Description);
    }

    protected void OnUpdated(object sender, EventArgs args) => EffectUpdated?.Invoke(this, EventArgs.Empty);

    protected void OnClicked(object sender, EventArgs args)
    {
        ButtonType type = sender switch
        {
            _ when sender == RemoveButton => ButtonType.Remove,
            _ when sender == MoveUpButton => ButtonType.MoveUp,
            _ when sender == MoveDownButton => ButtonType.MoveDown,
            _ => ButtonType.IgnoreEvent
        };
        if (type == ButtonType.IgnoreEvent)
            return;
        ButtonClicked?.Invoke(this, new(type));
    }

    public abstract Picture Process(Picture image);
}
