using uDrive.Models.Views;

namespace uDrive.Services;

public class InteractiveService
{
    public delegate void OnItemAddedHandler(InteractiveItem item);
    public event OnItemAddedHandler? OnItemAdded;
    
    public delegate void OnItemDeletedHandler(InteractiveItem item);
    public event OnItemDeletedHandler? OnItemDeleted;

    public void InvokeOnItemAdded(InteractiveItem item) => OnItemAdded?.Invoke(item);
    public void InvokeOnItemDeleted(InteractiveItem item) => OnItemDeleted?.Invoke(item);
}