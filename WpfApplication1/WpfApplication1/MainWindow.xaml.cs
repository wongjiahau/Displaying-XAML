using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;
using MaterialDesignThemes.Wpf;
using DisplayXamlDemo;
using Xavalon.XamlStyler.Core;
using Xavalon.XamlStyler.Core.Options;
using Path = System.IO.Path;
//https://stackoverflow.com/questions/32690299/looping-through-all-nodes-in-xml-file-with-c-sharp\
//https://github.com/Xavalon/XamlStyler
namespace DisplayXamlDemo {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {                            
            InitializeComponent();                        
            try {           
                string url =
                    "https://raw.githubusercontent.com/wongjiahau/Displaying-XAML/master/WpfApplication1/WpfApplication1/MainWindow.xaml";
                XamlDisplayer.DisplayXamlCode(this, url);
            }
            catch (Exception e) {
                MessageBox.Show(e.Message);
            }
        }


    }
}
