using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Model.Scene;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.ViewModels
{
    class AttachedNodeViewModel: INotifyPropertyChanged
    {
        private bool selected = false;
        public bool Selected
        {
            set
            {
                selected = value;

                if (node is MeshNode m)
                {
                    
                    m.PostEffects = value ? $"highlight[color:#FFFF00]" : "";
                    foreach (var n in node.TraverseUp())
                    {
                        if (n.Tag is AttachedNodeViewModel vm)
                        {
                            vm.Expanded = true;
                        }
                    }
                }
                OnPropertyChanged("Selected");

            }
            get => selected;
        }

        private bool expanded = true;
        public bool Expanded
        {
            set
            {
                expanded = value;
                OnPropertyChanged("Expanded");
            }
            get => expanded;
        }

        public bool IsAnimationNode { get => node.IsAnimationNode; }

        public string Name { get => node.Name; }

        private SceneNode node;

        public AttachedNodeViewModel(SceneNode node)
        {
            this.node = node;
            node.Tag = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
