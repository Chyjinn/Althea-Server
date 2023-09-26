using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Inventory
{

    public class Item
    {
        public uint DBID { get; set; }
        public int OwnerID { get; set; }
        public int OwnerType { get; set; }
        public uint ItemID { get; set; }
        public int ItemSection { get; set; }
        public string ItemValue { get; set; }//itemvalue, json
        public int ItemAmount { get; set; }
        public bool Duty { get; set; }
        public int ItemSlot { get; set; }
        public bool InUse { get; set; }
        public Item(uint dbid, int ownerid, int ownertype, uint itemid, int itemsection, string itemvalue, int itemamount, bool duty, int itemslot)
        {
            DBID = dbid;
            OwnerID = ownerid;
            OwnerType = ownertype;
            ItemID = itemid;
            ItemSection = itemsection;
            ItemValue = itemvalue;
            ItemAmount = itemamount;
            Duty = duty;
            ItemSlot = itemslot;
            InUse = false;
        }
    }


    public class Entry
    {
        public uint ItemID { get; set; }//itemid
        public string Name { get; set; }//item neve
        public string Description { get; set; }//leírás, ha van megjelenítjük
        public int ItemType { get; set; }//felhasználás kezeléséhez kell majd, pl Weapon akkor úgy kezeljük
        public string ItemImage { get; set; }//lehet local, pl. src/img.png, vagy url
        public int MaxStack { get; set; }
        public Entry(uint id, string name, string desc, int type, string itemimage, int stack) 
        {
            ItemID = id;
            Name = name;
            Description = desc;
            ItemType = type;
            ItemImage = itemimage;
            MaxStack = stack;
        }

    }
}
