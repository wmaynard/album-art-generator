using Maynard.ImageManipulator.Client.Utilities;

namespace Maynard.ImageManipulator.Client.Controls.Panels;

public class ImagePreview : Panel
{
    public string Title
    {
        get => TitleLabel.Text;
        set => TitleLabel.Text = value;
    }

    public string Base64
    {
        set => ReloadImage(value);
    }

    public bool Refreshable
    {
        get => RefreshButton.IsEnabled;
        set => RefreshButton.IsVisible = RefreshButton.IsEnabled = value;
    }
    
    public Image Image { get; set; }
    private Button RefreshButton { get; init; }
    public EventHandler OnRefresh { get; set; }

    public ImagePreview()
    {
        Image = new();
        RefreshButton = new()
        {
            Text = "Refresh",
            IsEnabled = false,
            IsVisible = false
        };
        RefreshButton.Clicked += RefreshClicked;
        
        Stack.Add(Image);
        Stack.Add(RefreshButton);
    }

    private ImageSource UpdateImagePath(string path)
    {
        string src1 = System.IO.Path.Combine(Microsoft.Maui.Storage.FileSystem.AppDataDirectory, path);
        string absolute = System.IO.Path.Combine(Directory.GetCurrentDirectory(), path);
        return ImageSource.FromFile(absolute);
    }

    private void ReloadImage(string base64)
    {
        Gui.Update(() =>
        {
            byte[] imageBytes = Convert.FromBase64String(base64);
            Image.Source = ImageSource.FromStream(() => new MemoryStream(imageBytes));
        });
    }

    private void RefreshClicked(object sender, EventArgs e)
    {
        OnRefresh?.Invoke(this, EventArgs.Empty);
    }
}