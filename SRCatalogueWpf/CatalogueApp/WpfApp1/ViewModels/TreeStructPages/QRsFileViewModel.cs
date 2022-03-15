using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Models;
using WpfApp1.Models.TreeStruct;

namespace WpfApp1.ViewModels.TreeStructPages
{
    class QRsFileViewModel : IPageViewModel, IDataErrorInfo, INotifyPropertyChanged
    {
        public string Name
        {
            get { return "Файл QR кодов"; }
        }

        public void Dispose() { }

        public void DeleteItem(int id)
        {
            List<QRStruct> indexes = new List<QRStruct>();
            //List<PicturesStruct> list = _picturesFileItems.ToList().FindAll(i => i.TreeId == id);
            foreach (QRStruct item in _qrsFileItems)
            {
                if (item.DR.RowState == DataRowState.Detached)
                {
                    indexes.Add(item);
                }
            }
            foreach (QRStruct ind in indexes)
            {
                _qrsFileItems.Remove(ind);
            }
            indexes = null;
        }

        public QRsFileViewModel(Database db)
        {
            database = db;
            _qrsFileItems = new ObservableCollection<QRStruct>();
            foreach (DataRow dr in db.dataset1.QRs)
            {
                _qrsFileItems.Add(new QRStruct(dr));
            }
        }

        ObservableCollection<QRStruct> _qrsFileItems;
        private Database database;
        public ObservableCollection<QRStruct> QRsFileItems
        {
            get
            {
                return _qrsFileItems;
            }
        }

        private bool _avail;
        public bool IsFilePathAvailable
        {
            get { return _avail; }
            set
            {
                _avail = value;
                OnPropertyChanged("IsFilePathAvailable");
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

        private string _filePath;

        public string FilePath
        {
            get { return _filePath; }
            set
            {
                _filePath = value;
                OnPropertyChanged("FilePath");
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
                    IsFilePathAvailable = false;
                    //error = "Заполните поле";
                }
                else
                {
                    QRStruct ms = QRsFileItems.ToList().Find(i => i.ID == Convert.ToInt32(ProjectId));
                    if (ms != null)
                    {
                        IsFilePathAvailable = false;
                        // error = "У раздела есть QR код";
                    }
                    else
                    {
                        Models.DataSet1.TreeRow tr = database.dataset1.Tree.FindByid(Convert.ToInt32(ProjectId));
                        if (tr != null)
                        {
                            if (tr.content != 1) IsFilePathAvailable = false;
                            else IsFilePathAvailable = true;
                        }
                        else IsFilePathAvailable = false;
                    }                    
                }

                if (FilePath == null)
                {
                    IsButtonAvailable = false;
                }
                else IsButtonAvailable = true;

                return error;
            }
        }
        public string Error
        {
            get { throw new NotImplementedException(); }
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

        private RelayCommand addFile;
        public RelayCommand AddFile
        {
            get
            {
                return addFile ??
                  (addFile = new RelayCommand(obj =>
                  {
                      DataRow dr = med.AddQRCode();
                      QRStruct vs = new QRStruct(dr);
                      QRsFileItems.Add(vs);

                      ProjectId = null;
                      FilePath = null;
                  }
              ));
            }
        }

       private MediaEdit med;

        private RelayCommand getQRFile;
        public RelayCommand GetQRFile
        {
            get
            {
                return getQRFile ??
                  (getQRFile = new RelayCommand(obj =>
                  {
                      med = new MediaEdit(database, ProjectId);
                      if (med.GetQRFile())
                      {
                          FilePath = med.old_file_name;
                      }
                  }
              ));
            }
        }

        private RelayCommand deleteQR;
        public RelayCommand DeleteQR
        {
            get
            {
                return deleteQR ??
                  (deleteQR = new RelayCommand(obj =>
                  {
                      QRStruct qr = obj as QRStruct;
                     if (database.DeleteQR(qr.Path, qr.ID.ToString()))
                      {
                          QRsFileItems.Remove(qr);
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
