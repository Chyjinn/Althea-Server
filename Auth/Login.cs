using GTANetworkAPI;
using MySql.Data.MySqlClient;
using Server.Data;
using System;
using System.Threading.Tasks;

namespace Server.Auth
{
    internal class Login : Script
    {
        [RemoteEvent("server:LoginAttempt")]
        public async void LoginAttempt(Player player, string username, string password)
        {
            bool timeout = false;
            if (NAPI.Data.HasEntitySharedData(player, "server:LoginTimeout"))
            {
                timeout = (bool)NAPI.Data.GetEntitySharedData(player, "server:LoginTimeout");
            }

                
            if (!timeout)
            {
                if (await Auth.AccountExists(username))
                {
                    if (await Auth.LoginPlayer(player,username,password))
                    {
                        player.SendChatMessage("Sikeres bejelentkezés!");
                    }
                    else
                    {
                        Auth.TimeoutPlayer(player);
                    }
                }
                else
                {
                    Auth.TimeoutPlayer(player);
                    player.SendChatMessage("Nem létezik ilyen felhasználónév");
                }
            }
            else
            {
                player.SendChatMessage("Várj egy kicsit az újra próbálkozással");
            }
        }
    }
}
