using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.ViewModels
{
    public interface IPageViewModel
    {
        PackIconKind Icon { get; }
        void Dispose();
        void Load();

        string IdentificationName { get; }
    }
   
}  
