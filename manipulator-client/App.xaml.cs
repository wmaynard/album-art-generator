using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace Maynard.ImageManipulator.Client;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }
    
    protected override Window CreateWindow(IActivationState activationState)
    {
        Window window = new(new AppShell());

        // Subscribe to the Destroying event
        window.Destroying += OnWindowDestroying;

        return window;
    }

    private void OnWindowDestroying(object sender, EventArgs e)
    {
        // Perform cleanup or save session data here
        SaveSessionData();
        // Console.WriteLine("Application is closing.");
    }

    private void SaveSessionData()
    {
        // Example: Saving a simple session timestamp
        Preferences.Set("LastSessionTime", DateTime.UtcNow.ToString());
    }
}