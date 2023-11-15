using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using GTANetworkAPI;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Utilities;
using Server.Characters;
using Server.Inventory;

namespace Server.Vehicles
{
    public class Jarmu
    {
        public uint ID { get; set; }
        public uint OwnerType { get; set; }
        public uint OwnerID { get; set; }
        public string Model { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public byte Red1 { get; set; }
        public byte Green1 { get; set; }
        public byte Blue1 { get; set; }
        public byte Red2 { get; set; }
        public byte Green2 { get; set; }
        public byte Blue2 { get; set; }
        public byte Pearlescent { get; set; }
        public bool Locked { get; set; }
        public bool Engine { get; set; }
        public string NumberPlateText { get; set; }
        public byte NumberPlateType { get; set; }
        public uint Dimension { get; set; }

        public Jarmu(uint id, uint ownertype, uint ownerid, string model, Vector3 pos, Vector3 rot, byte r1, byte g1, byte b1, byte r2, byte g2, byte b2, byte pearl, bool locked, bool engine, string numberplate, byte numberplatetype, uint dim)
        {
            ID = id;
            OwnerType = ownertype;
            OwnerID = ownerid;
            Model = model;
            Position = pos;
            Rotation = rot;
            Red1 = r1;
            Green1 = g1;
            Blue1 = b1;
            Red2 = r2;
            Green2 = g2;
            Blue2 = b2;
            Pearlescent = pearl;
            Locked = locked;
            Engine = engine;
            NumberPlateText = numberplate;
            NumberPlateType = numberplatetype;
            Dimension = dim;
        }
    }


    internal class Vehicles : Script
    {
        static Dictionary<int,Vehicle> vehicles = new Dictionary<int,Vehicle>();
        static Dictionary<Player, Vehicle> dealership = new Dictionary<Player, Vehicle>();
        int tempIndex = -1;
        ColShape dealer;

        public Vehicles()
        {
            CheckVehiclesToDespawn();
            
        }
        //bejáratok:

        //-38.2 -1108 26.5
        //-33 -1093.5 26.5
        //CAM: -813.95, 174.2, 76.78, 0, 0, -69
        [ServerEvent(Event.ResourceStart)]
        public void InitiateLoading()
        {
            //dealer = NAPI.ColShape.Create2DColShape(-52.5f, -1102.5f, 25.5f, 15f);
            dealer = NAPI.ColShape.CreateSphereColShape(new Vector3(-44.2f, -1098f, 26.5f), 8f);
            
            //dealer = NAPI.ColShape.Create3DColShape(new Vector3(-35f, -1108f, 20f), new Vector3(-51f, -1089f, 31f),0);
        }

        [Command("tempveh", Alias = "tempcar")]
        public void TemporaryVehicle(Player player, string model)
        {
            uint vHash = NAPI.Util.GetHashKey(model);
            Vehicle v = NAPI.Vehicle.CreateVehicle(vHash, new Vector3(player.Position.X, player.Position.Y + 2.0, player.Position.Z), 0f, 1, 1, "TEMP");
            v.Dimension = player.Dimension;
            player.SendChatMessage("Ideiglenes jármű létrehozva: " + model.ToLower() + " ("+tempIndex+")");
            
            vehicles[tempIndex] = v;
            tempIndex--;
        }

        [Command("rev")]
        public void RevVehicle(Player player, float revs)
        {
            player.TriggerEvent("client:rev", revs);
        
        }


