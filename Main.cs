﻿using System;
using System.Threading;
using System.IO;
using GTANetworkAPI;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.AccessControl;

namespace Server
{
    public static class WeaponDatabase
    {
        private static Dictionary<uint, string> weaponDictionary;

        static WeaponDatabase()
        {
            // Initialize the dictionary with the provided data
            weaponDictionary = new Dictionary<uint, string>()
        {
            {2460120199, "Antik Lovas Kard"},
            {2508868239, "Baseball Ütő"},
            {4192643659, "Törött üveg"},
            {2227010557, "Feszítővas"},
            {2725352035, "Ököl"},
            {2343591895, "Zseblámpa"},
            {1141786504, "Golfütő"},
            {1317494643, "Kalapács"},
            {4191993645, "Fejsze"},
            {3638508604, "Boxer"},
            {2578778090, "Kés"},
            {3713923289, "Machete"},
            {3756226112, "Rugós Kés"},
            {1737195953, "Gumibot"},
            {419712736, "Csőkulcs"},
            {3441901897, "Harci Balta"},
            {2484171525, "Billiárd Dákó"},
            {940833800, "Kőfejsze"},
            {453432689, "Pistol"},
            {3219281620, "Pistol MK2"},
            {1593441988, "Combat Pistol"},
            {584646201, "AP Pistol"},
            {911657153, "Stun Gun"},
            {2578377531, "Pistol .50"},
            {3218215474, "SNS Pistol"},
            {2285322324, "SNS Pistol MK2"},
            {3523564046, "Heavy Pistol"},
            {137902532, "Vintage Pistol"},
            {1198879012, "Flare Gun"},
            {3696079510, "Marksman Pistol"},
            {3249783761, "Heavy Revolver"},
            {3415619887, "Heavy Revolver MK2"},
            {2548703416, "Double Action"},
            {2939590305, "Up-n-Atomizer"},
            {324215364, "Micro SMG"},
            {736523883, "SMG"},
            {2024373456, "SMG MK2"},
            {4024951519, "Assault SMG"},
            {171789620, "Combat PDW"},
            {3675956304, "Machine Pistol"},
            {3173288789, "Mini SMG"},
            {1198256469, "Unholy Hellbringer"},
            {487013001, "Pump Shotgun"},
            {1432025498, "Pump Shotgun MK2"},
            {2017895192, "Sawed-Off Shotgun"},
            {3800352039, "Assault Shotgun"},
            {2640438543, "Bullpup Shotgun"},
            {2828843422, "Musket"},
            {984333226, "Heavy Shotgun"},
            {4019527611, "Double Barrel Shotgun"},
            {317205821, "Sweeper Shotgun"},
            {3220176749, "Assault Rifle"},
            {961495388, "Assault Rifle MK2"},
            {2210333304, "Carbine Rifle"},
            {4208062921, "Carbine Rifle MK2"},
            {2937143193, "Advanced Rifle"},
            {3231910285, "Special Carbine"},
            {2526821735, "Special Carbine MK2"},
            {2132975508, "Bullpup Rifle"},
            {2228681469, "Bullpup Rifle MK2"},
            {1649403952, "Compact Rifle"},
            {2634544996, "MG"},
            {2144741730, "Combat MG"},
            {3686625920, "Combat MG MK2"},
            {1627465347, "Gusenberg Sweeper"},
            {100416529, "Sniper Rifle"},
            {205991906, "Heavy Sniper"},
            {177293209, "Heavy Sniper MK2"},
            {3342088282, "Marksman Rifle"},
            {1785463520, "Marksman Rifle MK2"},
            {2982836145, "RPG"},
            {2726580491, "Grenade Launcher"},
            {1305664598, "Smoke Grenade Launcher"},
            {1119849093, "Minigun"},
            {2138347493, "Firework Launcher"},
            {1834241177, "Railgun"},
            {1672152130, "Homing Launcher"},
            {125959754, "Compact Grenade Launcher"},
            {3056410471, "Ray Minigun"},
            {2481070269, "Grenade"},
            {2694266206, "BZ Gas"},
            {4256991824, "Smoke Grenade"},
            {1233104067, "Flare"},
            {615608432, "Molotov koktél"},
            {741814745, "Sticky Bomb"},
            {2874559379, "Proximity Mine"},
            {126349499, "Snowball"},
            {3125143736, "Pipe Bomb"},
            {600439132, "Baseball"},
            {883325847, "Jerry Can"},
            {101631238, "Tűzoltókészülék"},
            {4222310262, "Ejtőernyő"},
            {2461879995, "Elektromos Kerítés"},
            {3425972830, "Vízágyú"},
            {133987706, "Nekiment egy jármű"},
            {2741846334, "Elütötte egy jármű"},
            {3452007600, "Esés"},
            {4194021054, "Állat"},
            {324506233, "Airstrike Rocket"},
            {2339582971, "Vérzés"},
            {2294779575, "Aktatáska"},
            {28811031, "Aktatáska 02"},
            {148160082, "Cougar"},
            {1223143800, "Barbed Wire"},
            {4284007675, "Fulladás"},
            {1936677264, "Megfulladt egy járműben"},
            {539292904, "Robbanás"},
            {910830060, "Kifáradás"},
            {3750660587, "Tűz"},
            {341774354, "Helikopter Baleset"},
            {3204302209, "Vehicle Rocket"},
            {2282558706, "Vehicle Akula Barrage"},
            {431576697, "Vehicle Akula Minigun"},
            {2092838988, "Vehicle Akula Missile"},
            {476907586, "Vehicle Akula Turret Dual"},
            {3048454573, "Vehicle Akula Turret Single"},
            {328167896, "Vehicle APC Cannon"},
            {190244068, "Vehicle APC MG"},
            {1151689097, "Vehicle APC Missile"},
            {3293463361, "Vehicle Ardent MG"},
            {2556895291, "Vehicle Avenger Cannon"},
            {2756453005, "Vehicle Barrage Rear GL"},
            {1200179045, "Vehicle Barrage Rear MG"},
            {525623141, "Vehicle Barrage Rear Minigun"},
            {4148791700, "Vehicle Barrage Top MG"},
            {1000258817, "Vehicle Barrage Top Minigun"},
            {3628350041, "Vehicle Bombushka Cannon"},
            {741027160, "Vehicle Bombushka Dual MG"},
            {3959029566, "Vehicle Cannon Blazer"},
            {1817275304, "Vehicle Caracara MG"},
            {1338760315, "Vehicle Caracara Minigun"},
            {2722615358, "Vehicle Cherno Missile"},
            {3936892403, "Vehicle Comet MG"},
            {2600428406, "Vehicle Deluxo MG"},
            {3036244276, "Vehicle Deluxo Missile"},
            {1595421922, "Vehicle Dogfighter MG"},
            {3393648765, "Vehicle Dogfighter Missile"},
            {2700898573, "Vehicle Dune Grenade Launcher"},
            {3507816399, "Vehicle Dune MG"},
            {1416047217, "Vehicle Dune Minigun"},
            {1566990507, "Vehicle Enemy Laser"},
            {1987049393, "Vehicle Hacker Missile"},
            {2011877270, "Vehicle Hacker Missile Homing"},
            {1331922171, "Vehicle Halftrack Dual MG"},
            {1226518132, "Vehicle Halftrack Quad MG"},
            {855547631, "Vehicle Havok Minigun"},
            {785467445, "Vehicle Hunter Barrage"},
            {704686874, "Vehicle Hunter Cannon"},
            {1119518887, "Vehicle Hunter MG"},
            {153396725, "Vehicle Hunter Missile"},
            {2861067768, "Vehicle Insurgent Minigun"},
            {507170720, "Vehicle Khanjali Cannon"},
            {2206953837, "Vehicle Khanjali Cannon Heavy"},
            {394659298, "Vehicle Khanjali GL"},
            {711953949, "Vehicle Khanjali MG"},
            {3754621092, "Vehicle Menacer MG"},
            {3303022956, "Vehicle Microlight MG"},
            {3846072740, "Vehicle Mobileops Cannon"},
            {3857952303, "Vehicle Mogul Dual Nose"},
            {3123149825, "Vehicle Mogul Dual Turret"},
            {4128808778, "Vehicle Mogul Nose"},
            {3808236382, "Vehicle Mogul Turret"},
            {2220197671, "Vehicle Mule4 MG"},
            {1198717003, "Vehicle Mule4 Missile"},
            {3708963429, "Vehicle Mule4 Turret GL"},
            {2786772340, "Vehicle Nightshark MG"},
            {1097917585, "Vehicle Nose Turret Valkyrie"},
            {3643944669, "Vehicle Oppressor MG"},
            {2344076862, "Vehicle Oppressor Missile"},
            {3595383913, "Vehicle Oppressor2 Cannon"},
            {3796180438, "Vehicle Oppressor2 MG"},
            {1966766321, "Vehicle Oppressor2 Missile"},
            {3473446624, "Vehicle Plane Rocket"},
            {1186503822, "Vehicle Player Buzzard"},
            {3800181289, "Vehicle Player Lazer"},
            {1638077257, "Vehicle Player Savage"},
            {2456521956, "Vehicle Pounder2 Barrage"},
            {2467888918, "Vehicle Pounder2 GL"},
            {2263283790, "Vehicle Pounder2 Mini"},
            {162065050, "Vehicle Pounder2 Missile"},
            {3530961278, "Vehicle Radar"},
            {3177079402, "Vehicle Revolter MG"},
            {3878337474, "Vehicle Rogue Cannon"},
            {158495693, "Vehicle Rogue MG"},
            {1820910717, "Vehicle Rogue Missile"},
            {50118905, "Vehicle Ruiner Bullet"},
            {84788907, "Vehicle Ruiner Rocket"},
            {3946965070, "Vehicle Savestra MG"},
            {231629074, "Vehicle Scramjet MG"},
            {3169388763, "Vehicle Scramjet Missile"},
            {1371067624, "Vehicle Seabreeze MG"},
            {3450622333, "Vehicle Searchlight"},
            {4171469727, "Vehicle Space Rocket"},
            {3355244860, "Vehicle Speedo4 MG"},
            {3595964737, "Vehicle Speedo4 Turret MG"},
            {2667462330, "Vehicle Speedo4 Turret Mini"},
            {968648323, "Vehicle Strikeforce Barrage"},
            {955522731, "Vehicle Strikeforce Cannon"},
            {519052682, "Vehicle Strikeforce Missile"},
            {1176362416, "Vehicle Subcar MG"},
            {3565779982, "Vehicle Subcar Missile"},
            {3884172218, "Vehicle Subcar Torpedo"},
            {1744687076, "Vehicle Tampa Dual Minigun"},
            {3670375085, "Vehicle Tampa Fixed Minigun"},
            {2656583842, "Vehicle Tampa Missile"},
            {1015268368, "Vehicle Tampa Mortar"},
            {1945616459, "Vehicle Tank"},
            {3683206664, "Vehicle Technical Minigun"},
            {1697521053, "Vehicle Thruster MG"},
            {1177935125, "Vehicle Thruster Missile"},
            {2156678476, "Vehicle Trailer Dualaa"},
            {341154295, "Vehicle Trailer Missile"},
            {1192341548, "Vehicle Trailer Quad MG"},
            {2966510603, "Vehicle Tula Dual MG"},
            {1217122433, "Vehicle Tula MG"},
            {376489128, "Vehicle Tula Minigun"},
            {1100844565, "Vehicle Tula Nose MG"},
            {3041872152, "Vehicle Turret Boxville"},
            {1155224728, "Vehicle Turret Insurgent"},
            {729375873, "Vehicle Turret Limo"},
            {2144528907, "Vehicle Turret Technical"},
            {2756787765, "Vehicle Turret Valkyrie"},
            {4094131943, "Vehicle Vigilante MG"},
            {1347266149, "Vehicle Vigilante Missile"},
            {2275421702, "Vehicle Viseris MG"},
            {1150790720, "Vehicle Volatol Dual MG"},
            {1741783703, "Jármű Vízágyú"}
        };
        }

