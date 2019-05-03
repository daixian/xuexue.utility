using System;
using System.Collections.Generic;

namespace xuexue.LitJson
{
    /// <summary>
    /// 要转化的对象类型和json之间的中间媒介类。这个类应该有一个无参构造
    /// </summary>
    [Obsolete("这个类好像还没写完")]
    public interface IIntermediary
    {
        /// <summary>
        /// 传入一个目标对象，填充自己的值。
        /// </summary>
        /// <returns></returns>
        void InitWithTarget(object targetObj);

        /// <summary>
        /// 此时这个媒介类对象已经有值，构造到一个目标对象。
        /// </summary>
        /// <returns>构造到一个目标对象</returns>
        object ToTarget();

        /// <summary>
        /// 重写一个已有的对象。
        /// </summary>
        /// <returns></returns>
        void OverrideTarget(object targetObj);
    }
}