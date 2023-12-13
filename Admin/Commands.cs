using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using Google.Protobuf.WellKnownTypes;
using GTANetworkAPI;
using Org.BouncyCastle.Asn1.X509;

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

    class Commands : Script
    {
        List<AdminCommand> acmds = new List<AdminCommand>();

        private bool checkAdmin()
        {
            return true;
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
            int adminlevel = player.GetData<int>("player:AdminLevel");
            if (Levels.IsPlayerAdmin("reloadacmds", adminlevel))
            {
                Admin.Levels.LoadAcmds();
                player.SendChatMessage("Admin parancsok újratöltése.");
            }
        }

        [Command("setcommandlevel", Alias = "setacmd", GreedyArg = true, Hide = true)]
        public async void SetAdminCommandLevel(Player player, string parameters = "")
        {
            int adminlevel = player.GetData<int>("player:AdminLevel");

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

        [Command("setadminlevel",Alias = "setalevel", GreedyArg = true, Hide = true)]
        public async void SetPlayerAdminLevel(Player player, string parameters = "")
        {


            int adminlevel = player.GetData<int>("player:AdminLevel");
            
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
                            target.SetData("player:AdminLevel", targetlevel);
                            uint accid = target.GetData<uint>("player:accID");
                            NAPI.Chat.SendChatMessageToAll(player.Name + " átállította " + target.Name + " admin szintjét. [" + adminlevel + "->" + targetlevel+"]");
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


        [Command("adminduty", Alias = "aduty")]
        public void AdminDuty(Player player)
        {
            if (player.HasSharedData("player:AdminDuty"))
            {
                bool state = player.GetSharedData<bool>("player:AdminDuty");
                player.SetSharedData("player:AdminDuty", !state);
                player.SendChatMessage("AdminDuty " + state);
            }
            else
            {
                player.SetSharedData("player:AdminDuty", true);
                player.SendChatMessage("AdminDuty true");
            }

        }

        [Command("setdimension", Alias = "setdim", GreedyArg = true, Hide = true)]
        public void SetDimension(Player player, string parameters = "")
        {
            int adminlevel = player.GetData<int>("player:AdminLevel");

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
            int adminlevel = player.GetData<int>("player:AdminLevel");
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
            player.TriggerEvent("client:LoadIPL", ipl);
        }

        [Command("gotopos", Alias ="gotoxyz")]
        public void GotoPlayer(Player player, float x, float y, float z)
        {
            player.Position = new Vector3(x, y, z);
        }

        [Command("get")]
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
                if (target.HasSharedData("player:Frozen"))
                {
                    bool state = target.GetSharedData<bool>("player:Frozen");
                    NAPI.Data.SetEntitySharedData(target, "player:Frozen", !state);
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
                    NAPI.Data.SetEntitySharedData(target, "player:Frozen", true);
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
            if (player.HasSharedData("player:Invisible"))
            {
                bool state = player.GetSharedData<bool>("player:Invisible");
                NAPI.Data.SetEntitySharedData(player, "player:Invisible", !state);
            }
            else
            {
                NAPI.Data.SetEntitySharedData(player, "player:Invisible", true);
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
            player.SetSharedData("player:WeaponTint", tint);
            
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
            int adminlevel = player.GetData<int>("player:AdminLevel");

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
                    NAPI.Data.SetEntitySharedData(player, "player:Invisible", false);
                    NAPI.Data.SetEntitySharedData(player, "player:Frozen", false);
                    player.TriggerEvent("client:Fly");
                }
                else
                {
                    NAPI.Notification.SendNotificationToPlayer(player, "FLY bekapcsolva.", false);
                    NAPI.Data.SetEntitySharedData(player, "player:Flying", true);
                    NAPI.Data.SetEntitySharedData(player, "player:Invisible", true);
                    NAPI.Data.SetEntitySharedData(player, "player:Frozen", true);
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
            if (player.HasSharedData("player:Crouching"))
            {
                bool state = player.GetSharedData<bool>("player:Crouching");
                player.SetSharedData("player:Crouching", !state);
            }
            else
            {
                player.SetSharedData("player:Crouching", true);
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
