namespace Maynard.ImageManipulator.Client.Utilities;

public static class ActionDescriptions
{
    public const string BLUR = "Blurs an image.  Neighboring pixels from a specified radius are sampled, then averaged together.  The larger the radius, the blurrier the image will be.  Stronger blurs take more time to process.";
    public const string CROP_TO_SQUARE = "Crops an image so that its width and height are equal.  The crop is centered, so image data on edges is lost.";
    public const string DIM = "Adds a radial dim to the outer edges of the image.  The stronger the effect, the more darkening will occur.  At strength 200, the image will have a \"spotlight\" in the center and the edges will be entirely black.";
    public const string RESIZE = "Resizes the image to a manually specified width and height.  If the aspect ratio changes in this way, the image will be stretched.";
    public const string SCALE_MIN_TO = "Resizes an image proportionally so that its smaller dimension (width or height) is set to the specified size.";
    public const string SCALE_TO_MAX_DIMENSION = "Scales an image up proportionally; its maximum dimension (width or height) will be used as the new minimum dimension.  Aspect ratio is maintained.";
    public const string SPOTIFY = "Slices the image up into tiles and adds a dim to each tile.  This adds a black lattice over the image with circular windows.  A larger tile size will yield larger windows.";
    public const string SUPERIMPOSE = "Writes the original image data over the current image.  This may fail if the current image is smaller than the original image.";
    
    public static readonly string HELP_BLUR = $"{TITLE_BLUR}{Environment.NewLine}{Environment.NewLine}{BLUR}{Environment.NewLine}";
    public static readonly string HELP_CROP_TO_SQUARE = $"{TITLE_CROP_TO_SQUARE}{Environment.NewLine}{Environment.NewLine}{CROP_TO_SQUARE}";
    public static readonly string HELP_DIM = $"{TITLE_DIM}{Environment.NewLine}{Environment.NewLine}{DIM}{Environment.NewLine}The effect strength is proportional to image size, not directly tied to the image resolution.";
    public static readonly string HELP_RESIZE = $"{TITLE_RESIZE}{Environment.NewLine}{Environment.NewLine}{RESIZE}{Environment.NewLine}This is useful if you want to make sure all of your images in a batch have the same dimensions when processed.";
    public static readonly string HELP_SCALE_MIN_TO = $"{TITLE_SCALE_MIN_TO}{Environment.NewLine}{Environment.NewLine}{SCALE_MIN_TO}{Environment.NewLine}This is useful early on if you want to ensure all of your images have the same baseline minimum dimension for later processing.";
    public static readonly string HELP_SCALE_TO_MAX_DIMENSION =  $"{TITLE_SCALE_TO_MAX_DIMENSION}{Environment.NewLine}{Environment.NewLine}{SCALE_TO_MAX_DIMENSION}{Environment.NewLine}If you crop the image after performing this step, you'll be guaranteed to have a perfectly-fitted square background to then continue processing.";
    public static readonly string HELP_SPOTIFY = $"{TITLE_SPOTIFY}{Environment.NewLine}{Environment.NewLine}{SPOTIFY}{Environment.NewLine}Tile size here is a raw pixel value.  If your images have different dimensions, the effect will not look uniform!  If you want a uniform effect, you'll need to scale your images before applying this effect.";
    public static readonly string HELP_SUPERIMPOSE = $"{TITLE_SUPERIMPOSE}{Environment.NewLine}{Environment.NewLine}{SUPERIMPOSE}{Environment.NewLine}The example shown here first scaled the image to its max dimension, cropped it, and added a light blur before this step.";
    
    public const string TITLE_BLUR = "Blur";
    public const string TITLE_CROP_TO_SQUARE = "Crop To Square";
    public const string TITLE_DIM = "Dim Edges";
    public const string TITLE_RESIZE = "Resize";
    public const string TITLE_SCALE_MIN_TO = "Scale To";
    public const string TITLE_SCALE_TO_MAX_DIMENSION = "Scale To Max";
    public const string TITLE_SPOTIFY = "Spot Effect";
    public const string TITLE_SUPERIMPOSE = "Superimpose Original";
}