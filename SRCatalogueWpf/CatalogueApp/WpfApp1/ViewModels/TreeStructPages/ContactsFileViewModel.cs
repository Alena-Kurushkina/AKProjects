using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WpfApp1.Models.TreeStruct;

namespace WpfApp1.ViewModels.TreeStructPages
{
    class ContactsFileViewModel: IPageViewModel, INotifyPropertyChanged, IDataErrorInfo
    {
        
            public string Name
            {
                get { return "Файл контактов"; }
            }

            public void Dispose() { }

        public void DeleteItem(int id)
        {
            List<ContactsStruct> indexes = new List<ContactsStruct>();
            //List<PicturesStruct> list = _picturesFileItems.ToList().FindAll(i => i.TreeId == id);
            foreach (ContactsStruct item in _contactsFileItems)
            {
                if (item.DR.RowState == DataRowState.Detached)
                {
                    indexes.Add(item);
                }
            }
            foreach (ContactsStruct ind in indexes)
            {
                _contactsFileItems.Remove(ind);
            }
            indexes = null;
        }

        private Database database;

            public ContactsFileViewModel(Database db)
            {
                database = db;
                _contactsFileItems = new ObservableCollection<ContactsStruct>();
                foreach (DataRow dr in db.dataset1.Contact)
                {
                    _contactsFileItems.Add(new ContactsStruct(dr));
                }
            }

            ObservableCollection<ContactsStruct> _contactsFileItems;

            public ObservableCollection<ContactsStruct> ContactsFileItems
            {
                get
                {
                    return _contactsFileItems;
                }
            }

        private string _prId;
        public string ProjectId
        {
            get { return _prId; }
            set
            {
                _prId = value;
                OnPropertyChanged("ProjectId");
            }
        }

        private string _fio;
        public string Fio
        {
            get { return _fio; }
            set
            {
                _fio = value;
                OnPropertyChanged("Fio");
            }
        }

        private string _tel;
        public string Tel
        {
            get { return _tel; }
            set
            {
                _tel = value;
                OnPropertyChanged("Tel");
            }
        }
        private string _email;
        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                OnPropertyChanged("Email");
            }
        }
        private string _org;
        public string Org
        {
            get { return _org; }
            set
            {
                _org = value;
                OnPropertyChanged("Org");
            }
        }

        private bool _bavail;
        public bool IsButtonAvailable
        {
            get { return _bavail; }
            set
            {
                _bavail = value;
                OnPropertyChanged("IsButtonAvailable");
            }
        }

        public string this[string columnName]
        {
            get
            {
                string error = String.Empty;               

                int id;

                if (ProjectId == "" || ProjectId == null || !int.TryParse(ProjectId, out id))
                {
                    IsButtonAvailable = false;
                    //error = "Заполните поле";
                }
                else { 
                    ContactsStruct ms = ContactsFileItems.ToList().Find(i => i.ID == Convert.ToInt32(ProjectId));
                    if (ms != null)
                    {
                        IsButtonAvailable = false;
                        // error = "У раздела есть контакт";
                    }
                    else
                    {
                        Models.DataSet1.TreeRow tr = database.dataset1.Tree.FindByid(Convert.ToInt32(ProjectId));
                        if (tr != null) { 
                            if (tr.content != 1) IsButtonAvailable = false;
                            else IsButtonAvailable = true;
                        }else IsButtonAvailable = false;
                    }
                }  

                return error;
            }
        }
        public string Error
        {
            get { throw new NotImplementedException(); }
        }

        private RelayCommand addContact;
        public RelayCommand AddContact
        {
            get
            {
                return addContact ??
                   (addContact = new RelayCommand(obj =>
                   {
                       DataGrid dg = obj as DataGrid;
                       DataRow dr = database.dataset1.Contact.NewRow();
                       dr[0] = ProjectId.Trim();
                       if (Org == null) dr[1] = ""; else dr[1] = Org.Trim();
                       if (Fio == null) dr[2] = ""; else dr[2] = Fio.Trim();
                       if (Tel == null) dr[3] = ""; else dr[3] = Tel.Trim();
                       if (Email == null) dr[4] = ""; else dr[4] = Email.Trim();
                       database.dataset1.Contact.Rows.Add(dr);
                       database.SaveContactsFile();

                       ContactsStruct cs = new ContactsStruct(dr);
                       ContactsFileItems.Add(cs);

                       ProjectId = null;
                       Org = null;
                       Fio = null;
                       Tel = null;
                       Email = null;


                   }
                   )); 
            }
        }

        private RelayCommand deleteContact;
        public RelayCommand DeleteContact
        {
            get
            {
                return deleteContact ??
                   (deleteContact = new RelayCommand(obj =>
                   {
                       ContactsStruct ps = obj as ContactsStruct;
                       System.Windows.MessageBoxResult result = System.Windows.MessageBox.Show("Вы действительно хотите удалить контакт '" + ps.Org+" "+ps.Fio + "'?", "Подтверждение", System.Windows.MessageBoxButton.OKCancel);
                       if (result == System.Windows.MessageBoxResult.OK)
                       {
                           database.dataset1.Contact.Rows.Find(ps.ID).Delete();
                           database.SaveContactsFile();

                           ContactsFileItems.Remove(ps);
                       }

                   }
                   ));
            }
        }

        private RelayCommand cellEditEnd;
        public RelayCommand CellEditEnd
        {
            get
            {
                return cellEditEnd ??
                  (cellEditEnd = new RelayCommand(obj =>
                  {
                      ContactsStruct ps = obj as ContactsStruct;
                      if (ps.IsAnyChanges)
                      {
                          database.SaveContactsFile();
                          ps.IsAnyChanges = false;
                      }

                  }
              ));
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
