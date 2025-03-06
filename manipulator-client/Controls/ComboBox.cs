using System.Reflection;
using Maynard.ImageManipulator.Client.Controls.ImageActions.Buttons;
using Maynard.ImageManipulator.Client.Controls.ImageActions.Definitions;
using Maynard.ImageManipulator.Client.Enums;
using Maynard.ImageManipulator.Client.Events;
using Maynard.ImageManipulator.Client.Utilities;
using Maynard.ImageManipulator.Client.Controls.Panels;
using Maynard.ImageManipulator.Client.Interfaces;

namespace Maynard.ImageManipulator.Client.Controls;

public class ComboBox : VerticalStackLayout, IPreferential
{
    private Button NewActionButton { get; set; }
    private Grid Grid { get; set; }
    private FadeInVerticalStack NestedStack { get; set; }
    public EventHandler<ComboBoxUpdatedArgs> Updated { get; set; }
    public List<ActionDefinition> Actions { get; set; } = new();
    public ComboBox()
    {
        Spacing = 5;
        NewActionButton = new()
        {
            Text = "New Action..."
        };
        NewActionButton.Clicked += OnClick_NewAction;
        Grid = new()
        {
            ColumnDefinitions =
            {
                new() { Width = new(2, GridUnitType.Star) },
                new() { Width = new(8, GridUnitType.Star) },
            },
        };
        NestedStack = new();

        NestedStack.Add(new EmptySpace(10));
        
        ActionButton[] buttons = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(ActionButton)))
            .Select(type => (ActionButton)Activator.CreateInstance(type))
            .ToArray();
        foreach (ActionButton button in buttons)
        {
            button.Clicked += ActionButtonClicked;
            NestedStack.Add(button);
        }

        Add(NewActionButton);
        Grid.Add(NestedStack, column: 1, row: 0);
        Add(Grid);
        Load();
    }

    private async Task InsertChildAt(IView view, int? index = null)
    {
        if (Children.Contains(view))
            Children.Remove(view);
        index ??= Children.IndexOf(NewActionButton);
        if (view is VisualElement visual)
        {
            visual.Opacity = 0;
            Children.Insert((int)index, view);
            await Tween.Linear(visual, element => element.Opacity, 0, 1, seconds: .25);
        }
    }

    private async void ActionButtonClicked(object sender, ActionButtonEventArgs args)
    {
        ActionDefinition definition = args.Control;
        await InsertChildAt(definition);
        Actions.Add(definition);
        NestedStack.Hide();
        definition.ButtonClicked += DefinitionButtonClicked;
        definition.EffectUpdated += FireEventUpdated;
        FireEventUpdated();
    }

    private async void DefinitionButtonClicked(object sender, ButtonClickEventArgs args)
    {
        if (sender is not ActionDefinition definition)
            return;
        int childIndex = Children.IndexOf(definition);
        int index = Actions.IndexOf(definition);
        if (index != childIndex)
            Log.Error("Indexes are out of sync; this should never happen.");
        switch (args.Type)
        {
            case ButtonType.Remove:
                Actions.Remove(definition);
                Children.Remove(definition);
                break;
            case ButtonType.MoveUp:
                if (index == 0)
                {
                    Log.Warn("Cannot move up when the definition is the first one.");
                    return;
                }
                await InsertChildAt(definition, childIndex - 1);
                break;
            case ButtonType.MoveDown:
                if (Children[childIndex + 1] is not ActionDefinition)
                {
                    Log.Warn("Cannot move down when the definition is the last one.");
                    return;
                }
                await InsertChildAt(definition, childIndex + 1);
                break;
            default:
                return;
        }
        FireEventUpdated();
    }

    private void FireEventUpdated(object sender = null, EventArgs e = null)
    {
        Actions.Clear();
        Actions.AddRange(Children.OfType<ActionDefinition>());
        Updated?.Invoke(sender ?? this, new(Actions.ToArray()));
        Save();
    }
    private void OnClick_NewAction(object sender, EventArgs e) => NestedStack.Show();
    public string Id { get; init; } = PreferenceKeys.ACTIONS;
    public void Reset() => Load();
    public string TransformationString => ActionDefinition.Serialize(Actions.ToArray());

    public void Save()
    {
        Preferences.Set(Id, ActionDefinition.Serialize(Actions.ToArray()));
        Log.Info("Current transformation saved.");
    }

    public async void Load() => Load(null);
    public async void Load(string fromTransformation)
    {
        string data = fromTransformation ?? Preferences.Get(Id, null);
        if (string.IsNullOrWhiteSpace(data))
        {
            Log.Info("No saved actions defined.");
            return;
        }

        try
        {
            foreach (ActionDefinition definition in Children.OfType<ActionDefinition>())
                Children.Remove(definition);
            Actions = ActionDefinition.Deserialize(data).ToList();
            for (int i = 0; i < Actions.Count; i++)
            {
                Actions[i].ButtonClicked += DefinitionButtonClicked;
                Actions[i].EffectUpdated += FireEventUpdated;
                await InsertChildAt(Actions[i], i);
            }
        }
        catch (Exception e)
        {
            Log.Error($"Unable to load action definitions.  The saved data will be lost. ({e.Message})");
            Preferences.Set(Id, null);
        }
        Updated?.Invoke(this, new(Actions.ToArray()));
    }
}

public class ComboBoxUpdatedArgs(ActionDefinition[] actions) : EventArgs
{
    public ActionDefinition[] Actions { get; set; } = actions;
}