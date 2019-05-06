using System;
using System.Diagnostics;

namespace xuexue.utility
{
    /// <summary>
    /// 这个模块的日志对外接口,添加对应log事件即可对外输出日志。
    /// </summary>
    public static class Log
    {
        public static event Action<string> EventLogDebug = null;

        public static event Action<string> EventLogInfo = null;

        public static event Action<string> EventLogWarning = null;

        public static event Action<string> EventLogError = null;

        public static void Debug(string msg)
        {
            if (EventLogDebug != null)
            {
                EventLogDebug(msg);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(msg, "Debug");//如果没有绑定事件，那么就随便输出一下到控制台算了
            }
        }

        public static void Info(string msg)
        {
            if (EventLogInfo != null)
            {
                EventLogInfo(msg);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(msg, "Info");//如果没有绑定事件，那么就随便输出一下到控制台算了
            }
        }

        public static void Warning(string msg)
        {
            if (EventLogWarning != null)
            {
                EventLogWarning(msg);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(msg, "Warning");//如果没有绑定事件，那么就随便输出一下到控制台算了
                System.Diagnostics.Debug.Write($"StackTrace:");
                StackTrace st = new StackTrace(true);
                StackFrame[] sfs = st.GetFrames();
                int showLineNum = 6 < sfs.Length ? 6 : sfs.Length;
                for (int i = 1; i < showLineNum; i++)//因为最上一行就是这个函数，所以从i=1开始
                {
                    if (string.IsNullOrEmpty(sfs[i].GetFileName()))
                        System.Diagnostics.Debug.WriteLine($"{sfs[i].GetMethod()}");
                    else
                        System.Diagnostics.Debug.WriteLine($"{sfs[i].GetFileName()}.{sfs[i].GetMethod()}:line{sfs[i].GetFileLineNumber()}");

                }
            }
        }

        public static void Error(string msg)
        {
            if (EventLogError != null)
            {
                EventLogError(msg);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(msg, "Error");//如果没有绑定事件，那么就随便输出一下到控制台算了
                System.Diagnostics.Debug.Write($"StackTrace:");
                StackTrace st = new StackTrace(true);
                StackFrame[] sfs = st.GetFrames();
                int showLineNum = 6 < sfs.Length ? 6 : sfs.Length;
                for (int i = 1; i < showLineNum; i++)//因为最上一行就是这个函数，所以从i=1开始
                {
                    if (string.IsNullOrEmpty(sfs[i].GetFileName()))
                        System.Diagnostics.Debug.WriteLine($"{sfs[i].GetMethod()}");
                    else
                        System.Diagnostics.Debug.WriteLine($"{sfs[i].GetFileName()}.{sfs[i].GetMethod()}:line{sfs[i].GetFileLineNumber()}");

                }
            }
        }
    }
}