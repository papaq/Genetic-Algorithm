using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenCon
{
    /// <summary>
    /// This class contains data about this very Function
    /// </summary>
    public class Function : FunctionBase
    {
        // Interval
        protected double intervalLeft, intervalRange;

        // Number of variables
        protected int numOfVariables;

        // Global optimum
        protected double globalOptimum;

        // Function itself
        private FunctionCalculator functionCalc;

        /// <summary>
        /// Constructor declaration
        /// </summary>
        /// <param name="left">Left side of the interval</param>
        /// <param name="range">Residual of both sides of the interval</param>
        /// <param name="vars">Number of variables in the function</param>
        /// <param name="optimum">The global optimum</param>
        /// <param name="idx">Index of the function name</param>
        protected Function(double left, double range, int vars, double optimum, int idx)
        {
            intervalLeft = left;
            intervalRange = range;
            numOfVariables = vars;
            globalOptimum = optimum;

            SetAFunction(idx);
        }

        /// <summary>
        /// Set the function from the list according to its name
        /// </summary>
        /// <param name="idx">Index of the function name in an array</param>
        private void SetAFunction(int idx)
        {
            FunctionLine allFunctions = new FunctionLine();
            functionCalc = new FunctionCalculator(allFunctions.ChooseAFunction(idx));
        }

        /// <summary>
        /// Calculate function result
        /// </summary>
        /// <param name="variables">Variables to be inserted in the function</param>
        /// <returns>Result of the function</returns>
        protected double CalculateFunction(double[] variables)
        {
            return functionCalc(variables);
        }
    }
}
