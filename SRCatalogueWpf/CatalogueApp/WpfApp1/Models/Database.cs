using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using WpfApp1.Models;
using WpfApp1.Models.TreeStruct;
using WpfApp1.ViewModel;
using WpfApp1.ViewModels.TreeStructPages;

namespace WpfApp1
{
    public class Database
    {
        public DataSet1 dataset1;

        public string initFileCataloque = "data";
        public string soursePath;//data\TreeEnergy\
        public string CatalogueTxtFile; //TreeEnergy.txt

        public string ModelsTxtFile = "exeModels.txt"; 
        public string PicturesTxtfile = "Pictures.txt";
        public string QRsTxtfile = "QRs.txt";
        public string TextsTxtfile = "Texts.txt";
        public string VideosTxtfile = "Videos.txt";
        public string ContactsTxtfile = "Contacts.txt";

        public string CatalogueModels = @"Catalogue_Models\";
        public string CataloguePictures = @"Catalogue_Pictures\";
        public string CatalogueQR = @"Catalogue_QR\";
        public string CatalogueTexts = @"Catalogue_Texts\";
        public string CatalogueVideos = @"Catalogue_Videos\";

        private Visibility _isAdmin;
        private int _curProject=0;

       
        public Database(string SourseFile, Visibility admin)
        { 
            dataset1 = new DataSet1();
            soursePath = initFileCataloque+@"\" + SourseFile + @"\";
            CatalogueTxtFile = SourseFile + ".txt";

            _isAdmin = admin;

            #region Reader

            try
            {               
                StreamReader reader1 = new StreamReader(soursePath+CatalogueTxtFile);
                string str1;
                while ((str1 = reader1.ReadLine()) != null)
                {
                    if (str1 != "") { 
                        string[] items = str1.Split('\t');
                        DataRow row = dataset1.Tree.NewRow();
                        row["id"] = items[0];
                        row["Name"] = items[1];
                        row["Parent_id"] = items[2];
                        row["content"] = items[3];
                        dataset1.Tree.Rows.Add(row);
                    }
                }
                 reader1.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Некорректный файл Tree.txt");
            }

            try
            {
                StreamReader reader2 = new StreamReader(soursePath+ModelsTxtFile);
                string str2;
                while ((str2 = reader2.ReadLine()) != null)
                {
                    if (str2 != "") { 
                        string[] items = str2.Split('\t');
                        DataRow row = dataset1.Models.NewRow();
                        row["id"] = items[0];
                        row["exeName"] = items[1];
                        dataset1.Models.Rows.Add(row);
                    }
                }
                reader2.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Некорректный файл exeModels.txt");
            }

            try
            {
                StreamReader reader3 = new StreamReader(soursePath+PicturesTxtfile);
                string str3;
                while ((str3 = reader3.ReadLine()) != null)
                {
                    if (str3 != "") { 
                        string[] items = str3.Split('\t');
                        DataRow row = dataset1.Pictures.NewRow();
                        row["id"] = items[0];
                        row["path"] = items[1];
                        row["description"] = items[2];
                        row["tree_id"] = items[3];
                        dataset1.Pictures.Rows.Add(row);
                    }
                }
                reader3.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Некорректный файл Pictures.txt");
            }

            try
            {
                StreamReader reader4 = new StreamReader(soursePath+TextsTxtfile);
                string str4;
                while ((str4 = reader4.ReadLine()) != null)
                {
                    if (str4 != "") { 
                        string[] items = str4.Split('\t');
                        DataRow row = dataset1.Texts.NewRow();
                        row["id"] = items[0];
                        row["path"] = items[1];                    
                        dataset1.Texts.Rows.Add(row);
                    }
                }
                reader4.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Некорректный файл Texts.txt");
            }

            try
            {
                StreamReader reader5 = new StreamReader(soursePath+QRsTxtfile);
                string str5;
                while ((str5 = reader5.ReadLine()) != null)
                {
                    if (str5 != "") { 
                        string[] items = str5.Split('\t');
                        DataRow row = dataset1.QRs.NewRow();
                        row["id"] = items[0];
                        row["path"] = items[1];
                        dataset1.QRs.Rows.Add(row);
                    }
                }
                reader5.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Некорректный файл QRs.txt");
            }

            try
            {
                StreamReader reader6 = new StreamReader(soursePath + VideosTxtfile);
                string str6;
                while ((str6 = reader6.ReadLine()) != null)
                {
                    if (str6 != "")
                    {
                        string[] items = str6.Split('\t');
                        DataRow row = dataset1.Videos.NewRow();
                        row["id"] = items[0];
                        row["path"] = items[1];
                        dataset1.Videos.Rows.Add(row);
                    }
                }
                reader6.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Некорректный файл Videos.txt");
            }

            try
            {
                StreamReader reader7 = new StreamReader(soursePath + ContactsTxtfile);
                string str7;
                while ((str7 = reader7.ReadLine()) != null)
                {
                    if (str7 != "")
                    {
                        string[] items = str7.Split('\t');
                        DataRow row = dataset1.Contact.NewRow();
                        row["id"] = items[0];
                        row["organization"] = items[1];
                        row["fio"] = items[2];
                        row["telephone"] = items[3];
                        row["email"] = items[4];
                        dataset1.Contact.Rows.Add(row);
                    }
                }
                reader7.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Некорректный файл Contacts.txt");
            }

            #endregion

        }

