using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SemPoliticsWpfDB.Converters
{
    public class ImageSourceLoader : IValueConverter
    {
        public static ImageSourceLoader Instance = new ImageSourceLoader();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string val = value.ToString().Replace('/', '\\').Substring(3);
            var path = string.Format((string)parameter, val);
            return System.Windows.Media.Imaging.BitmapFrame.Create(new Uri(path, UriKind.RelativeOrAbsolute));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
