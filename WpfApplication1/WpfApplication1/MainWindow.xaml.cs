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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            string namespaces = " xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' ";
            string button =
                @"<Button Content='Hi' x:Name='button'></Button>";
            button = button.Insert(button.IndexOf(' '), namespaces);
            MemoryStream stream = new MemoryStream(ASCIIEncoding.UTF8.GetBytes(button));
            Button b = (Button)XamlReader.Load(stream);
            this.StackPanel.Children.Add(b);
            this.Label.Text = button.Replace(namespaces, "");
        }
    }
}
