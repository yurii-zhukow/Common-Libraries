using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;



using Microsoft.Win32;


namespace YZ {
    public static partial class Helpers {


        public static T GetRegistryValue<T>(string path, string param, T deflt, bool createIfNotExist) {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return deflt;

            T read(RegistryKey root) {
                using (var key = root.OpenSubKey(path)) {
                    var res = (T)key.GetValue(param);
                    if (res == null) throw new NullReferenceException($"Registry path={path} key={param} cannot be NULL");
                    return res;
                }
            }

            try {
                return read(Registry.LocalMachine);
            }
            catch {
                try {
                    return read(Registry.CurrentUser);
                }
                catch {
                    if (createIfNotExist) SetRegistryValue<T>(path, param, deflt);
                }
            }
            return deflt;
        }

        public static bool SetRegistryValue<T>(string path, string param, T value) {

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return false;

            bool write(RegistryKey root) {
                using (var key = root.CreateSubKey(path)) key.SetValue(param, value);
                return true;
            }

            try {
                return write(Registry.LocalMachine);
            }
            catch {
                try {
                    return write(Registry.CurrentUser);
                }
                catch { }
            }
            return false;
        }

    }
}
