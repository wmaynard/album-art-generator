﻿using System;
using System.Collections.Generic;
using System.Linq;
using AppKit;
using CommunityToolkit.Maui.Storage;
using Foundation;
using Maynard.ImageManipulator.Client.Controls;
using Maynard.ImageManipulator.Client.Utilities;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Layouts;
using Microsoft.Maui.Storage;

namespace Maynard.ImageManipulator.Client.Pages;

public partial class MainPage : ContentPage
{
    private const string PREFERENCE_CURRENT_DIRECTORY = "currentDirectory";
    private const string PREFERENCE_PICKER_INDEXES = "pickerSelections";
    
    int count = 0;
    private List<ActionPickerRow> Pickers = new();
    private string CurrentDirectory { get; set; }

    public MainPage()
    {
        InitializeComponent();
        LoadPreferences();
        Log.Intercept(OnLogSent);
    }

    private async void RefreshImagePreview()
    {
        // TODO: Process the image here and update the progress bar / status.
    }

    public void LoadPreferences()
    {
        CurrentDirectory = Preferences.Get(PREFERENCE_CURRENT_DIRECTORY, null);
        DirectoryLabel.Text = $"Current Directory: {CurrentDirectory}";
        int[] pickerIndexes = Preferences.Get(PREFERENCE_PICKER_INDEXES, "")
            .Split(',')
            .Select(int.Parse)
            .ToArray();
        if (!pickerIndexes.Any())
            CreatePicker();
        else
            foreach (int index in pickerIndexes)
                CreatePicker(index);
    }

    public void CreatePicker(ActionPickerRow sender)
    {
        ActionPickerRow newest = CreatePicker();
        int start = Pickers.IndexOf(sender);
        int end = Pickers.Count() - 1;
        while (--end > start)
            (newest.SelectedIndex, Pickers[end].SelectedIndex) = (Pickers[end].SelectedIndex, newest.SelectedIndex);
    }
    public ActionPickerRow CreatePicker(int selectedIndex = 0)
    {
        ActionPickerRow picker = new(this, OnActionSelected)
        {
            SelectedIndex = selectedIndex
        };
        Pickers.Add(picker);
        return picker;
    }
    public void RemovePicker(ActionPickerRow picker) => Pickers.Remove(picker);
    

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
    
    protected override void OnDisappearing()
    {
        Preferences.Set(PREFERENCE_PICKER_INDEXES, string.Join(',', Pickers
            .Select(picker => picker.SelectedIndex)
        ));
        
        base.OnDisappearing();
    }

    public async void OnOpenDirectoryClicked(object sender, EventArgs e)
    {
        Log.Info("Opening directory...");

        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            FolderPickerResult result = await FolderPicker.PickAsync(CurrentDirectory ?? home);

            if (!result.IsSuccessful || result.Folder == null)
            {
                Log.Error($"Unable to pick a directory.  {result.Exception?.Message}");
                return;
            }

            string path = result.Folder.Path;
            Preferences.Set(PREFERENCE_CURRENT_DIRECTORY, path);
            DirectoryLabel.Text = $"Current directory: {path}";
        });
    }

    public void OnActionSelected(object sender, EventArgs e)
    {
        
    }
    
    public async void ShowToast(string message)
    {
        Toast toast = new();
        AbsoluteLayout.SetLayoutFlags(toast, AbsoluteLayoutFlags.PositionProportional);
        
        Point position = GetMousePosition();
        await toast.ShowToast(message, position.X, position.Y);
        
        AbsoluteLayout absoluteLayout = (AbsoluteLayout)Content;
        absoluteLayout.Children.Add(toast);
    }
    
    private Point GetMousePosition()
    {
#if WINDOWS
    var pos = Microsoft.UI.Input.PointerPoint.GetCurrentPoint(0).Position;
    return new Point(pos.X, pos.Y);
#else
        return new Point(200, 200); // Static fallback for mobile
#endif
    }
}

