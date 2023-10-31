using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GTANetworkAPI;
using MySql.Data.MySqlClient;
using Server.Characters;

namespace Server.Vehicles
{
    public class Jarmu
    {
        public uint ID { get; set; }
        public int OwnerType { get; set; }
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

        public Jarmu(uint id, int ownertype, uint ownerid, string model, Vector3 pos, Vector3 rot, byte r1, byte g1, byte b1, byte r2, byte g2, byte b2, byte pearl, bool locked, bool engine, string numberplate, byte numberplatetype, uint dim)
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
        int tempIndex = -1;

        [Command("tempveh", Alias = "tempcar")]
        public void TemporaryVehicle(Player player, string model)
        {
            uint vHash = NAPI.Util.GetHashKey(model);
            Vehicle v = NAPI.Vehicle.CreateVehicle(vHash, new Vector3(player.Position.X, player.Position.Y + 2.0, player.Position.Z), 0f, 1, 1, "TEMP");
            v.Dimension = player.Dimension;
            v.SetData("vehicle:ID", tempIndex);
            player.SendChatMessage("Ideiglenes jármű létrehozva: " + model.ToLower() + " ("+tempIndex+")");
            
            vehicles[tempIndex] = v;
            tempIndex--;
        }


        [Command("makeveh", Alias = "makevehicle")]
        public void CreateVehicle(Player player, string model)
        {
            uint vHash = NAPI.Util.GetHashKey(model);
            Vehicle v = NAPI.Vehicle.CreateVehicle(vHash, new Vector3(player.Position.X, player.Position.Y + 2.0, player.Position.Z), 0f, 1, 1, "VEGLEGES");
            v.Dimension = player.Dimension;


            v.SetData("vehicle:ID", tempIndex);
            player.SendChatMessage("Ideiglenes jármű létrehozva: " + model.ToLower() + " (" + tempIndex + ")");

            vehicles[tempIndex] = v;
            tempIndex--;
        }



        [Command("getveh")]
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

        [Command("engine", Alias = "eng")]
        public void StartStopEngine(Player player)
        {
            if (player.Vehicle != null)
            {
                player.Vehicle.EngineStatus = !player.Vehicle.EngineStatus;
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


        public static async Task<uint> CreateNewVehicle()
        {

        }


        public static async Task<Jarmu[]> GetPlayerVehicles(uint charID)
        {
            string query = $"SELECT * FROM `vehicles` WHERE `ownerID` = @CharacterID AND `ownerType` = 0";
            
            List<Jarmu> jarmuvek = new List<Jarmu>();
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = Database.DBCon.GetConString();
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
                                Jarmu j = new Jarmu(Convert.ToUInt32(reader["id"]), Convert.ToInt32(reader["ownerType"]), Convert.ToUInt32(reader["ownerID"]),reader["model"].ToString(), pos, rot, Convert.ToByte(reader["red1"]), Convert.ToByte(reader["green1"]), Convert.ToByte(reader["blue1"]), Convert.ToByte(reader["red2"]), Convert.ToByte(reader["green2"]), Convert.ToByte(reader["blue2"]), Convert.ToByte("pearlescent"), Convert.ToBoolean(reader["locked"]), Convert.ToBoolean(reader["engine"]), reader["numberPlateText"].ToString(), Convert.ToByte(reader["numberPlateType"]), Convert.ToUInt32(reader["dimension"]));
                                jarmuvek.Add(j);
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
        public async void LoadPlayerVehicles(Player player)
        {
            uint charid = player.GetData<uint>("player:charID");
            Jarmu[] j = await GetPlayerVehicles(charid);
            for (int i = 0; i < j.Length; i++)
            {
                if (vehicles.ContainsKey(Convert.ToInt32(j[i].ID)))//benne van már az ID tehát létezik a jármű, csak át kell állítani hogy ne despawnoljon
                {

                }
                else//nincs még jármű, létre akarjuk hozni
                {
                    uint vHash = NAPI.Util.GetHashKey(j[i].Model);
                    Vehicle v = NAPI.Vehicle.CreateVehicle(vHash, j[i].Position, 0f, 0, 0, j[i].NumberPlateText, 255, j[i].Locked, j[i].Engine, j[i].Dimension);
                    NAPI.Vehicle.SetVehicleCustomPrimaryColor(v, j[i].Red1, j[i].Green1, j[i].Blue1);
                    NAPI.Vehicle.SetVehicleCustomSecondaryColor(v, j[i].Red2, j[i].Green2, j[i].Blue2);
                    v.Rotation = j[i].Rotation;
                    NAPI.Vehicle.SetVehiclePearlescentColor(v, j[i].Pearlescent);
                    NAPI.Vehicle.SetVehicleMod(v, 53, j[i].NumberPlateType);
                    v.SetData("vehicle:ID", j[i].ID);
                    player.SendChatMessage("Jámű betöltve! " + j[i].Model + " (" + j[i].ID + ")");
                }
            }

            
        }

    

        public void SaveVehicle() 
        { 
        
        }
    }
}
