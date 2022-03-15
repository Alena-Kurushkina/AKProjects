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
using WpfApp1.ViewModel;

namespace WpfApp1.Views
{
    /// <summary>
    /// Логика взаимодействия для ProjectsListPage.xaml
    /// </summary>
    public partial class ProjectsListPage : Page
    {
        readonly CatalogueTreeViewModel _catalogueTree;
        private Database CatalogueDatabase;

        public ProjectsListPage(Database CurrentDatabase)
        {
            InitializeComponent();

            CatalogueDatabase = CurrentDatabase;

            //TreeViewItem item = tree.Items[CurrentDatabase.CurrentProject] as TreeViewItem;
            //item.BringIntoView();

            //получить данные 
            CNode rootNode = CatalogueDatabase.GetCatalogueTree();
            //создание модели представления
            _catalogueTree = new CatalogueTreeViewModel(rootNode, CatalogueDatabase,this);
            // привязка модели к treeview
            DataContext = _catalogueTree;            
        }

        void searchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                _catalogueTree.SearchCommand.Execute(null);
        }

        //вернуться на предыдущюю страницу
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CataloguesPage catP = new CataloguesPage(CatalogueDatabase);
            this.NavigationService.Navigate(catP);
            this.NavigationService.RemoveBackEntry();
            CatalogueDatabase.CurrentProject = 0;
        }

        private void tree_Selected(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = e.OriginalSource as TreeViewItem;
            if (item != null)
                item.BringIntoView();
        }

        private void TreeStruct_Click_1(object sender, RoutedEventArgs e)
        {
            ShowTreeStructPage trstr = new ShowTreeStructPage(CatalogueDatabase);
            this.NavigationService.Navigate(trstr);
            this.NavigationService.RemoveBackEntry();


        }
    }
}
