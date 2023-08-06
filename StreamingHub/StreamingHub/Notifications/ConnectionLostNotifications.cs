namespace StreamingHub.Notifications
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public static class ConnectionLostNotifications
    {
        private static int TIME_TO_FIRST_CONNECTION_LOST_MESSAGE = 900;

        private static string MESSAGE_TITLE = "Internet verbinding controller verbroken!";

        private static Task _notificationsOnConnectionLost;

        private static CancellationTokenSource _cancellationTokenSource;

        public static void RefreshNotificationsOnConnectionLost()
        {
            if (_notificationsOnConnectionLost != null)
            {
                _cancellationTokenSource.Cancel();
            }

            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;

            _notificationsOnConnectionLost = Task.Run(() =>
            {
                var count = 0;
                while (count < TIME_TO_FIRST_CONNECTION_LOST_MESSAGE)
                {
                    count++;

                    Thread.Sleep(1000);

                    cancellationToken.ThrowIfCancellationRequested();
                }

                var notificationHub = Notifications.Instance.Hub;

                var hoursConnectionLost = count / 60.0;
                var messageBody = $"Controller heeft al minstens {hoursConnectionLost:F0} minuten geen internet verbinding";
                var message = "{ \"notification\" : { \"title\": \"" + MESSAGE_TITLE + "\", \"body\":\"" + messageBody + "\"}}";

                notificationHub.SendFcmNativeNotificationAsync(message, cancellationToken);
            }, cancellationToken);
        }
    }
}
