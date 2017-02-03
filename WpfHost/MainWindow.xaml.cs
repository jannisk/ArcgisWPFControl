using System;
using System.Windows;
using System.Windows.Media;
using WFControlLibrary;

namespace WpfHost
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Application app;
        private Window _myWindow;
        FontWeight _initFontWeight;
        Double _initFontSize;
        FontStyle _initFontStyle;
        SolidColorBrush _initBackBrush;
        SolidColorBrush _initForeBrush;
        FontFamily _initFontFamily;
        bool _uiIsReady = false;


        public MainWindow()
        {
            InitializeComponent();
        }

        private void Init(object sender, RoutedEventArgs e)
        {
            app = System.Windows.Application.Current;
            _myWindow = (Window)app.MainWindow;
            _myWindow.SizeToContent = SizeToContent.WidthAndHeight;
            wfh.TabIndex = 10;
            _initFontSize = wfh.FontSize;
            _initFontWeight = wfh.FontWeight;
            _initFontFamily = wfh.FontFamily;
            _initFontStyle = wfh.FontStyle;
            _initBackBrush = (SolidColorBrush)wfh.Background;
            _initForeBrush = (SolidColorBrush)wfh.Foreground;
            ((WFInputControl) wfh.Child).OnButtonClick += PanelOnButtonClick;
            _uiIsReady = true;
        }

        private void PanelOnButtonClick(object sender, MyControlEventArgs args)
        {
            txtName.Inlines.Clear();
            txtAddress.Inlines.Clear();
            txtCity.Inlines.Clear();
            txtState.Inlines.Clear();
            txtZip.Inlines.Clear();

            if (!args.IsOK) return;
            txtName.Inlines.Add(" " + args.MyName);
            txtAddress.Inlines.Add(" " + args.MyStreetAddress);
            txtCity.Inlines.Add(" " + args.MyCity);
            txtState.Inlines.Add(" " + args.MyState);
            txtZip.Inlines.Add(" " + args.MyZip);
        }

        private void BackColorChanged(object sender, RoutedEventArgs e)
        {
            if (sender == rdbtnBackGreen)
                wfh.Background = new SolidColorBrush(Colors.LightGreen);
            else if (sender == rdbtnBackSalmon)
                wfh.Background = new SolidColorBrush(Colors.LightSalmon);
            else if (_uiIsReady == true)
                wfh.Background = _initBackBrush;
        }

        private void ForeColorChanged(object sender, RoutedEventArgs e)
        {
            if (sender == rdbtnForeRed)
                wfh.Foreground = new SolidColorBrush(Colors.Red);
            else if (sender == rdbtnForeYellow)
                wfh.Foreground = new SolidColorBrush(Colors.Yellow);
            else if (_uiIsReady == true)
                wfh.Foreground = _initForeBrush;
        }

        private void FontChanged(object sender, RoutedEventArgs e)
        {
            if (sender == rdbtnTimes)
                wfh.FontFamily = new FontFamily("Times New Roman");
            else if (sender == rdbtnWingdings)
                wfh.FontFamily = new FontFamily("Wingdings");
            else if (_uiIsReady == true)
                wfh.FontFamily = _initFontFamily;
        }
        private void FontSizeChanged(object sender, RoutedEventArgs e)
        {
            if (sender == rdbtnTen)
                wfh.FontSize = 10;
            else if (sender == rdbtnTwelve)
                wfh.FontSize = 12;
            else if (_uiIsReady == true)
                wfh.FontSize = _initFontSize;
        }
        private void StyleChanged(object sender, RoutedEventArgs e)
        {
            if (sender == rdbtnItalic)
                wfh.FontStyle = FontStyles.Italic;
            else if (_uiIsReady == true)
                wfh.FontStyle = _initFontStyle;
        }
        private void WeightChanged(object sender, RoutedEventArgs e)
        {
            if (sender == rdbtnBold)
                wfh.FontWeight = FontWeights.Bold;
            else if (_uiIsReady == true)
                wfh.FontWeight = _initFontWeight;
        }
    }
}
