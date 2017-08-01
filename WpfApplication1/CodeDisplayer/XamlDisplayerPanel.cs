using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;
using Xavalon.XamlStyler.Core;
using Xavalon.XamlStyler.Core.Options;

namespace DisplayXamlDemo {
    public class XamlDisplayerPanel : StackPanel {
        public XamlDisplayerPanel() {
            Grid.SetIsSharedSizeScope(this , true);
        }
        
        public static string DownloadFile(string sourceURL) //https://gist.github.com/nboubakr/7812375
           {
            long existLen = 0;
            var httpReq = (HttpWebRequest)WebRequest.Create(sourceURL);
            httpReq.AddRange((int)existLen);
            var httpRes = (HttpWebResponse)httpReq.GetResponse();
            var responseStream = httpRes.GetResponseStream();
            if (responseStream == null) return "Fail to fetch file";
            var streamReader = new StreamReader(responseStream);
            return streamReader.ReadToEnd();
        }

        public void Initialize(XmlDocument xmlDocument) {
            WrapEachChildWithXamlDisplayer();
            DisplayXamlCode(xmlDocument);
        }

        private void DisplayXamlCode(XmlNode node) {
            if (node.LocalName.Contains(nameof(XamlDisplayerPanel))) {
                foreach (XmlNode child in node.ChildNodes) {
                    var nameAttribute = child.Attributes["x:Name"];
                    if (nameAttribute == null) {
                        MessageBox.Show("Please specify the value of 'x:Name' for each element in XamlDisplayerPanel");
                    }
                    else {
                        var xamlDisplayer = _xamlDisplayerDic[nameAttribute.Value];
                        child.Attributes.Remove(nameAttribute);
                        string xamlToBeDisplayed = Beautify(child.OuterXml);
                        if (xamlDisplayer != null) xamlDisplayer.CodeToBeDisplayed = xamlToBeDisplayed;
                    }
                }
            }
            else if (node.HasChildNodes) {
                foreach (XmlNode child in node.ChildNodes) {
                    DisplayXamlCode(child);
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
                        .Replace("xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"" , "");
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

        private Dictionary<string , XamlDisplayer> _xamlDisplayerDic;
        private void WrapEachChildWithXamlDisplayer() {
            _xamlDisplayerDic = new Dictionary<string , XamlDisplayer>();
            var newChildren = new List<UIElement>();
            while (this.Children.Count > 0) {
                var child = Children[0];
                this.Children.Remove(child);
                newChildren.Add(child);
            }
            foreach (var child in newChildren) {
                var xamlDisplayer = new XamlDisplayer() {
                    Content = child
                };
                this.Children.Add(xamlDisplayer);
                _xamlDisplayerDic.Add(GetName(child) , xamlDisplayer);
            }

            string GetName(object obj)
            {
                // First see if it is a FrameworkElement
                var element = obj as FrameworkElement;
                if (element != null)
                    return element.Name;
                // If not, try reflection to get the value of a Name property.
                try { return (string)obj.GetType().GetProperty("Name").GetValue(obj , null); }
                catch {
                    // Last of all, try reflection to get the value of a Name field.
                    try { return (string)obj.GetType().GetField("Name").GetValue(obj); }
                    catch { return null; }
                }
            }

        }

        #region  IsCodeDisplayedProperty
        public bool IsCodeDisplayed {
            get { return (bool)GetValue(IsCodeDisplayedProperty); }
            set { SetValue(IsCodeDisplayedProperty , value); }
        }

        // Using a DependencyProperty as the backing store for IsCodeDisplayed.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCodeDisplayedProperty =
            DependencyProperty.Register("IsCodeDisplayed" , typeof(bool) , typeof(XamlDisplayerPanel) , new PropertyMetadata(true, IsCodeDisplayedPropertyChanged));

        private static void IsCodeDisplayedPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e) {
            var d = dependencyObject as XamlDisplayerPanel;
            bool newValue = (bool) e.NewValue;
            if (d == null) return;
            foreach (var child in d.Children) {
                var xamlDisplayer = child as XamlDisplayer;
                if (xamlDisplayer == null) continue;
                xamlDisplayer.IsCodeDisplayed = newValue;
            }
        }

        #endregion
    }

}
