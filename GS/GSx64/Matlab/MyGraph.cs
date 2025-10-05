using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS.Matlab
{
    public class MyGraph
    {
        public Object[] getNewData()
        /*
         * Create a System.Object array to use in MATLAB examples.
         * Returns containerArr System.Object array containing:
         *  fLabel System.String object
         *  plotData System.Object array containing:
         *      xLabel System.String object
         *      doubleArr System.Double array
        */
        {
            String fLabel = "Figure Showing New Graph Data";
            Double[] doubleArr = {
                                18, 32, 3.133, 44, -9.9, -13, 33.03 };
            String xLabel = "X-Axis Label";
            Object[] plotData = { xLabel, doubleArr };
            Object[] containerArr = { fLabel, plotData };
            return containerArr;
        }
    }
}
