using GTANetworkAPI;
using GTANetworkMethods;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Server.AnimSelector
{
    internal class AnimSelector : Script
    {
        [Command("animselector")]
        public void animSelector(GTANetworkAPI.Player player)
        {
            GTANetworkAPI.Player p = player;
            player.TriggerEvent("toggleSelectorWindow");            
            
        }
    }
}
