using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WpfApp1.Models;
using WpfApp1.Models.TreeStruct;
using WpfApp1.ViewModels.TreeStructPages;
using Word = Microsoft.Office.Interop.Word;

namespace WpfApp1.ViewModels
{
    class TreeStructPageViewModel : INotifyPropertyChanged
    {
        private TreeStructPages.IPageViewModel _currentPageViewModel;
        private List<TreeStructPages.IPageViewModel> _pageViewModels;

        private RelayCommand _changePageCommand;
        private RelayCommand _loadProjects;

        private Database database;
        public List<TreeStructPages.IPageViewModel> PageViewModels
        {
            get
            {
                if (_pageViewModels == null)
                    _pageViewModels = new List<TreeStructPages.IPageViewModel>();

                return _pageViewModels;
            }
        }

        public TreeStructPageViewModel(Database db)
        {
            database = db;
           mistakesFilePath = database.soursePath + @"\PrLoadingMistakes.txt";

            if (PageViewModels.Count != 0)
            {
                PageViewModels.Clear();
            }

            ObservableCollection<TreeStruct> _treeFileItems = new ObservableCollection<TreeStruct>();
            foreach (DataRow dr in db.dataset1.Tree)
            {
                _treeFileItems.Add(new TreeStruct(dr));
            }
            ProjectsFileViewModel pfvm = new ProjectsFileViewModel(db, _treeFileItems);
            PageViewModels.Add(pfvm);
            
            PicturesFileViewModel picfvm = new PicturesFileViewModel(db);
            PageViewModels.Add(picfvm);
            pfvm.childrenVM.Add(picfvm);

            ModelsFileViewModel mfvm = new ModelsFileViewModel(db);
            PageViewModels.Add(mfvm);
            pfvm.childrenVM.Add(mfvm);

            QRsFileViewModel qfvm = new QRsFileViewModel(db);
            PageViewModels.Add(qfvm);
            pfvm.childrenVM.Add(qfvm);

            VideosFileViewModel vfvm = new VideosFileViewModel(db);
            PageViewModels.Add(vfvm);
            pfvm.childrenVM.Add(vfvm);

            ContactsFileViewModel cfvm = new ContactsFileViewModel(db);
            PageViewModels.Add(cfvm);
            pfvm.childrenVM.Add(cfvm);

            TextsFileViewModel tfvm = new TextsFileViewModel(db);
            PageViewModels.Add(tfvm);
            pfvm.childrenVM.Add(tfvm);

            
            CurrentPageViewModel = PageViewModels[0];
        }

        public TreeStructPages.IPageViewModel CurrentPageViewModel
        {
            get
            {
                return _currentPageViewModel;
            }
            set
            {
                if (_currentPageViewModel != value)
                {
                    _currentPageViewModel = value;
                    OnPropertyChanged("CurrentPageViewModel");
                }
            }
        }

        private System.Windows.Visibility _loadingVisibility = System.Windows.Visibility.Hidden;
        public System.Windows.Visibility LoadingVisibility
        {
            get { return _loadingVisibility; }
            set { _loadingVisibility = value;
                OnPropertyChanged("LoadingVisibility");
            }
        }

        public RelayCommand ChangePageCommand
        {
            get
            {
                if (_changePageCommand == null)
                {

                    _changePageCommand = new RelayCommand(
                        p => ChangeViewModel((TreeStructPages.IPageViewModel)p),
                        p => p is TreeStructPages.IPageViewModel);
                }

                return _changePageCommand;
            }
        }

        int mistakes = 0;
        string mistakesFilePath;

