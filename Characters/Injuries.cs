using MySql.Data.MySqlClient;
using Server.Inventory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GTANetworkAPI;
using System.Linq;
using Org.BouncyCastle.Asn1.X509;
using MySqlX.XDevAPI.Relational;

namespace Server.Characters
{
    public class BoneTable
    {
        private static Dictionary<uint, string> boneNames = new Dictionary<uint, string>()
    {
        {65068, "Arc"},
        {37193, "Szemöldök Középső"},
        {46240, "Állkapocs"},
        {58331, "Bal Szemöldök Külső"},
        {21550, "Bal Arccsont"},
        {25260, "Bal Szem"},
        {45750, "Bal Felső Szemhéj"},
        {47419, "Bal Alsó Ajak"},
        {29868, "Bal Ajak Sarok"},
        {20279, "Bal Felső Ajak"},
        {20623, "Alsó Ajak"},
        {17188, "Alsó Ajak Gyökere"},
        {1356, "Jobb Szemöldök Külső"},
        {19336, "Jobb Arccsont"},
        {27474, "Jobb Szem"},
        {43536, "Jobb Felső Szeéj"},
        {49979, "Jobb Alsó Ajak"},
        {11174, "Jobb Ajak Sarok"},
        {17719, "Jobb Felső Ajak"},
        {47495, "Nyelv"},
        {61839, "Felső Ajak"},
        {20178, "Felső Ajak Gyökere"},
        {12844, "Fej"},
        {65245, "Bal Láb"},
        {36029, "Bal Kéz"},
        {35502, "Jobb Láb"},
        {6286, "Jobb Kéz"},
        {56604, "Gyökér"},
        {22711, "Bal Könyök"},
        {46078, "Bal Térd"},
        {2992, "Jobb Könyök"},
        {16335, "Jobb Térd"},
        {57717, "Bal Láb"},
        {60309, "Bal Kéz"},
        {24806, "Jobb Láb"},
        {28422, "Jobb Kéz"},
        {5232, "Bal Kar Forgás"},
        {61007, "Bal Alkar Forgás"},
        {23639, "Bal Comb Forgás"},
        {35731, "Nyak"},
        {37119, "Jobb Kar Forgás"},
        {43810, "Jobb Alkar Forgás"},
        {6442, "Jobb Comb Forgás"},
        {31086, "Fej"},
        {63931, "Bal Vádli"},
        {64729, "Bal Kulcscsont"},
        {26610, "Bal Ujj00"},
        {4089, "Bal Ujj01"},
        {4090, "Bal Ujj02"},
        {26611, "Bal Ujj10"},
        {4169, "Bal Ujj11"},
        {4170, "Bal Ujj12"},
        {26612, "Bal Ujj20"},
        {4185, "Bal Ujj21"},
        {4186, "Bal Ujj22"},
        {26613, "Bal Ujj30"},
        {4137, "Bal Ujj31"},
        {4138, "Bal Ujj32"},
        {26614, "Bal Ujj40"},
        {4153, "Bal Ujj41"},
        {4154, "Bal Ujj42"},
        {14201, "Bal Láb"},
        {61163, "Bal Alkar"},
        {18905, "Bal Kéz"},
        {58271, "Bal Comb"},
        {2108, "Bal Lábujj"},
        {45509, "Bal Váll"},
        {39317, "Nyak"},
        {11816, "Medence"},
        {36864, "Jobb Vádli"},
        {10706, "Jobb Kulcscsont"},
        {58866, "Jobb Ujj00"},
        {64016, "Jobb Ujj01"},
        {64017, "Jobb Ujj02"},
        {58867, "Jobb Ujj10"},
        {64096, "Jobb Ujj11"},
        {64097, "Jobb Ujj12"},
        {58868, "Jobb Ujj20"},
        {64112, "Jobb Ujj21"},
        {64113, "Jobb Ujj22"},
        {58869, "Jobb Ujj30"},
        {64064, "Jobb Ujj31"},
        {64065, "Jobb Ujj32"},
        {58870, "Jobb Ujj40"},
        {64080, "Jobb Ujj41"},
        {64081, "Jobb Ujj42"},
        {52301, "Jobb Láb"},
        {28252, "Jobb Alkar"},
        {57005, "Jobb Kéz"},
        {51826, "Jobb Comb"},
        {20781, "Jobb Lábujj"},
        {40269, "Jobb Váll"},
        {0, "-"},
        {57597, "Gerinc"},
        {23553, "Gerinc"},
        {24816, "Gerinc Alsó"},
        {24817, "Gerinc Középső"},
        {24818, "Gerinc Felső"}
    };

