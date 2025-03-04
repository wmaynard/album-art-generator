using Maynard.ImageManipulator.Client.Utilities;

namespace Maynard.ImageManipulator.Client.Controls;

public class Panel : ContentView
{
    private static readonly BindableProperty KEY_LABEL_TEXT =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(Panel), "Default Text",
            propertyChanged: OnLabelTextChanged);

    public string Title
    {
        get => (string)GetValue(KEY_LABEL_TEXT);
        set => SetValue(KEY_LABEL_TEXT, value);
    }

    private static void OnLabelTextChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is Panel panel && newValue is string newText)
            panel.Label.Text = newText;
    }

    // private readonly Label _titleLabel;
    private Label Label { get; set; }
    protected VerticalStackLayout Stack { get; set; }
    protected ScrollView Scroller { get; set; }
    protected Border Border { get; set; }
    
    public Panel()
    {
        Label = new()
        {
            Text = Title ?? "Placeholder",
            HorizontalTextAlignment = TextAlignment.Center,
            TextColor = WdmColors.WHITE
        };

        Stack = new()
        {
            Spacing = 10,
            Padding = 10
        };

        Scroller = new()
        {
            HorizontalOptions = LayoutOptions.FillAndExpand,
            VerticalOptions = LayoutOptions.FillAndExpand
        };

        Border = new()
        {
            Stroke = WdmBrushes.BLUE,
            StrokeThickness = 2,
            Padding = 5,
            Background = WdmBrushes.BLACK,
            HorizontalOptions = LayoutOptions.FillAndExpand,
            VerticalOptions = LayoutOptions.FillAndExpand,
            MinimumHeightRequest = 300,
        };

        Stack.Children.Add(Label);
        Scroller.Content = Stack;
        Border.Content = Scroller;
        Content = Border;
    }
}