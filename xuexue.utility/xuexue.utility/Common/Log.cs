using System;
using System.Diagnostics;

namespace xuexue.utility
{
    /// <summary>
    /// 这个模块的日志对外接口,添加对应log事件即可对外输出日志。
    /// </summary>
    public static class Debug
    {
        public static event Action<string> LogDebugEvent;
        public static event Action<string> LogInfoEvent;
        public static event Action<string> LogWarningEvent;
        public static event Action<string> LogErrorEvent;

        /// <summary>
        /// 设置全部输出到控制台
        /// </summary>
        public static void SetConsoleLog()
        {
            Debug.LogInfoEvent += (msg) => Console.WriteLine(msg);
            Debug.LogWarningEvent += (msg) => Console.WriteLine(msg);
            Debug.LogErrorEvent += (msg) => Console.WriteLine(msg);
            Debug.LogDebugEvent += (msg) => Console.WriteLine(msg);
        }

        public static void Cleanup()
        {
            Debug.LogInfoEvent = null;
            Debug.LogWarningEvent = null;
            Debug.LogErrorEvent = null;
            Debug.LogDebugEvent = null;
        }

        public static void LogDebug(string msg) => LogDebugEvent?.Invoke(msg);
        public static void Log(string msg) => LogInfoEvent?.Invoke(msg);
        public static void LogWarning(string msg) => LogWarningEvent?.Invoke(msg);
        public static void LogError(string msg) => LogErrorEvent?.Invoke(msg);
    }
}
