using System.Text.RegularExpressions;
using Maynard.ImageManipulator.Client.Extensions;
using Maynard.ImageManipulator.Client.Utilities;

namespace Maynard.ImageManipulator.Client.Controls.ImageActions.Definitions;

public class UpscaleDefinition : ActionDefinition
{
    private LabeledNumericEntry Entry { get; set; } 
    public UpscaleDefinition() : base(ActionDescriptions.TITLE_SCALE_MIN_TO, ActionDescriptions.SCALE_MIN_TO)
    {
        Entry = new("Target Minimum Dimension (pixels)", 100, 1080);
        Entry.ValueChanged += ValueChanged;
        
        Stack.Add(Entry);
    }
    private void ValueChanged(object sender, EventArgs e) => EffectUpdated?.Invoke(this, EventArgs.Empty);
    public override string LoadingMessage => $"Scales an image to a minimum dimension of {Entry.Value} pixels...";
    public override Picture Process(Picture picture) => picture.Upscale(targetMinimum: Entry.Value);
    public override Func<Picture, string, Picture> GenerateDelegate() => (picture, _) => picture.Upscale(targetMinimum: Entry.Value);
    public override object[] ConfigurableValues => [Entry.Value];
    protected override void Deserialize(string[] values) { Entry.Value = int.Parse(values[0]); }
}