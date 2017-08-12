using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using Xavalon.XamlStyler.Core;
using Xavalon.XamlStyler.Core.Options;

namespace CodeDisplayer {
    public class XamlDisplayerPanel : StackPanel {
        public XamlDisplayerPanel() {
            Grid.SetIsSharedSizeScope(this , true);
        }
        




        public void Initialize(XmlDocument xmlDocument) {
            WrapEachChildWithXamlDisplayer();
            DisplayXamlCode(xmlDocument);
            OnDisplayModePropertyChanged(this , new DependencyPropertyChangedEventArgs(DisplayModeProperty , null , this.DisplayMode));
            IsCodeDisplayedPropertyChanged(this , new DependencyPropertyChangedEventArgs(IsCodeDisplayedProperty , null , this.IsCodeDisplayed));
        }

        private void DisplayXamlCode(XmlNode node) {
            if (node.LocalName.Contains(nameof(XamlDisplayerPanel))) {
                for (var i = 0 ; i < node.ChildNodes.Count ; i++) {
                    XmlNode child = node.ChildNodes[i];
                    string xamlToBeDisplayed = Beautify(child.OuterXml);
                    _xamlDisplayers[i].CodeToBeDisplayed = xamlToBeDisplayed;
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

        private List<XamlDisplayer> _xamlDisplayers;
        private void WrapEachChildWithXamlDisplayer() {
            _xamlDisplayers = new List<XamlDisplayer>();
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
                _xamlDisplayers.Add(xamlDisplayer);
            }
        }

        #region  IsCodeDisplayedProperty
        public bool IsCodeDisplayed {
            get { return (bool)GetValue(IsCodeDisplayedProperty); }
            set { SetValue(IsCodeDisplayedProperty , value); }
        }

        // Using a DependencyProperty as the backing store for IsCodeDisplayed.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCodeDisplayedProperty =
            DependencyProperty.Register("IsCodeDisplayed" , typeof(bool) , typeof(XamlDisplayerPanel) , new PropertyMetadata(true , IsCodeDisplayedPropertyChanged));

        private static void IsCodeDisplayedPropertyChanged(DependencyObject dependencyObject , DependencyPropertyChangedEventArgs e) {
            var d = dependencyObject as XamlDisplayerPanel;
            bool newValue = (bool)e.NewValue;
            if (d == null) return;
            foreach (var child in d.Children) {
                var xamlDisplayer = child as XamlDisplayer;
                if (xamlDisplayer == null) continue;
                xamlDisplayer.IsCodeDisplayed = newValue;
            }
        }

        #endregion

        #region DisplayModeProperty


        public XamlDisplayer.DisplayModeEnum DisplayMode {
            get { return (XamlDisplayer.DisplayModeEnum)GetValue(DisplayModeProperty); }
            set { SetValue(DisplayModeProperty , value); }
        }

        // Using a DependencyProperty as the backing store for DisplayMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayModeProperty =
            DependencyProperty.Register("DisplayMode" , typeof(XamlDisplayer.DisplayModeEnum) , typeof(XamlDisplayerPanel) , new PropertyMetadata(XamlDisplayer.DisplayModeEnum.LeftRight , OnDisplayModePropertyChanged));

        private static void OnDisplayModePropertyChanged(DependencyObject dependencyObject , DependencyPropertyChangedEventArgs e) {
            var d = dependencyObject as XamlDisplayerPanel;
            var newValue = (XamlDisplayer.DisplayModeEnum)e.NewValue;
            if (d == null) return;
            foreach (var child in d.Children) {
                var xamlDisplayer = child as XamlDisplayer;
                if (xamlDisplayer == null) continue;
                xamlDisplayer.DisplayMode = newValue;
            }
        }
        #endregion
    }

}
