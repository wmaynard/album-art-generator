using Microsoft.Maui.ApplicationModel;

namespace Maynard.ImageManipulator.Client.Utilities;

public static class Gui
{
    public static async Task Update(Func<Task> action)
    {
        try
        {
            await MainThread.InvokeOnMainThreadAsync(action);
        }
        catch (Exception e)
        {
            Log.Error($"Unable to update main UI thread. ({e.Message})");
        }
    }

    public static Task Update(Action action)
    {
        try
        {
            MainThread.InvokeOnMainThreadAsync(action);
        }
        catch (Exception e)
        {
            Log.Error($"Unable to update main UI thread. ({e.Message})");
        }
        return Task.CompletedTask;
    }
}