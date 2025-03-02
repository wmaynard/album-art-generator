using Maynard.ImageManipulator.Client.Controls;
using Maynard.Imaging.Utilities;

namespace Maynard.ImageManipulator.Client;

public partial class MainPage : ContentPage
{
    int count = 0;

    public MainPage()
    {
        InitializeComponent();
        new ActionPickerRow(this, OnActionSelected);
        Log.Intercept(OnLogSent);
    }

    private void OnLogSent(object sender, LogEventArgs log)
    {
        
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();

    }

    public void OnOpenDirectoryClicked(object sender, EventArgs e)
    {
        
    }

    public void OnActionSelected(object sender, EventArgs e)
    {
        
    }
}

