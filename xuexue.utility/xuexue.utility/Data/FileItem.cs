using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xuexue.file
{
    /// <summary>
    /// 一个文件的记录项
    /// </summary>
    [Obsolete("写的不好,不再使用")]
    public class FileItem
    {
        /// <summary>
        /// 相对路径
        /// </summary>
        public string relativePath;

        /// <summary>
        /// 这个文件可能对应了一个id
        /// </summary>
        public int id;

        /// <summary>
        /// 这个文件的当前MD5值
        /// </summary>
        public string MD5;

        /// <summary>
        /// 这个文件可能对应了一个下载地址
        /// </summary>
        public string url;

        /// <summary>
        /// 这个文件可能有一个备注信息
        /// </summary>
        public string info;

        /// <summary>
        /// 它应该的正确的MD5，用于在下载前得到。
        /// </summary>
        public string MD5Correct;

        /// <summary>
        /// 这个文件可能有一个下载了一半的临时文件(临时文件的相对路径)。
        /// 一般可以.temp
        /// </summary>
        public string tempFilePath;


    }
}