        [Command("makeveh", Alias = "makevehicle")]
        public async void CreateVehicle(Player player, string model, string plate = "",byte red1 = 255, byte green1 = 255, byte blue1 = 255, byte red2 = 255, byte green2 = 255, byte blue2 = 255, byte pearlescent = 0)
        {
            uint vHash = NAPI.Util.GetHashKey(model);
            Vehicle v = NAPI.Vehicle.CreateVehicle(vHash, new Vector3(player.Position.X, player.Position.Y + 2.0, player.Position.Z), 0f, 1, 1, plate);
            v.Dimension = player.Dimension;
            uint charid = player.GetData<uint>("player:charID");
            NAPI.Vehicle.SetVehicleCustomPrimaryColor(v, red1, green1, blue1);
            NAPI.Vehicle.SetVehicleCustomSecondaryColor(v, red2, green2, blue2);
            NAPI.Vehicle.SetVehiclePearlescentColor(v, pearlescent);
            Jarmu j = new Jarmu(0, 0, charid, model, v.Position, v.Rotation ,red1, green1, blue1, red2, green2, blue2, pearlescent, false, false, plate, 0, 0);
            uint id = await AddVehicleToDatabase(j, player.Name);
            if (id != 0)
            {
                NAPI.Task.Run(() =>
                {
                    v.SetData("vehicle:ID", id);
                    vehicles[Convert.ToInt32(id)] = v;
                    player.SendChatMessage("Jármű létrehozva: " + model.ToLower() + " (" + id + ")");
                    Inventory.Items.GiveItem(player, player.Id, 15, id.ToString(), 1);
                }, 250);

            }
            else
            {
                NAPI.Task.Run(() =>
                {
                    player.SendChatMessage("Nem sikerült az adatbázisba menteni a járművet!");
                    v.Delete();
                }, 250);
            }
        }


        [Command("getveh", Alias ="getcar")]
        public void GetVehicle(Player player, int id)
        {
            if (vehicles.ContainsKey(id))//létezik a jármű
            {
                Vector3 offset = new Vector3(2f, 2f, 0f);
                vehicles[id].Position = player.Position+offset;
                vehicles[id].Rotation = player.Rotation;
                player.SendChatMessage("Jármű get: " + id);
            }
            else
            {
                player.SendChatMessage("Jármű nem létezik");
            }
        }



        

        [ServerEvent(Event.PlayerEnterColshape)]
        public void EnterCheckpoint(ColShape cp, Player player)
        {
            player.SendChatMessage("beleléptél a colshapeba");
            if (cp == dealer && player.Position.Z > 20f && player.Position.Z < 31f)//belépet a dealership-be
            {
                player.Dimension = Convert.ToUInt32(player.Id + 1);
                player.SendChatMessage("Beléptél az autókereskedésbe, dimenziód átállítva.");
                uint vHash = NAPI.Util.GetHashKey("blista");
                Vehicle preview = NAPI.Vehicle.CreateVehicle(vHash, new Vector3(-44f, -1097.75f, 26f), 150f, 0, 0, "DEALER", 255, false, false, player.Dimension);
                dealership[player] = preview;
            }
        }

        [ServerEvent(Event.PlayerExitColshape)]
        public void ExitCheckpoint(ColShape cp, Player player)
        {
            if (cp == dealer)//kilépett a dealership-ből
            {
                player.Dimension = 0;
                player.SendChatMessage("Kiléptél az autókereskedésből, dimenziód visszaállítva.");
                if (dealership.ContainsKey(player))
                {
                    dealership[player].Delete();
                }
                
            }
        }

        /*
        [Command("dealership")]
        public void OpenDealership(Player player)
        {
            player.Dimension = Convert.ToUInt32(player.Id+1);
            
            Vector3 offset = new Vector3(1f, 1f, 0f);
            
            player.Position = v.Position + offset;
            player.SendChatMessage("Ideiglenes jármű létrehozva: " + "elegy" + " (" + tempIndex + ")");
            player.SetSharedData("player:Frozen", true);
            player.SetSharedData("player:Invisible", true);
            vehicles[tempIndex] = v;
            tempIndex--;
            NAPI.Task.Run(() =>
            {
                NAPI.Player.SetPlayerIntoVehicle(player, v, 0);
                NAPI.Task.Run(() =>
                {
                    player.TriggerEvent("client:DealershipCamera");
                }, 250);
            }, 750);
        }
        */

        [Command("flipveh", Alias = "flipcar")]
        public void FlipVehicle(Player player, int id = 0)
        {
            if (id == 0)//saját jármű
            {
                if (player.Vehicle != null)
                {
                    player.Vehicle.Rotation = new Vector3(0f, 0f, player.Vehicle.Rotation.Z);
                    player.SendChatMessage("Jármű felállítva!");
                }
                else
                {
                    player.SendChatMessage("/flipveh [jármű id]");
                }
            }
            else if (vehicles.ContainsKey(id))//létezik a jármű
            {
                vehicles[id].Rotation = new Vector3(0f, 0f, vehicles[id].Rotation.Z);
                player.SendChatMessage("Jármű felállítva!");
            }


        }

