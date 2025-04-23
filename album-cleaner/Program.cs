global using SysDir = System.IO.Directory;
global using SysFile = System.IO.File;
global using Directory = album_cleaner.Directory;
global using File = album_cleaner.File;

Directory todo = Directory.FromRelativeToUserDirectory("_TODO");

foreach (Directory series in todo.Directories)
{
    Console.WriteLine($"Processing {series.Name}...");
    Console.WriteLine("    - Creating special directories");
    series.CreateSpecialDirectories(out Directory albumArt, out Directory boxArt, out Directory extras);

    foreach (Directory album in series.Directories.OrderBy(album => album.Path))
    {
        Console.WriteLine($"Processing {album.Name}...");
    
        Console.WriteLine("    - Creating special directories");

        Console.WriteLine("    - Collecting all images");
        boxArt.CollectImagesFrom(album);
        Console.WriteLine("    - Moving non-audio files to __Extras");
        extras.CollectExtrasFrom(album, replacePath: series.Path);
        Console.WriteLine("    - Deleting .DS_Store & .txt files");
        album.DeleteTxtDsStoreFiles();
        Console.WriteLine("    - Deleting empty directories");
        album.DeleteEmptyDirectories();
    }
}