using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GTANetworkAPI;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.X509;
using Server.Inventory;
using Server.Vehicles;

namespace Server.Admin
{
    class AdminCommand
    {
        public string Command { get; set; }//pl: "goto"
        public int Level { get; set; }//pl: 1
        public string Description { get; set; }//pl: "oda tud teleportálni egy játékoshoz"

        public AdminCommand(string cmd, int adminlevel, string description)
        {
            Command = cmd;
            Level = adminlevel;
            Description = description;
        }
    }

    class AdminJail
    {
        public long ID { get; set; }
        public int TargetAccID { get; set; }
        public int AdminAccID { get; set; }
        public string AdminNick { get; set; }
        public string Reason { get; set; }
        public int Time { get; set; }
        public int Remaining { get; set; }
        public DateTime Date { get; set; }
        public DateTime ServedDate { get; set; }
        public AdminJail(long id, int targetAccID, int adminAccID, string adminnick, string reason, int time, int remaining, DateTime date)
        {
            ID = id;
            TargetAccID = targetAccID;
            AdminAccID = adminAccID;
            AdminNick = adminnick;
            Reason = reason;
            Time = time;
            Remaining = remaining;
            Date = date;
        }
    }

        class Commands : Script
    {
        List<AdminCommand> acmds = new List<AdminCommand>();

        public Commands()
        {
            CountAdminJailTime();
        }

        private bool checkAdmin()
        {
            return true;
        }

        //SELECT * FROM `adminjail` WHERE `targetAccountId` LIKE 1 AND `servedDate` IS NULL

        public static async Task<AdminJail> GetPlayerAdminJail(uint accID)
        {
            AdminJail aj = null;
            string query = $"SELECT * FROM `adminjail` WHERE `targetAccountId` = @AccountID AND `servedDate` IS NULL AND `unjailBy` IS NULL LIMIT 1";//nem is töltötte le és nem vette ki senki őt a börtönből

            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = await Database.DBCon.GetConString();
                await con.OpenAsync();

                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@AccountID", accID);
                    cmd.Prepare();
                    try
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                aj = new AdminJail(Convert.ToInt64(reader["id"]), Convert.ToInt32(reader["targetAccountId"]), Convert.ToInt32(reader["adminAccountId"]), Convert.ToString(reader["adminNick"]), Convert.ToString(reader["reason"]), Convert.ToInt32(reader["time"]), Convert.ToInt32(reader["remaining"]), Convert.ToDateTime(reader["date"]));
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
            return aj;
        }

        public async static Task<long> InsertAdminJailIntoDb(uint targetaccid, uint adminaccid, string adminnick, string reason, int time)//adminjail insert adatbázisba
        {
            long id = -1;
            string query = $"INSERT INTO `adminjail` " +
                $"(`targetAccountId`, `adminAccountId`, `adminNick`, `reason`, `time`, `remaining`)" +
                $" VALUES " +
                $"(@TargetAccID, @AdminAccID, @AdminNick, @Reason, @Time, @Remaining)";

            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = await Database.DBCon.GetConString();
                await con.OpenAsync();

                using (MySqlCommand command = new MySqlCommand(query, con))
                {

                    command.Parameters.AddWithValue("@TargetAccID", targetaccid);
                    command.Parameters.AddWithValue("@AdminAccID", adminaccid);
                    command.Parameters.AddWithValue("@AdminNick", adminnick);
                    command.Parameters.AddWithValue("@Reason", reason);
                    command.Parameters.AddWithValue("@Time", time);
                    command.Parameters.AddWithValue("@Remaining", time);

                    command.Prepare();
                    try
                    {
                        int rows = await command.ExecuteNonQueryAsync();
                        if (rows > 0)
                        {
                            id = command.LastInsertedId;
                        }
                    }
                    catch (Exception ex)
                    {
                        Database.Log.Log_Server(ex.ToString());
                    }

                }
                await con.CloseAsync();
            }
            return id;
        }


        public static async Task<bool> UpdateAdminJailRemaining(long jailid)
        {
            bool state = false;
            string query = $"UPDATE `adminjail` SET `remaining` = `remaining`-1 WHERE `adminjail`.`id` = @JailID;";
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = await Database.DBCon.GetConString();
                await con.OpenAsync();

                using (MySqlCommand command = new MySqlCommand(query, con))
                {
                    command.Parameters.AddWithValue("@JailID", jailid);
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
                await con.CloseAsync();
            }
            return state;
        }

        public static async Task<bool> UpdateAdminJailServed(long jailid)
        {
            bool state = false;
            string query = $"UPDATE `adminjail` SET `servedDate` = @ServedDate WHERE `adminjail`.`id` = @JailID;";
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = await Database.DBCon.GetConString();
                await con.OpenAsync();

                using (MySqlCommand command = new MySqlCommand(query, con))
                {
                    command.Parameters.AddWithValue("@ServedDate", DateTime.Now);
                    command.Parameters.AddWithValue("@JailID", jailid);
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
                await con.CloseAsync();
            }
            return state;
        }

