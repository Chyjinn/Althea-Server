using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Jobs
{
    class TaxiCall
    {
        public int CallID;
        public string CallLocation;
        public DateTime CallTime;
        public string CallerName;
        public string CallerNumber;
        public Player Caller;
        public bool Active;
        public TaxiCall(Player player, int callid, string location, DateTime timestamp, string callername, string callernumber)
        {
            this.CallLocation = location;
            this.CallID = callid;
            this.CallTime = timestamp;
            this.CallerName = callername;
            this.CallerNumber = callernumber;
            this.Caller = player;
            this.Active = true;
        }


        public void SetCallState(bool state)
        {
            this.Active = state;
        }
    }


    internal class Taxi : Script
    {
        List<TaxiCall> calls = new List<TaxiCall>();
        int callid = 1;
        public TaxiCall[] GetActiveCalls()
        {
            List<TaxiCall> c = new List<TaxiCall>();
            foreach (var item in calls)
            {
                if (item.Active)
                {
                    c.Add(item);
                }
            }
            return c.ToArray();
        }


        [Command("taxilight")]
        public void TaxiLight(Player player)
        {
            player.TriggerEvent("client:TaxiLight");
        }

        [Command("calltaxi")]
        public void TempTaxi(Player player)
        {
            player.TriggerEvent("client:CallTaxi");
        }

        [RemoteEvent("server:CallTaxi")]
        public void CallTaxi(Player player, string street, string zone)
        {
            TaxiCall call = new TaxiCall(player, callid, street + " - " + zone, DateTime.Now, player.Name, "06 70 202 88 25");
            callid++;
            calls.Add(call);
            player.SendChatMessage("Taxit hívtál!");
            
        }

        [Command("getcalls")]
        public void GetCalls(Player player)
        {
            player.SendChatMessage("Taxi hívások:");
            TaxiCall[] c = GetActiveCalls();
            foreach (var item in c)
            {
                player.SendChatMessage(item.CallLocation + " - " + item.CallerName + " - " + item.CallTime);
            }
        }
    }
}
