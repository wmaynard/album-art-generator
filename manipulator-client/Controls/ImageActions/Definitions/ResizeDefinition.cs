using Maynard.ImageManipulator.Client.Extensions;
using Maynard.ImageManipulator.Client.Utilities;

namespace Maynard.ImageManipulator.Client.Controls.ImageActions.Definitions;

public class ResizeDefinition : ActionDefinition
{
    private LabeledNumericEntry WidthEntry { get; set; }
    private LabeledNumericEntry HeightEntry { get; set; }
    
    public ResizeDefinition() : base("Resize", ActionDescriptions.RESIZE)
    {
        WidthEntry = new("Width", minimum: 100, maximum: 1920);
        HeightEntry = new("Height", minimum: 100, maximum: 1920);
        WidthEntry.ValueChanged += ValueChanged;
        
        Stack.Add(WidthEntry);
        Stack.Add(HeightEntry);
    }

    private void ValueChanged(object sender, EventArgs e) => EffectUpdated?.Invoke(this, EventArgs.Empty);

    public override string LoadingMessage => $"Resizing an image to {WidthEntry.Value}x{HeightEntry.Value}...";
    public override Picture Process(Picture picture) => picture.Resize(WidthEntry.Value, HeightEntry.Value);
    public override Func<Picture, string, Picture> GenerateDelegate() => (picture, _) => picture.Resize(WidthEntry.Value, HeightEntry.Value);
    public override object[] ConfigurableValues => [WidthEntry.Value, HeightEntry.Value];

    protected override void Deserialize(string[] values)
    {
        WidthEntry.Value = int.Parse(values[0]);
        HeightEntry.Value = int.Parse(values[1]);
    }
}