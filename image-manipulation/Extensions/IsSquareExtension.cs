using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Maynard.Imaging.Extensions;

internal static class IsSquareExtension
{
    public static bool IsSquare(this Image<Rgba32> image) => image.Width == image.Height;
}