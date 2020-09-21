using System;
using System.Drawing;
using System.Threading;
using YZ.Helpers.WebRtc;

namespace WebSocketTest {
    class Program {


        static void Main(string[] args) {

            var srv = new ServerTest();
            while (true) {
                Thread.Sleep(1000);
                var t = DateTime.Now.ToString("hh:mm:ss.fff");
                Console.WriteLine(t);

                var g = Graphics.FromImage(frame);
                g.Clear(Color.DarkBlue);
                var rc = RectangleF.FromLTRB(0, 0, frame.Width, frame.Height);

                g.DrawString(string.Format("{0}", t), fBig, Brushes.LimeGreen, rc, sfTopRight);
                srv.PushFrame(frame);
            }
        }

        static Bitmap frame = new Bitmap(352, 288, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

        static readonly Font f = new Font("Tahoma", 14);
        static readonly Font fBig = new Font("Tahoma", 36);
        static readonly StringFormat sfTopLeft = new StringFormat() {
            Alignment = StringAlignment.Near,
            LineAlignment = StringAlignment.Near
        };

        static readonly StringFormat sfTopRight = new StringFormat() {
            Alignment = StringAlignment.Far,
            LineAlignment = StringAlignment.Near
        };

        static readonly StringFormat sfBottomLeft = new StringFormat() {
            Alignment = StringAlignment.Near,
            LineAlignment = StringAlignment.Far
        };


    }
}
