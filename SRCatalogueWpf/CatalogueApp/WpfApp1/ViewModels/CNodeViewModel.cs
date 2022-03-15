using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfApp1.Models;
using WpfApp1.ViewModels;
using WpfApp1.Views;

namespace WpfApp1.ViewModel
{
    public class CNodeViewModel : INotifyPropertyChanged
    {
        

       // public List<CNodeViewModel> _children;
        readonly CNodeViewModel _parent;
        readonly CNode _node;
        public Database Database;
        public CatalogueTreeViewModel CatTreeVM;
       // readonly bool _content;
        

        bool _isExpanded;
        bool _isSelected;

       //Конструкторы
        public CNodeViewModel(CNode node, CatalogueTreeViewModel ctwm)
            : this(node, null, ctwm)
        {
        }

        private CNodeViewModel(CNode node, CNodeViewModel parent, CatalogueTreeViewModel cattreevm)
        {           

            CatTreeVM = cattreevm;
            Database = cattreevm.CatalogueDatabase;
            
            _node = node;
            _parent = parent;
            //_children = new List<CNodeViewModel>(
            //        (from child in _node.Children
            //         select new CNodeViewModel(child, this, cattreevm))
            //         .ToList<CNodeViewModel>());
            Children = new ObservableCollection<CNodeViewModel>(
                   (from child in _node.Children
                    select new CNodeViewModel(child, this, cattreevm))
                    .ToList<CNodeViewModel>());

        }

        


        //public IList<CNodeViewModel> Children
        //{
        //    get { return _children; }
            
        //}

            public ObservableCollection<CNodeViewModel> Children { get; set; }

        public string Name
        {
            get { return _node.Name; }
            set { 
                _node.Name = value;
               this.OnPropertyChanged("Name");
            }
        }

        public string ID
        {
            get { return _node.ID; }
        }

        public CNodeViewModel Parent
        {
            get { return _parent; }
        }

        //public DataSet1 Dataset
        //{
        //    get { return _dataset; }
        //}
        public bool Content
        {
            get { return _node.Content; }
            set { _node.Content = value;
                OnPropertyChanged("Content");
            }
        }

       

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (value != _isExpanded)
                {
                    _isExpanded = value;
                    this.OnPropertyChanged("IsExpanded");
                }

