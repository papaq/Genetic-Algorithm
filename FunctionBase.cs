using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenCon
{
    /// <summary>
    /// This is the base class for others Function classes
    /// </summary>
    public class FunctionBase
    {
        // Function delegate
        public delegate double FunctionCalculator(double[] listOfVariables);
    }
}
