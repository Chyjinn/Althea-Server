using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Auth
{
    internal class Register : Script
    {
        [RemoteEvent("server:RegisterAttempt")]
        public void RegisterAttempt(Player player, string username, string email, string password1, string password2)
        {
            NAPI.Notification.SendNotificationToPlayer(player, "Username: " + username + " - Password: " + password1 + " - Email: " + email, false);
        }
    }
}
