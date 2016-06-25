using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenCon
{
    /// <summary>
    /// The class contains basic data for all functions
    /// </summary>
    class FunctionsInputData
    {
        public struct Interval
        {
            public double intervalLeft;
            public double intervalRight;
        };

        // Array of functions' intervals
        private Interval[] intervals;

        // Array of functions' Global optimum
        private double[] glOptimum;

        // Array of numbers of variables in a function
        private int[] numberOfVars;

        public FunctionsInputData()
        {
            FillIntervals();
            FillGlobalOptimum();
        }

        public Interval GetInterval(int idx)
        {
            return intervals[idx];
        }

        public double GetGlobalOptimum(int idx)
        {
            return glOptimum[idx];
        }

        public double GetNumOfVars(int idx)
        {
            return numberOfVars[idx];
        }

        private void FillIntervals()
        {
            intervals = new Interval[]
            {
                AddInterval(-2.048, 2.048),
                AddInterval(-2, 2),
                AddInterval(-5, 10),
                AddInterval(0, 10),
                AddInterval(-10, 10),
                AddInterval(-1.2, 1.2),
                AddInterval(-5.12, 5.12),
                AddInterval(-512, 512),
            };
        }

        private void FillGlobalOptimum()
        {
            glOptimum = new double[]
            {
                3905.93,
                3,
                0.3977272,
                0,
                0,
                0,
                0,
                10,
            };
        }

        private void FillNumberOfVars()
        {
            numberOfVars = new int[]
            {
                2,
                2,
                2,
                2,
                2,
                4,
                6,
                10,
            };
        }

        private Interval AddInterval(double left, double right)
        {
            return new Interval() { intervalLeft = left, intervalRight = right };
        }
    }
}
