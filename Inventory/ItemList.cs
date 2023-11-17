using GTANetworkAPI;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Utilities;
using Server.Interior;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Server.Inventory
{
    public class ItemList : Script
    {
        static List<Entry> itemList = new List<Entry>();

        //betöltjük adatbázisból az itemlistát
        public async static void InitiateItemList()
        {

            DateTime timestamp1 = DateTime.Now;

            await LoadItemList();

            DateTime timestamp2 = DateTime.Now;

            TimeSpan LoadTime = timestamp2 - timestamp1;

            NAPI.Util.ConsoleOutput("Itemlista betöltve " + LoadTime.Milliseconds + " ms alatt.");

        }

        [Command("refreshitemlist")]
        public async void RefreshItemList(Player player)
        {
            DateTime timestamp1 = DateTime.Now;

            await LoadItemList();

            DateTime timestamp2 = DateTime.Now;

            TimeSpan LoadTime = timestamp2 - timestamp1;
            NAPI.Task.Run(() =>
            {
                player.SendChatMessage("Itemlista újratöltve " + LoadTime.Milliseconds + " ms alatt.");
                NAPI.Util.ConsoleOutput("Itemlista betöltve " + LoadTime.Milliseconds + " ms alatt.");

                foreach (var item in NAPI.Pools.GetAllPlayers())
                {
                    string json = NAPI.Util.ToJson(itemList);
                    item.TriggerEvent("client:ItemListFromServer", json);
                    Items.LoadInventory(item);
                }
                
            }, 500);

        }

        public async static Task<bool> LoadItemList()
        {
            string query = $"SELECT * FROM `itemlist`";
            using (MySqlConnection con = new MySqlConnection())
            {
                try
                {
                    con.ConnectionString = await Database.DBCon.GetConString();
                    con.Open();

                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        cmd.Prepare();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                Entry entry = new Entry(Convert.ToUInt32(reader["itemID"]), Convert.ToString(reader["itemName"]), Convert.ToString(reader["itemDescription"]), Convert.ToInt32(reader["itemType"]), Convert.ToUInt32(reader["itemWeight"]), Convert.ToString(reader["itemImage"]), Convert.ToBoolean(reader["stackable"]));
                                itemList.Add(entry);
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    Database.Log.Log_Server(ex.ToString());
                }

                con.Close();

            }
            return true;
        }

        [RemoteEvent("server:RefreshItemList")]
        public async static void SendItemListToPlayer(Player player)
        {
            NAPI.Task.Run(() =>
            {
                string json = NAPI.Util.ToJson(itemList);
                player.TriggerEvent("client:ItemListFromServer", json);
            }, 500);


        }

        public static Entry GetItemById(uint id)
        {
            foreach (var item in itemList)
            {
                if (item.ItemID == id)
                {
                    return item;
                }
            }

            return null;
        }

        public static int GetItemType(uint itemid)
        {
            foreach (var item in itemList)
            {
                if (item.ItemID == itemid)
                {
                    return item.ItemType;
                }
            }
            return 0;
        }

        public static string GetItemName(uint itemid)
        {
            foreach (var item in itemList)
            {
                if (item.ItemID == itemid)
                {
                    return item.Name;
                }
            }
            return "Nem létező item.";
        }

    }
}
