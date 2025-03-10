using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Maynard.ImageManipulator.Client.Extensions;

public static class ScaleToMinimumDimensionExtension
{
    /// <summary>
    /// Scales an image using the specified dimension as the new maximum dimension.
    /// For example, if an image is 100x200px and the dimension is 150, the new image will be 75x150px.
    /// </summary>
    /// <param name="self">The original image.</param>
    /// <param name="newMaximum">The new maximum dimension for the image.</param>
    /// <returns>The scaled image for chaining.</returns>
    public static Image<Rgba32> ScaleToDimension(this Image<Rgba32> self, int newMaximum)
    {
        if (Math.Max(self.Width, self.Height) == newMaximum)
            return self;

        // 400, 600 | 300
        float scale = newMaximum / (float)self.MaximumDimension();
        int width = (int)(self.Width * scale);
        int height = (int)(self.Height * scale);
        self.Mutate(img => img.Resize(width, height));
        return self;
    }

    /// <summary>
    /// Creates a scaled copy of the image using the specified dimension as the new maximum dimension.
    /// For example, if an image is 100x200px and the dimension is 150, the new image will be 75x150px.
    /// </summary>
    /// <param name="self">The original image.</param>
    /// <param name="newMaximum">The new maximum dimension for the image.</param>
    /// <param name="scaled">The scaled copy of the original image.</param>
    /// <returns>The scaled copy of the original image for chaining.</returns>
    public static Image<Rgba32> ScaleToDimension(this Image<Rgba32> self, int newMaximum, out Image<Rgba32> scaled)
        => scaled = self.Clone().ScaleToDimension(newMaximum);
}