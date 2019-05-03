using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xuexue.LitJson;

namespace xuexue.utility.Incremental.DTO
{
    /// <summary>
    /// 一个文件项
    /// </summary>
    [xuexueJsonClass]
    public class Fileitem
    {
        /// <summary>
        /// 相对路径
        /// </summary>
        public string relativePath;

        /// <summary>
        /// 文件大小
        /// </summary>
        public long fileSize;

        /// <summary>
        /// 文件SHA256
        /// </summary>
        public string SHA256;
    }
}