        [Command("gotoveh", Alias ="gotocar")]
        public void GotoVehicle(Player player, int id)
        {
            if (vehicles.ContainsKey(id))//létezik a jármű
            {
                Vector3 offset = new Vector3(2f, 2f, 0f);
                player.Position = vehicles[id].Position + offset;
                player.SendChatMessage("Jármű goto: " + id);
            }
            else
            {
                player.SendChatMessage("Jármű nem létezik");
            }
        }


        [Command("engine", Alias = "eng")]
        public void StartStopEngine(Player player)
        {
            if (player.Vehicle != null)//járműben ül
            {
                if(player.Vehicle.HasData("vehicle:ID"))//van id-je
                {
                    uint vehid = player.Vehicle.GetData<uint>("vehicle:ID");
                    if (Inventory.Items.HasItemWithValue(player, 15, vehid.ToString()))//van neki járműkulcs iteme (15) és az itemvalue az id-je
                    {
                        player.Vehicle.EngineStatus = !player.Vehicle.EngineStatus;
                    }
                    else
                    {
                        player.SendChatMessage("Nincs kulcsod a járműhöz!");
                    }
                }
                else
                {
                    player.SendChatMessage("Temp veh motorja beindítva/leálltva. (ADMIN)");
                    player.Vehicle.EngineStatus = !player.Vehicle.EngineStatus;
                }
            }
        }


        [Command("lock", Alias = "unlock")]
        public void LockUnlockVehicle(Player player)
        {
            
            if (player.Vehicle != null)//járműben ül
            {
                uint vehid = player.Vehicle.GetData<uint>("vehicle:ID");
                if (Inventory.Items.HasItemWithValue(player, 15, vehid.ToString()))//van neki járműkulcs iteme (15) és az itemvalue az id-je
                {
                    player.Vehicle.EngineStatus = !player.Vehicle.EngineStatus;
                }
                else
                {
                    player.SendChatMessage("Nincs kulcsod a járműhöz!");
                }


            }
        }


        [Command("fixveh")]
        public void FixVehicle(Player player)
        {
            if(player.Vehicle != null)
            {
                player.SendChatMessage("Body health: " + NAPI.Vehicle.GetVehicleBodyHealth(player.Vehicle));
                player.SendChatMessage("Engine health: " + NAPI.Vehicle.GetVehicleEngineHealth(player.Vehicle));
                NAPI.Vehicle.RepairVehicle(player.Vehicle);
                //NAPI.Vehicle.SetVehicleBodyHealth(player.Vehicle, 1000);
                player.SendChatMessage("Body health: " + NAPI.Vehicle.GetVehicleBodyHealth(player.Vehicle));
                player.SendChatMessage("Engine health: " + NAPI.Vehicle.GetVehicleEngineHealth(player.Vehicle));
            }
            else
            {
                player.SendChatMessage("Nem ülsz járműben.");
            }
        }

        [Command("setmod", Alias = "mod")]
        public void VehicleMod(Player player, int mod, int value)
        {
            if (player.Vehicle != null)
            {
                NAPI.Vehicle.SetVehicleMod(player.Vehicle, mod, value);
                player.SendChatMessage("Tuning beállítva!");
            }
            else
            {
                player.SendChatMessage("Nem ülsz járműben!");
            }
        }


        [Command("tuning", Alias = "tune")]
        public void TuneVehicle(Player player, float torque, float power, bool drift)
        {
            if (player.Vehicle != null)
            {
                player.TriggerEvent("client:SetHandling", torque, power, drift);
                player.SendChatMessage("Tuning beállítva!");
            }
            else
            {
                player.SendChatMessage("Nem ülsz járműben!");
            }
        }

        [Command("setvehcolor", Alias = "setcarcolor")]
        public void SetVehicleColor(GTANetworkAPI.Player player, byte r1, byte g1, byte b1, byte r2, byte g2, byte b2)
        {
            GTANetworkAPI.Vehicle v = player.Vehicle;
            if (v != null)
            {
                NAPI.Vehicle.SetVehicleMod(v, 0, 0);
                    NAPI.Vehicle.SetVehicleCustomPrimaryColor(v, r1, g1, b1);
                    NAPI.Vehicle.SetVehicleCustomSecondaryColor(v, r2, g2, b2);
                    player.SendChatMessage("Jármű átszínezve!");
                    return;
                }
                else
                {
                    player.SendChatMessage("Nem ülsz járműben!");
                    return;
                }
        }