        public static string GetWeaponName(uint weaponId)
        {
            if (weaponDictionary.ContainsKey(weaponId))
            {
                return weaponDictionary[weaponId];
            }
            else
            {
                return "Ismeretlen";
            }
        }
    }

    public class Main : Script
    {
        //PLAYER -811.6, 175.1, 76.7, 110
        //CAM -814 174.1 76.7, -70

        [ServerEvent(Event.ResourceStart)]
        public void Start()
        {
            NAPI.Server.SetCommandErrorMessage("!{#ffffff}[!{##a83232}Hiba!{#ffffff}] Parancs nem található!");
            NAPI.Server.SetGlobalServerChat(false);
            NAPI.Server.SetAutoSpawnOnConnect(false);
            NAPI.Server.SetAutoRespawnAfterDeath(false);
            NAPI.World.SetWeather(Weather.EXTRASUNNY);
            //NAPI.Server.SetGlobalServerChat(true);
            //AutosavePlayers();
            NAPI.Task.Run(() =>
            {
                Admin.Levels.LoadAcmds();
                Inventory.ItemList.InitiateItemList();
                Inventory.Items.PopulateGroundItems();
                Characters.Clothing.LoadClothingShops();
                Interior.Properties.InitiateInteriors();
                Auth.Auth.DeleteTokens();

                
            
            },10000);
            //SetServerTime();
        }



