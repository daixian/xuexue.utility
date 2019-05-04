using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xuexue.utility;

namespace xuexue.file
{
    /// <summary>
    /// 某一个文件夹的根目录,它应该有在一直维护一个Json文件，记录着当前的文件夹状态。
    /// </summary>
    [Obsolete("写的不好,不再使用")]
    public class RootDir
    {
        /// <summary>
        /// 以本地文件relativePath为key的字典，每一个本地文件路径应该不同，否则就覆盖了。
        /// </summary>
        public Dictionary<string, FileItem> dict = new Dictionary<string, FileItem>();

        /// <summary>
        /// 这个文件夹的根目录(绝对路径),最后结尾会带斜杠。
        /// 注：
        /// 它本来由DirectoryInfo的FullName得到,如果构造DirectoryInfo时相对路径文件夹末尾不带斜杠，则结果也不带斜杠。
        /// 形如"D:\\Work\\DFlie\\UnitTest\\bin\\Debug\\testDir"
        /// 如果构造DirectoryInfo时输入的相对路径文件夹末尾带斜杠，则结果的fullName也带有斜杠
        /// </summary>
        public string dirPath;

        /// <summary>
        /// 通过一个现有的文件夹统计每个文件，实际上只对所有已存在的文件，记录了一个相对路径.
        /// </summary>
        /// <param name="dirPath">作为整个系统的根目录文件夹</param>
        public void InitWithPath(string dirPath)
        {
            dict.Clear();
            if (!Directory.Exists(dirPath))
            {
                Log.Error("RootDir.InitWithPath():输入的文件夹不存在!dirPath = " + dirPath);
                this.dirPath = dirPath;//这里发生错误，只随便的设置一下成员dirPath
                return;
            }

            DirectoryInfo dif = new DirectoryInfo(dirPath);
            this.dirPath = dif.FullName;//使用绝对路径来设置成员dirPath
            if (!this.dirPath.EndsWith("\\"))
            {
                this.dirPath += "\\";
            }

            FileInfo[] fis = dif.GetFiles("*", SearchOption.AllDirectories);//无筛选的得到所有文件
            for (int i = 0; i < fis.Length; i++)
            {
                //生成相对路径:这个文件的完整目录中替换根目录的部分即可,最后切分文件夹都使用斜杠/ (unity的API中基本是/)
                //相对路径结果前面不带斜杠
                string rpath = fis[i].FullName.Replace(dif.FullName, "").Replace("\\", "/");
                FileItem fileItem = new FileItem { relativePath = rpath };
                this.dict.Add(rpath, fileItem);
            }
        }

        /// <summary>
        /// 输入一个相对路径,得到完整的路径,要注意输入的相对路径不应该以斜杠开头。
        /// 输入形式为：dir1/dir2/dir3/file.txt (正反斜杠均可)
        /// </summary>
        /// <param name="relativePath">相对路径</param>
        /// <returns></returns>
        public string GetFullPath(string relativePath)
        {
            if (relativePath.StartsWith("\\") || relativePath.StartsWith("/"))
            {
                Log.Error("RootDir.GetFullPath():输入的相对路径不应该以斜杠开头, relativePath = " + relativePath);
                return null;
            }
            return Path.Combine(dirPath, relativePath).Replace("/", "\\");
        }

        /// <summary>
        /// 向记录中添加一项FileItem记录,如果已经重复那么返回false;
        /// </summary>
        /// <param name="fi">FileItem记录</param>
        /// <returns></returns>
        public bool AddItem(FileItem fi)
        {
            if (!dict.ContainsKey(fi.relativePath))
            {
                dict.Add(fi.relativePath, fi);
                return true;
            }
            else
            {
                Log.Error("RootDir.AddItem():输入了重复的relativePath=" + fi.relativePath);
                return false;
            }
        }

        /// <summary>
        /// 通常这些资源文件都是带有分配id的,那么通过一个id去查询一项纪录是否存在。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FileItem GetItemWithID(int id)
        {
            foreach (var kvp in dict)
            {
                if (kvp.Value.id == id)
                {
                    return kvp.Value;
                }
            }
            return default(FileItem);
        }

        /// <summary>
        /// 通过相对路径来得到一项
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        public FileItem GetItemWithRP(string relativePath)
        {
            if (!dict.ContainsKey(relativePath))
            {
                return dict[relativePath];
            }
            else
                return default(FileItem);
        }

        #region 对单项进行计算

        /// <summary>
        /// 计算MD5
        /// </summary>
        public void CalcMD5()
        {
            foreach (var kvp in dict)
            {
                FileItem fileItem = kvp.Value;
                string fullPath = this.GetFullPath(fileItem.relativePath);
                fileItem.MD5 = MD5Helper.FileMD5(fullPath);
            }
        }

        /// <summary>
        /// 异步计算MD5
        /// </summary>
        public RootDir CalcMD5(Action<RootDir, FileItem, int> action)
        {
            Task.Run(() =>
            {
                int count = 0;
                foreach (var kvp in dict)
                {
                    FileItem fileItem = kvp.Value;
                    string fullPath = this.GetFullPath(fileItem.relativePath);
                    fileItem.MD5 = MD5Helper.FileMD5(fullPath);
                    //如果有事件那么就传出事件
                    action?.Invoke(this, fileItem, count);
                }
            });
            return this;
        }


        #endregion

    }
}
