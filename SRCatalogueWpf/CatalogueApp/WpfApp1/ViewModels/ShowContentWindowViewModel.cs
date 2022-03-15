using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WpfApp1.Models;
using WpfApp1.ViewModel;
using WpfApp1.ViewModels.Pages;
using WpfApp1.Views.Pages;

namespace WpfApp1.ViewModels
{
   public class ShowContentWindowViewModel: INotifyPropertyChanged
    {
        #region Fields

        private RelayCommand _changePageCommand;
        

        private IPageViewModel _currentPageViewModel;
        private List<IPageViewModel> _pageViewModels;
        public CNodeViewModel SelectedNode;

        public Database database;
        public DataRow ModelDataRow;
        public DataRow[] ImagesDataRows;
        public DataRow TextDataRow;
        public DataRow QRcodDataRow;
        public DataRow VideosDataRow;
        public DataRow ContactDataRow;

        private string _projectName;
        public DescriptionText DescriptionText;
                

        
        #endregion

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        const int SW_HIDE = 0;

        public ShowContentWindowViewModel(CNodeViewModel selectedNode)
        {
            //thisWindow = window;
            //EnergyMainW = EnergyMW;
            database = selectedNode.Database;
            
            SelectedNode = selectedNode;

            LoadData();           
          
        }

        public void LoadData()
        { 
            ImagesDataRows = SelectedNode.Database.GetImages(SelectedNode.ID);
            TextDataRow = SelectedNode.Database.GetText(SelectedNode.ID);
            ModelDataRow = SelectedNode.Database.GetModel(SelectedNode.ID);
            QRcodDataRow = SelectedNode.Database.GetQRCod(SelectedNode.ID);
            VideosDataRow = SelectedNode.Database.GetVideo(SelectedNode.ID);
            ContactDataRow = SelectedNode.Database.GetContact(SelectedNode.ID);
            _projectName = SelectedNode.Name;//SelectedNode.Database.GetProjectName(SelectedNode.ID);

            if (PageViewModels.Count != 0)
            {
                PageViewModels.Clear();
            }

            DescriptionText = new DescriptionText(database, TextDataRow); 

            DescriptionViewModel dvm = new DescriptionViewModel(ImagesDataRows, DescriptionText, ContactDataRow, SelectedNode.ID, database);
            PageViewModels.Add(dvm);
            if (QRcodDataRow != null)
            {
                QRCodeViewModel qvm = new QRCodeViewModel(QRcodDataRow, database);
                PageViewModels.Add(qvm);
            }
            if (VideosDataRow != null)
            {
                VideoViewModel vvm = new VideoViewModel(VideosDataRow, database);
                PageViewModels.Add(vvm);
            }

            

            if (ModelDataRow != null)
            {
                
                if (ModelDataRow[1].ToString().Split('.').Last() == "exe")
                {
                    Create3DModelViewModel();
                }else
                {
                   Helix3DmodelViewModel hvm = new Helix3DmodelViewModel(ModelDataRow, database);
                   PageViewModels.Add(hvm);
                }
            }           
            
            CurrentPageViewModel = PageViewModels[0];
        }

        public void Create3DModelViewModel()
        {
            ThreeDModelViewModel tvm = new ThreeDModelViewModel(ModelDataRow, database);
            PageViewModels.Add(tvm);
        }

        #region Properties / Commands

             

        public string ProjectName
        {
            get
            {
               return _projectName;
            }
            set
            {
                _projectName = value;
                OnPropertyChanged("ProjectName");
            }
        }

        public Visibility IsAdmin
        {
            get { return database.IsAdmin; }
        }

        public string ProjectID
        {
            get
            {
                return SelectedNode.ID;
            }
        }

        public RelayCommand ChangePageCommand
        {
            get
            {
                if (_changePageCommand == null)
                {
                    
                    _changePageCommand = new RelayCommand(
                        p => ChangeViewModel((IPageViewModel)p),
                        p => p is IPageViewModel);
                }

                return _changePageCommand;
            }
        }

        public List<IPageViewModel> PageViewModels
        {
            get
            {
                if (_pageViewModels == null)
                    _pageViewModels = new List<IPageViewModel>();

                return _pageViewModels;
            }
        }

        public IPageViewModel CurrentPageViewModel
        {
            get
            {
                return _currentPageViewModel;
            }
            set
            {
                if (_currentPageViewModel != value)
                {
                    _currentPageViewModel = value;
                    OnPropertyChanged("CurrentPageViewModel");
                }
            }
        }

        #endregion

        #region Methods

        private void ChangeViewModel(IPageViewModel viewModel)
        {          

            if (CurrentPageViewModel.IdentificationName == "3DModel")
            {
                CurrentPageViewModel.Dispose();                
            }
            if (viewModel.IdentificationName== "3DModel")
            {
                viewModel.Load();
            }

             if (!PageViewModels.Contains(viewModel))
                PageViewModels.Add(viewModel);

            CurrentPageViewModel = PageViewModels.FirstOrDefault(vm => vm == viewModel);
        }

        #endregion


      

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

        }

        private RelayCommand closing;
        public RelayCommand Closing
        {
            get
            {
                return closing ??
                    (closing = new RelayCommand(obj =>
                    {
                        foreach (IPageViewModel listi in PageViewModels)
                        {
                            listi.Dispose();
                        }

                        PageViewModels.Clear();

                    }));
            }
        }

        //public void Closing()
        //{
        //     foreach (IPageViewModel listi in PageViewModels)
        //     {
        //         listi.Dispose();                   
        //     }         

        //     PageViewModels.Clear();           
            
        //}
   }
}
