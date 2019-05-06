using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xuexue.utility.Incremental;

namespace UnitTest
{
    [TestClass]
    public class IncrementalUpdateTest
    {

        /// <summary>
        /// 创建一个配置文件试试
        /// </summary>
        [TestMethod]
        public void CreateConfigFile()
        {
            IncrementalUpdate.CreateSoftVersionFile(".", new uint[] { 0, 0, 1, 0 }, null, ".", "./config.json");
        }
    }
}
