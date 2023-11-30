using Microsoft.AspNetCore.SignalR;

namespace semigura.Hubs
{
    public class StronglyTypedChatHub : Hub<IChatClient>
    {
        public async Task SendMessage(string user, string message)
            => await Clients.All.ReceiveMessage(user, message);

        public async Task SendMessageToCaller(string user, string message)
            => await Clients.Caller.ReceiveMessage(user, message);

        public async Task SendMessageToGroup(string user, string message, string group = "SignalR Users")
            => await Clients.Group(group).ReceiveMessage(user, message);
    }
}