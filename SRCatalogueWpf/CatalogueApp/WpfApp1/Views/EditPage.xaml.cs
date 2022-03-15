using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using WPF.JoshSmith.ServiceProviders.UI;
using WpfApp1.Models;
using WpfApp1.ViewModels;
using Image = WpfApp1.Models.Image;

namespace WpfApp1.Views
{
    /// <summary>
    /// Логика взаимодействия для EditPage.xaml
    /// </summary>
    public partial class EditPage : Page
    {
        private DescriptionText DescText;

        ListViewDragDropManager<Image> dragMgr;
        public EditPage()
        {
            InitializeComponent();

            cmbFontFamily.ItemsSource = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            cmbFontSize.ItemsSource = new List<double>() { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 };

            this.dragMgr = new ListViewDragDropManager<Image>(this.listView);
            this.dragMgr.ShowDragAdorner = true;
            this.dragMgr.DragAdornerOpacity = 0.5;
           this.dragMgr.ProcessDrop += dragMgr_ProcessDrop;

           
        }

        void dragMgr_ProcessDrop(object sender, ProcessDropEventArgs<Image> e)
        {
            // This shows how to customize the behavior of a drop.
            // Here we perform a swap, instead of just moving the dropped item.


             int higherIdx = Math.Max(e.OldIndex, e.NewIndex);
            int lowerIdx = Math.Min(e.OldIndex, e.NewIndex);

           
                // null values will cause an error when calling Move.
                // It looks like a bug in ObservableCollection to me.
                if (e.ItemsSource[lowerIdx] == null ||
                    e.ItemsSource[higherIdx] == null)
                    return;

            string lowid = e.ItemsSource[lowerIdx].id;
            string highid = e.ItemsSource[higherIdx].id;

            e.ItemsSource[lowerIdx].id = highid; //изменение в классе
            e.ItemsSource[higherIdx].id = lowid;

            e.ItemsSource[lowerIdx].ChangeOrder(lowid, highid); // изменение в базе


                // The item came from the ListView into which
                // it was dropped, so swap it with the item
                // at the target index.
            e.ItemsSource.Move(lowerIdx, higherIdx);
                e.ItemsSource.Move(higherIdx - 1, lowerIdx);

           
          
        }

      

