namespace Maynard.AlbumArt.Utilities;

public static class Timestamp
{
    public static long Now => DateTimeOffset.UtcNow.ToUnixTimeSeconds();
}