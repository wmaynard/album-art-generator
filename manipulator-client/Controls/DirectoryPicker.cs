using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Maui.Storage;
using Maynard.ImageManipulator.Client.Events;
using Maynard.ImageManipulator.Client.Interfaces;
using Maynard.ImageManipulator.Client.Utilities;

namespace Maynard.ImageManipulator.Client.Controls;


public class DirectoryPicker : HorizontalStackLayout, IPreferential
{
    public string Id { get; init; }
    protected Entry Entry { get; set; }
    protected Button OpenButton { get; set; }
    public string CurrentDirectory { get; set; }
    private static readonly string HOME_DIRECTORY = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    public EventHandler<DirectorySelectedEventArgs> OnDirectorySelected { get; set; }
    public EventHandler<DirectorySelectedEventArgs> OnReset { get; set; }
    
    public DirectoryPicker()
    {
        Spacing = 5;
        Entry = new()
        {
            IsReadOnly = true
        };
        OpenButton = new()
        {
            Text = "..."
        };
        Grid grid = new()
        {
            ColumnDefinitions =
            {
                new () { Width = new(8, GridUnitType.Star) },
                new () { Width = new(2, GridUnitType.Star) },
            },
            MinimumWidthRequest = 400,
            MaximumWidthRequest = 400
        };
        OpenButton.Clicked += OnOpenDirectoryClicked;
        
        grid.Add(Entry, column: 0);
        grid.Add(OpenButton, column: 1);
        Children.Add(grid);
        Load();
    }
    
    private async void OnOpenDirectoryClicked(object sender, EventArgs args)
    {
        try
        {
            Log.Info("Opening directory...");

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                FolderPickerResult result = await FolderPicker.PickAsync(CurrentDirectory ?? HOME_DIRECTORY);

                if (!result.IsSuccessful || result.Folder == null)
                {
                    Log.Error($"Unable to pick a directory.  {result.Exception?.Message}");
                    return;
                }

                string path = result.Folder.Path;
                CurrentDirectory = path;
                Tooltip.Set(Entry, path);
                OnDirectorySelected?.Invoke(this, new()
                {
                    Path = path,
                    Directory = new(path)
                });
                Save();
                Load();
            });
        }
        catch (Exception e)
        {
            Log.Error($"Something went wrong trying to handle the open directory callback. ({e.Message})");
        }
    }

    public void Reset()
    {
        CurrentDirectory = null;
        Entry.Text = "";
        OnReset?.Invoke(this, null);
    }

    public void Save()
    {
        Log.Info($"Saving '{Id}': '{CurrentDirectory}...");
        Preferences.Set(Id, CurrentDirectory);
    }

    public void Load()
    {
        Log.Info($"Loading '{Id}'...");
        if (string.IsNullOrWhiteSpace(Id))
            return;
        string path = Preferences.Get(Id, CurrentDirectory);
        if (string.IsNullOrWhiteSpace(path))
            path = HOME_DIRECTORY;
        CurrentDirectory = path;
        
        string partial = string
            .Join(Path.DirectorySeparatorChar, path
                .TrimEnd(Path.DirectorySeparatorChar)
                .Split(Path.DirectorySeparatorChar)
                .TakeLast(3)
                .Select(part => part.Length > 20
                    ? $"{part[..5]}...{part[^10..]}"
                    : part
                )
            );
        Entry.Text = partial == path
            ? path
            : $"..{Path.DirectorySeparatorChar}{partial}";
        // Entry.CursorPosition = partial.Length; // this doesn't work unfortunately
        // Entry.SelectionLength = 0;
    }
}