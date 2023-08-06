namespace StreamingHub.Notifications
{
    using Microsoft.Azure.NotificationHubs;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class Notifications
    {
        public static Notifications Instance = new Notifications();

        public NotificationHubClient Hub { get; }

        private Notifications()
        {
            Hub = NotificationHubClient.CreateClientFromConnectionString(
                "Endpoint=sb://zeeaquarium-notificaties.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=Kkg95FORUwiP6+/Vfs0SIcADcGCCRa/qrPKJ6SUo7bg=",
                "zeeaquarium-notificatie-hub");
        }
    }
}
