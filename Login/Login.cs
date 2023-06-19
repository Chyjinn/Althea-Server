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
        public void LoginAttempt(Player player, string username, string password, bool remember)
        {
            string playerSerial = player.Serial;
            ulong playerScId = player.SocialClubId;
            string playerScName = player.SocialClubName;
            LoginPlayer(player, username, password, playerSerial, playerScId, playerScName, remember);
        }

        [RemoteEvent("server:LoginAttemptWithToken")]
        public void LoginAttemptWithToken(Player player, uint accID, string token)
        {
            string playerSerial = player.Serial;
            ulong playerScId = player.SocialClubId;
            string playerScName = player.SocialClubName;
            LoginPlayerWithToken(player, accID, token, playerSerial, playerScId, playerScName);
        }

        public async void LoginPlayerWithToken(Player player, uint accountID, string token, string playerSerial, ulong playerScId, string playerScName)
        {
            if (await Auth.AccountExists(accountID))//0-t kapunk ha nincs ilyen account
            {
                string[] playerData = await Auth.GetLoginData(accountID);
                uint id = Convert.ToUInt32(playerData[0]);
                string username = playerData[1];
                string hash = playerData[2];
                string salt = playerData[3];
                string serial = playerData[4];
                ulong scID = Convert.ToUInt32(playerData[5]);
                string scName = playerData[6];
                if (playerSerial == serial)//Serial egyezik
                {
                    if (playerScId == scID && playerScName == scName)//Social Club egyezik
                    {
                        if (await Auth.VerifyToken(id,token))
                        {
                            string newtoken = await Auth.GenerateNewToken(id);
                            TimeSpan offset = TimeSpan.FromDays(14);
                            DateTime expiration = DateTime.Now + offset;
                            if (await Auth.SaveGeneratedToken(id, newtoken, expiration))//készítünk egy új tokent
                            {
                                if (await Auth.DeleteUsedToken(token))//töröljük a felhasznált tokent
                                {
                                    //új token mentése kliens oldalon
                                    NAPI.Task.Run(() =>
                                    {
                                        player.TriggerEvent("client:SaveToken", id, newtoken, expiration.ToString());
                                    });
                                }
                            }
                            //Token Login
                            NAPI.Task.Run(() =>
                            {
                                player.SetSharedData("player:accID", id);
                                player.TriggerEvent("client:DestroyAuthForm");
                                Selector.ProcessCharScreen(player);
                            });
                        }
                        else
                        {
                            NAPI.Task.Run(() =>
                            {
                                player.TriggerEvent("client:IncorrectToken");
                            });
                            //klienst átirányítani a sima bejelentkezéshez
                        }
                    }
                    else
                    {
                        await Auth.DeleteUsedToken(token);
                        NAPI.Task.Run(() =>
                        {
                            player.SendChatMessage("Social Club Error");
                        });
                    }
                }
                else
                {
                    await Auth.DeleteUsedToken(token);
                    NAPI.Task.Run(() =>
                    {
                        
                        player.SendChatMessage($"Serial error");
                    });
                }
            }
            else
            {
                await Auth.DeleteUsedToken(token);
                NAPI.Task.Run(() =>
                {
                    player.SendChatMessage("Nincs ilyen user");
                });
            }
        }

        public async void LoginPlayer(Player player,string username, string password,string playerSerial,ulong playerScId,string playerScName, bool remember)
        {
            if (await Auth.AccountExists(username))//0-t kapunk ha nincs ilyen account
            {
                string[] playerData = await Auth.GetLoginData(username);
                uint id = Convert.ToUInt32(playerData[0]);
                string hash = playerData[2];
                string salt = playerData[3];
                string serial = playerData[4];
                ulong scID = Convert.ToUInt32(playerData[5]);
                string scName = playerData[6];
                if (playerSerial == serial)//Serial egyezik
                {
                    if (playerScId == scID && playerScName == scName)//Social Club egyezik
                    {
                        if (Auth.verifyPassword(password, hash, salt))//Jelszó ellenőrzés
                        {
                            if (remember)//ha szeretné hogy emlékezzenek rá
                            {
                                string newtoken = await Auth.GenerateNewToken(id);
                                TimeSpan offset = TimeSpan.FromDays(14);
                                DateTime expiration = DateTime.Now + offset;
                                if (await Auth.SaveGeneratedToken(id, newtoken, expiration))//készítünk egy új tokent
                                {
                                        //új token mentése kliens oldalon
                                        NAPI.Task.Run(() =>
                                        {
                                            player.TriggerEvent("client:SaveToken", id, newtoken, expiration.ToString());
                                        });
                                }
                            }
                            //Login
                            NAPI.Task.Run(() =>
                            {
                                player.SetSharedData("player:accID", id);
                                player.TriggerEvent("client:DestroyAuthForm");
                                Selector.ProcessCharScreen(player);

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
