using System.Collections.Concurrent;
using Maynard.ImageManipulator.Client.Utilities;

namespace Maynard.ImageManipulator.Client.Controls;

public class FadeInVerticalStack : VerticalStackLayout
{
    private readonly ConcurrentQueue<VisualElement> _animationQueue = new();
    private bool IsAnimating { get; set; }
    
    public FadeInVerticalStack() { }

    public new void Add(IView child, InitialFade fade = InitialFade.Invisible)
    {
        base.Add(child);
        if (child is not VisualElement element)
            return;
        if (child is EmptySpace)
            return;

        element.IsVisible = fade switch
        {
            InitialFade.Visible => true,
            InitialFade.Invisible => false,
            InitialFade.BeginAnimation => false,
            _ => false
        };

        if (!element.IsVisible)
            Enqueue(element, start: fade == InitialFade.BeginAnimation);
    }

    private void Enqueue(VisualElement child, bool start = false)
    {
        bool restart = _animationQueue.Count == 0;
        _animationQueue.Enqueue(child);
        if (restart && !IsAnimating && start)
            BeginAnimation();
    }

    private async void BeginAnimation(object sender = null, TweenEventArgs e = null)
    {
        if (e != null)
        {
            int index = Children.IndexOf(e.Element);
            string name = e.Element.GetType().Name;
            Log.Info($"Animation finished. (#{index} | {name} | Opacity: {e.Element.Opacity} | Visible: {e.Element.IsVisible})");
        }
        Log.Info("Starting animation...");
        await Gui.Update(async () =>
        {
            if (!_animationQueue.TryDequeue(out VisualElement element))
                return;
            while (element is EmptySpace)
                if (!_animationQueue.TryDequeue(out element))
                    return;
            IsAnimating = true;
            element.Opacity = 0;
            element.IsVisible = true;
            await Tween.Linear(element, ele => ele.Opacity, 0, 1, seconds: 1, onComplete: BeginAnimation);
        });
        IsAnimating = false;
    }

    public void Hide()
    {
        foreach (IView child in Children)
        {
            if (child is not VisualElement element)
                continue;
            element.IsVisible = false;
            Enqueue(element);
        }
    }

    public void Show() => BeginAnimation();
}

public enum InitialFade
{
    Visible,
    Invisible,
    BeginAnimation
}