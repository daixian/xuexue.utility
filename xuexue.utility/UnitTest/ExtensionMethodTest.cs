using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using xuexue.utility;

namespace UnitTest
{
    [TestClass]
    public class ExtensionMethodTest
    {
        /// <summary>
        /// 测试:bytes转换到16进制文本
        /// </summary>
        [TestMethod]
        public void ToHex()
        {
            byte[] bs = { 0x1e, 0xaa };
            string hex = bs.ToHex();
            Assert.IsTrue(hex == "1EAA");
            hex = bs.ToHex(false);
            Assert.IsTrue(hex == "1eaa");
        }

        /// <summary>
        /// 测试文件的SHA256
        /// </summary>
        [TestMethod]
        public void FileInfo_SHA256()
        {
            //创建一个测试文件提供SHA256测试,用完再删掉
            string fileName = "./SHA256Test.dat";
            var fs = File.Create(fileName);
            for (int i = 0; i < 1024 * 1024 * 4; i++)
            {
                fs.WriteByte((byte)i);
            }
            fs.Flush();
            fs.Close();
            FileInfo fi = new FileInfo(fileName);
            //正确的SHA256的值为
            string correct = "2B07811057DF887086F06A67EDC6EBF911DE8B6741156E7A2EB1416A4B8B1B2E";
            Assert.IsTrue(fi.SHA256() == correct);
            File.Delete(fileName);
        }

        /// <summary>
        /// 空文件也是有一个固定值的
        /// </summary>
        [TestMethod]
        public void FileInfo_SHA256_empty()
        {
            //创建一个测试文件提供SHA256测试,用完再删掉
            string fileName = "./SHA256Test.dat";
            var fs = File.Create(fileName);
            fs.Flush();
            fs.Close();
            FileInfo fi = new FileInfo(fileName);
            //正确的SHA256的值为
            string correct = "E3B0C44298FC1C149AFBF4C8996FB92427AE41E4649B934CA495991B7852B855";
            Assert.IsTrue(fi.SHA256() == correct);
            File.Delete(fileName);
        }


        [TestMethod]
        public void DirInfo_SHA256()
        {
            //测试数据创建4个文件
            // -> 输入: 文件创建位置,文件写入文本内容
            //    输出: 相对路径结果,SHA256结果值
            Tuple<string, string, string, string>[] testData = {
                 Tuple.Create("1.txt", "aaaa", "1.txt", "61BE55A8E2F6B4E172338BDDF184D6DBEE29C98853E0A0485ECEE7F27B9AF0B4"),
                 Tuple.Create("2.txt", "bbbb", "2.txt", "81CC5B17018674B401B42F35BA07BB79E211239C23BFFE658DA1577E3E646877"),
                 Tuple.Create("3.txt", "cccc", "3.txt", "B6FBD675F98E2ABD22D4ED29FDC83150FEDC48597E92DD1A7A24381D44A27451"),
                 Tuple.Create("subdir/1.txt", "", "subdir/1.txt","E3B0C44298FC1C149AFBF4C8996FB92427AE41E4649B934CA495991B7852B855"),
            };

            DirectoryInfo di = new DirectoryInfo("./DirInfo_SHA256");
            if (di.Exists)
            {
                di.Delete(true);
            }
            di.Create();
            Directory.CreateDirectory(Path.Combine(di.FullName, "subdir"));

            foreach (var item in testData)
            {
                //创建文件
                File.WriteAllText(Path.Combine(di.FullName, item.Item1), item.Item2);
            }
            //这里加到了代码里,就不需要了,否则刚刚创建的文件有几率一直判断为不存在的文件夹
            //while (!di.Exists)
            //{
            //    di.Refresh();
            //    Thread.Sleep(100);
            //}
            var result = di.SHA256();
            Assert.IsNotNull(result);
            foreach (var resItem in result)
            {
                //找出这一项测试录入项
                Tuple<string, string, string, string>[] testItem = testData.Where(x => { return x.Item3 == resItem.Key; }).ToArray();
                Assert.IsTrue(testItem.Length == 1);
                Assert.IsTrue(resItem.Key == testItem[0].Item3);
                Assert.IsTrue(resItem.Value == testItem[0].Item4);
            }
            di.Delete(true);
        }


        /// <summary>
        /// 测试:一个文件和文件夹之间的相对路径
        /// </summary>
        [TestMethod]
        public void RelativePath_0()
        {
            //测试数据-> 输入:文件夹,文件;  输出:相对路径结果
            Tuple<string, string, string>[] testData = {
                Tuple.Create("./123", "./123/abc/1.txt", "abc/1.txt" ),
                Tuple.Create("./123/", "./123/abc/1.txt", "abc/1.txt" ),
                Tuple.Create(".\\123/", "./123/abc/1.txt", "abc/1.txt" ),
                Tuple.Create(".\\123\\", "./123/abc/1.txt", "abc/1.txt" ),
                Tuple.Create("/123", "/123/abc/1.txt", "abc/1.txt" ),
                Tuple.Create("/123/", "/123/abc/1.txt", "abc/1.txt" ),
                Tuple.Create("/123\\", "/123/abc/1.txt", "abc/1.txt" ),
                Tuple.Create<string,string,string>("/123\\", "/234/abc/1.txt", null),
            };

            foreach (var item in testData)
            {
                DirectoryInfo di = new DirectoryInfo(item.Item1);
                FileInfo fi = new FileInfo(item.Item2);
                string rpath = di.RelativePath(fi);
                Assert.IsTrue(rpath == item.Item3, $"失败:当前item=({item.Item1},{item.Item2} = {item.Item3}");
            }
        }

    }
}