        [Command("setvehextra", Alias = "setcarextra")]
        public void SetVehicleExtra(GTANetworkAPI.Player player, int slot, bool state)
        {
            GTANetworkAPI.Vehicle v = player.Vehicle;
            if (v != null)
            {

                NAPI.Vehicle.SetVehicleExtra(v, slot, state);
                player.SendChatMessage("Extra átállítva!");
                return;
            }
            else
            {
                player.SendChatMessage("Nem ülsz járműben!");
                return;
            }
        }


        [Command("setvehlivery", Alias = "setcarlivery")]
        public void SetVehicleLivery(GTANetworkAPI.Player player, int livery)
        {
                GTANetworkAPI.Vehicle v = player.Vehicle;
                if (v != null)
                {

                    NAPI.Vehicle.SetVehicleLivery(v, livery);
                    player.SendChatMessage("Paintjob átállítva!");
                    return;
                }
                else
                {
                    player.SendChatMessage("Nem ülsz járműben!");
                    return;
                }
        }

        /*
        public static async Task<uint> CreateNewVehicle()
        {

        }
        */

        public async static Task<uint> AddVehicleToDatabase(Jarmu j, string creator)//létrehozunk egy új itemet az adatbázisban
        {
            uint VehicleID = 0;
            //INSERT INTO `appearances` (`id`, `gender`, `eyeColor`, `hairStyle`, `hairColor`, `hairHighlight`, `parent1face`, `parent2face`, `parent3face`, `parent1skin`, `parent2skin`, `parent3skin`, `faceMix`, `skinMix`, `thirdMix`, `noseWidth`, `noseHeight`, `noseLength`, `noseBridge`, `noseTip`, `noseBroken`, `browHeight`, `browWidth`, `cheekboneHeight`, `cheekboneWidth`, `cheekWidth`, `eyes`, `lips`, `jawWidth`, `jawHeight`, `chinLength`, `chinPosition`, `chinWidth`, `chinShape`, `neckWidth`, `blemishId`, `blemishOpacity`, `facialhairId`, `facialhairColor`, `facialhairOpacity`, `eyebrowId`, `eyebrowColor`, `eyebrowOpacity`, `ageId`, `ageOpacity`, `makeupId`, `makeupOpacity`, `blushId`, `blushColor`, `blushOpacity`, `complexionId`, `complexionOpacity`, `sundamageId`, `sundamageOpacity`, `lipstickId`, `lipstickColor`, `lipstickOpacity`, `frecklesId`, `frecklesOpacity`, `chesthairId`, `chesthairColor`, `chesthairOpacity`, `bodyblemishId`, `bodyblemishOpacity`, `bodyblemish2Id`, `bodyblemish2Opacity`, `tattoos`) VALUES (NULL, '0', '', '0', '', '', '', '', '', '', '', '', '', '', '', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '255', '0', '255', '0', '0', '255', '0', '0', '255', '0', '255', '0', '255', '0', '0', '255', '0', '255', '0', '255', '0', '0', '255', '0', '255', '0', '0', '255', '0', '255', '0', NULL);
            //SELECT LAST_INSERT_ID();
            //INSERT INTO `vehicles` (`id`, `ownerType`, `ownerID`, `model`, `posX`, `posY`, `posZ`, `rotX`, `rotY`, `rotZ`, `red1`, `green1`, `blue1`, `red2`, `green2`, `blue2`, `pearlescent`, `locked`, `engine`, `numberPlateText`, `numberPlateType`, `dimension`, `createdBy`, `creationDate`) VALUES (NULL, '0', '11', 'elegy', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '1', 'SZEP', '0', '0', 'Chy', CURRENT_TIMESTAMP);
            string query = $"INSERT INTO `vehicles` (`ownerType`, `ownerID`, `model`, `posX`, `posY`, `posZ`, `rotX`, `rotY`, `rotZ`, `red1`, `green1`, `blue1`, `red2`, `green2`, `blue2`, `pearlescent`, `locked`, `engine`, `numberPlateText`, `numberPlateType`, `dimension`, `createdBy`)"+
                $" VALUES " +
                $"('0', @CharacterID, @Model, @PosX, @PosY, @PosZ, @RotX, @RotY, @RotZ, @Red1, @Green1, @Blue1, @Red2, @Green2, @Blue2, @Pearlescent, @Lock, @Engine, @Plate, @PlateType, @Dim, @Creator);";

            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = await Database.DBCon.GetConString();
                await con.OpenAsync();

                using (MySqlCommand command = new MySqlCommand(query, con))
                {
                    command.Parameters.AddWithValue("@CharacterID", j.OwnerID);
                    command.Parameters.AddWithValue("@Model", j.Model);
                    command.Parameters.AddWithValue("@PosX", j.Position.X);
                    command.Parameters.AddWithValue("@PosY", j.Position.Y);
                    command.Parameters.AddWithValue("@PosZ", j.Position.Z);
                    command.Parameters.AddWithValue("@RotX", j.Rotation.X);
                    command.Parameters.AddWithValue("@RotY", j.Rotation.Y);
                    command.Parameters.AddWithValue("@RotZ", j.Rotation.Z);
                    command.Parameters.AddWithValue("@Red1", j.Red1);
                    command.Parameters.AddWithValue("@Green1", j.Green1);
                    command.Parameters.AddWithValue("@Blue1", j.Blue1);
                    command.Parameters.AddWithValue("@Red2", j.Red2);
                    command.Parameters.AddWithValue("@Green2", j.Green2);
                    command.Parameters.AddWithValue("@Blue2", j.Blue2);
                    command.Parameters.AddWithValue("@Pearlescent", j.Pearlescent);
                    command.Parameters.AddWithValue("@Lock", j.Locked);
                    command.Parameters.AddWithValue("@Engine", j.Engine);
                    command.Parameters.AddWithValue("@Plate", j.NumberPlateText);
                    command.Parameters.AddWithValue("@PlateType", j.NumberPlateType);
                    command.Parameters.AddWithValue("@Dim", j.Dimension);
                    command.Parameters.AddWithValue("@Creator", creator);
                    command.Prepare();
                    try
                    {
                        int rows = await command.ExecuteNonQueryAsync();
                        if (rows > 0)
                        {
                            long lastid = command.LastInsertedId;
                            VehicleID = Convert.ToUInt32(lastid);
                        }
                    }
                    catch (Exception ex)
                    {
                        Database.Log.Log_Server(ex.ToString());
                    }

                }
            }
            return VehicleID;
        }