        public int CurrentProject
        {
            get { return _curProject; }
            set { _curProject = value; }
        }

        public Visibility IsAdmin
        {
            get { return _isAdmin; }
            set { _isAdmin = value; }

        }


        public CNode GetCatalogueTree() 
        {

            return new CNode(dataset1.Tree.Rows[0], dataset1.Tree);

        }

        public bool SetContent(string id)
        {
            dataset1.Tree.Rows.Find(Convert.ToInt32(id))[3] = 1;
            return true;
        }

        public string GetProjectName(string id)
        {
            return dataset1.Tree.Rows.Find(Convert.ToInt32(id))[1].ToString();
        }

        public DataRow[] GetImages(string id)
        {
            return dataset1.Pictures.Select($"tree_id= '{id}'");
           
        }

        public DataRow GetText(string id)
        {
            return dataset1.Texts.Rows.Find(id);
        }

        public DataRow GetQRCod(string id)
        {
            return dataset1.QRs.Rows.Find(id);
        }

        public DataRow GetModel(string id)
        {
            return dataset1.Models.Rows.Find(id);
        }
        public DataRow GetVideo(string id)
        {
            return dataset1.Videos.Rows.Find(id);
        }
        public DataRow GetContact(string id)
        {
            return dataset1.Contact.Rows.Find(id);
        }

        #region SafeFile

        public bool SavePictureFile()
        {
            try { 
                using (StreamWriter writer = new StreamWriter(soursePath+"Tf.txt"))
                {
                    foreach(DataRow dr in dataset1.Pictures)
                    {                    
                        writer.WriteLine(dr["id"].ToString() + '\t' + dr["path"].ToString() + '\t' + dr["description"].ToString() + '\t' + dr["tree_id"].ToString());
                    }
                    
                }
            }
            catch(Exception ex)
            {
               
                
                if(File.Exists(soursePath + "Tf.txt"))
                {
                    File.Delete(soursePath + "Tf.txt");
                }
                
                MessageBox.Show(ex.Message, "Ошибка при записи файла Pictures.txt. Изменения не сохранены");

                return false;
            }

            if (File.Exists(soursePath + PicturesTxtfile))
            {
                try
                {
                    File.Delete(soursePath + PicturesTxtfile);
                }
                catch(Exception ex)
                {
                    
                    if (File.Exists(soursePath + "Tf.txt"))
                    {
                        File.Delete(soursePath + "Tf.txt");
                    }
                    
                    MessageBox.Show(ex.Message, "Ошибка при записи файла Pictures.txt. Изменения не сохранены");

                    return false;
                }
            }

            File.Move(soursePath + "Tf.txt", soursePath + PicturesTxtfile);
            return true;
        }

