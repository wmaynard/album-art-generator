using CommunityToolkit.Maui.Views;
using Maynard.ImageManipulator.Client.Utilities;

namespace Maynard.ImageManipulator.Client.Controls;


public class SaveTransformationPopup : Popup
{
    private Entry Entry { get; init; }
    private Button Save { get; init; }
    public SaveTransformationPopup()
    {
        Entry = new()
        {
            Placeholder = "Enter a description"
        };
        Save = new()
        {
            Text = "Save"
        };

        Save.Clicked += OnClick;

        Content = new VerticalStackLayout
        {
            BackgroundColor = WdmColors.BLACK,
            MinimumWidthRequest = 450,
            Children = { Entry, Save },
            Padding = 20
        };
    }

    private void OnClick(object sender, EventArgs e)
    {
        string description = string.IsNullOrWhiteSpace(Entry.Text)
            ? "Unnamed Transformation"
            : Entry.Text;
        TransformationPersistenceManager.Save(description);
        Log.Info($"Saved transformation: {description}");
        Close(Entry.Text);
    }
}