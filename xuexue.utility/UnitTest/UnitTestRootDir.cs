using xuexue.file;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace UnitTest
{
    [TestClass]
    public class UnitTestRootDir
    {

        [TestMethod]
        public void TestMethod_RootDir()
        {
            Directory.CreateDirectory("testDir/root/");
            File.Create("testDir/file1.txt").Close();
            File.Create("testDir/file2.txt").Close();
            File.Create("testDir/root/file1.txt").Close();
            File.Create("testDir/root/file2.txt").Close();
            File.Create("testDir/root/file3.txt").Close();
            RootDir rootDir = new RootDir();
            rootDir.InitWithPath("testDir/");

            string fp1 = rootDir.GetFullPath("dir1/dir2/dir3/file.txt");
            Assert.IsTrue(fp1.Contains("testDir\\dir1\\dir2"));
            fp1 = rootDir.GetFullPath("dir1\\dir2\\dir3\\file.txt");
            Assert.IsTrue(fp1.Contains("testDir\\dir1\\dir2"));

            rootDir.CalcMD5();

            foreach (var kvp in rootDir.dict)
            {
                //空文件的md5值是固定的这个值
                Assert.IsTrue(kvp.Value.MD5 == "d41d8cd98f00b204e9800998ecf8427e");
            }
        }


    }
}