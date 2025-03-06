using Maynard.ImageManipulator.Client.Utilities;

namespace Maynard.ImageManipulator.Client.Controls.Panels;

public class SavedTransformationPanel : Panel
{
    private static readonly BindableProperty COLUMNS_BINDING = BindableProperty.Create(
        propertyName: nameof(Columns), 
        returnType: typeof(int), 
        declaringType: 
        typeof(SavedTransformationPanel), 
        defaultValue: 1, 
        propertyChanged: OnColumnSpanChanged
    );
    
    private static readonly BindableProperty START_KEY_BINDING = BindableProperty.Create(
        propertyName: nameof(UseBuiltIns), 
        returnType: typeof(bool), 
        declaringType: 
        typeof(SavedTransformationPanel), 
        defaultValue: false, 
        propertyChanged: OnBuiltInsChanged
    );

    public int Columns
    {
        get => (int)GetValue(COLUMNS_BINDING);
        init => SetValue(COLUMNS_BINDING, value);
    }

    public bool UseBuiltIns
    {
        get => (bool)GetValue(START_KEY_BINDING);
        init => SetValue(START_KEY_BINDING, value);
    }
    
    private Grid Grid { get; set; }

    public SavedTransformationPanel()
    {
        string data =
            "foobar\uf8ffMaynard.ImageManipulator.Client.Controls.ImageActions.Definitions.BlurDefinition\u231810\u2318\u2325Maynard.ImageManipulator.Client.Controls.ImageActions.Definitions.CropToSquareDefinition\u2318\u2325Maynard.ImageManipulator.Client.Controls.ImageActions.Definitions.RadialDimDefinition\u23181\u2318";
        
        for (int i = 0; i < 100; i++)
            if (Preferences.ContainsKey($"CustomTransformation1{i:00}"))
                Preferences.Remove($"CustomTransformation1{i:00}");
        for (int i = 0; i < 9; i++)
            Preferences.Set($"CustomTransformation10{i}", data.Replace("foobar", $"Xform {i}"));
        
        Grid = new()
        {
            ColumnDefinitions = new()
            {
                new() { Width = new(1, GridUnitType.Star) }
            }
        };

        Stack.Add(Grid);
    }

    public void Populate()
    {
        SavedTransformation[] transformations = TransformationPersistenceManager
            .List()
            .Where(data => data.IsBuiltIn == UseBuiltIns)
            .Select(data => new SavedTransformation(data))
            .ToArray();
        
        Grid.Children.Clear();
        
        int rows = (int)Math.Ceiling(transformations.Length / (float)Columns);
        for (int i = 0; i < transformations.Length; i++)
            Grid.Add(transformations[i], column: 0);
        
        int index = 0;
        for (int column = 0; column < Columns; column++)
            for (int row = 0; row < rows; row++)
                if (index < transformations.Length)
                    Grid.Add(transformations[index++], column: column, row: row);
                else
                    Grid.Add(new EmptySpace(1, 1), column: column, row: row);
    }

    private static void OnColumnSpanChanged(BindableObject bindable, object oldValue, object newValue)
    {
        return;
        if (bindable is not SavedTransformationPanel panel)
            return;
        int columns = (int)newValue;
        panel.Grid.ColumnDefinitions.Clear();
        for (int i = 0; i < columns; i++)
            panel.Grid.ColumnDefinitions.Add(new() { Width = new(1, GridUnitType.Star) });
    }
    
    private static void OnBuiltInsChanged(BindableObject bindable, object oldValue, object newValue)
    {
        return;
        if (bindable is not SavedTransformationPanel panel)
            return;
    }
}

public class SavedTransformation : Panel
{
    private CustomTransformationData Data { get; set; }
    private Label DescriptionLabel { get; set; }
    private Label DetailsLabel { get; set; }
    private Button DeleteButton { get; set; }
    private Button OpenButton { get; set; }
    
    public SavedTransformation(CustomTransformationData data)
    {
        Data = data;
        Border.Stroke = Data.IsBuiltIn
            ? WdmBrushes.SALMON
            : WdmBrushes.YELLOW;
        Title = Data.Description;
        DescriptionLabel = new() { Text = "Steps" };
        DetailsLabel = new() { Text = string.Join(Environment.NewLine, Data.Actions) };
        DeleteButton = new() { Text = "X" };
        OpenButton = new() { Text = "Open" };

        DeleteButton.Clicked += DeleteButtonClicked;
        OpenButton.Clicked += OpenClicked;

        Grid grid = new()
        {
            ColumnDefinitions =
            {
                new() { Width = new(3, GridUnitType.Star) },
                new() { Width = new(7, GridUnitType.Star) },
            }
        };
        grid.Add(DeleteButton, column: 0, row: 0);
        grid.Add(OpenButton, column: 1, row: 0);
        grid.Add(DescriptionLabel, column: 0, row: 1);
        grid.Add(DetailsLabel, column: 1, row: 1);
        Stack.Add(grid);
    }

    private async void DeleteButtonClicked(object sender, EventArgs e)
    {
        Grid parent = (Grid)Parent;
        
        await Tween.Linear(this, panel => panel.Opacity, start: 1, end: 0, seconds: .25);
        parent.Children.Remove(this);
        CustomTransformationData[] before = TransformationPersistenceManager.List();
        TransformationPersistenceManager.Delete(Data.Key);
        CustomTransformationData[] after = TransformationPersistenceManager.List();

        IView view = (IView)parent.Parent;
        while (view.Parent != null && view is not SavedTransformationPanel)
            view = (IView)view.Parent;
        if (view == null)
            return;
        await Tween.Linear(parent, grid => grid.Opacity, start: 1, end: 0, seconds: .25);
        ((SavedTransformationPanel)view).Populate();
        await Tween.Linear(parent, grid => grid.Opacity, start: 0, end: 1, seconds: .25);
    }
    

    private void OpenClicked(object sender, EventArgs e)
    {
        TransformationPersistenceManager.Load(Data.Key);
    }
}