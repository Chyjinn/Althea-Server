using GTANetworkAPI;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using Server.Characters;
using Server.Data;
using System;
using System.Threading.Tasks;

namespace Server.Auth
{
    internal class Login : Script
    {
        [RemoteEvent("server:LoginAttempt")]
        public void LoginAttempt(Player player, string username, string password)
        {
            string playerSerial = player.Serial;
            ulong playerScId = player.SocialClubId;
            string playerScName = player.SocialClubName;
            LoginPlayer(player, username, password, playerSerial, playerScId, playerScName);
        }

        public async void LoginPlayer(Player player,string username, string password,string playerSerial,ulong playerScId,string playerScName)
        {
            if (await Auth.AccountExists(username))//0-t kapunk ha nincs ilyen account
            {
                string[] playerData = await Auth.GetLoginData(username);
                uint id = Convert.ToUInt32(playerData[0]);
                string hash = playerData[2];
                string salt = playerData[3];
                string token = playerData[4];
                string serial = playerData[5];
                ulong scID = Convert.ToUInt32(playerData[6]);
                string scName = playerData[7];
                if (playerSerial == serial)//Serial egyezik
                {
                    if (playerScId == scID && playerScName == scName)//Social Club egyezik
                    {
                        if (Auth.verifyPassword(password, hash, salt))//Jelszó ellenőrzés
                        {
                            //Login
                            NAPI.Task.Run(() =>
                            {
                                player.SetSharedData("player:accID", id);
                                player.TriggerEvent("client:DestroyAuthForm");
                                CharacterScreen.ProcessCharScreen(player);
                            });
                        }
                        else
                        {
                            NAPI.Task.Run(() =>
                            {
                                player.SendChatMessage("Helytelen jelszó");
                            });
                            //Helytelen jelszó
                        }
                    }
                    else
                    {
                        NAPI.Task.Run(() =>
                        {
                            player.SendChatMessage("Social Club Error");
                        });
                    }
                }
                else
                {
                        NAPI.Task.Run(() =>
                        {
                            player.SendChatMessage($"Serial error");
                        });
                }
            }
            else
            {
                        NAPI.Task.Run(() =>
                        {
                            player.SendChatMessage("Nincs ilyen user");
                        });
            }
        }

        public async Task LoginSuccess(Player player)
        {
            player.SendChatMessage("Sikeres bejelentkezés");
        }

        public void LoginFailed(Player player)
        {
            player.SendChatMessage("valami nem jó");
        }

        public void OnTimeOut(Player player)
        {
            player.SendChatMessage("Várj egy kicsit az újra próbálkozással");
        }
    }
}
