using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using xuexue.utility;

namespace xuexue.file
{
    [Obsolete("写的不好,不再使用")]
    public static class FileHelper
    {
        /// <summary>
        /// 从一个文件夹里得到所有符合正则筛选条件的文件,返回这些文件的完整路径,如果文件夹不存在那么会返回null.
        /// 例如：检索某种后缀名(同时.manifest和.txt)的正则可以这样写@"\.manifest$|\.txt$"
        /// </summary>
        /// <param name="dir">文件夹</param>
        /// <param name="regex">文件名相关的正则</param>
        /// <param name="searchOption">.net库中的检索设置</param>
        /// <returns>符合条件的文件结果列表</returns>
        public static List<FileInfo> GetFiles(string dir, string regex = @"\S*", SearchOption searchOption = SearchOption.AllDirectories)
        {
            if (string.IsNullOrEmpty(dir))//空字符会认为文件夹不存在，所以就认为是根目录好了
            {
                dir = "./";
            }

            if (!Directory.Exists(dir))
            {
                Log.Error($"FileHelper.GetFiles():输入文件夹不存在{dir}");
                return null;
            }
            DirectoryInfo dif = new DirectoryInfo(dir);
            FileInfo[] fis = dif.GetFiles("*", searchOption);//无筛选的得到所有文件

            List<FileInfo> result = new List<FileInfo>();
            for (int i = 0; i < fis.Length; i++)
            {
                if (Regex.IsMatch(fis[i].Name, regex))
                {
                    result.Add(fis[i]);
                }
            }
            return result;
        }

        /// <summary>
        /// 从一个文件夹里得到所有不符合正则筛选条件的文件,返回这些文件的完整路径,如果文件夹不存在那么会返回null.
        /// 例如：排除某种后缀名(同时.manifest和.txt)的正则可以这样写@"\.manifest$|\.txt$"
        /// </summary>
        /// <param name="dir">文件夹</param>
        /// <param name="regex">文件名相关的正则</param>
        /// <param name="searchOption">.net库中的检索设置</param>
        /// <returns>符合条件的文件结果列表</returns>
        public static List<FileInfo> GetFilesExcept(string dir, string regex = @"\S*", SearchOption searchOption = SearchOption.AllDirectories)
        {
            if (string.IsNullOrEmpty(dir))//空字符会认为文件夹不存在，所以就认为是根目录好了
            {
                dir = "./";
            }

            if (!Directory.Exists(dir))
            {
                Log.Error($"FileHelper.GetFiles():输入文件夹不存在{dir}");
                return null;
            }
            DirectoryInfo dif = new DirectoryInfo(dir);
            FileInfo[] fis = dif.GetFiles("*", searchOption);//无筛选的得到所有文件

            List<FileInfo> result = new List<FileInfo>();
            for (int i = 0; i < fis.Length; i++)
            {
                if (!Regex.IsMatch(fis[i].Name, regex))
                {
                    result.Add(fis[i]);
                }
            }
            return result;
        }

        /// <summary>
        /// 清空文件夹
        /// </summary>
        /// <param name="dir">文件夹路径</param>
        public static void CleanDirectory(string dir)
        {
            if (string.IsNullOrEmpty(dir))
            {
                Log.Error($"FileHelper.CleanDirectory():输入文件夹路径为空");
                return;
            }

            if (!Directory.Exists(dir))
            {
                Log.Error($"FileHelper.CleanDirectory():输入文件夹不存在{dir}");
                return;
            }

            try
            {
                Directory.Delete(dir, true);//删除这个文件夹
                Directory.CreateDirectory(dir);//再创建一个空文件夹
            }
            catch (Exception e)
            {
                Log.Error($"FileHelper.CleanDirectory():删除子文件夹异常" + e.Message);
            }
        }

