using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Media;
using DevExpress.XtraEditors;
using WpfControlLibrary;

namespace WFHost
{
    public partial class Form1 : Form
    {
        private ElementHost _ctrlHost;
        private WpfControlLibrary.WpfInputControl _wpfInputControl;
        private WpfControlLibrary.MapWindow _mapWindow;

        System.Windows.FontWeight initFontWeight;
        double initFontSize;
        System.Windows.FontStyle initFontStyle;
        System.Windows.Media.SolidColorBrush initBackBrush;
        System.Windows.Media.SolidColorBrush initForeBrush;
        System.Windows.Media.FontFamily initFontFamily;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _ctrlHost = new ElementHost();
            _ctrlHost.Dock = DockStyle.Fill;
            panelControl1.Controls.Add(_ctrlHost);
            _wpfInputControl = new WpfInputControl();
            _mapWindow = new MapWindow();
            _mapWindow.InitializeComponent();
            _wpfInputControl.InitializeComponent();

            _ctrlHost.Child = _mapWindow;

            _wpfInputControl.OnButtonClick += _wpfInputControl_OnButtonClick;
            _wpfInputControl.Loaded += _wpfInputControl_Loaded;
         
        }

        void _wpfInputControl_Loaded(object sender, RoutedEventArgs e)
        {
            initBackBrush = (SolidColorBrush)_wpfInputControl.MyBackground;
            initForeBrush = _wpfInputControl.Foreground;
            initFontFamily = _wpfInputControl.FontFamily;
            initFontSize = _wpfInputControl.FontSize;
            initFontWeight = _wpfInputControl.FontWeight;
            initFontStyle = _wpfInputControl.FontStyle;
        }

        void _wpfInputControl_OnButtonClick(object sender, WFControlLibrary.MyControlEventArgs args)
        {
            if (args.IsOK)
            {
                lblAddress.Text = "Street Address: " + args.MyStreetAddress;
                lblCity.Text = "City: " + args.MyCity;
                lblName.Text = "Name: " + args.MyName;
                lblState.Text = "State: " + args.MyState;
                lblZip.Text = "Zip: " + args.MyZip;
            }
            else
            {
                lblAddress.Text = "Street Address: ";
                lblCity.Text = "City: ";
                lblName.Text = "Name: ";
                lblState.Text = "State: ";
                lblZip.Text = "Zip: ";
            }
        }

        private void radioGroup1_EditValueChanged(object sender, EventArgs e)
        {
            
            _wpfInputControl.MyBackground = initBackBrush;
        }

        private void radioGroup1_Click(object sender, EventArgs e)
        {
            var radioGroup = (RadioGroup) sender;
            switch (radioGroup.SelectedIndex)
            {
                case 0:
                    _wpfInputControl.MyBackground = initBackBrush;
                    break;
                case 1:
                    _wpfInputControl.MyBackground  = new SolidColorBrush(Colors.LightGreen);
                    break;
                case 2:
                    _wpfInputControl.MyBackground = new SolidColorBrush(Colors.LightSalmon);
                    break;

            }
        }
    }
}
