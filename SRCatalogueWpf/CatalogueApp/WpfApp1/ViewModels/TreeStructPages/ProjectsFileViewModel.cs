using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Models;
using WpfApp1.Models.TreeStruct;

namespace WpfApp1.ViewModels.TreeStructPages
{
    class ProjectsFileViewModel: IPageViewModel, IDataErrorInfo, INotifyPropertyChanged
    {
       
        public string Name
        {
            get { return "Файл проектов"; }
        }

        public void Dispose() { }

        public void DeleteItem(int id)
        { }

        public ProjectsFileViewModel(Database db, ObservableCollection<TreeStruct> tfi)
        {
            database = db;
            //_treeFileItems = new ObservableCollection<TreeStruct>();
            //foreach (DataRow dr in db.dataset1.Tree)
            //{
            //    _treeFileItems.Add(new TreeStruct(dr));
            //}
            _treeFileItems = tfi;
            childrenVM = new List<IPageViewModel>();

            
        }
        Database database;

        ObservableCollection<TreeStruct> _treeFileItems;

       public List<IPageViewModel> childrenVM;



        public ObservableCollection<TreeStruct> TreeFileItems
        {
            get
            {
                return _treeFileItems;
            }
            set
            {
                _treeFileItems = value;
                OnPropertyChanged("TreeFileItems");
            }
        }

        string projectName;

        public string ProjectName
        {
            get { return projectName; }
            set
            {
                projectName = value;
                OnPropertyChanged("ProjectName");
                
            }
        }

        string parentId;

        public string ParentId
        {
            get { return parentId; }
            set
            {
                parentId = value;
                OnPropertyChanged("ParentId");
            }
        }

        bool isContent = false;
        public bool IsContent
        {
            get { return isContent; }
            set
            {
                isContent = value;
                OnPropertyChanged("IsContent");
            }
        }

      
        private bool _errors = true;

        public bool IsAnyErrors
        {
            get
            {
                return _errors;
            }
            set
            {
                _errors = value;
                OnPropertyChanged("IsAnyErrors");
            }
        }

        public string this[string columnName]
        {
            get
            {
                string error = String.Empty;
                
                int id;
                if (ProjectName == "" || ProjectName == null || ParentId == "" || ParentId == null || !int.TryParse(ParentId,out id))
                {
                  IsAnyErrors = true;
                  //error = "Заполните поле";
                }
                else IsAnyErrors = false;
               
                return error;
            }
        }
        public string Error
        {
            get { throw new NotImplementedException(); }
        }



        private RelayCommand addProject;
        public RelayCommand AddProject
        {
            get
            {
                return addProject ??
                  (addProject = new RelayCommand(obj =>
                  {
                      DataGrid dg = obj as DataGrid;
                      DataRow dr;

                      if (IsContent)
                      {
                          dr = database.AddProject(ProjectName, ParentId);
                      }
                      else dr = database.AddPazdel(ProjectName, ParentId);

                      TreeStruct ts = new TreeStruct(dr);
                      
                      TreeFileItems.Add(ts);

                      ProjectName = null;
                      ParentId = null;
                      
                  }
              )); 
            }
        }

        private RelayCommand deleteProject;
        public RelayCommand DeleteProject
        {
            get
            {
                return deleteProject ??
                  (deleteProject = new RelayCommand(obj =>
                  {
                      TreeStruct gr = obj as TreeStruct;
                        MessageBoxResult result = System.Windows.MessageBox.Show("Вы действительно хотите удалить проект '" + gr.ID + "'?", "Подтверждение", MessageBoxButton.OKCancel);
                      if (result == MessageBoxResult.OK)
                      {
                          DataRow dr = database.dataset1.Tree.FindByid(gr.ID);
                          DataRow[] dt = database.dataset1.Tree.Select($"Parent_id= '{gr.ID}'");
                          if (dt.Length != 0)
                          {
                              MessageBoxResult res = MessageBox.Show("У этого раздела есть подразделы. Удалить всё?", "Внимание", MessageBoxButton.YesNo);
                              if (res != MessageBoxResult.Yes) return;
                          }
                          DeleteRazdel(dr);
                          database.SaveTreeFile();
                          TreeFileItems.Remove(gr);
                      }
                      
                  }
              ));
            }
        }

        private void DeleteRazdel(DataRow row)
        {
            DataRow[] dt = database.dataset1.Tree.Select($"Parent_id= '{row[0]}'");
            foreach (DataRow dr in dt)
            {
                DeleteRazdel(dr);
            }
            database.DeleteDescription(row[0].ToString());
            foreach (IPageViewModel vm in childrenVM)
            {
                vm.DeleteItem(Convert.ToInt32(row[0]));
            }
            row.Delete();
        }

        private RelayCommand cellEditEnd;
        public RelayCommand CellEditEnd
        {
            get
            {
                return cellEditEnd ??
                  (cellEditEnd = new RelayCommand(obj =>
                  {
                      TreeStruct ps = obj as TreeStruct;
                      if (ps.IsAnyChanges)
                      {
                          database.SaveTreeFile();
                          ps.IsAnyChanges = false;
                      }
                     
                  }
              ));
            }
        }

       

        private RelayCommand uncheckedEvent;
        public RelayCommand UncheckedEvent
        {
            get
            {
                return uncheckedEvent ??
                  (uncheckedEvent = new RelayCommand(obj =>
                  {
                      TreeStruct gr = obj as TreeStruct;
                      if (gr.Content==true) {
                          MessageBoxResult res = MessageBox.Show("Весь контент раздела будет удалён", "Внимание", MessageBoxButton.OKCancel);
                          if (res==MessageBoxResult.OK)
                          {
                              database.DeleteDescription(gr.ID.ToString());

                              foreach (IPageViewModel vm in childrenVM)
                              {
                                  vm.DeleteItem(gr.ID);
                              }
                              
                          }
                          else
                          {
                              gr.Content = true;
                          }
                      }
                      else
                      {
                          try
                          {
                              DescriptionText descText = new DescriptionText(database, gr.ID.ToString());

                          }
                          catch
                          {
                              MessageBox.Show("Не удалось создать проект");
                              gr.Content = false;
                              return;
                          }
                          database.dataset1.Tree.Rows.Find(gr.ID)[3] = 1;
                          database.SaveTreeFile();
                          // gr.Content = true;
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
