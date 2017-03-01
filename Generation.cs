using System;
using System.Collections.Generic;
using System.Linq;

namespace GenCon
{
    public delegate double[] DottedCrossover(double[] parent1, double[] parent2);

    // All processes of a generation
    public class Generation : Function 
    {
        /// <summary>
        /// Structure is to keep an element of a generation and all genes inside
        /// </summary>
        public struct Element
        {
            public double[] Variables;
        }

        /// <summary>
        /// Structure for Element details
        /// </summary>
        public struct ElementOptimum
        {
            public double Optimum;
            public double Fitness;
            public Element Element;
        }

        //public static double AvarageFitnessRate;    

        // List of all elements in a generation
        private List<Element> _generation;

        // Number of elements in a generation
        private readonly int _numberOfElements;

        // Array of fitness factors
        private double[] _elementsFitnessFactors;

        // Part of Elements taking part in elitism
        private readonly double _elitismRate;

        // Part of Elements that take part in mutations
        private readonly double _mutationRate;

        // Randoms come from here
        private readonly Random _random;

        // Value for comparison of two doubles
        private const double Tolerantic = 0.000001;

        /// <summary>
        /// Constructor declaration
        /// </summary>
        /// <param name="numOfLs">Number of elements in a generation</param>
        /// <param name="numOfVars">Number of variables in an element</param>
        /// <param name="left">Left side of the interval</param>
        /// <param name="right">Right side of the interval</param>
        /// <param name="optimum">Global optimum of the function</param>
        /// <param name="funcIdx">Index of the function in the name array</param>
        /// <param name="elitism">Elitism rate</param>
        /// <param name="mutation">Mutation rate</param>
        public Generation(int numOfLs, int numOfVars, double left, double right, double optimum, int funcIdx, double elitism, double mutation) 
            : base(left, right - left, numOfVars, optimum, funcIdx)
        {

            _numberOfElements = numOfLs;
            _generation = new List<Element>();
            _elementsFitnessFactors = new double[numOfLs];
            _elitismRate = elitism;
            _mutationRate = mutation;

            _random = new Random();

            FillGeneration();
        }

        /// <summary>
        /// Go through the whole life cycle of generation
        /// </summary>
        /// <returns>Element with best fitness</returns>
        public ElementOptimum LifeCycle()
        {

            // Fitness calculation
            // Selection
            // Crossing over
            // New fitness calculation
            // Elitism
            // Mutation
            // Replace parents with children


            // Calculate fitness for our generation
            var fitnessArray = CalculateFitness(_generation);

            // Check if Optimum is found than return it
            var maxFitnessIdx = FindMaxFitness(fitnessArray);
            if (fitnessArray[maxFitnessIdx] > .99)
            {
                return new ElementOptimum()
                {
                    Element = _generation[maxFitnessIdx],
                    Optimum = CalculateFunction(_generation[maxFitnessIdx].Variables),
                    Fitness = fitnessArray[maxFitnessIdx],
                };
            }

            // Choose parents for a new generation
            var parents = TournamentSelection(fitnessArray);

            // Create new generation and calculate fitness rates
            var newGeneration = CrossingOver(parents);
            var newGenFitnessArray = CalculateFitness(newGeneration);

            // Check if Optimum is found than return it
            maxFitnessIdx = FindMaxFitness(newGenFitnessArray);
            if (newGenFitnessArray[maxFitnessIdx] > .99)
            {
                return new ElementOptimum()
                {
                    Element = newGeneration[maxFitnessIdx],
                    Optimum = CalculateFunction(newGeneration[maxFitnessIdx].Variables),
                    Fitness = newGenFitnessArray[maxFitnessIdx],
                };
            }

            // Execute elitism
            newGeneration = Elitism(_generation, newGeneration, fitnessArray, newGenFitnessArray);
            
            // Execute mutation
            newGeneration = Mutation(newGeneration);

            // Set newGeneration as generation
            _generation.Clear();
            _generation = newGeneration;

            // Set variables to find return values
            fitnessArray = CalculateFitness(_generation);
            maxFitnessIdx = FindMaxFitness(fitnessArray);

            // Calculate avarage fitness
            //CalculateAvarageFitness(fitnessArray);

            // Return best element
            return new ElementOptimum()
            {
                Element = _generation[maxFitnessIdx],
                Optimum = CalculateFunction(_generation[maxFitnessIdx].Variables),
                Fitness = fitnessArray[maxFitnessIdx],
            };
        }

