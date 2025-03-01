using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Maynard.AlbumArt.Extensions;

public static class ScaleToMaxDimensionExtension
{
    public static Image<Rgba32> ScaleToMaxDimension(this Image<Rgba32> self)
    {
        Image<Rgba32> output = self.Width == self.Height ? null : self.Clone();
        if (output == null)
            return output;
        
        float scale = (float)Math.Max(output.Width, output.Height) / Math.Min(output.Width, output.Height);
        int width = (int)(output.Width * scale);
        int height = (int)(output.Height * scale);
        output.Mutate(img => img.Resize(width, height));
        return output;
    }
    
        
    /// <summary>
    /// Creates a scaled copy of the image using the maximum dimension as the new minimum dimension.
    /// For example, if an image is 100x200px, the new image will be 200x400px.
    /// </summary>
    /// <param name="self">The original image.</param>
    /// <param name="scaled">The scaled copy of the original image.</param>
    /// <returns>The scaled copy of the original image for chaining.</returns>
    public static Image<Rgba32> ScaleToMaxDimension(this Image<Rgba32> self, out Image<Rgba32> scaled)
    {
        scaled = self.Width == self.Height ? null : self.Clone();
        if (scaled == null)
            return scaled;
        
        float scale = (float)Math.Max(scaled.Width, scaled.Height) / Math.Min(scaled.Width, scaled.Height);
        int width = (int)(scaled.Width * scale);
        int height = (int)(scaled.Height * scale);
        scaled.Mutate(img => img.Resize(width, height));
        return scaled;
    }
}