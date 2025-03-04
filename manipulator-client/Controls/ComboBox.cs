namespace Maynard.ImageManipulator.Client.Controls;

public class ComboBox : VerticalStackLayout
{
    private Button NewActionButton { get; set; }
    private Grid Grid { get; set; }
    private VerticalStackLayout NestedStack { get; set; }
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
                new() { Width = new(1, GridUnitType.Star) },
                new() { Width = new(9, GridUnitType.Star) },
            },
        };
        NestedStack = new();

        for (int i = 0; i < 10; i++)
        {
            NestedStack.Add(new Button() { Text = $"Add Action {i}" });
            NestedStack.Add(new EmptySpace(10));
            NestedStack.Add(new Label() { Text = $"Description for {i}" });
            NestedStack.Add(new EmptySpace(30));
        }

        Add(NewActionButton);
        Grid.Add(NestedStack, column: 1, row: 0);
        Add(Grid);
    }

    public void OnClick_NewAction(object sender, EventArgs e)
    {
        
    }
}