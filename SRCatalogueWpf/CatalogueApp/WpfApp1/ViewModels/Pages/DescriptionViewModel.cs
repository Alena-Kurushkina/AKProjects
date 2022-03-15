using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Models;
using System.Data;
using WpfApp1.ViewModel;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Xps.Packaging;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;

namespace WpfApp1.ViewModels.Pages
{
    public class DescriptionViewModel: IPageViewModel, INotifyPropertyChanged
    {
        //private string _id; 
        public ObservableCollection<Image> Images { get; set; }
        private DataRow[] ImagesDataRows;        
        private string ProjectID;
        private Database database;
        public DescriptionViewModel(DataRow[] imdr, DescriptionText desctext, DataRow contact, string id, Database db)
        {
            ImagesDataRows = imdr;
            
            ProjectID = id;

            database = db;

            if (contact != null)  {
                _contOrg = contact[1].ToString();
                _contFio = contact[2].ToString();
                _contTel = contact[3].ToString();
                _contEmail = contact[4].ToString();
            }           

            Images = new ObservableCollection<Image>();          
                      
            foreach(DataRow dr in ImagesDataRows)
            {
                    if (File.Exists(database.soursePath+database.CataloguePictures + dr[1].ToString())) { 
                        Image im = new Image(ProjectID, database) { id = dr[0].ToString(), Title = dr[2].ToString(), Path = database.soursePath + database.CataloguePictures + dr[1].ToString()};
                        Images.Add(im);
                    }else
                    {
                        MessageBox.Show("Изображение " + dr[1].ToString() + " не найдено в папке Catalogue_Pictures. Возможно, во время удаления этого изображения произошла ошибка перезаписи файла Pictures.txt");
                    }
            };
            if (Images.Count != 0)
            {
               Images = new ObservableCollection<Image>(Images.OrderBy(u => u.id));
                _selectedImage = Images[0];
                
                
            }

            //if (TextDataRow!=null)
            //{
            //    docPath = database.soursePath+database.CatalogueTexts + TextDataRow[1].ToString();
            //    try { 
            //        var doc = new FlowDocument();
            //        var range = new TextRange(doc.ContentStart, doc.ContentEnd);
            //        FileStream fstr = new FileStream(docPath, System.IO.FileMode.Open);
            //        range.Load(fstr, DataFormats.Rtf);
            //        fstr.Close();
            //        _doc = doc;                    
            //        }
            //    catch(Exception e) { MessageBox.Show(e.Message); }
            //}
            //else _doc = null;
                       
            _doc = desctext.Doc;
                    
        }

        
        private FlowDocument _doc;
        public string docPath;

        private Image _selectedImage;

        private string _contOrg;
        private string _contFio;
        private string _contTel;
        private string _contEmail;
        
       
        public string IdentificationName
        {
            get { return "Description"; }
        }
        
        public PackIconKind Icon
        {
            get { return MaterialDesignThemes.Wpf.PackIconKind.TextSubject; }
        }

        public void Dispose() { }

        public void Load() { }
        public FlowDocument Document
        {
            get { return _doc; }
            set
            {
                _doc = value;
                OnPropertyChanged("Document");
            }
        }

        public string ContOrg
        {
            get { return _contOrg; }
        }
        public string ContFio
        {
            get { return _contFio; }
        }
        public string ContTel
        {
            get { return _contTel; }
        }
        public string ContEmail
        {
            get { return _contEmail; }
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

        

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
