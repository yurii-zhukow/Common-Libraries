﻿using System;
using System.Runtime.InteropServices;

namespace SDK.ffMpeg {
    public static class LinuxNativeMethods
    {
        public const int RTLD_NOW = 0x002;

        private const string Libdl = "libdl.so.2";

        [DllImport(Libdl)]
        public static extern IntPtr dlsym(IntPtr handle, string symbol);

        [DllImport(Libdl)]
        public static extern IntPtr dlopen(string fileName, int flag);
    }
}