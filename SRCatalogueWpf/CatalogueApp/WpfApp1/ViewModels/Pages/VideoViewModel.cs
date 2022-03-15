using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MaterialDesignThemes.Wpf;

namespace WpfApp1.ViewModels.Pages
{
    class VideoViewModel: IPageViewModel
    {
        public VideoViewModel(DataRow dr, Database db)
        {
            if (dr != null)
            {
                string p = db.soursePath + db.CatalogueVideos + dr[1].ToString();
                if (File.Exists(p))
                {
                    video_path = p;
                }
                else
                {
                    MessageBox.Show("Видео " + dr[1].ToString() + " не найдено в папке Catalogue_Videos. Возможно, во время удалении этого файла произошла ошибка перезаписи файла Videos.txt");
                }
            }
            else video_path = null;
        }

        readonly string video_path;

        public void Dispose()  {   }

        public void Load() { }

        public string IdentificationName
        {
            get { return "Video"; }
        }

        public PackIconKind Icon
        {
            get { return MaterialDesignThemes.Wpf.PackIconKind.Videocam; }
        }

        public string Video
        {
            get { return video_path; }
        }
    }
}
