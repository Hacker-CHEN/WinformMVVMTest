using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UW.Common
{
    public class CsvFileHelper
    {
        public static void SaveDtToCSV(DataTable dt, string fullPath)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(fullPath);
                if (!fileInfo.Directory.Exists)
                {
                    fileInfo.Directory.Create();
                }
                else
                {
                    fileInfo.Directory.Attributes = fileInfo.Directory.Attributes & ~FileAttributes.Hidden & FileAttributes.System;
                    if (fileInfo.Exists)
                        fileInfo.Attributes = ~(FileAttributes.ReadOnly | FileAttributes.Hidden | FileAttributes.System);
                }
                using (FileStream fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                {
                    using (StreamWriter streamWriter = new StreamWriter((Stream)fileStream, Encoding.Default))
                    {
                        string str1 = "";
                        for (int index = 0; index < dt.Columns.Count; ++index)
                        {
                            str1 += dt.Columns[index].ColumnName.ToString();
                            if (index < dt.Columns.Count - 1)
                                str1 += ",";
                        }
                        streamWriter.WriteLine(str1);
                        for (int index = 0; index < dt.Rows.Count; ++index)
                        {
                            string str2 = "";
                            for (int columnIndex = 0; columnIndex < dt.Columns.Count; ++columnIndex)
                            {
                                string source = dt.Rows[index][columnIndex].ToString().Replace("\"", "\"\"");
                                if (source.Contains<char>(',') || source.Contains<char>('"') || source.Contains<char>('\r') || source.Contains<char>('\n'))
                                    source = string.Format("\"{0}\"", (object)source);
                                str2 += source;
                                if (columnIndex < dt.Columns.Count - 1)
                                    str2 += ",";
                            }
                            streamWriter.WriteLine(str2);
                        }
                        File.SetAttributes(fullPath, FileAttributes.ReadOnly);
                        streamWriter.Close();
                        fileInfo.Attributes = fileInfo.Attributes & ~FileAttributes.System & ~FileAttributes.Hidden & ~FileAttributes.ReadOnly;
                        fileStream.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex.ToString().Replace("'", "").Replace("\"", ""));
            }
        }

        public static void SaveDrToCSV(DataRow dr, string fullPath)
        {
            try
            {
                string str1 = "";
                FileInfo fileInfo = new FileInfo(fullPath);
                if (!fileInfo.Directory.Exists)
                    fileInfo.Directory.Create();
                if (!File.Exists(fullPath))
                {
                    FileStream fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
                    StreamWriter streamWriter = new StreamWriter((Stream)fileStream, Encoding.UTF8);
                    for (int index = 0; index < dr.Table.Columns.Count; ++index)
                    {
                        str1 += dr.Table.Columns[index].ColumnName.ToString();
                        if (index < dr.Table.Columns.Count - 1)
                            str1 += ",";
                    }
                    streamWriter.WriteLine(str1);
                    streamWriter.Close();
                    fileStream.Close();
                }
                File.SetAttributes(fullPath, FileAttributes.Normal);
                using (FileStream fileStream = new FileStream(fullPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                {
                    using (StreamWriter streamWriter = new StreamWriter((Stream)fileStream))
                    {
                        string str2 = "";
                        for (int columnIndex = 0; columnIndex < dr.Table.Columns.Count; ++columnIndex)
                        {
                            string source = dr[columnIndex].ToString().Replace("\"", "\"\"");
                            if (source.Contains<char>(',') || source.Contains<char>('"') || source.Contains<char>('\r') || source.Contains<char>('\n'))
                                source = string.Format("\"{0}\"", (object)source);
                            str2 += source;
                            if (columnIndex < dr.Table.Columns.Count - 1)
                                str2 += ",";
                        }
                        streamWriter.WriteLine(str2);
                        streamWriter.Close();
                        fileStream.Close();
                    }
                }
                File.SetAttributes(fullPath, FileAttributes.ReadOnly);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex.ToString().Replace("'", "").Replace("\"", ""));
            }
        }

        public static DataTable OpenCSV(string filePath)
        {
            DataTable dataTable = new DataTable();
            if (File.Exists(filePath))
            {
                File.SetAttributes(filePath, FileAttributes.Normal);
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (StreamReader streamReader = new StreamReader((Stream)fileStream, Encoding.Default))
                    {
                        int num = 0;
                        bool flag = true;
                        string str;
                        while ((str = streamReader.ReadLine()) != null && !(str == ""))
                        {
                            if (flag)
                            {
                                string[] strArray = str.Split(',');
                                flag = false;
                                num = strArray.Length;
                                for (int index = 0; index < num; ++index)
                                {
                                    DataColumn column = new DataColumn(strArray[index]);
                                    dataTable.Columns.Add(column);
                                }
                            }
                            else
                            {
                                string[] strArray = str.Replace(",\"\"", ".\"\"").Replace("},{", "}.{").Split(',');
                                DataRow row = dataTable.NewRow();
                                for (int columnIndex = 0; columnIndex < num; ++columnIndex)
                                {
                                    strArray[columnIndex] = strArray[columnIndex].Replace(".\"\"", ",\"\"");
                                    strArray[columnIndex] = strArray[columnIndex].Replace("}.{", "},{");
                                    row[columnIndex] = (object)strArray[columnIndex];
                                }
                                dataTable.Rows.Add(row);
                            }
                        }
                        streamReader.Close();
                        fileStream.Close();
                        File.SetAttributes(filePath, FileAttributes.ReadOnly);
                    }
                }
            }
            return dataTable;
        }
    }
}
