using xuexue.file;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;

namespace UnitTest
{
    [TestClass]
    public class UnitTestFileHelper
    {
        [TestMethod]
        public void TestMethod_GetFiles()
        {
            var fs = File.Create("testFile.manifest");
            fs.Close();
            fs = File.Create("testFile2.manifest");
            fs.Close();

            fs = File.Create("testFile..manifest");
            fs.Close();
            fs = File.Create("testFile.txt2");
            fs.Close();

            var files = FileHelper.GetFiles("./", @"\.manifest$|\.txt2$", SearchOption.AllDirectories);
            for (int i = 0; i < files.Count; i++)
            {
                files[i].Delete();//删除这个.manifest文件
            }

            Assert.IsFalse(File.Exists("testFile.manifest"));//这两个文件应该已经被删除了
            Assert.IsFalse(File.Exists("testFile2.manifest"));//这两个文件应该已经被删除了
            Assert.IsFalse(File.Exists("testFile..manifest"));//这两个文件应该已经被删除了
            Assert.IsFalse(File.Exists("testFile.txt2"));//这两个文件应该已经被删除了
        }

        [TestMethod]
        public void TestMethod_GetFiles_AllFile()
        {
            var fs = File.Create("testFile.manifest");
            fs.Close();

            var files = FileHelper.GetFiles("./");
            Assert.IsTrue(files.Count > 0);

            DirectoryInfo dif = new DirectoryInfo("./");
            var files2 = dif.GetFiles("*", SearchOption.AllDirectories);
            Assert.IsTrue(files.Count == files2.Length); //应该是选择了所有的文件
        }

        [TestMethod]
        public void TestMethod_GetFilesExcept()
        {
            var fs = File.Create("testFile.manifest");
            fs.Close();
            var files = FileHelper.GetFilesExcept("./", @"\.manifest$");
            bool isHave = false;
            Assert.IsTrue(files.Count > 0);
            for (int i = 0; i < files.Count; i++)
            {
                if (files[i].Name == "testFile.manifest")
                {
                    isHave = true;
                }
            }
            Assert.IsFalse(isHave);//应该没有选出testFile.manifest
        }

        /// <summary>
        /// 这个测试是尝试通过正则的某种写法来反剔除掉不要某种规则的。
        /// 即用GetFiles()来实现GetFilesExcept()的功能。
        /// 结果没有成功，所以不再使用这个测试。
        /// </summary>
        //[TestMethod]
        private void TestMethod_GetFiles2()
        {
            var fs = File.Create("testFile.manifest");
            fs.Close();
            fs = File.Create("testFile..manifest");
            fs.Close();
            fs = File.Create("testFile...manifest");
            fs.Close();
            var files = FileHelper.GetFiles("./", @"[\s\S]+?\.(?!manifest$)");
            bool isHave = false;
            Assert.IsTrue(files.Count > 0);
            for (int i = 0; i < files.Count; i++)
            {
                if (files[i].Name == "testFile.manifest")
                {
                    isHave = true;
                }
                if (files[i].Name == "testFile..manifest")
                {
                    isHave = true;
                }
                if (files[i].Name == "testFile...manifest")
                {
                    isHave = true;
                }
            }
            Assert.IsFalse(isHave);//应该没有选出testFile.manifest
        }

        [TestMethod]
        public void TestMethod_Relative()
        {
            Assert.IsTrue(FileHelper.Relative("./123/1234", "./") == "123/1234");
            Assert.IsTrue(FileHelper.Relative("123/1234/", "./") == "123/1234/");
        }

        [TestMethod]
        public void TestMethod_DirCopyToMoveTo()
        {
            string testDir = "./TestDir";
            string backupDir = Path.Combine(testDir, "backup");

            DirectoryInfo di = new DirectoryInfo(testDir);

            if (!Directory.Exists(testDir))
                Directory.CreateDirectory(testDir);

            FileHelper.CleanDirectory(testDir);
            Assert.IsTrue(di.Exists && di.IsEmpty());

            var fs = File.Create(Path.Combine(testDir, "testFile.manifest"));
            fs.Close();
            fs = File.Create(Path.Combine(testDir, "testFile1.manifest"));
            fs.Close();
            fs = File.Create(Path.Combine(testDir, "testFile2.manifest"));
            fs.Close();

            di.CopyTo(backupDir);//拷贝到backup文件夹

            DirectoryInfo di2 = new DirectoryInfo(backupDir);
            FileInfo[] fis = di2.GetFiles();
            Assert.IsTrue(fis.Length == 3);

            di.CopyTo(backupDir);//再以当前状态拷贝一次到backup文件夹
            fis = di.GetFiles("*", SearchOption.AllDirectories);
            Assert.IsTrue(fis.Length == 9);

            di2.MoveMergeTo(di.FullName);//backup文件夹内容上提一次
            fis = di.GetFiles("*", SearchOption.AllDirectories);
            Assert.IsTrue(fis.Length == 6);
        }
    }
}