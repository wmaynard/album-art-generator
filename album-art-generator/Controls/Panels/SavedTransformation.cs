using Maynard.ImageManipulator.Client.Data;
using Maynard.ImageManipulator.Client.Utilities;

namespace Maynard.ImageManipulator.Client.Controls.Panels;

public class SavedTransformation : Panel
{
    private CustomTransformationData Data { get; set; }
    private Label DescriptionLabel { get; set; }
    private Label DetailsLabel { get; set; }
    private Button DeleteButton { get; set; }
    private Button OpenButton { get; set; }
    
    public SavedTransformation(CustomTransformationData data)
    {
        Data = data;
        Border.Stroke = Data.IsBuiltIn
            ? WdmBrushes.SALMON
            : WdmBrushes.YELLOW;
        Title = Data.Description;
        DescriptionLabel = new() { Text = "Steps" };
        DetailsLabel = new() { Text = string.Join(Environment.NewLine, Data.Actions) };
        DeleteButton = new() { Text = "X" };
        OpenButton = new() { Text = "Open" };

        DeleteButton.Clicked += DeleteButtonClicked;
        OpenButton.Clicked += OpenClicked;

        Grid grid = new()
        {
            ColumnDefinitions =
            {
                new() { Width = new(3, GridUnitType.Star) },
                new() { Width = new(7, GridUnitType.Star) },
            }
        };
        grid.Add(DeleteButton, column: 0, row: 0);
        grid.Add(OpenButton, column: 1, row: 0);
        grid.Add(DescriptionLabel, column: 0, row: 1);
        grid.Add(DetailsLabel, column: 1, row: 1);
        Stack.Add(grid);
    }

    public void HideDeleteButton() => DeleteButton.IsVisible = false;

    private async void DeleteButtonClicked(object sender, EventArgs e)
    {
        Grid parent = (Grid)Parent;
        
        await Tween.Linear(this, panel => panel.Opacity, start: 1, end: 0, seconds: .25);
        parent.Children.Remove(this);
        CustomTransformationData[] before = TransformationPersistenceManager.List();
        TransformationPersistenceManager.Delete(Data.Key);
        CustomTransformationData[] after = TransformationPersistenceManager.List();

        IView view = (IView)parent.Parent;
        while (view.Parent != null && view is not SavedTransformationPanel)
            view = (IView)view.Parent;
        if (view == null)
            return;
        await Tween.Linear(parent, grid => grid.Opacity, start: 1, end: 0, seconds: .25);
        ((SavedTransformationPanel)view).Populate();
        await Tween.Linear(parent, grid => grid.Opacity, start: 0, end: 1, seconds: .25);
    }
    

    private void OpenClicked(object sender, EventArgs e)
    {
        TransformationPersistenceManager.Load(Data.Key);
    }
}