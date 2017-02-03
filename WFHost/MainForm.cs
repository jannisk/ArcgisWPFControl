using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using WpfControlLibrary;

namespace WFHost
{
    public partial class MainForm : Form
    {
        private ElementHost _ctrlHost;
        private WpfControlLibrary.MapWindow _mapWindow;


        public MainForm()
        {
            InitializeComponent();

        }

        private void MainForm_Load(object sender, EventArgs e)
        {

            _ctrlHost = new ElementHost();
            _ctrlHost.Dock = DockStyle.Fill;
            panel1.Controls.Add(_ctrlHost);
            _mapWindow = new MapWindow();
            //_mapWindow.InitializeComponent();
            _ctrlHost.Child = _mapWindow;

        }
    }
}