        public bool SaveTreeFile()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(soursePath + "Tf.txt"))
                {
                    foreach (DataRow dr in dataset1.Tree)
                    {
                        writer.WriteLine(dr["id"].ToString() + '\t' + dr["Name"].ToString() + '\t' + dr["Parent_id"].ToString() + '\t' + dr["content"].ToString());
                    }

                }
            }
            catch (Exception ex)
            {
               

                if (File.Exists(soursePath + "Tf.txt"))
                {
                    File.Delete(soursePath + "Tf.txt");
                }
                
                MessageBox.Show(ex.Message, "Ошибка при записи файла Tree.txt. Изменения не сохранены");

                return false;
            }

            if (File.Exists(soursePath + CatalogueTxtFile))
            {
                try
                {
                    File.Delete(soursePath + CatalogueTxtFile);
                }
                catch (Exception ex)
                {
                   
                    if (File.Exists(soursePath + "Tf.txt"))
                    {
                        File.Delete(soursePath + "Tf.txt");
                    }
                    
                    MessageBox.Show(ex.Message, "Ошибка при записи файла Tree.txt. Изменения не сохранены");

                    return false;
                }
            }

            File.Move(soursePath + "Tf.txt", soursePath + CatalogueTxtFile);
            return true;
        }

        public bool SaveQRsFile()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(soursePath + "Tf.txt"))
                {
                    foreach (DataRow dr in dataset1.QRs)
                    {
                        writer.WriteLine(dr["id"].ToString() + '\t' + dr["path"].ToString());
                    }

                }
            }
            catch (Exception ex)
            {
               

                if (File.Exists(soursePath + "Tf.txt"))
                {
                    File.Delete(soursePath + "Tf.txt");
                }
                
                MessageBox.Show(ex.Message, "Ошибка при записи файла QRs.txt. Изменения не сохранены");

                return false;
            }

            if (File.Exists(soursePath + QRsTxtfile))
            {
                try
                {
                    File.Delete(soursePath + QRsTxtfile);
                }
                catch (Exception ex)
                {
                    
                    if (File.Exists(soursePath + "Tf.txt"))
                    {
                        File.Delete(soursePath + "Tf.txt");
                    }
                    
                    MessageBox.Show(ex.Message, "Ошибка при записи файла Tree.txt. Изменения не сохранены");

                    return false;
                }
            }

            File.Move(soursePath + "Tf.txt", soursePath + QRsTxtfile);
            return true;
        }

        public bool SaveVideosFile()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(soursePath + "Tf.txt"))
                {
                    foreach (DataRow dr in dataset1.Videos)
                    {
                        writer.WriteLine(dr["id"].ToString() + '\t' + dr["path"].ToString());
                    }

                }
            }
            catch (Exception ex)
            {


                if (File.Exists(soursePath + "Tf.txt"))
                {
                    File.Delete(soursePath + "Tf.txt");
                }

                MessageBox.Show(ex.Message, "Ошибка при записи файла Videos.txt. Изменения не сохранены");

                return false;
            }

            if (File.Exists(soursePath + VideosTxtfile))
            {
                try
                {
                    File.Delete(soursePath + VideosTxtfile);
                }
                catch (Exception ex)
                {

                    if (File.Exists(soursePath + "Tf.txt"))
                    {
                        File.Delete(soursePath + "Tf.txt");
                    }

                    MessageBox.Show(ex.Message, "Ошибка при записи файла Videos.txt. Изменения не сохранены");

                    return false;
                }
            }

            File.Move(soursePath + "Tf.txt", soursePath + VideosTxtfile);
            return true;

        }

        public bool SaveTextsFile()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(soursePath + "Tf.txt"))
                {
                    foreach (DataRow dr in dataset1.Texts)
                    {
                        writer.WriteLine(dr["id"].ToString() + '\t' + dr["path"].ToString());
                    }

                }
            }
            catch (Exception ex)
            {


                if (File.Exists(soursePath + "Tf.txt"))
                {
                    File.Delete(soursePath + "Tf.txt");
                }

                MessageBox.Show(ex.Message, "Ошибка при записи файла Texts.txt. Изменения не сохранены");

                return false;
            }

            if (File.Exists(soursePath + TextsTxtfile))
            {
                try
                {
                    File.Delete(soursePath + TextsTxtfile);
                }
                catch (Exception ex)
                {

                    if (File.Exists(soursePath + "Tf.txt"))
                    {
                        File.Delete(soursePath + "Tf.txt");
                    }

                    MessageBox.Show(ex.Message, "Ошибка при записи файла Texts.txt. Изменения не сохранены");

                    return false;
                }
            }

            File.Move(soursePath + "Tf.txt", soursePath + TextsTxtfile);
            return true;
        }

        public bool SaveModelsFile()
        {
            
            try
            {
                using (StreamWriter writer = new StreamWriter(soursePath + "Tf.txt"))
                {
                    foreach (DataRow dr in dataset1.Models)
                    {
                        writer.WriteLine(dr["id"].ToString() + '\t' + dr["exeName"].ToString());
                    }

                }
            }
            catch (Exception ex)
            {


                if (File.Exists(soursePath + "Tf.txt"))
                {
                    File.Delete(soursePath + "Tf.txt");
                }

                MessageBox.Show(ex.Message, "Ошибка при записи файла Models.txt. Изменения не сохранены");

                return false;
            }

            if (File.Exists(soursePath + ModelsTxtFile))
            {
                try
                {
                    File.Delete(soursePath + ModelsTxtFile);
                }
                catch (Exception ex)
                {

                    if (File.Exists(soursePath + "Tf.txt"))
                    {
                        File.Delete(soursePath + "Tf.txt");
                    }

                    MessageBox.Show(ex.Message, "Ошибка при записи файла Models.txt. Изменения не сохранены");

                    return false;
                }
            }

            File.Move(soursePath + "Tf.txt", soursePath + ModelsTxtFile);
            return true;

        }

        public bool SaveContactsFile()
        {
            
            try
            {
                using (StreamWriter writer = new StreamWriter(soursePath + "Tf.txt"))
                {
                    foreach (DataRow dr in dataset1.Contact)
                    {
                        writer.WriteLine(dr["id"].ToString() + '\t' + dr["organization"].ToString() + '\t' + dr["fio"].ToString() + '\t' + dr["telephone"].ToString() + '\t' + dr["email"].ToString());
                    }

                }
            }
            catch (Exception ex)
            {


                if (File.Exists(soursePath + "Tf.txt"))
                {
                    File.Delete(soursePath + "Tf.txt");
                }

                MessageBox.Show(ex.Message, "Ошибка при записи файла Contacts.txt. Изменения не сохранены");

                return false;
            }

            if (File.Exists(soursePath + ContactsTxtfile))
            {
                try
                {
                    File.Delete(soursePath + ContactsTxtfile);
                }
                catch (Exception ex)
                {

                    if (File.Exists(soursePath + "Tf.txt"))
                    {
                        File.Delete(soursePath + "Tf.txt");
                    }

                    MessageBox.Show(ex.Message, "Ошибка при записи файла Contacts.txt. Изменения не сохранены");

                    return false;
                }
            }

            File.Move(soursePath + "Tf.txt", soursePath + ContactsTxtfile);
            return true;
        }

        #endregion

        public DataRow AddPazdel(string Description, string PID)
        {
            DataRow dr = dataset1.Tree.NewRow();
            dr[1] = Description.Trim();
            dr[2] = PID;
            dr[3] = 0;
            dataset1.Tree.Rows.Add(dr);

            if (SaveTreeFile()) return dr;
            else return null;
        }

        public DataRow AddProject(string Description, string PID)
        {
            DataRow dr = dataset1.Tree.NewRow();
            dr[1] = Description.Trim();
            dr[2] = PID;
            dr[3] = 1;
            dataset1.Tree.Rows.Add(dr);

            try
            {
                DescriptionText descText = new DescriptionText(this, dr[0].ToString());
            }
            catch
            {
                MessageBox.Show("Не удалось создать проект");
                return null;
            }
            if (SaveTreeFile()) return dr;
            else return null;

        }
        

        public Image AddPicture(string Project_Id)
        {        
            Image im = new Image(Project_Id, this);
            string file_name = im.Add();
            if (file_name != null)
            {
                try
                {
                    DataRow dr = dataset1.Pictures.NewRow();
                    dr["path"] = file_name;
                    dr["description"] = im.Title;
                    dr["tree_id"] = im.tree_id;
                    dataset1.Pictures.Rows.Add(dr);

                    SavePictureFile();
                    im.id = dr["id"].ToString();

                    return im;

                    //Images.Add(im);
                    // System.Windows.MessageBox.Show("Изображение успешно добавлено");
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message, "Ошибка при добавлении строки в Pictures.txt");
                    return null;
                }

            }
            else return null;
        }

        public bool DeleteImage(Image Image_toDelete)
        {
            MessageBoxResult result = System.Windows.MessageBox.Show("Вы действительно хотите удалить изображение '" + Image_toDelete.Title + "'?", "Подтверждение", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                if (Image_toDelete.Delete() == true)
                {
                    try
                    {                                                             
                        dataset1.Pictures.Rows.Find(Convert.ToInt32(Image_toDelete.id)).Delete();
                        SavePictureFile();
                        
                        return true;
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show(ex.Message, "Ошибка при удалении записи из Pictures.txt");
                        return false;
                    }
                }
                else return false;

            }
            else return false;
        }


        public string AddQR(string Project_Id)
        {
            MediaEdit me = new MediaEdit(this, Project_Id);
            if (me.GetQRFile())
            {
                if (me.AddQRCode() != null)
                {
                    return me.new_file_name;
                }
                else return null;
            }
            else return null;
            //System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            //ofd.Filter = "Image Files(*.BMP;*.JPG;*.PNG)|*.BMP;*.JPG;*.PNG";
            //if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //{
            //    string Sourse_str = ofd.FileName.Substring(0, ofd.FileName.Length - ofd.SafeFileName.Length);
            //    string Dest_str = soursePath + CatalogueQR;
            //    string old_file_name = ofd.SafeFileName;

            //    string new_file_name = old_file_name;
            //    try
            //    {
            //        while (File.Exists(System.IO.Path.Combine(Dest_str, new_file_name)))
            //        {
            //            Rename_dialog rd = new Rename_dialog(new_file_name);
            //            if (rd.ShowDialog() == true)
            //            {
            //                new_file_name = rd.Renamed;
            //            }
            //            else return null;
            //        }
            //    }
            //    catch (IOException ex)
            //    {
            //        System.Windows.MessageBox.Show(ex.Message + " Переименуйте файл.", "Ошибка при добавлении изображения в Catalogue_QR");
            //        return null;
            //    }
            //    catch (Exception ex)
            //    {
            //        System.Windows.MessageBox.Show(ex.Message, "Ошибка при добавлении изображения в Catalogue_QR");
            //        return null;
            //    }
            //    try
            //    {
            //        File.Copy(System.IO.Path.Combine(Sourse_str, old_file_name), System.IO.Path.Combine(Dest_str, new_file_name));

            //        if (File.Exists(System.IO.Path.Combine(Dest_str, new_file_name)))
            //        {
            //            try
            //            {

            //                DataRow dr = dataset1.QRs.NewRow();
            //                dr["id"] = Project_Id;
            //                dr["path"] = new_file_name;

            //                dataset1.QRs.Rows.Add(dr);
            //                SaveQRsFile();

            //            }
            //            catch (Exception ex)
            //            {
            //                System.Windows.MessageBox.Show(ex.Message, "Ошибка при добавлении строки в QRs.txt");
            //                return null;
            //            }

            //            return new_file_name;
            //        }
            //        else return null;
            //    }
            //    catch (Exception ex)
            //    {
            //        System.Windows.MessageBox.Show(ex.Message, "Ошибка при добавлении изображения в Catalogue_QR");
            //        return null;
            //    }

            //}
            //else return null;
        }

        public bool DeleteQR(string qr_path, string Project_Id)
        {
            MessageBoxResult result = System.Windows.MessageBox.Show("Вы действительно хотите удалить QR код '" + qr_path + "'?", "Подтверждение", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                try
                {
                    System.IO.File.Delete(qr_path);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message, "Ошибка при удалении qr кодa из Catalogue_QR");
                    return false;
                }

                if (!File.Exists(qr_path))
                {
                    try
                    {
                        dataset1.QRs.Rows.Find(Project_Id).Delete();
                        SaveQRsFile();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show(ex.Message, "Ошибка при удалении qr кодa из QRs.txt");
                        return false;
                    }
                }
                else return false;

            }else return false;
        }

        public bool Delete3DModel(string path, string Project_Id)
        {
            MessageBoxResult result = System.Windows.MessageBox.Show("Вы действительно хотите удалить 3D модель '" + path + "'?", "Подтверждение", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                               
                string exten = path.Split('.').Last();

                try
                {
                    if (exten == "exe")
                    {
                        string m_name = path.Substring(0, path.Length - exten.Length - 1) + "_Data";

                        System.IO.File.Delete(soursePath + CatalogueModels + path);
                        Directory.Delete(soursePath + CatalogueModels + m_name, true);
                    }
                    else
                    {
                        System.IO.File.Delete(soursePath + CatalogueModels + path);
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message, "Ошибка при удалении файла 3D модели");
                    return false;
                }
                finally
                {

                    dataset1.Models.Rows.Find(Project_Id).Delete();
                    SaveModelsFile();                    
                } 

            }return false;
        }

        public string Add3DModel(string Project_Id)
        {
            MediaEdit me = new MediaEdit(this, Project_Id);
            if (me.Get3DModelFile())
            {
                if (me.Add3DModelToProject()!=null)
                {
                    return me.new_file_name;
                }
                else return null;
            }
            else return null;
            //System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            //ofd.Filter = "All Supported |*.exe;*.3d;*.3ds;*.3mf;*.ac;*.ac3d;*.acc;*.amf;*.ase;" +
            //    "*.ask;*.assbin;*.b3d;*.blend;*.bvh;*.cob;*.csm;*.dae;*.dxf;*.enff;*.fbx;*.glb;*.gltf;*.hmp;*.ifc;" +
            //    "*.ifczip;*.irr;*.irrmesh;*.lwo;*.lws;*.lxo;*.md2;*.md3;*.md5anim;*.md5camera;*.md5mesh;*.mdc;*.mdl;" +
            //    "*.mesh;*.mesh.xml;*.mot;*.ms3d;*.ndo;*.nff;*.obj;*.off;*.ogex;*.pk3;*.ply;*.pmx;*.prj;*.q3o;*.q3s;" +
            //    "*.raw;*.scn;*.sib;*.smd;*.stl;*.stp;*.ter;*.uc;*.vta;*.x;*.x3d;*.x3db;*.xgl;*.xml;*.zgl;|Exe Files(*.exe)|*.exe|(*.3d)|*.3d|" +
            //    "(*.3ds)|*.3ds|(*.3mf)|*.3mf|(*.ac)|*.ac|(*.ac3d)|*.ac3d|(*.acc)|*.acc|(*.amf)|*.amf|(*.ase)|*.ase|" +
            //    "(*.ask)|*.ask|(*.assbin)|*.assbin|(*.b3d)|*.b3d|(*.blend)|*.blend|(*.bvh)|*.bvh|(*.cob)|*.cob|(*.csm)|" +
            //    "*.csm|(*.dae)|*.dae|(*.dxf)|*.dxf|(*.enff)|*.enff|(*.fbx)|*.fbx|(*.glb)|*.glb|(*.gltf)|*.gltf|(*.hmp)|" +
            //    "*.hmp|(*.ifc)|*.ifc|(*.ifczip)|*.ifczip|(*.irr)|*.irr|(*.irrmesh)|*.irrmesh|(*.lwo)|*.lwo|(*.lws)|*.lws|" +
            //    "(*.lxo)|*.lxo|(*.md2)|*.md2|(*.md3)|*.md3|(*.md5anim)|*.md5anim|(*.md5camera)|*.md5camera|(*.md5mesh)|" +
            //    "*.md5mesh|(*.mdc)|*.mdc|(*.mdl)|*.mdl|(*.mesh)|*.mesh|(*.mesh.xml)|*.mesh.xml|(*.mot)|*.mot|(*.ms3d)|" +
            //    "*.ms3d|(*.ndo)|*.ndo|(*.nff)|*.nff|(*.obj)|*.obj|(*.off)|*.off|(*.ogex)|*.ogex|(*.pk3)|*.pk3|(*.ply)|" +
            //    "*.ply|(*.pmx)|*.pmx|(*.prj)|*.prj|(*.q3o)|*.q3o|(*.q3s)|*.q3s|(*.raw)|*.raw|(*.scn)|*.scn|(*.sib)|*.sib|" +
            //    "(*.smd)|*.smd|(*.stl)|*.stl|(*.stp)|*.stp|(*.ter)|*.ter|(*.uc)|*.uc|(*.vta)|*.vta|(*.x)|*.x|(*.x3d)|" +
            //    "*.x3d|(*.x3db)|*.x3db|(*.xgl)|*.xgl|(*.xml)|*.xml|(*.zgl)|*.zgl";
            //// ofd.Filter = "Exe Files(*.exe)|*.exe|"+ OpenFileFilter;

            //if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //{

            //    string Sourse_str = ofd.FileName.Substring(0, ofd.FileName.Length - ofd.SafeFileName.Length);
            //    string Dest_str = soursePath + CatalogueModels;
            //    string old_file_name = ofd.SafeFileName;

            //    string exten = old_file_name.Split('.').Last();//расширение файла
            //    string exe_name = old_file_name.Substring(0, old_file_name.Length - exten.Length - 1);//название без расширения  

            //    string new_file_name;

            //    if (exten == "exe")
            //    {
            //        string exe_old_data = Sourse_str + exe_name + "_Data"; //путь к каталогу _Data 

            //        if (Directory.Exists(exe_old_data))
            //        {
            //            new_file_name = old_file_name;
            //            while (File.Exists(System.IO.Path.Combine(Dest_str, new_file_name)))
            //            {
            //                Rename_dialog rd = new Rename_dialog(new_file_name);
            //                if (rd.ShowDialog() == true)
            //                {
            //                    new_file_name = rd.Renamed;
            //                }
            //                else return null;
            //            }
            //            string exe_new_name = new_file_name.Substring(0, new_file_name.Length - exten.Length - 1);//новое название без расширения
            //            string exe_new_data = Dest_str + exe_new_name + "_Data";

            //            try
            //            {
            //                //копирование файла exe
            //                File.Copy(System.IO.Path.Combine(Sourse_str, old_file_name), System.IO.Path.Combine(Dest_str, new_file_name));
            //            }
            //            catch (Exception ex)
            //            {
            //                System.Windows.MessageBox.Show(ex.Message);
            //                return null;
            //            }

            //            //копирование папки _Data
            //            try
            //            {
            //                Directory.CreateDirectory(exe_new_data);
            //                foreach (string dirPath in Directory.GetDirectories(exe_old_data, "*", SearchOption.AllDirectories))
            //                {
            //                    Directory.CreateDirectory(dirPath.Replace(exe_old_data, exe_new_data));
            //                }

            //            }
            //            catch (Exception e)
            //            {
            //                System.Windows.MessageBox.Show(e.Message);
            //                File.Delete(System.IO.Path.Combine(Dest_str, new_file_name));
            //                Directory.Delete(exe_new_data, true);
            //                return null;
            //            }

            //            try
            //            {

            //                string[] files = Directory.GetFiles(exe_old_data, "*.*", SearchOption.AllDirectories);
            //                foreach (string file in files)
            //                {
            //                    File.Copy(file, file.Replace(exe_old_data, exe_new_data), true);
            //                }
            //                // return new_file_name;
            //            }
            //            catch (Exception ex)
            //            {
            //                System.Windows.MessageBox.Show(ex.Message, "Ошибка при добавлении 3D модели в Catalogue_Models");
            //                Directory.Delete(exe_new_data, true);
            //                File.Delete(System.IO.Path.Combine(Dest_str, new_file_name));
            //                return null;
            //            }
            //        }
            //        else
            //        {
            //            System.Windows.MessageBox.Show("В указанной директории не был найден каталог " + exe_name + "_Data");
            //            return null;
            //        }
            //    }
            //    else
            //    {
            //        new_file_name = old_file_name;
            //        while (File.Exists(System.IO.Path.Combine(Dest_str, new_file_name)))
            //        {
            //            Rename_dialog rd = new Rename_dialog(new_file_name);
            //            if (rd.ShowDialog() == true)
            //            {
            //                new_file_name = rd.Renamed;
            //            }
            //            else return null;
            //        }

            //        try
            //        {
            //            //копирование файла
            //            File.Copy(System.IO.Path.Combine(Sourse_str, old_file_name), System.IO.Path.Combine(Dest_str, new_file_name));
            //        }
            //        catch (Exception ex)
            //        {
            //            System.Windows.MessageBox.Show(ex.Message);
            //            return null;
            //        }

            //       // return new_file_name;
            //    }
                
            //    if (new_file_name != null)
            //    {
            //        try
            //        {
            //            DataRow dr = dataset1.Models.NewRow();
            //            dr["id"] = Project_Id;
            //            dr["exeName"] = new_file_name;
            //            dataset1.Models.Rows.Add(dr);
            //            SaveModelsFile();
            //            return new_file_name;
            //        }
            //        catch (Exception ex)
            //        {
            //            System.Windows.MessageBox.Show(ex.Message, "Ошибка при добавлении строки в Models.txt");
            //            return null;
            //        }
            //    }
            //    else
            //    {
            //        System.Windows.MessageBox.Show("Не удалось добавить 3D модель.");
            //        return null;
            //    }
            //}
            //else return null;            
        }

       

        public bool DeleteVideo(string video_path, string Project_Id)
        {
            MessageBoxResult result = System.Windows.MessageBox.Show("Вы действительно хотите удалить видео '" + video_path + "'?", "Подтверждение", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                try
                {
                    System.IO.File.Delete(soursePath + CatalogueVideos + video_path);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message, "Ошибка при удалении видео из Catalogue_Videos");
                    return false;
                }

                if (!File.Exists(video_path))
                {
                    dataset1.Videos.Rows.Find(Project_Id).Delete();
                    SaveVideosFile();
                    return true;
                }
                else return false;
            }
            else return false;
        }

        public string AddVideo(string Project_Id)
        {
            MediaEdit me = new MediaEdit(this, Project_Id);
            if (me.GetVideoFile())
            {
                if (me.AddVideoToProject() != null)
                {
                    return me.video_path;
                }
                else return null;
            }
            else return null;
            //System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            //ofd.Filter = "Video Files (*.avi, *.mp4)|*.avi;*.mp4|Video Files (*.wmv)|*.wmv";
            //if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //{
            //    string Sourse_str = ofd.FileName.Substring(0, ofd.FileName.Length - ofd.SafeFileName.Length);
            //    string Dest_str = soursePath + CatalogueVideos;
            //    string old_file_name = ofd.SafeFileName;

            //    string new_file_name = old_file_name;
            //    try
            //    {
            //        while (File.Exists(System.IO.Path.Combine(Dest_str, new_file_name)))
            //        {
            //            Rename_dialog rd = new Rename_dialog(new_file_name);
            //            if (rd.ShowDialog() == true)
            //            {
            //                new_file_name = rd.Renamed;
            //            }
            //            else return null;
            //        }
            //    }
            //    catch (IOException ex)
            //    {
            //        System.Windows.MessageBox.Show(ex.Message + " Переименуйте файл.", "Ошибка при добавлении изображения в Catalogue_Videos");
            //        return null;
            //    }
            //    catch (Exception ex)
            //    {
            //        System.Windows.MessageBox.Show(ex.Message, "Ошибка при добавлении изображения в Catalogue_Videos");
            //        return null;
            //    }
            //    try
            //    {
            //        File.Copy(System.IO.Path.Combine(Sourse_str, old_file_name), System.IO.Path.Combine(Dest_str, new_file_name));
            //    }
            //    catch (Exception ex)
            //    {
            //        System.Windows.MessageBox.Show(ex.Message, "Ошибка при добавлении изображения в Catalogue_Videos");
            //        return null;
            //    }
            //    try
            //    {
            //       DataRow dr = dataset1.Videos.NewRow();
            //       dr["id"] = Project_Id;
            //       dr["path"] = new_file_name;

            //       dataset1.Videos.Rows.Add(dr);
            //       SaveVideosFile();
            //    }
            //    catch (Exception ex)
            //    {
            //         System.Windows.MessageBox.Show(ex.Message, "Ошибка при добавлении строки в Videos.txt");
            //         return null;
            //    }

            //    return new_file_name;

            //}
            //else return null;
           
        }

        public void DeleteDescription(string ID)
        {            

            //-----------------удаление изображений----------------------

            DataRow[] datarow = dataset1.Pictures.Select("tree_id=" + ID);
            try
            {
                foreach (DataRow dr in datarow)
                {

                    System.IO.File.Delete(soursePath + CataloguePictures + dr["path"].ToString());
                    dr.Delete();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка при удалении изображения из папки");
            }
            finally
            {
                SavePictureFile();
            }

            //--------удаление кода-----------------

            DataRow drow = dataset1.QRs.Rows.Find(ID);
            if (drow != null)
            {
                try
                {
                    System.IO.File.Delete(soursePath + CatalogueQR + drow["path"].ToString());
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message, "Ошибка при удалении qr кодa из Catalogue_QR");
                }
                finally
                {
                    drow.Delete();
                    SaveQRsFile();
                }
            }

            //----------удаление модели--------------

            drow = dataset1.Models.Rows.Find(ID);
            if (drow != null)
            {
                string path = drow["exeName"].ToString();
                string exten = path.Split('.').Last();

                try
                {
                    if (exten == "exe")
                    {
                        string m_name = path.Substring(0, path.Length - exten.Length - 1) + "_Data";

                        System.IO.File.Delete(soursePath + CatalogueModels + path);
                        Directory.Delete(soursePath + CatalogueModels + m_name, true);
                    }
                    else
                    {
                        System.IO.File.Delete(soursePath + CatalogueModels + path);
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message, "Ошибка при удалении файла 3D модели");
                }
                finally
                {
                    drow.Delete();
                    SaveModelsFile();
                }
            }

            //-----------удаление видео
            drow = dataset1.Videos.Rows.Find(ID);
            if (drow != null)
            {
                try
                {
                    System.IO.File.Delete(soursePath + CatalogueVideos + drow["path"].ToString());
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message, "Ошибка при удалении qr кодa из Catalogue_QR");
                }
                finally
                {
                    drow.Delete();
                    SaveVideosFile();
                }
            }

            //---------удаление контактов

            drow = dataset1.Contact.Rows.Find(ID);
            if (drow != null)
            {
                drow.Delete();
                SaveContactsFile();
            }

            //---------удаление текста

            drow = dataset1.Texts.Rows.Find(ID);
            if (drow != null)
            {
                try
                {
                    System.IO.File.Delete(soursePath + CatalogueTexts + drow["path"].ToString());
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message, "Ошибка при удалении видео из Catalogue_Texts");
                }
                finally
                {
                    drow.Delete();
                    SaveTextsFile();
                }
            }


            dataset1.Tree.Rows.Find(Convert.ToInt32(ID))[3] = 0;
            SaveTreeFile();
        }

        public void DeleteDatabase()
        {
            dataset1.Clear();                    
            dataset1.Dispose();
        }
    }
}
