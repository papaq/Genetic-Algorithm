using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            public double[] variables;
        }

        /// <summary>
        /// Structure for Element details
        /// </summary>
        public struct ElementOptimum
        {
            public double optimum;
            public double fitness;
            public Element element;
        }


        // List of all elements in a generation
        private List<Element> generation;

        // Number of elements in a generation
        private int numberOfElements;

        // Array of fitness factors
        private double[] elsFitnessFactors;

        // Part of Elements taking part in elitism
        private double elitismRate;

        // Part of Elements that take part in mutations
        private double mutationRate;

        // Randoms come from here
        private Random random;

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

            numberOfElements = numOfLs;
            generation = new List<Element>();
            elsFitnessFactors = new double[numOfLs];
            elitismRate = elitism;
            mutationRate = mutation;

            random = new Random();

            FillGeneration();
        }

        /// <summary>
        /// Go through the whole life cycle of generation
        /// </summary>
        /// <returns>Element with best fitness</returns>
        public ElementOptimum LifeCycle()
        {
            // Calculate fitness for our generation
            double[] fitnessArray = CalculateFitness(generation);

            // Check if Optimum found than return it
            int maxFitnessIdx = FindMaxFitness(generation, fitnessArray);
            if (fitnessArray[maxFitnessIdx] > .99)
            {
                return new ElementOptimum()
                {
                    element = generation[maxFitnessIdx],
                    optimum = CalculateFunction(generation[maxFitnessIdx].variables),
                    fitness = fitnessArray[maxFitnessIdx],
                };
            }

            // Choose parents for a new generation
            int[] parents = TournamentSelection(fitnessArray);

            // Create new generation and calculate fitness rates
            List<Element> newGeneration = Crossover(parents);
            double[] newGenFitnessArray = CalculateFitness(newGeneration);

            // Check if Optimum found than return it
            maxFitnessIdx = FindMaxFitness(newGeneration, newGenFitnessArray);
            if (newGenFitnessArray[maxFitnessIdx] > .98)
            {
                return new ElementOptimum()
                {
                    element = newGeneration[maxFitnessIdx],
                    optimum = CalculateFunction(newGeneration[maxFitnessIdx].variables),
                    fitness = newGenFitnessArray[maxFitnessIdx],
                };
            }

            // Execute elitism
            newGeneration = Elitism(generation, newGeneration, fitnessArray, newGenFitnessArray);
            
            // Execute mutation
            newGeneration = Mutation(newGeneration);

            // Set newGeneration as generation
            generation.Clear();
            generation = newGeneration;

            // Set variables to find return values
            fitnessArray = CalculateFitness(generation);
            maxFitnessIdx = FindMaxFitness(generation, fitnessArray);

            // Return best element
            return new ElementOptimum()
            {
                element = generation[maxFitnessIdx],
                optimum = CalculateFunction(generation[maxFitnessIdx].variables),
                fitness = fitnessArray[maxFitnessIdx],
            };
        }

        /// <summary>
        /// Find the index of an element with best fitness
        /// </summary>
        /// <param name="gen">Generation</param>
        /// <param name="fitnessArray">Array of fitness values</param>
        /// <returns>Index of an element</returns>
        private int FindMaxFitness(List<Element> gen, double[] fitnessArray)
        {
            int tempMaxFitnessIdx = 0;

            for (int i = 1; i < numberOfElements; i++)
            {
                if (fitnessArray[i] > fitnessArray[tempMaxFitnessIdx])
                {
                    tempMaxFitnessIdx = i;
                }
            }

            return tempMaxFitnessIdx;
        }

        /// <summary>
        /// Fills the generation with elements
        /// </summary>
        private void FillGeneration()
        {
            for (int i = 0; i < numberOfElements; i++)
            {
                generation.Add(new Element() {
                    variables = FillElementRandom(),
                });
            }
        }

        /// <summary>
        /// Fills element with random values of variables
        /// </summary>
        /// <returns>Array of element's variables</returns>
        private double[] FillElementRandom()
        {
            double[] varsArray = new double[numOfVariables];
            for (int i = 0; i < numOfVariables; i++)
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
            return random.NextDouble() * intervalRange + intervalLeft;
        }

        /// <summary>
        /// Calculates fitness of generation elements according to function value
        /// </summary>
        /// <param name="generation">Specific generation</param>
        /// <returns>Array of fitnesses for every element</returns>
        private double[] CalculateFitness(List<Element> generation)
        {
            double[] newFitnessFactors = new double[numberOfElements];
            double[] elementsMidRates = new double[numberOfElements];
            double reversedRateSum = 0;

            for (int i = 0; i < numberOfElements; i++)
            {
                elementsMidRates[i] = Math.Abs(globalOptimum - CalculateFunction(generation[i].variables));
                reversedRateSum += 1 / elementsMidRates[i];
            }

            for (int i = 0; i < numberOfElements; i++)
            {
                newFitnessFactors[i] = 1 / (elementsMidRates[i] * reversedRateSum);
            }

            return newFitnessFactors;
        }

        /// <summary>
        /// Selects parents for the new generation
        /// </summary>
        /// <param name="fitnessFactors">Array of fitnesses for every element</param>
        /// <param name="rnd">Instance of Random class</param>
        /// <returns>Array of parents that go in pairs one by one</returns>
        private int[] TournamentSelection(double[] fitnessFactors)
        {
            int[] parentsArray = new int[numberOfElements * 2];

            for (int i = 0; i < numberOfElements * 2; i++)
            {
                int candidate1 = random.Next(numberOfElements);
                int candidate2 = random.Next(numberOfElements);

                parentsArray[i] = fitnessFactors[candidate1] > fitnessFactors[candidate2] ? candidate1 : candidate2;
            }

            return parentsArray;
        }

        /// <summary>
        /// Executes crossover for each pair of parents into a new generation
        /// </summary>
        /// <param name="parentsArray">Array of parent pairs</param>
        /// <returns>A new generation</returns>
        private List<Element> Crossover(int[] parentsArray)
        {
            List<Element> newGeneration = new List<Element>();
            DottedCrossover crossoverType;

            switch (numOfVariables)
            {
                case 1:
                    crossoverType = new DottedCrossover(ZeroDottedCrossover);
                    break;
                case 2:
                    crossoverType = new DottedCrossover(OneDottedCrossover);
                    break;
                default:
                    crossoverType = new DottedCrossover(TwoDottedCrossover);
                    break;
            }

            for (int i = 0; i < numberOfElements; i++)
            {
                newGeneration.Add(new Element()
                {
                    variables = crossoverType(generation[parentsArray[i * 2]].variables, generation[parentsArray[i * 2 + 1]].variables)
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
        private double[] ZeroDottedCrossover(double[] parent1, double[] parent2)
        {
            return new double[] { random.Next(2) > 0 ? parent2[0] : parent1[0] };
        }

        /// <summary>
        /// Crossover copies a part of each parent into the child
        /// </summary>
        /// <param name="parent1">The first parent</param>
        /// <param name="parent2">The second parent</param>
        /// <returns></returns>
        private double[] OneDottedCrossover(double[] parent1, double[] parent2)
        {
            int dot = random.Next(numOfVariables - 1);
            double[] newElement = new double[numOfVariables];

            for (int i = 0; i < dot + 1; i++)
            {
                newElement[i] = parent1[i];
            }

            for (int i = dot + 1; i < numOfVariables; i++)
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
        private double[] TwoDottedCrossover(double[] parent1, double[] parent2)
        {
            int dot1, dot2;
            dot1 = random.Next(numOfVariables - 1);
            double[] newElement = new double[numOfVariables];

            do
            {
                dot2 = random.Next(numOfVariables - 1);
            } while (dot1 == dot2);

            if (dot1 > dot2)
            {
                int t = dot1;
                dot1 = dot2;
                dot2 = t;
            }
            

            for (int i = 0; i < dot1 + 1; i++)
            {
                newElement[i] = parent1[i];
            }

            for (int i = dot1 + 1; i < dot2 + 1; i++)
            {
                newElement[i] = parent2[i];
            }

            for (int i = dot2 + 1; i < numOfVariables; i++)
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
        private List<Element> Elitism(List<Element> oldG, List<Element> newG, double[] oldFitness, double[] newFitness)
        {
            int parentsWontDie = (int)(elitismRate * numberOfElements);
            List<double> oldFitnessList, newFitnessList;
            oldFitnessList = oldFitness.ToList();
            newFitnessList = newFitness.ToList();
            
            for (int i = 0; i < parentsWontDie; i++)
            {
                int oldButGold = oldFitnessList.FindIndex(fitness => fitness == oldFitnessList.Max());
                int newButPfe = newFitnessList.FindIndex(fitness => fitness == newFitnessList.Min());

                newG.RemoveAt(newButPfe);
                newG.Insert(newButPfe, new Element()
                {
                    variables = oldG[oldButGold].variables,
                });

                newFitnessList[newButPfe] = oldFitnessList[oldButGold];
            }

            return newG;
        }

        /// <summary>
        /// Execute mutations with elements from current generation
        /// </summary>
        /// <param name="generation">Generation to be mutated</param>
        /// <returns>Mutated generation</returns>
        private List<Element> Mutation(List<Element> generation)
        {
            int genesToBeMutated = (int)(mutationRate * numberOfElements);

            for (int i = 0; i < genesToBeMutated; i++)
            {
                int randomElement = random.Next(numberOfElements);
                int randomGene = random.Next(numOfVariables);

                generation[randomElement].variables[randomGene] = RandomFromRange();
            }

            return generation;
        }
    }
}
