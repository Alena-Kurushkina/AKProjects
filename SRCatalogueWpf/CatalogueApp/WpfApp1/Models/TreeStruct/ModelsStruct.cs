using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Models.TreeStruct
{
    class ModelsStruct
    {
        public ModelsStruct(DataRow dr)
        {
            DR = dr;
        }

        public DataRow DR;

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

    }
}
