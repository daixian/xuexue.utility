using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xuexue.utility.Incremental.DTO;
using xuexue.LitJson;

namespace xuexue.utility.Incremental
{
    /// <summary>
    /// 增量更新
    /// </summary>
    public class IncrementalUpdate
    {

        /// <summary>
        /// 创建一个配置文件
        /// </summary>
        public static void CreateConfigFile(string dirPath, uint[] version, string saveFilePath)
        {

            if (version == null || version.Length != 4)
            {
                Log.Error($"IncrementalUpdate.CreateConfigFile():版本号规定为4个长度的uint数组!");
                return;
            }

            DirectoryInfo di = new DirectoryInfo(dirPath);
            if (!di.Exists)
            {
                Log.Error($"IncrementalUpdate.CreateConfigFile():文件夹不存在{di.FullName}!");
                return;
            }

            SoftFile softFile = new SoftFile();
            softFile.version = version;
            softFile.rootPath = di.FullName;

            FileInfo[] fis = di.GetFiles("*", SearchOption.AllDirectories);//无筛选的得到所有文件
            for (int i = 0; i < fis.Length; i++)
            {
                Fileitem item = new Fileitem();
                //生成相对路径:这个文件的完整目录中替换根目录的部分即可,最后切分文件夹都使用斜杠/ (unity的API中基本是/)
                //相对路径结果前面不带斜杠
                if (di.FullName.EndsWith("\\") || di.FullName.EndsWith("/"))
                {
                    item.relativePath = fis[i].FullName.Substring(di.FullName.Length).Replace("\\", "/");
                }
                else
                {
                    //为了相对路径结果前面不带斜杠,所以+1
                    item.relativePath = fis[i].FullName.Substring(di.FullName.Length + 1).Replace("\\", "/");
                }
                item.fileSize = fis[i].Length;
                item.SHA256 = fis[i].SHA256();
                //计算然后添加到结果
                softFile.files.Add(item);
            }

            StreamWriter sw = File.CreateText(saveFilePath);
            JsonWriter jw = new JsonWriter(sw) { PrettyPrint = true };
            JsonMapper.ToJson(softFile, jw);
            sw.Flush();
            sw.Close();
        }

        /// <summary>
        /// 得到需要下载的文件列表
        /// </summary>
        public static void DownloadFileList()
        {

        }
    }
}