        public static string GetBoneName(uint id)
        {
            if (boneNames.ContainsKey(id))
            {
                return boneNames[id];
            }
            else
            {
                return "Csont nem található: " + id;
            }
        }
    }

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
            {4256991824, "Füstgránát"},
            {1233104067, "Flare"},
            {615608432, "Molotov koktél"},
            {741814745, "Sticky Bomb"},
            {2874559379, "Proximity Mine"},
            {126349499, "Hógolyó"},
            {3125143736, "Pipe Bomb"},
            {600439132, "Baseball ütő"},
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

    class Injury
    {
        public uint ID { get; set; }
        public uint CharacterID { get; set; }
        public string CausedBy { get; set; }//karakter név
        public int Damage { get; set; }
        public string DamageType { get; set; }
        public uint Bone { get; set; }
        public DateTime TimeStamp { get; set; }
        public bool Active { get; set; }
        public Injury(uint id, uint charid, string causedby, int dmg, string dmgtype, uint bone, DateTime timestamp, bool active) {
            ID = id;
            CharacterID = charid;
            CausedBy = causedby;
            Damage = dmg;
            DamageType = dmgtype;
            Bone = bone;
            TimeStamp = timestamp;
            Active = active;
        }

    }


    internal class Injuries : Script
    {
        static Dictionary<Player, List<Injury>> Serulesek = new Dictionary<Player, List<Injury>>();

        [ServerEvent(Event.PlayerDeath)]
        public void PlayerDeath(Player player, Player killer, uint reason)
        {
            if (player.HasData("Player:InAnim"))//animban van
            {
                StopPlayerAnim(player);
                NAPI.Task.Run(() =>
                {
                    string inj = "";
                    Vector3 pos = player.Position;
                    Vector3 rot = player.Rotation;
                    foreach (var item in Characters.Injuries.GetPlayerInjuries(player))
                    {
                        if (item.CausedBy != "-")
                        {
                            inj += item.DamageType + " | " + item.Damage + " | Okozta: " + item.CausedBy + " | " + BoneTable.GetBoneName(item.Bone) + " | " + item.TimeStamp + "\n";
                        }
                        else
                        {
                            inj += item.DamageType + " | " + item.Damage + " | " + BoneTable.GetBoneName(item.Bone) + " | " + item.TimeStamp + "\n";
                        }
                    }

                    bool animType = player.GetData<bool>("Player:InAnim");
                    player.ResetData("Player:InAnim");

                    if (killer != null)
                    {
                        Database.Log.Log_Server(killer.Name + " megölte " + player.Name + " játékost animból. Indok: " + Characters.WeaponDatabase.GetWeaponName(reason) + "\nSérülései:\n" + inj);
                    }
                    else
                    {
                        Database.Log.Log_Server(player.Name + " meghalt animból. Indok: " + Characters.WeaponDatabase.GetWeaponName(reason) + "\nSérülései:\n" + inj);
                    }

                    player.SetSharedData("Player:Frozen", true);
                }, 3000);
            }
            else//nincs animban
            {            
                //várunk 5 másodpercet hogy megálljon a karakter
                NAPI.Task.Run(() =>
                {
                    string inj = "";
                    Vector3 pos = player.Position;
                    Vector3 rot = player.Rotation;

                    foreach (var item in Characters.Injuries.GetPlayerInjuries(player))
                    {
                        if (item.CausedBy != "-")
                        {
                            inj += item.DamageType + " | " + item.Damage + " | Okozta: " + item.CausedBy + " | " + BoneTable.GetBoneName(item.Bone) + " | " + item.TimeStamp + "\n";
                        }
                        else
                        {
                            inj += item.DamageType + " | " + item.Damage + " | " + BoneTable.GetBoneName(item.Bone) + " | " + item.TimeStamp + "\n";
                        }
                    }


                    if (killer != null)
                    {
                        Database.Log.Log_Server(killer.Name + " animba kényszerítette " + player.Name + " játékost. Indok: " + Characters.WeaponDatabase.GetWeaponName(reason) + "\nSérülései:\n" + inj);
                    }
                    else
                    {

                        Database.Log.Log_Server(player.Name + " animba esett. Indok: " + Characters.WeaponDatabase.GetWeaponName(reason) + "\nSérülései:\n" + inj);
                    }

                    CheckForAnim(player);
                    

                    //beállítjuk a hp-ját 20-ra, megnézzük a sérüléseit és a megfelelő animba helyezzük őt

                    //Characters.Injuries.HealPlayerInjuries(player);
                }, 3000);

            }



        }

