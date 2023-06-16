using System;
using GTANetworkAPI;
using Server.Auth;

namespace Server.Login
{
    internal class Login : Script
    {

        [RemoteEvent("server:LoginAttempt")]
        public void LoginAttempt(Player player, string username, string password)
        {
            NAPI.Notification.SendNotificationToPlayer(player, password, false);
            /*Password p = new Password();
            string hashedPw = p.Hash(password);
            
            bool correct = p.Validate(hashedPw, password);
            if (correct)
            {
                NAPI.Notification.SendNotificationToPlayer(player, "Correct", false);
            }
            else
            {
                NAPI.Notification.SendNotificationToPlayer(player, "Incorrect", false);
            }
            */
        }

    }
}
