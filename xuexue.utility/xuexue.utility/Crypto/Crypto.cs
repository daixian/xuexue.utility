using System;
using System.IO;
using System.Security.Cryptography;

namespace xuexue.crypto
{
    /// <summary>
    /// 加密解密
    /// </summary>
    public class Crypto
    {
        /// <summary>
        /// 默认密钥向量
        /// </summary>
        private static byte[] _IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF, 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

        /// <summary>
        /// AES加密，传入一个文件流（从当前流位置开始工作），和之后要读取的流长度去阻塞的生成加密流。密钥长度为16字节
        /// 产生的输出流是：一个int的原始文件长度 + 加密流。
        /// </summary>
        /// <param name="inStream">输入流</param>
        /// <param name="length">从输入流里读取的长度</param>
        /// <param name="outStream">输出结果流</param>
        /// <param name="key">密钥（16字节）</param>
        /// <param name="blockLen">(Optional)加密时的块长度</param>
        /// <returns></returns>
        public static int AESEncrypt(Stream inStream, long length, Stream outStream, byte[] key, int blockLen = 4096)
        {
            //分组加密算法
            SymmetricAlgorithm des = Rijndael.Create();
            des.Key = key;//设置密钥及密钥向量
            des.IV = _IV;
            CryptoStream cs = new CryptoStream(outStream, des.CreateEncryptor(), CryptoStreamMode.Write);

            //outStream.Position = 0;//输出结果流没有太多必要去设置为0
            outStream.Write(BitConverter.GetBytes(length), 0, sizeof(int));//写入一个整个文件长度

            byte[] inputBuffer = new byte[blockLen];//数据buffer
            int lenRead = 0;//已读输入数据长度

            while (true)
            {
                bool isEnd = false;

                int wantReadLen = blockLen;//希望读取的长度
                if (lenRead + blockLen >= length)//这是最后一次读取
                {
                    wantReadLen = (int)length - lenRead;
                }

                int len = inStream.Read(inputBuffer, 0, wantReadLen);
                lenRead += len;//已读输入数据长度增加

                if (len < wantReadLen)
                {
                    isEnd = true;
                }

                cs.Write(inputBuffer, 0, len);
                if (lenRead == length)
                {
                    break;
                }
                if (isEnd)
                {
                    break;
                }
            }
            cs.FlushFinalBlock();

            //将来CryptoStream考虑Clear(),现在clear之后会关闭流，引起使用的不方便，最好考虑把包装器传出去

            return lenRead;
        }

        /// <summary>
        /// AES解密，将AESEncrypt的结果作为输入传入，输出一个解密后的流。.
        /// </summary>
        /// <param name="inStream">  加密过的整个流. </param>
        /// <param name="outStream"> 输出解密结果流. </param>
        /// <param name="key">       密钥. </param>
        /// <param name="blockLen">  (Optional)解密时的块长度</param>
        /// <returns> An int. </returns>
        public static int AESDecrypt(Stream inStream, Stream outStream, byte[] key, int blockLen = 4096)
        {
            SymmetricAlgorithm des = Rijndael.Create();
            des.Key = key;
            des.IV = _IV;
            CryptoStream cs = new CryptoStream(inStream, des.CreateDecryptor(), CryptoStreamMode.Read);

            //首先读一个文件总长有效数据长度
            byte[] lenBuffer = new byte[sizeof(int)];
            inStream.Read(lenBuffer, 0, lenBuffer.Length);
            int filelen = BitConverter.ToInt32(lenBuffer, 0);

            byte[] decryptBytes = new byte[blockLen];//解密后的原始数据

            int lenWrite = 0;//解密出来的有效数据的总长度
            while (true)
            {
                int decryptLen = cs.Read(decryptBytes, 0, blockLen);//成功解密出来的长度
                lenWrite += decryptLen;
                cs.Flush();
                outStream.Write(decryptBytes, 0, decryptLen);

                if (lenWrite >= filelen)
                {
                    break;
                }
            }
            outStream.Flush();

            //将来CryptoStream考虑Clear(),现在clear之后会关闭流，引起使用的不方便，最好考虑把包装器传出去

            return lenWrite;
        }
    }
}