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

namespace GPStandingsGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Controllers.GPStandings gpStandings1;
        Controllers.GPStandings gpStandings2;

        public MainWindow()
        {
            gpStandings1 = new Controllers.GPStandings();
            gpStandings2 = new Controllers.GPStandings();
            InitializeComponent();
        }

        private async void SubmitButton1_Click(object sender, RoutedEventArgs e)
        {
            string message = gpStandings1.CheckEntry(searchTextBox1.Text);

            if (message != null)
            {
                MessageBox.Show(message);
            }
            else
            {
                int year = int.Parse(searchTextBox1.Text);

                titleTextBlock1.Text = $"{year}";

                resultsDataGrid1.ItemsSource = await gpStandings1.GetStandings(year);

                if (gpStandings1.isSearchingConstructors)
                {
                    resultsDataGrid1.Columns[1].Visibility = Visibility.Hidden;
                }

                //await gpStandings1.GetStandings(year);

                //if (gpStandings1.isSearchingConstructors == true)
                //{
                //    DataContext = gpStandings1.constructorsCollection;
                //}

                //else
                //{
                //    DataContext = gpStandings1.driversCollection;
                //}
            }
        }

        private void ConstructorRadioButton1_Checked(object sender, RoutedEventArgs e)
        {
            gpStandings1.isSearchingConstructors = true;
        }

        private void DriverRadioButton1_Checked(object sender, RoutedEventArgs e)
        {
            gpStandings1.isSearchingConstructors = false;
        }

        private async void SubmitButton2_Click(object sender, RoutedEventArgs e)
        {
            string message = gpStandings2.CheckEntry(searchTextBox2.Text);

            if (message != null)
            {
                MessageBox.Show(message);
            }
            else
            {
                int year = int.Parse(searchTextBox2.Text);

                titleTextBlock2.Text = $"{year}";

                resultsDataGrid2.ItemsSource = await gpStandings2.GetStandings(year);

                if (gpStandings2.isSearchingConstructors)
                {
                    resultsDataGrid2.Columns[1].Visibility = Visibility.Hidden;
                }

                //await gpStandings2.GetStandings(year);

                //if (gpStandings2.isSearchingConstructors == true)
                //{
                //    DataContext = gpStandings2.constructorsCollection;
                //}

                //else
                //{
                //    DataContext = gpStandings2.driversCollection;
                //}
            }
        }

        private void ConstructorRadioButton2_Checked(object sender, RoutedEventArgs e)
        {
            gpStandings2.isSearchingConstructors = true;
        }

        private void DriverRadioButton2_Checked(object sender, RoutedEventArgs e)
        {
            gpStandings2.isSearchingConstructors = false;
        }
    }
}
