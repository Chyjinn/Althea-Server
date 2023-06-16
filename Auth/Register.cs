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
            string salt = Password.GenerateSalt(70);
            string pwdHashed = Password.HashPassword(password1, salt);

            //NAPI.Notification.SendNotificationToPlayer(player, password1 + " HASH: " + pwdHashed + " SALT: " + salt, false);

            if (Password.verifypassword(password1, pwdHashed, salt))
            {
                NAPI.Chat.SendChatMessageToPlayer(player, pwdHashed);
                NAPI.Notification.SendNotificationToPlayer(player, "Működik", false);
            }
            else
            {
                NAPI.Notification.SendNotificationToPlayer(player, "Nem működik", false);
            }
            
        }
    }
}
