using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyCommon
{
    public partial class SimpleInput : Form
    {
        public SimpleInput()
        {
            InitializeComponent();
        }

        public string InputName
        {
            get
            {
                return this.textBox1.Text;
            }
        }

        public DialogResult ShowInputDialog(string title = "")
        {
            this.Text = title;
            return this.ShowDialog();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.textBox1.Text))
            {
                return;
            }

            this.DialogResult = DialogResult.OK;
        }

        private void SimpleInput_Load(object sender, EventArgs e)
        {
            this.textBox1.Clear();
        }
    }
}
