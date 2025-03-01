using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Maynard.Imaging.Extensions;

public static class UpscaleExtension
{
    /// <summary>
    /// Upscales an image to a minimum width / height.  Does nothing if both width and height are larger than the
    /// specified target dimension.
    /// </summary>
    /// <param name="self">The original image.</param>
    /// <param name="targetMinimum">The minimum width or height, in pixels.</param>
    /// <returns>The upscaled image for chaining.</returns>
    public static Image<Rgba32> Upscale(this Image<Rgba32> self, int targetMinimum)
    {
        float scale = targetMinimum / (float)Math.Min(self.Width, self.Height);
        return scale < 1
            ? self
            : self.Resize((int)(self.Width * scale), (int)(self.Height * scale));
    }

    /// <summary>
    /// Copies an image, then upscales it to a minimum width / height.  Does nothing if both width and height are larger
    /// than the specified target dimension.
    /// </summary>
    /// <param name="self">The original image.</param>
    /// <param name="targetMinimum">The minimum width or height, in pixels.</param>
    /// <param name="upscaled">The upscaled image.</param>
    /// <returns>The upscaled image for chaining.</returns>
    public static Image<Rgba32> Upscale(this Image<Rgba32> self, int targetMinimum, out Image<Rgba32> upscaled)
        => upscaled = self.Clone().Upscale(targetMinimum);
}