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
            int charid = player.GetData<int>("Player:CharID");
            RefreshInventory(player, charid);
        }

        [Command("giveitem")]
        public void GiveItem(Player player, int itemid, int amount)
        {
           
        }


        public async static void RefreshInventory(Player player,int charid)
        {
            Item[] playerItems = await GetPlayerInventory(charid);
            player.SendChatMessage("ITEMID: " + playerItems[0].ItemID.ToString());
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
            string json = NAPI.Util.ToJson(items);
            player.TriggerEvent("client:InventoryFromServer", items);
        }


        public static async Task<Item[]> GetPlayerInventory(int charid)//felhasználónév alapján adja vissza az adatokat, ha nincs ilyen akkor üres string tömböt
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
                                if (await reader.ReadAsync())
                                {
                                    Item loadedItem = new Item(Convert.ToUInt32(reader["DbID"]), Convert.ToInt32(reader["ownerID"]), Convert.ToInt32(reader["ownerType"]), Convert.ToInt32(reader["itemID"]),reader["itemValue"].ToString(),Convert.ToInt32(reader["itemAmount"]),Convert.ToBoolean(reader["duty"]),Convert.ToInt32(reader["itemSlot"]));
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
    }
}