        public static async Task<bool> UpdateAdminUnJail(long jailid, uint adminid)
        {
            bool state = false;
            string query = $"UPDATE `adminjail` SET `unjailBy` = @AdminID WHERE `adminjail`.`id` = @JailID;";
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = await Database.DBCon.GetConString();
                await con.OpenAsync();

                using (MySqlCommand command = new MySqlCommand(query, con))
                {
                    command.Parameters.AddWithValue("@AdminID", adminid);
                    command.Parameters.AddWithValue("@JailID", jailid);
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
                await con.CloseAsync();
            }
            return state;
        }


        public async void CountAdminJailTime()
        {
            foreach (var item in NAPI.Pools.GetAllPlayers())
            {
                if (item.HasData("AdminJail:Remaining"))
                {
                    int remaining = item.GetData<int>("AdminJail:Remaining");
                    
                    if (remaining <= 0)
                    {
                        long ajid = item.GetData<long>("AdminJail:ID");
                        if(await UpdateAdminJailServed(ajid))//lejárt a jail, kivesszük őt
                        {
                            
                            NAPI.Task.Run(() =>
                            {
                                item.ResetData("AdminJail:Remaining");
                                Vector3 originalpos = item.GetData<Vector3>("AdminJail:OriginalPos");
                                item.Position = originalpos;
                                item.ResetData("AdminJail:OriginalPos");
                                item.ResetData("AdminJail:ID");
                                item.SendChatMessage("[AdminJail] Letöltötted a büntetésed.");
                                Database.Log.Log_Server("[AdminJail] " + item.Name + " letöltötte a börtönidejét. ");
                            });
                        }
                        else
                        {
                            Database.Log.Log_Server("[AdminJail] HIBA! " + item.Name + " letöltötte a büntetését de nem sikerült az adatbázis mentés.");
                        }
      

                    }
                    else
                    {
                        long ajid = item.GetData<long>("AdminJail:ID");
                        if(await UpdateAdminJailRemaining(ajid))//sikerült frissíteni adatbázisban a fennmaradó időt
                        {
                            NAPI.Task.Run(() =>
                            {
                                item.SetData("AdminJail:Remaining", remaining - 1);
                                item.SendChatMessage("[AdminJail] Hátralévő idő: " + remaining + " perc.");
                            });
                        }
                        else
                        {
                            Database.Log.Log_Server("[AdminJail] HIBA! " + item.Name + " börtönideje nem került levonásra.");
                        }
                    }
                }
                }

            NAPI.Task.Run(() =>
            {
                CountAdminJailTime();
            }, 60000);
        }

        [Command("adminduty", Alias = "aduty", Hide = true)]
        public void Adminduty(Player player)
        {
            int adminlevel = player.GetSharedData<int>("player:AdminLevel");

            if (Levels.IsPlayerAdmin("adminduty", adminlevel))
            {
                if (player.HasSharedData("player:AdminDuty"))
                {
                    string adminnick = player.GetSharedData<string>("player:AdminNick");
                    bool state = player.GetSharedData<bool>("player:AdminDuty");
                    player.SetSharedData("player:AdminDuty", !state);
                    if (!state)
                    {
                        NAPI.Chat.SendChatMessageToAll(adminnick + " adminszolgálatba lépett.");
                    }
                    else
                    {
                        NAPI.Chat.SendChatMessageToAll(adminnick + " kilépett az adminszolgálatból.");
                    }
                }
                else
                {
                    string adminnick = player.GetSharedData<string>("player:AdminNick");
                    player.SetSharedData("player:AdminDuty", true);
                    NAPI.Chat.SendChatMessageToAll(adminnick + " adminszolgálatba lépett.");
                }
                


            }
        }


        [Command("unjail", Alias = "unaj", Hide = true)]
        public async void UnJail(Player player, int targetid = -1)
        {
            int adminlevel = player.GetSharedData<int>("player:AdminLevel");

            if (Levels.IsPlayerAdmin("unjail", adminlevel))
            {
                if (targetid != -1)
                {
                    Player target = GetPlayerById(targetid);
                    if (target != null)
                    {
                        uint AdminAccID = player.GetData<uint>("Player:AccID");
                        string adminnick = player.GetSharedData<string>("player:AdminNick");
                        //1395.5, 1147.2, 114.3, -90
                        //kell neki egy set data hogy jailben van, ne frissítse a kari poziját mentésnél
                        //ha belép és van jailje akkor is dobja be és ugyan ez fusson le
                        int remaining = target.GetData<int>("AdminJail:Remaining");
                        long ajid = target.GetData<long>("AdminJail:ID");

                        if (await UpdateAdminUnJail(ajid, AdminAccID))
                        {
                           
                            NAPI.Task.Run(() =>
                            {
                                Database.Log.Log_Server("[AdminJail] " + adminnick + " kivette a börtönből " + target.Name + " játékost. Maradék idő: " + remaining + " [AJ-ID: " + ajid + "]");
                                player.SendChatMessage("[AdminJail] Kivetted a börtönből " + target.Name + " játékost. Maradék idő: " + remaining);
                                target.SendChatMessage("[AdminJail] " + adminnick + " kivett a börtönből.");

                                target.ResetData("AdminJail:Remaining");
                                Vector3 originalpos = target.GetData<Vector3>("AdminJail:OriginalPos");
                                target.Position = originalpos;
                                target.ResetData("AdminJail:OriginalPos");
                                target.ResetData("AdminJail:ID");
                            });

                        }
                    }
                    else
                    {
                        player.SendChatMessage("[HASZNÁLAT]: /unjail [játékos id]");
                    }
                }
                else
                {
                    player.SendChatMessage("[HASZNÁLAT]: /unjail [játékos id]");
                }

            }
        }