        /// <summary>
        /// Find the index of an element with best fitness
        /// </summary>
        /// <param name="fitnessArray">Array of fitness values</param>
        /// <returns>Index of an element</returns>
        private int FindMaxFitness(IReadOnlyList<double> fitnessArray)
        {
            var tempMaxFitnessIdx = 0;

            for (var i = 1; i < _numberOfElements; i++)
            {
                if (fitnessArray[i] > fitnessArray[tempMaxFitnessIdx])
                {
                    tempMaxFitnessIdx = i;
                }
            }

            return tempMaxFitnessIdx;
        }

        /*
        /// <summary>
        /// Calculates avarage fitness rate for ui
        /// </summary>
        /// <param name="fitnessRates">Array of fitness rates</param>
        private void CalculateAvarageFitness(double[] fitnessRates)
        {
            AvarageFitnessRate = fitnessRates.Sum() / fitnessRates.Length;
        }
        */

        /// <summary>
        /// Fills the generation with elements
        /// </summary>
        private void FillGeneration()
        {
            for (var i = 0; i < _numberOfElements; i++)
            {
                _generation.Add(new Element() {
                    Variables = FillElementRandom(),
                });
            }
        }

        /// <summary>
        /// Fills element with random values of variables
        /// </summary>
        /// <returns>Array of element's variables</returns>
        private double[] FillElementRandom()
        {
            var varsArray = new double[NumOfVariables];
            for (var i = 0; i < NumOfVariables; i++)
            {
                varsArray[i] = RandomFromRange();
            }

            return varsArray;
        }

        /// <summary>
        /// Created random value from the function rate
        /// </summary>
        /// <returns>Real number</returns>
        private double RandomFromRange()
        {
            return _random.NextDouble() * IntervalRange + IntervalLeftBorder;
        }

        /// <summary>
        /// Calculates fitness of generation elements according to function value
        /// </summary>
        /// <param name="generation">Specific generation</param>
        /// <returns>Array of fitnesses for every element</returns>
        private double[] CalculateFitness(IReadOnlyList<Element> generation)
        {
            var newFitnessFactors = new double[_numberOfElements];
            var elementsMidRates = new double[_numberOfElements];
            double reversedRateSum = 0;

            for (var i = 0; i < _numberOfElements; i++)
            {
                elementsMidRates[i] = Math.Abs(GlobalOptimum - CalculateFunction(generation[i].Variables));
                reversedRateSum += 1 / elementsMidRates[i];
            }

            for (var i = 0; i < _numberOfElements; i++)
            {
                newFitnessFactors[i] = 1 / (elementsMidRates[i] * reversedRateSum);
            }

            return newFitnessFactors;
        }

        /// <summary>
        /// Selects parents for the new generation
        /// </summary>
        /// <param name="fitnessFactors">Array of fitnesses for every element</param>
        /// <returns>Array of parents that go in pairs one by one</returns>
        private int[] TournamentSelection(IReadOnlyList<double> fitnessFactors)
        {
            var parentsArray = new int[_numberOfElements * 2];

            for (var i = 0; i < _numberOfElements * 2; i++)
            {
                var candidate1 = _random.Next(_numberOfElements);
                var candidate2 = _random.Next(_numberOfElements);

                parentsArray[i] = fitnessFactors[candidate1] > fitnessFactors[candidate2] ? candidate1 : candidate2;
            }

            return parentsArray;
        }

        /// <summary>
        /// Executes crossover for each pair of parents into a new generation
        /// </summary>
        /// <param name="parentsArray">Array of parent pairs</param>
        /// <returns>A new generation</returns>
        private List<Element> CrossingOver(IReadOnlyList<int> parentsArray)
        {
            var newGeneration = new List<Element>();
            DottedCrossover crossingOverType;

            switch (NumOfVariables)
            {
                case 1:
                    crossingOverType = ZeroDottedCrossingOver;
                    break;
                case 2:
                    crossingOverType = OneDottedCrossingOver;
                    break;
                default:
                    crossingOverType = TwoDottedCrossingOver;
                    break;
            }

            for (var i = 0; i < _numberOfElements; i++)
            {
                newGeneration.Add(new Element()
                {
                    Variables =
                        crossingOverType(_generation[parentsArray[i * 2]].Variables,
                            _generation[parentsArray[i * 2 + 1]].Variables)
                });
            }

            return newGeneration;
        }

