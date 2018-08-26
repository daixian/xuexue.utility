using System;
using System.Collections;
using System.Reflection;

namespace xuexue.LitJson
{
    /// <summary>
    /// 一个有爱的Json扩展类。写了一大半，发现了另外一条路= =!
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field)]
    [Obsolete]
     class xuexueJsonTest : Attribute
    {
        //是否注册过了float
        private static bool isRegisterType = false;

        /// <summary>
        /// 注册float
        /// </summary>
        private static void RegisterType()
        {
            if (!isRegisterType)
            {
                isRegisterType = true;
                JsonMapper.RegisterExporter<float>((obj, writer) => writer.Write(Convert.ToDouble(obj)));
                JsonMapper.RegisterImporter<double, float>(input => Convert.ToSingle(input));
            }
        }

        /// <summary>
        /// obj->Json
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [Obsolete]
        public static string ToJson(object obj)
        {
            return ToJson(obj, null, true);
        }

        /// <summary>
        /// Json->obj
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        [Obsolete]
        public static T ToObject<T>(string str)
        {
            return (T)ToObject(str, typeof(T), null);
        }

        private static string ToJson(object obj, JsonWriter jw = null, bool isToString = true)
        {
            if (obj == null)
            {
                return null;
            }

            if (jw == null)
                jw = new JsonWriter();

            Type objType = obj.GetType();

            if (!objType.IsDefined(typeof(xuexueJsonTest), false))//第二个参数表示不搜索继承链
            {
                //如果这个类没有加属性
               // throw new Exception($"这个类{objType}没有定义xuexueJson！");
            }
            jw.WriteObjectStart();
            jw.WritePropertyName("xuexue");

            jw.WriteArrayStart();

            //所有的字段的支持
            FieldInfo[] fis = objType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            foreach (FieldInfo fi in fis)
            {
                if (fi.IsDefined(typeof(xuexueJsonTest), false))
                {
                    if (fi.FieldType == typeof(bool))
                    {
                        jw.WriteObjectStart();
                        jw.WritePropertyName(fi.Name);
                        jw.Write((bool)fi.GetValue(obj));
                        jw.WriteObjectEnd();
                    }
                    else if (fi.FieldType == typeof(decimal))
                    {
                        jw.WriteObjectStart();
                        jw.WritePropertyName(fi.Name);
                        jw.Write((decimal)fi.GetValue(obj));
                        jw.WriteObjectEnd();
                    }
                    else if (fi.FieldType == typeof(double))
                    {
                        jw.WriteObjectStart();
                        jw.WritePropertyName(fi.Name);
                        jw.Write((double)fi.GetValue(obj));
                        jw.WriteObjectEnd();
                    }
                    else if (fi.FieldType == typeof(int))
                    {
                        jw.WriteObjectStart();
                        jw.WritePropertyName(fi.Name);
                        jw.Write((int)fi.GetValue(obj));
                        jw.WriteObjectEnd();
                    }
                    else if (fi.FieldType == typeof(long))
                    {
                        jw.WriteObjectStart();
                        jw.WritePropertyName(fi.Name);
                        jw.Write((long)fi.GetValue(obj));
                        jw.WriteObjectEnd();
                    }
                    else if (fi.FieldType == typeof(string))
                    {
                        jw.WriteObjectStart();
                        jw.WritePropertyName(fi.Name);
                        jw.Write(fi.GetValue(obj) as string);
                        jw.WriteObjectEnd();
                    }
                    else if (fi.FieldType == typeof(ulong))
                    {
                        jw.WriteObjectStart();
                        jw.WritePropertyName(fi.Name);
                        jw.Write((ulong)fi.GetValue(obj));
                        jw.WriteObjectEnd();
                    }
                    else if (fi.FieldType.IsDefined(typeof(xuexueJsonTest), false))//如果这是一个定义了xuexue的类
                    {
                        jw.WriteObjectStart();
                        jw.WritePropertyName(fi.Name);
                        object co = fi.GetValue(obj);
                        if (co == null)
                            jw.Write(null);
                        else
                            ToJson(co, jw, false);
                        jw.WriteObjectEnd();
                    }
                    else if (fi.FieldType.BaseType == typeof(System.Array))//数组的支持
                    {
                        jw.WriteObjectStart();
                        jw.WritePropertyName(fi.Name);
                        Array arr = fi.GetValue(obj) as Array;
                        if (arr == null)//如果这项为空那么就直接写空
                        {
                            jw.Write(null);
                        }
                        else
                        {
                            jw.WriteArrayStart();
                            int index = 0;
                            foreach (var co in arr)
                            {
                                if (co == null)
                                {
                                    jw.WriteObjectStart();
                                    jw.WritePropertyName(index.ToString());
                                    jw.Write(null);
                                    jw.WriteObjectEnd();
                                }
                                else
                                {
                                    Type coType = co.GetType();
                                    if (coType == typeof(bool))
                                    {
                                        jw.WriteObjectStart();
                                        jw.WritePropertyName(index.ToString());
                                        jw.Write((bool)co);
                                        jw.WriteObjectEnd();
                                    }
                                    else if (coType == typeof(decimal))
                                    {
                                        jw.WriteObjectStart();
                                        jw.WritePropertyName(index.ToString());
                                        jw.Write((decimal)co);
                                        jw.WriteObjectEnd();
                                    }
                                    else if (coType == typeof(double))
                                    {
                                        jw.WriteObjectStart();
                                        jw.WritePropertyName(index.ToString());
                                        jw.Write((double)co);
                                        jw.WriteObjectEnd();
                                    }
                                    else if (coType == typeof(int))
                                    {
                                        jw.WriteObjectStart();
                                        jw.WritePropertyName(index.ToString());
                                        jw.Write((int)co);
                                        jw.WriteObjectEnd();
                                    }
                                    else if (coType == typeof(long))
                                    {
                                        jw.WriteObjectStart();
                                        jw.WritePropertyName(index.ToString());
                                        jw.Write((long)co);
                                        jw.WriteObjectEnd();
                                    }
                                    else if (coType == typeof(string))
                                    {
                                        jw.WriteObjectStart();
                                        jw.WritePropertyName(index.ToString());
                                        jw.Write(co as string);
                                        jw.WriteObjectEnd();
                                    }
                                    else if (coType == typeof(ulong))
                                    {
                                        jw.WriteObjectStart();
                                        jw.WritePropertyName(index.ToString());
                                        jw.Write((ulong)co);
                                        jw.WriteObjectEnd();
                                    }
                                    else if (coType.IsDefined(typeof(xuexueJsonTest), false))//如果这是一个定义了xuexue的类
                                    {
                                        if (co == null)
                                        {
                                            jw.WriteObjectStart();
                                            jw.WritePropertyName(index.ToString());
                                            jw.Write(null);
                                            jw.WriteObjectEnd();
                                        }
                                        else
                                        {
                                            jw.WriteObjectStart();
                                            jw.WritePropertyName(index.ToString());
                                            ToJson(co, jw, false);
                                            jw.WriteObjectEnd();
                                        }
                                    }
                                }
                                index++;
                            }
                            jw.WriteArrayEnd();
                        }
                        jw.WriteObjectEnd();
                    }
                }
            }

            //所有的属性的支持
            PropertyInfo[] pis = objType.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            foreach (PropertyInfo pi in pis)
            {
                if (pi.IsDefined(typeof(xuexueJsonTest), false))
                {
                    if (pi.PropertyType == typeof(bool))
                    {
                        jw.WriteObjectStart();
                        jw.WritePropertyName(pi.Name);
                        jw.Write((bool)pi.GetValue(obj));
                        jw.WriteObjectEnd();
                    }
                    else if (pi.PropertyType == typeof(decimal))
                    {
                        jw.WriteObjectStart();
                        jw.WritePropertyName(pi.Name);
                        jw.Write((decimal)pi.GetValue(obj));
                        jw.WriteObjectEnd();
                    }
                    else if (pi.PropertyType == typeof(double))
                    {
                        jw.WriteObjectStart();
                        jw.WritePropertyName(pi.Name);
                        jw.Write((double)pi.GetValue(obj));
                        jw.WriteObjectEnd();
                    }
                    else if (pi.PropertyType == typeof(int))
                    {
                        jw.WriteObjectStart();
                        jw.WritePropertyName(pi.Name);
                        jw.Write((int)pi.GetValue(obj));
                        jw.WriteObjectEnd();
                    }
                    else if (pi.PropertyType == typeof(long))
                    {
                        jw.WriteObjectStart();
                        jw.WritePropertyName(pi.Name);
                        jw.Write((long)pi.GetValue(obj));
                        jw.WriteObjectEnd();
                    }
                    else if (pi.PropertyType == typeof(string))
                    {
                        jw.WriteObjectStart();
                        jw.WritePropertyName(pi.Name);
                        jw.Write(pi.GetValue(obj) as string);
                        jw.WriteObjectEnd();
                    }
                    else if (pi.PropertyType == typeof(ulong))
                    {
                        jw.WriteObjectStart();
                        jw.WritePropertyName(pi.Name);
                        jw.Write((ulong)pi.GetValue(obj));
                        jw.WriteObjectEnd();
                    }
                    else if (pi.PropertyType.IsDefined(typeof(xuexueJsonTest), false))//如果这是一个定义了xuexue的类
                    {
                        jw.WriteObjectStart();
                        jw.WritePropertyName(pi.Name);
                        object co = pi.GetValue(obj);
                        if (co == null)
                            jw.Write(null);
                        else
                            ToJson(co, jw, false);
                        jw.WriteObjectEnd();
                    }
                    else if (pi.PropertyType.BaseType == typeof(System.Array))//数组的支持
                    {
                        jw.WriteObjectStart();
                        jw.WritePropertyName(pi.Name);
                        jw.WriteArrayStart();
                        Array arr = pi.GetValue(obj) as Array;
                        int index = 0;
                        foreach (var co in arr)
                        {
                            Type coType = co.GetType();
                            if (coType == typeof(bool))
                            {
                                jw.WriteObjectStart();
                                jw.WritePropertyName(index.ToString());
                                jw.Write((bool)co);
                                jw.WriteObjectEnd();
                            }
                            else if (coType == typeof(decimal))
                            {
                                jw.WriteObjectStart();
                                jw.WritePropertyName(index.ToString());
                                jw.Write((decimal)co);
                                jw.WriteObjectEnd();
                            }
                            else if (coType == typeof(double))
                            {
                                jw.WriteObjectStart();
                                jw.WritePropertyName(index.ToString());
                                jw.Write((double)co);
                                jw.WriteObjectEnd();
                            }
                            else if (coType == typeof(int))
                            {
                                jw.WriteObjectStart();
                                jw.WritePropertyName(index.ToString());
                                jw.Write((int)co);
                                jw.WriteObjectEnd();
                            }
                            else if (coType == typeof(long))
                            {
                                jw.WriteObjectStart();
                                jw.WritePropertyName(index.ToString());
                                jw.Write((long)co);
                                jw.WriteObjectEnd();
                            }
                            else if (coType == typeof(string))
                            {
                                jw.WriteObjectStart();
                                jw.WritePropertyName(index.ToString());
                                jw.Write(co as string);
                                jw.WriteObjectEnd();
                            }
                            else if (coType == typeof(ulong))
                            {
                                jw.WriteObjectStart();
                                jw.WritePropertyName(index.ToString());
                                jw.Write((ulong)co);
                                jw.WriteObjectEnd();
                            }
                            else if (coType.IsDefined(typeof(xuexueJsonTest), false))//如果这是一个定义了xuexue的类
                            {
                                if (co == null)
                                {
                                    jw.WriteObjectStart();
                                    jw.WritePropertyName(index.ToString());
                                    jw.Write(null);
                                    jw.WriteObjectEnd();
                                }
                                else
                                {
                                    jw.WriteObjectStart();
                                    jw.WritePropertyName(index.ToString());
                                    ToJson(co, jw, false);
                                    jw.WriteObjectEnd();
                                }
                            }

                            index++;
                        }
                        jw.WriteArrayEnd();
                        jw.WriteObjectEnd();
                    }
                }
            }

            jw.WriteArrayEnd();
            jw.WriteObjectEnd();

            if (isToString)
                return jw.TextWriter.ToString();
            else
                return null;
        }

        private static object ToObject(string str, Type type, JsonReader jr = null)
        {
            if (jr == null)
                jr = new JsonReader(str);

            int objStack = 0;
            object obj = null;
            if (jr.Token == JsonToken.None)
                jr.Read();
            if (jr.Token == JsonToken.ObjectStart)
            {
                jr.Read();
                if (jr.Token == JsonToken.PropertyName &&
                    jr.Value.ToString() == "xuexue")
                {
                    obj = type.Assembly.CreateInstance(type.FullName);
                    jr.Read();
                    if (jr.Token == JsonToken.ArrayStart)//读掉xuexueObj自己的ArrayStart
                    {
                        objStack++;
                    }
                }
            }
            if (obj == null)
            {
                throw new Exception("xuexueJson.ToObject():类PropertyName有错误！");
            }

            FieldInfo[] fis = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            PropertyInfo[] pis = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);

            while (!jr.EndOfJson)
            {
                bool isReadMember = false;
                jr.Read();
                if (jr.Token == JsonToken.ArrayStart)
                {
                    objStack++;
                }
                else if (jr.Token == JsonToken.ArrayEnd)
                {
                    objStack--;
                    if (objStack == 0)
                    {
                        return obj;
                    }
                }
                else if (jr.Token == JsonToken.ObjectStart)
                {
                    jr.Read();
                    if (jr.Token == JsonToken.PropertyName)
                    {
                        foreach (var fi in fis)
                        {
                            if (fi.Name == jr.Value.ToString())
                            {
                                jr.Read();

                                if (jr.Token == JsonToken.Int ||
                                    jr.Token == JsonToken.Long ||
                                    jr.Token == JsonToken.Double ||
                                    jr.Token == JsonToken.String ||
                                    jr.Token == JsonToken.Boolean)
                                {
                                    fi.SetValue(obj, jr.Value);
                                }
                                else if (jr.Token == JsonToken.Null)
                                {
                                    fi.SetValue(obj, null);
                                }
                                else if (jr.Token == JsonToken.ObjectStart)
                                {
                                    fi.SetValue(obj, ToObject(null, fi.FieldType, jr));
                                }
                                else if (jr.Token == JsonToken.ArrayStart)//如果这个元素是一个数组
                                {
                                    objStack++;
                                    Type et = fi.FieldType.GetElementType();
                                    ArrayList list = new ArrayList();

                                    while (jr.Token != JsonToken.ArrayEnd)
                                    {
                                        jr.Read();
                                        if (jr.Token == JsonToken.ObjectStart)
                                        {
                                            jr.Read();
                                            jr.Read();
                                            if (jr.Token == JsonToken.Int ||
                                                jr.Token == JsonToken.Long ||
                                                jr.Token == JsonToken.Double ||
                                                jr.Token == JsonToken.String ||
                                                jr.Token == JsonToken.Boolean)
                                            {
                                                list.Add(jr.Value);
                                            }
                                            else if (jr.Token == JsonToken.Null)
                                            {
                                                list.Add(null);
                                            }
                                            else if (jr.Token == JsonToken.ObjectStart)
                                            {
                                                list.Add(ToObject(null, et, jr));
                                                jr.Read();//读掉一个ArrayEnd
                                            }
                                        }
                                    }
                                    objStack--;

                                    Array array = Array.CreateInstance(et, list.Count);
                                    for (int i = 0; i < list.Count; i++)
                                    {
                                        array.SetValue(list[i], i);
                                    }
                                    fi.SetValue(obj, array);
                                }
                                isReadMember = true;//已经找到了成员
                                break;
                            }
                        }

                        if (isReadMember) { continue; }//字段找到了就不用再找属性了

                        foreach (var pi in pis)
                        {
                            if (pi.Name == jr.Value.ToString())
                            {
                                jr.Read();
                                if (jr.Token == JsonToken.Int ||
                                    jr.Token == JsonToken.Long ||
                                    jr.Token == JsonToken.Double ||
                                    jr.Token == JsonToken.String ||
                                    jr.Token == JsonToken.Boolean)
                                {
                                    pi.SetValue(obj, jr.Value);
                                }
                                else if (jr.Token == JsonToken.Null)
                                {
                                    pi.SetValue(obj, null);
                                }
                                else if (jr.Token == JsonToken.ObjectStart)
                                {
                                    pi.SetValue(obj, ToObject(null, pi.PropertyType, jr));
                                }
                                break;
                            }
                        }
                    }
                }
            }

            return obj;
        }
    }
}