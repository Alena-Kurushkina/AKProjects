using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Models
{
    class MediaEdit
    {
        public MediaEdit(Database db, string Pr_Id)
        {
            database = db;           
            Project_Id = Pr_Id;
        }

        Database database;
        private string Dest_str;
        private string Sourse_str;
        private string Project_Id;

        public string new_file_name;
        private string exe_new_data;
        public string old_file_name;
        private string exe_old_data;

        public string video_path;
        public string old_video_path;

        public bool Get3DModelFile()
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "All Supported |*.exe;*.3d;*.3ds;*.3mf;*.ac;*.ac3d;*.acc;*.amf;*.ase;" +
                "*.ask;*.assbin;*.b3d;*.blend;*.bvh;*.cob;*.csm;*.dae;*.dxf;*.enff;*.fbx;*.glb;*.gltf;*.hmp;*.ifc;" +
                "*.ifczip;*.irr;*.irrmesh;*.lwo;*.lws;*.lxo;*.md2;*.md3;*.md5anim;*.md5camera;*.md5mesh;*.mdc;*.mdl;" +
                "*.mesh;*.mesh.xml;*.mot;*.ms3d;*.ndo;*.nff;*.obj;*.off;*.ogex;*.pk3;*.ply;*.pmx;*.prj;*.q3o;*.q3s;" +
                "*.raw;*.scn;*.sib;*.smd;*.stl;*.stp;*.ter;*.uc;*.vta;*.x;*.x3d;*.x3db;*.xgl;*.xml;*.zgl;|Exe Files(*.exe)|*.exe|(*.3d)|*.3d|" +
                "(*.3ds)|*.3ds|(*.3mf)|*.3mf|(*.ac)|*.ac|(*.ac3d)|*.ac3d|(*.acc)|*.acc|(*.amf)|*.amf|(*.ase)|*.ase|" +
                "(*.ask)|*.ask|(*.assbin)|*.assbin|(*.b3d)|*.b3d|(*.blend)|*.blend|(*.bvh)|*.bvh|(*.cob)|*.cob|(*.csm)|" +
                "*.csm|(*.dae)|*.dae|(*.dxf)|*.dxf|(*.enff)|*.enff|(*.fbx)|*.fbx|(*.glb)|*.glb|(*.gltf)|*.gltf|(*.hmp)|" +
                "*.hmp|(*.ifc)|*.ifc|(*.ifczip)|*.ifczip|(*.irr)|*.irr|(*.irrmesh)|*.irrmesh|(*.lwo)|*.lwo|(*.lws)|*.lws|" +
                "(*.lxo)|*.lxo|(*.md2)|*.md2|(*.md3)|*.md3|(*.md5anim)|*.md5anim|(*.md5camera)|*.md5camera|(*.md5mesh)|" +
                "*.md5mesh|(*.mdc)|*.mdc|(*.mdl)|*.mdl|(*.mesh)|*.mesh|(*.mesh.xml)|*.mesh.xml|(*.mot)|*.mot|(*.ms3d)|" +
                "*.ms3d|(*.ndo)|*.ndo|(*.nff)|*.nff|(*.obj)|*.obj|(*.off)|*.off|(*.ogex)|*.ogex|(*.pk3)|*.pk3|(*.ply)|" +
                "*.ply|(*.pmx)|*.pmx|(*.prj)|*.prj|(*.q3o)|*.q3o|(*.q3s)|*.q3s|(*.raw)|*.raw|(*.scn)|*.scn|(*.sib)|*.sib|" +
                "(*.smd)|*.smd|(*.stl)|*.stl|(*.stp)|*.stp|(*.ter)|*.ter|(*.uc)|*.uc|(*.vta)|*.vta|(*.x)|*.x|(*.x3d)|" +
                "*.x3d|(*.x3db)|*.x3db|(*.xgl)|*.xgl|(*.xml)|*.xml|(*.zgl)|*.zgl";
            // ofd.Filter = "Exe Files(*.exe)|*.exe|"+ OpenFileFilter;

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                Sourse_str = ofd.FileName.Substring(0, ofd.FileName.Length - ofd.SafeFileName.Length);
                Dest_str = database.soursePath + database.CatalogueModels;
                 old_file_name = ofd.SafeFileName;

                string exten = old_file_name.Split('.').Last();//расширение файла
                string exe_name = old_file_name.Substring(0, old_file_name.Length - exten.Length - 1);//название без расширения  

              //  string new_file_name;

                if (exten == "exe")
                {
                   exe_old_data = Sourse_str + exe_name + "_Data"; //путь к каталогу _Data 

                    if (Directory.Exists(exe_old_data))
                    {
                        new_file_name = old_file_name;
                        while (File.Exists(System.IO.Path.Combine(Dest_str, new_file_name)))
                        {
                            Rename_dialog rd = new Rename_dialog(new_file_name);
                            if (rd.ShowDialog() == true)
                            {
                                new_file_name = rd.Renamed;
                                
                            }
                            else
                            {
                                new_file_name = null;
                                return false;
                            }
                        }
                        string exe_new_name = new_file_name.Substring(0, new_file_name.Length - exten.Length - 1);//новое название без расширения

                        exe_new_data = Dest_str + exe_new_name + "_Data";
                        return true;

                    }
                    else
                    {
                        System.Windows.MessageBox.Show("В указанной директории не был найден каталог " + exe_name + "_Data");
                        return false;
                    }
                }
                else
                {
                    new_file_name = old_file_name;
                    while (File.Exists(System.IO.Path.Combine(Dest_str, new_file_name)))
                    {
                        Rename_dialog rd = new Rename_dialog(new_file_name);
                        if (rd.ShowDialog() == true)
                        {
                            new_file_name = rd.Renamed;
                            //return true;
                        }
                        else
                        {
                            new_file_name = null;
                            return false;
                        }
                    }
                    return true;
                }
                
            }
            else return false;
        }

        public DataRow Add3DModelToProject()
        {
            string exten = old_file_name.Split('.').Last();//расширение файла

            if (exten == "exe")
            {
                try
                {
                    //копирование файла exe
                    File.Copy(System.IO.Path.Combine(Sourse_str, old_file_name), System.IO.Path.Combine(Dest_str, new_file_name));
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                    return null;
                }

                //копирование папки _Data
                try
                {
                    Directory.CreateDirectory(exe_new_data);
                    foreach (string dirPath in Directory.GetDirectories(exe_old_data, "*", SearchOption.AllDirectories))
                    {
                        Directory.CreateDirectory(dirPath.Replace(exe_old_data, exe_new_data));
                    }

                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show(e.Message);
                    File.Delete(System.IO.Path.Combine(Dest_str, new_file_name));
                    Directory.Delete(exe_new_data, true);
                    return null;
                }

                try
                {

                    string[] files = Directory.GetFiles(exe_old_data, "*.*", SearchOption.AllDirectories);
                    foreach (string file in files)
                    {
                        File.Copy(file, file.Replace(exe_old_data, exe_new_data), true);
                    }
                    //return true;
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message, "Ошибка при добавлении 3D модели в Catalogue_Models");
                    Directory.Delete(exe_new_data, true);
                    File.Delete(System.IO.Path.Combine(Dest_str, new_file_name));
                    return null;
                }

            }
            else
            {
                try
                {
                    //копирование файла
                    File.Copy(System.IO.Path.Combine(Sourse_str, old_file_name), System.IO.Path.Combine(Dest_str, new_file_name));
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                    return null;
                }
            }

            if (new_file_name != null)
            {
                try
                {
                    DataRow dr = database.dataset1.Models.NewRow();
                    dr["id"] = Project_Id;
                    dr["exeName"] = new_file_name;
                    database.dataset1.Models.Rows.Add(dr);
                    database.SaveModelsFile();
                    return dr;
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message, "Ошибка при добавлении строки в Models.txt");
                    return null;
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Не удалось добавить 3D модель.");
                return null;
            }
        }

        public bool GetVideoFile()
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "Video Files (*.avi, *.mp4)|*.avi;*.mp4|Video Files (*.wmv)|*.wmv";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Sourse_str = ofd.FileName.Substring(0, ofd.FileName.Length - ofd.SafeFileName.Length);
                Dest_str = database.soursePath + database.CatalogueVideos;
                old_video_path = ofd.SafeFileName;

                 video_path = old_video_path;
                try
                {
                    while (File.Exists(System.IO.Path.Combine(Dest_str, video_path)))
                    {
                        Rename_dialog rd = new Rename_dialog(video_path);
                        if (rd.ShowDialog() == true)
                        {
                            video_path = rd.Renamed;                            
                        }
                        else return false;
                    }
                    return true;
                }
                catch (IOException ex)
                {
                    System.Windows.MessageBox.Show(ex.Message + " Переименуйте файл.", "Ошибка при добавлении изображения в Catalogue_Videos");
                    return false;
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message, "Ошибка при добавлении изображения в Catalogue_Videos");
                    return false;
                }
            }else return false;
        }

        public DataRow AddVideoToProject()
        {
            try
            {
                File.Copy(System.IO.Path.Combine(Sourse_str, old_video_path), System.IO.Path.Combine(Dest_str, video_path));
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Ошибка при добавлении изображения в Catalogue_Videos");
                return null;
            }
            try
            {
                DataRow dr = database.dataset1.Videos.NewRow();
                dr["id"] = Project_Id;
                dr["path"] = video_path;

                database.dataset1.Videos.Rows.Add(dr);
                database.SaveVideosFile();
                return dr;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Ошибка при добавлении строки в Videos.txt");
                return null;
            }
        }

        public bool GetQRFile()
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "Image Files(*.BMP;*.JPG;*.PNG)|*.BMP;*.JPG;*.PNG";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Sourse_str = ofd.FileName.Substring(0, ofd.FileName.Length - ofd.SafeFileName.Length);
                Dest_str = database.soursePath + database.CatalogueQR;
                old_file_name = ofd.SafeFileName;

                new_file_name = old_file_name;
                try
                {
                    while (File.Exists(System.IO.Path.Combine(Dest_str, new_file_name)))
                    {
                        Rename_dialog rd = new Rename_dialog(new_file_name);
                        if (rd.ShowDialog() == true)
                        {
                            new_file_name = rd.Renamed;
                        }
                        else return false;
                    }
                    return true;
                }
                catch (IOException ex)
                {
                    System.Windows.MessageBox.Show(ex.Message + " Переименуйте файл.", "Ошибка при добавлении изображения в Catalogue_QR");
                    return false;
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message, "Ошибка при добавлении изображения в Catalogue_QR");
                    return false;
                }
            }
            else return false;
        }

        public DataRow AddQRCode()
        {
            try
            {
                File.Copy(System.IO.Path.Combine(Sourse_str, old_file_name), System.IO.Path.Combine(Dest_str, new_file_name));

                if (File.Exists(System.IO.Path.Combine(Dest_str, new_file_name)))
                {
                    try
                    {

                        DataRow dr = database.dataset1.QRs.NewRow();
                        dr["id"] = Project_Id;
                        dr["path"] = new_file_name;

                        database.dataset1.QRs.Rows.Add(dr);
                        database.SaveQRsFile();
                        return dr;

                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show(ex.Message, "Ошибка при добавлении строки в QRs.txt");
                        return null;
                    }
                                      
                }
                else return null;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Ошибка при добавлении изображения в Catalogue_QR");
                return null;
            }
        }
    }
}
