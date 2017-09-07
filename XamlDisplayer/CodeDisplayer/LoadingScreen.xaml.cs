using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CodeDisplayer {
    /// <summary>
    /// Interaction logic for LoadingScreen.xaml
    /// </summary>
    public partial class LoadingScreen : Window {
        private static LoadingScreen _current;
        private LoadingScreen() {
            InitializeComponent();
        }

        public static void Display(string message) {
            _current = new LoadingScreen {TextBlock = {Text = message}};
            _current.Show();
        }

        public static void CloseDialog() {
            _current.Close();
        }
    }
}
