namespace Maynard.Imaging.Models;

public struct Location(string path)
{
    public string Directory = Path.GetDirectoryName(path);
    public string Filename = Path.GetFileNameWithoutExtension(path);
    public string Extension = Path.GetExtension(path);
    public string OriginalPath = path;
}