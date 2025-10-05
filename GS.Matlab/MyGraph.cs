using System;
using System.Runtime.Remoting.Messaging;


namespace GS.Matlab
{
    // Call Methods from .Net Assembly from Matlab 
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
            Double[] doubleArr =
            {
                18, 32, 3.133, 44, -9.9, -13, 33.03
            };
            String xLabel = "X-Axis Label";
            Object[] plotData = {xLabel, doubleArr};
            Object[] containerArr = {fLabel, plotData};
            return containerArr;
        }

        public Object[] getNewDataProp {
        get
        {
            String fLabel = "Figure Showing New Graph Data";
            Double[] doubleArr =
            {
                18, 32, 3.133, 44, -9.9, -13, 33.03
            };
            String xLabel = "X-Axis Label";
            Object[] plotData = {xLabel, doubleArr};
            Object[] containerArr = {fLabel, plotData};
            return containerArr;
        }
        }

    public object[] getObjectArray()
        {
            var arr1 = new double[] {1, 2, 3, 4, 5};
            var arr2 = new double[] { 6, 7, 8, 9, 10 };
            return new object[] {arr1, arr2};
        }
        public double[] getDoubleArray()
        {
            return new double[] { 1, 2, 3, 4, 5 };
        }

        public object[] getObjectArrayProp => new object[]
        {
            new double[] {1,2,3},
            new double[] {4,5,6,7,8,9}
        };
        public double[] getDoubleArrayProp => new double[] { 1, 2, 3, 4, 5 };
    }
}
