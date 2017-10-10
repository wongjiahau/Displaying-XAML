using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Xml;
using Xavalon.XamlStyler.Core;
using Xavalon.XamlStyler.Core.Options;

namespace CodeDisplayer {
    public class XamlDisplayerPanel : StackPanel {
        public static bool IsControlPanelDisplayed = false;
        public SourceEnum Source { get; set; } = SourceEnum.Null;
        public string LocalPath { get; set; } = null;
        public string RemotePath { get; set; } = null;
        public string SourceFileName { get; set; } = null;

        public XamlDisplayerPanel() {
            Grid.SetIsSharedSizeScope(this , true);
            this.Loaded += XamlDisplayerPanel_Loaded;
        }

        private void XamlDisplayerPanel_Loaded(object sender , RoutedEventArgs e) {
            LoadXamlFile();
            this.Loaded -= XamlDisplayerPanel_Loaded;
        }

        private void LoadControlPanel() {
            if (!IsControlPanelDisplayed) return;
            var controlPanel = new ControlPanel() { DataContext = this };
            BindingOperations.SetBinding(this , IsCodeDisplayedProperty , new Binding() { Source = controlPanel.IsCodeDisplayedToggleButton , Path = new PropertyPath("IsChecked") });
            BindingOperations.SetBinding(this , DisplayModeProperty , new Binding() { Source = controlPanel.OrientationToggleButton , Path = new PropertyPath("IsChecked") , Converter = new BoolToDisplayModeConverter() });
            BindingOperations.SetBinding(this , SearchedTextProperty , new Binding() { Source = controlPanel.SearchBox , Path = new PropertyPath("Text") });
            this.Children.Insert(0 , controlPanel);
        }

        private async void LoadXamlFile() {
            var xmlDocument = new XmlDocument();
            CheckIfInitialized();
            LoadingScreen.Display("Loading source file : " + SourceFileName + " . . .");
            await Task.Run(() => {
                switch (_defaultSource) {
                    case SourceEnum.LoadFromRemote:
                        try { xmlDocument.LoadXml(Helper.DownloadFile(RemotePath + SourceFileName)); }
                        catch (Exception ex) { MessageBox.Show(ex.Message + "\nCannot load file from " + RemotePath + SourceFileName); } break;
                    case SourceEnum.LoadFromLocal:
                        try { xmlDocument.LoadXml(File.ReadAllText(LocalPath + SourceFileName));  }
                        catch (Exception ex) { MessageBox.Show(ex.Message + "\nCannot load file from " + LocalPath + SourceFileName); } break;  }
            });
            LoadingScreen.CloseDialog();
            WrapEachChildWithXamlDisplayer();
            DisplayXamlCode(xmlDocument);
            OnDisplayModePropertyChanged(this , new DependencyPropertyChangedEventArgs(DisplayModeProperty , null , this.DisplayMode));
            IsCodeDisplayedPropertyChanged(this , new DependencyPropertyChangedEventArgs(IsCodeDisplayedProperty , null , this.IsCodeDisplayed));
            LoadControlPanel();
        }

        private void CheckIfInitialized() {
            if (SourceFileName == null) {
                SourceFileName = SearchForSourceFileName();
            }
            if (Source == SourceEnum.Null) Source = _defaultSource;
            LocalPath = LocalPath ?? _defaultLocalPath;
            RemotePath = RemotePath ?? _defaultRemotePath;
        }

        private string SearchForSourceFileName() {
            return FindFisrtChildOfXamlDisplayerHost(this).GetType().Name + ".xaml";
            DependencyObject FindFisrtChildOfXamlDisplayerHost(DependencyObject child)
            {
                var parent = LogicalTreeHelper.GetParent(child);
                if (parent == null || parent.GetType() == typeof(XamlDisplayerHost)) return child;
                return FindFisrtChildOfXamlDisplayerHost(parent);

            }
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
                string RemoveIrrelaventAttributes(string input)
                {
                    if (_attributesToBeRemoved == null) return input;
                    string cleansed = input;
                    foreach (var s in _attributesToBeRemoved) {
                        cleansed = cleansed.Replace(s , "");
                    }
                    return cleansed;
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

        #region Initializer
        public enum SourceEnum { LoadFromRemote, LoadFromLocal, Null }
        private static SourceEnum _defaultSource;
        private static string _defaultLocalPath;
        private static string _defaultRemotePath;
        private static List<string> _attributesToBeRemoved = new List<string>();

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

        #region SearchedTextProperty
        public string SearchedText {
            get { return (string)GetValue(SearchedTextProperty); }
            set { SetValue(SearchedTextProperty , value); }
        }

        // Using a DependencyProperty as the backing store for SearchedText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SearchedTextProperty =
            DependencyProperty.Register("SearchedText" , typeof(string) , typeof(XamlDisplayerPanel) , new PropertyMetadata("" , OnSearchedTextPropertyChanged));

        private static void OnSearchedTextPropertyChanged(DependencyObject dependencyObject , DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs) {
            var xamlDisplayerPanel = (XamlDisplayerPanel)dependencyObject;
            string input = ((string)dependencyPropertyChangedEventArgs.NewValue).ToLower();
            var xamlDisplayers = xamlDisplayerPanel._xamlDisplayers;
            for (int i = 0 ; i < xamlDisplayers.Count ; i++) {
                xamlDisplayers[i].Visibility =
                    xamlDisplayers[i].CodeToBeDisplayed.ToLower().Contains(input)
                        ? Visibility.Visible
                        : Visibility.Collapsed;
            }
        }
        #endregion

        #endregion
    }
}
