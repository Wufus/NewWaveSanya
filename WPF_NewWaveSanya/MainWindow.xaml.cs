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

namespace WPF_NewWaveSanya
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static public MainWindow Instance;
        public MainWindow()
        {
            InitializeComponent();
            Instance = this;
            UI_MainFrame.Navigate(new Pages.Adapters());
        }

        public void Navigate(Page _page)
        {
            UI_MainFrame.Navigate(_page);
        }
    }
}
