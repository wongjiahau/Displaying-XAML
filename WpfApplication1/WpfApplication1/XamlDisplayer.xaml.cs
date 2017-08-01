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

        #region CodeToBeDisplayedProperty
        public string CodeToBeDisplayed {
            get => (string)GetValue(CodeToBeDisplayedProperty);
            set => SetValue(CodeToBeDisplayedProperty , value);
        }
        public static readonly DependencyProperty CodeToBeDisplayedProperty =
            DependencyProperty.Register("CodeToBeDisplayed" , typeof(string) , typeof(XamlDisplayer) , new PropertyMetadata("" , OnXamlPropertyChanged));
        private static void OnXamlPropertyChanged(DependencyObject d , DependencyPropertyChangedEventArgs e) {
            var xamlDisplayer = d as XamlDisplayer;
            if (xamlDisplayer == null) return;
            xamlDisplayer.TextEditor.Text = (string)e.NewValue;
            xamlDisplayer._codeToBeCopied = (string)e.NewValue;
        }
        #endregion

        #region IsCodeDisplayed


        public bool IsCodeDisplayed {
            get { return (bool)GetValue(IsCodeDisplayedProperty); }
            set { SetValue(IsCodeDisplayedProperty , value); }
        }

        // Using a DependencyProperty as the backing store for IsCodeDisplayed.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCodeDisplayedProperty =
            DependencyProperty.Register("IsCodeDisplayed" , typeof(bool) , typeof(XamlDisplayer) , new PropertyMetadata(true , OnIsCodeDisplayedPropertyChanged));

        private static void OnIsCodeDisplayedPropertyChanged(DependencyObject d , DependencyPropertyChangedEventArgs e) {
            var xamlDisplayer = d as XamlDisplayer;
            if (xamlDisplayer == null) return;
            if ((bool) e.NewValue) {
                xamlDisplayer.ExpandCodeDisplaingArea();
            }
            else {
                xamlDisplayer.CollapseCodeDisplayingArea();
            }                        
        }

        private void CollapseCodeDisplayingArea() {
            var col1 = this.Grid.ColumnDefinitions[1];
            var col2 = this.Grid.ColumnDefinitions[2];
            if (col1.ActualWidth == 0 && col2.ActualWidth == 0) return;
            col1.Width = new GridLength(0);
            col2.Width = new GridLength(0);
        }

        private void ExpandCodeDisplaingArea() {
            var col1 = this.Grid.ColumnDefinitions[1];
            var col2 = this.Grid.ColumnDefinitions[2];
            if (col1.ActualWidth > 0 && col2.ActualWidth > 0) return;
            col1.Width = new GridLength(0, GridUnitType.Auto);            
            col2.Width = new GridLength(0, GridUnitType.Auto);
        }
        #endregion

        private string _codeToBeCopied;
        private void CopyButton_OnClicked(object sender , RoutedEventArgs e) {
            Clipboard.SetDataObject(_codeToBeCopied);
        }


    }
}
