using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;
using CodeDisplayer;

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
                    XamlDisplayerPanel.SourceEnum.LoadFromRemote ,
                defaultLocalPath: 
                    @"C:\Users\User\Source\Repos\Displaying-XAML\WpfApplication1\WpfApplication1\" ,
                defaultRemotePath: 
                    "https://raw.githubusercontent.com/wongjiahau/Displaying-XAML/master/WpfApplication1/WpfApplication1/" ,
                attributesToBeRemoved: 
                    new List<string>()
                    {
                        "xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"" ,
                        "xmlns:materialDesign=\"http://materialdesigninxaml.net/winfx/xaml/themes\"" ,
                        "xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\""  
                    }
                );

            MessageBox.Show(GetHost(XamlDisplayerPanel).GetType().Name);

        }

        public static DependencyObject GetHost(DependencyObject child) {
            var parent = LogicalTreeHelper.GetParent(child);
            if (parent == null) return child;
            return GetHost(parent);
        }

    }
}
