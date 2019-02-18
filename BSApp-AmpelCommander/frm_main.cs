using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using Newtonsoft.Json;

namespace BSApp_AmpelCommander
{
    public partial class frm_main : Form
    {
        AmpelVars vars = new AmpelVars();

        private double buf_VorTim = 20;
        private double buf_SchTim = 240;
        private double buf_horn = 0;
        private bool buf_wechs = true;
        private bool buf_ab = true;
        private bool run = false;


        private int set_VorTim = 20;
        private int set_SchTim = 240;
        private bool set_ABCD = true;
        private string set_Text = "www.ampel.bsapp.de";

        public bool Set_ABCD
        {
            get
            {
                return set_ABCD;
            }

            set
            {
                set_ABCD = value;
            }
        }

        public string Set_Text
        {
            get
            {
                return set_Text;
            }

            set
            {
                set_Text = value;
            }
        }

        public int Get_VorTim()
        {
            return set_VorTim;
        }

        public void Set_VorTim(int value)
        {
            if(value >= 0)
            {
                set_VorTim = value;
            }
        }

        public int Get_SchTim()
        {
            return set_SchTim;
        }

        public void Set_SchTim(int value)
        {
            if(value > 0)
            {
                set_SchTim = value;
            }
        }


        public frm_main()
        {
            InitializeComponent();
            bgw_sendData.RunWorkerAsync();
            this.BackColor = Color.FromArgb(66, 66, 66);
            lbl_title.ForeColor = Color.FromArgb(255, 235, 59);
            pb_yLine.BackColor = Color.FromArgb(255, 235, 59);
            setAllFont(this.Controls);
            btn_dispAmp.BackColor = Color.FromArgb(244, 67, 54);
            btn_start.BackColor = Color.FromArgb(76, 175, 80);
            btn_halt.BackColor = Color.FromArgb(255, 235, 59);
            btn_stop.BackColor = Color.FromArgb(244, 67, 54);
        }

        private void bgw_sendData_DoWork(object sender, DoWorkEventArgs e)
        {
            var Client = new UdpClient(15000);
            
            var ServerEp = new IPEndPoint(IPAddress.Any, 15000);

            while (true)
            {
                var RequestData = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(vars));
                Client.EnableBroadcast = true;
                Client.Send(RequestData, RequestData.Length, new IPEndPoint(IPAddress.Broadcast, 15000));

                if (bgw_sendData.CancellationPending)
                {
                    Client.Close();
                    break;
                }
            }
            
        }

        private void frm_main_FormClosing(object sender, FormClosingEventArgs e)
        {
            bgw_sendData.CancelAsync();
        }

