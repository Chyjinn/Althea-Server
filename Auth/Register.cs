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
                player.SendChatMessage("Van ilyen felhasználónév!");
            }
            else if (await Auth.EmailInUse(email))
            {
                player.SendChatMessage("Van már ilyen email cím!");
            }
            else if(await Auth.SocialClubInUse(player.SocialClubId))
            {
                player.SendChatMessage("Ehhez a Social Club fiókhoz van már felhasználó társítva!");
            }
            else if(await Auth.SerialInUse(player.Serial))
            {
                player.SendChatMessage("Ehhez a géphez van már felhasználó társítva!");
            }
            else
            {
                string salt = Auth.GenerateSalt(100);
                string pwdHashed = Auth.HashPassword(password, salt);
                Auth.RegisterPlayer(player, username, email, pwdHashed, salt);
            }
        }


    }
}
