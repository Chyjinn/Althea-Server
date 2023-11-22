using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Vehicles
{
    class DealershipVehicle
    {
        public string Model { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public uint Limited { get; set; }
        public DateTime AvailableFrom { get; set; }
        public DateTime AvailableUntil { get; set; }
    }

    internal class Dealership : Script
    {
        List<DealershipVehicle> DealershipVehicles = new List<DealershipVehicle>();
        ColShape dealer;

        [ServerEvent(Event.ResourceStart)]
        public void InitiateLoading()
        {
            //dealer = NAPI.ColShape.Create2DColShape(-52.5f, -1102.5f, 25.5f, 15f);
            dealer = NAPI.ColShape.CreateSphereColShape(new Vector3(-44.2f, -1098f, 26.5f), 8f);

            //dealer = NAPI.ColShape.Create3DColShape(new Vector3(-35f, -1108f, 20f), new Vector3(-51f, -1089f, 31f),0);
        }

        [Command("dealership")]
        public void OpenDealership(Player player)
        {

        }
    }
}
