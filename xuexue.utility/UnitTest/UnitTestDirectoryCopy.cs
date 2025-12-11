using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using xuexue.utility;

namespace UnitTest
{
    [TestClass]
    public class UnitTestDirectoryCopy
    {
        private string _tempRoot;

        #region 初始化样例

        [TestInitialize]
        public void Setup()
        {
            Debug.SetConsoleLog();

            _tempRoot = Path.Combine(Path.GetTempPath(), "DirUtilsTest_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempRoot);
            Debug.Log($"UnitTest.UnitTestDirectoryUtils: 创建临时目录: {_tempRoot}");
        }

        [TestCleanup]
        public void Cleanup()
        {


            DirectoryInfo dir = new DirectoryInfo(_tempRoot);
            if (dir.Exists) {
                dir.DeleteSafe();
            }
            // try {
            //     if (Directory.Exists(_tempRoot)) {
            //         Directory.Delete(_tempRoot, true);
            //     }
            // } catch (Exception e) {
            //     Debug.LogError($"{e}");
            // }

            Debug.Cleanup();
        }

        private DirectoryInfo CreateDir(params string[] relativePath)
        {
            string path = _tempRoot;
            foreach (var p in relativePath)
                path = Path.Combine(path, p);

            Directory.CreateDirectory(path);
            return new DirectoryInfo(path);
        }

        private void WriteText(string dir, string fileName, string content)
        {
            Directory.CreateDirectory(dir);
            File.WriteAllText(Path.Combine(dir, fileName), content);
        }

        #endregion

        // 基础复制测试（Overwrite，简单文件）
        [TestMethod]
        public void CopyTo_Overwrite_SimpleFiles()
        {
            // Arrange
            var sourceDir = CreateDir("source");
            var targetDirPath = Path.Combine(_tempRoot, "target");

            WriteText(sourceDir.FullName, "a.txt", "hello");
            WriteText(sourceDir.FullName, "b.txt", "world");

            // Act
            sourceDir.CopyTo(targetDirPath, DirectoryUtils.CopyMode.Overwrite);

            // Assert
            Assert.IsTrue(Directory.Exists(targetDirPath));
            Assert.IsTrue(File.Exists(Path.Combine(targetDirPath, "a.txt")));
            Assert.IsTrue(File.Exists(Path.Combine(targetDirPath, "b.txt")));

            Assert.AreEqual("hello", File.ReadAllText(Path.Combine(targetDirPath, "a.txt")));
            Assert.AreEqual("world", File.ReadAllText(Path.Combine(targetDirPath, "b.txt")));
        }

        // Overwrite 模式下保留多余文件
        [TestMethod]
        public void CopyTo_Overwrite_KeepExtraFilesInTarget()
        {
            // Arrange
            var sourceDir = CreateDir("source");
            var targetDirPath = Path.Combine(_tempRoot, "target");
            Directory.CreateDirectory(targetDirPath);

            WriteText(sourceDir.FullName, "a.txt", "from source");
            WriteText(targetDirPath, "extra.txt", "keep me");

            // Act
            sourceDir.CopyTo(targetDirPath, DirectoryUtils.CopyMode.Overwrite);

            // Assert
            Assert.IsTrue(File.Exists(Path.Combine(targetDirPath, "a.txt")));
            Assert.IsTrue(File.Exists(Path.Combine(targetDirPath, "extra.txt")), "Overwrite 模式不应该把多余文件删掉");
        }

        // Mirror 模式下删除多余文件
        [TestMethod]
        public void CopyTo_Mirror_RemoveExtraFilesInTarget()
        {
            // Arrange
            var sourceDir = CreateDir("source");
            var targetDirPath = Path.Combine(_tempRoot, "target");
            Directory.CreateDirectory(targetDirPath);

            WriteText(sourceDir.FullName, "a.txt", "from source");
            WriteText(targetDirPath, "extra.txt", "to be deleted");

            // Act
            sourceDir.CopyTo(targetDirPath, DirectoryUtils.CopyMode.Mirror);

            // Assert
            Assert.IsTrue(File.Exists(Path.Combine(targetDirPath, "a.txt")));
            Assert.IsFalse(File.Exists(Path.Combine(targetDirPath, "extra.txt")), "Mirror 模式下多余文件应该被删除");
        }

        // Mirror 模式下删除多余子目录
        [TestMethod]
        public void CopyTo_Mirror_RemoveExtraDirectoriesInTarget()
        {
            // Arrange
            var sourceDir = CreateDir("source");
            var sourceSub = CreateDir("source", "Sub1");
            WriteText(sourceSub.FullName, "a.txt", "ok");

            var targetDirPath = Path.Combine(_tempRoot, "target");
            var targetDir = new DirectoryInfo(targetDirPath);
            targetDir.Create();

            // 目标多一个多余目录
            var extraSub = Directory.CreateDirectory(Path.Combine(targetDirPath, "Extra"));
            WriteText(extraSub.FullName, "extra.txt", "to be deleted");

            // Act
            sourceDir.CopyTo(targetDirPath, DirectoryUtils.CopyMode.Mirror);

            // Assert
            Assert.IsTrue(Directory.Exists(Path.Combine(targetDirPath, "Sub1")));
            Assert.IsFalse(Directory.Exists(Path.Combine(targetDirPath, "Extra")), "Mirror 模式下多余目录应被删除");
        }

        [TestMethod]
        public void CopyTo_Overwrite_NestedDirectories()
        {
            // Arrange
            var sourceDir = CreateDir("source");
            var sub1 = CreateDir("source", "Sub1");
            var sub2 = CreateDir("source", "Sub1", "Sub2");

            WriteText(sourceDir.FullName, "root.txt", "root");
            WriteText(sub1.FullName, "s1.txt", "sub1");
            WriteText(sub2.FullName, "s2.txt", "sub2");

            var targetDirPath = Path.Combine(_tempRoot, "target");

            // Act
            sourceDir.CopyTo(targetDirPath, DirectoryUtils.CopyMode.Overwrite);

            // Assert
            Assert.IsTrue(File.Exists(Path.Combine(targetDirPath, "root.txt")));
            Assert.IsTrue(File.Exists(Path.Combine(targetDirPath, "Sub1", "s1.txt")));
            Assert.IsTrue(File.Exists(Path.Combine(targetDirPath, "Sub1", "Sub2", "s2.txt")));
        }


        [TestMethod]
        public void CopyTo_Overwrite_OverrideExistingFile()
        {
            // Arrange
            var sourceDir = CreateDir("source");
            var targetDirPath = Path.Combine(_tempRoot, "target");
            Directory.CreateDirectory(targetDirPath);

            WriteText(sourceDir.FullName, "same.txt", "from source");
            WriteText(targetDirPath, "same.txt", "old content");

            // Act
            sourceDir.CopyTo(targetDirPath, DirectoryUtils.CopyMode.Overwrite);

            // Assert
            string text = File.ReadAllText(Path.Combine(targetDirPath, "same.txt"));
            Assert.AreEqual("from source", text);
        }

        [TestMethod]
        public void CopyTo_EmptySourceDirectory_TargetCreated()
        {
            // Arrange
            var sourceDir = CreateDir("source_empty");
            var targetDirPath = Path.Combine(_tempRoot, "target_empty");

            // Act
            sourceDir.CopyTo(targetDirPath, DirectoryUtils.CopyMode.Overwrite);

            // Assert
            Assert.IsTrue(Directory.Exists(targetDirPath));
            Assert.AreEqual(0, Directory.GetFiles(targetDirPath).Length);
            Assert.AreEqual(0, Directory.GetDirectories(targetDirPath).Length);
        }

        [TestMethod]
        public void CopyTo_ProgressCallback_CalledCorrectTimes()
        {
            // Arrange
            var sourceDir = CreateDir("source");
            WriteText(sourceDir.FullName, "a.txt", "1");
            WriteText(sourceDir.FullName, "b.txt", "2");

            var sub = CreateDir("source", "Sub");
            WriteText(sub.FullName, "c.txt", "3");

            var targetDirPath = Path.Combine(_tempRoot, "target");

            int lastCurrent = 0;
            int lastTotal = 0;
            int callCount = 0;

            // Act
            sourceDir.CopyTo(
                targetDirPath,
                DirectoryUtils.CopyMode.Overwrite,
                (current, total) => {
                    callCount++;
                    lastCurrent = current;
                    lastTotal = total;
                });

            // Assert
            // 总共 3 个文件 => 进度回调应被调用 3 次（你实现里是一文件一次）
            Assert.AreEqual(3, callCount, "进度回调次数应等于文件数");
            Assert.AreEqual(3, lastCurrent);
            Assert.AreEqual(3, lastTotal);
        }

        [TestMethod]
        [ExpectedException(typeof(DirectoryNotFoundException))]
        public void CopyTo_SourceNotExist_Throw()
        {
            // Arrange
            var sourceDir = new DirectoryInfo(Path.Combine(_tempRoot, "not_exist"));
            var targetDirPath = Path.Combine(_tempRoot, "target");

            // Act
            sourceDir.CopyTo(targetDirPath, DirectoryUtils.CopyMode.Overwrite);
        }

        [TestMethod]
        public void CopyTo_Mirror_CleanupDeepExtraDirectories()
        {
            // Arrange
            var sourceDir = CreateDir("source");
            WriteText(sourceDir.FullName, "a.txt", "source");

            var targetDirPath = Path.Combine(_tempRoot, "target");
            var targetDir = Directory.CreateDirectory(targetDirPath);

            var extra1 = Directory.CreateDirectory(Path.Combine(targetDirPath, "Extra1"));
            var extra2 = Directory.CreateDirectory(Path.Combine(extra1.FullName, "Extra2"));
            WriteText(extra2.FullName, "extra.txt", "to be deleted");

            // Act
            sourceDir.CopyTo(targetDirPath, DirectoryUtils.CopyMode.Mirror);

            // Assert
            Assert.IsTrue(File.Exists(Path.Combine(targetDirPath, "a.txt")));
            Assert.IsFalse(Directory.Exists(extra1.FullName), "多层多余目录应被完全删除");
        }

        [TestMethod]
        public void CopyTo_Ext()
        {
            // Arrange
            var sourceDir = new DirectoryInfo("C:\\Program Files\\TrackingService");
            var targetDirPath = Path.Combine(_tempRoot, "TrackingServiceCopy");
            // Act
            sourceDir.CopyTo(targetDirPath, DirectoryUtils.CopyMode.Mirror);
            Debug.Log($"拷贝完成,文件夹在{targetDirPath}");
        }
    }
}
