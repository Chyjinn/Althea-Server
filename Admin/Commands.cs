using System;
using System.Collections.Generic;
using GTANetworkAPI;
using GTANetworkMethods;

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


        [Command("setplayerface", Alias = "face", GreedyArg = true)]
        public void SetPlayerApperance(GTANetworkAPI.Player player)
        {
            HeadBlend h = player.HeadBlend;
            h.ShapeFirst = 45;
            h.ShapeSecond = 21;
            h.ShapeThird = 25;
            h.SkinFirst = 12;
            h.SkinSecond = 23;
            h.SkinThird = 25;
            h.ShapeMix = 0.5f;
            h.SkinMix = 0.5f;
            h.ThirdMix = 0.5f;
            NAPI.Player.SetPlayerClothes(player, 11, 30, 0);
            NAPI.Player.SetPlayerHeadBlend(player, h);
        }

        [Command("setclothes", Alias = "clothes", GreedyArg = true)]
        public void SetPlayerClothes(GTANetworkAPI.Player player,string slot, string drawable, string texture)
        {

            player.SendChatMessage($"Ruha sikeresen beállítva! S:{slot}D:{drawable}T:{texture}");
            NAPI.Player.SetPlayerClothes(player, Convert.ToInt32(slot), Convert.ToInt32(drawable), Convert.ToInt32(texture));
            
        }

    }
}
