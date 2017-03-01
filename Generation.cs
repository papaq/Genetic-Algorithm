using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Animation;

namespace GenCon
{
    public delegate double[] DottedCrossingOver(double[] parent1, double[] parent2);

    public delegate int BestFunctionValueIndexCalculator(List<Generation.ElementValue> listOfFunctionValues);

    public delegate void BestValueChecker(Generation.ElementVariables newSpecies);

    // All processes of a generation
    public class Generation : Function 
    {
        /// <summary>
        /// Structure is to keep an element of a generation and all genes inside
        /// </summary>
        public struct ElementVariables
        {
            public double[] Variables;
        }

        /// <summary>
        /// Structure for Element details
        /// </summary>
        public struct ElementValue
        {
            public int Index;
            public double FunctionValue;
        }

        //public static double AvarageFitnessRate;    

        // List of all elements in a generation
        private List<ElementVariables> _generation;

        // Number of elements in a generation
        private readonly int _numberOfElements;
        
        // Part of Elements taking part in elitism
        private readonly double _elitismRate;

        // Part of Elements that take part in mutations
        private readonly double _mutationRate;

        // Crossing over calculator
        private DottedCrossingOver _executeCrossingOver;

        // Fitness calculator
        private BestFunctionValueIndexCalculator _calculateBestValueIndex;

        // Change best species for the better
        private BestValueChecker _checkIfNewBetter;

        // Best element found
        private ElementVariables _bestElement;

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
        /// <param name="isMin">Admits whether we search for min value</param>
        /// <param name="funcIdx">Index of the function in the name array</param>
        /// <param name="elitism">Elitism rate</param>
        /// <param name="mutation">Mutation rate</param>
        public Generation(int numOfLs, 
            int numOfVars, 
            double left, 
            double right, 
            double optimum,
            bool isMin,
            int funcIdx,
            double elitism, 
            double mutation)
            : base(left, right - left, numOfVars, optimum, funcIdx)
        {

            _numberOfElements = numOfLs;
            _generation = new List<ElementVariables>();
            _elitismRate = elitism;
            _mutationRate = mutation;

            SetCrossingOver();
            SetBestValueIndexCalculator(isMin);
            SetBestValueChecker(isMin);

            _random = new Random();

            FillGeneration();
        }

        private void SetCrossingOver()
        {
            switch (NumOfVariables)
            {
                case 1:
                    _executeCrossingOver = ZeroDottedCrossingOver;
                    break;
                case 2:
                    _executeCrossingOver = OneDottedCrossingOver;
                    break;
                default:
                    _executeCrossingOver = TwoDottedCrossingOver;
                    break;
            }
        }

        private void SetBestValueIndexCalculator(bool isMin)
        {
            if (isMin)
            {
                _calculateBestValueIndex = CalculateMinFunctionValue;
                return;
            }

            _calculateBestValueIndex = CalculateMaxFunctionValue;
        }

        private void SetBestValueChecker(bool isMin)
        {
            if (isMin)
            {
                _checkIfNewBetter = ChangeBestMin;
                return;
            }

            _checkIfNewBetter = ChangeBestMax;
        }


        /// <summary>
        /// Go through the whole life cycle of generation
        /// </summary>
        /// <returns>Element with best fitness</returns>
        public double LifeCycle()
        {

            // Fitness calculation
            // Selection
            // Crossing over
            // New fitness calculation
            // Elitism
            // Mutation
            // Replace parents with children


            // Calculate fitness for our generation
            var fitnessArray = CalculateFitness(_generation);/////////////////////////////////////////////////////

            // Check if Optimum is found than return it
            var maxFitnessIdx = FindMaxFitnessIndex(fitnessArray);



            // Set new best value, if really
            _checkIfNewBetter(_generation[maxFitnessIdx]);

            // Choose parents for a new generation
            var parents = TournamentSelection(fitnessArray);

            // Create new generation and calculate fitness rates
            var newGeneration = CrossingOver(parents);
            var newGenFitnessArray = CalculateFitness(newGeneration);

            // Check if Optimum is found than return it
            maxFitnessIdx = FindMaxFitnessIndex(newGenFitnessArray);



            // Set new best value, if really
            _checkIfNewBetter(newGeneration[maxFitnessIdx]);

            // Execute elitism
            newGeneration = Elitism(_generation, newGeneration, fitnessArray, newGenFitnessArray);
            
            // Execute mutation
            newGeneration = Mutation(newGeneration);

            // Set newGeneration as generation
            _generation.Clear();
            _generation = newGeneration;

            // Set variables to find return values
            fitnessArray = CalculateFitness(_generation);
            maxFitnessIdx = FindMaxFitnessIndex(fitnessArray);
            
            // Return best element
            return CalculateFunction(_bestElement.Variables);
        }

        /// <summary>
        /// Find the index of an element with best fitness
        /// </summary>
        /// <param name="fitnessArray">Array of fitness values</param>
        /// <returns>Index of an element</returns>
        private int FindMaxFitnessIndex(IReadOnlyList<double> fitnessArray)
        {
            /*
            var tempMaxFitnessIdx = 0;

            for (var i = 1; i < _numberOfElements; i++)
            {
                if (fitnessArray[i] > fitnessArray[tempMaxFitnessIdx])
                {
                    tempMaxFitnessIdx = i;
                }
            }

            return tempMaxFitnessIdx;
            */

            return FindMaxDoubleValueIdx(fitnessArray);
        }

