using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Maui.Storage;
using Foundation;
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
            Log.Verbose("Opening directory...");

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
        #if MACCATALYST15_0_OR_GREATER
        TrySavePermissions();
        #endif
        Log.Verbose($"Saving '{Id}': '{CurrentDirectory}...");
        Preferences.Set(Id, CurrentDirectory);
    }

    private void TrySavePermissions()
    {
        #if MACCATALYST15_0_OR_GREATER
        try
        {
            NSData bookmark = NSUrl
                .FromFilename(CurrentDirectory)
                .CreateBookmarkData
                (
                    #pragma warning disable CA1416
                    options: NSUrlBookmarkCreationOptions.WithSecurityScope, 
                    #pragma warning restore CA1416
                    resourceValues: null, 
                    relativeUrl: null, 
                    out NSError error
                );
            // Not sure why Rider thinks this is always false - it is not.
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (error == null)
            {
                Preferences.Set($"Bookmark{Id}", Convert.ToBase64String(bookmark.ToArray()));
                Log.Verbose("Bookmarked permissions.");
            }
        }
        catch
        {
            Log.Error("Failed to save directory with read / write permissions.  The app will require the user to select the directory in the future to read / write.");
        }
        #endif
    }

    private void TryLoadWithPermissions()
    {
        #if MACCATALYST15_0_OR_GREATER
        string bookmark = $"Bookmark{Id}";
        if (!Preferences.ContainsKey(bookmark))
            return;
        
        Log.Verbose("Loading bookmark...");
        byte[] bookmarkData = Convert.FromBase64String(Preferences.Get(bookmark, ""));
        
        NSUrl url = NSUrl.FromBookmarkData(
            data: NSData.FromArray(bookmarkData), 
            #pragma warning disable CA1416
            options: NSUrlBookmarkResolutionOptions.WithSecurityScope, 
            #pragma warning restore CA1416
            relativeToUrl: null,
            out bool isStale, 
            out NSError error
        );
        
        if (isStale)
            Log.Warn($"Permissions for the bookmarked folder are stale; access may need to be granted manually by picking the directory again. ({url.Path})");

        // Not sure why Rider thinks this is always false - it is not.
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        if (error == null)
        {
            if (url.StartAccessingSecurityScopedResource())
                Log.Verbose($"Access restored to {url.Path}");
            else
                Log.Error($"Failed to restore directory access. ({url.Path})");
        }
        else
        {
            Log.Warn("Unable to restore permissions; access will need to be granted manually by picking the directory again. ({url.Path})");
            Preferences.Remove(bookmark);
        }
        #endif
    }
    
    public void Load()
    {
        Log.Verbose($"Loading '{Id}'...");
        if (string.IsNullOrWhiteSpace(Id))
            return;
        string path = Preferences.Get(Id, CurrentDirectory);

        #if MACCATALYST15_0_OR_GREATER
        TryLoadWithPermissions();
        #endif
        
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
        OnDirectorySelected?.Invoke(this, new()
        {
            Path = path
        });
        // Entry.CursorPosition = partial.Length; // this doesn't work unfortunately
        // Entry.SelectionLength = 0;
    }
}