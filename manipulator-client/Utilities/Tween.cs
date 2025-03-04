using System;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Dispatching;

namespace Maynard.ImageManipulator.Client.Utilities;

using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Dispatching;

public static class Tween
{
    private const int FRAMERATE = 60;
    
    public static async Task Linear<T>(T element, Expression<Func<T, double>> property, double start, double end, double seconds, EventHandler<TweenEventArgs> onComplete = null) where T : VisualElement
    {
        if (property.Body is not MemberExpression memberExpr)
            throw new ArgumentException("Property must be a direct member expression", nameof(property));

        if (memberExpr.Member is not PropertyInfo propInfo)
            throw new ArgumentException("Expression must be a property", nameof(property));

        Func<T, double> getValue = (Func<T, double>)Delegate.CreateDelegate(typeof(Func<T, double>), propInfo.GetGetMethod()!);
        Action<T, double> setValue = (Action<T, double>)Delegate.CreateDelegate(typeof(Action<T, double>), propInfo.GetSetMethod()!);
        
        int totalFrames = (int)(seconds * FRAMERATE);
        double delta = (end - start) / totalFrames;

        // Begin the animation
        setValue(element, start);
        for (int i = 0; i < totalFrames; i++)
        {
            await Gui.Update(() => setValue(element, getValue(element) + delta));
            await Task.Delay(1000 / FRAMERATE);
        }
        await Gui.Update(() => setValue(element, end));
        onComplete?.Invoke(element, new(element));
    }
}

public class TweenEventArgs : EventArgs
{
    public VisualElement Element { get; set; }
    public TweenEventArgs(VisualElement element) => Element = element;
}