using Foundation;
using Maynard.ImageManipulator.Client.Utilities;

namespace Maynard.ImageManipulator.Client.Controls;


public class Terminal : ContentView
{
    private Border Border { get; set; }
    private ScrollView Scroller { get; set; }
    private Editor Editor { get; set; }
    
    public Terminal()
    {
        Border = new()
        {
            BackgroundColor = WdmColors.BLUE,
            Padding = 5
        };
        Scroller = new();
        Editor = new()
        {
            IsReadOnly = true,
            FontFamily = "Consolas",
            BackgroundColor = WdmColors.BLACK,
            TextColor = WdmColors.LIME
        };
        
        Scroller.Content = Editor;
        Border.Content = Scroller;
        Content = Border;
        
        Log.Intercept(OnLogSent);
    }
    
    private async void OnLogSent(object sender, LogEventArgs log)
    {
        await Gui.Update(async () =>
        {
            switch (log.Severity)
            {
                case Log.Severity.Warn:
                    await TemporarilyChangeColor(WdmColors.YELLOW, seconds: 10);
                    break;
                case Log.Severity.Error:
                    await TemporarilyChangeColor(WdmColors.RED, seconds: 30);
                    break;
                case Log.Severity.Critical:
                    await TemporarilyChangeColor(WdmColors.RED, WdmColors.BLACK, seconds: 60);
                    break;
                case Log.Severity.Verbose:
                    return;
                case Log.Severity.Info:
                default:
                    break;
            }
            
            Editor.Text += $"{log.Message}{Environment.NewLine}";

            // Fix sourced from: https://stackoverflow.com/questions/78298939/net-maui-editor-does-not-scroll-to-new-line-when-text-is-added-programattically
            UIKit.UITextView tv = (UIKit.UITextView)Editor.Handler?.PlatformView;
            if (tv == null)
                return;
            tv.ScrollEnabled = true;
            NSRange range = new (0, Editor.Text.Length);
            tv.ScrollRangeToVisible(range);
        });
    }

    private CancellationTokenSource _cts = new();

    private async Task TemporarilyChangeColor(Color text, int seconds) => TemporarilyChangeColor(WdmColors.BLACK, text, seconds);
    private async Task TemporarilyChangeColor(Color bg, Color text, int seconds)
    {
        await _cts.CancelAsync();
        _cts.Dispose();
        _cts = new();
        
        CancellationToken token = _cts.Token;
        try
        {
            await Gui.Update(async () =>
            {
                Editor.BackgroundColor = bg;
                Editor.TextColor = text;
                await Task.Delay(seconds * 1_000, token);
                Editor.BackgroundColor = WdmColors.BLACK;
                Editor.TextColor = WdmColors.LIME;
            });
        }
        catch (TaskCanceledException)
        {
            Log.Info("Console color changed again before time ran out; refreshing delay on color change.");
        }
    }
}