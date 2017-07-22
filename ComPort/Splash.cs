using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ComPort
{
    public partial class Splash : Form
    {
        public Splash()
        {
            InitializeComponent();
        }

        private void Splash_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.None;

        }

        private void load_timer_Tick(object sender, EventArgs e)
        {
    
            progressBar.Increment(1);
            if (progressBar.Value == 200)
            {
                load_timer.Stop();
            }    
            
            //load_timer.Start();
            //this.Close();
        }
    }
}
