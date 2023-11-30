namespace semigura.Hubs
{
    public interface IChatClient
    {
        Task ReceiveMessage(string user, string message);
    }

    //public class ChatHub : Hub
    //{
    //    public async Task SendMessage(string user, string message)
    //    {
    //        await Clients.All.SendAsync("ReceiveMessage", user, message);
    //    }
    //}
}