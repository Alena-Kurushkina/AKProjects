using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

using MessageBox = System.Windows.MessageBox;

namespace WpfApp1.Models
{
    public class Image: INotifyPropertyChanged
    {
        public Image(string tr_id, Database db)
        {
            tree_id = tr_id;
            database = db;
        }
        private string _path;
        private string _title;
        private Database database;
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged("Title");
            }
        }
        public string Path
        {
            get { return _path; }
            set { _path = value; OnPropertyChanged("Path"); }
        }

        public string id;
        public string tree_id;

        public bool Delete()
        {
            try
            {   
                System.IO.File.Delete(this.Path);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка при удалении изображения из папки");
                return false;
            } 
        }

        public void ChangeOrder(string id1, string id2)
        {
            DataRow datarow1 = database.dataset1.Pictures.FindByid(Convert.ToInt32(id1));
            DataRow datarow2 = database.dataset1.Pictures.FindByid(Convert.ToInt32(id2));
            string vr = (Convert.ToInt32(database.dataset1.Pictures.Select("id=MAX(id)")[0]["id"]) + 1).ToString();

            datarow1["id"] = vr;
            datarow2["id"] = id1;
            datarow1["id"] = id2;

            
                    
            database.SavePictureFile();
            
        }

        private RelayCommand changeImName;
        public RelayCommand ChangeImName
        {
            get
            {
                return changeImName ??
                  (changeImName = new RelayCommand(obj =>
                  {
                      MaterialDesignThemes.Wpf.Flipper flipper = (MaterialDesignThemes.Wpf.Flipper)obj;

                      DataRow datarow = database.dataset1.Pictures.FindByid(Convert.ToInt32(this.id));
                      datarow["description"] = this.Title;
                      database.SavePictureFile();

                      flipper.IsFlipped = false;
                      
                  }
                 ));
            }
        }


        public string Add()
        {
            System.Windows.Forms.OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files(*.BMP;*.JPG;*.PNG)|*.BMP;*.JPG;*.PNG";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
               
                    string Sourse_str = ofd.FileName.Substring(0, ofd.FileName.Length - ofd.SafeFileName.Length);
                    string Dest_str = database.soursePath+database.CataloguePictures;
                    string old_file_name = ofd.SafeFileName;

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
                        else return null;
                    } 
                }
                catch (IOException ex)
                {
                    System.Windows.MessageBox.Show(ex.Message + " Переименуйте файл.", "Ошибка при добавлении изображения");
                    return null;
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message, "Ошибка при переименовании файла");
                    return null;
                }

                    string im_desc = "";
                try { 
                    ImDescription_dialog descd = new ImDescription_dialog("Введите описание изображения", false, ofd.SafeFileName);
                    if (descd.ShowDialog() == true)
                    {
                        im_desc = descd.Im_Description;
                        this.Title = im_desc;
                    }
                    else return null;
                }
                catch(Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message, "Ошибка при добавлении описания");
                    return null;
                }

                try {
                    File.Copy(System.IO.Path.Combine(Sourse_str, old_file_name), System.IO.Path.Combine(Dest_str, new_file_name));
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message, "Ошибка при добавлении изображения в Catalogue_Pictures");
                    return null;
                }

                if (File.Exists(System.IO.Path.Combine(Dest_str, new_file_name)))
                {
                    this.Path = System.IO.Path.Combine(Dest_str, new_file_name);
                    return new_file_name; 
                }
                else return null;

            }
            else return null;
        }

        public string Add(string desc)
        {
            System.Windows.Forms.OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files(*.BMP;*.JPG;*.PNG)|*.BMP;*.JPG;*.PNG";
            if (ofd.ShowDialog() == DialogResult.OK)
            {

                string Sourse_str = ofd.FileName.Substring(0, ofd.FileName.Length - ofd.SafeFileName.Length);
                string Dest_str = database.soursePath + database.CataloguePictures;
                string old_file_name = ofd.SafeFileName;

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
                        else return null;
                    }
                }
                catch (IOException ex)
                {
                    System.Windows.MessageBox.Show(ex.Message + " Переименуйте файл.", "Ошибка при добавлении изображения");
                    return null;
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message, "Ошибка при переименовании файла");
                    return null;
                }

               
                this.Title = desc;
                    

                try
                {
                    File.Copy(System.IO.Path.Combine(Sourse_str, old_file_name), System.IO.Path.Combine(Dest_str, new_file_name));
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message, "Ошибка при добавлении изображения в Catalogue_Pictures");
                    return null;
                }

                if (File.Exists(System.IO.Path.Combine(Dest_str, new_file_name)))
                {
                    this.Path = System.IO.Path.Combine(Dest_str, new_file_name);
                    return new_file_name;
                }
                else return null;

            }
            else return null;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

        }
    }
}
