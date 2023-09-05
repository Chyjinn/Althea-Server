using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Characters
{
    internal class Clothing : Script
    {
        List<Checkpoint> clothingCpList = new List<Checkpoint>();


        [ServerEvent(Event.ResourceStart)]
        public void CreateClothingCP()
        {
            Checkpoint cp = NAPI.Checkpoint.CreateCheckpoint(CheckpointType.Cyclinder3, new Vector3(428f, -800f, 28.5f), new Vector3(0, 1f, 0), 1f, new Color(255, 0, 0, 100), 0);
            clothingCpList.Add(cp);
            NAPI.Blip.CreateBlip(73, new Vector3(428f, -800f, 150f), 1f, 0, "Ruhabolt", 255, 0, true, 0, 0);
            NAPI.Blip.CreateBlip(149, new Vector3(103.7f, -1939.2f, 50f), 1f, 85, "N?", 255, 0, true, 0, 0);
        }


        [ServerEvent(Event.PlayerEnterCheckpoint)]
        public void OnPlayerEnterCheckpoint(Checkpoint checkpoint, Player player)
        {
            if (clothingCpList.Contains(checkpoint))//ruha cp
            {
                Random r = new Random();
                player.Dimension = Convert.ToUInt32(r.Next(0, 5000));

                player.TriggerEvent("client:ClothingShop", true);
                player.SetSharedData("player:Frozen", true);
                player.TriggerEvent("client:EditorCamera", true);
            }
        }

        [Command("clothingshop", Alias = "clothes")]
        public void Clothes(Player player, bool state)
        {
            if (state)
            {
                Random r = new Random();
                player.Dimension = Convert.ToUInt32(r.Next(0, 5000));
                
                player.TriggerEvent("client:EditorCamera", true);
                player.TriggerEvent("client:ClothingShop", true);
                player.SetSharedData("player:Frozen", true);
            }
            else
            {
                player.Position = new Vector3(430f, -811.3f, 29.5f);
                player.Dimension = 0;
                player.TriggerEvent("client:DeleteCamera", true);
                player.TriggerEvent("client:ClothingShop", false);
                player.SetSharedData("player:Frozen", false);
                
            }
        }

        [RemoteEvent("server:CloseClothingShop")]
        public void CloseClothingShop(Player player)
        {
            player.Position = new Vector3(430f, -811.3f, 29.5f);
            player.Dimension = 0;
            player.SetSharedData("player:Frozen", false);
            player.TriggerEvent("client:DeleteCamera", true);
        }



        [RemoteEvent("server:TextureFromClient")]
        public void SetTexture(Player player, int slot, int texture)
        {
            ComponentVariation cn = new ComponentVariation();
            cn.Drawable = NAPI.Player.GetPlayerClothes(player, slot).Drawable;
            cn.Texture = texture;
            player.SetClothes(slot, cn.Drawable, cn.Texture);
        }

        [RemoteEvent("server:DrawableFromClient")]
        public void SetDrawable(Player player, int slot, int drawable)
        {
            ComponentVariation cn = NAPI.Player.GetPlayerClothes(player, slot);
            cn.Drawable = drawable;
            cn.Texture = NAPI.Player.GetPlayerClothes(player, slot).Texture;
            player.SetClothes(slot, drawable, 0);
        }
    }
}
