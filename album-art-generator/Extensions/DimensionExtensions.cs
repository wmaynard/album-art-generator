using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Maynard.ImageManipulator.Client.Extensions;

public static class DimensionExtensions
{
    public static int MinimumDimension(this Image<Rgba32> self) => Math.Min(self.Width, self.Height);
    public static int MaximumDimension(this Image<Rgba32> self) => Math.Max(self.Width, self.Height);
}