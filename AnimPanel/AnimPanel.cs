using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.AnimPanel
{
    internal class AnimPanel : Script
    {
        [Command("animpanel")]
        public void animPanel(GTANetworkAPI.Player player)
        {
            player.TriggerEvent("toggleAnimPanel");
        }
    }
}
