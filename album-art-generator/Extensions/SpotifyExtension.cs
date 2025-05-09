using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Maynard.ImageManipulator.Client.Extensions;

public static class SpotifyExtension
{
    private const int DIM_STRENGTH = 100;
    /// <summary>
    /// Processes "tiles" of an image and adds a radial dim to each one before stitching them back together.
    /// </summary>
    /// <param name="self">The original image.</param>
    /// <param name="tileSize">The size of each tile in pixels.  Must be greater than 1 to have any effect.</param>
    /// <param name="spotted">The processed image.</param>
    /// <returns>The processed image for chaining.</returns>
    public static Image<Rgba32> Spotify(this Image<Rgba32> self, int tileSize, int dimStrength)
        => self.ProcessTiles(tileSize, image => image.Vignette(dimStrength));
    
    /// <summary>
    /// Processes "tiles" of an image and adds a radial dim to each one before stitching them back together.
    /// </summary>
    /// <param name="self">The original image.</param>
    /// <param name="tileSize">The size of each tile in pixels.  Must be greater than 1 to have any effect.</param>
    /// <param name="spotted">The processed image.</param>
    /// <returns>The processed image for chaining.</returns>
    public static Image<Rgba32> Spotify(this Image<Rgba32> self, int tileSize, int dimStrength, out Image<Rgba32> spotted)
        => spotted = self.Clone().ProcessTiles(tileSize, image => image.Vignette(dimStrength));
}