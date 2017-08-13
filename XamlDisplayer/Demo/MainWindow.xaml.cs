using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;
using CodeDisplayer;
using System.Windows.Data;

//https://stackoverflow.com/questions/32690299/looping-through-all-nodes-in-xml-file-with-c-sharp\
//https://github.com/Xavalon/XamlStyler
namespace DisplayXamlDemo {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            CodeDisplayer.XamlDisplayerPanel.Initialize(
                source:
                    XamlDisplayerPanel.SourceEnum.LoadFromLocal,
                defaultLocalPath:
                    @"C:\Users\User\Source\Repos\Displaying-XAML\XamlDisplayer\Demo\" ,
                defaultRemotePath:
                    "https://raw.githubusercontent.com/wongjiahau/Displaying-XAML/master/XamlDisplayer/Demo/" ,
                attributesToBeRemoved:
                    new List<string>()
                    {
                        "xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"" ,
                        "xmlns:materialDesign=\"http://materialdesigninxaml.net/winfx/xaml/themes\"" ,
                        "xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\""
                    }
                );
        }
    }

    public class BoolToDisplayModeConverter : IValueConverter{
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var input = (bool) value;
            if (input) {
                return XamlDisplayer.DisplayModeEnum.TopBottom;
            }
            else {
                return XamlDisplayer.DisplayModeEnum.LeftRight;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