        [Command("adminjail", Alias ="aj", GreedyArg = true, Hide = true)]
        public async void AdminJail(Player player, string parameters = "")
        {
            
            //1395.5, 1147.2, 114.3, -90
            //kell neki egy set data hogy jailben van, ne frissítse a kari poziját mentésnél
            //ha belép és van jailje akkor is dobja be és ugyan ez fusson le
            
            

            
                //ha jailben van akkor törölje az előzőt neki és írjon egy figyelmeztetést/logot -> visszaélés ellen

                int targetid;
            int time;
            string reason = "";
            int adminlevel = player.GetSharedData<int>("player:AdminLevel");

            if (Levels.IsPlayerAdmin("adminjail", adminlevel))
            {
                string[] p = parameters.Split(' ');

                if (p.Length >= 3)//legalább 3 részből áll a parancs, 1. targetid, 2. idő, 3. indok egy szó legalább
                {
                    if (Int32.TryParse(p[0], out targetid) && Int32.TryParse(p[1], out time))//az elsőt is tudjuk parse-olni és a másodikat is
                    {
                        //összeállítjuk az indokot
                        time = time++;
                        for (int i = 1; i < p.Length-1; i++)
                        {
                            reason += p[i+1] + " ";
                        }

                        Player target = GetPlayerById(targetid);
                        if (true)//player != target)
                        {
                            if (target.HasData("Player:CharID") && target.HasData("Player:AccID"))//ha be van jelentkezve és karakterben is van
                            {
                                uint adminaccid = player.GetData<uint>("Player:AccID");
                                uint targetaccid = target.GetData<uint>("Player:AccID");
                                string adminnick = player.GetSharedData<string>("Player:AdminNick");


                                if (target.HasData("AdminJail:ID"))
                                {
                                    long oldajid = target.GetData<long>("AdminJail:ID");
                                    int remaining = target.GetData<int>("AdminJail:Remaining");
                                    if (await UpdateAdminUnJail(oldajid, adminaccid))
                                    {
                                        NAPI.Task.Run(() =>
                                        {
                                            Database.Log.Log_Server("[AdminJail] " + adminnick + " újra börtönbe helyezte " + target.Name + " játékost " + time + " percre. Maradék ideje "+ remaining +" perc volt. [RÉGI AJ ID: "+oldajid+"] Indok: " + reason);
                                        });
                                    }
                                }

                                
                                //1395.5, 1147.2, 114.3, -90
                                //kell neki egy set data hogy jailben van, ne frissítse a kari poziját mentésnél
                                //ha belép és van jailje akkor is dobja be és ugyan ez fusson le

                                long ajid = await InsertAdminJailIntoDb(targetaccid, adminaccid, adminnick, reason, time);
                                NAPI.Task.Run(() =>
                                {
                                    target.SetData("AdminJail:ID", ajid);
                                    Database.Log.Log_Server("[AdminJail] " + adminnick + " bebörtönözte " + target.Name + " játékost " + time + " percre. Indok: " + reason);

                                    player.SendChatMessage("[AdminJail] Bebörtönözted " + target.Name + " játékos " + time + " percre. Indok: " + reason);
                                    target.SendChatMessage("[AdminJail] " + adminnick + " bebörtönzött téged " + time + " percre. Indok: " + reason);

                                    target.SetData("AdminJail:OriginalPos", target.Position);
                                    target.SetData("AdminJail:Remaining", time);

                                    target.Position = new Vector3(1395.5f, 1147.2f, 114.3f);
                                    target.Rotation = new Vector3(0, 0, -90f);
                                    target.Dimension = target.Id;
                                });
                            }
                            else
                            {
                                player.SendChatMessage("[AdminJail] A játékos nincs bejelentkezve.");
                            }

                        }
                        else
                        {
                            player.SendChatMessage("[AdminJail] Saját magadat nem tudod börtönbe helyezni.");
                        }
                    }
                    else
                    {
                        player.SendChatMessage("[HASZNÁLAT]: /adminjail [játékos id] [idő (perc)] [indok]");
                    }


                }
                else
                {
                    player.SendChatMessage("[HASZNÁLAT]: /adminjail [játékos id] [idő (perc)] [indok]");
                }
            }



            if (Levels.IsPlayerAdmin("adminjail", adminlevel))
            {

            }
        }

        [Command("makeup")]
        public void makeup(Player player, int overlay, byte index, float opacity, byte firstcolor, byte secondcolor )
        {
            HeadOverlay h = new HeadOverlay();
            h.Index = index;
            h.Opacity = opacity;
            h.Color = firstcolor;
            h.SecondaryColor = secondcolor;
            player.SetHeadOverlay(overlay, h);
        }

