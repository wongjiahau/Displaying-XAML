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
using System.Xml;
using Xavalon.XamlStyler.Core;
using Xavalon.XamlStyler.Core.Options;


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

        public static void HandleAllNode(Control host , XmlNode node) {
            if (node.LocalName == "XamlDisplayer") {
                (host.FindName(node.Attributes["x:Name"].Value) as XamlDisplayer)
                    .Xaml =
                    Beautify(node.InnerXml);

            }
            else if (node.HasChildNodes) {
                foreach (XmlNode child in node.ChildNodes) {
                    HandleAllNode(host , child);
                }
            }


        }

        private static string Beautify(string fullXaml) {
            var styler = new StylerService(new StylerOptions());
            string result = styler.StyleDocument(fullXaml);
            result = RemoveIrrelaventAttributes(result);
            result = RemoveEmptyLines(result);       
            return result;
            string RemoveIrrelaventAttributes(string xaml)
            {
                return xaml
                        .Replace("xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"" , "")
                        .Replace("xmlns:materialDesign=\"http://materialdesigninxaml.net/winfx/xaml/themes\"" , "")
                    ;
            }

            string RemoveEmptyLines(string xaml) {
                var sb = new StringBuilder(xaml.Length);
                char previousChar = '\0';
                for (int i = 0; i < xaml.Length; i++) {
                    char currentChar = xaml[i];
                    if (currentChar == '\r' && previousChar == '\n') {
                        i += 2; //skip /r/n
                        while(i+1 < xaml.Length && xaml[i+1]==' ')
                        i ++; //skip the extra linefeed and spacing                        
                    }
                    else {
                        sb.Append(currentChar);
                    }
                    if (currentChar!=' ') previousChar = currentChar;
                }
                return sb.ToString();
            }
        }
    }
}
