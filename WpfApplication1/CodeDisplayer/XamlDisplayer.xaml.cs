using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;

namespace CodeDisplayer {
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
            var content = e.NewValue as FrameworkElement;
            if (xamlDisplayer==null || content == null) return;            
            xamlDisplayer.ContentPresenter.Content = content;
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
        }
        #endregion

        #region IsCodeDisplayedProperty


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
            if ((bool)e.NewValue) {
                xamlDisplayer.ExpandCodeDisplaingArea();
            }
            else {
                xamlDisplayer.CollapseCodeDisplayingArea();
            }
        }

        private void CollapseCodeDisplayingArea() {
            var row = this.Grid.RowDefinitions[1];
            row.Height = new GridLength(0);
            var col = this.Grid.ColumnDefinitions[1];
            col.Width = new GridLength(0);
        }

        private void ExpandCodeDisplaingArea() {
            var row = this.Grid.RowDefinitions[1];
            if (row.ActualHeight > 0) return;
            row.Height = new GridLength(0 , GridUnitType.Auto);
            var col = this.Grid.ColumnDefinitions[1];
            if (col.ActualWidth > 0) return;
            col.Width = new GridLength(0 , GridUnitType.Auto);
        }
        #endregion

        #region DisplayModeProperty
        public enum DisplayModeEnum { LeftRight, TopBottom }


        public DisplayModeEnum DisplayMode {
            get { return (DisplayModeEnum)GetValue(DisplayModeProperty); }
            set { SetValue(DisplayModeProperty , value); }
        }

        // Using a DependencyProperty as the backing store for DisplayMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayModeProperty =
            DependencyProperty.Register("DisplayMode" , typeof(DisplayModeEnum) , typeof(XamlDisplayer) , new PropertyMetadata(DisplayModeEnum.LeftRight , OnDisplayModeChanged));

        private static void OnDisplayModeChanged(DependencyObject d , DependencyPropertyChangedEventArgs de) {
            var xamlDisplayer = d as XamlDisplayer;
            var newValue = (DisplayModeEnum)de.NewValue;
            switch (newValue) {
                case DisplayModeEnum.LeftRight:
                    xamlDisplayer.SwitchToLeftRightMode();
                    break;
                case DisplayModeEnum.TopBottom:
                    xamlDisplayer.SwitchToTopBottomMode();
                    break;
            }
        }

        private void SwitchToTopBottomMode() {
            Grid.SetRow(CodeArea , 1);
            Grid.SetColumn(CodeArea , 0);
            Grid.SetRow(CopyButton , 1);
            Grid.SetColumn(CopyButton , 0);

        }


        private void SwitchToLeftRightMode() {
            Grid.SetRow(CodeArea , 0);
            Grid.SetColumn(CodeArea , 1);
            Grid.SetRow(CopyButton , 0);
            Grid.SetColumn(CopyButton , 1);

        }

        #endregion

        #region EventHandlers        
        private void CopyButton_OnClicked(object sender , RoutedEventArgs e) {
            Clipboard.SetDataObject(TextEditor.SelectedText.Length == 0 ? TextEditor.Text : TextEditor.SelectedText);
            Popup.IsOpen = true;
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1.5);
            timer.Start();
            timer.Tick += delegate {
                Popup.IsOpen = false;
                timer.Stop();
            };
        }

        private void CodeArea_OnMouseEnter(object sender , MouseEventArgs e) {
            CopyButton.Visibility = Visibility.Visible;
        }

        private void CodeArea_OnMouseLeave(object sender , MouseEventArgs e) {
            if (CopyButton.IsMouseOver) return;
            CopyButton.Visibility = Visibility.Hidden;
        }
        #endregion


        private void XamlDisplayer_OnLoaded(object sender, RoutedEventArgs e) {
            var c = this.Content as FrameworkElement;            
            if (c == null) return;
            c.HorizontalAlignment = HorizontalAlignment.Center;
            c.VerticalAlignment = VerticalAlignment.Center;
            if ((c as ContentControl) != null) return;
            if (double.IsNaN(c.Width)) {
                c.Width = 150;
            }
            if (double.IsNaN(c.ActualHeight)) {
                c.Height = 150;
            }

        }
    }
}
