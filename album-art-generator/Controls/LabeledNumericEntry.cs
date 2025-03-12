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
    public EventHandler ValueChanged;

    public int Value
    {
        get => Parse();
        set => Entry.Text = value.ToString();
    }
    
    public LabeledNumericEntry(string label, int minimum, int maximum)
    {
        MinimumValue = minimum;
        MaximumValue = maximum;
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
        Entry.Text = MinimumValue.ToString();
        Entry.TextChanged += OnTextChanged;
        // Entry.Unfocused += OnFocusLost;
        
        this.Add(Label, column: 0, row: 0);
        this.Add(Entry, column: 1, row: 0);
        this.Add(RangeLabel, column: 1, row: 1);
    }

    private int Parse() => int.TryParse(IntegerRegex().Match(Entry.Text ?? "").Value, out int value)
        ? value
        : Math.Min(MaximumValue, Math.Max(MinimumValue, 0));

    private CancellationTokenSource _cts = new();
    private async void OnTextChanged(object sender, EventArgs e)
    {
        await _cts.CancelAsync();
        _cts = new();
        CancellationToken token = _cts.Token;
        try
        {
            await Task.Run(async () =>
            {
                Thread.Sleep(2_500);
                if (token.IsCancellationRequested)
                    return;
            
                int value = Parse();

                bool numeric = value.ToString().Equals(Entry.Text);
                bool outOfRange = value < MinimumValue || value > MaximumValue;
                if (!numeric || outOfRange)
                {
                    await Gui.Update(() =>
                    {
                        Entry.TextChanged -= OnTextChanged;
                        Entry.Text = ((int)Math.Min(MaximumValue, Math.Max(MinimumValue, value))).ToString();
                        Entry.TextChanged += OnTextChanged;
                    });
                    Log.Warn("Invalid entry.  Value must be numeric only.");
                }
                
                if (PreviousText != Entry.Text)
                    ValueChanged?.Invoke(this, EventArgs.Empty);
                PreviousText = Entry.Text;
            }, token);
        }
        catch (TaskCanceledException)
        {
            Log.Verbose("Numeric entry validation was cancelled.");
        }
    }

    [GeneratedRegex(@"\d+")]
    private static partial Regex IntegerRegex();
}