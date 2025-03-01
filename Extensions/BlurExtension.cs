using Maynard.AlbumArt.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Maynard.AlbumArt.Extensions;

public static class BlurExtension
{
    /// <summary>
    /// Blurs an image using linear averages.
    /// </summary>
    /// <param name="self">The original image.</param>
    /// <param name="strength">The pixel radius to consider for averaging.</param>
    /// <param name="blurred">The blurred copy of the original image.</param>
    /// <returns>The blurred copy of the original image for chaining.</returns>
    // public static Image<Rgba32> Blur(this Image<Rgba32> self, int strength)
    // {
    //     List<Rgba32[]> rows = new();
    //     self.ProcessPixelRows(image =>
    //     {
    //         for (int y = 0; y < image.Height; y++)
    //             rows.Add(image.GetRowSpan(y).ToArray());
    //     });
    //
    //     Queue<Rgba32> data = new();
    //     Queue<AveragingWindow> spotlight = new();
    //     
    //     for (int y = 0; y < rows.Count; y++)
    //     for (int x = 0; x < rows[y].Length; x++)
    //     {
    //         AveragingWindow neighbors = y == 0
    //             ? new(ref rows, x, y, strength)
    //             : spotlight.Dequeue();
    //         data.Enqueue(neighbors.Average());
    //         neighbors.ShiftUp(y + strength < rows.Count
    //             ? rows[y + strength]
    //             : null
    //         );
    //         spotlight.Enqueue(neighbors);
    //     }
    //
    //     // blurred = new(self.Width, self.Height);
    //     self.ProcessPixelRows(image =>
    //     {
    //         for (int y = 0; y < image.Height; y++)
    //         {
    //             Span<Rgba32> row = image.GetRowSpan(y);
    //             for (int x = 0; x < row.Length; x++)
    //                 row[x] = data.Dequeue();
    //         }
    //     });
    //     return self;
    // }
    
    /// <summary>
    /// Blurs an image using linear averages.
    /// </summary>
    /// <param name="self">The original image.</param>
    /// <param name="strength">The pixel radius to consider for averaging.</param>
    /// <param name="blurred">The blurred copy of the original image.</param>
    /// <returns>The blurred copy of the original image for chaining.</returns>
    public static Image<Rgba32> Blur(this Image<Rgba32> self, int strength, out Image<Rgba32> blurred)
    {
        List<Rgba32[]> rows = new();
        self.ProcessPixelRows(image =>
        {
            for (int y = 0; y < image.Height; y++)
                rows.Add(image.GetRowSpan(y).ToArray());
        });

        Queue<Rgba32> data = new();
        Queue<AveragingWindow> spotlight = new();
        
        for (int y = 0; y < rows.Count; y++)
        for (int x = 0; x < rows[y].Length; x++)
        {
            AveragingWindow neighbors = y == 0
                ? new(ref rows, x, y, strength)
                : spotlight.Dequeue();
            data.Enqueue(neighbors.Average());
            neighbors.ShiftUp(y + strength < rows.Count
                ? rows[y + strength]
                : null
            );
            spotlight.Enqueue(neighbors);
        }

        blurred = new(self.Width, self.Height);
        blurred.ProcessPixelRows(image =>
        {
            for (int y = 0; y < image.Height; y++)
            {
                Span<Rgba32> row = image.GetRowSpan(y);
                for (int x = 0; x < row.Length; x++)
                    row[x] = data.Dequeue();
            }
        });
        return blurred;
    }
}