using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using WpfApplication1;
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
            Snackbar.MessageQueue = new SnackbarMessageQueue();            
            var xdoc = new XmlDocument();
            xdoc.Load(@"C:\Users\User\Source\Repos\Displaying-XAML\WpfApplication1\WpfApplication1\MainWindow.xaml");
            XamlDisplayer.HandleAllNode(this, xdoc);                  
        }

        

        private void CopyButtonEventSetter_OnHandler(object sender , RoutedEventArgs e) {
            var button = (sender as Button);
            if (button.Tag != null)
                Clipboard.SetDataObject(button.Tag);
            Snackbar.MessageQueue.Enqueue(" Copied to clipboard");
        }
    }
}
