namespace album_cleaner;

public class File
{
    private static readonly string[] AUDIO_EXTENSIONS = { ".mp3", ".wav", ".flac", ".ogg", ".aac", ".m4a" };
    private static readonly string[] IMAGE_EXTENSIONS = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".webp" };
    
    public string Path { get; set; }
    public string Name { get; set; }
    public string Extension { get; set; }
    public long Bytes { get; set; }
    public bool IsAudio => AUDIO_EXTENSIONS.Contains(Extension);
    public bool IsImage => IMAGE_EXTENSIONS.Contains(Extension);

    public File(string path)
    {
        Path = path;
        Name = System.IO.Path.GetFileName(path);
        Extension = System.IO.Path.GetExtension(path);
        Bytes = new FileInfo(Path).Length;
    }
}