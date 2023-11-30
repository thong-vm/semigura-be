using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;
using semigura.DBContext.Business;

namespace semigura.Hubs
{
    public class NotificationHub : Microsoft.AspNetCore.SignalR.Hub
    {
        public static HashSet<string> ConnectedIds = new HashSet<string>();

        public async static Task SendOneMsgNotification(string message)
        {
            //IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            //if (Clients != null) await Clients.All.SendAsync(message);
        }

        public override Task OnConnectedAsync()
        {
            ConnectedIds.Add(Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            ConnectedIds.Remove(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }

        public async static Task RefreshDataNotification()
        {
            //IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            var response = GetLastestNotification();
            //await hubContext.Clients.All.SendAsync(response.ToString());
            //await Clients.All.SendAsync(response.ToString());
        }

        public void Send(string message)
        {
            if (message == "1")
            {
                //IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();

                var response = GetLastestNotification();

                //hubContext.Clients.All.SendAsync(response.ToString());
                Clients.All.SendAsync(response.ToString());
            }
        }

        private static JObject GetLastestNotification()
        {
            //TODO
            var business = new S03005Business(new DBContext.Entities.DBEntities(), null); // TODO
            var list = business.GetListNotificationNotCompleted();

            var response = new JObject();
            response.Add(new JProperty("type", "1"));
            response.Add(new JProperty("notifications", (JArray)JToken.FromObject(list)));

            return response;
        }

    }
}
