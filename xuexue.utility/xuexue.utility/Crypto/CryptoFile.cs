using System.IO;
using System.Text;

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
        /// 加密文件的流，这个流从0位置到开始都是加密文件数据，包括前面的info信息。
        /// </summary>
        public Stream enStream;

        /// <summary>
        /// 解密流，原始数据流（这个流从0位置开始都是有效的原始数据）
        /// </summary>
        public Stream deStream;

        /// <summary>
        /// 解密一个文件
        /// </summary>
        public static CryptoFile Decrypt(string cfFilePath, Stream deStream)
        {
            if (!File.Exists(cfFilePath))//如果传进来的文件不存在那么就直接退出
            {
                return null;
            }

            return null;
        }

        /// <summary>
        /// 输入一个原始文件路径，初始化整个类成员。加密数据流就默认是一个MemoryStream。
        /// 这个函数之后要调用MakeCFStream()函数去完成加密。
        /// </summary>
        /// <param name="oriPath"></param>
        /// <returns></returns>
        public static CryptoFile Encrypt(string oriFilePath, string info, Stream enStream, byte[] key, int blockLen = 4096)
        {
            if (!File.Exists(oriFilePath))//如果传进来的文件不存在那么就直接退出
            {
                return null;
            }

            CryptoFile cfile = new CryptoFile();
            cfile.md5 = xuexue.file.MD5Helper.FileMD5(oriFilePath);
            cfile.info = info;
            cfile.enStream = enStream;//设置加密流
            cfile.deStream = new FileStream(oriFilePath, FileMode.Open);//原始数据流就是解密流，读文件

            StreamWriter sw = new StreamWriter(cfile.enStream);
            sw.WriteLine(cfile.header);//先写一个文件头(使用一行)

            byte[] bymd5 = Encoding.UTF8.GetBytes(cfile.md5);
            byte[] byInfo = Encoding.UTF8.GetBytes(cfile.info);
            int headerLen = bymd5.Length + byInfo.Length;
            sw.Write(headerLen);//写一个头的总长度.

            sw.Write(bymd5.Length);//写一个md5信息
            sw.Write(bymd5);

            sw.Write(byInfo.Length);//写一个info信息
            sw.Write(byInfo);
            sw.Flush();

            //enStream.Position = 0;
            //使用原始文件流，原始文件流的长度
            Crypto.AESEncrypt(cfile.deStream, cfile.deStream.Length, enStream, key);

            return null;
        }
    }
}