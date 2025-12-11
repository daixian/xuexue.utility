using System;
using System.Collections.Generic;
using System.IO;

namespace xuexue.utility
{
    public static class DirectoryUtils
    {
        #region 拷贝目录

        public enum CopyMode
        {
            Mirror, // 完全一致：删除多余文件
            Overwrite, // 覆盖，不删除多余文件
        }

        /// <summary>
        /// 把source目录的内容复制到targetPath目录中
        /// </summary>
        /// <param name="source"></param>
        /// <param name="targetPath"></param>
        /// <param name="mode"></param>
        /// <param name="progress"></param>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public static void CopyTo(this DirectoryInfo source, string targetPath,
            CopyMode mode = CopyMode.Mirror,
            Action<int, int> progress = null // 进度回调
        )
        {
            if (!source.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {source.FullName}");

            DirectoryInfo targetRoot = new DirectoryInfo(targetPath);
            if (!targetRoot.Exists)
                targetRoot.Create();

            // ---------- 第 1 步：预扫描文件总数 ----------
            int totalFiles = CountFilesBFS(source);
            int processed = 0;

            // ---------- 第 2 步：BFS 拷贝 ----------
            Queue<(DirectoryInfo src, DirectoryInfo dst)> q = new Queue<(DirectoryInfo src, DirectoryInfo dst)>();
            q.Enqueue((source, targetRoot));

            while (q.Count > 0) {
                var (srcDir, dstDir) = q.Dequeue();

                // 确保目标目录存在
                if (!dstDir.Exists)
                    dstDir.Create();

                // 复制文件
                foreach (var file in srcDir.GetFiles()) {
                    string targetFilePath = Path.Combine(dstDir.FullName, file.Name);
                    file.CopyTo(targetFilePath, overwrite: true);

                    processed++;
                    progress?.Invoke(processed, totalFiles);
                }

                // 加入子目录 BFS
                foreach (var subDir in srcDir.GetDirectories()) {
                    string dstSubDirPath = Path.Combine(dstDir.FullName, subDir.Name);
                    DirectoryInfo dstSubDir = new DirectoryInfo(dstSubDirPath);

                    q.Enqueue((subDir, dstSubDir));
                }
            }

            // ---------- 第 3 步：Mirror 清理 ----------
            if (mode == CopyMode.Mirror) {
                CleanExtraBFS(targetRoot, source);
            }
        }

        // BFS 计算文件总数
        private static int CountFilesBFS(DirectoryInfo root)
        {
            int count = 0;
            Queue<DirectoryInfo> q = new Queue<DirectoryInfo>();
            q.Enqueue(root);

            while (q.Count > 0) {
                var dir = q.Dequeue();
                count += dir.GetFiles().Length;

                foreach (var sub in dir.GetDirectories())
                    q.Enqueue(sub);
            }
            return count;
        }

        // Mirror 模式下清理多余文件（同样 BFS 无递归）
        private static void CleanExtraBFS(DirectoryInfo dstRoot, DirectoryInfo srcRoot)
        {
            Queue<(DirectoryInfo src, DirectoryInfo dst)> q = new Queue<(DirectoryInfo src, DirectoryInfo dst)>();
            q.Enqueue((srcRoot, dstRoot));

            while (q.Count > 0) {
                var (srcDir, dstDir) = q.Dequeue();

                // 删除多余文件
                foreach (var dstFile in dstDir.GetFiles()) {
                    string srcFile = Path.Combine(srcDir.FullName, dstFile.Name);
                    if (!File.Exists(srcFile))
                        dstFile.Delete();
                }

                // 删除多余目录
                foreach (var dstSub in dstDir.GetDirectories()) {
                    string srcSub = Path.Combine(srcDir.FullName, dstSub.Name);
                    if (!Directory.Exists(srcSub)) {
                        dstSub.Delete(true);
                    }
                    else {
                        q.Enqueue((new DirectoryInfo(srcSub), dstSub));
                    }
                }
            }
        }

        #endregion


        #region 删除目录

        /// <summary>
        /// 一个文件一个文件安全删除，如果删除失败则输出日志然后跳过。
        /// BFS 遍历目录结构。
        /// </summary>
        public static void DeleteSafe(this DirectoryInfo dir)
        {
            if (dir == null || !dir.Exists)
                return;

            Queue<DirectoryInfo> q = new Queue<DirectoryInfo>();

            // BFS：先把所有目录遍历出来（从上往下）
            q.Enqueue(dir);

            // 用列表记录所有目录，稍后反向删除（从下往上）
            List<DirectoryInfo> allDirs = new List<DirectoryInfo>();
            allDirs.Add(dir);

            while (q.Count > 0) {
                var current = q.Dequeue();

                // 目录内的子目录入队
                foreach (var subDir in current.GetDirectories()) {
                    q.Enqueue(subDir);
                    allDirs.Add(subDir);
                }

                // 删除当前目录下所有文件
                foreach (var file in current.GetFiles()) {
                    try {
                        file.Delete();
                    } catch (Exception ex) {
                        Debug.LogError($"DeleteSafe: 文件删除失败 {file.FullName}, Error: {ex.Message}");
                    }
                }
            }

            // 从最深目录开始删
            allDirs.Reverse();

            foreach (var d in allDirs) {
                try {
                    d.Delete();
                } catch (Exception ex) {
                    Debug.LogError($"DeleteSafe: 目录删除失败 {d.FullName}, Error: {ex.Message}");
                }
            }
        }

        #endregion
    }
}
