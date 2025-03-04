using Maynard.ImageManipulator.Client.Utilities;

namespace Maynard.ImageManipulator.Client.Controls;

public class ComboBox : VerticalStackLayout
{
    private Button NewActionButton { get; set; }
    private Grid Grid { get; set; }
    private FadeInVerticalStack NestedStack { get; set; }
    public ComboBox()
    {
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
                // new() { Width = new(1, GridUnitType.Star) },
            },
        };
        NestedStack = new();

        NestedStack.Add(new EmptySpace(10));
        
        NestedStack.Add(new BlurButton { Clicked = ActionButtonClicked });
        for (int i = 0; i < 3; i++)
        {
            // NestedStack.Add(new Button { Text = $"Add Action {i}" });
            NestedStack.Add(new EmptySpace(10));
            NestedStack.Add(new Label { Text = $"Description for {i}" });
            NestedStack.Add(new EmptySpace(30));
        }

        Add(NewActionButton);
        Grid.Add(NestedStack, column: 1, row: 0);
        Add(Grid);
    }

    private async Task InsertChild(IView view)
    {
        int index = Children.IndexOf(NewActionButton);
        if (view is VisualElement visual)
        {
            visual.Opacity = 0;
            Children.Insert(index, view);
            await Tween.Linear(visual, element => element.Opacity, 0, 1, seconds: .25);
        }
    }

    public async void ActionButtonClicked(object sender, ActionButtonEventArgs args)
    {
        ActionDefinition definition = args.Control;
        await InsertChild(definition);
        NestedStack.Hide();
    }

    public void OnClick_NewAction(object sender, EventArgs e)
    {
        NestedStack.Show();
    }
}