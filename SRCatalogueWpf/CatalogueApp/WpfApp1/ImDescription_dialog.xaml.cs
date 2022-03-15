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
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для ImDescription_dialog.xaml
    /// </summary>
    public partial class ImDescription_dialog : Window
    {
        public ImDescription_dialog(string Text, bool Multilyne, string defaultname)
        {
            InitializeComponent();
            textbl.Text = Text;
            descBox.Text = defaultname;
            if (Multilyne is true)
            {
                descBox.Width = 400;
                descBox.Height = 60;
            }
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        public string Im_Description
        {
            get { return descBox.Text; }
        }
    }
}
