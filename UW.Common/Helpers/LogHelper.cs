using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UW.Common
{
    public class LogHelper
    {
        private static string logfilePath = AppDomain.CurrentDomain.BaseDirectory + "log\\";

        static LogHelper() => LogHelper.DeleteFiles();

        private static void DeleteFiles()
        {
            if (!Directory.Exists(LogHelper.logfilePath))
                return;
            foreach (string directory in Directory.GetDirectories(LogHelper.logfilePath))
            {
                foreach (string file in Directory.GetFiles(directory))
                {
                    if ((DateTime.Now - File.GetCreationTime(file)).TotalDays > 90.0)
                    {
                        try
                        {
                            File.Delete(file);
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }

        public static void LogRuning(string message)
        {
            LogHelper.WriteTextLog(LogHelper.logfilePath + "运行日志", message);
            LogHelper.WriteAllTextLog(LogHelper.logfilePath + "综合日志", message, "运行日志");
        }

        public static void LogInteraction(string message, LogType logLevel = LogType.Info)
        {
            LogHelper.WriteTextLog(LogHelper.logfilePath + "交互日志", "【" + logLevel.GetDescription() + "】" + message);
            LogHelper.WriteAllTextLog(LogHelper.logfilePath + "综合日志", "【" + logLevel.GetDescription() + "】" + message, "交互日志");
        }

        public static void LogMES(string message)
        {
            LogHelper.WriteTextLog(LogHelper.logfilePath + "接口调用日志", message);
            LogHelper.WriteAllTextLog(LogHelper.logfilePath + "综合日志", message, "接口调用日志");
        }

        public static void LogOperation(string message)
        {
            LogHelper.WriteTextLog(LogHelper.logfilePath + "操作日志", message);
            LogHelper.WriteAllTextLog(LogHelper.logfilePath + "综合日志", message, "操作日志");
        }

        public static void LogError(string message)
        {
            LogHelper.WriteTextLog(LogHelper.logfilePath + "异常错误日志", message);
            LogHelper.WriteAllTextLog(LogHelper.logfilePath + "综合日志", message, "异常错误日志");
        }

        private static void WriteTextLog(string path, string logText)
        {
            string str = DateTime.Now.ToString("yyyyMMdd") + ".txt";
            StringBuilder stringBuilder = new StringBuilder();
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            stringBuilder.Append(string.Format("{0}\\{1}", (object)path, (object)str));
            using (FileStream fileStream = new FileStream(stringBuilder.ToString(), FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            {
                using (StreamWriter streamWriter = new StreamWriter((Stream)fileStream, Encoding.Default))
                {
                    logText = "[" + DateTime.Now.ToString("yyyyMMdd HH:mm:ss fff") + "] " + logText;
                    streamWriter.WriteLine(logText);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
            }
        }

        /// <summary>
        /// 保存全部日志
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="logText">日志内容</param>
        /// <param name="type">类型</param>
        private static void WriteAllTextLog(string path, string logText, string type)
        {
            string str = $"{DateTime.Now.ToString("yyyyMMdd")}.txt";
            StringBuilder stringBuilder = new StringBuilder();
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            stringBuilder.Append(string.Format("{0}\\{1}", (object)path, (object)str));
            using (FileStream fileStream = new FileStream(stringBuilder.ToString(), FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            {
                using (StreamWriter streamWriter = new StreamWriter((Stream)fileStream, Encoding.Default))
                {
                    logText = $"[{DateTime.Now.ToString("yyyyMMdd HH:mm:ss fff")}-{type}] " + logText;
                    streamWriter.WriteLine(logText);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
            }
        }
    }
}
