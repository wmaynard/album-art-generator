using Maynard.ImageManipulator.Client.Pages;
using Microsoft.Maui.Controls;

namespace Maynard.ImageManipulator.Client;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(TransformationPage), typeof(TransformationPage));
        Routing.RegisterRoute(nameof(LoadPage), typeof(LoadPage));
        Routing.RegisterRoute(nameof(HelpPage), typeof(HelpPage));
    }
}