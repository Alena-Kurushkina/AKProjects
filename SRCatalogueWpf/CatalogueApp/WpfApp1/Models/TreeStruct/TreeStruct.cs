using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1.Models.TreeStruct
{
    class TreeStruct: INotifyPropertyChanged
    {
        public TreeStruct (DataRow dr)
        {
            DR = dr;
        }

        private DataRow DR;
        public bool IsAnyChanges = false;
        
        public int ID
        {
            get { return Convert.ToInt32(DR[0]); }
            set { DR[0] = value; }
        }

        public string Name
        {
            get { return DR[1].ToString(); }
            set {
                string n = value.ToString().Trim();
              if (DR[1].ToString() != n)
                {
                    DR[1] = n;
                    IsAnyChanges = true;
                }
            }
        }

        public int Pid
        {
            get { return Convert.ToInt32(DR[2]); }
            set {
                if (Convert.ToInt32(DR[2]) != value)
                {
                    DR[2] = value;
                    IsAnyChanges = true;
                }
            }
        }

        public bool Content
        {
            get { return Convert.ToBoolean(DR[3]); }
            set
            {
                if (value == true)
                {
                    DR[3] = 1;
                }
                else
                {                    
                    DR[3] = 0;
                }
                OnPropertyChanged("Content");
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
