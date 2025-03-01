using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Maynard.AlbumArt.Extensions;

public static class ResizeExtension
{
    public static Image<Rgba32> Resize(this Image<Rgba32> self, int width, int height)
    {
        self.Mutate(image => image.Resize(width, height));
        return self;
    }

    public static Image<Rgba32> Resize(this Image<Rgba32> self, int width, int height, out Image<Rgba32> resized)
        => resized = self.Clone().Resize(width, height);
}