using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Inventory
{
    internal class Item
    {
        public enum ItemType
        {
            Weapon,
            Magazine,
            Consumable
        }
        public uint ID { get; set; }//itemid
        public string Name { get; set; }//item neve
        public string Description { get; set; }//leírás, ha van megjelenítjük
        public ItemType Type { get; set; }//felhasználás kezeléséhez kell majd, pl Weapon akkor úgy kezeljük
        public string ItemImage { get; set; }//lehet local, pl. src/img.png, vagy url
        public int MaxStack { get; set; }
        public Item(uint id, string name, string desc, ItemType type, string itemimage, int stack) 
        {
            ID = id;
            Name = name;
            Description = desc;
            Type = type;
            ItemImage = itemimage;
            MaxStack = stack;
        }

    }
}
