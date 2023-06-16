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

            if (Password.verifypassword(password1, pwdHashed, salt))
            {

                NAPI.Notification.SendNotificationToPlayer(player, "Sikeres regisztráció!", false);
            }
            else
            {
                NAPI.Notification.SendNotificationToPlayer(player, "Nem működik", false);
            }
            
        }
    }
}