        [ServerEvent(Event.VehicleDamage)]
        public void VehicleDamage(Vehicle v, float bodyhp, float enginehp)
        {
            Database.Log.Log_Server(v.DisplayName + " HP: " + NAPI.Vehicle.GetVehicleHealth(v.Handle) + " ;Body: " + NAPI.Vehicle.GetVehicleBodyHealth(v.Handle) + " loss: " + bodyhp + " ;Engine: " + NAPI.Vehicle.GetVehicleEngineHealth(v.Handle)  + " loss: "+ enginehp);
        }


        [ServerEvent(Event.PlayerDamage)]
        public void PlayerDamage(Player v, float hploss, float armorloss)
        {
            NAPI.Chat.SendChatMessageToAll(v.Name + " sebződés: " + hploss);
        }

        [Command("vehdamage")]
        public void VehDamage(Player player, int hp, float engine, float body)
        {
            player.Vehicle.SetSharedData("vehicle:Health", hp);
            player.Vehicle.SetSharedData("vehicle:EngineHealth", engine);
            player.Vehicle.SetSharedData("vehicle:BodyHealth", body);
        }
        List<GTANetworkAPI.Object> objects = new List<GTANetworkAPI.Object>();

        [Command("objecttest")]
        public void ObjectTest(Player player, string objectname)
        {
            for (int i = -5; i <= 5; i++)
            {
                for (int j = -5; j <= 5; j++)
                {
                    Vector3 placement = new Vector3(player.Position.X + i * 1f, player.Position.Y + j * 1f, player.Position.Z);
                    GTANetworkAPI.Object obj = NAPI.Object.CreateObject(NAPI.Util.GetHashKey(objectname), placement,new Vector3(0f,0f,0f), 255, player.Dimension);
                    
                    objects.Add(obj);
                }
            }
            player.SendChatMessage("Objectszám: " + objects.Count);
            player.TriggerEvent("client:StreamServerObjects");
        }


