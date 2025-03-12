using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Maynard.ImageManipulator.Client.Extensions;

internal static class PixelBreakdownExtensions
{
    /// <summary>
    /// Converts an image into a queue of individual pixels.  It's up to the consumer of this method to repack them
    /// into an image of correct dimensions.
    /// </summary>
    /// <param name="self">The original image.</param>
    /// <returns>A queue of individual pixels starting from the top left.</returns>
    internal static Queue<Rgba32> Enqueue(this Image<Rgba32> self)
    {
        Queue<Rgba32> output = new();
        self.ProcessPixelRows(image =>
        {
            for (int y = 0; y < image.Height; y++)
            {
                Span<Rgba32> row = image.GetRowSpan(y);
                foreach (Rgba32 pixel in row)
                    output.Enqueue(pixel);
            }
        });
        return output;
    }

    /// <summary>
    /// Counterpart to Enqueue(), this will take an image and load pixels data into it.
    /// </summary>
    /// <param name="self">The image to load pixels into.</param>
    /// <param name="pixels">The pixels to load.</param>
    internal static void Load(this Image<Rgba32> self, Queue<Rgba32> pixels)
    {
        self.ProcessPixelRows(image =>
        {
            for (int y = 0; y < image.Height; y++)
            {
                Span<Rgba32> row = image.GetRowSpan(y);
                for (int x = 0; x < row.Length; x++)
                    row[x] = pixels.Dequeue();
            }
        });
    }
}

