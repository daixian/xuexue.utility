using System.IO;
using System.Text;

namespace xuexue.crypto
{
    /// <summary>
    /// 加密文件定义，增加一个文件头一个md5文本一个info
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

        ///-------------------------------------------------------------------------------------------------
        /// <summary> 解密一个文件流. </summary>
        ///
        /// <remarks> Surface, 2018/9/18. </remarks>
        ///
        /// <param name="enStream"> 加密文件的流，这个流从0位置到开始都是加密文件数据，包括前面的info信息. </param>
        /// <param name="deStream"> 解密流，原始数据流（这个流从0位置开始都是有效的原始数据）. </param>
        /// <param name="key">      The key. </param>
        /// <param name="blockLen"> (Optional) 加密buffer长度. </param>
        ///
        /// <returns> A CryptoFile. </returns>
        ///-------------------------------------------------------------------------------------------------
        public static CryptoFile Decrypt(Stream enStream, Stream deStream, byte[] key, int blockLen = 4096)
        {
            CryptoFile cfile = new CryptoFile() { isCryptoFile = true, enStream = enStream, deStream = deStream };
            byte[] byheader = Encoding.UTF8.GetBytes(cfile.header);
            enStream.Position = 0;
            BinaryReader br = new BinaryReader(enStream);
            //验证文件头是否是加密文件
            for (int i = 0; i < byheader.Length; i++)
            {
                if (br.ReadByte() != byheader[i])
                {
                    cfile.isCryptoFile = false;
                    break;
                }
            }
            if (cfile.isCryptoFile == false)
            {
                return cfile;
            }

            cfile.md5 = br.ReadString();
            cfile.info = br.ReadString();

            cfile.deStream.Position = 0;
            Crypto.AESDecrypt(enStream, cfile.deStream, key, blockLen);

            return cfile;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary> 加密一个文件流. </summary>
        ///
        /// <remarks> Surface, 2018/9/18. </remarks>
        ///
        /// <param name="oriStream"> 原始文件流. </param>
        /// <param name="info">      一些可以被记录的其他附加信息. </param>
        /// <param name="enStream">  加密文件的流，这个流从0位置到开始都是加密文件数据，包括前面的info信息。. </param>
        /// <param name="key">       The key. </param>
        /// <param name="blockLen">  (Optional) 加密buffer长度. </param>
        ///
        /// <returns> A CryptoFile. </returns>
        ///-------------------------------------------------------------------------------------------------
        public static CryptoFile Encrypt(Stream oriStream, string info, Stream enStream, byte[] key, int blockLen = 4096)
        {
            CryptoFile cfile = new CryptoFile() { isCryptoFile = true };
            cfile.md5 = xuexue.file.MD5Helper.MD5(oriStream);
            cfile.info = info;
            cfile.enStream = enStream;//设置加密流
            cfile.deStream = oriStream;//原始数据流就是解密流
            cfile.deStream.Position = 0;

            BinaryWriter bw = new BinaryWriter(cfile.enStream);
            bw.Write(Encoding.UTF8.GetBytes(cfile.header));//先写一个文件头,这样写不带长度
            bw.Write(cfile.md5);//写一个md5信息，这个自带string长度
            bw.Write(cfile.info);//写一个info信息，这个自带string长度

            bw.Flush();

            //使用原始文件流，原始文件流的长度
            Crypto.AESEncrypt(cfile.deStream, cfile.deStream.Length, enStream, key);
            return cfile;
        }
    }
}