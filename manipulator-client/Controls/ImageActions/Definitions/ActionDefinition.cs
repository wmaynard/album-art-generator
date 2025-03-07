using System.Text;
using Maynard.ImageManipulator.Client.Controls.ImageActions.Buttons;
using Maynard.ImageManipulator.Client.Controls.Panels;
using Maynard.ImageManipulator.Client.Enums;
using Maynard.ImageManipulator.Client.Events;
using Maynard.ImageManipulator.Client.Interfaces;
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
    public abstract Func<Picture, string, Picture> GenerateDelegate();
    
    public abstract object[] ConfigurableValues { get; }

    private const char SEPARATOR = '\u2318';
    private const char CLASS_SEPARATOR = '\u2325';
    public string Serialize()
    {
        StringBuilder sb = new();
        sb.Append(GetType().FullName);
        sb.Append(SEPARATOR);
        if (ConfigurableValues != null)
            foreach (object value in ConfigurableValues)
            {
                sb.Append(value);
                sb.Append(SEPARATOR);
            }

        return sb.ToString();
    }
    public static string Serialize(params ActionDefinition[] actions) 
        => string.Join(CLASS_SEPARATOR, actions.Select(action => action.Serialize()));

    // This is WET with Deserialize
    public static string[] Describe(string data)
    {
        List<string> output = new();
        string[] definitionStrings = data.Split(CLASS_SEPARATOR);
        foreach (string definitionString in definitionStrings)
        {
            string[] parts = definitionString.Split(CLASS_SEPARATOR);
            string name = parts.First();
            name = name[(name.LastIndexOf('.') + 1)..];
            string[] parameters = parts.Skip(1).ToArray();
            output.Add($"{name}({string.Join(", ", parameters)})");
        }

        return output.ToArray();
    }
    public static ActionDefinition[] Deserialize(string data)
    {
        List<ActionDefinition> output = new();
        string[] definitionStrings = data.Split(CLASS_SEPARATOR);
        foreach (string definitionString in definitionStrings)
        {
            string[] parts = definitionString.Split(SEPARATOR);
            string name = parts.First();
            string[] parameters = parts.Skip(1).ToArray();
            ActionDefinition instance = (ActionDefinition)Activator.CreateInstance(Type.GetType(name));
            if (instance == null)
            {
                Log.Error($"Failed to load an instance of {name[(name.LastIndexOf('.') + 1)..]}, it will be ignored.");
                continue;
            }

            instance.Deserialize(parameters);
            output.Add(instance);
        }

        return output.ToArray();
    }

    protected abstract void Deserialize(string[] values);
}