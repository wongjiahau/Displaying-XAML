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
using Path = System.IO.Path;

namespace DisplayXamlDemo {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            Snackbar.MessageQueue = new SnackbarMessageQueue();
            ListView.ItemsSource = new List<Control_Definition>()
            {

                new Control_Definition(
@"<Button Style='{StaticResource MaterialDesignRaisedLightButton}'>LIGHT</Button>"),
                new Control_Definition(
@"<Button Style='{StaticResource MaterialDesignRaisedButton}'>MEDIUM</Button>"),
                new Control_Definition(
@"<Button Style='{StaticResource MaterialDesignRaisedDarkButton}'>DARK</Button>"),
                new Control_Definition(
@"<Button Style='{StaticResource MaterialDesignRaisedAccentButton}'>ACCENT</Button>"),
                new Control_Definition(
@"<Button Style='{StaticResource MaterialDesignFlatButton}'>FLAT</Button>"),
                new Control_Definition(
@"<Button Style='{StaticResource MaterialDesignFloatingActionMiniLightButton}'>
    <materialDesign:PackIcon Kind='Alarm' Height='24' Width='24' />
</Button>"),
                new Control_Definition(
@"<Button Style='{StaticResource MaterialDesignFloatingActionMiniButton}'>
    <materialDesign:PackIcon Kind='Alarm' Height='24' Width='24' />
</Button>"),
                new Control_Definition(
@"<Button Style='{StaticResource MaterialDesignFloatingActionMiniDarkButton}'>
    <materialDesign:PackIcon Kind='Alarm' Height='24' Width='24' />
</Button>"),
                new Control_Definition(
@"<Button Style='{StaticResource MaterialDesignFloatingActionLightButton}'>
    <materialDesign:PackIcon Kind='Alarm' Height='24' Width='24' />
</Button>"),
                new Control_Definition(
@"<Button Style='{StaticResource MaterialDesignFloatingActionButton}'>
    <materialDesign:PackIcon Kind='Alarm' Height='24' Width='24' />
</Button>"),
                new Control_Definition(
@"<Button Style='{StaticResource MaterialDesignFloatingActionDarkButton}'>
    <materialDesign:PackIcon Kind='Alarm' Height='24' Width='24' />
</Button>"),
                new Control_Definition(
@"<materialDesign:PopupBox  Style='{StaticResource MaterialDesignMultiFloatingActionPopupBox}'
    PlacementMode='BottomAndAlignCentres'>
    <StackPanel>
        <Button Opacity='0.5'>1</Button>
        <Button>2</Button>
        <Button>3</Button>
     </StackPanel>
</materialDesign:PopupBox>"),
                new Control_Definition(
@"<materialDesign:Badged Badge='3'>
    <Button>MAIL</Button>
</materialDesign:Badged>")
            };                        
            var xdoc = new XmlDocument();
            xdoc.Load(@"C:\Users\User\Source\Repos\Displaying-XAML\WpfApplication1\WpfApplication1\MainWindow.xaml");
            foreach (XmlNode node in xdoc.ChildNodes) {
                HandleNode(node);
            }         
        }

        private  void HandleNode(XmlNode node) {
            if (node.LocalName == "XamlDisplayer") {
                (this.FindName(node.Attributes["x:Name"].Value) as XamlDisplayer)
                    .Xaml = node.InnerXml;                
            }
            else if (node.HasChildNodes) {
                foreach (XmlNode child in node.ChildNodes) {
                    HandleNode(child);
                }
            }                            
        }

        private void CopyButtonEventSetter_OnHandler(object sender , RoutedEventArgs e) {
            var button = (sender as Button);
            if (button.Tag != null)
                Clipboard.SetDataObject(button.Tag);
            Snackbar.MessageQueue.Enqueue(" Copied to clipboard");
        }
    }
}
