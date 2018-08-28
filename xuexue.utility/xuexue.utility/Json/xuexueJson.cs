﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace xuexue.LitJson
{
    /// <summary>
    /// 一个有爱且好用的LitJson扩展类。
    /// 这是一个Json行为的策略配置，可以标记在类的上面，也注册到到一个字典中。
    /// 没有写的话就是默认配置，和写了之后没有专门设置是一样的。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class xuexueJsonClass : Attribute
    {
        /// <summary>
        /// 默认构造
        /// </summary>
        public xuexueJsonClass()
        {
        }

        /// <summary>
        /// 使用使能的成员名来构造
        /// </summary>
        /// <param name="enableMembers"></param>
        public xuexueJsonClass(params string[] enableMembers)
        {
            this.enableMembers = new List<string>(enableMembers);
        }

        /// <summary>
        /// 默认是否反射字段(默认是true)
        /// </summary>
        public bool defaultFieldConstraint = true;

        /// <summary>
        ///  默认是否反射属性(默认是false)
        /// </summary>
        public bool defaultPropertyConstraint = false;

        /// <summary>
        /// 字段的反射Flags
        /// </summary>
        public BindingFlags fieldflags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;

        /// <summary>
        /// 属性的反射Flags
        /// </summary>
        public BindingFlags propertyflags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;


        #region 附加条件

        /// <summary>
        /// 特别要使能的成员（成员名），如果它不为null,那么只有这个数组中的成员被使能
        /// </summary>
        public List<string> enableMembers = null;

        /// <summary>
        /// 特别要忽略的成员（成员名），如果它不为null,那么这个数组中的成员一定被忽略
        /// </summary>
        public List<string> ignoreMembers = null;

        #endregion 附加条件

        /// <summary>
        /// 媒介类型，它必须要带有IIntermediary
        /// </summary>
        //public Type intermediary = null;
    }

    /// <summary>
    /// 经过xuexueJsonClass的配置策略筛选出来之后，只有带这个xuexueJson的字段和属性才会被生效。
    /// 条件优先级 enableMembers > ignoreMembers > xuexueJson > xuexueJsonIgnore > defaultMemberConstraint
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class xuexueJson : Attribute
    {
    }

    /// <summary>
    /// 如果标记了xuexueJsonIgnore，那么这个成员被约束为忽略.
    /// 条件优先级 enableMembers > ignoreMembers > xuexueJson > xuexueJsonIgnore >defaultMemberConstraint
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class xuexueJsonIgnore : Attribute
    {
    }

    /// <summary>
    /// 注册类的配置类型
    /// </summary>
    public static class JsonTypeRegister
    {
        static JsonTypeRegister()
        {
            JsonMapper.RegisterExporter<float>((obj, writer) => writer.Write(Convert.ToDouble(obj)));
            JsonMapper.RegisterImporter<double, float>(input => Convert.ToSingle(input));
        }

        /// <summary>
        /// 配置字典
        /// </summary>
        internal static Dictionary<Type, xuexueJsonClass> dictTypeXXJC = new Dictionary<Type, xuexueJsonClass>();

        /// <summary>
        /// 要忽略的类的列表
        /// </summary>
        internal static List<Type> listIgnoreClass = new List<Type>();

        /// <summary>
        /// 当前默认使用的类Attribute，不可以将它置为null，可以修改里面的内容。但是要保证序列化和反序列化的时候此处配置一致。即整个项目设置一致。
        /// </summary>
        public static xuexueJsonClass defaultClassAttribute = new xuexueJsonClass();

        /// <summary>
        /// 给一个类型绑定一个Json设置
        /// </summary>
        /// <param name="type"></param>
        /// <param name="xxjsonClass"></param>
        public static void BindType(Type type, xuexueJsonClass xxjsonClass)
        {
            if (!dictTypeXXJC.ContainsKey(type))
            {
                dictTypeXXJC.Add(type, xxjsonClass);
            }
            else
            {
                dictTypeXXJC[type] = xxjsonClass;
            }
        }

        /// <summary>
        /// 添加一个要忽略的类型
        /// </summary>
        /// <param name="type"></param>
        public static void AddIgnoreClass(Type type)
        {
            if (!listIgnoreClass.Contains(type))
            {
                listIgnoreClass.Add(type);
            }
        }
    }
}