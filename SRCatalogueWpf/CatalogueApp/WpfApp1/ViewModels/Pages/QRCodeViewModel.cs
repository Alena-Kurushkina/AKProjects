using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.ViewModel;
using WpfApp1.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.IO;
using System.Windows;

namespace WpfApp1.ViewModels.Pages
{
   
    public class QRCodeViewModel :IPageViewModel
    {
        public string _QR_path;
        public DataRow QRcodDataRow;
        private Database database;
        public QRCodeViewModel(DataRow qrdr, Database db)
        {
            QRcodDataRow = qrdr;
            database = db;

            if (QRcodDataRow != null)
            {
                string p = database.soursePath + database.CatalogueQR + QRcodDataRow[1].ToString();
                if (File.Exists(p))
                {
                    _QR_path = p;
                }
                else
                {
                    MessageBox.Show("Изображение " + QRcodDataRow[1].ToString() + " не найдено в папке Catalogue_QR. Возможно, во время удалении этого изображения произошла ошибка перезаписи файла QRs.txt");
                }
            }
            else _QR_path = null;
        }

        public string QR_cod
        {
            get {
                return _QR_path;
            }
            set
            {
                _QR_path = value;
                OnPropertyChanged(QR_cod);
            }
        }

        public PackIconKind Icon
        {
            get { return MaterialDesignThemes.Wpf.PackIconKind.QrcodeScan; }
        }

        public void Dispose() { }

        public void Load() { }

        public string IdentificationName
        {
            get { return "QRCode"; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
