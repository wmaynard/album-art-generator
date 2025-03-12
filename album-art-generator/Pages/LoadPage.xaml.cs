using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Maynard.ImageManipulator.Client.Controls.Panels;

namespace Maynard.ImageManipulator.Client.Pages;

public partial class LoadPage : ContentPage
{
    public LoadPage()
    {
        InitializeComponent();
    }

    private void VisualElement_OnLoaded(object sender, EventArgs e)
    {
        if (sender is SavedTransformationPanel panel)
            panel.Populate();
    }
}