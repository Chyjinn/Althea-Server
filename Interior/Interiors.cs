using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Interior
{
    internal class Interiors : Script
    {
        //CHAR: -811.68, 175.2, 76.74, 0, 0, 109.73
        //CAM: -813.95, 174.2, 76.78, 0, 0, -69
        [Command("interior")]
        public void InteriorTest(Player player)
        {
            Vector3 pos = player.Position;
            pos.Z = pos.Z - 1f;
            Checkpoint cp = NAPI.Checkpoint.CreateCheckpoint(CheckpointType.Cyclinder3, pos, new Vector3(0, 1, 0), 1f, new Color(255, 255, 255, 100));
        }


        [Command("skycam")]
        public void SkyCam(Player player, int state)
        {
            player.TriggerEvent("client:CamToAir", state);
        }
    }
}
