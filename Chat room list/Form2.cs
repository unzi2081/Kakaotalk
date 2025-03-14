using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chat_room_list
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
    
        }
        public void ShowRichEditHandle(IntPtr richEditHandle)
        {
            // 핸들값을 텍스트 박스에 표시
            label1.Text = $"handle:{richEditHandle}";
        }
        private void Form2_Load(object sender, EventArgs e)
        {

        }
    }
}
