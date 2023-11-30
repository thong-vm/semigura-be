using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;

namespace semigura.Hubs
{
    public class S01002Hub : Hub
    {
        public async static Task RefreshData()
        {
            //IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<S01002Hub>();
            var response = new JObject();
            response.Add(new JProperty("type", "1"));
            //await hubContext.Clients.All.SendAsync(response.ToString());
            //await Clients.Group(nameof(S01002Hub)).All.SendAsync(response.ToString());
        }
    }
}