using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using DevExpress.Xpf.Core;

namespace VoucherCreateApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string ArgUserName { set; get; }
        public static string ArgConnStr { set; get; }
        private void OnAppStartup_UpdateThemeName(object sender, StartupEventArgs e)
        {
            ArgConnStr = e.Args[0];
            ArgUserName = e.Args[1];
            
            DevExpress.Xpf.Core.ApplicationThemeHelper.UpdateApplicationThemeName();
        }

    }
}
