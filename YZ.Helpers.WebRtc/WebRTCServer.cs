using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

using Fleck;

using LitJson;

using WebRtc.NET;

namespace YZ.WebRtc {
    public class WebRTCServer : IDisposable {
        public class WebRtcSession {
            public readonly ManagedConductor WebRtc;
            public readonly CancellationTokenSource Cancel;

            public WebRtcSession() {
                WebRtc = new ManagedConductor();
                Cancel = new CancellationTokenSource();
            }
        }

        public readonly ConcurrentDictionary<Guid, IWebSocketConnection> UserList = new ConcurrentDictionary<Guid, IWebSocketConnection>();
        public readonly ConcurrentDictionary<Guid, WebRtcSession> Streams = new ConcurrentDictionary<Guid, WebRtcSession>();

        WebSocketServer server;

        public WebRTCServer(string ip, int port, string key, Size frameSize, X509Certificate2 cert) : this($"wss://{ip}:{port}", key, cert) {
            FrameSize = frameSize;
        }

        public WebRTCServer(string url, string key, X509Certificate2 cert) {
            server = new WebSocketServer($"{url}/{key}") {
                Certificate = cert,
                //EnabledSslProtocols=System.Security.Authentication.SslProtocols.None
                RestartAfterListenError=true,
            };
            server.Start(socket => {
                socket.OnOpen = () => {
                    try {
                        OnConnected(socket);
                    } catch (Exception ex) {
                        Debug.WriteLine($"OnConnected: {ex}");
                    }
                };
                socket.OnMessage = message => {
                    try {
                        OnReceive(socket, message);
                    } catch (Exception ex) {
                        Debug.WriteLine($"OnReceive: {ex}");
                    }
                };
                socket.OnClose = () => {
                    try {
                        OnDisconnect(socket);
                    } catch (Exception ex) {
                        Debug.WriteLine($"OnDisconnect: {ex}");
                    }
                };
                socket.OnError = (e) => {
                    try {
                        OnDisconnect(socket);
                        socket.Close();
                    } catch (Exception ex) {
                        Debug.WriteLine($"OnError: {ex}");
                    }
                };
            });
        }
        public string Url => server?.Location ?? "ws://??? (no server)";

        private void OnConnected(IWebSocketConnection context) {
            if (UserList.Count < ClientLimit) {
                Debug.WriteLine($"OnConnected: {context.ConnectionInfo.Id}, {context.ConnectionInfo.ClientIpAddress}");

                UserList[context.ConnectionInfo.Id] = context;
            } else {
                Debug.WriteLine($"OverLimit, Closed: {context.ConnectionInfo.Id}, {context.ConnectionInfo.ClientIpAddress}");
                context.Close();
            }
        }

        private int clientLimit = 5;
        public int ClientLimit {
            get {
                lock (this) {
                    return clientLimit;
                }
            }
            set {
                lock (this) {
                    clientLimit = value;
                }
            }
        }

        public int ClientCount => UserList.Count;
        public int StreamsCount => Streams.Count;

        public Size FrameSize { get; }

        private void OnDisconnect(IWebSocketConnection context) {
            Debug.WriteLine($"OnDisconnect: {context.ConnectionInfo.Id}, {context.ConnectionInfo.ClientIpAddress}");
            {
                IWebSocketConnection ctx;
                UserList.TryRemove(context.ConnectionInfo.Id, out ctx);

                WebRtcSession s;
                if (Streams.TryRemove(context.ConnectionInfo.Id, out s)) {
                    s.Cancel.Cancel();
                }
            }
        }

        public const string offer = "offer";
        public const string onicecandidate = "onicecandidate";

