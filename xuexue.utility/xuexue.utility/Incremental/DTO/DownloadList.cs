using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xuexue.utility.Incremental.DTO
{
    /// <summary>
    /// 下载文件的列表
    /// </summary>
    public class DownloadList
    {
        /// <summary>
        /// 当前版本
        /// </summary>
        public uint[] curVersion = new uint[4];

        /// <summary>
        /// 目标版本
        /// </summary>
        public uint[] targetVersion = new uint[4];

        /// <summary>
        /// 所有的文件项
        /// </summary>
        public List<DownloadFileItem> files = new List<DownloadFileItem>();
    }
}
