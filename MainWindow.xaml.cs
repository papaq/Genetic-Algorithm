using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace GenCon
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        // Array of Functions' names
        private string[] _functionList;

        // Flag to control whether we calculate or stop
        private bool _goOn;

        private int _functionIndex;

        private int _numberOfEls, _numberOfVars, _numberOfGenerations;
        private double _leftBorder, _rightBorder;
        private double _globalOptimum, _elitismRate, _mutationRate;
        private bool _isMinWeLookingFor;

        readonly FunctionsInputData _funcInputData;

        public MainWindow()
        {
            InitializeComponent();

            // Fill Funcs' names array
            FillFuncNamesArray();

            // Init Functions' input data
            _funcInputData = new FunctionsInputData();


            // Put List into Combobox
            ComboBox_FunctionList.ItemsSource = _functionList;
            // The first in list is chosen
            ComboBox_FunctionList.SelectedValue = ComboBox_FunctionList.Items[0];
            
        }
        
        private void FillFuncNamesArray()
        {
            _functionList = new[]
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
                if (!_goOn)
                {
                    _goOn = true;
                }

                FillStaticVars();

                var genThread = new Thread(FindGeneration);
                genThread.Start();
                var timeThread = new Thread(CountTime);
                timeThread.Start();
            }
            else
            {
                Button_StartStop.Content = "Start";
                if (_goOn)
                {
                    _goOn = false;
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _goOn = false;
        }

        private void FillStaticVars()
        {
            _numberOfEls = Convert.ToInt32(TextBox_NumberOfEls.Text);
            _numberOfVars = _funcInputData.GetNumOfVars(_functionIndex);
            _numberOfGenerations = Convert.ToInt32(TextBox_NofGenerations.Text);
            _leftBorder = _funcInputData.GetInterval(_functionIndex).LeftBorder;
            _rightBorder = _funcInputData.GetInterval(_functionIndex).RightBorder;
            _globalOptimum = _funcInputData.GetGlobalOptimum(_functionIndex);
            _elitismRate = Convert.ToDouble(textBox_Elitism_ParentsInNextGen.Text);
            _mutationRate = Convert.ToDouble(TextBox_MutantsInGen.Text);
        }

        private void ComboBox_FunctionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBox_FunctionList.Items.Count == 0 || _funcInputData == null)
            {
                return;
            }

            if (!Button_StartStop.IsEnabled && Equals(Button_StartStop.Content, "Start"))
            {
                Button_StartStop.IsEnabled = true;
            }

            _functionIndex = ComboBox_FunctionList.SelectedIndex;

            TextBox_IntervalLeft.Text = _funcInputData.GetInterval(_functionIndex).LeftBorder.ToString(CultureInfo.InvariantCulture);
            TextBox_IntervalRight.Text = _funcInputData.GetInterval(_functionIndex).RightBorder.ToString(CultureInfo.InvariantCulture);
            Label_GlobalOptimum.Content = "Global " + (_funcInputData.IsFuncMin(_functionIndex) ? "min" : "max");
            _isMinWeLookingFor = _funcInputData.IsFuncMin(_functionIndex);

            TextBox_GlobalOptimum.Text =
                _funcInputData.GetGlobalOptimum(_functionIndex).ToString(CultureInfo.InvariantCulture);
        }

        private void CountTime()
        {
            var stopW = System.Diagnostics.Stopwatch.StartNew();
            while (_goOn)
            {
                InvokeActonWithDispatcher(Label_GenerationCurrent, delegate {
                    Label_TimeGone.Content = (stopW.ElapsedMilliseconds/1000).ToString();
                });
                Thread.Sleep(1000);
            }
            stopW.Stop();
        }

        private void FindGeneration()
        {
            var generationInstance = new Generation(
                _numberOfEls,
                _numberOfVars,
                _leftBorder,
                _rightBorder,
                _globalOptimum,
                _isMinWeLookingFor,
                _functionIndex,
                _elitismRate,
                _mutationRate);
            
            for (var i = 0; i < _numberOfGenerations; i++)
            {
                var currentBestElement = generationInstance.LifeCycle();
                
                var idx = i;
                InvokeActonWithDispatcher(Label_GenerationCurrent, delegate {
                    Label_GenerationCurrent.Content = idx.ToString();
                });

                InvokeActonWithDispatcher(Label_GenerationCurrent, delegate {
                    Label_OptimumNow.Content = String.Format("{0:0.##### }", currentBestElement);
                });
                
                Thread.Sleep(0);
                
                if (_goOn) continue;

                InvokeActonWithDispatcher(Label_GenerationCurrent, delegate {
                    Button_StartStop.Content = "Start";
                });
                return;
            }

            _goOn = false;
            InvokeActonWithDispatcher(Label_GenerationCurrent, delegate {
                Button_StartStop.Content = "Start";
            });
        }

        private static void InvokeActonWithDispatcher(DispatcherObject uiElement, Action action)
        {
            uiElement.Dispatcher.BeginInvoke(action);
        }
    }
}
