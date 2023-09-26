using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTANetworkAPI;
using MySql.Data.MySqlClient;
using Server.Characters;

namespace Server.Inventory
{
    public class Items : Script
    {
        static List<Item> ServerItems = new List<Item>();

        public static Item[] GetPlayerItemsByCharacterId(int characterId)
        {
            List<Item> playerItems = new List<Item>();
            foreach (var item in ServerItems)
            {
                if (item.OwnerType == 0 && item.OwnerID == characterId)
                {
                    playerItems.Add(item);
                }
            }
            return playerItems.ToArray();
        }


        public static void LoadInventory(Player player)
        {
            uint charid = player.GetData<UInt32>("player:charID");
            RefreshInventory(player, charid);
        }

        [Command("giveitem")]
        public void GiveItem(Player player, int itemid, int amount)
        {
           
        }


        public async static void RefreshInventory(Player player,uint charid)
        {
            Item[] playerItems = await GetPlayerInventory(charid);
            
            foreach (var item in playerItems)
            {
                if (!ServerItems.Contains(item))
                {
                    ServerItems.Add(item);
                }
            }
            Inventory.ItemList.SendItemListToPlayer(player);
            SendInventoryToPlayer(player, playerItems);
        }


        public async static void SendInventoryToPlayer(Player player, Item[] items)
        {
            NAPI.Task.Run(() =>
            {
                string json = NAPI.Util.ToJson(items);
                player.TriggerEvent("client:InventoryFromServer", json);
                Database.Log.Log_Server(json);
            }, 500);


        }


        public static async Task<Item[]> GetPlayerInventory(uint charid)//felhasználónév alapján adja vissza az adatokat, ha nincs ilyen akkor üres string tömböt
        {
            string query = $"SELECT * FROM `items` WHERE `ownerType` = 0 AND `ownerID` = @CharacterID";
            List<Item> items = new List<Item>();
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = Database.DBCon.GetConString();
                try
                {
                    await con.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@CharacterID", charid);
                        cmd.Prepare();
                        try
                        {
                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    Item loadedItem = new Item(Convert.ToUInt32(reader["DbID"]), Convert.ToInt32(reader["ownerID"]), Convert.ToInt32(reader["ownerType"]), Convert.ToUInt32(reader["itemID"]),reader["itemValue"].ToString(),Convert.ToInt32(reader["itemAmount"]),Convert.ToBoolean(reader["duty"]),Convert.ToInt32(reader["itemSlot"]));
                                    items.Add(loadedItem);

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
                await con.CloseAsync();
                return items.ToArray();
            }
        }


        public void CleanUpInventory(int characterID)
        {
            //playerInventories[characterID].Clear();
        }


        public void AddItemToCharacter(int characterId, Item item)
        {
            //playerInventories[characterId].Add(item);
        }

        public Item GetItemByData(uint ownerid, int section, int slot)
        {
            foreach (var item in ServerItems)
            {
                if (item.OwnerID == ownerid && ItemList.GetItemSection(item.ItemID) == section && item.ItemSlot == slot)
                {
                    return item;
                }
            }
            return null;
        }


        [RemoteEvent("server:UseItem")]
        public void ItemUse(Player player, int section, int slot)
        {
            uint charid = player.GetData<UInt32>("player:charID");
            Item i = GetItemByData(charid, section, slot);
            if (i != null)
            {
                HandleItemUse(player,i);
            }
            else//hiba van, frissítjük a player inventoryját
            {
                RefreshInventory(player, charid);
            }
        }


        [RemoteEvent("server:MoveItemInInventory")]
        public void MoveItemInInventory(Player player, int section, int startslot, int endslot)
        {
            uint charid = player.GetData<UInt32>("player:charID");
            player.SendChatMessage("item move " + startslot + "->" + endslot);
            Item i = GetItemByData(charid, section, startslot);
            if (i != null)
            {
                player.SendChatMessage("item not null");
                if (TryItemMove(player,section,startslot,endslot))
                {
                    //adatbázis kezelés
                    player.SendChatMessage(ItemList.GetItemName(i.ItemID) + " áthelyezve, kezdő slot: " + startslot + " cél slot: " + endslot);
                }
                else
                {

                    player.SendChatMessage("item destination not empty");
                }
            }
            else//hiba van, frissítjük a player inventoryját
            {
                player.SendChatMessage("refresh inv");
                RefreshInventory(player, charid);
            }
        }


        public bool TryItemMove(Player player,int section, int startslot, int endslot)
        {
            uint charid = player.GetData<UInt32>("player:charID");

            if (GetItemByData(charid, section, endslot) == null)
            {
                Item i = GetItemByData(charid, section, startslot);
                i.ItemSlot = endslot;
                return true;
            }

            return false;

        }



        public void HandleItemUse(Player player, Item i)
        {
            if (i.InUse)//használatban van, el akarjuk tenni
            {
                switch (ItemList.GetItemType(i.ItemID))
                {
                    case 1://fegyver
                        player.RemoveWeapon(WeaponHash.Pistol);
                        i.InUse = false;
                        player.TriggerEvent("client:ItemUseToCEF", ItemList.GetItemSection(i.ItemID), i.ItemSlot, 0);
                        break;
                    case 2:
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (ItemList.GetItemType(i.ItemID))
                {
                    case 1://fegyver
                        player.GiveWeapon(WeaponHash.Pistol, 50);
                        i.InUse = true;
                        player.TriggerEvent("client:ItemUseToCEF",ItemList.GetItemSection(i.ItemID),i.ItemSlot, 1);
                        break;
                    case 2:
                        break;
                    default:
                        break;
                }
            }


            
        }
    }
}
