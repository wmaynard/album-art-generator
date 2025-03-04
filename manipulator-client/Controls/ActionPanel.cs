using Maynard.ImageManipulator.Client.Interfaces;

namespace Maynard.ImageManipulator.Client.Controls;

public class ActionPanel : Panel, IPreferential
{
    public string Id { get; init; } = null;
    private List<IView> ActionPickers { get; set; } = new();
    private ComboBox ActionDropDown { get; set; }

    public ActionPanel()
    {
        ActionDropDown = new();
        
        Stack.Children.Add(ActionDropDown);
        Load();
    }

    public void DoNothing() { }
    public void Reset() => DoNothing();
    public void Save() => DoNothing();
    public void Load() => DoNothing();

}