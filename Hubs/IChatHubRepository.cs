namespace semigura.Hubs
{
    public interface IChatHubRepository
    {
        int ClientsCount { get; }

        Task SendMessage(string user, string message);
        Task SendMessageToGroup(string user, string message, string group = "SignalR Users");

        Task NotifyOnSensorUpdated();
        Task NotifyRefreshData();
        Task NotifyOnCloseNotification();
    }
}