        [Command("nametagtest")]
        public void nametagtest(Player player)
        {
            player.TriggerEvent("client:NametagTest");
        }

        [Command("skycam")]
        public void SkyCam(Player player, bool state)
        {
            player.TriggerEvent("client:SkyCam", state);
        }


        [Command("editcurrentlyrequiredadminlevelforcommandandsaveintodatabase")]
        public void EditAdminLevel(Player player)
        {
            player.SendChatMessage("[HASZNÁLAT]: /editcurrentlyrequiredadminlevelforcommandandsaveintodatabase [parancs] [admin szint]");
        }

        [Command("reloadacmds")]
        public void ReloadAdminCommands(Player player)
        {
            int adminlevel = player.GetSharedData<int>("player:AdminLevel");
            if (Levels.IsPlayerAdmin("reloadacmds", adminlevel))
            {
                Admin.Levels.LoadAcmds();
                player.SendChatMessage("Admin parancsok újratöltése.");
            }
        }

        [Command("setcommandlevel", Alias = "setacmd", GreedyArg = true, Hide = true)]
        public async void SetAdminCommandLevel(Player player, string parameters = "")
        {
            int adminlevel = player.GetSharedData<int>("player:AdminLevel");

            if (Levels.IsPlayerAdmin("setcommandlevel", adminlevel))
            {
                string[] p = parameters.Split(' ');

                if (p.Length == 2)
                {
                    string cmd = p[0];

                    if (Int32.TryParse(p[1], out int targetlevel) == true)
                    {
                        player.SendChatMessage("/" + cmd + " parancs sikeresen átállítva [" + targetlevel + "]");
                        await Admin.Levels.SetCommandLevel(cmd,targetlevel);
                    }
                }
                else
                {
                    player.SendChatMessage("[HASZNÁLAT]: /setcommandlevel [parancs] [admin szint]");
                }
            }
        }

        [Command("setadminnick", Alias = "setanick", GreedyArg = true, Hide = true)]
        public async void SetPlayerAdminNick(Player player, string parameters = "")
        {
            int adminlevel = player.GetSharedData<int>("player:AdminLevel");

            if (Levels.IsPlayerAdmin("setadminnick", adminlevel))
            {
                Player target = null;
                string[] p = parameters.Split(' ');

                if (p.Length == 2)
                {

                    if (p[0] == "*")
                    {
                        target = player;
                    }
                    else
                    {
                        if (Int32.TryParse(p[0], out int targetid) == true)
                        {
                            target = GetPlayerById(targetid);
                        }
                        else
                        {
                            player.SendChatMessage("Játékos nem található!");
                        }
                    }

                    //megkeressük a target játékost
                    if (target != null)
                    {
                        string targetnick = p[1];
                        string targetoriginalnick = target.GetSharedData<string>("player:AdminNick");
                        string adminnick = player.GetSharedData<string>("player:AdminNick");
                        target.SetSharedData("player:AdminNick", targetnick);
                        uint accid = target.GetData<uint>("Player:AccID");
                        NAPI.Chat.SendChatMessageToAll(adminnick + " átállította " + target.Name + " admin nevét. [" + targetoriginalnick + "->" + targetnick + "]");
                        await Admin.Levels.SetAdminNick(accid, targetnick);
                    }
                    else
                    {
                        player.SendChatMessage("Játékos nem található");
                    }
                    
                }
                else
                {
                    player.SendChatMessage("[HASZNÁLAT]: /setadminlevel [játékos ID] [admin szint]");
                }
            }
        }

        [Command("setadminlevel",Alias = "setalevel", GreedyArg = true, Hide = true)]
        public async void SetPlayerAdminLevel(Player player, string parameters = "")
        {
            int adminlevel = player.GetSharedData<int>("player:AdminLevel");
            
            if (Levels.IsPlayerAdmin("setadminlevel", adminlevel))
            {
                Player target = null;
                string[] p = parameters.Split(' ');

                if (p.Length == 2)
                {

                if (p[0] == "*")
                {
                    target = player;
                }
                else
                {
                    if(Int32.TryParse(p[0], out int targetid) == true)
                    {
                        target = GetPlayerById(targetid);
                    }
                    else
                    {
                        player.SendChatMessage("Játékos nem található!");
                    }
                }

                    if (Int32.TryParse(p[1], out int targetlevel) == true)
                    {
                        //megkeressük a target játékost
                        if (target != null)
                        {
                            int targetoriginallevel = target.GetSharedData<int>("player:AdminLevel");
                            string adminnick = player.GetSharedData<string>("player:AdminNick");
                            target.SetSharedData("player:AdminLevel", targetlevel);
                            uint accid = target.GetData<uint>("Player:AccID");
                            NAPI.Chat.SendChatMessageToAll(adminnick + " átállította " + target.Name + " admin szintjét. [" + targetoriginallevel + "->" + targetlevel+"]");
                            await Admin.Levels.SetAdminLevel(accid, targetlevel);
                        }
                        else
                        {
                            player.SendChatMessage("Játékos nem található");
                        }
                    }
                }
                else
                {
                    player.SendChatMessage("[HASZNÁLAT]: /setadminlevel [játékos ID] [admin szint]");
                }
            }
        }


