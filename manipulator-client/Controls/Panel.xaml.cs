using Microsoft.Maui.Controls;

using Microsoft.Maui.Controls;

namespace Maynard.ImageManipulator.Client.Controls;

public partial class Panel : ContentView
{
    public static readonly BindableProperty LABEL_TEXT_PROPERTY =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(Panel), "Default Text",
            propertyChanged: OnLabelTextChanged);

    public string Title
    {
        get => (string)GetValue(LABEL_TEXT_PROPERTY);
        set => SetValue(LABEL_TEXT_PROPERTY, value);
    }

    private static void OnLabelTextChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is Panel panel && newValue is string newText)
            panel.LabelTitle.Text = newText;
    }

    public Panel() => InitializeComponent();
}
