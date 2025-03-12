using Maynard.ImageManipulator.Client.Utilities;

namespace Maynard.ImageManipulator.Client.Controls;

public class SliderWithLabels : VerticalStackLayout
{
    public string Title
    {
        get => TitleLabel.Text;
        set
        {
            TitleLabel.Text = value;
            TitleLabel.IsVisible = !string.IsNullOrEmpty(value);
        }
    }
    public double Minimum
    {
        get => Slider.Minimum;
        set
        {
            Slider.Minimum = value;
            MinimumLabel.Text = $"{value}";
        }
    }

    public double Maximum
    {
        get => Slider.Maximum;
        set
        {
            Slider.Maximum = value;
            MaximumLabel.Text = $"{value}";
        }
    }
    
    public double Value
    {
        get => Slider.Value;
        set
        {
            Slider.Value = value;
            CurrentLabel.Text = $"{value}";
        }
    }

    public Color ThumbColor
    {
        get => Slider.ThumbColor;
        set => Slider.ThumbColor = value;
    }

    public Color MinimumTrackColor
    {
        get => Slider.MinimumTrackColor;
        set => Slider.MinimumTrackColor = value;
    }

    public Color MaximumTrackColor
    {
        get => Slider.MaximumTrackColor;
        set => Slider.MaximumTrackColor = value;
    }
    
    private Slider Slider { get; set; }
    private Grid Grid { get; set; }
    private Label MinimumLabel { get; set; }
    private Label MaximumLabel { get; set; }
    private Label CurrentLabel { get; set; }
    private Label TitleLabel { get; set; }
    public EventHandler<ValueChangedEventArgs> ValueChanged;
    
    public SliderWithLabels()
    {
        Grid = new()
        {
            ColumnDefinitions = new()
            {
                new() { Width = new(1, GridUnitType.Star) },
                new() { Width = new(1, GridUnitType.Star) },
                new() { Width = new(1, GridUnitType.Star) }
            }
        };
        MinimumLabel = new() { Text = "0" };
        CurrentLabel = new() { Text = "0", HorizontalTextAlignment = TextAlignment.Center, TextColor = WdmColors.LIME };
        MaximumLabel = new() { Text = "100", HorizontalTextAlignment = TextAlignment.End };
        TitleLabel = new() { Text = "" };
        
        Slider = new()
        {
            Minimum = 0,
            Maximum = 100,
            Value = 0, 
            ThumbColor = WdmColors.BLUE,
            MinimumTrackColor = WdmColors.GREEN,
            MaximumTrackColor = WdmColors.WHITE,
        };
        Slider.ValueChanged += OnValueChanged;

        Add(TitleLabel);
        Grid.Add(MinimumLabel, column: 0, row: 0);
        Grid.Add(CurrentLabel, column: 1, row: 0);
        Grid.Add(MaximumLabel, column: 2, row: 0);
        Add(Grid);
        Add(Slider);
    }

    private async void OnValueChanged(object sender, ValueChangedEventArgs args)
    {
        int previous = (int)Math.Round(args.OldValue);
        int current = (int)Math.Round(args.NewValue);
            
        Slider.Value = current;
        await Gui.Update(() => CurrentLabel.Text = $"{current}");
        if (previous != current)
            ValueChanged?.Invoke(this, new(previous, current));
    }
}