        private static int FindMaxDoubleValueIdx(IReadOnlyList<double> arr)
        {
            var maxIndex = 0;
            var maxValue = arr[0];

            for (var i = 1; i < arr.Count; i++)
            {
                if (!(arr[i] > maxValue)) continue;

                maxIndex = i;
                maxValue = arr[i];
            }

            return maxIndex;
        }

        private static int FindMinDoubleValueIdx(IReadOnlyList<double> arr)
        {
            var maxIndex = 0;
            var maxValue = arr[0];

            for (var i = 1; i < arr.Count; i++)
            {
                if (!(arr[i] > maxValue)) continue;

                maxIndex = i;
                maxValue = arr[i];
            }

            return maxIndex;
        }

        private void ChangeBestMin(ElementVariables newSpecies)
        {
            if (_bestElement.Variables == null)
            {
                _bestElement = newSpecies;
                return;
            }

            if (CalculateFunction(_bestElement.Variables) > CalculateFunction(newSpecies.Variables))
            {
                _bestElement = newSpecies;
            }
        }

        private void ChangeBestMax(ElementVariables newSpecies)
        {
            if (_bestElement.Variables == null)
            {
                _bestElement = newSpecies;
                return;
            }

            if (CalculateFunction(_bestElement.Variables) < CalculateFunction(newSpecies.Variables))
            {
                _bestElement = newSpecies;
            }
        }

        /// <summary>
        /// Fills the generation with elements
        /// </summary>
        private void FillGeneration()
        {
            for (var i = 0; i < _numberOfElements; i++)
            {
                _generation.Add(new ElementVariables() {
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
        private double[] CalculateFitness(IReadOnlyList<ElementVariables> generation)
        {
            var newFitnessFactors = new double[_numberOfElements];
            var elementsMidRates = new double[_numberOfElements];
            double reversedRateSum = 0;

            var bestValueIndex = _calculateBestValueIndex(MakeListOfElementsFunctionValues(_generation));
            var bestValue = CalculateFunction(generation[bestValueIndex].Variables);

            for (var i = 0; i < _numberOfElements; i++)
            {
                elementsMidRates[i] = i == bestValueIndex 
                    ? 0.1 ////////////////////////////////////////////////////////////////////// Or another??????
                    : Math.Abs(bestValue - CalculateFunction(generation[i].Variables));
                reversedRateSum += 1 / elementsMidRates[i];
            }

            for (var i = 0; i < _numberOfElements; i++)
            {
                newFitnessFactors[i] = 1 / (elementsMidRates[i] * reversedRateSum);
            }

            return newFitnessFactors;
        }

        private static int CalculateMinFunctionValue(List<ElementValue> listOfFunctionValues)
        {
            var bestValueIndex = listOfFunctionValues.OrderByDescending(x => x.FunctionValue).Last().Index;

            return bestValueIndex;
        }

        private static int CalculateMaxFunctionValue(List<ElementValue> listOfFunctionValues)
        {
            var bestValueIndex = listOfFunctionValues.OrderByDescending(x => x.FunctionValue).First().Index;

            return bestValueIndex;
        }

        private List<ElementValue> MakeListOfElementsFunctionValues(IEnumerable<ElementVariables> generation)
        {
            var index = 0;

            var listOfFunctionValues = generation.Select(element => new ElementValue
            {
                FunctionValue = CalculateFunction(element.Variables),
                Index = index++,
            }).ToList();

            return listOfFunctionValues;
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
        private List<ElementVariables> CrossingOver(IReadOnlyList<int> parentsArray)
        {
            var newGeneration = new List<ElementVariables>();

            for (var i = 0; i < _numberOfElements; i++)
            {
                newGeneration.Add(new ElementVariables()
                {
                    Variables =
                        _executeCrossingOver(_generation[parentsArray[i * 2]].Variables,
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
        private List<ElementVariables> Elitism(IReadOnlyList<ElementVariables> oldG, List<ElementVariables> newG, IEnumerable<double> oldFitness,
            IEnumerable<double> newFitness)
        {
            var parentsWontDie = (int)(_elitismRate * _numberOfElements);
            var oldFitnessList = oldFitness.ToList();
            var newFitnessList = newFitness.ToList();

            for (var i = 0; i < parentsWontDie; i++)
            {
                var oldButGold = FindMaxDoubleValueIdx(oldFitnessList);
                var newButPfe = FindMinDoubleValueIdx(newFitnessList);


                newG.RemoveAt(newButPfe);
                newG.Insert(newButPfe, new ElementVariables()
                {
                    Variables = oldG[oldButGold].Variables,
                });


                newFitnessList[newButPfe] = oldFitnessList[oldButGold];

                // Reset best at parents list
                oldFitnessList[oldButGold] = 0;
            }

            return newG;
        }

        /// <summary>
        /// Execute mutations with elements from current generation
        /// </summary>
        /// <param name="gener">Generation to be mutated</param>
        /// <returns>Mutated generation</returns>
        private List<ElementVariables> Mutation(List<ElementVariables> gener)
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
