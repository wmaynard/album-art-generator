using System.Text.RegularExpressions;
using Maynard.ImageManipulator.Client.Utilities;
using Maynard.Imaging.Extensions;

namespace Maynard.ImageManipulator.Client.Controls.ImageActions.Definitions;

public class UpscaleDefinition : ActionDefinition
{
    private LabeledNumericEntry Entry { get; set; } 
    public UpscaleDefinition() : base("Upscale", ActionDescriptions.UPSCALE)
    {
        Entry = new("Target Minimum Dimension (pixels)", 100, 1080);
        
        Stack.Add(Entry);
    }

    public override string LoadingMessage => $"Upscaling an image to a minimum dimension of {Entry.Value} pixels...";
    public override Picture Process(Picture image) => image.Upscale(targetMinimum: Entry.Value);
}