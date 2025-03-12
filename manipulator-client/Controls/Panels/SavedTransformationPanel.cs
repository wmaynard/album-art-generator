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
        
        if (UseBuiltIns)
            foreach (SavedTransformation transformation in transformations)
                transformation.HideDeleteButton();
        
        Grid.Children.Clear();
        
        int rows = (int)Math.Ceiling(transformations.Length / (float)Columns);
        for (int i = 0; i < transformations.Length; i++)
            Grid.Add(transformations[i], column: 0);
        
        int index = 0;
        for (int row = 0; row < rows; row++)
            for (int column = 0; column < Columns; column++)
                if (index < transformations.Length)
                    Grid.Add(transformations[index++], column: column, row: row);
                else
                    Grid.Add(new EmptySpace(1, 1), column: column, row: row);
    }

    private static void OnColumnSpanChanged(BindableObject bindable, object oldValue, object newValue)
    {
        return;
    }
    
    private static void OnBuiltInsChanged(BindableObject bindable, object oldValue, object newValue)
    {
        return;
    }
}