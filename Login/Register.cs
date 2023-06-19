using GTANetworkAPI;
using MySql.Data.MySqlClient;
using Server.Data;
using System;
using System.Threading.Tasks;

namespace Server.Auth
{
    internal class Register : Script
    {

        [RemoteEvent("server:RegisterAttempt")]
        public async void RegisterAttempt(Player player, string username, string email, string password)
        {
            if (await Auth.AccountExists(username))
            {
                NAPI.Task.Run(() =>
                {
                    player.SendChatMessage("Van már ilyen felhasználónév!");
                });
            }
            else if (await Auth.EmailInUse(email))
            {
                NAPI.Task.Run(() =>
                {
                    player.SendChatMessage("Van már ilyen email cím!");
                });
            }
            else if(await Auth.SocialClubInUse(player.SocialClubId))
            {
                    NAPI.Task.Run(() =>
                    {
                        player.SendChatMessage("Ehhez a Social Club fiókhoz van már felhasználó társítva!");
                    });
            }
            else if(await Auth.SerialInUse(player.Serial))
            {
                    NAPI.Task.Run(() =>
                    {
                        player.SendChatMessage("Ehhez a géphez van már felhasználó társítva!");
                    });
            }
            else
            {
                string salt = Auth.GenerateSalt(100);
                string pwdHashed = Auth.HashPassword(password, salt);
                string serial = player.Serial;
                ulong scID = player.SocialClubId;
                string scName = player.SocialClubName;
                if(await Auth.RegisterPlayer(player, username, email, pwdHashed, salt, serial, scID, scName))
                {
                    NAPI.Task.Run(() =>
                    {
                        player.TriggerEvent("client:IncorrectToken");
                    });
                }
            }
        }


    }
}