        private void cmbFontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbFontFamily.SelectedItem != null)
                rtb.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, cmbFontFamily.SelectedItem);
        }
        private void cmbFontSize_TextChanged(object sender, TextChangedEventArgs e)
        {

            rtb.Selection.ApplyPropertyValue(Inline.FontSizeProperty, cmbFontSize.Text);

        }

        private void rtbEditor_SelectionChanged(object sender, RoutedEventArgs e)
        {
            TextRange selection = null;
            if (rtb.Selection != null)
            {
                selection = new TextRange(rtb.Selection.Start, rtb.Selection.End);
            }
            if (selection != null)
            {
                object temp = selection.GetPropertyValue(Inline.FontWeightProperty);
                lbBold.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontWeights.Bold));
                temp = selection.GetPropertyValue(Inline.FontStyleProperty);
                lbItalic.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontStyles.Italic));
                temp = selection.GetPropertyValue(Inline.TextDecorationsProperty);
                lbUnderline.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(TextDecorations.Underline));

                temp = selection.GetPropertyValue(Inline.FontFamilyProperty);
                if (temp != DependencyProperty.UnsetValue)
                {
                    cmbFontFamily.SelectedItem = temp;
                }

                temp = selection.GetPropertyValue(Inline.FontSizeProperty);
                if (temp != DependencyProperty.UnsetValue)
                {
                    cmbFontSize.Text = temp.ToString();
                }

                temp = selection.GetPropertyValue(Paragraph.TextAlignmentProperty);
                AlLeft.IsChecked = (temp.Equals(TextAlignment.Left)) && (temp != DependencyProperty.UnsetValue);
                AlCenter.IsChecked = (temp.Equals(TextAlignment.Center)) && (temp != DependencyProperty.UnsetValue);
                AlRight.IsChecked = (temp.Equals(TextAlignment.Right)) && (temp != DependencyProperty.UnsetValue);
                AlJustify.IsChecked = (temp.Equals(TextAlignment.Justify)) && (temp != DependencyProperty.UnsetValue);

                if (AlLeft.IsChecked == true) { AlIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.FormatAlignLeft; }
                if (AlCenter.IsChecked == true) { AlIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.FormatAlignCenter; }
                if (AlRight.IsChecked == true) { AlIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.FormatAlignRight; }
                if (AlJustify.IsChecked == true) { AlIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.FormatAlignJustify; }
            }
        }

        private void lbBold_Click(object sender, RoutedEventArgs e)
        {
            TextRange selection = new TextRange(rtb.Selection.Start, rtb.Selection.End);

            if (selection != null)
            {
                object tdc = rtb.Selection.GetPropertyValue(Inline.FontWeightProperty);
                if (tdc == null || !tdc.Equals(FontWeights.Bold))
                {
                    tdc = FontWeights.Bold;
                }
                else
                {
                    tdc = FontWeights.Normal;
                }
                selection.ApplyPropertyValue(Inline.FontWeightProperty, tdc);
            }
        }

        private void lbItalic_Click(object sender, RoutedEventArgs e)
        {
            TextRange selection = new TextRange(rtb.Selection.Start, rtb.Selection.End);

            if (selection != null)
            {
                object fsp = rtb.Selection.GetPropertyValue(Inline.FontStyleProperty);
                if (fsp == null || !fsp.Equals(FontStyles.Italic))
                {
                    fsp = FontStyles.Italic;
                }
                else
                {
                    fsp = FontStyles.Normal;
                }
                selection.ApplyPropertyValue(Inline.FontStyleProperty, fsp);
            }
        }

        private void lbUnderline_Click(object sender, RoutedEventArgs e)
        {
            TextRange selection = new TextRange(rtb.Selection.Start, rtb.Selection.End);

            if (selection != null)
            {
                TextDecorationCollection fsp = (TextDecorationCollection)rtb.Selection.GetPropertyValue(Inline.TextDecorationsProperty);

                if (fsp.Contains(TextDecorations.Underline[0]))
                {
                    TextDecorationCollection noUnder = new TextDecorationCollection(fsp);
                    noUnder.Remove(TextDecorations.Underline[0]);  //this is a bool, and could replace Contains above
                    selection.ApplyPropertyValue(Inline.TextDecorationsProperty, noUnder);
                }
                else
                {

                    fsp = TextDecorations.Underline;
                    selection.ApplyPropertyValue(Inline.TextDecorationsProperty, fsp);
                }                
                
            }

        }



        private void AlJustify_Click(object sender, RoutedEventArgs e)
        {
            if (rtb.Selection != null)
            {
                rtb.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Justify);
                AlCenter.IsChecked = false;
                AlLeft.IsChecked = false;
                AlRight.IsChecked = false;
            }
        }

        private void AlRight_Click(object sender, RoutedEventArgs e)
        {
            if (rtb.Selection != null)
            {
                rtb.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Right);
                AlCenter.IsChecked = false;
                AlLeft.IsChecked = false;
                AlJustify.IsChecked = false;
            }
        }

        private void AlCenter_Click(object sender, RoutedEventArgs e)
        {
            if (rtb.Selection != null)
            {
                rtb.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Center);
                AlRight.IsChecked = false;
                AlLeft.IsChecked = false;
                AlJustify.IsChecked = false;
            }
        }

        private void AlLeft_Click(object sender, RoutedEventArgs e)
        {
            if (rtb.Selection != null)
            {
                rtb.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Left);
                AlCenter.IsChecked = false;
                AlRight.IsChecked = false;
                AlJustify.IsChecked = false;
            }
        }

        public void Save()
        {
            EditWindowViewModel ew = this.DataContext as EditWindowViewModel;
            DescText = ew.descText;

            DescText.SaveChanges(rtb);

        }
        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Save();
        }



        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Rich Text Format (*.rtf)|*.rtf|All files (*.*)|*.*";
            if (dlg.ShowDialog() == true)
            {
                try { 
                    FileStream fileStream = new FileStream(dlg.FileName, FileMode.Open);
                    TextRange range = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
                    range.Load(fileStream, DataFormats.Rtf);
                }catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {

            Save();
            
            EditWindowViewModel edvm = this.DataContext as EditWindowViewModel;
            ContentPage prevP = new ContentPage(edvm.SelectedNode);
            this.NavigationService.Navigate(prevP);
            this.NavigationService.RemoveBackEntry();           
                                   
        }

        private void DeleteProjectClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult res = System.Windows.MessageBox.Show("Вы действительно хотите удалить все материалы данного проекта?", "Подтверждение", MessageBoxButton.OKCancel);
            if (res == MessageBoxResult.OK)
            {
                EditWindowViewModel edvm = this.DataContext as EditWindowViewModel;
                edvm.DeleteProject();
                ProjectsListPage pr = new ProjectsListPage(edvm.database);
                this.NavigationService.Navigate(pr);
                this.NavigationService.RemoveBackEntry();
            }

        }

        //private void ForecolorButton_Click(object sender, RoutedEventArgs e)
        //{
        //    ColorPicker cp = new ColorPicker();
        //    if (cp.ShowDialog() == true)
        //    {

        //        if (rtb.Selection != null)
        //        {
        //            rtb.Selection.ApplyPropertyValue(Inline.ForegroundProperty, new SolidColorBrush(cp.SelectedColor));
        //        }
        //    }
        //}

        //private void Background_Click(object sender, RoutedEventArgs e)
        //{
        //    ColorPicker cp = new ColorPicker();
        //    if (cp.ShowDialog() == true)
        //    {

        //        if (rtb.Selection != null)
        //        {
        //            Color col = cp.SelectedColor;
        //            SolidColorBrush br = new SolidColorBrush(col);
        //            rtb.Selection.ApplyPropertyValue(FlowDocument.BackgroundProperty, br);
        //        }
        //    }
        //}

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
           
        }

        private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (rtb.Selection != null)
            {
                rtb.Selection.ApplyPropertyValue(Inline.ForegroundProperty, new SolidColorBrush(foregroundColor.SelectedColor.Value));
            }
        }

        private void backgroundColor_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (rtb.Selection != null)
            {                
                rtb.Selection.ApplyPropertyValue(FlowDocument.BackgroundProperty, new SolidColorBrush(backgroundColor.SelectedColor.Value));
            }
        }
    }
}
