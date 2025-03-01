using Maynard.AlbumArt;
using Maynard.AlbumArt.Models;
using Maynard.AlbumArt.Utilities;

try
{
    string directory = Directory.GetCurrentDirectory();
    foreach (Art art in Art.Scan(directory))
    {
        Log.Info($"Processing {art.Location.Filename}...");
        if (art.Location.Filename != "sonic3")
            continue;
        art.GenerateBackground();
    }

    Log.Info("Done!");
}
catch (Exception e)
{ 
    Log.Error(e.Message);
}
