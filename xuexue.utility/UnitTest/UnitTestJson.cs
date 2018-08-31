using xuexue.file;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using xuexue.LitJson;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using System;
using xuexue.crypto;

namespace UnitTest
{
    public class TestLitJson
    {
        public int a;

        public TestLitJson2 b;

        public TestLitJson3 c;
        public List<TestLitJson3> d;

        public enum Type
        {
            QuickPhoto,
            ChoiceQuesion,
            AnswerQuesion
        }
        public Type type;
    }

    public class TestLitJson2
    {
        public int a;
        public float b;
        public int c;
    }

    public class TestLitJson3
    {
        public int a;

        [xuexueJson(priority =1)]
        public float b;

        [xuexueJsonIgnore]
        public int c;

        [xuexueJson]
        public GameObject go;

        public List<GameObject> goList;

        public GameObject[] goArr;

        [xuexueJson]
        public DateTime dt;

        [xuexueJsonIgnore]
        public int testpriority;

        [xuexueJson(priority = -10)]
        public int testpriority2;

        [xuexueJson(priority =10)]
        private bool AutoUpdateBeforeToJson
        {
            set { }
            get
            {
                if (testpriority != -1)
                {
                    throw new Exception("优先级测试失败！");
                }
                return true;
            }
        }

        [xuexueJson(priority = -1)]
        private bool AutoUpdateBeforeToJson2
        {
            set { }
            get
            {
                testpriority2 = 999;
                testpriority = -1;
                return true;
            }
        }
    }

    public class TestLitJson3IM 
    {
        public string data;

        public void OverrideTarget(object targetObj)
        {
             
        }

        public void InitWithTarget(object targetObj)
        {
            TestLitJson3 o = targetObj as TestLitJson3;
            data = "data" + o.a;          
        }

        public object ToTarget()
        {
            TestLitJson3 to = new TestLitJson3();
           
            return to;
        }
    }

    [xuexueJsonClass(defaultFieldConstraint = false, defaultPropertyConstraint = false)]
    public class TestLitJson4
    {
        [xuexueJson]
        public int a { get; set; }
         
    }

    [TestClass]
    public class UnitTestJson
    {
        /// <summary>
        /// 测试Json的序列化和反序列
        /// </summary>
        [TestMethod]
        public void TestMethod_FileItemToJson()
        {
            TestLitJson tlj = new TestLitJson() { a = 1, b = new TestLitJson2(), c = new TestLitJson3() { a = 123, b = 345 }, d = new List<TestLitJson3>(),
                type = TestLitJson.Type.ChoiceQuesion  };

            string str = JsonMapper.ToJson(tlj);
            TestLitJson t = JsonMapper.ToObject<TestLitJson>(str);

            //FileItem fi = new FileItem
            //{
            //    relativePath = "C:/123/123",
            //    url = "http://test"
            //};
            //RootDir rd = new RootDir();
            //rd.AddItem(fi);
            //string json = XueXueJson.ToJson(rd);

            //RootDir rd2 = XueXueJson.FromJson<RootDir>(json);
            //Assert.IsTrue(rd2.dict.Count == 1);
            //Assert.IsTrue(rd2.dict[fi.relativePath].url == fi.url);
        }


        //[TestMethod]
        //public void TestMethod_JsonTest()
        //{
        //    TestClass tc = new TestClass()
        //    {
        //        arr = new int[3] { 1, 2, 3 },
        //        arr2 = null,
        //        arr3 = new TestClass00[3] { new TestClass00() { a = 1 }, new TestClass00() { a = 2 }, null },
        //        list = new List<int>() { 666, 777 },
        //        a = 123,
        //        b = 345,
        //        c = 456,
        //        d = "123",
        //        e = null,
        //        f = new TestClass00() { a = 666 }
        //    };
        //    string str = xuexueJsonTest.ToJson(tc);
        //    TestClass tc2 = xuexueJsonTest.ToObject<TestClass>(str);
        //    Assert.IsTrue(tc.a == tc2.a);
        //    Assert.IsTrue(tc.b == tc2.b);
        //    Assert.IsTrue(tc.c == tc2.c);
        //    Assert.IsTrue(tc.d == tc2.d);
        //    Assert.IsTrue(tc.e == tc2.e);
        //    Assert.IsTrue(tc.f.a == tc2.f.a);

        //    Assert.IsTrue(tc.arr[0] == tc2.arr[0]);
        //    Assert.IsTrue(tc.arr[1] == tc2.arr[1]);
        //    Assert.IsTrue(tc.arr[2] == tc2.arr[2]);
        //    Assert.IsTrue(tc.arr2 == tc2.arr2);
        //    Assert.IsTrue(tc.arr3[0].a == tc2.arr3[0].a);
        //    Assert.IsTrue(tc.arr3[1].a == tc2.arr3[1].a);
        //    Assert.IsTrue(tc.arr3[2] == tc2.arr3[2]);
        //}


        [TestMethod]
        public void TestMethod_UnityTest()
        {
            JsonTypeRegister.BindType(typeof(Vector3), new xuexueJsonClass("x", "y", "z") { defaultFieldConstraint = false });
            JsonTypeRegister.AddIgnoreClass(typeof(GameObject));

            TestLitJson4 tlj4 = new TestLitJson4() { a = 12 };
            string str4 = JsonMapper.ToJson(tlj4);
            TestLitJson4 tlj4_1 = JsonMapper.ToObject<TestLitJson4>(str4);
            Assert.IsTrue(tlj4.a == tlj4_1.a);

            float t1b = 2;
            TestLitJson3 t1 = new TestLitJson3() { a = 1, b = t1b, c = 3, dt = DateTime.Now, testpriority2 = 5 };
            TestLitJson3 t2 = new TestLitJson3() { a = 5, b = 6, c = 7 };

            string str = JsonMapper.ToJson(t1);
            TestLitJson3 rest1 = JsonMapper.ToObject<TestLitJson3>(str);

            Assert.IsTrue(rest1.testpriority2 == 5);

            JsonMapper.OverrideObject(str, t2);
            Assert.IsTrue(t2.a == t1.a);
            Assert.IsTrue(t2.b == t1.b);
            Assert.IsTrue(t2.c == 7);
            Assert.IsTrue(t2.dt.ToShortDateString() == t1.dt.ToShortDateString());

            Vector3 v3 = new Vector3(1, 2, 3);
            Vector3 v31 = new Vector3(2, 2, 2);      
            Vector3 v3_2 = JsonMapper.ToObject<Vector3>(JsonMapper.ToJson(v3));
            Assert.IsTrue(v3.x == v3_2.x);
            Assert.IsTrue(v3.y == v3_2.y);
            Assert.IsTrue(v3.z == v3_2.z);

            //string str = JsonMapper.ToJson(v3);
        }

    }
}