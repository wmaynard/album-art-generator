using CommunityToolkit.Maui.Storage;
using Maynard.ImageManipulator.Client.Utilities;
using Font = Microsoft.Maui.Graphics.Font;

namespace Maynard.ImageManipulator.Client.Controls;

public class DirectoryPanel : Panel
{
    protected DirectoryPicker Picker { get; set; }
    public string CurrentDirectory => Picker.CurrentDirectory;
    private Label ScannedDirectoryTitle { get; set; }
    private Label ScannedDirectoryCount { get; set; }
    private Label ScannedFilesTitle { get; set; }
    private Label ScannedFilesCount { get; set; }
    private Label ScanFailuresTitle { get; set; }
    private Label ScanFailuresCount { get; set; }
    private Grid ResultsGrid { get; set; }
    private ActivityIndicator Throbber { get; set; }
    
    private List<string> ScannedImagePaths { get; set; } = new();
    private Button ScanAgain { get; set; }
    
    public DirectoryPanel()
    {
        Title = "Current Directory";

        Picker = new()
        {
            OnDirectorySelected = OnDirectoryChanged
        };
        ScannedDirectoryTitle = new() { HorizontalTextAlignment = TextAlignment.End, Text = "Scanned Directories: " };
        ScannedDirectoryCount = new() { HorizontalTextAlignment = TextAlignment.End, Text = "0" };
        ScannedFilesTitle = new() { HorizontalTextAlignment = TextAlignment.End, Text = "Files found: " };
        ScannedFilesCount = new() { HorizontalTextAlignment = TextAlignment.End, Text = "0" };
        ScanFailuresTitle = new() { HorizontalTextAlignment = TextAlignment.End, Text = "Skipped Directories: " };
        ScanFailuresCount = new() { HorizontalTextAlignment = TextAlignment.End, Text = "0" };
        
        ResultsGrid = new()
        {
            ColumnDefinitions =
            {
                new () { Width = new(8, GridUnitType.Star) },
                new () { Width = new(2, GridUnitType.Star) },
            },
            MinimumWidthRequest = 400,
            MaximumWidthRequest = 400
        };
        
        Throbber = new()
        {
            IsRunning = false, // Initially hidden
            IsVisible = false,
            Color = WdmColors.SALMON,
            Scale = 2
        };
        ScanAgain = new()
        {
            Text = "Scan Again",
            IsVisible = false
        };
        ScanAgain.Clicked += OnScanAgainClicked;
        
        ResultsGrid.Add(ScannedDirectoryTitle, column: 0, row: 0);
        ResultsGrid.Add(ScannedDirectoryCount, column: 1, row: 0);
        ResultsGrid.Add(ScanFailuresTitle, column: 0, row: 1);
        ResultsGrid.Add(ScanFailuresCount, column: 1, row: 1);
        ResultsGrid.Add(ScannedFilesTitle, column: 0, row: 2);
        ResultsGrid.Add(ScannedFilesCount, column: 1, row: 2);
        Stack.Children.Add(Picker);
        Stack.Children.Add(ResultsGrid);
        Stack.Children.Add(Throbber);
        Stack.Children.Add(ScanAgain);
    }

    private CancellationTokenSource _cts = new();

    private void OnScanAgainClicked(object sender, EventArgs __)
    {
        if (sender is Button button)
            button.IsEnabled = false;
        OnDirectoryChanged(sender, new()
        {
            Path = CurrentDirectory,
            Directory = new(CurrentDirectory)
        });
    }
    
    private async void OnDirectoryChanged(object _, DirectorySelectedEventArgs args)
    {
        await _cts.CancelAsync();
        _cts = new();

        try
        {
            Log.Info($"Beginning scan of {args.Path}...");
            UpdateScanUi(isStarting: true);
            ScannedImagePaths = await Task.Run(() => ScanDirectories(args.Path, _cts.Token), _cts.Token);
            UpdateScanUi(isStarting: false);
            Log.Info("Scan complete!");
        }
        catch (OperationCanceledException)
        {
            Log.Info("Current directory changed; file scan restarted.");
        }
        catch (Exception e)
        {
            Log.Error($"Failed to scan directories. ({e.Message})");
        }
    }

    private void UpdateScanUi(bool isStarting)
    {
        if (isStarting)
        {
            ScannedFilesCount.Text = "0";
            ScannedDirectoryCount.Text = "0";
        }

        ScanAgain.IsEnabled = !isStarting;
        ScanAgain.IsVisible = !isStarting;
        Throbber.IsRunning = isStarting;
        Throbber.IsVisible = isStarting;
    }
    
    public async Task<List<string>> ScanDirectories(string path, CancellationToken token)
    {
        try
        {
            string[] extensions = [ ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".webm" ];
            List<string> images = Directory
                .EnumerateFiles(path)
                .Where(file => extensions.Contains(Path.GetExtension(file).ToLower()))
                .ToList();
        
            await Gui.Update(() =>
            {
                ScannedFilesCount.Text = $"{ int.Parse(ScannedFilesCount.Text) + images.Count }";
                ScannedDirectoryCount.Text = $"{ int.Parse(ScannedDirectoryCount.Text) + 1 }";
            });

            foreach (string directory in Directory.EnumerateDirectories(path))
                if (!token.IsCancellationRequested)
                    images.AddRange(await ScanDirectories(directory, token));

            return images;
        }
        catch (PathTooLongException e)
        {
            await Gui.Update(() =>
            {
                ScannedFilesCount.Text = $"{int.Parse(ScannedFilesCount.Text) + 1}";
            });
            Log.Error($"A path is too long and could not be scanned. ({path})");
            return new();
        }
    }
}

public class DirectoryPicker : HorizontalStackLayout, IPreferential
{
    protected Entry Entry { get; set; }
    protected Button OpenButton { get; set; }
    public string CurrentDirectory { get; set; }
    private static readonly string HOME_DIRECTORY = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    public EventHandler<DirectorySelectedEventArgs> OnDirectorySelected { get; set; }
    
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

    public void Save() => Preferences.Set(PreferenceKeys.CURRENT_DIRECTORY, CurrentDirectory);

    public void Load()
    {
        string path = Preferences.Get(PreferenceKeys.CURRENT_DIRECTORY, CurrentDirectory);
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

public interface IPreferential
{
    public void Save();
    public void Load();
}

public class DirectorySelectedEventArgs : EventArgs
{
    public DirectoryInfo Directory { get; set; }
    public string Path { get; set; }
}