        [Command("haz")]
        public void Haz(Player player)
        {
            Vector3 v = new Vector3(-815.22f, 177.95f, 76.74f);
            player.Position = v;
            
        }

        [Command("kick")]
        public void KickPlayer(Player player, ushort id, string reason)
        {
            Player target = RAGE.Entities.Players.All.Where((p) => p.Id == id).FirstOrDefault();
            target.Kick(reason);
        }

        [Command("fov")]
        public void SetFOV(Player player, float fov)
        {
            player.TriggerEvent("client:SetFPSFov", fov);
        }


        [Command("sprint")]
        public void SetSprint(Player player, float value)
        {
            player.TriggerEvent("client:SetRunning", value);
        }


        [Command("helicam")]
        public void GetId(Player player)
        {
            player.TriggerEvent("client:HeliCam");
        }


        [Command("ragdoll")]
        public void Ragdoll(Player player)
        {
            if (player.HasSharedData("player:Ragdoll"))
            {
                bool state = player.GetSharedData<bool>("player:Ragdoll");
                NAPI.Data.SetEntitySharedData(player, "player:Ragdoll", !state);
                if (!state)
                {
                    player.SendChatMessage("ragdoll on");
                    player.SetSharedData("player:Ragdoll", true);
                }
                else
                {
                    player.SendChatMessage("ragdoll off");
                    player.SetSharedData("player:Ragdoll", false); ;
                }

            }
            else
            {
                player.SendChatMessage("ragdoll on");
                NAPI.Data.SetEntitySharedData(player, "player:Ragdoll", true);
            }


            
        }



        [Command("sethp")]
        public void SetHP(Player player, int targetid = -1, int hp = 100)
        {
            if (targetid == -1)
            {
                if (player.Dead == true)
                {
                    player.WarpOutOfVehicle();
                    Vector3 pos = player.Position;
                    NAPI.Player.SpawnPlayer(player, pos);
                    player.Health = hp;
                    player.SendChatMessage("HP-d sikeresen átállítva. (" + hp + ")");
                }
                else
                {
                    NAPI.Player.SetPlayerHealth(player, hp);
                    player.SendChatMessage("HP-d sikeresen átállítva. (" + hp + ")");
                }
            }
            else
            {
                Player target = GetPlayerById(targetid);
                if (target != null)
                {
                    if (target.Dead == true)
                    {
                        player.WarpOutOfVehicle();
                        Vector3 pos = target.Position;
                        NAPI.Player.SpawnPlayer(target, pos);
                        player.Health = hp;
                        player.SendChatMessage(target.Name + " HP-ja átállítva. (" + hp + ")");
                        target.SendChatMessage(player.Name + " átállította a HP-d. (" + hp + ")");
                    }
                    else
                    {
                        NAPI.Player.SetPlayerHealth(target, hp);
                        player.SendChatMessage(target.Name + " HP-ja átállítva. (" + hp + ")");
                        target.SendChatMessage(player.Name + " átállította a HP-d. (" + hp + ")");
                    }
                    
                }
                else
                {
                    player.SendChatMessage("Nincs ilyen játékos");
                }


            }
        }



        [Command("setarmor", Alias = "setarmour")]
        public void SetArmor(Player player, int targetid = -1, int armor = 100)
        {
            if (targetid == -1)
            {
                    NAPI.Player.SetPlayerArmor(player, armor);
                    player.SendChatMessage("Armor sikeresen átállítva. (" + armor + ")");
            }
            else
            {
                Player target = GetPlayerById(targetid);
                if (target != null)
                {
                        NAPI.Player.SetPlayerArmor(target, armor);
                        player.SendChatMessage(target.Name + " armor-ja átállítva. (" + armor + ")");
                        target.SendChatMessage(player.Name + " átállította az armor-od. (" + armor + ")");

                }
                else
                {
                    player.SendChatMessage("Nincs ilyen játékos");
                }


            }
        }

        public static Player GetPlayerById(int id)
        {
            return NAPI.Pools.GetAllPlayers().FirstOrDefault(plr => plr.Id == id);
        }

        [Command("id", Hide = true)]
        public void GetId(Player player, int id = -1)
        {
            if (id == -1)
            {
                id = player.Id;
                player.SendChatMessage("Az ID-d: " + player.Id.ToString());
            }
            else
            {
                Player target = GetPlayerById(id);
                if (target != null)
                {
                    player.SendChatMessage("ID: " + target.Id.ToString() + " - " + target.Name);
                }
                else
                {
                    player.SendChatMessage("Nincs ilyen játékos");
                }
            }

            
        }

        [Command("players")]
        public void ListPlayers(Player player)
        {
            List<Player> playerlist = NAPI.Pools.GetAllPlayers();
            foreach (var item in playerlist)
            {
                player.SendChatMessage("[" + item.Id + "] " + item.Name);
            }
        }

        [Command("dim", Alias = "dimesion")]
        public void ShowDimension(Player player)
        {

            player.SendChatMessage("Dimenziód: " + player.Dimension.ToString());
        }


