using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Maynard.AlbumArt.Extensions;

public static class ScaleToMaxDimensionExtension
{
    /// <summary>
    /// Scales an image using the maximum dimension as the new minimum dimension.
    /// For example, if an image is 100x200px, the new image will be 200x400px.
    /// </summary>
    /// <param name="self">The original image.</param>
    /// <returns>The scaled image for chaining.</returns>
    public static Image<Rgba32> ScaleToMaxDimension(this Image<Rgba32> self)
    {
        if (self.IsSquare())
            return self;
        
        float scale = (float)Math.Max(self.Width, self.Height) / Math.Min(self.Width, self.Height);
        int width = (int)(self.Width * scale);
        int height = (int)(self.Height * scale);
        self.Mutate(img => img.Resize(width, height));
        return self;
    }

    /// <summary>
    /// Creates a scaled copy of the image using the maximum dimension as the new minimum dimension.
    /// For example, if an image is 100x200px, the new image will be 200x400px.
    /// </summary>
    /// <param name="self">The original image.</param>
    /// <param name="scaled">The scaled copy of the original image.</param>
    /// <returns>The scaled copy of the original image for chaining.</returns>
    public static Image<Rgba32> ScaleToMaxDimension(this Image<Rgba32> self, out Image<Rgba32> scaled)
        => scaled = self.Clone().ScaleToMaxDimension();
}