                // Expand all the way up to the root.
                if (_isExpanded && _parent != null)
                    _parent.IsExpanded = true;
            }
        }

       

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is selected.
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    this.OnPropertyChanged("IsSelected");
                }
            }
        }



        public bool NameContainsText(string text)
        {
            if (String.IsNullOrEmpty(text) || String.IsNullOrEmpty(this.Name))
                return false;

            return this.Name.IndexOf(text, StringComparison.InvariantCultureIgnoreCase) > -1;
        }

       

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

        }


        //для кнопки "Просмотр"
        private RelayCommand showContent;
        public RelayCommand ShowContent
        {
            get
            {
                return showContent ??
                  (showContent = new RelayCommand(obj =>
                  {
                     CNodeViewModel selnode = obj as CNodeViewModel;
                     if (selnode != null)
                     {
                          Database.CurrentProject = Convert.ToInt32(selnode.ID);                        
                          ContentPage contP = new ContentPage(selnode);                          
                          CatTreeVM.thisPage.NavigationService.Navigate(contP);
                     }
                  },
                (obj) => Content)); // 
            }
        }

        private RelayCommand addProject;
        public RelayCommand AddProject
        {
            get
            {
                return addProject ??
                  (addProject = new RelayCommand(obj =>
                  {
                      CNodeViewModel selnode = obj as CNodeViewModel;
                      if (selnode != null)
                      {
                           ImDescription_dialog dialog = new ImDescription_dialog("Введите название раздела", true, "");
                          if (dialog.ShowDialog() == true)
                          {
                              
                              DataRow dr = this.Database.AddPazdel(dialog.Im_Description, selnode.ID);
                              if (dr!=null) { 

                                  CNode n = new CNode(dr, this.Database.dataset1.Tree);
                                  CNodeViewModel nvm = new CNodeViewModel(n, selnode, CatTreeVM);
                                  // selnode._children.Add(nvm);
                                  selnode.Children.Add(nvm);
                              }

                          }
                      }
                  },
                (obj)=> obj!=null)); // 
            }
        }

        private RelayCommand deleteProject;
        public RelayCommand DeleteProject
        {
            get
            {
                return deleteProject ??
                  (deleteProject = new RelayCommand(obj =>
                  {
                      CNodeViewModel selnode = obj as CNodeViewModel;
                      if (selnode != null)
                      {
                         if (selnode.Children.Count != 0)
                         {
                             MessageBoxResult result = MessageBox.Show("Этот раздел содержит подразделы. Вы уверены, что хотите удалить этот раздел со всем содержимым?", "Внимание", MessageBoxButton.YesNo);

                              if (result == MessageBoxResult.Yes)
                              {
                                  DeleteProjAndCh(selnode);
                                  selnode.Database.SaveTreeFile();
                                  selnode.Parent.Children.Remove(selnode);
                              }
                              else return;
                         }else
                         {
                              MessageBoxResult res = MessageBox.Show("Раздел и его контент будут удалёны из каталога", "Предупреждение", MessageBoxButton.OKCancel);
                              if (res == MessageBoxResult.OK)
                              {
                                  DeleteDescription(selnode);
                                  selnode.Database.dataset1.Tree.Rows.Find(Convert.ToInt32(selnode.ID)).Delete();
                                  selnode.Database.SaveTreeFile();
                                  selnode.Parent.Children.Remove(selnode);
                              }
                              else return;
                         }
                          //if (dialog.ShowDialog() == true)
                          //{
                          //    // MessageBox.Show("Будет добавлен раздел " + dialog.Im_Description);

                          //    DataRow dr = this.Database.dataset1.Tree.NewRow();
                          //    dr[1] = dialog.Im_Description.Trim();
                          //    dr[2] = selnode.ID;
                          //    dr[3] = 0;
                          //    this.Database.dataset1.Tree.Rows.Add(dr);

                          //    if (this.Database.SaveTreeFile())
                          //    {

                          //        CNode n = new CNode(dr, this.Database.dataset1.Tree);
                          //        CNodeViewModel nvm = new CNodeViewModel(n, selnode, CatTreeVM);
                          //        // selnode._children.Add(nvm);
                          //        selnode.Children.Add(nvm);
                          //    }

                          //}
                      }
                  },
                (obj) => obj != null)); // 
            }
        }

        private void DeleteProjAndCh(CNodeViewModel cnod)
        {
            foreach (CNodeViewModel cn in cnod.Children)
            {
                DeleteProjAndCh(cn);
              //  DeleteDescription(cn);
               // cnod.Database.dataset1.Tree.Rows.Find(Convert.ToInt32(cn.ID)).Delete();                
               // cnod.Parent.Children.Remove(cnod);
            }
            DeleteDescription(cnod);
            cnod.Database.dataset1.Tree.Rows.Find(Convert.ToInt32(cnod.ID)).Delete();           
           // cnod.Parent.Children.Remove(cnod);
        }

        private RelayCommand addDesc;
        public RelayCommand AddDesc
        {
            get
            {
                return addDesc ??
                  (addDesc = new RelayCommand(obj =>
                  {
                      CNodeViewModel selnode = obj as CNodeViewModel;
                      if (selnode != null)
                      {
                           this.Database.dataset1.Tree.Rows.Find(Convert.ToInt32(selnode.ID))[3] = 1;

                          if (this.Database.SaveTreeFile()) { 

                              selnode.Content = true;

                              EditWindowViewModel viewmodel = new EditWindowViewModel(selnode);

                            EditPage editP = new EditPage();
                            editP.DataContext = viewmodel;
                            CatTreeVM.thisPage.NavigationService.Navigate(editP);
                              this.Database.CurrentProject = Convert.ToInt32(selnode.ID);
                          } 
                      }
                  },
                (obj) => Content==false)); // 
            }
        }

        private RelayCommand deleteDesc;
        public RelayCommand DeleteDesc
        {
            get
            {
                return deleteDesc ??
                  (deleteDesc = new RelayCommand(obj =>
                  {
                      CNodeViewModel selnode = obj as CNodeViewModel;
                      if (selnode != null)
                      {
                          MessageBoxResult res = MessageBox.Show("Все материалы этого проекта будут удалены", "Предупреждение", MessageBoxButton.OKCancel);
                          if (res == MessageBoxResult.OK)
                          {
                              DeleteDescription(selnode);
                          }
                          else return;                         

                      }
                  },
                (obj) => Content == true)); 
            }
        } 


        private void DeleteDescription(CNodeViewModel seln)
        {

            Database.DeleteDescription(seln.ID);
            this.Content = false;
        }





    }
}
