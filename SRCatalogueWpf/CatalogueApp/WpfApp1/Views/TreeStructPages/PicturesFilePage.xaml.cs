using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1.Views.TreeStructPages
{
    /// <summary>
    /// Логика взаимодействия для PicturesFilePage.xaml
    /// </summary>
    public partial class PicturesFilePage : UserControl
    {
        public PicturesFilePage()
        {
            InitializeComponent();
        }

        private void mainDataGrid_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            MessageBox.Show("DataContextChanged");
        }
    }
}
