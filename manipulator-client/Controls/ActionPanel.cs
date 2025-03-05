using Maynard.ImageManipulator.Client.Interfaces;
using Maynard.ImageManipulator.Client.Utilities;

namespace Maynard.ImageManipulator.Client.Controls;

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

    private void HandleEffectUpdates(object sender, ComboBoxUpdatedArgs args)
    {
        Log.Info("Update the image preview - something changed.");
    }

    public void DoNothing() { }
    public void Reset() => DoNothing();
    public void Save() => DoNothing();
    public void Load() => DoNothing();

}