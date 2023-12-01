using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Inventory
{

    public class Item
    {
        public uint DBID { get; set; }
        public uint OwnerID { get; set; }
        public int OwnerType { get; set; }
        public uint ItemID { get; set; }
        public string ItemValue { get; set; }//itemvalue, json
        public int ItemAmount { get; set; }
        public bool InUse { get; set; }
        public bool Duty { get; set; }
        public int Priority { get; set; }
        public Item(uint dbid, uint ownerid, int ownertype, uint itemid, string itemvalue, int itemamount, bool inuse, bool duty, int priority)
        {
            DBID = dbid;
            OwnerID = ownerid;
            OwnerType = ownertype;
            ItemID = itemid;
            ItemValue = itemvalue;
            ItemAmount = itemamount;
            Duty = duty;
            Priority = priority;
            InUse = inuse;
        }
    }

    public class Clothing
    {
        public int Drawable { get; set; }
        public int Texture { get; set; }
        public Clothing(int drawable, int texture)
        {
            this.Drawable = drawable;
            this.Texture = texture;
        }
    }

    public class Top : Clothing
    {
        public int UndershirtDrawable { get; set; }
        public int UndershirtTexture { get; set; }
        public int Torso { get; set; }

        public Top(int drawable, int texture, int undershirtdraw, int undershirttext, int torso) : base(drawable, texture)
        {
            this.UndershirtDrawable = undershirtdraw;
            this.UndershirtTexture = undershirttext;
            this.Torso = torso;
        }
    }


    public class Entry
    {
        public uint ItemID { get; set; }//itemid
        public string Name { get; set; }//item neve
        public string Description { get; set; }//leírás, ha van megjelenítjük
        public int ItemType { get; set; }//felhasználás kezeléséhez kell majd, pl Weapon akkor úgy kezeljük
        public string ItemImage { get; set; }//lehet local, pl. src/img.png, vagy url
        public uint ItemWeight { get; set; }
        public string Object { get; set; }
        public bool Stackable { get; set; }
        public Entry(uint id, string name, string desc, int type, uint weight, string itemimage, string obj, bool stack)
        {
            ItemID = id;
            Name = name;
            Description = desc;
            ItemType = type;
            ItemWeight = weight;
            ItemImage = itemimage;
            Object = obj;
            Stackable = stack;
        }

    }
}
