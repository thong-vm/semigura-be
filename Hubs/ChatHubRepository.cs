using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace semigura.Hubs
{
    public class ChatHubRepository : IChatHubRepository
    {
        private IHubContext<StronglyTypedChatHub, IChatClient> _strongChatHubContext { get; }

        public ChatHubRepository(IHubContext<StronglyTypedChatHub, IChatClient> chatHubContext)
        {
            _strongChatHubContext = chatHubContext;
        }
        public int ClientsCount
        {
            get
            {
                return 0; //TODO
            }
        }

        public async Task SendMessage(string user, string message)
            => await _strongChatHubContext.Clients.All.ReceiveMessage(user, message);

        public async Task SendMessageToGroup(string user, string message, string group = "SignalR Users")
            => await _strongChatHubContext.Clients.Group(group).ReceiveMessage(user, message);

        public async Task NotifyOnSensorUpdated()
        {
            var msg = JsonSerializer.Serialize(new { type = "1" });
            var user = "Server";
            await SendMessage(user, msg);
        }

        public async Task NotifyRefreshData()
        {
            var msg = JsonSerializer.Serialize(new { type = "1" });
            var user = "Server";
            await SendMessage(user, msg);
        }

        public async Task NotifyOnCloseNotification()
        {
            var msg = JsonSerializer.Serialize(new { type = "1" });
            var user = "Server";
            await SendMessage(user, msg);
        }
    }
}