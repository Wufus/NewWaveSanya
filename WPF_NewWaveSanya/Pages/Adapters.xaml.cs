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
    /// Логика взаимодействия для Adapters.xaml
    /// </summary>
    public partial class Adapters : Page
    {
        public Adapters()
        {
            InitializeComponent();
        }

        private void AddIxxat_Click(object sender, RoutedEventArgs e)
        {
            Adapter.Ixxat _ixxat = new Adapter.Ixxat { Name = "Test Can" };
            Adapter.Can.AddAdapter(_ixxat);
            UI_NoAdapters.Visibility = Visibility.Collapsed;

            Button b = new Button
            {
                Content = _ixxat.Name,
                Margin = new Thickness(0, 5, 0, 5),
                Style = (Style)Application.Current.Resources["MyButtonStyle"]
            };
            b.Click += AdapterClicked;  
            UI_Adapters.Children.Add(b);
        }

        private void AdapterClicked(object sender, RoutedEventArgs e)
        {
            //if tag property exist
            if (Adapter.Can.SelectAdapter((Adapter.Ixxat)((Button)sender).Tag))
            {
                MainWindow.Instance.Navigate(new Pages.BigIcons());
            }
        }

    }
}
