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
using WpfApp1.Models;
using WpfApp1.Models.TreeStruct;

namespace WpfApp1.ViewModels.TreeStructPages
{
    class ModelsFileViewModel : IPageViewModel, IDataErrorInfo, INotifyPropertyChanged
    {
        public string Name
        {
            get { return "Файл 3D моделей"; }
        }

        public void Dispose() { }

        
        public void DeleteItem(int id)
        {
            List<ModelsStruct> indexes = new List<ModelsStruct>();
            //List<PicturesStruct> list = _picturesFileItems.ToList().FindAll(i => i.TreeId == id);
            foreach (ModelsStruct item in _modelsFileItems)
            {
                if (item.DR.RowState == DataRowState.Detached)
                {
                    indexes.Add(item);
                }
            }
            foreach (ModelsStruct ind in indexes)
            {
                _modelsFileItems.Remove(ind);
            }
            indexes = null;
        }

        private Database database;

        public ModelsFileViewModel(Database db)
        {
            database = db;
            _modelsFileItems = new ObservableCollection<ModelsStruct>();
            foreach (DataRow dr in db.dataset1.Models)
            {
                _modelsFileItems.Add(new ModelsStruct(dr));
            }
        }

        ObservableCollection<ModelsStruct> _modelsFileItems;

        public ObservableCollection<ModelsStruct> ModelsFileItems
        {
            get
            {
                return _modelsFileItems;
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
                else {
                    ModelsStruct ms = ModelsFileItems.ToList().Find(i => i.ID == Convert.ToInt32(ProjectId));
                    if (ms != null)
                    {
                        IsFilePathAvailable = false;
                        // error = "У раздела есть 3D модель";
                    }
                    else
                    {
                        DataSet1.TreeRow tr = database.dataset1.Tree.FindByid(Convert.ToInt32(ProjectId));
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

        MediaEdit modelME;

        private RelayCommand getModel;
        public RelayCommand GetModelFile
        {
            get
            {
                return getModel ??
                   (getModel = new RelayCommand(obj =>
                   {
                       modelME = new MediaEdit(database, ProjectId);
                       if (modelME.Get3DModelFile())
                       {
                           FilePath = modelME.old_file_name;
                       }
                       else FilePath = null;
                      
                   }
                   ));
            }
        }

        private RelayCommand addModel;
        public RelayCommand AddModel
        {
            get
            {
                return addModel ??
                   (addModel = new RelayCommand(obj =>
                   {
                       DataRow dr = modelME.Add3DModelToProject();
                       if (dr!=null)
                       {
                           ModelsStruct ms = new ModelsStruct(dr);
                           ModelsFileItems.Add(ms);

                           FilePath = null;
                           ProjectId = null;
                       }

                   }
                   ));
            }
        }

        private RelayCommand deleteModel;
        public RelayCommand DeleteModel
        {
            get
            {
                return deleteModel ??
                   (deleteModel = new RelayCommand(obj =>
                   {
                       ModelsStruct ps = obj as ModelsStruct;

                       database.Delete3DModel(ps.Path, ps.ID.ToString());

                       _modelsFileItems.Remove(ps);
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
