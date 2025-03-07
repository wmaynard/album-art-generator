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
        // string data =
        //     "foobar\uf8ffMaynard.ImageManipulator.Client.Controls.ImageActions.Definitions.BlurDefinition\u231810\u2318\u2325Maynard.ImageManipulator.Client.Controls.ImageActions.Definitions.CropToSquareDefinition\u2318\u2325Maynard.ImageManipulator.Client.Controls.ImageActions.Definitions.RadialDimDefinition\u23181\u2318";
        //
        // for (int i = 0; i < 100; i++)
        //     if (Preferences.ContainsKey($"CustomTransformation1{i:00}"))
        //         Preferences.Remove($"CustomTransformation1{i:00}");
        // for (int i = 0; i < 9; i++)
        //     Preferences.Set($"CustomTransformation10{i}", data.Replace("foobar", $"Xform {i}"));
        
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