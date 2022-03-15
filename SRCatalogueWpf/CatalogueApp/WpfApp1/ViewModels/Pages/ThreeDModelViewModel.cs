using MaterialDesignThemes.Wpf;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Windows;

namespace WpfApp1.ViewModels.Pages
{
    public class ThreeDModelViewModel :IPageViewModel
    {
        public DataRow ModelDataRow;
               

        public ThreeDModelViewModel(DataRow ModelDR, Database db) {

            ModelDataRow = ModelDR;
            if (ModelDataRow != null)
            {
                model_path = db.soursePath+db.CatalogueModels + ModelDataRow[1].ToString();
               // Load();              

            }

        }

        public string model_path;

       public UIElement _child;
       private UnityHwndHost host;



        public PackIconKind Icon
        {
            get { return MaterialDesignThemes.Wpf.PackIconKind.CubeOutline; }
        }

       

        public void Load()
        {
            host = new UnityHwndHost(model_path);
        }

        public void Dispose() {

            if (host != null) {
                host.Dispose(); 
            }            

        }

        public string IdentificationName
        {
            get { return "3DModel"; }
        }

        public UIElement Model
        {
            get
            {                
                return _child = host;
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
