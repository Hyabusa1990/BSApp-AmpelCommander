using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BSApp_AmpelCommander
{
    public partial class frm_settings : Form
    {
        frm_main main;
        public frm_settings(frm_main pMain)
        {
            InitializeComponent();
            main = pMain;

            this.BackColor = Color.FromArgb(66, 66, 66);
            lbl_title.ForeColor = Color.FromArgb(255, 235, 59);
            pb_yLine.BackColor = Color.FromArgb(255, 235, 59);
            lbl_abcd.ForeColor = Color.FromArgb(255, 235, 59);
            lbl_schTim.ForeColor = Color.FromArgb(255, 235, 59);
            lbl_text.ForeColor = Color.FromArgb(255, 235, 59);
            lbl_vorTim.ForeColor = Color.FromArgb(255, 235, 59);
            lbl_schTimAnz.ForeColor = Color.FromArgb(255, 235, 59);
            lbl_vorTimAnz.ForeColor = Color.FromArgb(255, 235, 59);

            btn_abcdON.BackColor = Color.FromArgb(76, 175, 80);

            set_Text();
            setAllFont(this.Controls);
            
        }

        private void set_Text()
        {
            lbl_schTimAnz.Text = main.Get_SchTim() + " s";
            lbl_vorTimAnz.Text = main.Get_VorTim() + " s";
            tb_text.Text = main.Set_Text;

            if (main.Set_ABCD)
            {
                btn_abcdON.BackColor = Color.FromArgb(76, 175, 80);
                btn_abcdOFF.BackColor = Color.Transparent;
            }
            else
            {
                btn_abcdON.BackColor = Color.Transparent;
                btn_abcdOFF.BackColor = Color.FromArgb(244, 67, 54);
            }
        }

        private void setAllFont(Control.ControlCollection conts)
        {
            Graphics g = this.CreateGraphics();

            foreach (Control con in conts)
            {
                if (con is Button || con is Label)
                {
                    float sizeX = 0;
                    float sizeY = 0;
                    int fontSize = 1;
                    //LBL Time
                    do
                    {
                        fontSize++;
                        con.Font = new Font(con.Font.FontFamily, fontSize);
                        SizeF stringSize = new SizeF();
                        stringSize = g.MeasureString(con.Text, con.Font);
                        sizeX = stringSize.Width;
                        sizeY = stringSize.Height;
                    }
                    while (sizeX < con.Width - (con.Width * 0.08) && sizeY < con.Height - (con.Height * 0.08));
                }
                else if(con is TextBox)
                {
                    float sizeX = 0;
                    float sizeY = 0;
                    int fontSize = 1;
                    //LBL Time
                    do
                    {
                        fontSize++;
                        con.Font = new Font(con.Font.FontFamily, fontSize);
                        SizeF stringSize = new SizeF();
                        stringSize = g.MeasureString(con.Text, con.Font);
                        sizeX = stringSize.Width;
                        sizeY = stringSize.Height;
                    }
                    while (sizeX < con.Width - (con.Width * 0.08) && sizeY < con.Parent.Height - (con.Parent.Height * 0.08));
                }
                else if (con.HasChildren)
                {
                    setAllFont(con.Controls);
                }
            }
        }

        private void btn_back_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_vorTimDown_Click(object sender, EventArgs e)
        {
            main.Set_VorTim(main.Get_VorTim() - 10);
            set_Text();
        }

        private void btn_vorTimUp_Click(object sender, EventArgs e)
        {
            main.Set_VorTim(main.Get_VorTim() + 10);
            set_Text();
        }

        private void btn_schTimDown_Click(object sender, EventArgs e)
        {
            main.Set_SchTim(main.Get_SchTim() - 10);
            set_Text();
        }

        private void btn_schTimUp_Click(object sender, EventArgs e)
        {
            main.Set_SchTim(main.Get_SchTim() + 10);
            set_Text();
        }

        private void btn_abcdON_Click(object sender, EventArgs e)
        {
            main.Set_ABCD = true;
            set_Text();
        }

        private void btn_abcdOFF_Click(object sender, EventArgs e)
        {
            main.Set_ABCD = false;
            set_Text();
        }

        private void tb_text_TextChanged(object sender, EventArgs e)
        {
            main.Set_Text = tb_text.Text;
        }
    }
}