        public RelayCommand LoadProjects
        {
            get
            {
                return _loadProjects??
                  (_loadProjects = new RelayCommand(obj =>
                  {
                      string Dest_str;
                      FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog()
                      { Description = "Выберите папку с проектами",
                        ShowNewFolderButton = false,
                      };                      
                      DialogResult result = folderBrowserDialog1.ShowDialog();
                      if (result == DialogResult.OK)
                      {
                          string source = folderBrowserDialog1.SelectedPath;

                          DialogResult res = MessageBox.Show("Загрузить проекты из папки " + source + " ?", "Внимание", MessageBoxButtons.YesNo);
                          if (res == DialogResult.Yes) {

                              LoadingVisibility = System.Windows.Visibility.Visible;

                              using (StreamWriter sw = new StreamWriter(mistakesFilePath, false))
                              {
                                  sw.WriteLine(" ");
                              }

                              string[] pathes = Directory.GetFiles(source, "*", SearchOption.AllDirectories);
                              
                              Word.Application apl = new Word.Application();

                              foreach (string path in pathes)
                              {
                                  string[] parts = path.Split('\\');
                                  string ProjectId = parts[parts.Length - 2];
                                  string file = parts.Last();
                                  string exten = file.Split('.').Last();//расширение файла
                                  string name = file.Substring(0, file.Length - exten.Length - 1);//название без расширения 

                                  Models.DataSet1.TreeRow tr = database.dataset1.Tree.FindByid(Convert.ToInt32(ProjectId));
                                  if (tr == null)
                                  {
                                      mistakes++;
                                      using (StreamWriter sw = new StreamWriter(mistakesFilePath, true))
                                      {
                                          MessageBox.Show("Проекта №" + ProjectId + " нет в списке. Невозможно загрузить контент.");
                                          sw.WriteLine("Проект: " + ProjectId + " Ошибка: Такого проекта нет в списке.");
                                      }
                                  }
                                  else
                                  {
                                      if (tr.content == 0)
                                      {
                                          try
                                          {
                                              DescriptionText descText = new DescriptionText(database, ProjectId);
                                              tr.content = 1;
                                          }
                                          catch (Exception e)
                                          {
                                              MessageBox.Show("Не удалось добавить файл описание к проекту " + ProjectId);
                                              mistakes++;
                                              using (StreamWriter sw = new StreamWriter(mistakesFilePath, true))
                                              {
                                                  sw.WriteLine("Проект: " + ProjectId +" Не удалось добавить файл описание к проекту. Контент не был добавлен. Ошибка: " + e.Message);
                                              }
                                          }
                                          if (!database.SaveTreeFile())
                                          {
                                              mistakes++;
                                          }

                                      }
                                      if (tr.content == 1)
                                      {
                                          switch (exten)
                                          {
                                              case "txt":
                                                  List<string> contacts = new List<string>();
                                                  try
                                                  {
                                                      using (StreamReader sr = new StreamReader(path))
                                                      {
                                                          while (sr.Peek() >= 0)
                                                          {
                                                              contacts.Add(sr.ReadLine());
                                                          }
                                                      }
                                                  }
                                                  catch (Exception e)
                                                  {
                                                      mistakes++;
                                                      using (StreamWriter sw = new StreamWriter(mistakesFilePath, true))
                                                      {
                                                          sw.WriteLine("Проект: " + ProjectId + " Файл: " + file + " Ошибка: " + e.Message);
                                                      }
                                                      goto ifmist;
                                                  }

                                                  Models.DataSet1.ContactRow cr = database.dataset1.Contact.FindByid(Convert.ToInt32(ProjectId));
                                                  if (cr == null)
                                                  {
                                                      DataRow dr = database.dataset1.Contact.NewRow();
                                                      dr[0] = ProjectId;
                                                      dr[1] = contacts[0]; //организация
                                                      dr[2] = contacts[1];//фио
                                                      dr[3] = contacts[2];//телефон
                                                      dr[4] = contacts[3];//email
                                                      database.dataset1.Contact.Rows.Add(dr);
                                                  }
                                                  else
                                                  {
                                                      cr[1] = contacts[0]; //организация
                                                      cr[2] = contacts[1];//фио
                                                      cr[3] = contacts[2];//телефон
                                                      cr[4] = contacts[3];//email
                                                  }

                                                  ifmist:
                                                  break;


                                              case "docx":
                                                  Models.DataSet1.TextsRow textr = database.dataset1.Texts.FindByid(Convert.ToInt32(ProjectId));
                                                  string dest = database.soursePath + database.CatalogueTexts + textr.path;

                                                  Word.Document sdoc = null;
                                                  try
                                                  {
                                                      sdoc = apl.Documents.Open(path);
                                                      sdoc.SaveAs(Application.StartupPath + "\\" + dest, Word.WdSaveFormat.wdFormatRTF);
                                                  }
                                                  catch (Exception e)
                                                  {
                                                      mistakes++;
                                                      using (StreamWriter sw = new StreamWriter(mistakesFilePath, true))
                                                      {
                                                          sw.WriteLine("Проект: " + ProjectId + " Файл: " + file + " Ошибка: " + e.Message);
                                                      }
                                                  }
                                                  finally
                                                  {
                                                      if (sdoc != null) sdoc.Close();
                                                      // apl.Quit();
                                                  }
                                                  break;


                                              case "rtf":
                                                  try
                                                  {
                                                      Models.DataSet1.TextsRow textr1 = database.dataset1.Texts.FindByid(Convert.ToInt32(ProjectId));
                                                      string dest1 = database.soursePath + database.CatalogueTexts + textr1.path;
                                                      File.Copy(path, Application.StartupPath + "\\" + dest1, true);
                                                  }
                                                  catch (Exception e)
                                                  {
                                                      mistakes++;
                                                      using (StreamWriter sw = new StreamWriter(mistakesFilePath, true))
                                                      {
                                                          sw.WriteLine("Проект: " + ProjectId + " Файл: " + file + " Ошибка: " + e.Message);
                                                      }
                                                  }
                                                  break;


                                              case "jpg":
                                              case "png":
                                              case "bmp":

                                                  string Sourse_str = path;
                                                  Dest_str = database.soursePath + database.CataloguePictures;
                                                  string old_file_name = file;

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
                                                          else goto norename;
                                                      }
                                                  }
                                                  catch (Exception ex)
                                                  {
                                                      mistakes++;
                                                      using (StreamWriter sw = new StreamWriter(mistakesFilePath, true))
                                                      {
                                                          sw.WriteLine("Проект: " + ProjectId + " Файл: " + file + " Ошибка: " + ex.Message);
                                                      }
                                                      goto norename;
                                                  }
                                                  try
                                                  {
                                                      File.Copy(Sourse_str, System.IO.Path.Combine(Dest_str, new_file_name));
                                                  }
                                                  catch (Exception ex)
                                                  {
                                                      mistakes++;
                                                      using (StreamWriter sw = new StreamWriter(mistakesFilePath, true))
                                                      {
                                                          sw.WriteLine("Проект: " + ProjectId + " Файл: " + file + " Ошибка: " + ex.Message);
                                                      }
                                                      goto norename;
                                                  }

                                                  DataRow mdr = database.dataset1.Pictures.NewRow();
                                                  mdr[1] = new_file_name;
                                                  mdr[2] = name;
                                                  mdr[3] = ProjectId;
                                                  database.dataset1.Pictures.Rows.Add(mdr);

                                                  norename:
                                                  break;
                                          }
                                      }
                                  }
                              }
                              apl.Quit();

                              if (!database.SaveContactsFile()) mistakes++;
                              if (!database.SavePictureFile()) mistakes++;

                              LoadingVisibility = System.Windows.Visibility.Hidden;

                              MessageBox.Show("Загрузка проектов завершена." +
                                  " Ошибок: " + mistakes.ToString() + " Содержание ошибок можно посмотреть в папке проекта в файле PrLoadingMistakes.txt");

                          }
                      }
                  }
              ));
            }
        }
        private void ChangeViewModel(TreeStructPages.IPageViewModel viewModel)
        {

            if (!PageViewModels.Contains(viewModel))
                PageViewModels.Add(viewModel);

            CurrentPageViewModel = PageViewModels.FirstOrDefault(vm => vm == viewModel);
        }



        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

        }
    }
}
