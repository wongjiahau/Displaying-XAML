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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;



namespace WpfApplication1 {
    /// <summary>
    /// Interaction logic for XamlDisplayer.xaml
    /// </summary>
    public partial class XamlDisplayer : UserControl {
        public XamlDisplayer() {
            InitializeComponent();
        }

        #region  ContentProperty
        public new object Content {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty , value); }
        }

        public new static readonly DependencyProperty ContentProperty = DependencyProperty.Register
 (nameof(Content) , typeof(object) , typeof(XamlDisplayer) ,
new FrameworkPropertyMetadata(default(object) , OnContentPropertyChanged));

        private static void OnContentPropertyChanged(DependencyObject d , DependencyPropertyChangedEventArgs e) {
            var xamlDisplayer = d as XamlDisplayer;
            var content = e.NewValue as Control;
            if (content != null) {
                content.Initialized += (sender , args) => {
                    string xamlToBeDisplayed = Beautify(XamlWriter.Save(sender));
                    if (xamlDisplayer != null) {
                        xamlDisplayer.TextEditor.Text = xamlToBeDisplayed;
                        xamlDisplayer.ContentPresenter.Content = sender;
                    }
                };
            }

        }

        private static void Content_Loaded(object sender , RoutedEventArgs e) {
        }

        private static string Beautify(string fullXaml) {
            return fullXaml;
        }

        #endregion


        #region TextProperty


        public string Xaml {
            get { return (string)GetValue(XamlProperty); }
            set { SetValue(XamlProperty , value); }
        }

        // Using a DependencyProperty as the backing store for Xaml.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty XamlProperty =
            DependencyProperty.Register("Xaml" , typeof(string) , typeof(XamlDisplayer) , new PropertyMetadata("" , OnXamlPropertyChanged));

        private static void OnXamlPropertyChanged(DependencyObject d , DependencyPropertyChangedEventArgs e) {
            (d as XamlDisplayer).TextEditor.Text = (string)e.NewValue;
        }

        #endregion
        private void CopyButton_OnClicked(object sender , RoutedEventArgs e) {
            string toBeCopied = (string)(sender as Button).Tag;
            Clipboard.SetDataObject(toBeCopied);
        }
    }
}
