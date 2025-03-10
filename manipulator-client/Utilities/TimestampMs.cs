namespace Maynard.ImageManipulator.Client.Utilities;

public static class TimestampMs
{
    public static long Now => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    public static long Measure(string message, Action action)
    {
        long start = Now;
        action();
        long duration = Now - start;
        message ??= "Time taken:";
        Log.Info($"{message} {duration:N0}ms");
        return duration;
    }

    public static long Measure(Action action) => Measure(null, action);
}