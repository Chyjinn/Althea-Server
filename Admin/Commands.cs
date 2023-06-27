using System;
using System.Collections.Generic;
using Google.Protobuf.WellKnownTypes;
using GTANetworkAPI;

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
        public void onResourceStart()
        {
            acmds.Add(new AdminCommand("fly", 1, "repülés"));
        }


        private bool checkAdmin()
        {
            return true;
        }
        
        [Command("haz")]
        public void Haz(Player player)
        {
            Vector3 v = new Vector3(-815.22f, 177.95f, 76.74f);
            player.Position = v;
            
        }

        [Command("freeze")]
        public void FreezePlayer(Player player, int target)
        {
            
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


        [Command("fly", "repülés", Alias ="freecam",GreedyArg = false)]
        public void ToggleFly(Player player)
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

        [Command("flashlight")]
        public void GiveWeapon(Player player)
        {
            NAPI.Player.GivePlayerWeapon(player, WeaponHash.Flashlight, 1);
        }

        [Command("createveh", Alias = "makeveh", GreedyArg = true)]
        public void CreateVehicle(GTANetworkAPI.Player player, string model)
        {
            if (checkAdmin())
                {
                uint vHash = NAPI.Util.GetHashKey(model);
                NAPI.Vehicle.CreateVehicle(vHash, new Vector3(player.Position.X, player.Position.Y + 2.0, player.Position.Z), 0f, 1, 1, "TEMP");
                    
                }
        }

        [Command("setvehcolor", Alias = "setcarcolor")]
        public void SetVehicleColor(GTANetworkAPI.Player player,byte r1, byte g1, byte b1, byte r2, byte g2, byte b2)
        {
            GTANetworkAPI.Vehicle v = player.Vehicle;
            if (v != null)
            {
                    if (checkAdmin())
                    {
                        NAPI.Vehicle.SetVehicleCustomPrimaryColor(v, r1, g1, b1);
                        NAPI.Vehicle.SetVehicleCustomSecondaryColor(v, r2, g2, b2);
                        player.SendChatMessage("Jármű átszínezve!");
                        return;
                    }
                    else
                    {
                        player.SendChatMessage("Add meg a színeket!");
                        return;
                    }
                
            }
            player.SendChatMessage("Nem ülsz járműben!");
            return;
        }

        [Command("gotopos")]
        public void GotoPositionXYZ(GTANetworkAPI.Player player, float X, float Y, float Z)
        {
            NAPI.Player.SpawnPlayer(player, new Vector3(X, Y, Z), player.Heading);
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


        [Command("setclothes", Alias = "clothes", GreedyArg = true)]
        public void SetPlayerClothes(GTANetworkAPI.Player player,string slot, string drawable, string texture)
        {

            player.SendChatMessage($"Ruha sikeresen beállítva! S:{slot}D:{drawable}T:{texture}");
            NAPI.Player.SetPlayerClothes(player, Convert.ToInt32(slot), Convert.ToInt32(drawable), Convert.ToInt32(texture));
            
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


        [Command("settime", GreedyArg = true)]
        public void SetTime(GTANetworkAPI.Player player, string hours, string minutes, string seconds)
        {
            NAPI.World.SetTime(Convert.ToInt16(hours), Convert.ToInt16(minutes), Convert.ToInt16(seconds));
            NAPI.World.SetWeather(Weather.CLEAR);
        }

        [Command("loadipl", Alias = "ipl", GreedyArg = true)]
        public void LoadIPL(GTANetworkAPI.Player player, string name)
        {
            NAPI.ClientEvent.TriggerClientEvent(player, "LoadIPL", name);
        }


        [Command("charedit", Alias = "editchar", GreedyArg =true)]
        public void CharEdit(GTANetworkAPI.Player player)
        {

        }

    }
}
