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

namespace WPF_NewWaveSanya.Pages
{
    /// <summary>
    /// Логика взаимодействия для BigIcons.xaml
    /// </summary>
    public partial class BigIcons : Page
    {
        public BigIcons()
        {
            InitializeComponent();
        }

        private void ButtonBes_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.Navigate(new Pages.Scenario.Bes());
        }
    }
}
