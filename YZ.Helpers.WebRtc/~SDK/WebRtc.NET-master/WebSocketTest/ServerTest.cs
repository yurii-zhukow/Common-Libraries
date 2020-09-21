using System;
using System.Collections.Generic;
using System.Text;
using WebRtc.NET;
using YZ.Helpers.WebRtc;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace WebSocketTest {
    public class ServerTest {
        WebRTCServer webSocketServer;

        public ServerTest() {

            try {

                webSocketServer = new WebRTCServer(9000);
                unsafe {
                    webSocketServer.OnRenderFrame = OnRenderFrame;
                }
            } catch (Exception ex) {
                Console.WriteLine("checkBoxWebsocket_CheckedChanged: " + ex.Message);
            }


        }

        readonly TurboJpegEncoder encoderRemote = TurboJpegEncoder.CreateEncoder();

        byte[] bgrBuffremote;
        Bitmap renderFrame = null;
        object OnRenderFrameLock = new object();
        public unsafe void OnRenderFrame(byte* yuv, uint w, uint h) {
            lock (OnRenderFrameLock) {
                if (0 == encoderRemote.EncodeI420toBGR24(yuv, w, h, ref bgrBuffremote, true)) {
                    if (renderFrame == null) {
                        var bufHandle = GCHandle.Alloc(bgrBuffremote, GCHandleType.Pinned);
                        renderFrame = new Bitmap((int)w, (int)h, (int)w * 3, PixelFormat.Format24bppRgb, bufHandle.AddrOfPinnedObject());
                    }
                }


            }

            try {

            } catch // don't throw on form exit
              {
            }
        }


        public void PushFrame(Bitmap img) {

            var l = img.LockBits(new Rectangle(Point.Empty, img.Size), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            try {
                foreach (var s in webSocketServer.Streams) {

                    unsafe {
                        var yuv = s.Value.WebRtc.VideoCapturerI420Buffer();
                        if (yuv != null) {
                            encoderRemote.EncodeI420((byte*)l.Scan0, img.Width, img.Height, (int)TJPF.TJPF_BGR, 0, true, yuv);
                        }
                    }

                    s.Value.WebRtc.PushFrame();
                }
            } catch (Exception) {
            } finally {
                img.UnlockBits(l);

            }
        }




    }
}
