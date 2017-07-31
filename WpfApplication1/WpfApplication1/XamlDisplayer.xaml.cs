using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty , value); }
        }

        public new static readonly DependencyProperty ContentProperty = DependencyProperty.Register
 (nameof(Content) , typeof(object) , typeof(XamlDisplayer) ,
new FrameworkPropertyMetadata(default(object) , OnContentPropertyChanged));

        private static void OnContentPropertyChanged(DependencyObject d , DependencyPropertyChangedEventArgs e) {
            var xamlDisplayer = d as XamlDisplayer;
            var content = e.NewValue as Control;
            xamlDisplayer.ContentPresenter.Content = content;
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

        private string _codeToBeCopied;

        private static void OnXamlPropertyChanged(DependencyObject d , DependencyPropertyChangedEventArgs e) {
            var xamlDisplayer = d as XamlDisplayer;
            if (xamlDisplayer == null) return;
            xamlDisplayer.TextEditor.Text = (string)e.NewValue;
            xamlDisplayer._codeToBeCopied = (string)e.NewValue;
        }

        #endregion

        private void CopyButton_OnClicked(object sender , RoutedEventArgs e) {
            Clipboard.SetDataObject(_codeToBeCopied);
        }

        public static void DisplayXamlCode(Control host, string sourceCodeUrl) {
            string sourceCode = DownloadFile(sourceCodeUrl);
            var doc = new XmlDocument();
            doc.LoadXml(sourceCode);
            DisplayXamlCode(host, doc);
            
            string DownloadFile(string sourceURL) //https://gist.github.com/nboubakr/7812375
            {
                int bufferSize = 1024;
                bufferSize *= 1000;
                long existLen = 0;
                var httpReq = (HttpWebRequest)WebRequest.Create(sourceURL);
                httpReq.AddRange((int)existLen);
                var httpRes = (HttpWebResponse)httpReq.GetResponse();
                var responseStream = httpRes.GetResponseStream();
                if (responseStream == null) return "Fail to fetch file";
                int byteSize;
                byte[] downBuffer = new byte[bufferSize];
                var streamReader = new StreamReader(responseStream);
                return streamReader.ReadToEnd();
            }
        }

        private static void DisplayXamlCode(Control host , XmlNode node) {                    
            if (node.LocalName == "XamlDisplayer") {
                string xamlToBeDisplayed = Beautify(node.InnerXml);
                var nameAttribute = node.Attributes["x:Name"];
                if (nameAttribute == null) {
                    MessageBox.Show("Please specify the value of 'x:Name' for each XamlDisplayer");
                }
                else {
                    (host.FindName(nameAttribute.Value) as XamlDisplayer)
                        .Xaml = xamlToBeDisplayed;
                }
            }
            else if (node.HasChildNodes) {
                foreach (XmlNode child in node.ChildNodes) {
                    DisplayXamlCode(host , child);
                }
            }
            string Beautify(string fullXaml)
            {
                var styler = new StylerService(new StylerOptions() { IndentWithTabs = true });
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

                string RemoveEmptyLines(string xaml)
                {
                    var sb = new StringBuilder(xaml.Length);
                    char previousChar = '\0';
                    for (int i = 0 ; i < xaml.Length ; i++) {
                        char currentChar = xaml[i];
                        if (currentChar == '\r' && previousChar == '\n') {
                            //skip \r,\n,\t
                            while (i + 1 < xaml.Length &&
                                   (xaml[i + 1] == '\r' ||
                                   xaml[i + 1] == '\n' ||
                                   xaml[i + 1] == '\t')) {
                                i++;
                            }
                        }
                        else {
                            sb.Append(currentChar);
                        }
                        if (currentChar != ' ' && currentChar != '\t') previousChar = currentChar;
                    }
                    return sb.ToString();
                }
            }

        }


        
    }
}
