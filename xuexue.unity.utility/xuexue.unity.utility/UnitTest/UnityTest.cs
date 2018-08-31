using System;
using System.Reflection;
using UnityEngine;

namespace xuexue.unity.utility.unittest
{
    /// <summary>
    /// Unity下的测试方法
    /// </summary>
    public class UnityTest
    {
        /// <summary>
        /// 运行所有测试
        /// </summary>
        public static void Run()
        {
            Assembly assembly = null;
            Assembly[] arr = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var item in arr)
            {
                if (item.GetName().Name == "Assembly-CSharp")
                {
                    assembly = item;
                    break;
                }
            }

            Type[] typeArr = assembly.GetTypes();

            foreach (Type t in typeArr)
            {
                if (t.IsDefined(typeof(TestClass), false))
                {
                    object obj = Activator.CreateInstance(t);
                    var methods = t.GetMethods();
                    foreach (var m in methods)
                    {
                        if (m.IsDefined(typeof(TestMethod)))
                        {
                            m.Invoke(obj, new object[] { });
                            Debug.Log($"<color=#00ff00ff>UnityTest.Run():执行测试 {t.Name}.{m.Name} 通过！</color>");
                        }
                    }
                }
            }
        }
    }
}