        private void OnReceive(IWebSocketConnection context, string msg) {
            Debug.WriteLine($"OnReceive {context.ConnectionInfo.Id}: {msg}");

            if (!msg.Contains("command")) return;

            if (UserList.ContainsKey(context.ConnectionInfo.Id)) {
                JsonData msgJson = JsonMapper.ToObject(msg);
                string command = msgJson["command"].ToString();

                switch (command) {
                    case offer: {
                            if (UserList.Count <= ClientLimit && !Streams.ContainsKey(context.ConnectionInfo.Id)) {
                                var session = Streams[context.ConnectionInfo.Id] = new WebRtcSession();
                                {
                                    using (var go = new ManualResetEvent(false)) {
                                        var t = Task.Factory.StartNew(() => {
                                            ManagedConductor.InitializeSSL();

                                            using (session.WebRtc) {
                                                session.WebRtc.AddServerConfig("stun:stun.l.google.com:19302", string.Empty, string.Empty);
                                                session.WebRtc.AddServerConfig("stun:stun.anyfirewall.com:3478", string.Empty, string.Empty);
                                                session.WebRtc.AddServerConfig("stun:stun.stunprotocol.org:3478", string.Empty, string.Empty);
                                                //session.WebRtc.AddServerConfig("turn:192.168.0.100:3478", "test", "test");

                                                session.WebRtc.SetAudio(false);


                                                //if (!Form.checkBoxVirtualCam.Checked)
                                                //{
                                                //    if (!string.IsNullOrEmpty(Form.videoDevice))
                                                //    {
                                                //        var vok = session.WebRtc.OpenVideoCaptureDevice(Form.videoDevice);
                                                //        Trace.WriteLine($"OpenVideoCaptureDevice: {vok}, {Form.videoDevice}");
                                                //    }
                                                //}
                                                //else
                                                {
                                                    session.WebRtc.SetVideoCapturer(FrameSize.Width, FrameSize.Height, 25, false);
                                                }

                                                var ok = session.WebRtc.InitializePeerConnection();
                                                if (ok) {
                                                    go.Set();

                                                    // javascript side makes the offer in this demo
                                                    //session.WebRtc.CreateDataChannel("msgDataChannel");

                                                    while (!session.Cancel.Token.IsCancellationRequested &&
                                                           session.WebRtc.ProcessMessages(1000)) {
                                                        Debug.Write(".");
                                                    }
                                                    session.WebRtc.ProcessMessages(1000);
                                                } else {
                                                    Debug.WriteLine("InitializePeerConnection failed");
                                                    context.Close();
                                                }
                                            }

                                        }, session.Cancel.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

                                        if (go.WaitOne(9999)) {
                                            session.WebRtc.OnIceCandidate += delegate (string sdp_mid, int sdp_mline_index, string sdp) {
                                                if (context.IsAvailable) {
                                                    JsonData j = new JsonData();
                                                    j["command"] = "OnIceCandidate";
                                                    j["sdp_mid"] = sdp_mid;
                                                    j["sdp_mline_index"] = sdp_mline_index;
                                                    j["sdp"] = sdp;
                                                    context.Send(j.ToJson());
                                                }
                                            };

                                            session.WebRtc.OnSuccessAnswer += delegate (string sdp) {
                                                if (context.IsAvailable) {
                                                    JsonData j = new JsonData();
                                                    j["command"] = "OnSuccessAnswer";
                                                    j["sdp"] = sdp;
                                                    context.Send(j.ToJson());
                                                }
                                            };

                                            session.WebRtc.OnFailure += delegate (string error) {
                                                Trace.WriteLine($"OnFailure: {error}");
                                            };

                                            session.WebRtc.OnError += delegate {
                                                Trace.WriteLine("OnError");
                                            };

                                            session.WebRtc.OnDataMessage += delegate (string dmsg) {
                                                Trace.WriteLine($"OnDataMessage: {dmsg}");
                                            };

                                            session.WebRtc.OnDataBinaryMessage += delegate (byte[] dmsg) {
                                                Trace.WriteLine($"OnDataBinaryMessage: {dmsg.Length}");
                                            };

                                            unsafe {
                                                session.WebRtc.OnRenderRemote += delegate (byte* frame_buffer, uint w, uint h) {
                                                    OnRenderFrame(frame_buffer, w, h);
                                                };

                                            }

                                            var d = msgJson["desc"];
                                            var s = d["sdp"].ToString();

                                            session.WebRtc.OnOfferRequest(s);
                                        }
                                    }
                                }
                            }
                        }
                        break;

                    case onicecandidate: {
                            var c = msgJson["candidate"];

                            var sdpMLineIndex = (int)c["sdpMLineIndex"];
                            var sdpMid = c["sdpMid"].ToString();
                            var candidate = c["candidate"].ToString();

                            var session = Streams[context.ConnectionInfo.Id];
                            {
                                session.WebRtc.AddIceCandidate(sdpMid, sdpMLineIndex, candidate);
                            }
                        }
                        break;
                }
            }
        }

        //public ManagedConductor.OnCallbackRender OnRenderFrame;

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
            //var img = new Bitmap(frame);
            var l = img.LockBits(new Rectangle(Point.Empty, img.Size), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            try {
                foreach (var s in this.Streams) {

                    unsafe {
                        var yuv = s.Value.WebRtc.VideoCapturerI420Buffer();
                        if (yuv != null) {
                            encoderRemote.EncodeI420((byte*)l.Scan0, img.Width, img.Height - 1, (int)TJPF.TJPF_BGR, 0, true, yuv);
                        }
                    }

                    s.Value.WebRtc.PushFrame();
                }
            } catch (Exception) {
            } finally {
                img.UnlockBits(l);

            }
        }

        public void Dispose() {
            try {
                foreach (var s in Streams) {
                    if (!s.Value.Cancel.IsCancellationRequested) {
                        s.Value.Cancel.Cancel();
                    }
                }

                foreach (IWebSocketConnection i in UserList.Values) {
                    i.Close();
                }

                server.Dispose();
                UserList.Clear();
                Streams.Clear();
            } catch { }
        }
    }

}
