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

namespace WpfApp1.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для DescriptionView.xaml
    /// </summary>
    public partial class DescriptionView : UserControl
    {
        public DescriptionView()
        {
            InitializeComponent();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
           // MessageBox.Show("UserControl_Unloaded");
            //richtb.Document = new FlowDocument();
        }



        //private void tb_Click(object sender, RoutedEventArgs e)
        //{
        //    TextSelection ts = Richtextbox.Selection;
        //    if (!ts.IsEmpty)
        //    {
        //        ts.ApplyPropertyValue(TextElement.FontSizeProperty, 20.0);
        //    }
        //}

        //private bool _columnSizeChanging;
        //private void Image_SizeChanged(object sender, SizeChangedEventArgs e)
        //{
        //    _columnSizeChanging = true;
        //    if (sender != null)
        //        Mouse.AddPreviewMouseUpHandler(this, BaseDataGrid_MouseLeftButtonUp);
        //}
        //void BaseDataGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    if (_columnSizeChanging)
        //    {
        //        _columnSizeChanging = false;
        //         MessageBox.Show("fhfh");

        //    }

        //}
    }
}
