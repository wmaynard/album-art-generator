using Maynard.AlbumArt.Utilities;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using Vector2 = System.Numerics.Vector2;

namespace Maynard.AlbumArt;

public struct Location(string path)
{
    public string Directory = Path.GetDirectoryName(path);
    public string Filename = Path.GetFileNameWithoutExtension(path);
    public string Extension = Path.GetExtension(path);
    public string OriginalPath = path;
}
public class Art
{
    public static PngEncoder _encoder = new();
    
    public Location Location { get; set; }
    private string ScaledPath => $"{Location.Directory}/scaled-{Location.Filename}.png";
    private string BackgroundPath => $"{Location.Directory}/background-{Location.Filename}.png";
    private string CroppedPath => $"{Location.Directory}/cropped-{Location.Filename}.png";
    private string DimmedPath => $"{Location.Directory}/dimmed-{Location.Filename}.png";
    public string TestPath => $"{Location.Directory}/test-{Location.Filename}.png";
    public string TilePath => $"{Location.Directory}/test-{Location.Filename}.png";
    public Vector2 OriginalSize { get; set; }

    private Art(string path) => Location = new(path);

    private string PrefixSavePath(string prefix) => $"{Location.Directory}/{prefix}-{Location.Filename}.png";
    
    public void GenerateBackground()
    {
        Image<Rgba32> original = Image.Load<Rgba32>(Location.OriginalPath);
        Image<Rgba32> final = original
            .ScaleToMaxDimension(out Image<Rgba32> scaled)
            .Blur(5, out Image<Rgba32> blurred)
            .CropToSquare(out Image<Rgba32> cropped)
            .Dim(50, out Image<Rgba32> dimmed)
            .Spotify(10, out Image<Rgba32> spotted)
            ?.Superimpose(original, out Image<Rgba32> superimposed)
            ?? original;
        
        scaled.Save(PrefixSavePath("1-scaled"), _encoder);
        blurred.Save(PrefixSavePath("2-blurred"), _encoder);
        cropped.Save(PrefixSavePath("3-cropped"), _encoder);
        dimmed.Save(PrefixSavePath("4-dimmed"), _encoder);
        spotted.Save(PrefixSavePath("5-spotted"), _encoder);
        final.Save(PrefixSavePath("6-background"), _encoder);
    }


    public static List<Art> Scan(string path)
    {
        string[] extensions = [ ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".webm" ];
        List<Art> output = Directory
            .EnumerateFiles(path)
            .Where(file => extensions.Contains(Path.GetExtension(file).ToLower()))
            .Select(file => new Art(file))
            .ToList();
        
        foreach (string directory in Directory.EnumerateDirectories(path))
            output.AddRange(Scan(directory));

        return output;
    }
}