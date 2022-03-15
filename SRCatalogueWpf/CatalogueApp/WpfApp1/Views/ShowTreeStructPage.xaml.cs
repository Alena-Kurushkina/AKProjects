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

namespace WpfApp1.Views
{
    /// <summary>
    /// Логика взаимодействия для ShowTreeStructPage.xaml
    /// </summary>
    public partial class ShowTreeStructPage : Page
    {
        public ShowTreeStructPage(Database db)
        {
            InitializeComponent();
            database = db;
            this.DataContext = new TreeStructPageViewModel(db);

            //TreeDatagrid.ItemsSource = db.dataset1.Tree.DefaultView;
            //PicturesDatagrid.ItemsSource = db.dataset1.Pictures.DefaultView;
            //ModelsDatagrid.ItemsSource = db.dataset1.Models.DefaultView;
            //QRsDatagrid.ItemsSource = db.dataset1.QRs.DefaultView;
            //VideosDatagrid.ItemsSource = db.dataset1.Videos.DefaultView;
            //ContactsDatagrid.ItemsSource = db.dataset1.Contact.DefaultView;
            //TextsDatagrid.ItemsSource = db.dataset1.Texts.DefaultView;
        }

        private Database database;

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            ProjectsListPage prl = new ProjectsListPage(database);
            this.NavigationService.Navigate(prl);
            this.NavigationService.RemoveBackEntry();
        }
    }
}
