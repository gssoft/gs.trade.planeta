using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarCreator;
using GS.Events;

namespace CaBarCreator
{
    class Program3
    {

        private static void Main(string[] args)
        {
            var barCntxt = new BarCreatorContext();
            barCntxt.Init();

            var eventHubTask = barCntxt.WorkTasks.GetByKey("EventHubReceiver");
            var randomTickerPusher = barCntxt.WorkTasks.GetByKey("RandomTickPusher");
            var ddeTickReceiver = barCntxt.WorkTasks.GetByKey("TickerTickReceiver");

            

        }
    }
}
