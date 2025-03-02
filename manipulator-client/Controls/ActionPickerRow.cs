using System;
using System.Collections.Generic;
using System.Linq;
using Maynard.ImageManipulator.Client.Pages;
using Maynard.ImageManipulator.Client.Utilities;
using Maynard.Imaging.Utilities;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace Maynard.ImageManipulator.Client.Controls;

public class ActionPickerRow : HorizontalStackLayout
{
    private MainPage ParentPage;
    private EventHandler SelectionHandler { get; set; }
    private Picker Picker { get; set; }
    private List<Button> Buttons { get; set; } = new();

    public int SelectedIndex
    {
        get => Picker.SelectedIndex;
        set => Picker.SelectedIndex = value;
    }
    
    public ActionPickerRow(MainPage page, EventHandler selectionHandler)
    {
        ParentPage = page;
        SelectionHandler = selectionHandler;
        
        CreatePicker();
        CreateButton("+", OnClick_AddButton);
        CreateButton("-", OnClick_RemoveButton);
        CreateButton("▲", OnClick_UpButton);
        CreateButton("▼", OnClick_DownButton);
        
        VerticalStackLayout parent = (VerticalStackLayout)ParentPage.FindByName("ActionPanel");
        parent.Add(this);
    }

    private void OnClick_AddButton(object sender, EventArgs e) => ParentPage.CreatePicker(this);

    private void Swap(Button clicked, bool above)
    {
        ActionPickerRow[] kids = ((VerticalStackLayout)clicked.Parent.Parent)
            .Children
            .OfType<ActionPickerRow>()
            .ToArray();
        if (kids.Length <= 1)
        {
            Log.Error("Unable to swap action; there is only one action.");
            return;
        }

        int self = -1;
        int other = -1;
        for (int i = 0; i < kids.Length; i++)
            if (kids[i] == clicked.Parent)
            {
                self = i;
                other = above ? i - 1 : i + 1;
                break;
            }

        if (other == -1 || other >= kids.Length)
        {
            Log.Error("Can't swap actions; the target action is out of bounds.");
            return;
        }

        Picker selfPicker = kids[self].Children.OfType<Picker>().FirstOrDefault();
        Picker otherPicker = kids[other].Children.OfType<Picker>().FirstOrDefault();

        if (selfPicker == null || otherPicker == null)
        {
            Log.Error("Can't swap actions; one (or more) of the actions' pickers is null.");
            return;
        }
        
        (selfPicker.SelectedIndex, otherPicker.SelectedIndex) = (otherPicker.SelectedIndex, selfPicker.SelectedIndex);
    }

    private void OnClick_RemoveButton(object sender, EventArgs e)
    {
        Button clicked = (Button)sender;
        VerticalStackLayout panel = (VerticalStackLayout)clicked.Parent.Parent;

        ActionPickerRow[] kids = panel.Children.OfType<ActionPickerRow>().ToArray();
        if (kids.Length <= 1)
        {
            Log.Error("Unable to remove action; doing so would remove the last instance.");
            return;
        }
        
        panel.Children.Remove((IView)clicked.Parent);
        ParentPage.RemovePicker(this);
        
        // clicked.Parent.Parent.RemoveLogicalChild(clicked.Parent);
    }
    
    private void OnClick_UpButton(object sender, EventArgs e) => Swap((Button)sender, true);
    
    private void OnClick_DownButton(object sender, EventArgs e) => Swap((Button)sender, false);

    private Picker CreatePicker()
    {
        Picker = new()
        {
            ItemsSource = ReflectionHelper.GetActionNames(),
            WidthRequest = 150,
            MinimumWidthRequest = 150
        };
        
        Picker.SelectedIndexChanged += SelectionHandler;
        Picker.SelectedIndex = 0;
        Add(Picker);
        return Picker;
    }
    private Button CreateButton(string text, EventHandler clicked)
    {
        Button output = new()
        {
            Text = text
        };
        output.Clicked += clicked;
        Add(output);
        return output;
    }
}