        /// <summary>
        /// Crossover copies one of two parents fully into the child
        /// </summary>
        /// <param name="parent1">The first parent</param>
        /// <param name="parent2">The second parent</param>
        /// <returns></returns>
        private double[] ZeroDottedCrossingOver(double[] parent1, double[] parent2)
        {
            return new[] { _random.Next(2) > 0 ? parent2[0] : parent1[0] };
        }

        /// <summary>
        /// Crossover copies a part of each parent into the child
        /// </summary>
        /// <param name="parent1">The first parent</param>
        /// <param name="parent2">The second parent</param>
        /// <returns></returns>
        private double[] OneDottedCrossingOver(double[] parent1, double[] parent2)
        {
            var dot = _random.Next(NumOfVariables - 1);
            var newElement = new double[NumOfVariables];

            for (var i = 0; i < dot + 1; i++)
            {
                newElement[i] = parent1[i];
            }

            for (var i = dot + 1; i < NumOfVariables; i++)
            {
                newElement[i] = parent2[i];
            }

            return newElement;
        }

        /// <summary>
        /// Crossover copies two parts of the first parent and one of the second one into the child
        /// </summary>
        /// <param name="parent1">The first parent</param>
        /// <param name="parent2">The second parent</param>
        /// <returns></returns>
        private double[] TwoDottedCrossingOver(double[] parent1, double[] parent2)
        {
            int dot2;
            var dot1 = _random.Next(NumOfVariables - 1);
            var newElement = new double[NumOfVariables];

            do
            {
                dot2 = _random.Next(NumOfVariables - 1);
            } while (dot1 == dot2);

            if (dot1 > dot2)
            {
                var t = dot1;
                dot1 = dot2;
                dot2 = t;
            }
            

            for (var i = 0; i < dot1 + 1; i++)
            {
                newElement[i] = parent1[i];
            }

            for (var i = dot1 + 1; i < dot2 + 1; i++)
            {
                newElement[i] = parent2[i];
            }

            for (var i = dot2 + 1; i < NumOfVariables; i++)
            {
                newElement[i] = parent1[i];
            }

            return newElement;
        }

        /// <summary>
        /// Exchange bad elements from the new generation with good ones from the old g
        /// </summary>
        /// <param name="oldG">Old generation</param>
        /// <param name="newG">New generation</param>
        /// <param name="oldFitness">Fitness rate of the old g</param>
        /// <param name="newFitness">Fitness rate of the new g</param>
        /// <returns>New generation with some parent elements</returns>
        private List<Element> Elitism(IReadOnlyList<Element> oldG, List<Element> newG, IEnumerable<double> oldFitness,
            IEnumerable<double> newFitness)
        {
            var parentsWontDie = (int)(_elitismRate * _numberOfElements);
            var oldFitnessList = oldFitness.ToList();
            var newFitnessList = newFitness.ToList();
            
            for (var i = 0; i < parentsWontDie; i++)
            {
                var oldButGold = oldFitnessList.FindIndex(
                    fitness => Math.Abs(fitness - oldFitnessList.Max()) < Tolerantic
                    );
                var newButPfe = newFitnessList.FindIndex(
                    fitness => Math.Abs(fitness - newFitnessList.Min()) < Tolerantic
                    );

                newG.RemoveAt(newButPfe);
                newG.Insert(newButPfe, new Element()
                {
                    Variables = oldG[oldButGold].Variables,
                });

                newFitnessList[newButPfe] = oldFitnessList[oldButGold];
            }

            return newG;
        }

        /// <summary>
        /// Execute mutations with elements from current generation
        /// </summary>
        /// <param name="gener">Generation to be mutated</param>
        /// <returns>Mutated generation</returns>
        private List<Element> Mutation(List<Element> gener)
        {
            var genesToBeMutated = (int)(_mutationRate * _numberOfElements);

            for (var i = 0; i < genesToBeMutated; i++)
            {
                var randomElement = _random.Next(_numberOfElements);
                var randomGene = _random.Next(NumOfVariables);

                gener[randomElement].Variables[randomGene] = RandomFromRange();
            }

            return gener;
        }
    }
}