        [Command("setdimension", Alias = "setdim", GreedyArg = true, Hide = true)]
        public void SetDimension(Player player, string parameters = "")
        {
            int adminlevel = player.GetSharedData<int>("player:AdminLevel");

            if (Levels.IsPlayerAdmin("setdimension", adminlevel))
            {
                Player target = null;
                string[] p = parameters.Split(' ');//felbontjuk a dolgokat

                if (p[0] == "*")
                {
                    target = player;
                }
                else
                {
                    if (Int32.TryParse(p[0], out int targetid) == true)
                    {
                        target = GetPlayerById(targetid);
                    }
                    else
                    {
                        player.SendChatMessage("Játékos nem található!");
                    }
                }

                if (Int32.TryParse(p[1], out int dimension) == true)//megvizsgáljuk hogy jót adott-e meg
                {
                    //megkeressük a target játékost
                    if (target != null)
                    {
                        target.Dimension = Convert.ToUInt32(dimension);//átállítjuk és kiírjuk akinek kell
                        player.SendChatMessage(target.Name + " dimenzióját átállítottad: " + dimension);
                        target.SendChatMessage(player.Name + " átállította a dimenziódat: " + dimension);
                    }
                    else
                    {
                        player.SendChatMessage("Játékos nem található");//nincs ilyen player
                    }
                }
                else
                {
                    player.SendChatMessage("[HASZNÁLAT]: /setdimension [játékos ID] [dimenzió]");//rossz értékeket adott meg (nem lehet parse-elni)
                }
            }
        }


        [Command("admin")]
        public void GetAdminLevel(Player player)
        {
            int adminlevel = player.GetSharedData<int>("player:AdminLevel");
            player.SendChatMessage("Admin szinted: " + adminlevel);
        }


        [Command("goto")]
        public void GotoPlayer(Player player, int targetid)
        {
            Player target = GetPlayerById(targetid);
            if (target != null)
            {
                Vector3 offset = target.Position;
                offset.Y += 0.5f;
                player.Position = offset;
                player.Dimension = target.Dimension;
                player.SendChatMessage("Goto");
            }
        }

        [Command("objectmover")]
        public void ObjectMover(Player player, string objectname)
        {
            player.TriggerEvent("client:PlaceObject", objectname);
            player.SendChatMessage(objectname + " elhelyezése.");
        }

        [Command("ipl")]
        public void LoadIplForPlayer(Player player, string ipl)
        {
            player.TriggerEvent("client:RequestIPL", ipl);
        }
        [Command("removeipl")]
        public void RemoveIplForPlayer(Player player, string ipl)
        {
            player.TriggerEvent("client:RemoveIPL", ipl);
        }



        [Command("gotopos", Alias ="gotoxyz")]
        public void GotoPlayer(Player player, float x, float y, float z)
        {
            player.Position = new Vector3(x, y, z);
        }

        [Command("get", Alias ="gethere")]
        public void GetPlayer(Player player, int targetid)
        {
            Player target = GetPlayerById(targetid);
            if (target != null)
            {
                target.Position = player.Position;
            }
        }
        
        [Command("freeze",Description = "Freeze így")]
        public void FreezePlayer(Player player, int targetid = -1)
        {
            Player target = GetPlayerById(targetid);
            if (target != null)
            {
                if (target.HasSharedData("Player:Frozen"))
                {
                    bool state = target.GetSharedData<bool>("Player:Frozen");
                    NAPI.Data.SetEntitySharedData(target, "Player:Frozen", !state);
                    if (!state)
                    {
                        player.SendChatMessage("Lefagyasztottad " + target.Name + " játékost.");
                        target.SendChatMessage(player.Name + " lefagyasztott téged.");
                    }
                    else
                    {
                        player.SendChatMessage("Kiolvasztottad " + target.Name + " játékost.");
                        target.SendChatMessage(player.Name + " kiolvasztott téged.");
                    }

                }
                else
                {
                    NAPI.Data.SetEntitySharedData(target, "Player:Frozen", true);
                    player.SendChatMessage("Lefagyasztottad " + target.Name + " játékost.");
                    target.SendChatMessage(player.Name + " lefagyasztott téged.");
                }
            }
            else
            {
                player.SendChatMessage("Játékos nem található");
            }

        }

        [Command("disappear", Alias = "dis")]
        public void Disappear(Player player)
        {
            if (player.HasSharedData("Player:Invisible"))
            {
                bool state = player.GetSharedData<bool>("Player:Invisible");
                NAPI.Data.SetEntitySharedData(player, "Player:Invisible", !state);
            }
            else
            {
                NAPI.Data.SetEntitySharedData(player, "Player:Invisible", true);
            }
        }

        [Command("teszt", "/teszt [x] [y] [z]")]
        public void Teszteles(Player player, float x, float y, float z)
        {
            try
            {
                player.TriggerEvent("client:YTtest");
            }
            catch (FormatException ex)
            {
                player.SendChatMessage(x.ToString());
                player.SendChatMessage(ex.ToString());
            }
        }


        [Command("yt", "youtube", Hide =true)]
        public void YoutubeTest(Player player)
        {
                player.TriggerEvent("client:YTtest");
        }

