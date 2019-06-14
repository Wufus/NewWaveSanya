﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace WPF_NewWaveSanya
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Adapter.Can.FinalizeApp();
            Adapter.DeviceListMonitor.StopDeviceListMonitor();
        }
    }
}