        [Command("wiregame")]
        public void WireMinigame(Player player)
        {
            player.TriggerEvent("client:ShowWireGame");
        }



        [Command("getvehiclehp")]
        public void showHP(Player player)
        {
            player.SendChatMessage("ENGINE: " + NAPI.Vehicle.GetVehicleEngineHealth(player.Vehicle) + " ; BODY: " + NAPI.Vehicle.GetVehicleBodyHealth(player.Vehicle));
        }

        [Command("deformveh")]
        public void DeformVeh(Player player, float offsetX, float offsetY, float offsetZ, float damage, float radius , bool focusonmodel)
        {
            player.TriggerEvent("client:DamageVehicle", offsetX, offsetY, offsetZ, damage, radius, focusonmodel);
        }



        public async void SetServerTime()
        {
            NAPI.Task.Run(() =>
            {
                DateTime time = DateTime.Now;
                NAPI.World.SetTime(Convert.ToInt16(time.Hour), Convert.ToInt16(time.Minute), Convert.ToInt16(time.Second));
                SetServerTime();
            }, 60000);
        }


        public void AutosavePlayers()
        {
            List<Player> players = NAPI.Pools.GetAllPlayers();
            NAPI.Chat.SendChatMessageToAll("AUTOSAVE: " + players.Count + " játékos.");
            foreach (var item in players)
            {
                if (item.HasData("Player:CharID"))
                {
                    Vector3 pos = item.Position;
                    Vector3 rot = item.Rotation;
                    uint charid = item.GetData<uint>("Player:CharID");
                    SavePlayerPos(charid, pos.X, pos.Y, pos.Z, rot.Z);
                }
            }
            NAPI.Task.Run(() =>
            {
                AutosavePlayers();
            }, 30000);
        }

