using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Maynard.ImageManipulator.Client.Utilities;
using Microsoft.Maui.Controls;

namespace Maynard.ImageManipulator.Client.Pages;

public partial class HelpPage : ContentPage
{
    public HelpPage()
    {
        InitializeComponent();

        Dictionary<string, string> images = new()
        {
            { "blur.jpg", ActionDescriptions.HELP_BLUR },
            { "crop.jpg", ActionDescriptions.HELP_CROP_TO_SQUARE },
            { "dim.jpg", ActionDescriptions.HELP_DIM },
            { "resize.jpg", ActionDescriptions.HELP_RESIZE },
            { "scale_min_to.jpg", ActionDescriptions.HELP_SCALE_MIN_TO },
            { "scale_to_max.jpg", ActionDescriptions.HELP_SCALE_TO_MAX_DIMENSION },
            { "spotify.jpg", ActionDescriptions.HELP_SPOTIFY },
            { "superimpose.jpg", ActionDescriptions.HELP_SUPERIMPOSE }
        };

        bool debugColors = false;
        int row = 0;
        foreach (KeyValuePair<string, string> pair in images)
        {
            Image image = new()
            {
                Source = pair.Key,
                WidthRequest = 900,
                Aspect = Aspect.AspectFit,
                VerticalOptions = LayoutOptions.Start  // Ensure image is placed at the row's top
            };
            if (debugColors)
                image.BackgroundColor = WdmColors.PURPLE;
            
            Label label = new()
            {
                Text = pair.Value,
                FontSize = 16,
                VerticalOptions = LayoutOptions.Start, // Align text with the top of the image
                LineBreakMode = LineBreakMode.WordWrap
            };
            if (debugColors)
                label.BackgroundColor = WdmColors.RED;

            HelpGrid.Add(image, column: 0, row: row);
            HelpGrid.Add(label, column: 1, row: row++);
        }
        if (debugColors)
            HelpGrid.BackgroundColor = WdmColors.LIME;
    }
}