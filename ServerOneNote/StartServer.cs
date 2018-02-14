using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using OneNoteOCRDll;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Drawing.Imaging;
using EA.GatorCore.OneNoteOCR;
using EA.GatorCore;
using EA.GatorCore.Interfaces;
using OneNoteOCRDll;
using System.Runtime.InteropServices;

namespace ServerOneNote
{
    class StartServer
    {
        private static ActionNote findText;
        private static OneNoteOCR ocr;

        static void Main(string[] args)
        {
                // Some biolerplate to react to close window event, CTRL-C, kill, etc
                _handler += new EventHandler(Handler);
                SetConsoleCtrlHandler(_handler, true);
                ocr = new OneNoteOCRDll.OneNoteOCR();
                findText = new ActionNote();
                // start server
                TcpListener listener = new TcpListener(IPAddress.Any, 1289);
                listener.Start();
                OldStartServer(listener);
        }

        public static void OldStartServer(TcpListener listener)
        {
            Console.WriteLine("Server started");
            while (!exitSystem)
            {
                try
                {
                    var client = listener.AcceptTcpClient();

                    var currentDat = DateTime.Now;
                    if (client.Connected)
                    {
                        var sendCommunication = new Communication(client);
                        Console.WriteLine("Client connected");
                        //var currentDate = String.Format("{0}{1}{2}", DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond);
                        Console.WriteLine(String.Format("Current for client {0:HH:mm:ss:fff}", currentDat));

                        var textToSearch = sendCommunication.ReceiveText();
                        sendCommunication.SendText("ok");
                        var imageReceived = sendCommunication.ReceiveImage();
                        var text = findText.FindText(imageReceived, textToSearch);
                        sendCommunication.SendTextFound(text);
                        client.Close();
                        Console.WriteLine("Client disconnected");
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Random exception thrown");
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.InnerException);
                    Console.WriteLine(ex.StackTrace);
                }
            }
        }


        public static bool exitSystem = false;

        #region Trap application termination
        [DllImport("Kernel32")]
        static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        delegate bool EventHandler(CtrlType sig);
        static EventHandler _handler;

        enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        static bool Handler(CtrlType sig)
        {
            Console.WriteLine("Exiting system due to external CTRL-C, or process kill, or shutdown");

            //do your cleanup here
            ocr.ReleaseObject(); //simulate some cleanup delay

            Console.WriteLine("Cleanup complete");

            //allow main to run off
            exitSystem = true;

            //shutdown right away so there are no lingering threads
            Environment.Exit(-1);

            return true;
        }
        #endregion
    }
}
