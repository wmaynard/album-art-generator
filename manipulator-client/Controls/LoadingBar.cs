using Maynard.ImageManipulator.Client.Utilities;

namespace Maynard.ImageManipulator.Client.Controls;

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