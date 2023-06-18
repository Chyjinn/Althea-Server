using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTANetworkAPI;
using GTANetworkMethods;
namespace Server.Animator
{
    internal class Commands : Script
    {
        [Command("animator")]
        public void playAnimator(GTANetworkAPI.Player player)
        {
            NAPI.Chat.SendChatMessageToAll("Anyád", "Megy a parancs");
            //player.TriggerEvent("client:playAnimator");
            //player.SetSharedData("isInAnimator", "true");
            player.TriggerEvent("client:bindRightArrow");
        }        
    }
}
