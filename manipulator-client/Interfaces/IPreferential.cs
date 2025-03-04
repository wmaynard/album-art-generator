namespace Maynard.ImageManipulator.Client.Interfaces;

public interface IPreferential
{
    public string Id { get; init; }
    public void Reset();
    public void Save();
    public void Load();
}