using System;
using GTANetworkAPI;

namespace Server.Login
{
    internal class Login : Script
    {

        [RemoteEvent("server:LoginAttempt")]
        public void LoginAttempt(Player player, string username, string password)
        {
            NAPI.Notification.SendNotificationToPlayer(player, "Username: " + username + " - Password: " + password, false);

        }

    }
}