        public async void SavePlayerPos(uint charid, float posX, float posY, float posZ, float rot)
        {
            await SavePlayerPosition(charid, posX, posY, posZ, rot);
        }

        [ServerEvent(Event.PlayerDisconnected)]
        public async void OnPlayerDisconnect(Player player, DisconnectionType type, string reason)
        {
            Vector3 pos = player.Position;
            Vector3 rot = player.Rotation;
            if (player.HasData("Player:CharID"))
            {
                uint charid = player.GetData<uint>("Player:CharID");

                if (player.HasData("AdminJail:Remaining") == false)//nem ül adminjailben, tehát menthetjük a poziját
                {
                    if (!await SavePlayerPosition(charid, pos.X, pos.Y, pos.Z, rot.Z))
                    {
                        Database.Log.Log_Server(player.Name + " nem lett mentve!");
                    }
                }

            }
        }

        public static async Task<bool> SavePlayerPosition(uint charid, float posX, float posY, float posZ, float rot)
        {

            string query = $"UPDATE `characters` SET `posX` = @PositionX, `posY` = @PositionY, `posZ` = @PositionZ, `rot` = @Rotation WHERE `characters`.`id` = @CharacterID;";
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = await Database.DBCon.GetConString();
                await con.OpenAsync();

                using (MySqlCommand command = new MySqlCommand(query, con))
                {
                    command.Parameters.AddWithValue("@PositionX", posX);
                    command.Parameters.AddWithValue("@PositionY", posY);
                    command.Parameters.AddWithValue("@PositionZ", posZ);
                    command.Parameters.AddWithValue("@Rotation", rot);
                    command.Parameters.AddWithValue("@CharacterID", charid);
                    command.Prepare();
                    try
                    {
                        int rows = await command.ExecuteNonQueryAsync();
                        if (rows > 0)
                        {
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Database.Log.Log_Server(ex.ToString());
                    }
                }
            }
            return false;
        }

        [ServerEvent(Event.PlayerSpawn)]
        public void PlayerSpawned(Player player)
        {
            //player.TriggerEvent("CreateAuthForms");
            //player.TriggerEvent("SetLoginCamera", -426.0f, 1117.0f, 350.0f, 0.0f, 0.0f, -163.0f, 70.0f);
        }

        [RemoteEvent("server:Fly")]
        public void Fly(Player player, int x, int y, int z)
        {
            GTANetworkAPI.Vector3 v = new GTANetworkAPI.Vector3(x, y, z);
            player.Position = v;
         
        }

        [RemoteEvent("server:Log")]
        public void ClientToLog(Player player, string text)
        {
            Database.Log.Log_Server(player.Name + " kliens log: " + text);
        }

        [ServerEvent(Event.PlayerDeath)]
        public void PlayerDeath(Player player, Player killer, uint reason)
        {
            if (killer != null)
            {
                Database.Log.Log_Server(killer.Name + " megölte "+ player.Name + " játékost. Indok: " + WeaponDatabase.GetWeaponName(reason));
            }
            else
            {
                Database.Log.Log_Server(player.Name + " meghalt. Indok: " + WeaponDatabase.GetWeaponName(reason));
            }

        }
    }
}