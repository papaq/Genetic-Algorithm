using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            }
            else
            {
                Button_StartStop.Content = "Start";
            }
        }

        private void ComboBox_FunctionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBox_FunctionList.Items.Count == 0 || funcInputData == null)
            {
                return;
            }

            if (!Button_StartStop.IsEnabled)
            {
                Button_StartStop.IsEnabled = true;
            }

            int indexChosen = ComboBox_FunctionList.SelectedIndex;

            TextBox_IntervalLeft.Text = funcInputData.GetInterval(indexChosen).intervalLeft.ToString();
            TextBox_IntervalRight.Text = funcInputData.GetInterval(indexChosen).intervalRight.ToString();

            TextBox_GlobalOptimum.Text = funcInputData.GetGlobalOptimum(indexChosen).ToString();
        }
    }
}