        [Command("tint")]
        public void WeaponTint(Player player, int tint)
        {
            player.SendChatMessage("Tint átállítva");
            player.SetSharedData("Player:WeaponTint", tint);
            
        }

        [Command("weapon")]
        public void WeaponCommand(Player sender, string hash)
        {
            //NAPI.Player.GivePlayerWeapon(sender, hash, 500);
            NAPI.Player.GivePlayerWeapon(sender, NAPI.Util.GetHashKey(hash), 3000);
        }

        [Command("camtest")]
        public void Camtest(Player player)
        {
            player.TriggerEvent("client:camtest");
        }


        [Command("radargun")]
        public void RadarGun(Player player, bool state)
        {
            player.TriggerEvent("client:RadarGun", state);
        }
        [Command("speedcam")]
        public void SpeedCam(Player player, bool state)
        {
            player.TriggerEvent("client:SpeedCam", state);
        }

        [Command("fly", Alias ="freecam")]
        public void ToggleFly(Player player)
        {
            int adminlevel = player.GetSharedData<int>("player:AdminLevel");

            if (Levels.IsPlayerAdmin("fly", adminlevel))
            {
                bool state = false;
                if (NAPI.Data.HasEntitySharedData(player, "player:Flying"))
                {
                    state = (bool)NAPI.Data.GetEntitySharedData(player, "player:Flying");
                }

                if (state)
                {
                    NAPI.Notification.SendNotificationToPlayer(player, "FLY kikapcsolva.", false);
                    NAPI.Data.SetEntitySharedData(player, "player:Flying", false);
                    NAPI.Data.SetEntitySharedData(player, "Player:Invisible", false);
                    NAPI.Data.SetEntitySharedData(player, "Player:Frozen", false);
                    player.TriggerEvent("client:Fly");
                }
                else
                {
                    NAPI.Notification.SendNotificationToPlayer(player, "FLY bekapcsolva.", false);
                    NAPI.Data.SetEntitySharedData(player, "player:Flying", true);
                    NAPI.Data.SetEntitySharedData(player, "Player:Invisible", true);
                    NAPI.Data.SetEntitySharedData(player, "Player:Frozen", true);
                    player.TriggerEvent("client:Fly");
                }
            }
            else
            {
                NAPI.Chat.SendChatMessageToPlayer(player, "Nincs jogod ehhez a parancshoz!");
            }

        }
        [RemoteEvent("server:VehicleIndicator")]
        public void VehicleIndicator(Player player, bool side)
        {
            if (player.Vehicle != null)
            {
                Vehicle v = player.Vehicle;
                if (side)//jobb 
                {
                    if (v.HasSharedData("vehicle:IndicatorRight"))
                    {
                        bool state = v.GetSharedData<bool>("vehicle:IndicatorRight");
                        v.SetSharedData("vehicle:IndicatorRight", !state);
                    }
                    else
                    {
                        v.SetSharedData("vehicle:IndicatorRight", true);
                    }

                }
                else//bal
                {
                    if (v.HasSharedData("vehicle:IndicatorLeft"))
                    {
                        bool state = v.GetSharedData<bool>("vehicle:IndicatorLeft");
                        v.SetSharedData("vehicle:IndicatorLeft", !state);
                    }
                    else
                    {
                        v.SetSharedData("vehicle:IndicatorLeft", true);
                    }
                }
            }

        }

        [RemoteEvent("server:ToggleCrouching")]
        public void ToggleCrouching(Player player)
        {
            if (player.HasSharedData("Player:Crouching"))
            {
                bool state = player.GetSharedData<bool>("Player:Crouching");
                player.SetSharedData("Player:Crouching", !state);
            }
            else
            {
                player.SetSharedData("Player:Crouching", true);
            }
        }

        [Command("serial")]
        public void ShowSerial(Player player)
        {
            player.SendChatMessage(player.Serial);
            Database.Log.Log_Server(player.Serial);
        }

        [Command("obj")]
        public void ObjectTest(Player player, string objectname)
        {
            NAPI.Object.CreateObject(NAPI.Util.GetHashKey(objectname), player.Position, player.Rotation, 255, player.Dimension);
        }

        [Command("flashlight")]
        public void GiveWeapon(Player player)
        {
            NAPI.Player.GivePlayerWeapon(player, WeaponHash.Flashlight, 1);
        }

        [Command("siren")]
        public void VehicleSiren(Player player, string siren)
        {
            Vehicle v = NAPI.Player.GetPlayerVehicle(player);
            v.SetSharedData("vehicle:Siren", siren);
        }

        [Command("setweather", Alias ="weather")]
        public void SetWeather(GTANetworkAPI.Player player, string weather)
        {
            NAPI.World.SetWeather(weather);
        }

        [Command("settime")]
        public void SetTime(GTANetworkAPI.Player player, string hours, string minutes, string seconds)
        {

            NAPI.World.SetTime(Convert.ToInt16(hours), Convert.ToInt16(minutes), Convert.ToInt16(seconds));
        }

        [Command("setdof")]
        public void SetDOF(GTANetworkAPI.Player player, bool state, float near, float far)
        {
            player.TriggerEvent("client:SetDOF", state, near, far);
        }

        static float WindDirection = 0f;
        static float WindSpeed = 0f;


