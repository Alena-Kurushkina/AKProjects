using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Models.TreeStruct
{
    class PicturesStruct
    {
        public PicturesStruct(DataRow dr)
        {
            DR = dr;           
            
        }        

        public DataRow DR;
        public bool IsAnyChanges = false;

        public int ID
        {
            get { return Convert.ToInt32(DR[0]); }
            set { DR[0] = value; }
        }

        public string Path
        {
            get { return DR[1].ToString(); }
            set
            {
                string n = value.ToString().Trim();
                DR[1] = n;
            }
        }

        public string Description
        {
            get { return DR[2].ToString(); }
            set {
                string n = value.ToString().Trim();
                if (DR[2].ToString()!= n)
                {
                    DR[2] = n;
                    IsAnyChanges = true;
                }
            }
        }

        public int TreeId
        {
            get { return Convert.ToInt32(DR[3]); }
            set {
                if (Convert.ToInt32(DR[3]) != value)
                {
                    DR[3] = value;
                    IsAnyChanges = true;
                }
            }
        }

       
    }
}
