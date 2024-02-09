using GTANetworkAPI;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Cms;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Characters
{
    public class Editor : Script
    {
        [RemoteEvent("server:NewChar")]
        public static void NewChar(Player player)//új karakter
        {
            if (player.HasData("Player:AccID"))
            {
                uint accID = player.GetData<uint>("Player:AccID");
                player.TriggerEvent("client:DeleteCamera");
                player.TriggerEvent("client:hideCharScreen");
                player.TriggerEvent("client:SkyCam", true);
                SetupCharEditor(player, accID);
            }
        }


        [RemoteEvent("server:CharEdit")]
        public static void EditChar(Player player, uint charID)//meglévő karakter
        {
            if (player.HasData("Player:AccID"))
            {
                uint accID = player.GetData<uint>("Player:AccID");
                player.TriggerEvent("client:DeleteCamera");
                player.TriggerEvent("client:hideCharScreen");
                player.TriggerEvent("client:SkyCam", true);
                SetupCharEditor(player, accID, charID);
            }
        }

        public async static void SetupCharEditor(Player player, uint accID)//új karakter létrehozása
        {
            if (player.HasData("Player:AccID"))
            {
                uint characternum = await Data.GetNumberOfCharacters(accID);
                if (player.GetData<uint>("Player:CharSlots") > characternum)
                {
                    Character c = new Character(0, "", DateTime.Now, "", 0, 0f, 0f, 0f, 0f);
                    Appearance a = new Appearance(0, true, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 50, 50, 50, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                    c.Appearance = a;
                    

                    NAPI.Task.Run(() =>
                    {
                        Appearance.HandleCharacterAppearance(player, c);
                        player.Position = new Vector3(-812.2f, 175f, 76.75f);
                        player.Rotation = new Vector3(0f, 0f, 110f);


                        player.TriggerEvent("client:SkyCam", false);

                        NAPI.Task.Run(() =>
                        {
                            player.TriggerEvent("client:CharEdit", true);
                            player.TriggerEvent("client:EditorCamera");
                            player.TriggerEvent("client:LoadCharacterAppearance", c);
                            
                        }, 4500);

                    }, 3000);
                }
                else
                {
                    NAPI.Task.Run(() =>
                    {
                        player.SendChatMessage("AccID: "+accID+" - Betelt a karakter slotod. "+ player.GetData<uint>("Player:CharSlots").ToString() + " < " + characternum.ToString());
                        player.TriggerEvent("client:CharEdit", false);
                        player.TriggerEvent("client:DeleteCamera");
                        player.TriggerEvent("client:SkyCam", false);
                        Selector.ProcessCharScreen(player);
                    }, 1000);
                    
                }


            }
        }

        public async static void SetupCharEditor(Player player, uint accID, uint charID)//meglévő karakter betöltése
        {
            if (player.HasData("Player:AccID"))
            {
                Character c = await Data.LoadCharacterData(accID, charID);
                Appearance a = await Data.LoadCharacterAppearance(c);
                c.Appearance = a;

                NAPI.Task.Run(() =>
                {
                    player.Position = new Vector3(-812.2f, 175f, 76.75f);
                    player.Rotation = new Vector3(0f, 0f, 110f);


                    player.TriggerEvent("client:SkyCam", false);

                    NAPI.Task.Run(() =>
                    {
                        player.TriggerEvent("client:LoadCharacterAppearance", c);
                        player.TriggerEvent("client:CharEdit", true);
                        player.TriggerEvent("client:EditorCamera");
                        //player.PlayAnimation("nm@hands", "hands_up", 1);
                    }, 4500);

                }, 3000);
            }
        }

        [RemoteEvent("server:FinishEditing")]
        public async void FinishEditing(Player player, string c)
        {
            uint accID = player.GetData<uint>("Player:AccID");
            player.TriggerEvent("client:CharEdit", false);
            player.TriggerEvent("client:DeleteCamera");
            player.TriggerEvent("client:SkyCam", true);
            Character character = NAPI.Util.FromJson<Character>(c);

            if (character.Id != 0)//ha nem 0 akkor meglévő karakterről van szó
            {
                if (await Data.EditExistingCharacterInDatabase(player, character))
                {
                    NAPI.Task.Run(() =>
                    {
                        player.SetSharedData("Player:Frozen", false);
                        Selector.ProcessCharScreen(player);
                    }, 5000);
                }
                else
                {
                    player.SendChatMessage("Adatbázis hiba!");
                }
            }
            else//0 a karakter ID, szóval újat kell létrehoznunk.
            {
                //új karaktert hoz létre
                if (await Data.AddNewCharacterToDatabase(player, accID, character))
                {
                    NAPI.Task.Run(() =>
                    {
                        player.SetSharedData("Player:Frozen", false);
                        player.StopAnimation();
                        Selector.ProcessCharScreen(player);
                    }, 5000);
                }
                else
                {
                    player.SendChatMessage("Adatbázis hiba!");
                }
            }
        }

        [RemoteEvent("server:PlayTPose")]
        public static void TPose(Player player)
        {
            player.PlayAnimation("nm@hands", "hands_up", 1);
        }

        [RemoteEvent("server:RotateCharRight")]
        public static void RotateCharRight(Player player)
        {
            Vector3 rot = player.Rotation;
            rot.Z += 1f;
            player.Rotation = rot;
        }

        [RemoteEvent("server:RotateCharLeft")]
        public static void RotateCharLeft(Player player)
        {
            Vector3 rot = player.Rotation;
            rot.Z -= 1f;
            player.Rotation = rot;
        }

    }
}
