using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace xuexue.utility
{
    /// <summary>
    /// 各种各样的扩展方法
    /// </summary>
    public static class ExtensionMethod
    {
        /// <summary>
        /// 字节数组转成十六进制的字符串,默认使用大写字母
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="isToUpper">是否是大写的</param>
        /// <returns></returns>
        public static string ToHex(this byte[] bytes, bool isToUpper = true)
        {
            StringBuilder sb = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
            {
                if (isToUpper)
                    sb.Append(b.ToString("X2"));//大写
                else
                    sb.Append(b.ToString("x2"));//小写
            }
            return sb.ToString();
        }

        /// <summary>
        /// 计算SHA256,发生错误则返回null
        /// </summary>
        /// <param name="fi"></param>
        /// <returns>SHA256的字符串,16进制表示,大写字母</returns>
        public static string SHA256(this FileInfo fi)
        {
            //如果文件存在
            if (!fi.Exists)
            {
                Log.Error($"ExtensionMethod.SHA256():文件不存在{fi.FullName}!");
                return null;
            }
            try
            {
                FileStream fs = fi.OpenRead();
                SHA256 sha256 = System.Security.Cryptography.SHA256.Create();
                byte[] bytes = sha256.ComputeHash(fs);//32字节的结果,256位?
                fs.Close();
                return bytes.ToHex();
            }
            catch (Exception e)
            {
                Log.Error($"ExtensionMethod.SHA256():计算{fi.FullName}异常->{e.Message}");
            }
            return null;
        }

        /// <summary>
        /// 计算文件夹里的所有文件,返回一个相对文件路径和SHA256的字典
        /// </summary>
        /// <param name="di"></param>
        /// <returns>相对文件路径为key,SHA256为value</returns>
        public static Dictionary<string, string> SHA256(this DirectoryInfo di)
        {
            if (!di.Exists)
            {
                //如果文件夹不存在,那么尝试刷新一下信息,否则可能会一直都判断不存在
                di.Refresh();
                if (!di.Exists)
                {
                    Log.Error($"ExtensionMethod.SHA256():文件夹不存在{di.FullName}!");
                    return null;
                }
            }

            Dictionary<string, string> result = new Dictionary<string, string>();

            FileInfo[] fis = di.GetFiles("*", SearchOption.AllDirectories);//无筛选的得到所有文件
            for (int i = 0; i < fis.Length; i++)
            {
                //生成相对路径:这个文件的完整目录中替换根目录的部分即可,最后切分文件夹都使用斜杠/ (unity的API中基本是/)
                //相对路径结果前面不带斜杠
                string rpath;
                if (di.FullName.EndsWith("\\") || di.FullName.EndsWith("/"))
                {
                    rpath = fis[i].FullName.Substring(di.FullName.Length).Replace("\\", "/");
                }
                else
                {
                    //为了相对路径结果前面不带斜杠,所以+1
                    rpath = fis[i].FullName.Substring(di.FullName.Length + 1).Replace("\\", "/");
                }

                //计算然后添加到结果
                result.Add(rpath, fis[i].SHA256());
            }
            return result;
        }

        /// <summary>
        /// 文件相对文件夹的路径
        /// </summary>
        /// <param name="di">根文件夹</param>
        /// <param name="fi">目前是文件夹内的一个文件</param>
        /// <returns></returns>
        public static string RelativePath(this DirectoryInfo di, FileInfo fi)
        {
            //转义正则里面的\
            Match m = Regex.Match(fi.FullName, "^" + di.FullName.Replace("\\", "\\\\"));
            if (m.Success)
            {
                string rpath;
                if (di.FullName.EndsWith("\\") || di.FullName.EndsWith("/"))
                {
                    rpath = fi.FullName.Substring(m.Index + m.Length).Replace("\\", "/");
                }
                else
                {
                    //为了相对路径结果前面不带斜杠,所以+1
                    rpath = fi.FullName.Substring(m.Index + m.Length + 1).Replace("\\", "/");
                }
                return rpath;
            }
            Log.Error($"ExtensionMethod.RelativePath():暂时不支持不在文件夹内的文件,dir={di.FullName} , file={fi.FullName}!");
            return null;
        }

    }
}
