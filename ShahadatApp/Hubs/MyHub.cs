using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ShahadatApp.Hubs
{
    [HubName("myHub")]
    public class MyHub: Hub
    {
            public static void Send(string message)
            {
                IHubContext context = GlobalHost.ConnectionManager.GetHubContext<MyHub>();
                context.Clients.All.DisplayNotification(message);
                context.Clients.All.displayStatus();
        }
        public static void PushNotification(string message)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<MyHub>();

            //bool selector(KeyValuePair<string, string> s) => recipients.Contains(s.Key);
            ////Func<KeyValuePair<string, string>, bool> selector = s => recipients.Contains(s.Value);
            //var connectionIds = Recipients.Where(selector).Select(y => y.Value).ToList();

            hubContext.Clients.All.DisplayNotification(message);
            //hubContext.Clients.Clients(connectionIds).DisplayNotification(title, url);
        }

        public override Task OnConnected()
        {
            return base.OnConnected();
        }
    }
}