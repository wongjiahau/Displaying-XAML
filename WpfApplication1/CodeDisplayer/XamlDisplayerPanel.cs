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
        public SourceEnum Source = SourceEnum.Null;
        public string LocalPath = null;
        public string RemotePath = null;
        public string SourceFileName = null;

        public XamlDisplayerPanel() {
            if(SourceFileName == null) throw new Exception("SourceFileName must be defined. E.g. MainWindow.xaml");
            if (Source == SourceEnum.Null) Source = _defaultSource;
            LocalPath = LocalPath ?? _defaultLocalPath;
            RemotePath = RemotePath ?? _defaultRemotePath;

            Grid.SetIsSharedSizeScope(this , true);
            this.Loaded += LoadXamlFile;
        }

        private void LoadXamlFile(object sender , RoutedEventArgs e) {
            var xmlDocument = new XmlDocument();
            switch (_defaultSource) {
                case SourceEnum.Remote:
                    xmlDocument.LoadXml(Helper.DownloadFile(RemotePath + SourceFileName));
                    break;
                case SourceEnum.Local:
                    xmlDocument.LoadXml(File.ReadAllText(LocalPath + SourceFileName));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            WrapEachChildWithXamlDisplayer();
            DisplayXamlCode(xmlDocument);
            OnDisplayModePropertyChanged(this , new DependencyPropertyChangedEventArgs(DisplayModeProperty , null , this.DisplayMode));
            IsCodeDisplayedPropertyChanged(this , new DependencyPropertyChangedEventArgs(IsCodeDisplayedProperty , null , this.IsCodeDisplayed));
        }

        #region Initializer
        public enum SourceEnum { Remote, Local, Null }
        private static SourceEnum _defaultSource;
        private static string _defaultLocalPath;
        private static string _defaultRemotePath;
        private static List<string> _attributesToBeRemoved;

        /// <summary>
        /// This method configure how all XamlDisplayerPanel should load and display XAML code
        /// </summary>
        /// <param name="source">
        /// Where to load the XAML. Either from remote(e.g. GitHub) or local machine
        /// </param>
        /// <param name="defaultLocalPath">
        /// Where to load XAML file if source is defined as Local
        /// E.g. C:\Users\User\Source\Repos\MyProject\Folder1
        /// </param>
        /// <param name="defaultRemotePath">
        /// Where to load XAML file if source is defined as Remote
        /// E.g. https://raw.githubusercontent.com/User/Repo/Branch/Folder/
        /// </param>
        /// <param name="attributesToBeRemoved">
        /// List of attribues that you wish to hide from reader's sight
        /// E.g. xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\ 
        /// </param>
        public static void Initialize(SourceEnum source , string defaultLocalPath , string defaultRemotePath , List<string> attributesToBeRemoved) {
            _defaultSource = source;
            _defaultLocalPath = defaultLocalPath;
            _defaultRemotePath = defaultRemotePath;
            _attributesToBeRemoved = attributesToBeRemoved;
        }
        #endregion

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

        #region DependencyProperties
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
        #endregion
    }
}
