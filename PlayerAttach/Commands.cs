using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTANetworkAPI;
using GTANetworkMethods;

namespace Server.PlayerAttach
{
    internal class Commands: Script
    {
        Dictionary<GTANetworkAPI.Player, List<GTANetworkAPI.Object>> objDic = new Dictionary<GTANetworkAPI.Player, List<GTANetworkAPI.Object>>();
        List<GTANetworkAPI.Object> tempObjList = new List<GTANetworkAPI.Object>();
        [Command("cp")]
        public void createCp(GTANetworkAPI.Player client)
        {
            var clientPosition = NAPI.Entity.GetEntityPosition(client);
            client.TriggerEvent("client:getGroundHeight", getPlayerPos(client));            
            NAPI.Chat.SendChatMessageToAll(clientPosition.ToString()); //Teszt miatt kell
                        
        }
        [Command("box")]
        public void createBox(GTANetworkAPI.Player client)
        {
            var obj = NAPI.Object.CreateObject(2930714276, client.Position, client.Rotation, 255, 0);
            foreach (var item in objDic)
            {
                tempObjList.Add(obj);
                if (item.Key != client)
                {                    
                    objDic.Add(client, tempObjList);
                }
                else
                {
                    objDic[client] = tempObjList;
                }
            }
            client.TriggerEvent("client:getBoxDictionary", client, obj);
            //client.TriggerEvent("client:createBox", client);
        }
        [RemoteEvent("server:getBoxDictionary")]
        public void getBoxDictionary()
        {
            Dictionary<GTANetworkAPI.Player, List<GTANetworkAPI.Object>> objDictionary = objDic;
        }


        [RemoteEvent("server:getGroundHeight")]
        public void getGroundHeight(GTANetworkAPI.Player client, float lastArgsToVectorZ)
        {
            NAPI.Chat.SendChatMessageToAll("Lefut a szerver" + lastArgsToVectorZ);
            var clientPosition = NAPI.Entity.GetEntityPosition(client);
            //azért teszem ide, hogy jó legyen
            clientPosition.Z = lastArgsToVectorZ;
            NAPI.Checkpoint.CreateCheckpoint(CheckpointType.CylinderCheckerboard, clientPosition, new Vector3(0, 1, 0), 1f, new Color(255, 0, 0), 0);
        }

        public Vector3 getPlayerPos(GTANetworkAPI.Player client)
        {
            var playerPos = NAPI.Entity.GetEntityPosition(client);
            return playerPos;
        }
        [ServerEvent(Event.PlayerDisconnected)]
        
        public void onPlayerDisconnect(GTANetworkAPI.Player client, DisconnectionType type, string reason)
        {
            foreach (var item in objDic)
            {
                if(item.Key == client)
                {                    
                    foreach (var tempObj in tempObjList)
                    {
                        NAPI.Entity.DeleteEntity(tempObj);
                    }
                }
            }
        }


    }
    
}
