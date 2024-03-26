using Avalonia.Controls;
using ReactiveUI;

namespace LauncherNet.ViewModels;

public class ViewModelBase : ReactiveObject
{
    public Window Window { get; init; }

    protected ViewModelBase(Window window)
    {
        Window = window;
    }
}