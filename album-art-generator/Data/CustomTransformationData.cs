using Maynard.ImageManipulator.Client.Utilities;

namespace Maynard.ImageManipulator.Client.Data;

public class CustomTransformationData
{
    public bool IsBuiltIn => Index < TransformationPersistenceManager.CUSTOM_START;
    public string Key { get; init; }
    public int Index { get; init; }
    public string Description { get; init; }
    public string[] Actions { get; set; }
}