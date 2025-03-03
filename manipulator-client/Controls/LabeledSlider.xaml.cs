using Microsoft.Maui.Controls;

namespace Maynard.ImageManipulator.Client.Controls
{
    public partial class LabeledSlider : ContentView
    {
        public static readonly BindableProperty LabelTextProperty =
            BindableProperty.Create(nameof(LabelText), typeof(string), typeof(LabeledSlider), string.Empty, propertyChanged: OnLabelTextChanged);

        public static readonly BindableProperty SliderValueProperty =
            BindableProperty.Create(nameof(SliderValue), typeof(int), typeof(LabeledSlider), 1, BindingMode.TwoWay);

        public LabeledSlider()
        {
            InitializeComponent();
            ValueSlider.ValueChanged += OnSliderValueChanged;
        }

        public string LabelText
        {
            get => (string)GetValue(LabelTextProperty);
            set => SetValue(LabelTextProperty, value);
        }

        public int SliderValue
        {
            get => (int)GetValue(SliderValueProperty);
            set => SetValue(SliderValueProperty, value);
        }

        private static void OnLabelTextChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is LabeledSlider control)
            {
                control.TitleLabel.Text = (string)newValue;
            }
        }

        private void OnSliderValueChanged(object sender, ValueChangedEventArgs e)
        {
            int newValue = (int)Math.Round(e.NewValue);
            SliderValue = newValue;
            ValueSlider.Value = newValue; // Snap to integer values
            TitleLabel.Text = $"{LabelText}: {newValue}"; // Display selected value
        }
    }
}