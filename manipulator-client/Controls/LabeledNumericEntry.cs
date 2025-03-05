using System.Text.RegularExpressions;
using Maynard.ImageManipulator.Client.Utilities;

namespace Maynard.ImageManipulator.Client.Controls;


public partial class LabeledNumericEntry : Grid
{
    private int MinimumValue { get; init; }
    private int MaximumValue { get; init; }
    private Label Label { get; init; }
    private Label RangeLabel { get; init; }
    private Entry Entry { get; init; }
    private string PreviousText { get; set; }
    public int Value => Parse();
    
    public LabeledNumericEntry(string label, int minimum, int maximum)
    {
        if (minimum >= maximum)
            throw new Exception("Minimum value must be less than maximum value.");
        
        ColumnDefinitions = new()
        {
            new() { Width = new(1, GridUnitType.Star) },
            new() { Width = new(1, GridUnitType.Star) },
        };
        Label = new() { Text = label };
        RangeLabel = new()
        {
            Text = $"[{minimum} - {maximum}]",
            HorizontalTextAlignment = TextAlignment.End
        };
        Entry = new() { Keyboard = Keyboard.Numeric, };
        Entry.TextChanged += OnTextChanged;
        
        this.Add(Label, column: 0, row: 0);
        this.Add(Entry, column: 1, row: 0);
        this.Add(RangeLabel, column: 1, row: 1);
    }

    private int Parse() => int.TryParse(IntegerRegex().Replace(Entry.Text, string.Empty), out int value)
        ? value
        : Math.Min(MaximumValue, Math.Max(MinimumValue, 0));

    private void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        int value = Parse();

        if (!value.ToString().Equals(Entry.Text) || value < MinimumValue || value > MaximumValue)
        {
            Entry.TextChanged -= OnTextChanged;
            Entry.Text = PreviousText;
            Entry.TextChanged += OnTextChanged;
            Log.Warn("Invalid entry.  Value must be numeric only.");
        }
        
        PreviousText = Entry.Text;
    }

    [GeneratedRegex(@"^(?!-)\d+")]
    private static partial Regex IntegerRegex();
}