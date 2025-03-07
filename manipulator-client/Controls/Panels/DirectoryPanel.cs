using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Maui.Views;
using Maynard.ImageManipulator.Client.Events;
using Maynard.ImageManipulator.Client.Interfaces;
using Maynard.ImageManipulator.Client.Utilities;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using Font = Microsoft.Maui.Graphics.Font;

namespace Maynard.ImageManipulator.Client.Controls.Panels;

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
    private Button SaveActionSetButton { get; set; }
    private Button RunTransformationButton { get; set; }
    private LoadingBar ProgressBar { get; set; }
    
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

        SaveActionSetButton = new()
        {
            Text = "Save Current Transformation"
        };
        SaveActionSetButton.Clicked += SaveActionClicked;

        RunTransformationButton = new()
        {
            Text = "Run Transformation Batch"
        };
        RunTransformationButton.Clicked += RunTransformationButtonOnClicked;
        ProgressBar = new();
        ProgressBar.IsVisible = false;
        
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
        Stack.Add(SaveActionSetButton);
        Stack.Add(RunTransformationButton);
        Stack.Add(ProgressBar);
        Load();
    }

    private CancellationTokenSource _processingCts = new();
    private async void RunTransformationButtonOnClicked(object sender, EventArgs e)
    {
        await _processingCts.CancelAsync();
        _processingCts = new();
        CancellationToken token = _processingCts.Token;
        
        await ProgressBar.Start(targetPoints: 2);
        await ProgressBar.ProgressTo("Scanning for files...", points: 1);
        List<FileInfo> files = (await ScanDirectories(CurrentDirectory, token))
            .Select(path => new FileInfo(path))
            .Where(info => !info.Name.StartsWith("processed-"))
            .ToList();
        long totalBytes = files.Sum(info => info.Length);
        await ProgressBar.ProgressTo("Preparing tasks...", points: 2);
        
        Func<Picture, string, Picture>[] delegates = ActionPanel.Instance.GenerateTransformationDelegates();
        ConcurrentQueue<Func<Task>> tasks = new();
        ConcurrentQueue<long> totalProcessed = new();
        string FormatBytes(long bytes) => $"{bytes / 1024f / 1024f:0.00} MB";

        foreach (FileInfo info in files)
            tasks.Enqueue(async () =>
            {
                try
                {
                    Picture picture = Picture.Load<Rgba32>(info.FullName);
                    foreach (Func<Picture, string, Picture> action in delegates)
                        picture = action(picture, info.FullName);

                    string directory = OutputDirectory ?? info.Directory.FullName;
                    string path = Path.Combine(directory, $"processed-{info.Name}");
                    if (File.Exists(path))
                        File.Delete(path);
                    await picture.SaveAsync(path, new JpegEncoder(), token);

                    long total = info.Length;
                    while (totalProcessed.TryDequeue(out long processed))
                        total += processed;
                    totalProcessed.Enqueue(total);
                    total = Math.Min(total, totalBytes);
                    string message = $"{FormatBytes(total)} / {FormatBytes(totalBytes)} MB";
                    await ProgressBar.Progress(message, info.Length);
                }
                catch (Exception exception)
                {
                    Log.Error($"Unable to process file {info.FullName}!");
                }
            });

        await ProgressBar.Start(targetPoints: totalBytes);
        await ProgressBar.SetMessage("Beginning processing tasks...");

        const int WORKER_COUNT = 8;
        SemaphoreSlim semaphore = new(WORKER_COUNT);
        List<Task> runningTasks = new();

        while (tasks.TryDequeue(out Func<Task> task))
        {
            await semaphore.WaitAsync(token); // Wait for available slot

            runningTasks.Add(Task.Run(async () =>
            {
                try                     { await task();                                         }
                catch (Exception ex)    { Log.Error($"Task processing failed! ({ex.Message})"); }
                finally                 { semaphore.Release();                                  }
            }, token));

            if (runningTasks.Count < WORKER_COUNT)
                continue;
            
            Task completedTask = await Task.WhenAny(runningTasks);
            runningTasks.Remove(completedTask); // Remove completed tasks
        }

        // Wait for all tasks to complete
        await Task.WhenAll(runningTasks);

        await ProgressBar.ProgressTo("Processing complete!", totalBytes);
        ProgressBar.Stop();
    }

    private CancellationTokenSource _cts = new();

    private async void SaveActionClicked(object sender, EventArgs e)
    {
        IView view = this;
        while (view != null && view is not Page page)
            view = view.Parent as IView;
        if (view == null)
        {
            Log.Error("Could not fetch Page; unable to save.");
            return;
        }
        
        string description = (string)await ((Page)view).ShowPopupAsync(new SaveTransformationPopup());
        if (string.IsNullOrWhiteSpace(description))
        {
            Log.Info("Unable to save current transformation; the popup was canceled or an empty value entered.");
            return;
        }
        TransformationPersistenceManager.Save(description);
    }
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
        if (await Permissions.RequestAsync<Permissions.StorageRead>() != PermissionStatus.Granted)
        {
            Log.Error("Storage access denied. Cannot scan directories.");
            return;
        }
        if (await Permissions.RequestAsync<Permissions.StorageWrite>() != PermissionStatus.Granted)
        {
            Log.Error("Storage access denied. Cannot scan directories.");
            return;
        }
        
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
        catch (UnauthorizedAccessException ex)
        {
            Log.Error($"Unauthorized access to directory '{args.Path}'.  Grant access to this directory first.");
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