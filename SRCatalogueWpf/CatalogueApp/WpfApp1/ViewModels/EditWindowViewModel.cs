
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using HelixToolkit.Wpf.SharpDX.Assimp;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using WpfApp1.Models;
using WpfApp1.ViewModel;
using WpfApp1.Views;

namespace WpfApp1.ViewModels
{
    class EditWindowViewModel: INotifyPropertyChanged
    {
       
        private DataRow[] ImagesDataRows;
        public DataRow TextDataRow;
        private DataRow ModelDataRow;
        private DataRow QRCodDataRow;
        private DataRow VideoDataRow;
        private DataRow ContactDataRow;
        public Database database;

        public CNodeViewModel SelectedNode;
        
        //private string _model_name;
        readonly string Project_Id;
        private string Project_name;
        public ObservableCollection<Image> Images { get; set; }
        private Image _selectedImage;
        public DescriptionText descText;
        private FlowDocument _doc;
        private string _QR_cod;
        public string model_path;
        private string video_path;
        private string _contOrg;
        private string _contFio;
        private string _contTel;
        private string _contEmail;

       // private string OpenFileFilter = $"{HelixToolkit.Wpf.SharpDX.Assimp.Importer.SupportedFormatsString}";

        public EditWindowViewModel(CNodeViewModel selectedNode)
        {
            SelectedNode = selectedNode;
            database = selectedNode.Database;
            Project_Id = selectedNode.ID;            
            Project_name = selectedNode.Database.GetProjectName(Project_Id);

            Images = new ObservableCollection<Image>();
            descText = new DescriptionText(database, Project_Id);
            _doc = descText.Doc;

            DataRow dr = selectedNode.Database.dataset1.Contact.NewRow();
            dr["id"] = selectedNode.ID;
            dr["organization"] = "";
            dr["fio"] = "";
            dr["telephone"] = "";
            dr["email"] = "";
            selectedNode.Database.dataset1.Contact.Rows.Add(dr);
            database.SaveContactsFile();
        }

        public EditWindowViewModel(ShowContentWindowViewModel content)
        {
                       
            ImagesDataRows = content.ImagesDataRows;
            
            TextDataRow = content.TextDataRow;
            
            ModelDataRow = content.ModelDataRow;
            
            QRCodDataRow = content.QRcodDataRow;

            VideoDataRow = content.VideosDataRow;

            ContactDataRow = content.ContactDataRow;

            database = content.database;

            Project_Id = content.ProjectID;
            ProjectName = content.ProjectName;

            SelectedNode = content.SelectedNode;    
            


            Images = new ObservableCollection<Image>(); 
            foreach (DataRow dr in ImagesDataRows)
            {
                if (File.Exists(database.soursePath+database.CataloguePictures + dr[1].ToString()))
                {
                    Image im = new Image(Project_Id, database) { id = dr[0].ToString(), Title = dr[2].ToString(), Path = database.soursePath + database.CataloguePictures + dr[1].ToString() };
                    Images.Add(im);
                }
               
            };
            if (Images.Count != 0)
            {
                Images = new ObservableCollection<Image>(Images.OrderBy(u => u.id));
            }

            descText = content.DescriptionText;
            _doc = descText.Doc;

            if (QRCodDataRow != null)
            {
                if (File.Exists(database.soursePath + database.CatalogueQR + QRCodDataRow[1].ToString()))
                {
                    _QR_cod = database.soursePath + database.CatalogueQR + QRCodDataRow[1].ToString();
                }
            }
            else _QR_cod = null;

            if (ModelDataRow != null)
            {
                model_path = ModelDataRow[1].ToString();

            }
            else  model_path = null;

            if (VideoDataRow != null)
            {
                string p = database.soursePath + database.CatalogueVideos + VideoDataRow[1].ToString();
                if (File.Exists(p))
                {
                    video_path = VideoDataRow[1].ToString();
                }
            }
            else video_path = null;

            if (ContactDataRow != null)
            {
                _contOrg = ContactDataRow[1].ToString();
                _contFio = ContactDataRow[2].ToString();
                _contTel = ContactDataRow[3].ToString();
                _contEmail = ContactDataRow[4].ToString();
            }
        }

        private bool _isFlipped = false;

        public bool IsFlipped
        {
            get { return _isFlipped; }
            set
            {
                _isFlipped = value;
                OnPropertyChanged("IsFlipped");
            }
        }
        public string ProjectName
        {
            get { return Project_name; }
            set
            {
                Project_name = value;
                OnPropertyChanged("ProjectName");
            }
        }



        public string QRCod
        {
            get
            {
                return _QR_cod;                
            }
            set
            {
                _QR_cod = value;
                OnPropertyChanged("QRCod");
            }
        }

        public string Video
        {
            get
            {
                if (video_path == null) return "Нет";
                else  return video_path;
            }
            set
            {
                video_path = value;
                OnPropertyChanged("Video");
            }
        }

