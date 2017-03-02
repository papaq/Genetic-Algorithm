namespace GenCon
{
    /// <summary>
    /// The class contains basic data for all functions
    /// </summary>
    internal class FunctionsInputData
    {
        public struct Interval
        {
            public double LeftBorder;
            public double RightBorder;
        };

        // Array of functions' intervals
        private Interval[] _intervals;

        // Array of functions' Global optimum
        private double[] _glOptimum;

        // Array of numbers of variables in a function
        private int[] _numberOfVars;

        private bool[] _min;

        public FunctionsInputData()
        {
            FillIntervals();
            FillGlobalOptimum();
            FillNumberOfVars();
            FillMaxOrMin();
        }

        public Interval GetInterval(int idx)
        {
            return _intervals[idx];
        }

        public double GetGlobalOptimum(int idx)
        {
            return _glOptimum[idx];
        }

        public int GetNumOfVars(int idx)
        {
            return _numberOfVars[idx];
        }

        public bool IsFuncMin(int idx)
        {
            return _min[idx];
        }

        private void FillIntervals()
        {
            _intervals = new[]
            {
                AddInterval(-2.048, 2.048),
                AddInterval(-2, 2),
                AddInterval(-5, 10),
                AddInterval(0, 10),
                AddInterval(-10, 10),
                AddInterval(-1.2, 1.2),
                AddInterval(-2.048, 2.048),
                AddInterval(-5.12, 5.12),
                AddInterval(-512, 512),
            };
        }

        private void FillGlobalOptimum()
        {
            _glOptimum = new[]
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
            _numberOfVars = new[]
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

        private void FillMaxOrMin()
        {
            _min = new[]
            {
                false,
                true,
                true,
                true,
                true,
                true,
                true,
                false,
            };
        }

        private static Interval AddInterval(double left, double right)
        {
            return new Interval() { LeftBorder = left, RightBorder = right };
        }
    }
}
