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

        /// <summary>
        /// 无参构造
        /// </summary>
        public Fileitem() { }

        /// <summary>
        /// 拷贝构造
        /// </summary>
        /// <param name="o"></param>
        public Fileitem(Fileitem o)
        {
            this.relativePath = o.relativePath;
            this.fileSize = o.fileSize;
            this.SHA256 = o.SHA256;
        }

        /// <summary>
        /// 重载等号
        /// </summary>
        public static bool operator ==(Fileitem o1, Fileitem o2)
        {
            bool status = false;
            //先比较大小,再比较SHA256
            if (o1.fileSize == o2.fileSize &&
                o1.SHA256 == o2.SHA256 &&
                o1.relativePath == o2.relativePath)
            {
                status = true;
            }
            return status;
        }

        /// <summary>
        /// 重载不等号
        /// </summary>
        public static bool operator !=(Fileitem o1, Fileitem o2)
        {
            if (o1 == o2)
            {
                return false;
            }
            return true;
        }
    }
}
