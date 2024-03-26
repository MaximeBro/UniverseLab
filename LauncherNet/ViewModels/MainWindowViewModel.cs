using System.Windows.Input;
using Avalonia.Controls;
using LauncherNet.Views;
using ReactiveUI;

namespace LauncherNet.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public MainPageViewModel MainPage { get; set; }
    public ICommand CloseCommand { get; }

    public MainWindowViewModel(Window window) : base(window)
    {
        MainPage = new MainPageViewModel((MainWindow)Window);
        
        CloseCommand = ReactiveCommand.Create(CloseApp);
    }

    private void CloseApp() => Window.Close();
}