        public string Model_name
        {
            get
            {
                if (model_path == null) return "Нет";
                else return model_path;
            }
            set
            {
                model_path = value;
                OnPropertyChanged("Model_name");
            }
        }


        //public void Dispose()
        //{
        //    Images.Clear();
        //}

        public string ContOrg
        {
            get { return _contOrg; }
            set
            {
                _contOrg = value;
                OnPropertyChanged("ContOrg");
            }
        }
        public string ContFio
        {
            get { return _contFio; }
            set
            {
                _contFio = value;
                OnPropertyChanged("ContFio");
            }
        }
        public string ContTel
        {
            get { return _contTel; }
            set
            {
                _contTel = value;
                OnPropertyChanged("ContTel");
            }
        }
        public string ContEmail
        {
            get { return _contEmail; }
            set
            {
                _contEmail = value;
                OnPropertyChanged("ContEmail");
            }
        }

        public Image SelectedImage
        {
            get { return _selectedImage; }
            set
            {
                _selectedImage = value;
                OnPropertyChanged("SelectedImage");

            }
        }

        public FlowDocument Document
        {
            get { return _doc; }
            set
            {
                _doc = value;
                OnPropertyChanged("Document");
            }
        }


        //private RelayCommand changeImName;
        //public RelayCommand ChangeImName
        //{
        //    get
        //    {
        //        return changeImName ??
        //          (changeImName = new RelayCommand(obj =>
        //          {
        //              IsFlipped = false;
        //          }
        //         )); 
        //    }
        //}


        private RelayCommand removeCommand;
        public RelayCommand RemoveCommand
        {
            get
            {
                return removeCommand ??
                  (removeCommand = new RelayCommand(obj =>
                  {
                      Image Image_toDelete = obj as Image;
                      if (Image_toDelete != null)
                      {
                          bool res = database.DeleteImage(Image_toDelete);
                          if (res)
                          { 
                              Images.Remove(Image_toDelete);
                              System.Windows.MessageBox.Show("Изображение успешно удалено");

                          }                        
                      }
                  },
                 (obj) => Images.Count > 0)); //
            }
        }

        private RelayCommand addCommand;
        public RelayCommand AddCommand
        {
            get
            {
                return addCommand ??
                  (addCommand = new RelayCommand(obj =>
                  {
                      Image im = database.AddPicture(Project_Id);
                      Images.Add(im);
                      System.Windows.MessageBox.Show("Изображение успешно добавлено");

                                          
                  }
                 )); 
            }
        }

        private bool ChangeName(string Name)
        {
            try {

               database.dataset1.Tree.Rows.Find(Convert.ToInt32(this.Project_Id))["Name"]=Name;
               database.SaveTreeFile();
                
                return true;
            
            }
            catch(Exception e)
            {
                System.Windows.MessageBox.Show(e.Message, "Ошибка при изменении названия проекта");
                return false;
            }            
          
        }

        private RelayCommand projectNameChange;
        public RelayCommand ProjectNameChange
        {
            get
            {
                return projectNameChange ??
                  (projectNameChange = new RelayCommand(obj =>
                  {
                      string NewName = obj as string;
                      if ((NewName != null)&&(NewName!=ProjectName))
                      {
                         string name = NewName.Trim();
                          if (this.ChangeName(name))
                          {
                              this.ProjectName = name;
                              System.Windows.MessageBox.Show("Название успешно изменено");
                          }                         
                      }
                  },
                 (obj) => obj.ToString().Trim()!="")); //
            }
        }

        private RelayCommand deleteQRCod;
        public RelayCommand DeleteQRCod
        {
            get
            {
                return deleteQRCod ??
                  (deleteQRCod = new RelayCommand(obj =>
                  {
                      
                      string qr_path = obj as string;
                      if (qr_path != null)
                      {
                          if (database.DeleteQR(qr_path, this.Project_Id))
                          {
                              QRCod = null;
                              System.Windows.MessageBox.Show("Изображение QR кода успешно удалено");
                          }
                      }
                  },
                 (obj) => QRCod!=null)); //
            }
        }

       

