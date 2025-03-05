using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace Maynard.Imaging.Extensions;

public static class Base64Extension
{
    public static string ToBase64(this Image<Rgba32> image)
    {
        using MemoryStream ms = new();
        image.Save(ms, new JpegEncoder());
        return Convert.ToBase64String(ms.ToArray());
    }
}