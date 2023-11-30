using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;

namespace semigura.Hubs
{
    public class S02003Hub : Hub
    {
        public async static Task RefreshData()
        {
            //IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<S02003Hub>();
            var response = new JObject();
            response.Add(new JProperty("type", "1"));
            //await hubContext.Clients.All.SendAsync(response.ToString());
            //Clients.Group(nameof(S02003Hub)).All.SendAsync(response.ToString());
        }
    }
}