using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WFControlLibrary
{
    public delegate void MyControlEventHandler(object sender, MyControlEventArgs args);


    public partial class WFInputControl: UserControl
    {
        public event MyControlEventHandler OnButtonClick;

        public WFInputControl()
        {
            InitializeComponent();
        }

        private void sbOK_Click(object sender, System.EventArgs e)
        {
            var retvals = new MyControlEventArgs(true,
                                                         txtName.Text,
                                                         txtAddress.Text,
                                                         txtCity.Text,
                                                         txtState.Text,
                                                         txtZip.Text);
            OnButtonClick(this, retvals);
        }
    }
}
