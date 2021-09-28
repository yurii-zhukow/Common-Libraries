using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YZ.Tests {
    [TestClass]
    public class EncryptionTest {
        [TestMethod]
        public void TestEncryptDecrypt() {
            var test = new Crypt(3);
            var v = "abc Hello, 555";
            var encrypted = test.Encrypt(v);
            var decrypted = test.DecryptString(encrypted);
            Assert.IsTrue(decrypted == v);
        }
    }
}
