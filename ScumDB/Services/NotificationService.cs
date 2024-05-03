namespace ScumDB.Services;

public class NotificationService
{
	public delegate Task AuthenticationNotification();

	public event AuthenticationNotification OnAuthenticationNotificationReceived = null!;

	public void SendAuthNotification()
	{
		OnAuthenticationNotificationReceived?.Invoke();
	}
}