        public static async Task<Jarmu[]> GetPlayerVehicles(uint charID)
        {
            string query = $"SELECT * FROM `vehicles` WHERE `ownerID` = @CharacterID AND `ownerType` = 0";
            
            List<Jarmu> jarmuvek = new List<Jarmu>();
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = await Database.DBCon.GetConString();
                await con.OpenAsync();

                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@CharacterID", charID);
                    cmd.Prepare();
                    try
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                Vector3 pos = new Vector3(Convert.ToSingle(reader["posX"]), Convert.ToSingle(reader["posY"]), Convert.ToSingle(reader["posZ"]));
                                Vector3 rot = new Vector3(Convert.ToSingle(reader["rotX"]), Convert.ToSingle(reader["rotY"]), Convert.ToSingle(reader["rotZ"]));
                                Jarmu j = new Jarmu(Convert.ToUInt32(reader["id"]), Convert.ToUInt32(reader["ownerType"]), Convert.ToUInt32(reader["ownerID"]),reader["model"].ToString(), pos, rot, Convert.ToByte(reader["red1"]), Convert.ToByte(reader["green1"]), Convert.ToByte(reader["blue1"]), Convert.ToByte(reader["red2"]), Convert.ToByte(reader["green2"]), Convert.ToByte(reader["blue2"]), Convert.ToByte(reader["pearlescent"]), Convert.ToBoolean(reader["locked"]), Convert.ToBoolean(reader["engine"]), reader["numberPlateText"].ToString(), Convert.ToByte(reader["numberPlateType"]), Convert.ToUInt32(reader["dimension"]));
                                jarmuvek.Add(j);
                                //public Jarmu(uint id, int ownertype, uint ownerid, string model, Vector3 pos, Vector3 rot, byte r1, byte g1, byte b1, byte r2, byte g2, byte b2, byte pearl, bool locked, bool engine, string numberplate, byte numberplatetype, uint dim)
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Database.Log.Log_Server(ex.ToString());
                    }
                }
            }
            return jarmuvek.ToArray();
        }

        //lesz majd egy DATA-ja a kocsinak, todespawn és egy datetime lesz benne. ha elmúlt a datetime akkor despawnolja, pl óránta nézi meg az összes járművet
        public async static void LoadPlayerVehicles(Player player)
        {
            uint charid = player.GetData<uint>("player:charID");
            Jarmu[] j = await GetPlayerVehicles(charid);

            foreach (var item in j)
            {
                if (vehicles.ContainsKey(Convert.ToInt32(item.ID)))//benne van már az ID tehát létezik a jármű, csak át kell állítani hogy ne despawnoljon
                {

                }
                else//nincs még jármű, létre akarjuk hozni
                {
                    NAPI.Task.Run(() =>
                    {
                        uint vHash = NAPI.Util.GetHashKey(item.Model);
                        Vehicle v = NAPI.Vehicle.CreateVehicle(vHash, item.Position, 0f, 0, 0, item.NumberPlateText, 255, item.Locked, item.Engine, item.Dimension);
                        NAPI.Vehicle.SetVehicleCustomPrimaryColor(v, item.Red1, item.Green1, item.Blue1);
                        NAPI.Vehicle.SetVehicleCustomSecondaryColor(v, item.Red2, item.Green2, item.Blue2);
                        v.Rotation = item.Rotation;
                        NAPI.Vehicle.SetVehiclePearlescentColor(v, item.Pearlescent);
                        NAPI.Vehicle.SetVehicleMod(v, 53, item.NumberPlateType);
                        v.SetData("vehicle:ID", item.ID);
                        v.SetData("vehicle:OwnerType", item.OwnerType);
                        v.SetData("vehicle:OwnerID", item.OwnerID);
                        player.SendChatMessage("Jámű betöltve! " + item.Model + " (" + item.ID + ")");
                        vehicles[Convert.ToInt32(item.ID)] = v;
                    }, 1000);
                }
            }
        }

        [ServerEvent(Event.PlayerDisconnected)]
        public void SetVehiclesToDespawn(Player player, DisconnectionType dc, string reason)
        {
            foreach (var item in vehicles)
            {
                if (item.Value.HasData("vehicle:OwnerType") && item.Value.HasData("vehicle:OwnerID"))//vannak beállítva tulajdonoshoz adatok
                {
                    if (item.Value.GetData<uint>("vehicle:OwnerType") == 0)//0 tehát játékos által birtokolt
                    {
                        uint charid = player.GetData<uint>("player:charID");
                        if (item.Value.GetData<uint>("vehicle:OwnerID") == charid)//a tulajdonos megegyezik a lelépő játékossal
                        {
                            DateTime dateTime = DateTime.Now;
                            TimeSpan span = TimeSpan.FromDays(3);
                            DateTime despawn = dateTime.Add(span);
                            
                            Database.Log.Log_Server("Jármű ID: " + item.Key + " - Idő: "+ DateTime.Now + " Despawn: " + despawn);
                            item.Value.SetData("vehicle:Despawn", despawn);
                        }
                    }
                }
            }
        }


        public void CheckVehiclesToDespawn()
        {
            foreach (var item in vehicles)
            {
                if (item.Value.HasData("vehicle:Despawn"))
                {
                    DateTime despawn = item.Value.GetData<DateTime>("vehicle:Despawn");
                    if (despawn < DateTime.Now)
                    {
                        int vehid = Convert.ToInt32(item.Value.GetData<uint>("vehicle:ID"));
                        item.Value.Delete();//töröljük a járművet
                        vehicles.Remove(vehid);//töröljük a listából
                        Database.Log.Log_Server("Jármú despawnolva.");
                    }
                }
            }

            NAPI.Task.Run(() =>
            {
                CheckVehiclesToDespawn();
            }, 120000);
        }

    

        public void SaveVehicle() 
        { 
        
        }
    }
}
