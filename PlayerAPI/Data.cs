using System;
using GTANetworkAPI;

namespace Server.PlayerAPI
{
    class Data
    {
        public static readonly String DataIdentifier = "PlayerInfo";
        public Player PlayerData { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
        public int Cash { get; set; }
        public int Age { get; set; }
        public int Health { get; set; }
        public int Armor { get; set; }

       
        public Data(Player player)
        {
            this.PlayerData = player;
            this.Name = player.Name;
            this.Cash = 0;
            this.Age = 0;
            this.Health = 0;
            this.Armor = 0;
        }

        public void SetHealth(int health)
        {
            this.Health = health;
            this.PlayerData.Health = health;
        }

    }
}
