using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace LauncherNet.Views;

public partial class MainWindow : Window
{
    private bool _mouseDownForWindowMoving = false;
    private PointerPoint _originalPoint;
    
    public MainWindow()
    {
        InitializeComponent();
        PointerPressed += OnPointerPressed;
        PointerMoved += OnPointerMoved;
        PointerReleased += OnPointerReleased;
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        base.OnClosing(e);
        PointerPressed -= OnPointerPressed;
        PointerMoved -= OnPointerMoved;
        PointerReleased -= OnPointerReleased;
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_mouseDownForWindowMoving) return;
        if (e.Source is Border border && border.Name == "TitleBarBorder" ||
            e.Source is TextBlock text && text.Name == "TitleBarText")
        {
            PointerPoint currentPoint = e.GetCurrentPoint(this);
            Position = new PixelPoint(Position.X + (int)(currentPoint.Position.X - _originalPoint.Position.X), Position.Y + (int)(currentPoint.Position.Y - _originalPoint.Position.Y));
        }
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.Source is Border border && border.Name == "TitleBarBorder" ||
            e.Source is TextBlock text && text.Name == "TitleBarText")
        {
            _mouseDownForWindowMoving = true;
            _originalPoint = e.GetCurrentPoint(this);
        }
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (e.Source is Border border && border.Name == "TitleBarBorder" ||
            e.Source is TextBlock text && text.Name == "TitleBarText")
        {
            _mouseDownForWindowMoving = false;
        }
    }
}