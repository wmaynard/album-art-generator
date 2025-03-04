using Microsoft.Maui.ApplicationModel;

namespace Maynard.ImageManipulator.Client.Utilities;

public static class Gui
{
    public static async Task Update(Func<Task> action) => await MainThread.InvokeOnMainThreadAsync(action);
    public static Task Update(Action action) => MainThread.InvokeOnMainThreadAsync(() =>
    {
        try
        {
            action();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        return Task.CompletedTask;
    });
}