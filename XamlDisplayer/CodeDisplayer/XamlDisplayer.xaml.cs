using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;
using CodeDisplayer;

namespace CodeDisplayer {
    /// <summary>
    /// This class is to display the Xaml of its child
    /// </summary>
    public partial class XamlDisplayer : UserControl {
        public XamlDisplayer() {
            InitializeComponent();
        }

        #region DependencyProperties
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
            if (xamlDisplayer == null || content == null) return;
            if (GetDisplayCode(content))
                xamlDisplayer.ContentPresenter.Content = content;
            else {
                xamlDisplayer.Grid.Visibility = Visibility.Collapsed;
                var sp = xamlDisplayer.StackPanel;
                sp.HorizontalAlignment = HorizontalAlignment.Left;
                sp.Margin = new Thickness(0 , 40 , 0 , 0);
                sp.Children.Add(content);
            }
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

        #region IsCodeDisplayingPanelExpanded


        public bool IsCodeDisplayingPanelExpanded {
            get { return (bool)GetValue(IsCodeDisplayingPanelExpandedProperty); }
            set { SetValue(IsCodeDisplayingPanelExpandedProperty , value); }
        }

        // Using a DependencyProperty as the backing store for IsCodeDisplayed.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCodeDisplayingPanelExpandedProperty =
            DependencyProperty.Register("IsCodeDisplayingPanelExpanded" , typeof(bool) , typeof(XamlDisplayer) , new PropertyMetadata(true , OnIsCodeDisplayingPanelExpandedPropertyChanged));

        private static void OnIsCodeDisplayingPanelExpandedPropertyChanged(DependencyObject d , DependencyPropertyChangedEventArgs e) {
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

        #endregion

        #region AttachedProperties
        public static readonly DependencyProperty DisplayCodeProperty = DependencyProperty.RegisterAttached(
          "DisplayCode" ,
          typeof(Boolean) ,
          typeof(XamlDisplayerPanel) ,
          new FrameworkPropertyMetadata(true , FrameworkPropertyMetadataOptions.AffectsRender)
        );

        public static void SetDisplayCode(UIElement element , Boolean value) {
            element.SetValue(DisplayCodeProperty , value);
        }

        public static Boolean GetDisplayCode(UIElement element) {
            return (Boolean)element.GetValue(DisplayCodeProperty);
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

        private void XamlDisplayer_OnLoaded(object sender , RoutedEventArgs e) {
            var c = this.Content as FrameworkElement;
            if (c == null) return;
            c.HorizontalAlignment = HorizontalAlignment.Center;
            c.VerticalAlignment = VerticalAlignment.Center;
        }

        private void ContentArea_OnLoaded(object sender , RoutedEventArgs e) {
            if (ContentArea.ActualHeight > TextEditor.MaxHeight)
                TextEditor.MaxHeight = ContentArea.ActualHeight;
            if (ContentArea.ActualWidth > TextEditor.MaxWidth)
                TextEditor.MaxWidth = ContentArea.ActualWidth;
        }
        #endregion
    }
}

