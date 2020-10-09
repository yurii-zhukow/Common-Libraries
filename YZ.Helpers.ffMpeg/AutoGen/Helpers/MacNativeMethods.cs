using System;
using System.Runtime.InteropServices;

namespace SDK.FFMpeg {
    public static class MacNativeMethods
    {
        public const int RTLD_NOW = 0x002;

        private const string Libdl = "libdl";

        [DllImport(Libdl)]
        public static extern IntPtr dlsym(IntPtr handle, string symbol);

        [DllImport(Libdl)]
        public static extern IntPtr dlopen(string fileName, int flag);
    }
}