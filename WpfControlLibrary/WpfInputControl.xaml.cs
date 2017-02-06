using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WFControlLibrary;

namespace WpfControlLibrary
{
    public delegate void MyControlEventHandler(object sender, MyControlEventArgs args);

    /// <summary>
    /// Interaction logic for WPFInputControl.xaml
    /// </summary>
    public partial class WpfInputControl : Grid
    {
        public event MyControlEventHandler OnButtonClick;
        private FontWeight _fontWeight;
        private double _fontSize;
        private FontFamily _fontFamily;
        private FontStyle _fontStyle;
        private SolidColorBrush _foreground;
        private SolidColorBrush _background;

        public FontWeight FontWeight
        {
            get { return _fontWeight; }
            set
            {
                _fontWeight = value;
                nameLabel.FontWeight = value;
                addressLabel.FontWeight = value;
                cityLabel.FontWeight = value;
                stateLabel.FontWeight = value;
                zipLabel.FontWeight = value;
            }
        }
        public double FontSize
        {
            get { return _fontSize; }
            set
            {
                _fontSize = value;
                nameLabel.FontSize = value;
                addressLabel.FontSize = value;
                cityLabel.FontSize = value;
                stateLabel.FontSize = value;
                zipLabel.FontSize = value;
            }
        }
        public FontStyle FontStyle
        {
            get { return _fontStyle; }
            set
            {
                _fontStyle = value;
                nameLabel.FontStyle = value;
                addressLabel.FontStyle = value;
                cityLabel.FontStyle = value;
                stateLabel.FontStyle = value;
                zipLabel.FontStyle = value;
            }
        }
        public FontFamily FontFamily
        {
            get { return _fontFamily; }
            set
            {
                _fontFamily = value;
                nameLabel.FontFamily = value;
                addressLabel.FontFamily = value;
                cityLabel.FontFamily = value;
                stateLabel.FontFamily = value;
                zipLabel.FontFamily = value;
            }
        }

        public SolidColorBrush MyBackground
        {
            get { return _background; }
            set
            {
                _background = value;
                rootElement.Background = value;
            }
        }
        public SolidColorBrush Foreground
        {
            get { return _foreground; }
            set
            {
                _foreground = value;
                nameLabel.Foreground = value;
                addressLabel.Foreground = value;
                cityLabel.Foreground = value;
                stateLabel.Foreground = value;
                zipLabel.Foreground = value;
            }
        }

        public WpfInputControl()
        {
            InitializeComponent();
        }

        private void Init(object sender, RoutedEventArgs e)
        {
            //They all have the same style, so use nameLabel to set initial values.
            _fontWeight = nameLabel.FontWeight;
            _fontSize = nameLabel.FontSize;
            _fontFamily = nameLabel.FontFamily;
            _fontStyle = nameLabel.FontStyle;
            _foreground = (SolidColorBrush)nameLabel.Foreground;
            _background = (SolidColorBrush)rootElement.Background;
        }

        private void ButtonClicked(object sender, RoutedEventArgs e)
        {
            var retvals = new MyControlEventArgs(true,
                                                        txtName.Text,
                                                        txtAddress.Text,
                                                        txtCity.Text,
                                                        txtState.Text,
                                                        txtZip.Text);
            if (sender == btnCancel)
            {
                retvals.IsOK = false;
            }
            if (OnButtonClick != null)
                OnButtonClick(this, retvals);

        }
    }
}
