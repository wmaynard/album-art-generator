using Maynard.Imaging.Extensions;
using Maynard.Imaging.Utilities;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using Vector2 = System.Numerics.Vector2;

namespace Maynard.Imaging.Models;

public class Art
{
    public static PngEncoder _encoder = new();
    
    public Location Location { get; set; }

    private Art(string path) => Location = new(path);

    private string PrefixSavePath(string prefix) => $"{Location.Directory}/{prefix}-{Location.Filename}.png";
    
    public void GenerateBackground()
    {
        Image<Rgba32> original = Image.Load<Rgba32>(Location.OriginalPath);
        Image<Rgba32> final = original
            .ScaleToMaxDimension(out Image<Rgba32> scaled)
            // .Blur(5, out Image<Rgba32> blurred)
            // .CropToSquare(out Image<Rgba32> cropped)
            // .Dim(50, out Image<Rgba32> dimmed)
            // .Spotify(10, out Image<Rgba32> spotted)
            // ?.Superimpose(original, out Image<Rgba32> superimposed)
            ?? original;

        scaled.Blur(5, out Image<Rgba32> blur1);
        // scaled.Blur(20);
        
        blur1.Save(PrefixSavePath("1-blur"), _encoder);
        // scaled.Save(PrefixSavePath("2-blurOriginal"), _encoder);
        
        //
        // scaled.Save(PrefixSavePath("1-scaled"), _encoder);
        // blurred.Save(PrefixSavePath("2-blurred"), _encoder);
        // cropped.Save(PrefixSavePath("3-cropped"), _encoder);
        // dimmed.Save(PrefixSavePath("4-dimmed"), _encoder);
        // spotted.Save(PrefixSavePath("5-spotted"), _encoder);
        // final.Save(PrefixSavePath("6-background"), _encoder);
    }


    public static List<Art> Scan(string path)
    {
        string[] extensions = [ ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".webp" ];
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