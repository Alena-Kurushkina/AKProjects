using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            string path = @"data\ExceptionsList.txt";
            string appendText = Environment.NewLine + e.Exception.TargetSite.DeclaringType +" "+ e.Exception.TargetSite + " Исключение " +e.Exception.GetType().ToString() +" "+ e.Exception.Message +" "+ DateTime.Now + Environment.NewLine;
            File.AppendAllText(path, appendText);
        }        
        
    }
}
