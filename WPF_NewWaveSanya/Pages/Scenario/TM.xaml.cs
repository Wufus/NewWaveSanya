using Ixxat.Vci4.Bal.Can;
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
    public partial class TM : Page
    {
        ICanMessage Tx1, Tx2, Tx3, Tx4;

        public TM()
        {
            InitializeComponent();

            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.UI_MainFrame.GoBack();
        }
    }
}
