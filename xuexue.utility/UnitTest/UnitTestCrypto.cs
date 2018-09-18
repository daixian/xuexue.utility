using xuexue.file;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using xuexue.LitJson;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using System;
using xuexue.crypto;

namespace UnitTest
{
    [TestClass]
    public class UnitTestCrypto
    {
        [TestMethod]
        public void TestMethod_Crypto()
        {
            //要加密的原始数据
            byte[] data = new byte[1024 * 1024 + 7];//假设有一个1M大的文件
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (byte)(i % 256);
            }

            //使用xuexue这个文本来随便生成一个密钥
            MemoryStream msKey = new MemoryStream();
            StreamWriter sw = new StreamWriter(msKey);
            while (msKey.Length < 16)
            {
                sw.Write("xuexue");
                sw.Flush();
            }
            msKey.SetLength(16);

            byte[] key = msKey.ToArray();//生成一个16字节的密钥


            MemoryStream msIn = new MemoryStream(data);//使用原始数据生成一个流
            MemoryStream msAES = new MemoryStream();//加密后的数据
            int elen = Crypto.AESEncrypt(msIn, data.Length, msAES, key);

            msAES.Position = 0;//把加密后的数据流置回头

            MemoryStream msEDResult = new MemoryStream();//解密的结果流
            int dlen = Crypto.AESDecrypt(msAES, msEDResult, key);

            byte[] dataEDResult = msEDResult.ToArray();

            //确认结果和原先是否一致
            Assert.IsTrue(data.Length == dataEDResult.Length);
            for (int i = 0; i < data.Length; i++)
            {
                Assert.IsTrue(data[i] == dataEDResult[i]);
            }

        }


        [TestMethod]
        public void TestMethod_CryptoFile()
        {
            //要加密的原始数据
            byte[] data = new byte[1024 * 1024 + 7];//假设有一个1M大的文件
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (byte)(i % 256);
            }

            //使用xuexue这个文本来随便生成一个密钥
            MemoryStream msKey = new MemoryStream();
            StreamWriter sw = new StreamWriter(msKey);
            while (msKey.Length < 16)
            {
                sw.Write("xuexue");
                sw.Flush();
            }
            msKey.SetLength(16);

            byte[] key = msKey.ToArray();//生成一个16字节的密钥


            MemoryStream msIn = new MemoryStream(data);//使用原始数据生成一个流
            MemoryStream msAES = new MemoryStream();//加密后的数据
            CryptoFile cfFile1= CryptoFile.Encrypt(msIn, "加密测试123", msAES, key);

            msAES.Position = 0;//把加密后的数据流置回头

            MemoryStream msEDResult = new MemoryStream();//解密的结果流
            CryptoFile cfFile = CryptoFile.Decrypt(msAES, msEDResult, key);

            byte[] dataEDResult = msEDResult.ToArray();

            //确认结果和原先是否一致
            Assert.IsTrue(data.Length == dataEDResult.Length);
            for (int i = 0; i < data.Length; i++)
            {
                Assert.IsTrue(data[i] == dataEDResult[i]);
            }

            Assert.IsTrue(cfFile1.info == cfFile.info);
            Assert.IsTrue(cfFile1.md5 == cfFile.md5);
        }
    }
}
