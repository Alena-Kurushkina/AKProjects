using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Models.TreeStruct
{
    class ContactsStruct
    {
        public ContactsStruct(DataRow dr)
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

        public string Org
        {
            get { return DR[1].ToString(); }
            set
            {
                if (DR[1].ToString() != value.ToString())
                {
                    string n = value.ToString().Trim();
                    DR[1] = n;
                    IsAnyChanges = true;
                }
            }
        }
        public string Fio
        {
            get { return DR[2].ToString(); }
            set
            {
                if (DR[2].ToString() != value.ToString())
                {
                    string n = value.ToString().Trim();
                    DR[2] = n;
                    IsAnyChanges = true;
                }
            }
        }
        public string Tel
        {
            get { return DR[3].ToString(); }
            set
            {
                if (DR[3].ToString() != value.ToString())
                {
                    string n = value.ToString().Trim();
                    DR[3] = n;
                    IsAnyChanges = true;
                }
            }
        }
        public string Email
        {
            get { return DR[4].ToString(); }
            set
            {
                if (DR[4].ToString() != value.ToString())
                {
                    string n = value.ToString().Trim();
                    DR[4] = n;
                    IsAnyChanges = true;
                }
            }
        }
    }
}
