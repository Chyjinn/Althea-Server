using GTANetworkAPI;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Server.Inventory
{
    internal class ItemList
    {
        static List<Item> itemList = new List<Item>();
        //betöltjük adatbázisból az itemlistát
        [ServerEvent(Event.ResourceStart)]
        public async void InitiateLoading()
        {
            DateTime timestamp1 = DateTime.Now;
           
            await LoadItemList();

            DateTime timestamp2 = DateTime.Now;

            TimeSpan LoadTime = timestamp2 - timestamp1;
            
            NAPI.Util.ConsoleOutput("Itemlista betöltve " + LoadTime.Milliseconds + " ms alatt.");
        }


        public static async Task LoadItemList()
        {
            string query = $"SELECT * FROM `items`";
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = Database.DBCon.GetConString();
                await con.OpenAsync();

                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    try
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {

                                Item entry = new Item(Convert.ToUInt32(reader["itemID"]), Convert.ToString(reader["itemName"]), Convert.ToString(reader["itemDescription"]), (Item.ItemType)Convert.ToUInt32(reader["itemType"]), Convert.ToString(reader["itemImage"]), Convert.ToInt32(reader["itemStack"]));
                                itemList.Add(entry);
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
        }

        public Item GetItemById(uint id)
        {
            foreach (var item in itemList)
            {
                if (item.ID == id)
                {
                    return item;
                }
            }

            return null;
        }

    }
}