        [Command("setwind")]
        public void SetWind(GTANetworkAPI.Player player, float dir, float speed)
        {
            WindDirection = dir;
            WindSpeed = speed;
            List<Player> p = NAPI.Pools.GetAllPlayers();
            foreach (var item in p)
            {
                item.TriggerEvent("client:SetWind", WindDirection, WindSpeed);
            }
            player.SendChatMessage("Szélirány és sebesség átállítva!");
        }

        [RemoteEvent("server:SendWind")]
        public void SendWind(GTANetworkAPI.Player player)
        {
            player.TriggerEvent("client:SetWind", WindDirection, WindSpeed);
        }




        [Command("respawn")]
        public void RespawnPlayer(GTANetworkAPI.Player player)
        {
            NAPI.Player.SpawnPlayer(player, new Vector3(-424, 1115, 326));
        }


        [Command("setplayerhead", Alias = "head", GreedyArg = true)]
        public void SetPlayerHead(GTANetworkAPI.Player player, byte Shape1, byte Shape2, byte Shape3, byte Skin1, byte Skin2, byte Skin3, float ShapeMix, float SkinMix, float ThirdMix)
        {
            HeadBlend h = player.HeadBlend;
            h.ShapeFirst = Shape1;
            h.ShapeSecond = Shape2;
            h.ShapeThird = Shape3;
            h.SkinFirst = Skin1;
            h.SkinSecond = Skin2;
            h.SkinThird = Skin3;
            h.ShapeMix = ShapeMix;
            h.SkinMix = SkinMix;
            h.ThirdMix = ThirdMix;
            NAPI.Player.SetPlayerHeadBlend(player, h);
        }
        /*
        [Command("setplayerface", Alias = "face", GreedyArg = true)]
        public void SetPlayerFace(GTANetworkAPI.Player player, )
        {
            float[] faceFeatures = new float[20];
            for (int i = 0; i < faceFeatures.Length; i++)
            {
                faceFeatures[i] = 0;
                NAPI.Player.SetPlayerFaceFeature(player, i, 0);
            }
        }*/


        [Command("setclothes", GreedyArg = true)]
        public void SetPlayerClothes(GTANetworkAPI.Player player,string slot, string drawable, string texture)
        {

            player.SendChatMessage($"Ruha sikeresen beállítva! S:{slot}D:{drawable}T:{texture}");
            NAPI.Player.SetPlayerClothes(player, Convert.ToInt32(slot), Convert.ToInt32(drawable), Convert.ToInt32(texture));
        }
        [Command("setprop", GreedyArg = true)]
        public void SetPlayerProps(GTANetworkAPI.Player player, string slot, string drawable, string texture)
        {
            player.SendChatMessage($"Prop sikeresen beállítva! S:{slot}D:{drawable}T:{texture}");
            NAPI.Player.SetPlayerAccessory(player, Convert.ToInt32(slot), Convert.ToInt32(drawable), Convert.ToInt32(texture));
        }



        [Command("testclothes")]
        public void SetPlayerClothes(GTANetworkAPI.Player player)
        {
            HeadBlend h = player.HeadBlend;
            h.ShapeFirst = 21;
            h.ShapeSecond = 45;
            h.ShapeThird = 31;
            h.SkinFirst = 21;
            h.SkinSecond = 45;
            h.SkinThird = 31;
            h.ShapeMix = 0.5f;
            h.SkinMix = 0.5f;
            h.ThirdMix = 0.5f;
            NAPI.Player.SetPlayerHeadBlend(player, h);

            float[] faceFeatures = new float[20];
            for (int i = 0; i < faceFeatures.Length; i++)
            {
                faceFeatures[i] = 0;
                NAPI.Player.SetPlayerFaceFeature(player, i, 0);
            }
            Dictionary<int, HeadOverlay> headOv = new Dictionary<int, HeadOverlay>();
            Decoration[] dec = new Decoration[0];
            NAPI.Player.SetPlayerCustomization(player, false, h, 0, 0, 0, faceFeatures, headOv, dec);
            NAPI.Player.SetPlayerClothes(player, Convert.ToInt32(11), Convert.ToInt32(450), Convert.ToInt32(0));

        }

        [Command("testoverlay")]//szemöldök, ráncok, stb.
        public void TestOverlay(Player player)
        {
            HeadOverlay overlay = new HeadOverlay();
            overlay.Index = 0;
            overlay.Opacity = 0.8f;
            overlay.Color = 10;
            overlay.SecondaryColor = 10;
            NAPI.Player.SetPlayerHeadOverlay(player,0, overlay);//meg kell nézni hogy az ID mit befolyásol, az overlayek sorrendjét? mert van az overlaynek is index tulajdonsága
        }

        [Command("testdecoration")]//tetkók, shirt decals
        public void TestDecoration(Player player)
        {
            uint hash = NAPI.Util.GetHashKey("tetkó collection-ból");
            uint hash2 = NAPI.Util.GetHashKey("valami tetkó");
            Decoration d = new Decoration();
            d.Collection = hash;
            d.Overlay = hash2;
            NAPI.Player.SetPlayerDecoration(player, d);
        }





    }
}
