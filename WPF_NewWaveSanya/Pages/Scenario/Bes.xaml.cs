using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF_NewWaveSanya.Pages.Scenario
{
    /// <summary>
    /// Логика взаимодействия для Bes.xaml
    /// </summary>
    public partial class Bes : Page
    {
        public Bes()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.Navigate(new Pages.BigIcons());
        }
    }
}
