using Maynard.ImageManipulator.Client.Utilities;
using Maynard.Imaging.Extensions;

namespace Maynard.ImageManipulator.Client.Controls.ImageActions.Definitions;

public class ResizeDefinition : ActionDefinition
{
    private LabeledNumericEntry WidthEntry { get; set; }
    private LabeledNumericEntry HeightEntry { get; set; }
    
    public ResizeDefinition() : base("Resize", ActionDescriptions.RESIZE)
    {
        WidthEntry = new("Width", minimum: 100, maximum: 1920);
        HeightEntry = new("Height", minimum: 100, maximum: 1920);
        
        Stack.Add(WidthEntry);
        Stack.Add(HeightEntry);
    }
    public override string LoadingMessage => $"Resizing an image to {WidthEntry.Value}x{HeightEntry.Value}...";
    public override Picture Process(Picture image) => image.Resize(WidthEntry.Value, HeightEntry.Value);
    public override object[] ConfigurableValues => [WidthEntry.Value, HeightEntry.Value];

    protected override void Deserialize(string[] values)
    {
        WidthEntry.Value = int.Parse(values[0]);
        HeightEntry.Value = int.Parse(values[1]);
    }
}