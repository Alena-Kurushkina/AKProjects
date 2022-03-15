using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using WpfApp1.ViewModels;

namespace WpfApp1.Views
{
    /// <summary>
    /// Логика взаимодействия для ContentPage.xaml
    /// </summary>
    public partial class ContentPage : Page
    {
        public ContentPage(CNodeViewModel SelectedNode)
        {
            InitializeComponent();
            this.DataContext = new ShowContentWindowViewModel(SelectedNode);
        }
       

        //вернуться назад
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ShowContentWindowViewModel vm = this.DataContext as ShowContentWindowViewModel;
           
            ProjectsListPage prP = new ProjectsListPage(vm.database);
            this.NavigationService.Navigate(prP);
            this.NavigationService.RemoveBackEntry();
          
             //vm.database.CurrentProject = 0;
        }

        
        private void Edit_button_Click(object sender, RoutedEventArgs e)
        {
            ShowContentWindowViewModel vm = this.DataContext as ShowContentWindowViewModel;           

            EditWindowViewModel viewmodel = new EditWindowViewModel(vm);

            EditPage editP = new EditPage();
            editP.DataContext = viewmodel;
            this.NavigationService.Navigate(editP);            
                    
        }

        //private void DisplayModel_Click(object sender, RoutedEventArgs e)
        //{
            //ShowContentWindowViewModel vm = this.DataContext as ShowContentWindowViewModel;
            //if (vm.ModelDataRow != null)
            //{
            //    vm.NoModel = false;

            //    // Define variables to track the peak
            //    // memory usage of the process.
            //    long peakPagedMem = 0,
            //         peakWorkingSet = 0,
            //         peakVirtualMem = 0;

            //    using (Process myProcess = new Process())
            //    {

            //        myProcess.StartInfo.FileName = vm.SelectedNode.Database.soursePath + vm.SelectedNode.Database.CatalogueModels + vm.ModelDataRow[1];
            //        myProcess.Start();


            //        // Display the process statistics until
            //        // the user closes the program.

            //        if (!myProcess.HasExited)
            //        {
            //            Console.WriteLine();

            //            // Update the values for the overall peak memory statistics.
            //            peakPagedMem = myProcess.PeakPagedMemorySize64;
            //            peakVirtualMem = myProcess.PeakVirtualMemorySize64;
            //            peakWorkingSet = myProcess.PeakWorkingSet64;

            //            if (myProcess.Responding)
            //            {
            //                Console.WriteLine("Status = Running");
            //            }
            //            else
            //            {
            //                Console.WriteLine("Status = Not Responding");
            //            }
            //        }

            //        do { } while (!myProcess.WaitForExit(1000));

            //        Console.WriteLine();
            //        Console.WriteLine($"  Process exit code          : {myProcess.ExitCode}");

            //        // Display peak memory statistics for the process.
            //        Console.WriteLine($"  Peak physical memory usage : {peakWorkingSet}");
            //        Console.WriteLine($"  Peak paged memory usage    : {peakPagedMem}");
            //        Console.WriteLine($"  Peak virtual memory usage  : {peakVirtualMem}");

            //        myProcess.Dispose();
            //    }

            //    vm.NoModel = true;
            //}
            //else
            //{
            //    MessageBox.Show("Данный проект не содержит 3D модель", "Внимание", MessageBoxButton.OK);
            //}
        //}
      
    }
}
