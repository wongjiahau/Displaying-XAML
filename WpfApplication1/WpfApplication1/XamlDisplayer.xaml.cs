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


namespace DisplayXamlDemo {
    /// <summary>
    /// This class is to display the Xaml of its child
    /// </summary>
    public partial class XamlDisplayer : UserControl {
        public XamlDisplayer() {
            InitializeComponent();
            
        }
        #region  ContentProperty
        public new object Content {
            get => GetValue(ContentProperty);
            set => SetValue(ContentProperty , value);
        }
        public new static readonly DependencyProperty ContentProperty = DependencyProperty.Register
 (nameof(Content) , typeof(object) , typeof(XamlDisplayer) ,
new FrameworkPropertyMetadata(default(object) , OnContentPropertyChanged));
        private static void OnContentPropertyChanged(DependencyObject d , DependencyPropertyChangedEventArgs e) {
            var xamlDisplayer = d as XamlDisplayer;
            var content = e.NewValue as Control;
            if (xamlDisplayer != null) xamlDisplayer.ContentPresenter.Content = content;
        }
        #endregion

        #region XamlProperty
        public string Xaml {
            get => (string)GetValue(XamlProperty);
            set => SetValue(XamlProperty , value);
        }        
        public static readonly DependencyProperty XamlProperty =
            DependencyProperty.Register("Xaml" , typeof(string) , typeof(XamlDisplayer) , new PropertyMetadata("" , OnXamlPropertyChanged));        
        private static void OnXamlPropertyChanged(DependencyObject d , DependencyPropertyChangedEventArgs e) {
            var xamlDisplayer = d as XamlDisplayer;
            if (xamlDisplayer == null) return;
            xamlDisplayer.TextEditor.Text = (string)e.NewValue;
            xamlDisplayer._codeToBeCopied = (string)e.NewValue;
        }
        #endregion

        private string _codeToBeCopied;
        private void CopyButton_OnClicked(object sender , RoutedEventArgs e) {
            Clipboard.SetDataObject(_codeToBeCopied);
        }
    }
}
