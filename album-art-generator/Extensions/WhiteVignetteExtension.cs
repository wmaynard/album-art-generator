using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Maynard.ImageManipulator.Client.Extensions;

public static class WhiteVignetteExtension
{
    /// <summary>
    /// Adds a radial dodge to an image, strongest on the outside.
    /// </summary>
    /// <param name="self">The original image.</param>
    /// <param name="strength">The percentage of lightening you want to occur, values 0-200.  At a value of 200,
    /// the entire image will be black.</param>
    /// <returns>The vignetted image for chaining.</returns>
    public static Image<Rgba32> WhiteVignette(this Image<Rgba32> self, int strength)
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
                    int x = Math.Abs(image.Width / 2 - col);
                    int y = Math.Abs(image.Height / 2 - row);
                    double distance = Math.Sqrt(x * x + y * y);

                    // The farther from the center, the stronger the whitening effect
                    double dodgePercentage = Math.Min(1, (strength / 100.0) * (distance / maxDistance));

                    Rgba32 pixel = pixels[col];
                    Rgba32 replacement = new()
                    {
                        R = (byte)Math.Min(255, pixel.R + (255 - pixel.R) * dodgePercentage),
                        G = (byte)Math.Min(255, pixel.G + (255 - pixel.G) * dodgePercentage),
                        B = (byte)Math.Min(255, pixel.B + (255 - pixel.B) * dodgePercentage),
                        A = pixel.A
                    };
                    pixels[col] = replacement;
                }
            }
        });
        return self;
    }

    /// <summary>
    /// Copies an image, then adds a white vignette to an image.
    /// </summary>
    /// <param name="self">The original image.</param>
    /// <param name="strength">The percentage of lightening you want to occur, values 0-200.  At a value of 200,
    /// the entire image will be black.</param>
    /// <param name="dimmed">The vignetted copy of the image.</param>
    /// <returns>The vignetted copy of the image for chaining.</returns>
    public static Image<Rgba32> WhiteVignette(this Image<Rgba32> self, int strength, out Image<Rgba32> dimmed)
        => dimmed = self.Clone().WhiteVignette(strength);
}