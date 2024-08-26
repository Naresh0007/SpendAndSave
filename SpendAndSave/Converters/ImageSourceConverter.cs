using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using System.Reflection;

namespace SpendAndSave.Converters
{
    public class ImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string entryType)
            {
                // Convert the entry type to lowercase and build the resource name
                string imageName = $"{entryType.ToLower()}";

                // Try to load the resource image
                var assembly = typeof(ImageSourceConverter).GetTypeInfo().Assembly;
                var resourcePath = $"SpendAndSave.Resources.drawable.{imageName}";

                try
                {
                    return ImageSource.FromResource(resourcePath, assembly);
                }
                catch
                {
                    // If the specific image is not found, return a default image
                    return ImageSource.FromResource("SpendAndSave.Resources.drawable.default", assembly);
                }
            }

            // Return the default image if value is not valid
            return ImageSource.FromResource("SpendAndSave.Resources.drawable.default", typeof(ImageSourceConverter).GetTypeInfo().Assembly);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Not used in this scenario
            throw new NotImplementedException();
        }
    }
}
