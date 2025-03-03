using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Maynard.Imaging.Utilities;

namespace Maynard.ImageManipulator.Client.Controls;


public partial class ActionAdder : ContentView
{
    public static readonly BindableProperty ItemsSourceProperty =
        BindableProperty.Create(nameof(ItemsSource), typeof(ObservableCollection<string>), typeof(ActionAdder), new ObservableCollection<string>());

    public static readonly BindableProperty SelectedItemProperty =
        BindableProperty.Create(nameof(SelectedItem), typeof(string), typeof(ActionAdder), null, BindingMode.TwoWay);

    public event EventHandler AddClicked;
    public event EventHandler RemoveClicked;

    public ActionAdder()
    {
        InitializeComponent();
        ItemPicker.SelectedIndexChanged += OnPickerSelectionChanged;
        ItemPicker.ItemsSource = ReflectionHelper.GetActionNames();
        ItemPicker.SelectedIndex = 0;
    }

    public ObservableCollection<string> ItemsSource
    {
        get => (ObservableCollection<string>)GetValue(ItemsSourceProperty);
        set
        {
            SetValue(ItemsSourceProperty, value);
            ItemPicker.ItemsSource = value;
        }
    }

    public string SelectedItem
    {
        get => (string)GetValue(SelectedItemProperty);
        set
        {
            SetValue(SelectedItemProperty, value);
            ItemPicker.SelectedItem = value;
        }
    }

    private void OnPickerSelectionChanged(object sender, EventArgs e)
    {
        SelectedItem = (string)ItemPicker.SelectedItem;
    }
}