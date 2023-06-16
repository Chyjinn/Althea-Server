using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using GTANetworkAPI;
using static Google.Protobuf.WellKnownTypes.Field.Types;

namespace Server.Admin
{
    class Commands : Script
    {
        [ServerEvent(Event.ResourceStart)]
        public void onResourceStart()
        {
            NAPI.Util.ConsoleOutput("Parancsok készen!");
        }
        private bool checkAdmin()
        {
            return true;
        }

        [Command("fly", Alias ="freecam")]
        public void ToggleFly(GTANetworkAPI.Player player)
        {
            bool state = false;
            if (NAPI.Data.HasEntitySharedData(player, "flying"))
            {
                state = (bool)NAPI.Data.GetEntitySharedData(player, "flying");
            }

            if (state)
            {
                NAPI.Notification.SendNotificationToPlayer(player, "FLY kikapcsolva.", false);
                NAPI.Data.SetEntitySharedData(player, "flying", false);
                NAPI.Data.SetEntitySharedData(player, "invisible", false);
                NAPI.Data.SetEntitySharedData(player, "frozen", false);
                player.TriggerEvent("client:Fly");

            }
            else
            {
                NAPI.Notification.SendNotificationToPlayer(player, "FLY bekapcsolva.", false);
                NAPI.Data.SetEntitySharedData(player, "flying", true);
                NAPI.Data.SetEntitySharedData(player, "invisible", true);
                NAPI.Data.SetEntitySharedData(player, "frozen", true);
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
                if (r1 != null && g1 != null && b1 != null && r2 != null && g2 != null && b2 != null)
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
