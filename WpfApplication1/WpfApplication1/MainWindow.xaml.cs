using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MaterialDesignThemes.Wpf;

namespace DisplayXamlDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Snackbar.MessageQueue = new SnackbarMessageQueue();
            ListView.ItemsSource = new List<Control_Definition>()
            {

                new Control_Definition(@"<Button Style='{StaticResource MaterialDesignRaisedLightButton}'>Raised-Light</Button>"),
                new Control_Definition(@"<Button Style='{StaticResource MaterialDesignRaisedButton}'>Raised</Button>"),
                new Control_Definition(@"<Button Style='{StaticResource MaterialDesignRaisedDarkButton}'>Raised-Dark</Button>"),
                new Control_Definition(@"<Button Style='{StaticResource MaterialDesignRaisedAccentButton}'>Raised-Accent</Button>"),
                new Control_Definition(@"<Button Style='{StaticResource MaterialDesignFlatButton}'>Flat</Button>"),
            };

        }

        private void CopyButtonEventSetter_OnHandler(object sender, RoutedEventArgs e) {
            var button = (sender as Button);
            if(button.Tag!=null)
            Clipboard.SetDataObject(button.Tag);                        
            Snackbar.MessageQueue.Enqueue(" Copied to clipboard");
        }
    }
}
