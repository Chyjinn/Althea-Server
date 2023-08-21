﻿using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Characters
{
    class Editor
    {
        //CHAR: -811.68, 175.2, 76.74, 0, 0, 109.73
        //CAM: -813.95, 174.2, 76.78, 0, 0, -69
        [Command("chatedit", Alias = "chareditor")]
        public void CharEdit(Player player)
        {
            player.SetSharedData("player:Frozen", true);
            player.Position = new Vector3(-811.68f, 175.2f, 76.74f);
            player.Rotation = new Vector3(0f, 0f, 110f);
            player.TriggerEvent("client:SetCamera", -814.3f, 174.1f, 77f, -10f, 0f, -72f, 48f);
            player.TriggerEvent("client:CharEdit");
            
        }

    }
}
