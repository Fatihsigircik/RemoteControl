using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Quobject.SocketIoClientDotNet.Client;

namespace deneme5
{
    class Program
    {
        private static Socket socket;
        static void Main(string[] args)
        {

            var userName = "User";

            var option = new IO.Options
            {
                Reconnection = true,
                ReconnectionDelay = 1000,
                ReconnectionDelayMax = 3000,
                Timeout = 20000,
                ForceNew = true,
                //QueryString = "username=uname&password=pwd"
            };

            //socket = IO.Socket("http://ns993.tekrom.com", option);
            socket = IO.Socket("http://localhost:3000", option);

            socket.On("mouseChange", (obj) =>
            {
                string[] point = obj.ToString().Split(',');
                Win32.SetCursorPosition(Convert.ToInt32(point[0]), Convert.ToInt32(point[1]));
            });

            socket.On("mouseDown", (obj) =>
            {
                Console.WriteLine("mouseDown");
                Win32.MouseEvent(Win32.MouseEventFlags.LeftDown);
            });

            socket.On("mouseUp", (obj) =>
            {
                Console.WriteLine("mouseUp");
                Win32.MouseEvent(Win32.MouseEventFlags.LeftUp);
            });

            socket.On("keyPress", (obj) =>
            {
                Console.WriteLine("keyPress");
                System.Windows.Forms.SendKeys.SendWait(Convert.ToChar(obj).ToString());
            });

            socket.On("connection", (obj) =>
            {
                Console.WriteLine(obj);

                socket.Emit("sever", (data) =>
                {
                    Console.WriteLine(data.ToString());
                }, new object[] { "" });

                System.Threading.Thread t = new Thread(Tick);
                t.Start();
                //tmr_Tick(new object(), new EventArgs());
            });

            Console.ReadLine();
        }

        private static void Tick()
        {

            while (true)
            {


                using (MemoryStream m = new MemoryStream())
                {


                    ImageCodecInfo myImageCodecInfo;
                    System.Drawing.Imaging.Encoder myEncoder;
                    EncoderParameter myEncoderParameter;
                    EncoderParameters myEncoderParameters;

                    Bitmap printscreen = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
                    Graphics graphics = Graphics.FromImage(printscreen);
                    graphics.CopyFromScreen(0, 0, 0, 0, printscreen.Size);


                    myImageCodecInfo = GetImageEncoders("image/jpeg");

                    myEncoder = System.Drawing.Imaging.Encoder.Quality;

                    myEncoderParameters = new EncoderParameters(1);
                    myEncoderParameter = new EncoderParameter(myEncoder, 40L);

                    myEncoderParameters.Param[0] = myEncoderParameter;


                    printscreen.Save(m, myImageCodecInfo, myEncoderParameters);
                    byte[] imageBytes = m.ToArray();

                    string base64String = Convert.ToBase64String(imageBytes);

                    SendMessage(base64String);
                }

            }
        }


        static void SendMessage(string msg)
        {
            socket.Emit("sendMessage", (data) =>
            {
                Console.WriteLine(data.ToString());
            }, new object[] { msg });


        }




        private static ImageCodecInfo GetImageEncoders(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

    }




    public class Win32
    {

        [Flags]
        public enum MouseEventFlags
        {
            LeftDown = 0x00000002,
            LeftUp = 0x00000004,
            MiddleDown = 0x00000020,
            MiddleUp = 0x00000040,
            Move = 0x00000001,
            Absolute = 0x00008000,
            RightDown = 0x00000008,
            RightUp = 0x00000010
        }

        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out MousePoint lpMousePoint);

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        public static void SetCursorPosition(int X, int Y)
        {
            SetCursorPos(X, Y);
        }

        public static void SetCursorPosition(MousePoint point)
        {
            SetCursorPos(point.X, point.Y);
        }

        public static MousePoint GetCursorPosition()
        {
            MousePoint currentMousePoint;
            var gotPoint = GetCursorPos(out currentMousePoint);
            if (!gotPoint) { currentMousePoint = new MousePoint(0, 0); }
            return currentMousePoint;
        }

        public static void MouseEvent(MouseEventFlags value)
        {
            MousePoint position = GetCursorPosition();

            mouse_event
                ((int)value,
                 position.X,
                 position.Y,
                 0,
                 0)
                ;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MousePoint
        {
            public int X;
            public int Y;

            public MousePoint(int x, int y)
            {
                X = x;
                Y = y;
            }

        }

    }

}

