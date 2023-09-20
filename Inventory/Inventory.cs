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
    public class Inventory : Script
    {
        static Dictionary<int, List<Item>> playerInventories = new Dictionary<int, List<Item>>();//character id, list of items
        Dictionary<int, List<Item>> vehicleInventories = new Dictionary<int, List<Item>>();

        public List<Item> GetPlayerItemsByCharacterId(int characterId)
        {
            return playerInventories[characterId];
        }


        public static void LoadPlayerInventory(Player player)
        {
            int charid = player.GetData<int>("Player:CharID");
            LoadInventory(player,charid);
        }

        public async static void LoadInventory(Player player,int charid)
        {
            Item[] items = await GetPlayerInventory(charid);
            playerInventories[charid] = items.ToList();
            SendInventoryToPlayer(player, items);
        }


        public async static void SendInventoryToPlayer(Player player, Item[] items)
        {

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
            playerInventories[characterID].Clear();
        }


        public void AddItemToCharacter(int characterId, Item item)
        {
            playerInventories[characterId].Add(item);
        }
    }
}
