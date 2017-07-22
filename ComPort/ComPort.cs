using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Management;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Threading;
using System.Text.RegularExpressions;
using Microsoft.Win32;


namespace ComPort
{
    public partial class ComPort : MetroFramework.Forms.MetroForm
    {
        string vidID = "0483", pidID = "374B";
        //string vidID = "";
        //string pidID = "";
        string comPort;
        bool flag1 = false;

        private const int WM_DEVICECHANGE = 0x219;
        private const int DBT_DEVICEARRIVAL = 0x8000;
        private const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
        private const int DBT_DEVTYP_VOLUME = 0x00000002;
        bool flag;


        public ComPort()
        {
            Thread splashThread = new Thread(new ThreadStart(SplashStart));
            splashThread.Start();
            Thread.Sleep(2000);
            InitializeComponent();
            splashThread.Abort();
        }


        // Loading Splash Screen
        public void SplashStart()
        {
            Application.Run(new Splash());
        }


        //
        public void serialPortCom() 
        {
            SerialPort mySerialPort = new SerialPort("COM25");

            mySerialPort.BaudRate = 115200 ;
            mySerialPort.Parity = Parity.None;
            mySerialPort.StopBits = StopBits.One;
            mySerialPort.DataBits = 8;
            mySerialPort.Handshake = Handshake.None;
            mySerialPort.RtsEnable = true;
            mySerialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            mySerialPort.Open();

        }

        // Auto Detection of USB
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            switch (m.Msg)
            {
                case WM_DEVICECHANGE:
                    switch ((int)m.WParam)
                    {
                        case DBT_DEVICEARRIVAL:
                            Thread thread = new Thread(() =>
                            {
                                pidID = pid_textBox.Text.ToString();
                                vidID = vid_textBox.Text.ToString();
                                using (var searcher =
    new ManagementObjectSearcher(@"Select * From Win32_USBControllerDevice"))
                                {

                                    try
                                    {
                                        using (var collection = searcher.Get())
                                        {
                                            foreach (var device in collection)
                                            {
                                                var usbDevice = Convert.ToString(device);

                                                if (usbDevice.Contains(pidID) && usbDevice.Contains(vidID))
                                                {
                                                    flag = true;
                                                }
                                                else
                                                {
                                                    flag = false;
                                                }

                                            }
                                        }
                                    }
                                    catch (Exception)
                                    {

                                        throw;
                                    }
                                }
                            });
                            thread.Start();
                            thread.Join();
                            
                            if (flag == true)
                            {
                                comPort_status.Image = Properties.Resources.connect_icon;
                                if (flag1 == false)
                                {
                                    serialPortCom();
                                    flag1 = true;
                                }
                                
                            }
                            break;

                        case DBT_DEVICEREMOVECOMPLETE:
                            comPort_status.Image = Properties.Resources.disconnect_icon;
                            if (flag1 == true)
                            {
                                flag1 = false;
                            }
                            break;
                    }
                    break;
            }
        }

        private void send_btn_Click(object sender, EventArgs e)
        {
            SerialPort sp = new SerialPort();
            string text = send_tbox.Text.ToString();
            sp.Write(text);
        }

        // When Data is reiceved on serial port it calls this method
        private static void DataReceivedHandler(
                        object s,
                        SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort) s;
            string indata = sp.ReadExisting();
            if (indata == "k")
            {
                sp.Write("ack");
            }
        }

    }
}
