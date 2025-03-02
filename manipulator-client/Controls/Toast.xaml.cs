using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Maynard.ImageManipulator.Client.Controls;

public partial class Toast : ContentView
{
    private Point _initialMousePosition;
    private bool _shouldDismiss = false;
    
    public Toast()
    {
        InitializeComponent();
    }
    
    public async Task ShowToast(string message, double x, double y)
    {
        ToastLabel.Text = message;

        // Set position relative to mouse cursor
        AbsoluteLayout.SetLayoutBounds(this, new Rect(x - 10, y - 10, 200, 200));

        // Fade in
        await this.FadeTo(1, 500, Easing.CubicInOut);

        _initialMousePosition = new Point(x, y);
        _shouldDismiss = false;

        // Start dismissal logic
        await Task.WhenAny(Task.Delay(5000), WaitForMouseMove());

        if (!_shouldDismiss)
        {
            await this.FadeTo(0, 500, Easing.CubicInOut);
            ((AbsoluteLayout)Parent)?.Children.Remove(this);
        }
    }
    
    private async Task WaitForMouseMove()
    {
        while (!_shouldDismiss)
        {
            var position = GetMousePosition();
            double distance = Math.Sqrt(Math.Pow(position.X - _initialMousePosition.X, 2) +
                                        Math.Pow(position.Y - _initialMousePosition.Y, 2));

            if (distance > 50)
            {
                _shouldDismiss = true;
                await this.FadeTo(0, 500, Easing.CubicInOut);
                ((AbsoluteLayout)Parent)?.Children.Remove(this);
                break;
            }

            await Task.Delay(100);
        }
    }
    
    private Point GetMousePosition()
    {
        // This is platform-dependent
#if WINDOWS
            var pos = Microsoft.UI.Input.PointerPoint.GetCurrentPoint(0).Position;
            return new Point(pos.X, pos.Y);
#else
        return new Point(0, 0); // No direct way on mobile, can use gestures instead
#endif
    }
}