        public bool HeadShot(Player player)
        {
            if (GetPlayerInjuries(player).Count > 0)
            {
                string bone = BoneTable.GetBoneName(GetPlayerInjuries(player).Last().Bone);
                if (bone == "Fej" || bone == "Arc")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }

        public bool IsDamageTypeSerious(string dmgtype)
        {
            if (dmgtype == "Ütés" || dmgtype == "Esés" || dmgtype == "Tárgy" || dmgtype == "Autóbaleset" || dmgtype == "Elütés")
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        public bool HasSeriousInjury(Player player)
        {
            bool state = false;
            foreach (var item in GetPlayerInjuries(player))
            {
                if (item.DamageType == "Ütés" || item.DamageType == "Esés" || item.DamageType == "Tárgy" || item.DamageType == "Autóbaleset" || item.DamageType == "Elütés")
                {
                    
                }
                else
                {
                    state = true;
                    continue;
                }
            }
            return state;
        }

        public void CheckForAnim(Player player)
        {
            if (HasSeriousInjury(player))
            {
                SetPlayerIntoAnim(player, 2);
            }
            else
            {
                SetPlayerIntoAnim(player, 1);
            }
        }

        public static void CheckPlayerDamage(Player player)
        {
            NAPI.Task.Run(() =>
            {
                player.SetSharedData("Player:Injured", player.Health);
            }, 500);
        }

        public static void SetPlayerIntoAnim(Player player, int animType)
        {
            NAPI.Player.SpawnPlayer(player, player.Position, player.Rotation.Z);
            NAPI.Player.SetPlayerHealth(player, 20);
            switch (animType)
            {
                case 0:
                    player.TriggerEvent("client:EnableInjuredCrawl");
                    player.SetData("Player:InAnim", true);
                    break;

                case 1:
                    player.TriggerEvent("client:EnableInjuredCrawl");
                    player.SetData("Player:InAnim", true);
                    break;

                case 2:
                    player.TriggerEvent("client:EnableAnim");
                    player.SetData("Player:InAnim", false);
                    break;
                default:
                    break;
            }

        }

        public static void StopPlayerAnim(Player player)
        {
            NAPI.Task.Run(() =>
            {
                player.TriggerEvent("client:DisableInjuredCrawl");
                player.TriggerEvent("client:DisableAnim");
            });
        }


        public static async void SetPlayerInjuries(Player player)
        {
            if (player.HasData("Player:CharID"))
            {
                uint charid = player.GetData<uint>("Player:CharID");
                Injury[] PlayerInjuries = await LoadPlayerInjuries(charid);
                Serulesek[player] = PlayerInjuries.ToList();
            }
        }

        public static List<Injury> GetPlayerInjuries(Player player)
        {
            if(Serulesek.ContainsKey(player))
            {
                return Serulesek[player];
            }
            else
            {
                return new List<Injury>();
            }
        }

        [RemoteEvent("server:PlayerDamagedByVehicle")]
        public void PlayerDamagedByVehicle(Player player, int damage, uint bone)
        {
            string dmgtype = "Elütés";

            AddPlayerInjury(player, "-", damage, bone, dmgtype);
        }

        [RemoteEvent("server:PlayerDamaged")]
        public void PlayerDamaged(Player player,  int damage, uint bone)
        {
            string dmgtype = "Esés";

            AddPlayerInjury(player, "-",damage, bone, dmgtype);
        }

        [RemoteEvent("server:PlayerDamagedByObject")]
        public void PlayerDamagedByObject(Player player, int damage, uint bone)
        {
            string dmgtype = "Tárgy";

            AddPlayerInjury(player, "-", damage, bone, dmgtype);
        }

        [RemoteEvent("server:PlayerHitPlayer")]
        public void PlayerHitPlayer(Player player, int targetid, int damage, uint bone)
        {
            Player target = Admin.Commands.GetPlayerById(targetid);
            WeaponHash weapon = NAPI.Player.GetPlayerCurrentWeapon(target);
            string dmgtype = "Ütés";
            if (weapon.ToString() != WeaponHash.Unarmed.ToString())
            {
                dmgtype = weapon.ToString();
            }
            AddPlayerInjury(player, target.Name, damage, bone, dmgtype);
        }

        [RemoteEvent("server:PlayerCrashed")]
        public void PlayerCrashed(Player player, int damage, uint bone, float bodyhpdmg)
        {
            string dmgtype = "Autóbaleset";
            if (bodyhpdmg >= 50f)
            {
                PlayerAnimInVehicle(player);
            }
            AddPlayerInjury(player, "-", damage, bone, dmgtype);
        }

        [RemoteEvent("server:PlayerCrashedPlayer")]
        public void PlayerCrashedPlayer(Player player, int targetid, int damage, uint bone, float bodyhpdmg)
        {
            Player target = Admin.Commands.GetPlayerById(targetid);
            string dmgtype = "Autóbaleset";
            if (bodyhpdmg >= 50f)
            {
                PlayerAnimInVehicle(player);
            }
            AddPlayerInjury(player, target.Name, damage, bone, dmgtype);
        }

        [RemoteEvent("server:PlayerRammed")]
        public void PlayerRammed(Player player,  int damage, uint bone)
        {
            string dmgtype = "Elütés";
            AddPlayerInjury(player,"-", damage, bone, dmgtype);
        }


        public void PlayerAnimInVehicle(Player player)
        {
            player.SendChatMessage("Animba estél!");
            NAPI.Player.SetPlayerHealth(player, 20);
            player.TriggerEvent("client:EnableAnim");
            player.SetData("Player:InAnim", false);
        }


        public async void AddPlayerInjury(Player player, string causedby, int damage, uint bone, string dmgtype)
        {
            uint charid = player.GetData<uint>("Player:CharID");
            Injury inj = await AddInjuryToDatabase(charid, causedby, damage, bone, dmgtype);
            if (inj != null)
            {
                if (Serulesek.ContainsKey(player))
                {
                    Serulesek[player].Add(inj);
                    CheckPlayerDamage(player);
                }
                else
                {
                    Serulesek.Add(player, new List<Injury> { inj });
                    CheckPlayerDamage(player);
                }
            }
            else
            {
                Database.Log.Log_Server("HIBA! Sérülés nem lett hozzáadva az adatbázishoz. [" + causedby + " - " + dmgtype + " - " + damage + "]");
            }
        }

        public static async void HealPlayerInjuries(Player player)
        {
            if (player.HasData("Player:CharID"))
            {
                uint charid = player.GetData<uint>("Player:CharID");
                if (await HealInjuries(charid))
                {
                    Serulesek[player] = new List<Injury>();
                    NAPI.Task.Run(() =>
                    {
                        StopPlayerAnim(player);
                    });
                }
            }

        }

        [Command("injuries")]
        public void GetInj(Player player)
        {
            player.SendChatMessage("Sérüléseid:");
            foreach (var item in GetPlayerInjuries(player))
            {
                player.SendChatMessage(item.DamageType + " | DMG: " + item.Damage + " | " + BoneTable.GetBoneName(item.Bone) + " | "  + item.TimeStamp);
            }
        }

        public async static Task<bool> HealInjuries(uint charid)
        {
            bool state = false;
            string query = $"UPDATE `injuries` SET `active` = false WHERE `injuries`.`characterID` = @CharID;";
            //string query2 = $"UPDATE `characters` SET `characterName` = @CharacterName, `dob` = @DOB, `pob` = @POB WHERE `appearanceId` = @AppearanceID";
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = await Database.DBCon.GetConString();
                await con.OpenAsync();

                using (MySqlCommand command = new MySqlCommand(query, con))
                {
                    command.Parameters.AddWithValue("@CharID", charid);
                    command.Prepare();
                    try
                    {
                        int rows = await command.ExecuteNonQueryAsync();
                        if (rows > 0)
                        {
                            state = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Database.Log.Log_Server(ex.ToString());
                    }
                }
                con.CloseAsync();
            }

            return state;
        }

        public async static Task<bool> HealInjury(Injury inj)
        {
            bool state = false;
            string query = $"UPDATE `injuries` SET `active` = false WHERE `injuries`.`id` = @InjuryID;";
            //string query2 = $"UPDATE `characters` SET `characterName` = @CharacterName, `dob` = @DOB, `pob` = @POB WHERE `appearanceId` = @AppearanceID";
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = await Database.DBCon.GetConString();
                await con.OpenAsync();

                using (MySqlCommand command = new MySqlCommand(query, con))
                {
                    command.Parameters.AddWithValue("@InjuryID", inj.ID);
                    command.Prepare();
                    try
                    {
                        int rows = await command.ExecuteNonQueryAsync();
                        if (rows > 0)
                        {
                            state = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Database.Log.Log_Server(ex.ToString());
                    }
                }
                con.CloseAsync();
            }

            return state;
        }

        public static async Task<Injury[]> LoadPlayerInjuries(uint charid)
        {
            string query = $"SELECT * FROM `injuries` WHERE `characterID` = @CharID AND `active` = 1";
            List<Injury> inj = new List<Injury>();
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = await Database.DBCon.GetConString();
                try
                {
                    await con.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@CharID", charid);
                        cmd.Prepare();
                        try
                        {
                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    Injury i = new Injury(Convert.ToUInt32(reader["id"]), Convert.ToUInt32(reader["characterID"]), Convert.ToString(reader["causedBy"]), Convert.ToInt32(reader["damage"]), Convert.ToString(reader["damageType"]), Convert.ToUInt32(reader["bone"]), Convert.ToDateTime(reader["timestamp"]), Convert.ToBoolean(reader["active"]));
                                    inj.Add(i);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Database.Log.Log_Server(ex.ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    Database.Log.Log_Server(ex.ToString());
                }
                con.CloseAsync();
                return inj.ToArray();
            }
        }

        public async static Task<Injury> AddInjuryToDatabase(uint charid, string causedby, int damage, uint bone, string dmgtype)//létrehozunk egy új itemet az adatbázisban
        {
            Injury inj = null;
            //INSERT INTO `appearances` (`id`, `gender`, `eyeColor`, `hairStyle`, `hairColor`, `hairHighlight`, `parent1face`, `parent2face`, `parent3face`, `parent1skin`, `parent2skin`, `parent3skin`, `faceMix`, `skinMix`, `thirdMix`, `noseWidth`, `noseHeight`, `noseLength`, `noseBridge`, `noseTip`, `noseBroken`, `browHeight`, `browWidth`, `cheekboneHeight`, `cheekboneWidth`, `cheekWidth`, `eyes`, `lips`, `jawWidth`, `jawHeight`, `chinLength`, `chinPosition`, `chinWidth`, `chinShape`, `neckWidth`, `blemishId`, `blemishOpacity`, `facialhairId`, `facialhairColor`, `facialhairOpacity`, `eyebrowId`, `eyebrowColor`, `eyebrowOpacity`, `ageId`, `ageOpacity`, `makeupId`, `makeupOpacity`, `blushId`, `blushColor`, `blushOpacity`, `complexionId`, `complexionOpacity`, `sundamageId`, `sundamageOpacity`, `lipstickId`, `lipstickColor`, `lipstickOpacity`, `frecklesId`, `frecklesOpacity`, `chesthairId`, `chesthairColor`, `chesthairOpacity`, `bodyblemishId`, `bodyblemishOpacity`, `bodyblemish2Id`, `bodyblemish2Opacity`, `tattoos`) VALUES (NULL, '0', '', '0', '', '', '', '', '', '', '', '', '', '', '', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '255', '0', '255', '0', '0', '255', '0', '0', '255', '0', '255', '0', '255', '0', '0', '255', '0', '255', '0', '255', '0', '0', '255', '0', '255', '0', '0', '255', '0', '255', '0', NULL);
            //SELECT LAST_INSERT_ID();
            string query = $"INSERT INTO `injuries` " +
                $"(`characterID`, `causedBy`, `damage`, `damageType`, `bone`)" +
                $" VALUES " +
                $"(@CharID, @CausedBy, @Damage, @DmgType, @Bone)";

            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = await Database.DBCon.GetConString();
                await con.OpenAsync();

                using (MySqlCommand command = new MySqlCommand(query, con))
                {

                    command.Parameters.AddWithValue("@CharID", charid);
                    command.Parameters.AddWithValue("@CausedBy", causedby);
                    command.Parameters.AddWithValue("@Damage", damage);
                    command.Parameters.AddWithValue("@Bone", bone);
                    command.Parameters.AddWithValue("@DmgType", dmgtype);

                    command.Prepare();
                    try
                    {
                        int rows = await command.ExecuteNonQueryAsync();
                        if (rows > 0)
                        {
                            long lastid = command.LastInsertedId;
                            inj = new Injury(Convert.ToUInt32(lastid), charid, causedby, damage, dmgtype, bone, DateTime.Now, true);
                        }
                    }
                    catch (Exception ex)
                    {
                        Database.Log.Log_Server(ex.ToString());
                    }

                }
                con.CloseAsync();
            }
            return inj;
        }
    }
}
