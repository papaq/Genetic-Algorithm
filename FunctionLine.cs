using System;
using System.Linq;

namespace GenCon
{
    /// <summary>
    /// Class contains all functions, that can be used in the program
    /// </summary>
    public class FunctionLine : FunctionBase
    {
        public static FunctionCalculator ChooseAFunction(int idx)
        {
            switch (idx)
            {
                case 0:
                    return DeJong();
                case 1:
                    return GoldsteinPrice();
                case 2:
                    return Branin();
                case 3:
                    return MartinGaddy();
                case 4:
                    return Rosenbrock();
                case 5:
                    return Rosenbrock2();
                case 6:
                    return HyperSphere();
                default:
                    return Griewangk();
            }
        }

        private static FunctionCalculator DeJong()
        {
            return
                parList =>
                    3905.93 - 100 * Math.Pow(Math.Pow(parList[0], 2) - parList[1], 2) -
                    Math.Pow(1 - parList[0], 2);
        }

        private static FunctionCalculator GoldsteinPrice()
        {
            return parList =>
            {
                return (1 + Math.Pow(parList[0] + parList[1] + 1, 2)
                    * (19 - 14 * parList[0] + 3 * Math.Pow(parList[0], 2) 
                        - 14 * parList[1] + 6 * parList[0] * parList[1] + 3 * Math.Pow(parList[1], 2)))
                    * (30 + Math.Pow(2 * parList[0] - 3 * parList[1], 2)
                    * (18 - 32 * parList[0] + 12 * Math.Pow(parList[0], 2) + 48 * parList[1] 
                        - 36 * parList[0] * parList[1] + 27 * Math.Pow(parList[1], 2)));
            };
        }

        private static FunctionCalculator Branin()
        {
            return parList =>
            {
                const double a = 1;
                var b = 5.1 / Math.Pow(Math.PI, 2) / 4;
                const double c = 5 / Math.PI;
                const double r = 6;
                const double s = 10;
                const double t = 0.125 / Math.PI;

                return a * Math.Pow(parList[1] - b * Math.Pow(parList[0], 2) + c * parList[0] - r, 2) 
                    + s * (1 - t) * Math.Cos(parList[0]) + s;
            };
        }

        private static FunctionCalculator MartinGaddy()
        {
            return parList =>
            {
                return Math.Pow(parList[0] - parList[1], 2) 
                    + Math.Pow((parList[0] + parList[1] - 10) / 3, 2);
            };
        }

        private static FunctionCalculator Rosenbrock()
        {
            return parList =>
            {
                return 100 * Math.Pow(Math.Pow(parList[0], 2) - parList[1], 2) 
                    + Math.Pow(1 - parList[0], 2);
            };
        }

        private static FunctionCalculator Rosenbrock2()
        {
            return parList =>
            {
                double sum = 0;

                for (var i = 0; i < 3; i++)
                {
                    sum += 100 * Math.Pow(Math.Pow(parList[i], 2) - parList[i + 1], 2) 
                        + Math.Pow(1 - parList[i], 2);
                }

                return sum;
            };
        }

        private static FunctionCalculator HyperSphere()
        {
            return parList =>
            {
                return parList.Sum(t => Math.Pow(t, 2));
            };
        }

        private static FunctionCalculator Griewangk()
        {
            return parList =>
            {
                double sum = 0;
                double mult = 1;

                for (var i = 1; i <= parList.Length; i++)
                {
                    sum += Math.Pow(parList[i - 1], 2) / 4000;
                    mult *= Math.Cos(parList[i - 1] / Math.Sqrt(i));
                }

                return 1 / (1.1 + sum - mult);
            };
        }
    }
}