        /// <summary>
        /// 判断一个文件夹是否已经完全空了.如果这个文件夹不存在，那么也直接返回了一个ture.
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static bool IsEmpty(this DirectoryInfo dir)
        {
            if (!dir.Exists)
            {
                return true;
            }

            if (dir.GetFiles().Length == 0 && dir.GetDirectories().Length == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 是否已经没有文件了
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static bool IsNoFile(this DirectoryInfo dir)
        {
            if (!dir.Exists)
            {
                return true;
            }

            if (dir.GetFiles("*", SearchOption.AllDirectories).Length == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 拷贝一个文件夹里的所有文件到另一个文件夹，会覆盖目标文件夹里的文件
        /// </summary>
        /// <param name="srcDir"></param>
        /// <param name="targetDir"></param>
        /// <returns>实际Copy的文件数</returns>
        public static int CopyTo(string srcDir, string targetDir)
        {
            if (string.IsNullOrEmpty(srcDir))
            {
                Log.Error($"FileHelper.CopyTo():输入文件夹路径为空");
                return 0;
            }

            if (!Directory.Exists(srcDir))
            {
                Log.Error($"FileHelper.CopyTo():输入文件夹不存在{srcDir}");
                return 0;
            }
            return CopyTo(new DirectoryInfo(srcDir), targetDir);
        }

        /// <summary>
        /// 拷贝文件夹，将MD5值不同的文件进行backup，放置到backupDir的目录下
        /// </summary>
        /// <param name="srcDir"></param>
        /// <param name="targetDir"></param>
        /// <param name="backupDir"></param>
        /// <returns>实际Copy的文件数</returns>
        public static int CopyTo(string srcDir, string targetDir, string backupDir)
        {
            if (string.IsNullOrEmpty(srcDir))
            {
                Log.Error($"FileHelper.CopyTo():输入文件夹路径为空");
                return 0;
            }

            if (!Directory.Exists(srcDir))
            {
                Log.Error($"FileHelper.CopyTo():输入文件夹不存在{srcDir}");
                return 0;
            }
            return CopyTo(new DirectoryInfo(srcDir), targetDir, backupDir);
        }

        /// <summary>
        /// 拷贝一个文件夹里的所有文件到另一个文件夹，会覆盖目标文件夹里的文件
        /// </summary>
        /// <param name="srcDir"></param>
        /// <param name="targetDir"></param>
        /// <returns>实际Copy的文件数</returns>
        public static int CopyTo(this DirectoryInfo srcDir, string targetDir)
        {
            int opCount = 0;
            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }
            FileInfo[] fis = srcDir.GetFiles("*", SearchOption.AllDirectories);//得到所有文件
            for (int i = 0; i < fis.Length; i++)
            {
                FileInfo srcfi = fis[i];
                string rel = Relative(srcfi.FullName, srcDir.FullName);
                FileInfo targetFi = new FileInfo(Path.Combine(targetDir, rel));
                if (!targetFi.Directory.Exists)
                {
                    Directory.CreateDirectory(targetFi.Directory.FullName);
                }
                if (File.Exists(targetFi.FullName))//如果target文件存在
                {
                    string md5Target = MD5Helper.FileMD5(targetFi.FullName);
                    string md5Src = MD5Helper.FileMD5(srcfi.FullName);
                    if (md5Target != md5Src)//如果target文件MD5不一致
                    {
                        File.Copy(srcfi.FullName, targetFi.FullName, true);//覆盖拷贝
                        opCount++;
                    }
                }
                else//如果target文件不存在
                {
                    File.Copy(srcfi.FullName, targetFi.FullName, true);//覆盖拷贝
                    opCount++;
                }
            }
            return opCount;
        }

        /// <summary>
        /// 拷贝文件夹，将MD5值不同的文件进行backup，放置到backupDir的目录下
        /// </summary>
        /// <param name="srcDir"></param>
        /// <param name="targetDir"></param>
        /// <param name="backupDir"></param>
        /// <returns>实际Copy的文件数</returns>
        public static int CopyTo(this DirectoryInfo srcDir, string targetDir, string backupDir)
        {
            int opCount = 0;
            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }
            FileInfo[] fis = srcDir.GetFiles("*", SearchOption.AllDirectories);//得到所有文件
            for (int i = 0; i < fis.Length; i++)
            {
                FileInfo srcfi = fis[i];
                string rel = Relative(srcfi.FullName, srcDir.FullName);
                FileInfo targetFi = new FileInfo(Path.Combine(targetDir, rel));
                if (!targetFi.Directory.Exists)
                {
                    Directory.CreateDirectory(targetFi.Directory.FullName);
                }
                if (File.Exists(targetFi.FullName))//如果target文件存在
                {
                    string md5Target = MD5Helper.FileMD5(targetFi.FullName);
                    string md5Src = MD5Helper.FileMD5(srcfi.FullName);
                    if (md5Target != md5Src)//如果target文件MD5不一致
                    {
                        FileInfo backupFile = new FileInfo(Path.Combine(backupDir, rel));
                        if (!backupFile.Directory.Exists)
                        {
                            Directory.CreateDirectory(backupFile.Directory.FullName);
                        }
                        try { File.Move(targetFi.FullName, backupFile.FullName); } catch (Exception) { }

                        File.Copy(srcfi.FullName, targetFi.FullName, true);//覆盖拷贝
                        opCount++;
                    }
                }
                else//如果target文件不存在
                {
                    File.Copy(srcfi.FullName, targetFi.FullName, true);//覆盖拷贝
                    opCount++;
                }
            }
            return opCount;
        }

        /// <summary>
        /// 原版的MoveTo诸多限制，这个函数是一个更强大的move。包含了自动和原来的文件夹合并策略。
        /// </summary>
        /// <param name="srcDir"></param>
        /// <param name="targetDir"></param>
        public static void MoveMergeTo(this DirectoryInfo srcDir, string targetDir)
        {
            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }

            DirectoryInfo[] dis = srcDir.GetDirectories("*", SearchOption.AllDirectories);//得到所有子文件夹
            List<string> listTargetDir = new List<string>();//Contains方法可以比对字符串
            for (int i = 0; i < dis.Length; i++)
            {
                DirectoryInfo srcdi = dis[i];
                string rel = Relative(srcdi.FullName, srcDir.FullName);
                DirectoryInfo targetDi = new DirectoryInfo(Path.Combine(targetDir, rel));//目标路径
                if (!targetDi.Exists)//如果目标文件夹不存在就创建
                {
                    Directory.CreateDirectory(targetDi.FullName);
                }
                listTargetDir.Add(targetDi.FullName);
            }

            FileInfo[] fis = srcDir.GetFiles("*", SearchOption.AllDirectories);//得到所有文件

            for (int i = 0; i < fis.Length; i++)//给所有待移动文件改名,防止先移动的文件覆盖了后移动的文件
            {
                File.Move(fis[i].FullName, fis[i].FullName + ".xxtemp");
            }

            for (int i = 0; i < fis.Length; i++)
            {
                FileInfo srcfi = fis[i];
                string rel = Relative(srcfi.FullName, srcDir.FullName);
                FileInfo targetFi = new FileInfo(Path.Combine(targetDir, rel));//目标路径

                if (targetFi.Exists)
                {
                    File.Delete(targetFi.FullName);//删除目标文件
                }
                if (!targetFi.Directory.Exists)//如果目标文件的父目录不存在就创建
                {
                    Directory.CreateDirectory(targetFi.Directory.FullName);
                }
                File.Move(srcfi.FullName + ".xxtemp", targetFi.FullName);
            }

            for (int i = 0; i < dis.Length; i++)
            {
                DirectoryInfo srcdi = dis[i];
                if (srcdi.Exists && srcdi.IsNoFile() && !listTargetDir.Contains(srcdi.FullName))//同时这个路径还不是一个目标路径
                {
                    Directory.Delete(srcdi.FullName, true);
                }
            }
        }

        /// <summary>
        /// 求相对路径,需要rootPath确实是childPath的父级.返回结果一律使用"/",相对路径第一个字符不含"/".
        /// </summary>
        /// <param name="childPath">子级目录</param>
        /// <param name="rootPath">父级目录</param>
        /// <returns></returns>
        public static string Relative(string childPath, string rootPath)
        {
            string fChildPath = new FileInfo(childPath).FullName.Replace("\\", "/");
            string fRootPath = new FileInfo(rootPath).FullName.Replace("\\", "/"); ;
            if (!fRootPath.EndsWith("/"))
            {
                fRootPath += "/";
            }
            if (fChildPath.Contains(fRootPath))
                return fChildPath.Replace(fRootPath, "");
            else
                Log.Error($"FileHelper.Relative(): {childPath} ->{fRootPath}不是{rootPath} ->{fChildPath}的父级!");
            return null;
        }

        /// <summary>
        /// 创建它的父文件夹
        /// </summary>
        /// <param name="fi"></param>
        /// <returns>进行了创建返回true，不需要创建返回false</returns>
        public static bool CreateParentDir(this FileInfo fi)
        {
            if (!fi.Directory.Exists)
            {
                Directory.CreateDirectory(fi.Directory.FullName);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 检查并且创建文件夹
        /// </summary>
        /// <param name="path"></param>
        public static void CheckCreateParentDir(string path)
        {
            if (path.EndsWith("\\") || path.EndsWith("/"))
            {
                Log.Error($"FileHelper.CheckCreateParentDir(): 输入的{path}不是文件路径!");
                return;
            }
            FileInfo fi = new FileInfo(path);
            if (!fi.Directory.Exists)
            {
                Directory.CreateDirectory(fi.Directory.FullName);
            }
        }
    }
}