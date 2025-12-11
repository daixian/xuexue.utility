using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace xuexue.utility
{
    public static class FileUtils
    {
        /// <summary>
        /// 获取文件大小
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        public static long GetFileSize(this FileInfo fileInfo)
        {
            return fileInfo.Length;
        }

        /// <summary>
        /// 在windows下有时候需要先整个读取一次文件让这个文件缓存,1MB缓存读取文件并返回总读取字节数 .
        /// </summary>
        public static long ReadFileWith1MBuffer(string filePath)
        {
            // DNET的池最大只有64K的缓存,所以这里就不用池了
            const int bufferSize = 1024 * 1024; // 1MB
            byte[] buffer = new byte[bufferSize];
            long totalBytesRead = 0;

            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                int bytesRead;
                while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0) {
                    totalBytesRead += bytesRead;
                    // 你可以在这里处理 buffer 中的内容，比如解析、存储、分析等
                }
            }
            return totalBytesRead;
        }

        /// <summary>
        /// 计算文件的SHA-256哈希值
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        public static string GetSHA256(this FileInfo fileInfo)
        {
            if (!fileInfo.Exists) {
                // byte[] hashBytes = sha256.ComputeHash(Array.Empty<byte>());
                // 文件不存在,返回空的SHA-256哈希值.
                return "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855";
            }
            // 如果文件存在,则计算SHA-256哈希值,如果是空文件那么会返回和文件不存在一样的值.
            using (SHA256 sha256 = SHA256.Create()) {
                // 打开文件以读取，使用只读模式
                using (FileStream fs = new FileStream(fileInfo.FullName,
                           FileMode.Open, FileAccess.Read, FileShare.Read,
                           bufferSize: 1024 * 1024)) {
                    byte[] hashBytes = sha256.ComputeHash(fs);
                    StringBuilder hashStringBuilder = new StringBuilder();
                    foreach (byte b in hashBytes) {
                        hashStringBuilder.Append(b.ToString("x2")); // 使用小写的十六进制表示
                    }
                    return hashStringBuilder.ToString();
                }
            }
        }

        /// <summary>
        /// 计算文件的 MD5 哈希值
        /// </summary>
        /// <param name="fileInfo">要计算哈希值的文件信息</param>
        /// <returns>文件的 MD5 哈希值的十六进制字符串表示</returns>
        public static string GetMD5(this FileInfo fileInfo)
        {
            if (!fileInfo.Exists) {
                // 文件不存在，返回空文件的 MD5 哈希值（16 个零字节的哈希）
                return "d41d8cd98f00b204e9800998ecf8427e";
            }

            using (MD5 md5 = MD5.Create()) {
                using (FileStream fs = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                    byte[] hashBytes = md5.ComputeHash(fs);
                    StringBuilder hashStringBuilder = new StringBuilder();
                    foreach (byte b in hashBytes) {
                        hashStringBuilder.Append(b.ToString("x2")); // 使用小写的十六进制表示
                    }
                    return hashStringBuilder.ToString();
                }
            }
        }

        /// <summary>
        /// 计算文件的 MD5 哈希值并返回字节数组(16字节)。
        /// </summary>
        /// <param name="fileInfo">要计算哈希值的文件信息。</param>
        /// <returns>文件的 MD5 哈希值的字节数组。</returns>
        public static byte[] GetMD5Bytes(this FileInfo fileInfo)
        {
            using (MD5 md5 = MD5.Create()) {
                if (!fileInfo.Exists) {
                    return md5.ComputeHash(Array.Empty<byte>()); //对应于MD5 的空输入哈希值
                }
                using (FileStream fs = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                    byte[] result = md5.ComputeHash(fs);
                    if (result.Length != 16) {
                        Debug.LogError("GetMD5Bytes():MD5的长度不是16字节!");
                    }
                    return result;
                }
            }
        }

        /// <summary>
        /// 确保文件夹存在.
        /// </summary>
        /// <param name="directoryPath"></param>
        public static string EnsureDirectoryExists(this string directoryPath)
        {
            // 加上异常处理
            try {
                if (!Directory.Exists(directoryPath)) {
                    Directory.CreateDirectory(directoryPath);
                }
            } catch (Exception e) {
                Debug.LogError($"Utils.EnsureDirectoryExists():创建文件夹{directoryPath}失败: {e.Message}");
            }
            return directoryPath;
        }

        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <returns></returns>
        public static string DeleteDirectory(this string directoryPath)
        {
            // 加上异常处理
            try {
                if (Directory.Exists(directoryPath)) {
                    Directory.Delete(directoryPath);
                }
            } catch (Exception e) {
                Debug.LogError($"Utils.EnsureDirectoryExists():删除文件夹{directoryPath}失败: {e.Message}");
            }
            return directoryPath;
        }

        /// <summary>
        /// 确保文件所在的文件夹存在.
        /// </summary>
        /// <param name="filePath"></param>
        public static void EnsureFileDirectoryExists(this string filePath)
        {
            // 加上异常处理
            try {
                string directoryPath = Path.GetDirectoryName(filePath);
                if (directoryPath == null) {
                    Debug.LogError($"Utils.EnsureDirectoryExists():创建{filePath}的父文件夹失败");
                    return;
                }

                if (!Directory.Exists(directoryPath)) {
                    Directory.CreateDirectory(directoryPath);
                }
            } catch (Exception e) {
                Debug.LogError($"Utils.EnsureDirectoryExists():创建{filePath}的父文件夹失败: {e.Message}");
            }
        }

        /// <summary>
        /// 验证文件名是否符合规范
        /// </summary>
        /// <param name="fileName">要验证的文件名</param>
        /// <returns>文件名是否符合规范</returns>
        public static bool IsValidFileName(this string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) {
                return false;
            }

            if (fileName.Length > 255) {
                return false;
            }

            char[] invalidChars = { '\\', '/', ':', '*', '?', '"', '<', '>', '|' };
            foreach (char c in invalidChars) {
                if (fileName.Contains(c)) {
                    return false;
                }
            }
            return true;
        }
    }
}
