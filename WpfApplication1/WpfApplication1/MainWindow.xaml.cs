using System.IO;
using System.Windows;
using System.Xml;

//https://stackoverflow.com/questions/32690299/looping-through-all-nodes-in-xml-file-with-c-sharp\
//https://github.com/Xavalon/XamlStyler
namespace DisplayXamlDemo {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            string url =
                "https://raw.githubusercontent.com/wongjiahau/Displaying-XAML/master/WpfApplication1/WpfApplication1/MainWindow.xaml";
            string localSource = @"C:\Users\User\Source\Repos\Displaying-XAML\WpfApplication1\WpfApplication1\MainWindow.xaml";
            string xaml = File.ReadAllText(localSource);
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xaml);
            this.XamlDisplayerPanel.Initialize(xmlDoc);
            //XamlDisplayer.DisplayXamlCode(this, url);


        }


    }
}
