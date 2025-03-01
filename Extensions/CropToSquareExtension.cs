using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Maynard.AlbumArt.Extensions;

public static class CropToSquareExtension
{
    /// <summary>
    /// Crops an image to a square area.  The resulting image will trim from two sides of the original image, leaving a
    /// window focused on the central pixel.
    /// </summary>
    /// <param name="self">The original image.</param>
    /// <param name="cropped">The cropped copy of the original image.</param>
    /// <returns>The cropped copy of the original image for chaining.</returns>
    public static Image<Rgba32> CropToSquare(this Image<Rgba32> self, out Image<Rgba32> cropped)
    {
        cropped = self.Width == self.Height ? self.Clone() : null;
        if (cropped != null)
            return cropped;

        int dimension = Math.Min(self.Width, self.Height);
        cropped = new(dimension, dimension);

        Queue<Rgba32> data = new();
        self.ProcessPixelRows(image =>
        {
            int remainingRows = dimension;
            
            bool landscape = image.Width > image.Height;
            bool portrait = !landscape;
            
            int y = landscape ? 0 : (image.Height - image.Width) / 2;

            while (remainingRows-- > 0)
            {
                Span<Rgba32> row = image.GetRowSpan(y++);
                int x = portrait ? 0 : (image.Width - image.Height) / 2;

                for (int remaining = dimension; remaining > 0; remaining--)
                    data.Enqueue(row[x++]);
            }
        });
        
        cropped.ProcessPixelRows(image =>
        {
            for (int y = 0; y < image.Height; y++)
            {
                Span<Rgba32> row = image.GetRowSpan(y);
                for (int x = 0; x < row.Length; x++)
                    row[x] = data.Dequeue();
            }
        });
        return cropped;
    }
}