using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WpfApp1.Models.TreeStruct;

namespace WpfApp1.ViewModels.TreeStructPages
{
    class PicturesFileViewModel : IPageViewModel, INotifyPropertyChanged, IDataErrorInfo
    {
        public string Name
        {
            get { return "Файл изображений"; }
        }

        public void Dispose() { }

        public void DeleteItem(int id) 
        {
            List<PicturesStruct> indexes = new List<PicturesStruct>();
            //List<PicturesStruct> list = _picturesFileItems.ToList().FindAll(i => i.TreeId == id);
            foreach (PicturesStruct item in _picturesFileItems)
            {
               if (item.DR.RowState==DataRowState.Detached)
               {
                    indexes.Add(item);                  
               }
            }
            foreach(PicturesStruct ind in indexes)
            {
                _picturesFileItems.Remove(ind);
            }
            indexes = null;
        
        }

        Database database;

        public PicturesFileViewModel(Database db)
        {
            database = db;
            _picturesFileItems = new ObservableCollection<PicturesStruct>();
            foreach (DataRow dr in db.dataset1.Pictures)
            {
                _picturesFileItems.Add(new PicturesStruct(dr));
            }
           
        }

       ObservableCollection<PicturesStruct> _picturesFileItems;

        public ObservableCollection<PicturesStruct> PicturesFileItems
        {
            get
            {
                return _picturesFileItems;
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

        private string _prId;
        public string ProjectId
        {
            get { return _prId; }
            set { _prId = value;
                OnPropertyChanged("ProjectId");
            }
        }


        private string desc;
        public string Desc
        {
            get { return desc; }
            set
            {
                desc = value;
                OnPropertyChanged("Desc");
            }
        }

        private bool _avail;
        public bool IsFilePathAvailable
        {
            get { return _avail; }
            set { _avail = value;
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
                    Models.DataSet1.TreeRow tr = database.dataset1.Tree.FindByid(Convert.ToInt32(ProjectId));
                    if ( tr!= null)
                    {
                        if (tr.content == 1) IsFilePathAvailable = true;
                        else IsFilePathAvailable = false;

                    }else IsFilePathAvailable = false;
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

        private string Sourse_str;
        private string Dest_str;
        private string old_file_name;

        private RelayCommand addFile;
        public RelayCommand AddFile
        {
            get
            {
                return addFile ??
                  (addFile = new RelayCommand(obj =>
                  {
                      System.Windows.Forms.OpenFileDialog ofd = new OpenFileDialog();
                      ofd.Filter = "Image Files(*.BMP;*.JPG;*.PNG)|*.BMP;*.JPG;*.PNG";
                      if (ofd.ShowDialog() == DialogResult.OK)
                      {

                          Sourse_str = ofd.FileName.Substring(0, ofd.FileName.Length - ofd.SafeFileName.Length);
                          Dest_str = database.soursePath + database.CataloguePictures;
                          old_file_name = ofd.SafeFileName;

                          string new_file_name = old_file_name;
                          try
                          {
                              while (File.Exists(System.IO.Path.Combine(Dest_str, new_file_name)))
                              {
                                  Rename_dialog rd = new Rename_dialog(new_file_name);
                                  if (rd.ShowDialog() == true)
                                  {
                                      new_file_name = rd.Renamed;
                                  }
                                  else
                                  {
                                     // FilePath = null;
                                      return;
                                  }
                              }
                          }
                          catch (IOException ex)
                          {
                              System.Windows.MessageBox.Show(ex.Message + " Переименуйте файл.", "Ошибка при добавлении изображения");
                              return;
                          }
                          catch (Exception ex)
                          {
                              System.Windows.MessageBox.Show(ex.Message, "Ошибка при переименовании файла");
                              return;
                          }

                          FilePath = new_file_name;
                      }else
                      {
                          //FilePath = null;
                          return;
                      }
                  }
              ));
            }
        }

        private RelayCommand addPicture;
        public RelayCommand AddPicture
        {
            get
            {
                return addPicture ??
                   (addPicture = new RelayCommand(obj =>
                   {
                       DataGrid dg = obj as DataGrid;

                       try
                       {
                           File.Copy(System.IO.Path.Combine(Sourse_str, old_file_name), System.IO.Path.Combine(Dest_str, FilePath));
                       }
                       catch (Exception ex)
                       {
                           System.Windows.MessageBox.Show(ex.Message, "Ошибка при добавлении изображения в Catalogue_Pictures");
                           return;
                       }

                       try
                       {
                           DataRow dr = database.dataset1.Pictures.NewRow();
                           dr["path"] = FilePath;
                           dr["description"] = Desc;
                           dr["tree_id"] = ProjectId;
                           database.dataset1.Pictures.Rows.Add(dr);

                           database.SavePictureFile();

                           PicturesStruct ps = new PicturesStruct(dr);
                           PicturesFileItems.Add(ps);

                           Desc = null;
                           ProjectId = null;
                           FilePath = null;
                          
                       }
                       catch (Exception ex)
                       {
                           System.Windows.MessageBox.Show(ex.Message, "Ошибка при добавлении строки в Pictures.txt");
                           return;
                       }
                   }
                   ));
            }
        }

        private RelayCommand deletePicture;
        public RelayCommand DeletePicture
        {
            get
            {
                return deletePicture ??
                   (deletePicture = new RelayCommand(obj =>
                   {
                       PicturesStruct ps = obj as PicturesStruct;

                       System.Windows.MessageBoxResult result = System.Windows.MessageBox.Show("Вы действительно хотите удалить изображение '" + ps.Path + "'?", "Подтверждение", System.Windows.MessageBoxButton.OKCancel);
                       if (result == System.Windows.MessageBoxResult.OK)
                       {
                           try
                           {
                               System.IO.File.Delete(System.IO.Path.Combine(database.soursePath + database.CataloguePictures, ps.Path));

                           }
                           catch (Exception ex)
                           {
                               MessageBox.Show(ex.Message, "Ошибка при удалении изображения из папки");
                               return;
                           }

                           try
                           {
                               database.dataset1.Pictures.Rows.Find(Convert.ToInt32(ps.ID)).Delete();
                               database.SavePictureFile();

                               PicturesFileItems.Remove(ps);
                               return;
                           }
                           catch (Exception ex)
                           {
                               System.Windows.MessageBox.Show(ex.Message, "Ошибка при удалении записи из Pictures.txt");
                               return;
                           }
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
                      PicturesStruct ps = obj as PicturesStruct;
                      if (ps.IsAnyChanges)
                      {
                          database.SavePictureFile();
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
