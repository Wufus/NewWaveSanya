using Ixxat.Vci4;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPF_NewWaveSanya.Adapter;

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

            DeviceListMonitor.ListChanged += DeviceListMonitor_ListChanged;
            DeviceListMonitor.StartDeviceListMonitor();

        }

        private void DeviceListMonitor_ListChanged()
        {
            Dispatcher.BeginInvoke(new ThreadStart(delegate
            {
                // прописываешь, что сделать в главном окне
                if (DeviceListMonitor.Devices.Count == 0)
                {
                    UI_Adapters.Children.Clear();
                    UI_NoAdapters.Visibility = Visibility.Visible;
                }
                else
                {
                    UI_Adapters.Children.Clear();
                    UI_NoAdapters.Visibility = Visibility.Collapsed;
                    foreach (String s in DeviceListMonitor.Devices)
                    {
                        Button b = new Button
                        {
                            Content = s,
                            Margin = new Thickness(0, 5, 0, 5),
                            Style = (Style)Application.Current.Resources["MyButtonStyle"]
                        };
                        b.Click += AdapterClicked;
                        UI_Adapters.Children.Add(b);
                    }
                }
            }));
        }

        private void AddIxxat_Click(object sender, RoutedEventArgs e)
        {
            /*Adapter.Ixxat _ixxat = new Adapter.Ixxat { Name = "Test Can" };
            Adapter.Can.AddAdapter(_ixxat);
            UI_NoAdapters.Visibility = Visibility.Collapsed;

            Button b = new Button
            {
                Content = _ixxat.Name,
                Margin = new Thickness(0, 5, 0, 5),
                Style = (Style)Application.Current.Resources["MyButtonStyle"]
            };
            b.Click += AdapterClicked;  
            UI_Adapters.Children.Add(b);*/
        }

        private void AdapterClicked(object sender, RoutedEventArgs e)
        {
            // TODO: передача строки с именем адаптера таким способм недопустима. необходимо использовать Tag
            if(Can.InitSocket(((Button)sender).Content.ToString(), 125))
            {
                MainWindow.Instance.Navigate(new Pages.BigIcons());
            }
        }

    }
}
