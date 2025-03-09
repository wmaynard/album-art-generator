using Maynard.Imaging.Utilities;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Maynard.Imaging.Extensions;

public static class SuperimposeExtension
{
    /// <summary>
    /// Layers an image on top of the current one.  The image will be centered vertically or horizontally.
    /// </summary>
    /// <param name="self">The original image.</param>
    /// <param name="other">The image to be superimposed.</param>
    /// <returns>The combined image for chaining.</returns>
    public static Image<Rgba32> Superimpose(this Image<Rgba32> self, Image<Rgba32> other)
    {
        // if (other.Width > self.Width || other.Height > self.Height)
        //     throw new("Superimpose requires the background image to be at least the same size in both dimensions to work.");
        
        if (other.Width == self.Width && other.Height == self.Height)
        {
            Log.Warn("Superimposed image has the same dimensions as the background image.  The background image is lost.");
            self = other.Clone();
            return self;
        }
        // Guarantee the original image reaches the edge of one dimension
        if (!(other.Width == self.Width || other.Height == self.Height))
            other = other.Upscale(Math.Min(self.Width, self.Height));

        Queue<Rgba32> data = other.Enqueue();
        self.ProcessPixelRows(image =>
        {
            int startX = Math.Abs(self.Width - other.Width) / 2;
            int startY = Math.Abs(self.Height - other.Height) / 2;

            for (int y = startY; y < Math.Min(image.Height, startY + other.Height); y++)
            {
                Span<Rgba32> row = image.GetRowSpan(y);
                int remaining = other.Width;
                for (int x = startX; x < row.Length && remaining-- > 0; x++)
                {
                    if (data.Count == 0)
                        System.Diagnostics.Debugger.Break();
                    row[x] = data.Dequeue();
                }
            }
        });

        return self;
    }
    
    /// <summary>
    /// Layers an image on top of the current one.  The image will be centered vertically or horizontally.
    /// </summary>
    /// <param name="self">The original image.</param>
    /// <param name="other">The image to be superimposed.</param>
    /// <param name="superimposed">The combined image.</param>
    /// <returns>The combined image for chaining.</returns>
    public static Image<Rgba32> Superimpose(this Image<Rgba32> self, Image<Rgba32> other, out Image<Rgba32> superimposed)
        => superimposed = self.Clone().Superimpose(other);
}