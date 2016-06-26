using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GenCon
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Array of Functions' names
        private string[] FunctionList;

        // Flag to control whether we calculate or stop
        private bool goOn = false;

        private int functionIndex = 0;

        private static int numberOfEls, numberOfVars;
        private static double intervalLeft, intervalRight;
        private static double globalOptimum, elitismRate, mutationRate;

        FunctionsInputData funcInputData;

        public MainWindow()
        {
            InitializeComponent();

            // Fill Funcs' names array
            FillFuncNamesArray();

            // Init Functions' input data
            funcInputData = new FunctionsInputData();


            // Put List into Combobox
            ComboBox_FunctionList.ItemsSource = FunctionList;
            // The first in list is chosen
            ComboBox_FunctionList.SelectedValue = ComboBox_FunctionList.Items[0];
            
        }
        
        private void FillFuncNamesArray()
        {
            FunctionList = new string[]
            {
                "De Jong",
                "Goldstein & Price",
                "Branin",
                "Martin & Gaddy",
                "Rosenbrock",
                "Rosenbrock II",
                "Hyper Sphere",
                "Griewangk",
            };
        }

        private void Button_StartStop_Click(object sender, RoutedEventArgs e)
        {
            if (Equals(Button_StartStop.Content, "Start"))
            {
                Button_StartStop.Content = "Stop";
                if (!goOn)
                {
                    goOn = true;
                }

                FillStaticVars();

                Thread thread = new Thread(FindGeneration);
                thread.Start();
            }
            else
            {
                Button_StartStop.Content = "Start";
                if (goOn)
                {
                    goOn = false;
                }
            }
        }

        private void FillStaticVars()
        {
            numberOfEls = Convert.ToInt16(TextBox_NumberOfEls.Text);
            numberOfVars = funcInputData.GetNumOfVars(functionIndex);
            intervalLeft = funcInputData.GetInterval(functionIndex).intervalLeft;
            intervalRight = funcInputData.GetInterval(functionIndex).intervalRight;
            globalOptimum = funcInputData.GetGlobalOptimum(functionIndex);
            elitismRate = Convert.ToDouble(textBox_Elitism_ParentsInNextGen.Text);
            mutationRate = Convert.ToDouble(TextBox_MutantsInGen.Text);
        }

        private void ComboBox_FunctionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBox_FunctionList.Items.Count == 0 || funcInputData == null)
            {
                return;
            }

            if (!Button_StartStop.IsEnabled && Equals(Button_StartStop.Content, "Start"))
            {
                Button_StartStop.IsEnabled = true;
            }

            functionIndex = ComboBox_FunctionList.SelectedIndex;

            TextBox_IntervalLeft.Text = funcInputData.GetInterval(functionIndex).intervalLeft.ToString();
            TextBox_IntervalRight.Text = funcInputData.GetInterval(functionIndex).intervalRight.ToString();

            TextBox_GlobalOptimum.Text = funcInputData.GetGlobalOptimum(functionIndex).ToString();
        }

        private void FindGeneration()
        {
            Generation generationInstance = new Generation(
                numberOfEls,
                numberOfVars,
                intervalLeft,
                intervalRight,
                globalOptimum,
                functionIndex,
                elitismRate,
                mutationRate);

            long genIdx = 1;
            while (goOn)
            {
                Generation.ElementOptimum currentBestElement = generationInstance.LifeCycle();
                

                if (genIdx % 10 == 0)
                {
                    InvokeActonWithDispatcher(Label_GenerationCurrent, new Action(delegate ()
                    {
                        Label_GenerationCurrent.Content = genIdx.ToString();
                    }));
                }

                InvokeActonWithDispatcher(Label_GenerationCurrent, new Action(delegate ()
                {
                    Label_OptimumNow.Content = currentBestElement.optimum.ToString();
                }));

                Thread.Sleep(10);


                if (currentBestElement.fitness > .99)
                {
                    goOn = false;
                    InvokeActonWithDispatcher(Label_GenerationCurrent, new Action(delegate ()
                    {
                        Button_StartStop.Content = "Start";
                    }));
                }

                genIdx++;
            }
        }

        private void InvokeActonWithDispatcher(UIElement uiElement, Action action)
        {
            uiElement.Dispatcher.BeginInvoke(action);
        }
    }
}
