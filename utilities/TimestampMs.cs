namespace Maynard.AlbumArt.Utilities;

public class TimestampMs
{
    public static long Now => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
}