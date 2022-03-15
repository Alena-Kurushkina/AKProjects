using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfApp1.Views;

namespace WpfApp1.ViewModel
{
   public class CatalogueTreeViewModel
    {
       

        readonly ReadOnlyCollection<CNodeViewModel> _roots; //firstgeneration
        readonly CNodeViewModel _rootCNode;
        readonly ICommand _searchCommand;
        public Database CatalogueDatabase;
        public ProjectsListPage thisPage;
      

        IEnumerator<CNodeViewModel> _matchingNodesEnumerator; //matchingpeopleenumerator
        string _searchText = String.Empty;

       
        public CatalogueTreeViewModel(CNode rootNode, Database db, ProjectsListPage thisp)
        {
            CatalogueDatabase = db;
            thisPage = thisp;
         
            _rootCNode = new CNodeViewModel(rootNode,this);
            _rootCNode.IsExpanded = true;
            
            _roots = new ReadOnlyCollection<CNodeViewModel>(  new CNodeViewModel[] { _rootCNode });
                      
            _searchCommand = new SearchCatalogueTreeCommand(this);
         
            FindCurProject(CatalogueDatabase.CurrentProject, _rootCNode);
        }


      

        public Visibility IsAdmin
        {
            get { return CatalogueDatabase.IsAdmin; }
        }
        

        /// <summary>
        /// Returns a read-only collection containing the first person 
        /// in the family tree, to which the TreeView can bind.
        /// </summary>
        public ReadOnlyCollection<CNodeViewModel> GetRoots
        {
            get { return _roots; }
        }

             


        /// <summary>
        /// Returns the command used to execute a search in the family tree.
        /// </summary>
        public ICommand SearchCommand
        {
            get { return _searchCommand; }
        }

        private class SearchCatalogueTreeCommand : ICommand
        {
            readonly CatalogueTreeViewModel _catalogueTree;

            public SearchCatalogueTreeCommand(CatalogueTreeViewModel familyTree)
            {
                _catalogueTree = familyTree;
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            event EventHandler ICommand.CanExecuteChanged
            {
                // I intentionally left these empty because
                // this command never raises the event, and
                // not using the WeakEvent pattern here can
                // cause memory leaks.  WeakEvent pattern is
                // not simple to implement, so why bother.
                add { }
                remove { }
            }

            public void Execute(object parameter)
            {
                _catalogueTree.PerformSearch();
            }
        }

        

        /// <summary>
        /// Gets/sets a fragment of the name to search for.
        /// </summary>
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                if (value == _searchText)
                    return;

                _searchText = value;

                _matchingNodesEnumerator = null;
            }
        }

       

        void PerformSearch()
        {
            if (_matchingNodesEnumerator == null || !_matchingNodesEnumerator.MoveNext())
                this.VerifyMatchingNodesEnumerator();

           var cnode = _matchingNodesEnumerator.Current;

            if (cnode == null)
                return;

            // Ensure that this person is in view.
            if (cnode.Parent != null)
                cnode.Parent.IsExpanded = true;

            cnode.IsSelected = true;
        }

        void VerifyMatchingNodesEnumerator()
        {
            var matches = this.FindMatches(_searchText, _rootCNode);
            _matchingNodesEnumerator = matches.GetEnumerator();

            if (!_matchingNodesEnumerator.MoveNext())
            {
                MessageBox.Show(
                    "Поиск не дал результатов.",
                    "Попробуйте ещё раз",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                    );
            }
        }

        IEnumerable<CNodeViewModel> FindMatches(string searchText, CNodeViewModel cnode)
        {
            if (cnode.NameContainsText(searchText))
                yield return cnode;

            foreach (CNodeViewModel child in cnode.Children)
                foreach (CNodeViewModel match in this.FindMatches(searchText, child))
                    yield return match;
        }

        private void FindCurProject(int id, CNodeViewModel ch)
        {
            foreach (CNodeViewModel node in ch.Children)
            {                
                if (node.ID == id.ToString()) { node.Parent.IsExpanded = true; }
                FindCurProject(id, node);                
            }            
        }


        //private RelayCommand selectCurProject;
        //public RelayCommand SelectCurProject
        //{
        //    get
        //    {
        //        return selectCurProject ??
        //          (selectCurProject = new RelayCommand(obj =>
        //          {
        //             // MessageBox.Show("Work");
        //              FindCurProject(CatalogueDatabase.CurrentProject, _rootCNode);
        //          })); 
        //    }
        //}
    }
}
