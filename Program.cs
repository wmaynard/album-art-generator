using Maynard.AlbumArt;

try
{
    string directory = Directory.GetCurrentDirectory();
    foreach (Art art in Art.Scan(directory))
    {
        Console.WriteLine($"Processing {art.Location.Filename}...");
        if (art.Location.Filename != "sonic3")
            continue;
        art.GenerateBackground();
    }

    Console.WriteLine("Done!");
}
catch (Exception e)
{ 
    Console.WriteLine(e.Message);
}
