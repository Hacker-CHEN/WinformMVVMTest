using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UW.Common
{
    public class RecordHelper
    {
        private static string recordFilePath = AppDomain.CurrentDomain.BaseDirectory + "record\\";

        static RecordHelper()
        {
            RecordHelper.DeleteFiles();
        }

        private static void DeleteFiles()
        {
            if (!Directory.Exists(RecordHelper.recordFilePath))
                return;
            foreach (string directory in Directory.GetDirectories(RecordHelper.recordFilePath))
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

        private static void WriteTextLog(string path, string logText, string trackId)
        {
            string str = DateTime.Now.ToString("yyyyMMdd") + ".txt";
            StringBuilder stringBuilder = new StringBuilder();
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            stringBuilder.Append($"{path}\\{str}");
            using (FileStream fileStream = new FileStream(stringBuilder.ToString(), FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            {
                using (StreamWriter streamWriter = new StreamWriter((Stream)fileStream, Encoding.Default))
                {
                    logText = $"[{DateTime.Now.ToString("yyyyMMdd HH:mm:ss fff")}] 产品码：{logText}，进站通道：{trackId}";
                    streamWriter.WriteLine(logText);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
            }
        }

        public static void RecordTrackIn(string message, string trackInId)
        {
            RecordHelper.WriteTextLog($"{RecordHelper.recordFilePath}进站历史记录", message, trackInId);
        }

        public static void RecordTrackOut(string message, string trackOutId)
        {
            RecordHelper.WriteTextLog($"{RecordHelper.recordFilePath}出站历史记录", message, trackOutId);
        }
    }
}
