using GTANetworkAPI;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.X509;
using Server.Inventory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Server.Interior
{
    public class Interior
    {
        public uint ID { get; set; }
        public string Name { get; set; }
        public uint OwnerType { get; set; }
        public uint OwnerID { get; set; }
        public string OwnerName { get; set; }
        public Vector3 EntrancePos { get; set; }
        public Vector3 EntranceHeading { get; set; }
        public uint EntranceDimension { get; set; }
        public ColShape Entrance { get; set; }
        public Vector3 ExitPos { get; set; }
        public Vector3 ExitHeading { get; set; }
        public uint ExitDimension { get; set; }
        public ColShape Exit { get; set; }
        public string IPL { get; set; }
        public Interior(uint id, string name, uint ownertype, uint ownerid, Vector3 entrancepos, Vector3 entranceheading, uint entrancedim, Vector3 exitpos, Vector3 exitheading, uint exitdim, string ipl) 
        {
            ID = id;
            Name = name;
            OwnerType = ownertype;
            OwnerID = ownerid;
            EntrancePos = entrancepos;
            EntranceHeading = entranceheading;
            EntranceDimension = entrancedim;
            ExitPos = exitpos;
            ExitHeading = exitheading;
            ExitDimension = exitdim;
            IPL = ipl;
            NAPI.Task.Run(() =>
            {
                Entrance = NAPI.ColShape.CreateCylinderColShape(EntrancePos, 1f, 1f, EntranceDimension); //.CreateCheckpoint(CheckpointType.Cyclinder3, EntrancePos, EntranceHeading, 1f, new Color(255, 255, 255, 100), EntranceDimension);
                Exit = NAPI.ColShape.CreateCylinderColShape(ExitPos, 1f, 1f, ExitDimension); // NAPI.Checkpoint.CreateCheckpoint(CheckpointType.Cyclinder3, ExitPos, ExitHeading, 1f, new Color(255, 255, 255, 100), ExitDimension);
                NAPI.TextLabel.CreateTextLabel(Name, EntrancePos,3f, 1f, 1, new Color(89,173,235,210),false, EntranceDimension);
                NAPI.TextLabel.CreateTextLabel("Kijárat", ExitPos, 3f, 1f, 1, new Color(255, 255, 255,255), false, ExitDimension);
            }, 500);
        }
        public void SetOwnerName(string name)
        {
            OwnerName = name;
        }
    }
    internal class Interiors : Script
    {
        static List<Interior> interiors = new List<Interior>();

       
        public Interior GetInteriorByExit(ColShape cp)
        {
            foreach (var item in interiors)
            {
                if (item.Exit == cp
)
                {
                    return item;
                }
            }
            return null;
        }

        public Interior GetInteriorByEntrance(ColShape cp)
        {
            foreach (var item in interiors)
            {
                if (item.Entrance == cp)
                {
                    return item;
                }
            }
            return null;
        }
        //CHAR: -811.68, 175.2, 76.74, 0, 0, 109.73
        //CAM: -813.95, 174.2, 76.78, 0, 0, -69
        public async static void InitiateInteriors()
        {
            DateTime timestamp1 = DateTime.Now;

            await LoadInteriors();

            DateTime timestamp2 = DateTime.Now;

            TimeSpan LoadTime = timestamp2 - timestamp1;

            NAPI.Util.ConsoleOutput("Interiorok betöltve " + LoadTime.Milliseconds + " ms alatt.");
        }

        public async static Task LoadInteriors()
        {
            string query = $"SELECT * FROM `interiors`";
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = await Database.DBCon.GetConString();
                await con.OpenAsync();

                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Prepare();
                    try
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                Interior i = new Interior(Convert.ToUInt32(reader["id"]), reader["name"].ToString(), Convert.ToUInt32(reader["ownerType"]), Convert.ToUInt32(reader["ownerID"]), new Vector3(Convert.ToSingle(reader["entranceX"]), Convert.ToSingle(reader["entranceY"]), Convert.ToSingle(reader["entranceZ"])), new Vector3(0f, 0f, Convert.ToSingle(reader["entranceHeading"])), Convert.ToUInt32(reader["entranceDimension"]), new Vector3(Convert.ToSingle(reader["exitX"]), Convert.ToSingle(reader["exitY"]), Convert.ToSingle(reader["exitZ"])), new Vector3(0f, 0f, Convert.ToSingle(reader["exitHeading"])), Convert.ToUInt32(reader["exitDimension"]), reader["ipl"].ToString());
                                i.OwnerName = "Nigga bigga";
                                interiors.Add(i);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Database.Log.Log_Server(ex.ToString());
                    }
                }
                await con.CloseAsync();
            }
        }

        [ServerEvent(Event.PlayerEnterColshape)]
        public void EnterCheckpoint(ColShape cp, Player player)
        {
            Interior entrance = GetInteriorByEntrance(cp);
            if (entrance != null)//bejárat
            {
                player.SetData("player:entranceCP", entrance);
                player.SendChatMessage("INTERIOR: " + entrance.Name +"\nBelépéshez használd a /enter parancsot.");
            }
            else//kijárat lehet
            {
                Interior exit = GetInteriorByExit(cp);
                if (exit != null)//létezik és kijárat
                {
                    player.SetData("player:exitCP", exit);
                    player.SendChatMessage("INTERIOR: " + exit.Name + "\nKilépéshez használd a /exit parancsot.");
                }
            }
        }

        [ServerEvent(Event.PlayerExitColshape)]
        public void ExitCheckpoint(ColShape cp, Player player)
        {
            player.ResetData("player:entranceCP");
            player.ResetData("player:exitCP");
        }


        [Command("enter", Alias="exit")]
        public void EnterInterior(Player player)
        {
            if (player.HasData("player:entranceCP"))
            {
                Interior i = player.GetData<Interior>("player:entranceCP");
                player.Position = i.ExitPos;
                player.Rotation = i.ExitHeading;
                player.Dimension = i.ExitDimension;
                player.SendChatMessage("Beléptél az interiorba.");
            }
            else if (player.HasData("player:exitCP"))
            {
                Interior i = player.GetData<Interior>("player:exitCP");
                player.Position = i.EntrancePos;
                player.Rotation = i.EntranceHeading;
                player.Dimension = i.EntranceDimension;
                player.SendChatMessage("Kiléptél az interiorból.");
            }
        }


        [Command("createinterior")]
        public void InteriorTest(Player player, uint interiorid)
        {
            player.TriggerEvent("client:GetCPHeight");
            Vector3 cpPos = player.Position;
            Vector3 cpRot = player.Rotation;
            uint cpDim = player.Dimension;
            NAPI.Task.Run(() =>
            {
                float cpZ = player.GetData<float>("interior:CPheight");
            }, 2000);

            
        }


        [Command("interior")]
        public void InteriorTest(Player player, float scale)
        {
            player.TriggerEvent("client:GetCPHeight");
            NAPI.Task.Run(() =>
            {

            }, 2000);

            float cpZ = player.GetData<float>("interior:CPheight");
            
            Vector3 offset = new Vector3(1f, 1f, 0f);
            Vector3 offset2 = new Vector3(2f, 2f, 0f);
            Checkpoint cp = NAPI.Checkpoint.CreateCheckpoint(CheckpointType.Cyclinder3, player.Position, player.Rotation, scale, new Color(255, 255, 255, 100));
        }

        [RemoteEvent("server:SetCPHeight")]
        public void SetCheckpointHeight(Player player, float height)
        {
            player.SetData("interior:CPheight", height);
        }

    }
}
