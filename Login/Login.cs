using GTANetworkAPI;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using Server.Characters;
using System;
using System.Diagnostics.Tracing;
using System.Threading.Tasks;

namespace Server.Auth
{
    internal class Login : Script
    {
        [ServerEvent(Event.PlayerConnected)]
        public void OnPlayerConnect(Player player)
        {
            player.SetSharedData("player:Frozen", true);
            string serial = player.Serial;
            ulong socialID = player.SocialClubId;


            CheckBan(player,serial,socialID);

            player.TriggerEvent("client:SkyCam", true);
            
            player.Dimension = 9;
        }

        public async void CheckBan(Player player, string serial, ulong scID)
        {
            if (await Auth.IsBanned(scID, serial))//ha bannolva van a serial vagy social club
            {
                string[] banData = await Auth.GetBanData(scID, serial);
                NAPI.Task.Run(() =>
                {
                    player.TriggerEvent("client:BanScreen", banData[1], banData[2], banData[3], banData[4]);
                    player.TriggerEvent("client:HUD", false);
                });
                
            }
            else
            {
                NAPI.Task.Run(() =>
                {
                    player.TriggerEvent("client:LoginScreen");
                    player.TriggerEvent("client:HUD",false);
                });
                
            }
        }



        [RemoteEvent("server:LoginAttempt")]//kliens hívja meg (login.html)
        public void LoginAttempt(Player player, string username, string password, bool remember)
        {
            string playerSerial = player.Serial;
            ulong playerScId = player.SocialClubId;
            string playerScName = player.SocialClubName;
            //ide be kell illeszteni egy ban ellenőrzést (username, serial és social club id alapján)
            LoginPlayer(player, username, password, playerSerial, playerScId, playerScName, remember);//eddig azért nem használtunk async-et hogy megszerezzük a játékos serial,SocialClub adatait - RAGE API hívás csak main threaden
        }

        [RemoteEvent("server:LoginAttemptWithToken")]//kliens hívja meg (logintoken.html)
        public void LoginAttemptWithToken(Player player, uint accID, string token)
        {
            string playerSerial = player.Serial;
            ulong playerScId = player.SocialClubId;
            string playerScName = player.SocialClubName;
            //ide be kell illeszteni egy ban ellenőrzést (accid, serial és social club id alapján)
            LoginPlayerWithToken(player, accID, token, playerSerial, playerScId, playerScName);//eddig azért nem használtunk async-et hogy megszerezzük a játékos serial,SocialClub adatait - RAGE API hívás csak main threaden
        }

        public async void LoginPlayerWithToken(Player player, uint accountID, string token, string playerSerial, ulong playerScId, string playerScName)//átváltunk async-ra az adatbáziskezelés miatt
        {
            if (await Auth.AccountExists(accountID))//0-t kapunk ha nincs ilyen account
            {
                string[] playerData = await Auth.GetLoginData(accountID);//létezik az account szóval lekérdezzük az adatait, elmentjük és később össze hasonlítjuk
                uint id = Convert.ToUInt32(playerData[0]);
                string username = playerData[1];
                string hash = playerData[2];
                string salt = playerData[3];
                string serial = playerData[4];
                ulong scID = Convert.ToUInt32(playerData[5]);
                string scName = playerData[6];
                uint characterSlots = Convert.ToUInt32(playerData[7]);
                
                if (await Auth.VerifyToken(id, token))//Token ellenőrzés
                {
                    if (true)//playerSerial == serial)//Serial egyezik
                    {
                        if (playerScId == scID && playerScName == scName)//Social Club egyezik
                        {
                            string newtoken = await Auth.GenerateNewToken(id);
                            TimeSpan offset = TimeSpan.FromDays(14);//hány napig éljenek a mentett token-ek
                            DateTime expiration = DateTime.Now + offset;
                            if (await Auth.SaveGeneratedToken(id, newtoken, expiration))//mivel token-el jelentkezett be ezért mindenképpen menteni fogunk egy újat
                            {
                                if (await Auth.DeleteUsedToken(token))//töröljük a felhasznált tokent az adatbázisból
                                {
                                    Tuple<int, string> adminData = await Auth.GetAdminData(id);

                                    //Token Login sikeres, töröljük a bejelentkezési formot és tovább küldjük a playert a karakter választóba
                                    //mentjük az új tokent kliens oldalon
                                    NAPI.Task.Run(() =>
                                    {
                                        player.TriggerEvent("client:SaveToken", id, newtoken, expiration.ToString());
                                        player.SetData("player:accID", id);
                                        player.SetData("player:charSlots", characterSlots);
                                        player.TriggerEvent("client:DestroyAuthForm");
                                        Selector.ProcessCharScreen(player);

                                        player.SetData("player:AdminLevel", adminData.Item1);
                                        player.SetData("player:AdminNick", adminData.Item2);

                                    });
                                }
                            }
                        }
                        else//Social Club
                            {
                                await Auth.DeleteUsedToken(token);
                                NAPI.Task.Run(() =>
                                {
                                    player.SendChatMessage("Social Club Error");
                                });
                            }
                        }
                        else//serial
                        {
                            await Auth.DeleteUsedToken(token);
                            NAPI.Task.Run(() =>
                            {

                                player.SendChatMessage($"Serial error");
                            });
                        }
                }
                else//token ellenőrzés
                {
                    if (await Auth.TokenInUse(token))//nem sikerült a token megerősítése, ezért megnézzük hogy létezik-e
                    {
                        //ha létezik a token akkor töröljük
                        await Auth.DeleteUsedToken(token);
                    }
                    //Token Login sikertelen, klienst átirányítjuk a sima bejelentkezéshez
                    NAPI.Task.Run(() =>
                    {
                        player.TriggerEvent("client:IncorrectToken");
                    });
                }
            }
            else//account nem létezik
            {
                if (await Auth.TokenInUse(token))//ha a token létezik de az account nem hozzá való
                {
                    //töröljük a tokent
                    await Auth.DeleteUsedToken(token);
                }
                NAPI.Task.Run(() =>
                {
                    player.SendChatMessage("Nincs ilyen user");
                });
            }
        }

        public async void LoginPlayer(Player player,string username, string password,string playerSerial,ulong playerScId,string playerScName, bool remember)//átváltunk async-ra az adatbázis kezelés miatt
        {
            if (await Auth.AccountExists(username))//0-t kapunk ha nincs ilyen account
            {
                string[] playerData = await Auth.GetLoginData(username);//megszerezzük a felhasználó adatait és mentjük
                uint id = Convert.ToUInt32(playerData[0]);
                string hash = playerData[2];
                string salt = playerData[3];
                string serial = playerData[4];
                ulong scID = Convert.ToUInt32(playerData[5]);
                string scName = playerData[6];
                uint characterSlots = Convert.ToUInt32(playerData[7]);
                if (true)//19playerSerial == serial)//Serial egyezik
                {
                    if (playerScId == scID && playerScName == scName)//Social Club egyezik
                    {
                        if (Auth.verifyPassword(password, hash, salt))//Jelszó ellenőrzés
                        {
                            if (remember)//ha szeretné hogy emlékezzenek rá
                            {
                                string newtoken = await Auth.GenerateNewToken(id);
                                TimeSpan offset = TimeSpan.FromDays(14);//token offset, napban
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
                            //Külön beléptetjük a játékost akár sikerült menteni az új tokent akár nem
                            NAPI.Task.Run(() =>
                            {
                                player.SetData("player:accID", id);
                                player.SetData("player:charSlots", characterSlots);
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


        //Ezeket még meg kell vizsgálni majd ha lesz notification rendszer
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
