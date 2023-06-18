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
        public void playAnimator(GTANetworkAPI.Player player, bool valami)
        {
            if (valami)
            {
                NAPI.Chat.SendChatMessageToAll("Anyád", "Megy a parancs");
                //player.TriggerEvent("client:playAnimator");
                //player.SetSharedData("isInAnimator", "true");
                player.TriggerEvent("client:bindRightArrow");                

            }
            else
            {
                player.TriggerEvent("client:unbindRightArrow");
            }
            
        }

        [RemoteEvent("server:playAnim")]

        public void playAnim(GTANetworkAPI.Player player, string animDic, string animName)
        {
            player.PlayAnimation(animDic, animName, 1);
        }

        [RemoteEvent("server:stopAnim")]
        public void stopAnim(GTANetworkAPI.Player player)
        {            
            player.StopAnimation();
        }
        //Ha azt akarod, hogy az anim bekapcsolva maradjon de tudj sétálni akkor használd a 49-es flag-et.
        [Command("anim")]
        public void anim(GTANetworkAPI.Player player, string animDict, string animName, int flag)
        {
            player.PlayAnimation(animDict, animName, flag);            
        }
        [Command("stopanim")]
        public void stopAnimCommand(GTANetworkAPI.Player player)
        {
            player.StopAnimation();
        }
    }
}
