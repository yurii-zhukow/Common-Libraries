using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace YZ {

    public static partial class Helpers {
      
        public unsafe static void Yuv12ToRgb(IntPtr srcPtr, int w, int h, ref byte[] dst, out int stride) {

            byte* src = (byte*)srcPtr;
            stride = w * 3;
            var strideOffs = (4 - (stride % 4)) % 4;
            stride += strideOffs;
            var sz = stride * h;
            if (dst == null || dst.Length < sz) dst = new byte[sz];

            //for (int i = 0, j = 0; j < nSize; i += 12, j += 4) {
            //    tempFrameBuf[i + 0] = (byte)(ptr[j] + ptr[j + 3] * ((1 - 0.299) / 0.615));
            //    tempFrameBuf[i + 1] = (byte)(ptr[j] - ptr[j + 1] * ((0.114 * (1 - 0.114)) / (0.436 * 0.587)) - ptr[j + 3] * ((0.299 * (1 - 0.299)) / (0.615 * 0.587)));
            //    tempFrameBuf[i + 2] = (byte)(ptr[j] + ptr[j + 1] * ((1 - 0.114) / 0.436));
            //    tempFrameBuf[i + 3] = (byte)(ptr[j + 2] + ptr[j + 3] * ((1 - 0.299) / 0.615));
            //    tempFrameBuf[i + 4] = (byte)(ptr[j + 2] - ptr[j + 1] * ((0.114 * (1 - 0.114)) / (0.436 * 0.587)) - ptr[j + 3] * ((0.299 * (1 - 0.299)) / (0.615 * 0.587)));
            //    tempFrameBuf[i + 5] = (byte)(ptr[j + 2] + ptr[j + 1] * ((1 - 0.114) / 0.436));
            //}

            int w3 = w * 3;
            int offsSrc = 0, offsDst = 0;
            byte gr;

            for (int y = 0; y < h; y++) {
                for (int x = 0; x < w; x++) {

                    //var cy = ptr[offsSrc++];
                    //var cu = ptr[offsSrc++];
                    //var cv = ptr[offsSrc++];

                    //byte cr = (byte)(cy + 1.4075 * (cv - 128));
                    //byte cg = (byte)(cy - 0.3455 * (cu - 128) - (0.7169 * (cv - 128)));
                    //byte cb = (byte)(cy + 1.7790 * (cu - 128));

                    gr = src[offsSrc++];
                    dst[offsDst + 0] = gr;
                    //dst[offsDst + 1] = gr;
                    //dst[offsDst + 2] = gr;
                    offsDst += 3;
                }
                for (int z = 0; z < strideOffs; z++) {
                    dst[offsDst++] = 0;
                }
            }

            offsDst = 0;
            for (int y = 0; y < h / 2; y++) {
                for (int x = 0; x < w / 2; x++) {

                    gr = src[offsSrc++];

                    dst[offsDst + 1] = gr;
                    dst[offsDst + 4] = gr;
                    dst[offsDst + w3 + 1] = gr;
                    dst[offsDst + w3 + 4] = gr;
                    offsDst += 6;
                }
                offsDst += strideOffs;
                offsDst += w3;
            }

            offsDst = 0;

            for (int y = 0; y < h / 2; y++) {
                for (int x = 0; x < w / 2; x++) {

                    gr = src[offsSrc++];

                    dst[offsDst + 2] = gr;
                    dst[offsDst + 5] = gr;
                    dst[offsDst + w3 + 2] = gr;
                    dst[offsDst + w3 + 5] = gr;
                    offsDst += 6;
                }
                offsDst += strideOffs;

                offsDst += w3;
            }


            byte Y, U, V, R, G, B;
            int C, D, E;
            offsDst = 0;
            const byte b255 = (byte)255;
            const byte b0 = (byte)0;
            byte clip(int src) => src < 0 ? b0 : src > 255 ? b255 : (byte)src;

            for (int y = 0; y < h; y++) {
                for (int x = 0; x < w; x++) {

                    Y = dst[offsDst + 0];
                    U = dst[offsDst + 1];
                    V = dst[offsDst + 2];

                    C = Y - 16;
                    D = U - 128;
                    E = V - 128;

                    R = clip((298 * C + 409 * E + 128) >> 8);
                    G = clip((298 * C - 100 * D - 208 * E + 128) >> 8);
                    B = clip((298 * C + 516 * D + 128) >> 8);

                    dst[offsDst + 0] = R;
                    dst[offsDst + 1] = G;
                    dst[offsDst + 2] = B;
                    offsDst += 3;
                }
                offsDst += strideOffs;
            }


        }

    }

}