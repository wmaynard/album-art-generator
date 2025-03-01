using Maynard.AlbumArt.Utilities;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Maynard.AlbumArt.Extensions;

public static class SuperimposeExtension
{
    /// <summary>
    /// Layers an image on top of the current one.  The image will be centered vertically or horizontally.
    /// TODO: Test what happens when superimpose is used with an image larger than the source.
    /// </summary>
    /// <param name="self">The original image.</param>
    /// <param name="other">The image to be superimposed.</param>
    /// <param name="superimposed">The combined image.</param>
    /// <returns>The combined image for chaining.</returns>
    public static Image<Rgba32> Superimpose(this Image<Rgba32> self, Image<Rgba32> other, out Image<Rgba32> superimposed)
    {
        bool sameDimensions = other.Width == self.Width && other.Height == self.Height;
        superimposed = (sameDimensions ? other : self).Clone();
        if (sameDimensions)
            return superimposed;

        Queue<Rgba32> data = other.Enqueue();
        superimposed.ProcessPixelRows(image =>
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
                    row[x] = data.Dequeue(); // TODO
                }
            }
        });

        return superimposed;
    }
}