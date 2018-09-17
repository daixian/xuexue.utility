using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xuexue.crypto
{
    /// <summary>
    /// 加密文件定义
    /// </summary>
    public class CryptoFile
    {

        /// <summary>
        /// 一个已经加密的文件路径
        /// </summary>
        public string filePath;

        /// <summary>
        /// 是否成功的被识别为加密文件
        /// </summary>
        public bool isCryptoFile;

        /// <summary>
        /// 加密文件头
        /// </summary>
        public string header = "xxcf:aes";

        /// <summary>
        /// 原始文件的md5值
        /// </summary>
        public string md5;

        /// <summary>
        /// 一些可以被记录的其他附加信息
        /// </summary>
        public string info = "";

        /// <summary>
        /// 原始数据流（这个流从0位置开始都是有效的原始数据）
        /// </summary>
        public Stream oriStream;

        /// <summary>
        /// 这个整个加密文件的流（这个流从0位置到开始都是有效的加密文件数据）
        /// </summary>
        public Stream cfStream;

        /// <summary>
        /// 进行一个初步的初始化，主要判断文件是不是已知的加密文件，解密数据流就默认是一个MemoryStream。
        /// </summary>
        public bool initWithCFPath(string cfPath)
        {
            if (!File.Exists(cfPath))//如果传进来的文件不存在那么就直接退出
            {
                return false;
            }





            return true;
        }

        /// <summary>
        /// 输入一个原始文件路径，初始化整个类成员。加密数据流就默认是一个MemoryStream。
        /// </summary>
        /// <param name="oriPath"></param>
        /// <returns></returns>
        public bool initWithOriPath(string oriPath)
        {
            if (!File.Exists(oriPath))//如果传进来的文件不存在那么就直接退出
            {
                return false;
            }
            md5 = xuexue.file.MD5Helper.FileMD5(oriPath);
            oriStream = new FileStream(oriPath, FileMode.Open);

            return true;
        }

        /// <summary>
        /// 使用当前的成员来构造加密流，调用完毕之后整个加密流就可以随便使用了。这个函数不会关闭加密流。
        /// </summary>
        public void MakeCFStream(byte[] key)
        {
            cfStream.Position = 0;
            StreamWriter sw = new StreamWriter(cfStream);
            sw.WriteLine(header);//先写一个文件头(使用一行)

            byte[] bymd5 = Encoding.UTF8.GetBytes(md5);
            byte[] byInfo = Encoding.UTF8.GetBytes(info);
            int headerLen = bymd5.Length + byInfo.Length;
            sw.Write(headerLen);//写一个头的总长度.

            sw.Write(bymd5.Length);//写一个md5信息
            sw.Write(bymd5);

            sw.Write(byInfo.Length);//写一个info信息
            sw.Write(byInfo);
            sw.Flush();

            oriStream.Position = 0;
            Crypto.AESEncrypt(oriStream, oriStream.Length, cfStream, key);

        }

    }
}
