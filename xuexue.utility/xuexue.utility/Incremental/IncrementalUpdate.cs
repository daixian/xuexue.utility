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
    /// 增量更新,c#部分的功能吧
    /// </summary>
    public class IncrementalUpdate
    {

        /// <summary>
        /// 计算一个软件版本文件
        /// </summary>
        /// <param name="rootPath">写进json的软件安装目录</param>
        /// <param name="version"></param>
        /// <param name="rootUrl"></param>
        /// <param name="dirPath">要计算的文件夹</param>
        /// <param name="saveJsonPath">要保存的json文件位置</param>
        public static void CreateSoftVersionFile(string rootPath, uint[] version, string rootUrl, string dirPath, string saveJsonPath)
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

            SoftFile softFile = new SoftFile
            {
                version = version,
                rootPath = rootPath,
                rootUrl = rootUrl
            };

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

            StreamWriter sw = File.CreateText(saveJsonPath);
            JsonWriter jw = new JsonWriter(sw) { PrettyPrint = true };
            JsonMapper.ToJson(softFile, jw);
            sw.Flush();
            sw.Close();
        }

        /// <summary>
        /// 新老版本两版软件之间的需要更新的文件列表,老软件信息可以为null
        /// </summary>
        /// <param name="verOld">老软件信息,可以为null</param>
        /// <param name="verNew">新版软件信息,可以为null</param>
        public static DownloadList CompareToDownloadList(SoftFile verOld, SoftFile verNew)
        {
            DownloadList downloadList = new DownloadList
            {
                targetVersion = verNew.version //目标版本
            };

            if (verOld != null)
            {
                downloadList.curVersion = verOld.version;//当前版本
            }

            //遍历所有新文件
            foreach (var item in verNew.files)
            {   //如果这一项在老文件里包含了,那么就不用下载
                if (verOld == null || !verOld.IsContainFile(item))
                {
                    DownloadFileItem dfi = new DownloadFileItem(item);
                    if (verNew.rootUrl.EndsWith("/"))
                        dfi.url = verNew.rootUrl + dfi.relativePath;
                    else
                        dfi.url = verNew.rootUrl + "/" + dfi.relativePath;
                    downloadList.files.Add(dfi);
                }
            }

            return downloadList;
        }
    }
}
