using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Linq;
using System.Text;

namespace YZ {

    public static partial class Helpers {
        public static string Md5(this string s) {
            var md5 = System.Security.Cryptography.MD5.Create();
            return md5.ComputeHash(Encoding.UTF8.GetBytes(s)).ToString(format: "x2");
        }

        public static string GeneratePassword() {
            var rnd = new Random();
            var data = new byte[6];
            rnd.NextBytes(data);
            return System.Convert.ToBase64String(data);
        }
    }

    public class Crypt {

        static Random rnd = new Random();

        static byte[] alphabet = 128.Generate(t => (byte)(t + 32));

        static byte[] getKey() {
            alphabet = alphabet.Shuffle().ToArray();
            return alphabet.Shuffle().Take(16).ToArray();
        }
        public static string GenerateKey(int deep = 3) => Encoding.ASCII.GetString(deep.Constraint(1, 10).Generate(x => getKey()).SelectMany(t => t).ToArray());

        byte[][] multiKey, multiKeyReversed;

        void setKey(byte[] key) {
            multiKey = key.SplitBy(16).Where(t => t.Count() == 16).Take(10).Select(t => t.ToArray()).ToArray();
            multiKeyReversed = multiKey.Reverse().ToArray();
        }

        /// <summary>
        /// Security binary Key encoded to Base64
        /// Length is varying depends on initial encryption level
        /// </summary>
        public string Key {
            get => System.Convert.ToBase64String(multiKey.SelectMany(t => t).ToArray());
            set => setKey(System.Convert.FromBase64String(value));
        }

        /// <summary>
        /// Creates new instance with default level of encryption (2 levels by default)
        /// </summary>
        public Crypt() : this(2) { }

        /// <summary>
        /// Creates new instance with desired level of encryption
        /// </summary>
        /// <param name="deep">Desired number of levels of encryption</param>
        public Crypt(int deep) : this(deep.Constraint(1, 10).Generate(x => getKey()).SelectMany(t => t).ToArray()) { }
        /// <summary>
        /// Creates new instance using predefined Key (should be valid Key, generated previously by another instance)
        /// </summary>
        /// <param name="key"></param>
        public Crypt(string key) { Key = key; }
        Crypt(byte[] key) { setKey(key); }

        static byte[] generateNoise(byte value, byte[] noise) {
            var cnt = rnd.Next(0, 6);
            var l = noise.Length;
            if (cnt == 0) return new[] { value };
            var res = new byte[cnt];
            res[0] = value;
            for (var i = 1; i < cnt; i++) res[i] = noise[rnd.Next(0, l)];
            return res;
        }
        static byte[] addNoise(byte[] data, byte[] noise) {
            var res = data.SelectMany(t => generateNoise(t, noise)).ToArray();
            return (rnd.NextDouble() > 0.5) ? res : generateNoise(noise[rnd.Next(0, noise.Length)], noise).Concat(res).ToArray();
        }
        static byte[] removeNoise(byte[] data, byte[] key) => data.Join(key, t => t, t => t, (a, b) => a).ToArray();

        static byte[] encode(byte[] data, byte[] key) {
            if (data.Length == 0 || key.Length != 16) return Array.Empty<byte>();
            if (!(key?.Count() == 16)) return data;
            var res = new byte[data.Length * 2];
            for (var i = 0; i < data.Length; i++) {
                int hi = (data[i] >> 4) & 15, lo = data[i] & 15;
                res[i * 2] = key[(hi + i * 2) % 16];
                res[i * 2 + 1] = key[(lo + i * 2 + 1) % 16]; ;
            }
            return addNoise(res, alphabet.Except(key).ToArray());
        }
        static byte[] decode(byte[] data, byte[] key) {
            if (data.Length == 0) return Array.Empty<byte>();
            if (!(key?.Count() == 16)) return data;
            data = removeNoise(data, key);

            byte subMod(int a, int b, int m) => (byte)((a + m - (b % m)) % m);

            var res = new byte[data.Length / 2];
            for (var i = 0; i < res.Length; i++) {
                byte hi = subMod(Array.IndexOf(key, (data[i * 2])), i * 2, 16), lo = subMod(Array.IndexOf(key, (data[i * 2 + 1])), i * 2 + 1, 16);
                res[i] = (byte)((hi << 4) | lo);
            }
            return res;

        }
        /// <summary>
        /// Encrypts binary data 
        /// </summary>
        /// <param name="data">Source binary data to be encrypted</param>
        /// <returns>Encrypted data</returns>
        public byte[] Encrypt(byte[] data) => multiKey.Aggregate(data, encode);
        /// <summary>
        /// Encrypts UTF8 string data 
        /// </summary>
        /// <param name="data">Source UTF8 string  to be encrypted</param>
        /// <returns>Encrypted binary data</returns>
        public byte[] Encrypt(string data) => Encrypt(Encoding.UTF8.GetBytes(data));
        /// <summary>
        /// Encrypts UTF8 string data and returns result as ASCII string
        /// </summary>
        /// <param name="data">Source UTF8 string  to be encrypted</param>
        /// <returns>Encrypted binary data</returns>
        public string EncryptString(string data) => Encoding.ASCII.GetString(Encrypt(Encoding.UTF8.GetBytes(data)));
        /// <summary>
        /// Decrypts binary encrypted raw data
        /// </summary>
        /// <param name="data">Binary encrypted raw data</param>
        /// <returns>Decrypted data</returns>
        public byte[] Decrypt(byte[] data) => multiKeyReversed.Aggregate(data, decode);
        /// <summary>
        /// Decrypts binary encrypted raw data and returns result as UTF8 string
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string DecryptString(byte[] data) => Encoding.UTF8.GetString(Decrypt(data));
        /// <summary>
        /// Decrypts encrypted UTF8 string and returns result as UTF8 string
        /// </summary>
        /// <param name="data">UTF8-string</param>
        /// <returns>decoded data as UTF8 string</returns>
        public string DecryptString(string data) => DecryptString(Encoding.UTF8.GetBytes(data));
    }

}