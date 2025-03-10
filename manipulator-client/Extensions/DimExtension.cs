using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Maynard.ImageManipulator.Client.Extensions;

public static class DimExtension
{
    /// <summary>
    /// Adds a radial dim to an image, strongest on the outside.
    /// </summary>
    /// <param name="self">The original image.</param>
    /// <param name="strength">The percentage of darkening you want to occur, values 0-200.  At a value of 200,
    /// the entire image will be black.</param>
    /// <returns>The dimmed image for chaining.</returns>
    public static Image<Rgba32> Dim(this Image<Rgba32> self, int strength)
    {
        int maxX = self.Width / 2;
        int maxY = self.Height / 2;
        double maxDistance = Math.Sqrt(maxX * maxX + maxY * maxY);
        
        self.ProcessPixelRows(image =>
        {
            for (int row = 0; row < image.Height; row++)
            {
                Span<Rgba32> pixels = image.GetRowSpan(row);
                for (int col = 0; col < pixels.Length; col++)
                {
                    int x = Math.Abs(image.Width / 2 - row);
                    int y = Math.Abs(image.Height / 2 - col);
                    double distance = Math.Sqrt(x * x + y * y);
                    
                    double dimPercentage = Math.Max(0, 1 - (strength / 100.0) * (distance / maxDistance));

                    Rgba32 pixel = pixels[col];
                    Rgba32 replacement = new()
                    {
                        R = (byte)(int)(pixel.R * dimPercentage),
                        G = (byte)(int)(pixel.G * dimPercentage),
                        B = (byte)(int)(pixel.B * dimPercentage),
                        A = pixel.A
                    };
                    pixels[col] = replacement;
                }
            }
        });
        return self;
    }

    /// <summary>
    /// Copies an image, then adds a radial dim to an image, strongest on the outside.
    /// </summary>
    /// <param name="self">The original image.</param>
    /// <param name="strength">The percentage of darkening you want to occur, values 0-200.  At a value of 200,
    /// the entire image will be black.</param>
    /// <param name="dimmed">The dimmed copy of the image.</param>
    /// <returns>The dimmed copy of the image for chaining.</returns>
    public static Image<Rgba32> Dim(this Image<Rgba32> self, int strength, out Image<Rgba32> dimmed)
        => dimmed = self.Clone().Dim(strength);
}