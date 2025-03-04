using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Maui.Storage;
using Maynard.ImageManipulator.Client.Events;
using Maynard.ImageManipulator.Client.Interfaces;
using Maynard.ImageManipulator.Client.Utilities;
using Font = Microsoft.Maui.Graphics.Font;

namespace Maynard.ImageManipulator.Client.Controls;

public class DirectoryPanel : Panel, IPreferential
{
    public string Id { get; init; } = PreferenceKeys.PANEL_DIRECTORY;
    protected DirectoryPicker ScanPicker { get; set; }
    protected DirectoryPicker OutputPicker { get; set; }
    public string CurrentDirectory => ScanPicker.CurrentDirectory;
    public string OutputDirectory => OutputPicker.CurrentDirectory;
    private Label ScannedDirectoryTitle { get; set; }
    private Label ScannedDirectoryCount { get; set; }
    private Label ScannedFilesTitle { get; set; }
    private Label ScannedFilesCount { get; set; }
    private Label ScanFailuresTitle { get; set; }
    private Label ScanFailuresCount { get; set; }
    private Label OutputDirectoryLabel { get; set; }
    private Grid ResultsGrid { get; set; }
    private ActivityIndicator Throbber { get; set; }
    
    private List<string> ScannedImagePaths { get; set; } = new();
    private Button ScanAgain { get; set; }
    private Button ClearOutput { get; set; }
    
    public DirectoryPanel()
    {
        Title = "Current Directory";

        ScanPicker = new()
        {
            Id = PreferenceKeys.CURRENT_DIRECTORY,
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
        OutputDirectoryLabel = new()
        {
            Text = "Output Directory (Optional)",
            HorizontalTextAlignment = TextAlignment.Center,
        };
        OutputPicker = new()
        {
            Id = PreferenceKeys.OUTPUT_DIRECTORY,
            OnDirectorySelected = OnOutputDirectoryChanged,
            OnReset = OnOutputDirectoryChanged
        };
        ClearOutput = new()
        {
            Text = "Clear Output Directory",
            IsVisible = false,
            IsEnabled = false
        };
        ClearOutput.Clicked += OnClearOutputClicked;
        
        ResultsGrid.Add(ScannedDirectoryTitle, column: 0, row: 0);
        ResultsGrid.Add(ScannedDirectoryCount, column: 1, row: 0);
        ResultsGrid.Add(ScanFailuresTitle, column: 0, row: 1);
        ResultsGrid.Add(ScanFailuresCount, column: 1, row: 1);
        ResultsGrid.Add(ScannedFilesTitle, column: 0, row: 2);
        ResultsGrid.Add(ScannedFilesCount, column: 1, row: 2);
        Stack.Children.Add(ScanPicker);
        Stack.Children.Add(ResultsGrid);
        Stack.Children.Add(Throbber);
        Stack.Children.Add(ScanAgain);
        Stack.Children.Add(new BoxView // For extra padding
        {
            HeightRequest = 50,
            Background = Brush.Transparent
        });
        Stack.Children.Add(OutputDirectoryLabel);
        Stack.Children.Add(OutputPicker);
        Stack.Children.Add(ClearOutput);
        Load();
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

    private void OnClearOutputClicked(object sender, EventArgs _) => OutputPicker.Reset();

    private async void OnOutputDirectoryChanged(object _, DirectorySelectedEventArgs args)
    {
        ClearOutput.IsEnabled = ClearOutput.IsVisible = !string.IsNullOrWhiteSpace(OutputPicker.CurrentDirectory);
        OutputPicker.Save();
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

    public void Reset()
    {
        ScanPicker.Reset();
        OutputPicker.Reset();
    }
    public void Save()
    {
        ScanPicker.Save();
        OutputPicker.Save();
    }

    public void Load()
    {
        ScanPicker.Load();
        OutputPicker.Load();
    }
}