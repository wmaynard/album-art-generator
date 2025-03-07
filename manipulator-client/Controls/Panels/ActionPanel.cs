using System.Text;
using Maynard.ImageManipulator.Client.Controls.ImageActions.Definitions;
using Maynard.ImageManipulator.Client.Interfaces;
using Maynard.ImageManipulator.Client.Utilities;

namespace Maynard.ImageManipulator.Client.Controls.Panels;

public class ActionPanel : Panel
{
    public static ActionPanel Instance { get; private set; }
    private List<IView> ActionPickers { get; set; } = new();
    private ComboBox ActionDropDown { get; set; }

    public Func<Picture, string, Picture>[] GenerateTransformationDelegates() =>
        Children.OfType<ActionDefinition>()
            .Select(definition => definition.GenerateDelegate())
            .ToArray();

    public ActionPanel()
    {
        ActionDropDown = new();
        ActionDropDown.Updated += HandleEffectUpdates;
        
        Stack.Children.Add(ActionDropDown);
        Instance = this;
    }
    
    // Janky to do it this way, will find a cleaner accessor later, e.g. dependency injection
    private void HandleEffectUpdates(object sender, ComboBoxUpdatedArgs args)
    {
        PreviewPanel preview = ((Grid)Parent)?.Children?.OfType<PreviewPanel>().FirstOrDefault();
        if (preview == null)
            return;
        
        ActionDropDown.Updated -= HandleEffectUpdates;
        ActionDropDown.Updated += preview.UpdatePreview;
        ActionDropDown.Updated?.Invoke(sender, args);
    }
    
    public string TransformationString => ActionDropDown.TransformationString;
    public void Load(string transformation) => ActionDropDown.Load(transformation);
}