        private RelayCommand addQRCod;
        public RelayCommand AddQRCod
        {
            get
            {
                return addQRCod ??
                  (addQRCod = new RelayCommand(obj =>
                  {
                      string qr_name = database.AddQR(this.Project_Id);

                      QRCod = database.soursePath+database.CatalogueQR+qr_name;
                      System.Windows.MessageBox.Show("QR код успешно добавлен");
                         
                  },
                 (obj) => QRCod == null)); 
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
                      string path = obj as string;
                      if (path != null)
                      {
                           if (database.Delete3DModel(path, this.Project_Id))
                           {
                              Model_name = null;
                              System.Windows.MessageBox.Show("3D модель удалена");

                           }
                      }
                  },
                 (obj) => model_path != null)); 
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
                      string model_name = database.Add3DModel(this.Project_Id);
                      if (model_name!=null)
                      {                         
                              Model_name = model_name;
                              System.Windows.MessageBox.Show("3D модель успешно добавлена");
                      }
                         
                  },
                 (obj) => model_path == null));
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
                      //BitmapImage bmImage = obj as BitmapImage;
                      // string qr_path = bmImage.UriSource.ToString();
                      string video_path = obj as string;
                      if (video_path != null)
                      {
                          
                              if (database.DeleteVideo(video_path, this.Project_Id))
                              {       
                                      Video = null;
                                      System.Windows.MessageBox.Show("Видео успешно удалено");                                 
                              }
                         
                      }
                  },
                 (obj) => video_path != null)); //
            }
        }

       
        private RelayCommand addVideo;
        public RelayCommand ComAddVideo
        {
            get
            {
                return addVideo ??
                  (addVideo = new RelayCommand(obj =>
                  {
                      string video_name = database.AddVideo(this.Project_Id);
                      if (video_name != null)
                      {
                              Video = video_name;
                              System.Windows.MessageBox.Show("Видео успешно добавлено");
                         
                      }
                  },
                 (obj) => video_path == null));
            }
        }


        private RelayCommand changeContact;
        public RelayCommand ChangeContact
        {
            get
            {
                return changeContact ??
                  (changeContact = new RelayCommand(obj =>
                  {
                      bool flag = false;

                      DataRow ContactDataRow = database.dataset1.Contact.Rows.Find(this.Project_Id);
                      if (ContactDataRow == null)
                      {
                          ContactDataRow = database.dataset1.Contact.NewRow();
                          ContactDataRow["id"] = this.Project_Id;
                          database.dataset1.Contact.Rows.Add(ContactDataRow);                          
                      }

                      if (ContOrg != ContactDataRow[1].ToString())
                      {
                          ContactDataRow["organization"] = ContOrg;
                          flag = true;
                      }

                      if (ContFio != ContactDataRow[2].ToString())
                      {
                          ContactDataRow["fio"] = ContFio;
                          flag = true;
                      }

                      if (ContTel != ContactDataRow[3].ToString())
                      {
                          ContactDataRow["telephone"] = ContTel;
                          flag = true;
                      }

                      if (ContEmail != ContactDataRow[4].ToString())
                      {
                          ContactDataRow["email"] = ContEmail;
                          flag = true;
                      }

                      if (flag)
                      {
                          database.SaveContactsFile();
                          System.Windows.MessageBox.Show("Изменения сохранены");
                      }

                  }));
            }
        }

        

        public void DeleteProject()
        {
            
                //-------удаление изображений--------------
                if (Images.Count != 0)
                {
                    foreach (Image pict in Images)
                    {
                        if (pict.Delete())
                        {
                            database.dataset1.Pictures.Rows.Find(Convert.ToInt32(pict.id)).Delete();
                        }
                    }
                    database.SavePictureFile();

                    Images.Clear();
                }
                SelectedImage = null;

                //--------удаление кода-----------------
                if (QRCod != null)
                {
                    try
                    {
                        System.IO.File.Delete(QRCod);
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show(ex.Message, "Ошибка при удалении qr кодa из Catalogue_QR");
                    }
                    finally
                    {
                        database.dataset1.QRs.Rows.Find(this.Project_Id).Delete();
                        database.SaveQRsFile();
                    }
                }
                QRCod = null;

                //----------удаление модели--------------
                if (model_path != null)
                {
                    database.Delete3DModel(model_path, this.Project_Id);
                }

                //-----------удаление видео
                if (video_path != null)
                {
                    try
                    {
                        System.IO.File.Delete(database.soursePath + database.CatalogueVideos + video_path);
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show(ex.Message, "Ошибка при удалении видео из Catalogue_Videos");
                    }
                    finally
                    {
                        database.dataset1.Videos.Rows.Find(this.Project_Id).Delete();
                        database.SaveVideosFile();
                    }
                }
                Video = null;

                //---------удаление контактов
               DataRow dr = database.dataset1.Contact.Rows.Find(this.Project_Id);
                if (dr != null)
                {
                    dr.Delete();
                    database.SaveContactsFile();
                }
                

                //---------удаление текста
                if (Document != null)
                {
                    try
                    {
                        System.IO.File.Delete(descText.TextPath);
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show(ex.Message, "Ошибка при удалении видео из Catalogue_Texts");
                    }
                    finally
                    {
                        database.dataset1.Texts.Rows.Find(this.Project_Id).Delete();
                        database.SaveTextsFile();
                    }
                }
                Document = null;

                //----------------------------
                this.database.dataset1.Tree.Rows.Find(Convert.ToInt32(this.Project_Id))[3] = 0;
                database.SaveTreeFile();
            
        }


        //private RelayCommand delProject;
        //public RelayCommand DeleteProject
        //{
        //    get
        //    {
        //        return delProject ??
        //            (delProject = new RelayCommand(obj =>
        //            {
                       
                            
        //                }
        //            }));
        //    }
        //}

        //---------------------------------------------------------------------------------------------

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
