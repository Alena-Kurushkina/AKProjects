using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfApp1.Views;

namespace WpfApp1.ViewModels
{
    class CataloguesPageViewModel : INotifyPropertyChanged
    {
        public CataloguesPageViewModel(Database db) {
            database = db;
        }

        
        Database database;

        public Visibility IsAdmin
        {
            get {
                return database.IsAdmin;
            }
            set
            {
                database.IsAdmin = value;
                OnPropertyChanged("IsAdmin");
            }
        }

                
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