        private void frm_main_ResizeEnd(object sender, EventArgs e)
        {
            setAllFont(this.Controls);
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
                else if (con.HasChildren)
                {
                    setAllFont(con.Controls);
                }
            }
        }

        private void update_Display()
        {
            if (!Set_ABCD)
            {
                btn_dispABCD.Text = "";
                btn_abcd.Enabled = false;
                vars.Bo_abcd = Set_ABCD;
            }
            else
            {
                if (btn_dispABCD.Text != vars.Str_abcd)
                {
                    btn_dispABCD.Text = vars.Str_abcd;
                }
            }

            if (vars.Bo_time)
            {
                btn_text.BackColor = Color.DimGray;
                setAllFont(this.Controls);
                vars.Str_time = "0";
            }
            else
            {
                btn_text.BackColor = Color.FromArgb(76, 175, 80);
                setAllFont(this.Controls);
            }

            if (vars.Bo_green)
            {
                btn_dispAmp.BackColor = Color.FromArgb(76, 175, 80);
                btn_dispAmp.Text = "GR";
            }

            if (vars.Bo_yellow)
            {
                btn_dispAmp.BackColor = Color.FromArgb(255, 235, 59);
                btn_dispAmp.Text = "GE";
            }

            if (vars.Bo_red)
            {
                btn_dispAmp.BackColor = Color.FromArgb(244, 67, 54);
                btn_dispAmp.Text = "RO";
            }

            lbl_time.Text = vars.Str_time;
        }

        private void lbl_title_Click(object sender, EventArgs e)
        {
            setAllFont(this.Controls);
        }

        private void btn_settings_Click(object sender, EventArgs e)
        {
            frm_settings set = new frm_settings(this);
            set.Show();
        }

        private void lbl_title_DoubleClick(object sender, EventArgs e)
        {
            if(MessageBox.Show("AMPEL BEENDEN?","BEENDEN",MessageBoxButtons.YesNo,MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                Close();
            }
        }

        private void tim_schuss_Tick(object sender, EventArgs e)
        {
            if (run)
            {
                if (buf_VorTim - 0.5 > 0)
                {
                    buf_VorTim = buf_VorTim - 0.5;

                    vars.Bo_green = false;
                    vars.Bo_red = true;
                    vars.Bo_yellow = false;
                    vars.Bo_time = true;
                    vars.Str_time = string.Format("{0:0}", (set_VorTim - buf_VorTim));
                    if (Set_ABCD)
                    {
                        if (buf_ab)
                        {
                            vars.Str_abcd = "A/B";
                        }
                        else
                        {
                            vars.Str_abcd = "C/D";
                        }
                    }
                }
                else if (buf_VorTim - 0.5 == 0)
                {
                    buf_horn = 1;
                    buf_VorTim = buf_VorTim - 0.5;
                    buf_SchTim = set_SchTim - 1;

                    vars.Bo_green = true;
                    vars.Bo_red = false;
                    vars.Bo_yellow = false;
                    vars.Bo_time = true;
                    vars.Str_time = string.Format("{0:0}", (set_VorTim - buf_VorTim));

                    if (Set_ABCD)
                    {
                        if (buf_ab)
                        {
                            vars.Str_abcd = "A/B";
                        }
                        else
                        {
                            vars.Str_abcd = "C/D";
                        }
                    }
                }
                else if (buf_SchTim > 30)
                {
                    buf_SchTim = buf_SchTim - 0.5;

                    vars.Bo_green = true;
                    vars.Bo_red = false;
                    vars.Bo_yellow = false;
                    vars.Bo_time = true;
                    vars.Str_time = string.Format("{0:0}", buf_SchTim);
                    if (Set_ABCD)
                    {
                        if (buf_ab)
                        {
                            vars.Str_abcd = "A/B";
                        }
                        else
                        {
                            vars.Str_abcd = "C/D";
                        }
                    }
                }
                else if(buf_SchTim - 0.5 > 0)
                {
                    buf_SchTim = buf_SchTim - 0.5;

                    vars.Bo_green = false;
                    vars.Bo_red = false;
                    vars.Bo_yellow = true;
                    vars.Bo_time = true;
                    vars.Str_time = string.Format("{0:0}", buf_SchTim);
                    if (Set_ABCD)
                    {
                        if (buf_ab)
                        {
                            vars.Str_abcd = "A/B";
                        }
                        else
                        {
                            vars.Str_abcd = "C/D";
                        }
                    }
                }
                else if (buf_SchTim - 0.5 == 0)
                {
                    buf_SchTim = buf_SchTim - 0.5;
                    if (buf_wechs)
                    {
                        buf_horn = 2;
                        buf_ab = !buf_ab;
                        buf_VorTim = set_VorTim;
                        buf_wechs = false;
                    }
                    else
                    {
                        buf_horn = 3;
                    }
                }
                else if(buf_SchTim == 0)
                {
                    btn_start.Enabled = true;
                    btn_stop.Enabled = false;
                    btn_halt.Enabled = false;
                    btn_text.Enabled = true;

                    run = false;
                    vars.Bo_green = false;
                    vars.Bo_red = true;
                    vars.Bo_yellow = false;
                    vars.Bo_time = true;
                    vars.Str_time = string.Format("{0:0}", buf_SchTim);
                    if (Set_ABCD)
                    {
                        if (buf_ab)
                        {
                            vars.Str_abcd = "A/B";
                        }
                        else
                        {
                            vars.Str_abcd = "C/D";
                        }
                    }
                }
            }

            if (buf_horn > 0)
            {
                buf_horn = buf_horn - 0.5;
                vars.Bo_horn = !vars.Bo_horn;
                if (vars.Bo_horn)
                {
                    lbl_time.ForeColor = Color.Red;
                }
                else
                {
                    lbl_time.ForeColor = Color.WhiteSmoke;
                }
            }
            else
            {
                vars.Bo_horn = false;
            }

            update_Display();
        }

        private void btn_start_Click(object sender, EventArgs e)
        {
            btn_start.Enabled = false;
            btn_stop.Enabled = true;
            btn_abcd.Enabled = vars.Bo_abcd;
            btn_halt.Enabled = true;
            btn_text.Enabled = false;
            vars.Bo_time = true;

            buf_VorTim = set_VorTim;
            buf_wechs = Set_ABCD;
            if (Set_ABCD)
            {
                vars.Bo_abcd = Set_ABCD;
            }
            buf_horn = 2;
            run = true;
        }

        private void btn_abcd_Click(object sender, EventArgs e)
        {
            if (Set_ABCD)
            {
                buf_ab = !buf_ab;
                if (buf_ab)
                {
                    vars.Str_abcd = "A/B";
                }
                else
                {
                    vars.Str_abcd = "C/D";
                }
            }
        }

        private void btn_halt_Click(object sender, EventArgs e)
        {
            if (run)
            {
                buf_horn = 5;
                run = false;
                btn_stop.Enabled = false;
                btn_abcd.Enabled = false;
                btn_start.Enabled = false;
            }
            else
            {
                buf_horn = 1;
                run = true;
                btn_abcd.Enabled = vars.Bo_abcd;
                btn_stop.Enabled = true;
            }
            
        }

        private void btn_stop_Click(object sender, EventArgs e)
        {
            if (buf_VorTim > 0)
            {
                buf_VorTim = 1;
            }

            if(buf_SchTim > 0)
            {
                buf_SchTim = 1;
            }
        }

        private void btn_text_Click(object sender, EventArgs e)
        {
            vars.Bo_time = !vars.Bo_time;
            if (!vars.Bo_time)
            {
                vars.Str_time = Set_Text;
            }
        }
    }
}
