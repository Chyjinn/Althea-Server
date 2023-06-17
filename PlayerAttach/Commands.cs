using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTANetworkAPI;
using GTANetworkMethods;
using Org.BouncyCastle.Bcpg;

namespace Server.PlayerAttach
{
    internal class Commands: Script
    {
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
            client.TriggerEvent("client:createBox", client);
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

    }
    
}
