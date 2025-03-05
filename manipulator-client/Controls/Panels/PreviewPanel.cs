global using Picture = SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32>;
using System.IO;
using Maynard.ImageManipulator.Client.Controls.ImageActions.Definitions;
using Maynard.ImageManipulator.Client.Samples;
using Maynard.ImageManipulator.Client.Utilities;
using Maynard.Imaging.Extensions;
using SixLabors.ImageSharp.PixelFormats;


namespace Maynard.ImageManipulator.Client.Controls.Panels;

public class PreviewPanel : Panel
{
    private static readonly string PATH_ORIGINAL = $"/Samples/mandrill.jpg";
    private static readonly string PATH_PROCESSED = $"/Samples/mandrill-processed.jpg";
    private Grid Grid { get; set; }
    private ImagePreview Original { get; set; }
    private ImagePreview Processed { get; set; }
    private LoadingBar LoadingBar { get; set; }
    
    public PreviewPanel()
    {
        Grid = new()
        {
            RowDefinitions =
            {
                new() { Height = new(5, GridUnitType.Star) },
                new() { Height = new(5, GridUnitType.Star) },
                new() { Height = new(1, GridUnitType.Star) },
            },
            ColumnDefinitions =
            {
                new() { Width = new (1, GridUnitType.Star)}
            }
        };
        Original = new()
        {
            Title = "Sample Image",
            Base64 = Mandrill.BASE_64
        };
        Processed = new()
        {
            Title = "After Processing",
            Base64 = Mandrill.BASE_64,
            Refreshable = true
        }; 

        LoadingBar = new()
        {
            Spacing = 10,
            HorizontalOptions = LayoutOptions.Fill,
        };
        LoadingBar.OnComplete += (_, _) => Log.Info("Processed image preview updated!");
        
        Grid.Add(Original, column: 0, row: 0);
        Grid.Add(Processed, column: 0, row: 1);
        Grid.Add(LoadingBar, column: 0, row: 2);
        Stack.Add(Grid);
    }

    private CancellationTokenSource _src = new();
    public async void UpdatePreview(object sender, ComboBoxUpdatedArgs args)
    {
        await _src.CancelAsync();
        _src = new();
        if (!args.Actions.Any())
            return;

        try
        {
            CancellationToken token = _src.Token;
            await Task.Run(async () =>
            {
                await LoadingBar.Start(5);
                await LoadingBar.ProgressTo("Waiting on further user input....", 1);
                await Task.Delay(1_000, token);
                await LoadingBar.ProgressTo("Waiting on further user input.", 2);
                await Task.Delay(1_000, token);
                await LoadingBar.ProgressTo("Waiting on further user input..", 3);
                await Task.Delay(1_000, token);
                await LoadingBar.ProgressTo("Waiting on further user input...", 4);
                await Task.Delay(1_000, token);
                await LoadingBar.ProgressTo("Waiting on further user input....", 5);
                await Task.Delay(1_000, token);
                Log.Info("Starting update task");

                await LoadingBar.Start(args.Actions.Length);
                byte[] imageBytes = Convert.FromBase64String(Mandrill.BASE_64);
                using MemoryStream ms = new(imageBytes);
                Picture original = Picture.Load<Rgba32>(ms);
                Picture clone = original.Clone();

                for (int steps = 0; steps < args.Actions.Length; steps++)
                {
                    ActionDefinition definition = args.Actions[steps];
                    await LoadingBar.SetMesasge(definition.LoadingMessage);
                    clone = definition is SuperimposeDefinition superimpose
                        ? superimpose.Process(clone, original)
                        : definition.Process(clone);
                    Processed.Base64 = clone.ToBase64();
                    await LoadingBar.ProgressTo(definition.LoadingMessage, steps + 1);
                }

                Log.Info("Finished updating image preview.");
                await LoadingBar.MarkComplete("Updated.");
            }, token);
        }
        catch (TaskCanceledException)
        {
            Log.Info("Update task was cancelled.");
            await LoadingBar.MarkComplete("Canceled; more changes detected.");
        }
        catch (Exception e)
        {
            Log.Error($"Unexpected problem when processing image. ({e.Message}");
        }
    }
}

public class LoadingBar : HorizontalStackLayout
{
    private HorizontalStackLayout Stack { get; set; }
    private VerticalStackLayout VerticalStack { get; set; }
    private ActivityIndicator Throbber { get; set; }
    private Label Status { get; set; }
    private ProgressBar ProgressBar { get; set; }
    public EventHandler OnComplete { get; set; }

    public string Text
    {
        get => Status.Text;
        set => Status.Text = value;
    }
    private int TotalPoints { get; set; }

    public float Percentage => 0; // TODO

    public LoadingBar()
    {
        Stack = new()
        {
            Spacing = 10,
            Padding = new(10)
        };
        VerticalStack = new();
        Status = new()
        {
            HorizontalTextAlignment = TextAlignment.Center
        };
        Throbber = new()
        {
            IsRunning = false, // Initially hidden
            Color = WdmColors.SALMON,
            Scale = 2
        };
        ProgressBar = new()
        {
            Progress = 0,
            BackgroundColor = WdmColors.WHITE,
            ProgressColor = WdmColors.GREEN
        };
        TotalPoints = 1000;
        
        VerticalStack.Add(Status);
        VerticalStack.Add(ProgressBar);
        Stack.Add(Throbber);
        Stack.Add(VerticalStack);
        Add(Stack);
    }

    public async Task<LoadingBar> Start(int targetPoints)
    {
        await Gui.Update(() =>
        {
            ProgressBar.Progress = 0;
            Throbber.IsVisible = true;
            Throbber.IsRunning = true;
            IsVisible = true;
            TotalPoints = targetPoints;
        });
        return this;
    }

    public async Task SetMesasge(string message) => await Gui.Update(() => Text = message);
    public async Task ProgressTo(string message, int points)
    {
        float progress = points / (float)TotalPoints;
        await Gui.Update(async () =>
        {
            Text = message;
            uint ms = 250;
            await ProgressBar.ProgressTo(progress, ms, Easing.Linear);
            if (points == TotalPoints)
                Stop();
        });
    }

    public async Task MarkComplete(string message) => await Gui.Update(async () =>
    {
        Text = message;
        await ProgressBar.ProgressTo(TotalPoints, 0, Easing.Linear);
    });

    public void Stop()
    {
        Throbber.IsRunning = false;
        OnComplete?.Invoke(this, EventArgs.Empty);
    }
}

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