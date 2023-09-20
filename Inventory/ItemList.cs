using GTANetworkAPI;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Server.Inventory
{
    public class ItemList : Script
    {
        static List<Entry> itemList = new List<Entry>();
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
            string query = $"SELECT * FROM `itemlist`";
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
                                Entry entry = new Entry(Convert.ToInt32(reader["itemID"]), Convert.ToString(reader["itemName"]), Convert.ToString(reader["itemDescription"]), (Entry.ItemType)Convert.ToUInt32(reader["itemType"]), Convert.ToString(reader["itemImage"]), Convert.ToInt32(reader["itemStack"]));
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

        public Entry GetItemById(uint id)
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

    }
}
