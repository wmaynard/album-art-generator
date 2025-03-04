namespace Maynard.ImageManipulator.Client.Utilities;

public static class ActionDescriptions
{
    public const string BLUR = "Blurs an image.  Neighboring pixels from a specified radius are sampled, then averaged together.  The larger the radius, the blurrier the image will be.  Stronger blurs take more time to process.";
    public const string CROP_TO_SQUARE = "Crops an image so that its width and height are equal.  The crop is centered, so image data on edges is lost.";
    public const string DIM = "Adds a radial dim to the outer edges of the image.  The stronger the effect, the more darkening will occur.  At strength 200, the image will be entirely black.";
    public const string RESIZE = "Resizes the image to a manually specified width and height.  If the aspect ratio changes in this way, the image will be stretched.";
    public const string SCALE_TO_MAX_DIMENSION = "Scales an image up proportionally; its larger dimension (width or height) will be used as the new smaller dimension.  Aspect ratio is maintained.";
    public const string SPOTIFY = "Slices the image up into tiles and adds a dim to each tile.  This adds a black lattice over the image with circular windows.  A larger tile size will yield larger windows.";
    public const string SUPERIMPOSE = "Writes image data from a previous step over the current image.  The current image must be larger in both dimensions or this will fail.";
    public const string UPSCALE = "Resizes an image proportionally so that its smaller dimension (width or height) is set to the specified size.";

}