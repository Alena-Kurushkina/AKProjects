using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Model.Scene;
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
using WpfApp1.ViewModels;

namespace WpfApp1.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для Helix3Dmodel.xaml
    /// </summary>
    public partial class Helix3Dmodel : UserControl
    {
        public Helix3Dmodel()
        {
            InitializeComponent();

            Viewport.AddHandler(Element3D.MouseDown3DEvent, new RoutedEventHandler((s, e) =>
            {
                var arg = e as MouseDown3DEventArgs;

                if (arg.HitTestResult == null)
                {
                    return;
                }
                if (arg.HitTestResult.ModelHit is SceneNode node && node.Tag is AttachedNodeViewModel vm)
                {
                    vm.Selected = !vm.Selected;
                }
            }));

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Viewport.ZoomExtents(1000);
           
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
           // Helix3DmodelViewModel vm = this.DataContext as Helix3DmodelViewModel;
           // vm.OpenFile();

            
        }

        private void TreeView_Selected(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = e.OriginalSource as TreeViewItem;
            if (item != null)
                item.BringIntoView();
        }
    }
}
