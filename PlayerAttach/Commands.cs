using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTANetworkAPI;
using GTANetworkMethods;

namespace Server.PlayerAttach
{
    internal class Commands : Script
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
            tempObjList.Add(obj);
            client.PlayAnimation("anim@amb@business@coc@coc_packing_hi@", "base_foldedbox", 1);
            client.SetSharedData("anim", "anim@amb@business@coc@coc_packing_hi@");
            if (objDic.ContainsKey(client))
            {
                objDic[client] = tempObjList;
            }
            else
            {
                objDic.Add(client, tempObjList);
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
                //Data.Log.Log_Server("onPlayerDisconnet lefut a foreach");
                if (item.Key == client)
                {
                    //Data.Log.Log_Server("onPlayerDisconnet lefut az IF-ben");
                    foreach (var tempObj in tempObjList)
                    {
                        //Data.Log.Log_Server("onPlayerDisconnet lefut a második foreach");
                        NAPI.Entity.DeleteEntity(tempObj);
                    }
                }
            }
        }

        /*[RemoteEvent("server:stopAnim")]
        public void stopAnim(GTANetworkAPI.Player client)
        {
            if (NAPI.Data.HasEntitySharedData(client, "anim"))
            {
                var val = NAPI.Data.GetEntitySharedData(client, "anim");
                Data.Log.Log_Server("val: " + val);
                if (val != "")
                {
                    client.StopAnimation();
                    NAPI.Data.SetEntitySharedData(client, "anim", "");
                }
            }
        }*/


    }
    
}
