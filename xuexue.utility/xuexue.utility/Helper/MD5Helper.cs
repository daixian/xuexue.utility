using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace xuexue.file
{
    public static class MD5Helper
    {
        /// <summary>
        /// 计算一个流的md5，不能设置起始和结束,从起始计算到结束
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string MD5(Stream stream)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            stream.Position = 0;
            byte[] retVal = md5.ComputeHash(stream);
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte b in retVal)
            {
                stringBuilder.Append(b.ToString("x2"));
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 计算一个文件的MD5,返回文本值。
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>MD5的文本值</returns>
        public static string FileMD5(string filePath)
        {
            FileInfo fi = new FileInfo(filePath);
            if (!File.Exists(fi.FullName))
            {
                Log.Warning("MD5Helper.FileMD5():输入的文件不存在,输出空文件MD5。filePath = " + filePath);
                return "d41d8cd98f00b204e9800998ecf8427e";//当前计算的空文件的md5
            }

            byte[] retVal;
            using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                if (file.Length == 0)
                {
                    Log.Warning("MD5Helper.FileMD5():输入的是一个空文件,filePath = " + filePath);
                }

                file.Seek(0, SeekOrigin.Begin);
                MD5 md5 = new MD5CryptoServiceProvider();
                retVal = md5.ComputeHash(file);
            }

            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte b in retVal)
            {
                stringBuilder.Append(b.ToString("x2"));
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 由一个文件比对MD5值是否正确
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="oriMD5">正确的MD5</param>
        /// <param name="oriFileSize">正确的文件大小</param>
        /// <returns>是否正确</returns>
        public static bool Compare(string filePath, string oriMD5, long oriFileSize)
        {
            FileInfo fi = new FileInfo(filePath);
            if (!File.Exists(fi.FullName))
            {
                return false;
            }

            byte[] retVal;
            using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                if (file.Length == 0 && oriFileSize == 0)
                {
                    return true;
                }
                if (file.Length != oriFileSize)//如果文件长度就不等，那么直接返回
                {
                    return false;
                }

                file.Seek(0, SeekOrigin.Begin);
                MD5 md5 = new MD5CryptoServiceProvider();
                retVal = md5.ComputeHash(file);
            }

            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte b in retVal)
            {
                stringBuilder.Append(b.ToString("x2"));
            }
            string newMD5 = stringBuilder.ToString();

            //最后再比对md5值
            return oriMD5 == newMD5;
        }
    }
}