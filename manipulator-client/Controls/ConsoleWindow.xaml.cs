using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Foundation;
using Maynard.ImageManipulator.Client.Utilities;

namespace Maynard.ImageManipulator.Client.Controls;

public partial class ConsoleWindow : ContentView
{
    public ConsoleWindow()
    {
        InitializeComponent();
        Log.Intercept(OnLogSent);
    }
    
    private void OnLogSent(object sender, LogEventArgs log)
    {
        ConsoleLog.Text += $"{log.Message}{Environment.NewLine}";
        // Fix sourced from: https://stackoverflow.com/questions/78298939/net-maui-editor-does-not-scroll-to-new-line-when-text-is-added-programattically
        UIKit.UITextView tv = (UIKit.UITextView)ConsoleLog.Handler?.PlatformView;
        if (tv == null)
            return;
        tv.ScrollEnabled = true;
        NSRange range = new (0, ConsoleLog.Text.Length);
        tv.ScrollRangeToVisible(range);
    }
}