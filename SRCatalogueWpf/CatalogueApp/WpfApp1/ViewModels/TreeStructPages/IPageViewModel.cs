using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.ViewModels.TreeStructPages
{
    interface IPageViewModel
    {
        string Name { get; }
        void Dispose();

        void DeleteItem(int id);
    }
}
