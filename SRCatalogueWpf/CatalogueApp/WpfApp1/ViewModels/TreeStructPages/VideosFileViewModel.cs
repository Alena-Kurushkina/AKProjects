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
    class VideosFileViewModel : IPageViewModel, INotifyPropertyChanged, IDataErrorInfo
    {
        public string Name
        {
            get { return "Файл видео"; }
        }

        public void Dispose() { }

        public void DeleteItem(int id)
        {
            List<VideosStruct> indexes = new List<VideosStruct>();
            //List<PicturesStruct> list = _picturesFileItems.ToList().FindAll(i => i.TreeId == id);
            foreach (VideosStruct item in _videosFileItems)
            {
                if (item.DR.RowState == DataRowState.Detached)
                {
                    indexes.Add(item);
                }
            }
            foreach (VideosStruct ind in indexes)
            {
                _videosFileItems.Remove(ind);
            }
            indexes = null;
        }

        public VideosFileViewModel(Database db)
        {
            database = db;
            _videosFileItems = new ObservableCollection<VideosStruct>();
            foreach (DataRow dr in db.dataset1.Videos)
            {
                _videosFileItems.Add(new VideosStruct(dr));
            }
        }
        private Database database;

        ObservableCollection<VideosStruct> _videosFileItems;

        public ObservableCollection<VideosStruct> VideosFileItems
        {
            get
            {
                return _videosFileItems;
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
            set
            {
                _prId = value;
                OnPropertyChanged("ProjectId");
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
                    VideosStruct ms = VideosFileItems.ToList().Find(i => i.ID == Convert.ToInt32(ProjectId));
                    if (ms != null)
                    {
                        IsFilePathAvailable = false;
                        // error = "У раздела есть 3D модель";
                    }
                    else
                    {
                        Models.DataSet1.TreeRow tr = database.dataset1.Tree.FindByid(Convert.ToInt32(ProjectId));
                        if (tr != null)
                        {
                            if (tr.content != 1) IsFilePathAvailable = false;
                            else IsFilePathAvailable = true;
                        }else IsFilePathAvailable = false;
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

        private MediaEdit meded;

        private RelayCommand addFile;
        public RelayCommand AddFile
        {
            get
            {
                return addFile ??
                  (addFile = new RelayCommand(obj =>
                  {
                       meded = new MediaEdit(database, ProjectId);
                      if (meded.GetVideoFile())
                      {
                          FilePath = meded.old_video_path;
                      }
                  }
              ));
            }
        }

        private RelayCommand addToProject;
        public RelayCommand AddToProject
        {
            get
            {
                return addToProject ??
                  (addToProject = new RelayCommand(obj =>
                  {
                      DataRow dr = meded.AddVideoToProject();
                      VideosStruct vs = new VideosStruct(dr);
                      VideosFileItems.Add(vs);

                      ProjectId = null;
                      FilePath = null;

                  }
              ));
            }
        }

        private RelayCommand deleteVideo;
        public RelayCommand DeleteVideo
        {
            get
            {
                return deleteVideo ??
                  (deleteVideo = new RelayCommand(obj =>
                  {
                      VideosStruct vs = obj as VideosStruct;
                      database.DeleteVideo(vs.Path, vs.ID.ToString());
                      VideosFileItems.Remove(vs);

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
