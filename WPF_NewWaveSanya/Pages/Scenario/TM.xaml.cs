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
using WPF_NewWaveSanya.Adapter;

namespace WPF_NewWaveSanya.Pages.Scenario
{
    public partial class TM : Page
    {
        //ICanMessage Tx1, Tx2, Tx3, Tx4;
        Boolean IsRed;
        Boolean IsGreen;
        Boolean IsBlue;
        Byte Colored;

        public TM()
        {
            InitializeComponent();
            UI_TMText1.TextChanged += TMText_TextChanged;
            UI_TMText2.TextChanged += TMText_TextChanged;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.UI_MainFrame.GoBack();
        }

        private void ButtonIsBlue_Click(object sender, RoutedEventArgs e)
        {
            IsBlue = !IsBlue;
            UI_ColoredBlue.Background = new SolidColorBrush(IsBlue ? Colors.LightSkyBlue : Colors.WhiteSmoke);
            SendPacks();
        }
        private void ButtonIsRed_Click(object sender, RoutedEventArgs e)
        {
            IsRed = !IsRed;
            UI_ColoredRed.Background = new SolidColorBrush(IsRed ? Colors.Salmon : Colors.WhiteSmoke);
            SendPacks();
        }

        private void TMText_TextChanged(object sender, TextChangedEventArgs e)
        {
            SendTMPacks();
        }

        private void ButtonIsGreen_Click(object sender, RoutedEventArgs e)
        {
            IsGreen = !IsGreen;
            UI_ColoredGreen.Background = new SolidColorBrush(IsGreen ? Colors.PaleGreen : Colors.WhiteSmoke);
            SendPacks();
        }

        private void ButtonSetTMEnd_Click(object sender, RoutedEventArgs e)
        {
            IsRed = true;
            IsGreen = true;
            IsBlue = false;
            UI_ColoredRed.Background = new SolidColorBrush(Colors.Salmon);
            UI_ColoredGreen.Background = new SolidColorBrush(Colors.PaleGreen);
            UI_ColoredBlue.Background = new SolidColorBrush(Colors.WhiteSmoke);
            UI_TMText1.Text = "1234567890";
            UI_TMText2.Text = "";
            CalculateColor();
            SendTMPacks();
        }
        private void CalculateColor()
        {
            Colored = (Byte)((IsBlue ? 8 : 0) + (IsGreen ? 0x80 : 0) + (IsRed ? 0x40 : 0));
        }
        private void SendPacks()
        {
            CalculateColor();
            SendTMPacks();
            SendTLPacks();
        }

        private void SendTMPacks()
        {
            Byte[] s = new byte[30];
            Byte[] txt = Encoding.GetEncoding("windows-1251").GetBytes(UI_TMText1.Text.ToCharArray());
            for (int i = 0; i < txt.Length; i++)
                s[i] = txt[i];
            Byte[] txt2 = Encoding.GetEncoding("windows-1251").GetBytes(UI_TMText2.Text.ToCharArray());
            for (int i = 0; i < txt2.Length; i++)
                s[i + 15] = txt2[i];
            Byte length = (Byte)((txt.Length << 4) + (txt2.Length));
            Can.TransmitData8(0x27A, new byte[] { Colored, length, s[0], s[1], s[2], s[3], s[4], s[5] });
            Can.TransmitData8(0x37A, new byte[] { s[6], s[7], s[8], s[9], s[10], s[11], s[12], s[13] });
            Can.TransmitData8(0x47A, new byte[] { s[14], s[15], s[16], s[17], s[18], s[19], s[20], s[21] });
            Can.TransmitData8(0x57A, new byte[] { s[22], s[23], s[24], s[25], s[26], s[27], s[28], s[29] });

        }
        private void SendTLPacks()
        {
            Can.TransmitData8(0x27c, new byte[] { Colored, 0, 0, 0, 0, 0, 0, 0 });
            Can.TransmitData8(0x37c, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 });
            Can.TransmitData8(0x47c, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 });
            Can.TransmitData8(0x57c, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 });
        }
    }
}
