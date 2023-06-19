using Google.Protobuf.Collections;
using GTANetworkAPI;
using GTANetworkMethods;
using MySqlX.XDevAPI;
using RAGE;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.BoxCarrier
{    
    internal class Commands: Script
    {        
        Dictionary<GTANetworkAPI.Player, List<GTANetworkAPI.Object>> objDic = new Dictionary<GTANetworkAPI.Player, List<GTANetworkAPI.Object>>();
        List<GTANetworkAPI.Object> objects = new List<GTANetworkAPI.Object> ();

        [Command("drawbox")]
        public void drawBox(GTANetworkAPI.Player player, bool startStop, string cpType)
        {
            NAPI.Chat.SendChatMessageToAll("megy a drawbox");
            if (startStop)
            {
                player.TriggerEvent("client:drawBox", cpType);
            }
            else
            {
                player.TriggerEvent("client:delDrawBox", cpType);
            }
        }

        [Command("destroyobj")]
        public void destroyObjByHandle(GTANetworkAPI.Player player, string handle, string rotX, string rotY, string rotZ)
        {
            int objHandle = Convert.ToInt32(handle);
            player.TriggerEvent("client:destroyObjByHandle", objHandle, rotX, rotY, rotZ);
        }

        [Command("boxcarrier")]
        public void boxCarrying(GTANetworkAPI.Player player)
        {            
            player.PlayAnimation("anim@heists@box_carry@", "idle", 49);
            player.TriggerEvent("client:attachBox");            

        }        

        [RemoteEvent("server:boxCreation")]
        public void boxCreation(GTANetworkAPI.Player player)
        {
            /*var obj = NAPI.Object.CreateObject(2930714276, player.Position, player.Rotation, 255, 0);
            objects.Add(obj);
            if (objDic.ContainsKey(player))
            {
                objDic[player] = objects;
            }
            else
            {
                objDic.Add(player, objects);
            }
            NAPI.Chat.SendChatMessageToAll("Létre kellene jönnie az objektnek.");
            player.SetSharedData("objectEntity", obj.Id);                        */

        }        

        [ServerEvent(Event.PlayerDisconnected)]

        public void onPlayerDisconnect(GTANetworkAPI.Player client, DisconnectionType type, string reason)
        {
            /*foreach (var item in objDic)
            {
                Data.Log.Log_Server("onPlayerDisconnet lefut a foreach");
                if (item.Key == client)
                {
                    //Data.Log.Log_Server("onPlayerDisconnet lefut az IF-ben");
                    foreach (var tempObj in objects)
                    {
                        //Data.Log.Log_Server("onPlayerDisconnet lefut a második foreach");
                        NAPI.Entity.DeleteEntity(tempObj);
                    }
                }
            }*/
            client.TriggerEvent("client:destroyBoxes");
            //NAPI.ClientEvent.TriggerClientEvent(client, "client:destroyBoxes");
        }
    }
}
