namespace Maynard.ImageManipulator.Client.Events;

public class DirectorySelectedEventArgs : EventArgs
{
    public DirectoryInfo Directory { get; set; }
    public string Path { get; set; }
}