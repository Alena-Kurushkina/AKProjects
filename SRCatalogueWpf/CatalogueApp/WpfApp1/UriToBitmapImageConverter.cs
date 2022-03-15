using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media.Imaging;

namespace WpfApp1
{
    class UriToBitmapImageConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var path = value as string;
            if (path != null)
            {
                if (File.Exists(path))
                {
                    var uri = new Uri(path, UriKind.Relative);
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = uri;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                   // int p = bitmap.PixelWidth;
                    return bitmap;
                }
                else
                {
                   // File.WriteAllLines(@"data\Pictures.txt",
                   //             File.ReadLines(@"data\Pictures.txt").Where(l => l.Split('\t')[1] != path.Split('/').Last()).ToList());

                    return DependencyProperty.UnsetValue;
                }
            }
            else {

                // return DependencyProperty.UnsetValue;
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
