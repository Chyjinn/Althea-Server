using GTANetworkAPI;
using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;

namespace Server.Auth
{
    internal class Register : Script
    {

        [RemoteEvent("server:RegisterAttempt")]
        public void RegisterAttempt(Player player, string username, string email, string password)
        {
            string playerSerial = player.Serial;
            ulong playerScId = player.SocialClubId;
            string playerScName = player.SocialClubName;
            RegisterPlayer(player, username, email, password, playerSerial, playerScId, playerScName);//player adatait megszerezni - RAGE API hívások csak main thread-en mennek
        }

        public async void RegisterPlayer(Player player, string username, string email, string password, string playerSerial, ulong playerScId, string playerScName)//itt kezeljük a regisztrációt
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
            /*else if(await Auth.SocialClubInUse(playerScId))
            {
                    NAPI.Task.Run(() =>
                    {
                        player.SendChatMessage("Ehhez a Social Club fiókhoz van már felhasználó társítva!");
                    });
            }
            else if(await Auth.SerialInUse(playerSerial))
            {
                    NAPI.Task.Run(() =>
                    {
                        player.SendChatMessage("Ehhez a géphez van már felhasználó társítva!");
                    });
            }*/
            else//az összes ellenőrzés rendben van, regisztrálhatjuk a játékost
            {
                string salt = Auth.GenerateSalt(100);
                string pwdHashed = Auth.HashPassword(password, salt);
                if(await Auth.RegisterPlayer(player, username, email, pwdHashed, salt, playerSerial, playerScId, playerScName))//megpróbálunk regisztrálni
                {
                    //ha sikerült akkor akár be is jelentkeztethetjük a playert, hiszent új account - vagy átrakjuk a bejelentkezéshez
                    NAPI.Task.Run(() =>
                    {
                        player.TriggerEvent("client:IncorrectToken");
                    });
                }
                else
                {
                    //nem sikerült a regisztráció, TODO: valamit jelezzen
                }
            }
        }


    }
}
