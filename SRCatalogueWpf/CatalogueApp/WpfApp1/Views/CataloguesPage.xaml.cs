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
    /// Логика взаимодействия для CataloguesPage.xaml
    /// </summary>
    public partial class CataloguesPage : Page
    {
        public CataloguesPage()
        {
            InitializeComponent();            
            CurrentDatabase= new Database("TreeEnergy", Visibility.Hidden);
            DataContext = new CataloguesPageViewModel(CurrentDatabase);
        }
        public CataloguesPage(Database db)
        {
            InitializeComponent();
            CurrentDatabase = db;          
            DataContext = new CataloguesPageViewModel(CurrentDatabase);            
        }


        Database CurrentDatabase;
        Visibility IsAdmin;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentDatabase.CatalogueTxtFile != "TreeEnergy.txt") { 
                if (CurrentDatabase != null)
                {
                    IsAdmin = CurrentDatabase.IsAdmin;
                    CurrentDatabase.DeleteDatabase();
                    CurrentDatabase = null;
                }
                CurrentDatabase = new Database("TreeEnergy", IsAdmin); 
            }
            ProjectsListPage nextPage = new ProjectsListPage(CurrentDatabase);
            this.NavigationService.Navigate(nextPage);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (CurrentDatabase.CatalogueTxtFile != "TreeERIT.txt")
            {
                if (CurrentDatabase != null)
                {
                    IsAdmin = CurrentDatabase.IsAdmin;
                    CurrentDatabase.DeleteDatabase();
                    CurrentDatabase = null;
                }

                CurrentDatabase = new Database("TreeERIT", IsAdmin);
            }
            ProjectsListPage nextPage = new ProjectsListPage(CurrentDatabase);
            this.NavigationService.Navigate(nextPage);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (CurrentDatabase.CatalogueTxtFile != "TreeTI4R.txt")
            {
                if (CurrentDatabase != null)
                {
                    IsAdmin = CurrentDatabase.IsAdmin;
                    CurrentDatabase.DeleteDatabase();
                    CurrentDatabase = null;
                }

                CurrentDatabase = new Database("TreeTI4R", IsAdmin);
            }
            ProjectsListPage nextPage = new ProjectsListPage(CurrentDatabase);
            this.NavigationService.Navigate(nextPage);
        }

        private void DialogHost_DialogClosing(object sender, MaterialDesignThemes.Wpf.DialogClosingEventArgs eventArgs)
        {
                       
            if ((bool)eventArgs.Parameter == true)
            {
                  Crypto cr = new Crypto(CurrentDatabase);
               // cr.Encrypt("p2ssw0rd");
                string pas=cr.Decrypt(enterstr.Password.Length);
                if (pas == enterstr.Password)
                {
                    CurrentDatabase.IsAdmin = Visibility.Visible;
                    CataloguesPageViewModel vm = this.DataContext as CataloguesPageViewModel;
                    vm.IsAdmin = Visibility.Visible;
                    EnterBtn.Visibility = Visibility.Hidden;
                    enterstr.Password = null;
                }
                else
                {
                    MessageBox.Show("Пароль неверный");
                    enterstr.Password = null;
                }

            }
            
        }

        private void Dialog_ChangePas_Closing(object sender, MaterialDesignThemes.Wpf.DialogClosingEventArgs eventArgs)
        {

            if ((bool)eventArgs.Parameter == true)
            {
                if (enternewstr.Password.Length > 3)
                {
                    Crypto cr = new Crypto(CurrentDatabase);
                    string pas = cr.Decrypt(enteroldstr.Password.Length);

                    if (pas == enteroldstr.Password)
                    {
                        cr.Encrypt(enternewstr.Password);
                        CurrentDatabase.IsAdmin = Visibility.Hidden;
                        CataloguesPageViewModel vm = this.DataContext as CataloguesPageViewModel;
                        vm.IsAdmin = Visibility.Hidden;
                        EnterBtn.Visibility = Visibility.Visible;
                        enteroldstr.Password = null;
                        enternewstr.Password = null;
                    }
                    else
                    {
                        MessageBox.Show("Пароль неверный");
                        enteroldstr.Password = null;
                        enternewstr.Password = null;
                    }
                }else
                {
                    MessageBox.Show("Пароль слишком короткий");
                    enteroldstr.Password = null;
                    enternewstr.Password = null;
                }
            }

        }
    }
}
