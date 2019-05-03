using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xuexue.utility.Incremental.DTO
{
    /// <summary>
    /// 要下载的文件项
    /// </summary>
    public class DownloadFileItem : Fileitem
    {
        /// <summary>
        /// 从一个Fileitem构造
        /// </summary>
        /// <param name="fi"></param>
        public DownloadFileItem(Fileitem fi) : base(fi)
        {

        }

        /// <summary>
        /// 下载url
        /// </summary>
        public string url;
    }
}
