using Maynard.ImageManipulator.Client.Utilities;

namespace Maynard.ImageManipulator.Client.Controls;

public class PanelFromCode : ContentView
{
    private static readonly BindableProperty LABEL_TEXT_PROPERTY =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(PanelFromCode), "Default Text",
            propertyChanged: OnLabelTextChanged);

    public string Title
    {
        get => (string)GetValue(LABEL_TEXT_PROPERTY);
        set => SetValue(LABEL_TEXT_PROPERTY, value);
    }

    private static void OnLabelTextChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is PanelFromCode panel && newValue is string newText)
            panel.Label.Text = newText;
    }

    // private readonly Label _titleLabel;
    private Label Label { get; set; }
    private VerticalStackLayout Stack { get; set; }
    private ScrollView Scroller { get; set; }
    private Border Border { get; set; }

    public PanelFromCode()
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
        Stack.Children.Add(Label);

        Scroller = new()
        {
            HorizontalOptions = LayoutOptions.FillAndExpand,
            VerticalOptions = LayoutOptions.FillAndExpand,
        };

        Border = new()
        {
            Stroke = WdmBrushes.BLUE,
            StrokeThickness = 2,
            Padding = 5,
            Background = WdmBrushes.BLACK,
            HorizontalOptions = LayoutOptions.FillAndExpand,
            VerticalOptions = LayoutOptions.FillAndExpand,
            MinimumHeightRequest = 300
        };

        Stack.Add(Label);
        Scroller.Content = Stack;
        Border.Content = Scroller;
        Content = Border;
    }
}