﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenCon
{
    /// <summary>
    /// Class contains all functions, that can be used in the program
    /// </summary>
    public class FunctionLine : FunctionBase
    {
        public FunctionCalculator ChooseAFunction(int idx)
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
                    return RosenbrockII();
                default:
                    return Griewangk();
            }
        }

        private FunctionCalculator DeJong()
        {
            return parList =>
            {
                return (3905.93) - 100 * Math.Pow((Math.Pow(parList[0], 2) - parList[1]), 2) - Math.Pow((1 - parList[0]), 2);
            };
        }

        private FunctionCalculator GoldsteinPrice()
        {
            return parList =>
            {
                return (1 + Math.Pow((parList[0] + parList[1] + 1), 2)
                * (19 - 14 * parList[0] + 3 * Math.Pow(parList[0], 2) - 14 * parList[1] + 6 * parList[0] * parList[1] + 3 * Math.Pow(parList[1], 2)))
                * (30 + Math.Pow((2 * parList[0] - 3 * parList[1]), 2)
                * (18 - 32 * parList[0] + 12 * Math.Pow(parList[0], 2) + 48 * parList[1] - 36 * parList[0] * parList[1] + 27 * Math.Pow(parList[1], 2)));
            };
        }

        private FunctionCalculator Branin()
        {
            return parList =>
            {
                return Math.Pow((parList[1] - 5.1 * Math.Pow(7 / 22, 2) / 4 * Math.Pow(parList[0], 2) + 35 * parList[0] / 22 - 6), 2)
                + (10 - 70 / 176) * Math.Cos(parList[0]) + 10;
            };
        }

        private FunctionCalculator MartinGaddy()
        {
            return parList =>
            {
                return Math.Pow((parList[0] - parList[1]), 2) + Math.Pow(((parList[0] + parList[1] - 10) / 3), 2);
            };
        }

        private FunctionCalculator Rosenbrock()
        {
            return parList =>
            {
                return 100 + Math.Pow((Math.Pow(parList[0], 2) - parList[1]), 2) + Math.Pow((1 - parList[0]), 2);
            };
        }

        private FunctionCalculator RosenbrockII()
        {
            return parList =>
            {
                double sum = 0;

                for (int i = 0; i < 3; i++)
                {
                    sum += 100 * Math.Pow((Math.Pow(parList[i], 2) - parList[i + 1]), 2) + Math.Pow((1 - parList[i]), 2);
                }

                return sum;
            };
        }

        private FunctionCalculator HyperSphere()
        {
            return parList =>
            {
                double sum = 0;

                for (int i = 0; i < parList.Length; i++)
                {
                    sum += Math.Pow(parList[i], 2);
                }

                return sum;
            };
        }

        private FunctionCalculator Griewangk()
        {
            return parList =>
            {
                double sum = 0;
                double mult = 1;

                for (int i = 0; i < parList.Length; i++)
                {
                    sum += Math.Pow(parList[i], 2) / 4000;
                    mult *= Math.Cos(parList[i] / Math.Sqrt(i + 1));
                }

                return 1 / (1.1 + sum - mult);
            };
        }
    }
}