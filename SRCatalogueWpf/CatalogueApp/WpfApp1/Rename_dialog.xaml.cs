﻿using System;
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
    /// Логика взаимодействия для Rename_dialog.xaml
    /// </summary>
    public partial class Rename_dialog : Window
    {
        public Rename_dialog(string name)
        {
            InitializeComponent();
            renameBox.Text = name;
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        public string Renamed
        {
            get { return renameBox.Text; }
        }
    }
}
