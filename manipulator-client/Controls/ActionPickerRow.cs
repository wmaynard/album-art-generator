using Maynard.Imaging.Utilities;

namespace Maynard.ImageManipulator.Client.Controls;

public class ActionPickerRow : HorizontalStackLayout
{
    private ContentPage ParentPage;
    private EventHandler SelectionHandler { get; set; }
    private Picker Picker { get; set; }
    private List<Button> Buttons { get; set; } = new();
    
    public ActionPickerRow(ContentPage page, EventHandler selectionHandler)
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

    private void OnClick_AddButton(object sender, EventArgs e)
    {
        new ActionPickerRow(ParentPage, SelectionHandler);
    }

    private void OnClick_RemoveButton(object sender, EventArgs e)
    {
        Button clicked = (Button)sender;
        VerticalStackLayout panel = (VerticalStackLayout)clicked.Parent.Parent;

        HorizontalStackLayout[] kids = panel.Children.OfType<HorizontalStackLayout>().ToArray();
        
        // clicked.Parent.Parent.RemoveLogicalChild(clicked.Parent);
    }
    
    private void OnClick_UpButton(object sender, EventArgs e)
    {
        
    }
    
    private void OnClick_DownButton(object sender, EventArgs e)
    {
        
    }

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