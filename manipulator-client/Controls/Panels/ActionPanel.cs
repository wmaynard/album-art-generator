using Maynard.ImageManipulator.Client.Interfaces;
using Maynard.ImageManipulator.Client.Utilities;

namespace Maynard.ImageManipulator.Client.Controls.Panels;

public class ActionPanel : Panel, IPreferential
{
    public string Id { get; init; } = null;
    private List<IView> ActionPickers { get; set; } = new();
    private ComboBox ActionDropDown { get; set; }

    public ActionPanel()
    {
        ActionDropDown = new();
        ActionDropDown.Updated += HandleEffectUpdates;
        
        Stack.Children.Add(ActionDropDown);
        Load();
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

    public void DoNothing() { }
    public void Reset() => DoNothing();
    public void Save() => DoNothing();
    public void Load() => DoNothing();
}