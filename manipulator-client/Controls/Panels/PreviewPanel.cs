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
        LoadingBar.OnComplete += (_, _) => Log.Verbose("Processed image preview updated!");
        
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
                // Log.Info("Starting update task");

                await LoadingBar.Start(args.Actions.Length);
                byte[] imageBytes = Convert.FromBase64String(Mandrill.BASE_64);
                using MemoryStream ms = new(imageBytes);
                Picture original = Picture.Load<Rgba32>(ms);
                Picture clone = original.Clone();

                for (int steps = 0; steps < args.Actions.Length; steps++)
                {
                    ActionDefinition definition = args.Actions[steps];
                    await LoadingBar.SetMessage(definition.LoadingMessage);
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
            // Log.Info("Update task was cancelled.");
            await LoadingBar.MarkComplete("Canceled; more changes detected.");
        }
        catch (Exception e)
        {
            Log.Error($"Unexpected problem when processing image. ({e.Message}");
        }
    }
}