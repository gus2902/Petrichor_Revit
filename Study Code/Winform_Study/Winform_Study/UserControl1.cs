using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Winform_Study
{
    public partial class UserControl1 : UserControl
    {
        public UserControl1()
        {
            InitializeComponent();
        }

        private void UserControl1_Load(object sender, EventArgs e)
        {
            this.Controls.Add(pBar);

            Label lb_pBar = new Label();
            lb_pBar.Parent = pBar;
            lb_pBar.Text = "0%";
            lb_pBar.AutoSize = true;
            lb_pBar.Location = new Point(pBar.Width / 2, pBar.Height / 2);
            lb_pBar.BackColor = Color.Transparent;
            
            label1.Parent = pBar;
            label1.BackColor = Color.Transparent;
            label1.Location = new Point(0, 0);

            this.Controls.Add(lb_pBar);
            this.Controls.SetChildIndex(lb_pBar, 0);

        }

        private void progressBar1_Paint(object sender, PaintEventArgs e)
        {
            // 진행 상황 계산
            int percentage = (int)(((double)pBar.Value / (double)pBar.Maximum) * 100);

            // 그리기 작업
            using (Font font = new Font("Arial", 10))
            {
                string text = percentage.ToString() + "%";
                SizeF textSize = e.Graphics.MeasureString(text, font);
                PointF textPoint = new PointF(pBar.Width / 2 - textSize.Width / 2,
                                              pBar.Height / 2 - textSize.Height / 2);

                e.Graphics.DrawString(text, font, Brushes.Black, textPoint);
            }
        }
    }
}
