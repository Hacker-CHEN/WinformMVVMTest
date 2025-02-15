using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UW.Common
{
    public static class DataSet2Object
    {
        public static List<T> ToList<T>(this DataTable dt) => dt.DataToList<T>();

        public static string ToJsonList(this DataTable dt, string timeFormat = "yyyy-MM-dd HH:mm:ss")
        {
            return DataSet2Object.DataToJsonList(dt, timeFormat);
        }

        public static string DataToJsonList(DataSet ds)
        {
            return ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0 ? "[]" : DataSet2Object.DataToJsonList(ds.Tables[0]);
        }

        public static string DataToJsonList(DataTable dt, string timeFormat = "yyyy-MM-dd HH:mm:ss")
        {
            if (dt == null || dt.Rows.Count == 0)
                return "[]";
            StringBuilder stringBuilder = new StringBuilder();
            for (int index = 0; index < dt.Rows.Count; ++index)
            {
                if (stringBuilder.Length > 0)
                    stringBuilder.Append(",");
                stringBuilder.Append(DataSet2Object.DataToJsonObj(dt.Rows[index], timeFormat));
            }
            stringBuilder.Insert(0, "[");
            stringBuilder.Append("]");
            return stringBuilder.ToString();
        }

        public static string DataToJsonObj(DataRow dr, string timeFormat = "yyyy-MM-dd HH:mm:ss")
        {
            if (dr == null)
                return string.Empty;
            StringBuilder stringBuilder = new StringBuilder();
            for (int index = 0; index < dr.Table.Columns.Count; ++index)
            {
                if (stringBuilder.Length > 0)
                    stringBuilder.Append(",");
                stringBuilder.AppendFormat("\"{0}\":", (object)dr.Table.Columns[index].ColumnName);
                if (DBNull.Value != dr[index])
                {
                    Type dataType = dr.Table.Columns[index].DataType;
                    if (dataType == typeof(string))
                        stringBuilder.AppendFormat("\"{0}\"", (object)Convert.ToString(dr[index]).Trim());
                    else if (dataType == typeof(int) || dataType == typeof(int?) || dataType == typeof(int?) || dataType == typeof(double) || dataType == typeof(double?) || dataType == typeof(double?) || dataType == typeof(Decimal) || dataType == typeof(Decimal?) || dataType == typeof(Decimal?) || dataType == typeof(short) || dataType == typeof(short?) || dataType == typeof(short?) || dataType == typeof(byte) || dataType == typeof(byte?) || dataType == typeof(byte?) || dataType == typeof(long) || dataType == typeof(long?) || dataType == typeof(long?))
                        stringBuilder.AppendFormat("{0}", (object)Convert.ToString(dr[index]));
                    else if (dataType == typeof(DateTime) || dataType == typeof(DateTime?) || dataType == typeof(DateTime?))
                        stringBuilder.AppendFormat("\"{0}\"", (object)Convert.ToDateTime(dr[index]).ToString(timeFormat));
                    else if (dataType == typeof(bool) || dataType == typeof(bool?) || dataType == typeof(bool?) || dataType == typeof(bool) || dataType == typeof(bool?) || dataType == typeof(bool?))
                        stringBuilder.AppendFormat("{0}", (object)Convert.ToBoolean(dr[index]).ToString().ToLower());
                    else
                        stringBuilder.AppendFormat("\"{0}\"", (object)Convert.ToString(dr[index]));
                }
                else
                    stringBuilder.Append("null");
            }
            stringBuilder.Insert(0, "{");
            stringBuilder.Append("}");
            return stringBuilder.ToString();
        }

        public static List<T> ToList<T>(this DataSet ds) => DataSet2Object.DataToList<T>(ds);

        public static List<T> DataToList<T>(this DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
                return new List<T>();
            List<T> list = new List<T>();
            for (int index = 0; index < dt.Rows.Count; ++index)
            {
                T obj = DataSet2Object.DataToObj<T>(dt.Rows[index]);
                if ((object)obj != null)
                    list.Add(obj);
            }
            return list;
        }

        public static List<T> DataToList<T>(DataSet ds)
        {
            return ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0 ? new List<T>() : ds.Tables[0].DataToList<T>();
        }

        public static T DataToObj<T>(DataTable dt)
        {
            return dt == null || dt.Rows.Count == 0 ? default(T) : DataSet2Object.DataToObj<T>(dt.Rows[0]);
        }

        public static T DataToObj<T>(DataSet ds)
        {
            return ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0 ? default(T) : DataSet2Object.DataToObj<T>(ds.Tables[0].Rows[0]);
        }

        public static T ToModel<T>(this DataSet ds) => DataSet2Object.DataToObj<T>(ds);

        public static T ToModel<T>(this DataRow dr) => DataSet2Object.DataToObj<T>(dr);

        public static T ToModel<T>(this DataTable dt)
        {
            return dt == null || dt.Rows.Count == 0 ? default(T) : DataSet2Object.DataToObj<T>(dt.Rows[0]);
        }

        public static T DataToObj<T>(DataRow dr)
        {
            if (dr == null)
                return default(T);
            Type type1 = typeof(T);
            if (!(type1 == typeof(string)) && !(type1 == typeof(int)) && !(type1 == typeof(int?)) && !(type1 == typeof(int?)) && !(type1 == typeof(double)) && !(type1 == typeof(double?)) && !(type1 == typeof(double?)) && !(type1 == typeof(DateTime)) && !(type1 == typeof(DateTime?)) && !(type1 == typeof(DateTime?)) && !(type1 == typeof(Decimal)) && !(type1 == typeof(Decimal?)) && !(type1 == typeof(Decimal?)) && !(type1 == typeof(short)) && !(type1 == typeof(short?)) && !(type1 == typeof(short?)) && !(type1 == typeof(byte)) && !(type1 == typeof(byte?)) && !(type1 == typeof(byte?)) && !(type1 == typeof(long)) && !(type1 == typeof(long?)) && !(type1 == typeof(long?)))
            {
                T instance = (T)Assembly.Load(type1.Assembly.FullName).CreateInstance(type1.FullName);
                Type type2 = instance.GetType();
                for (int index = 0; index < dr.Table.Columns.Count; ++index)
                {
                    PropertyInfo property = type2.GetProperty(dr.Table.Columns[index].ColumnName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
                    if (!((PropertyInfo)null == property) && DBNull.Value != dr[index])
                    {
                        if (property.PropertyType.IsEnum)
                            property.SetValue((object)instance, Enum.ToObject(property.PropertyType, dr[index]), (object[])null);
                        else if (property.PropertyType == typeof(string))
                            property.SetValue((object)instance, (object)dr[index].ToString(), (object[])null);
                        else if (property.PropertyType == typeof(int) || property.PropertyType == typeof(int?) || property.PropertyType == typeof(int?))
                            property.SetValue((object)instance, (object)Convert.ToInt32(dr[index]), (object[])null);
                        else if (property.PropertyType == typeof(double) || property.PropertyType == typeof(double?) || property.PropertyType == typeof(double?))
                            property.SetValue((object)instance, (object)Convert.ToDouble(Convert.ToDecimal(dr[index])), (object[])null);
                        else if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?) || property.PropertyType == typeof(DateTime?))
                            property.SetValue((object)instance, (object)Convert.ToDateTime(dr[index]), (object[])null);
                        else if (property.PropertyType == typeof(Decimal) || property.PropertyType == typeof(Decimal?) || property.PropertyType == typeof(Decimal?))
                            property.SetValue((object)instance, (object)Convert.ToDecimal(dr[index]), (object[])null);
                        else if (property.PropertyType == typeof(short) || property.PropertyType == typeof(short?) || property.PropertyType == typeof(short?))
                            property.SetValue((object)instance, (object)Convert.ToInt16(dr[index]), (object[])null);
                        else if (property.PropertyType == typeof(byte) || property.PropertyType == typeof(byte?) || property.PropertyType == typeof(byte?))
                            property.SetValue((object)instance, (object)Convert.ToByte(dr[index]), (object[])null);
                        else if (property.PropertyType == typeof(long) || property.PropertyType == typeof(long?) || property.PropertyType == typeof(long?))
                            property.SetValue((object)instance, (object)Convert.ToInt64(dr[index]), (object[])null);
                        else if (property.PropertyType == typeof(bool) || property.PropertyType == typeof(bool?) || property.PropertyType == typeof(bool?))
                            property.SetValue((object)instance, (object)Convert.ToBoolean(dr[index]), (object[])null);
                        else if (property.PropertyType == typeof(bool) || property.PropertyType == typeof(bool?) || property.PropertyType == typeof(bool?))
                            property.SetValue((object)instance, (object)Convert.ToBoolean(dr[index]), (object[])null);
                        else
                            property.SetValue((object)instance, dr[index], (object[])null);
                    }
                }
                return instance;
            }
            return DBNull.Value != dr[0] && string.Empty != dr[0].ToString() ? (T)dr[0] : default(T);
        }

        public static Dictionary<TKey, TValue> DataToDictionary<TKey, TValue>(
          DataSet ds,
          string columnName)
        {
            return ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0 ? new Dictionary<TKey, TValue>() : DataSet2Object.DataToDictionary<TKey, TValue>(ds.Tables[0], columnName);
        }

        public static Dictionary<TKey, TValue> DataToDictionary<TKey, TValue>(
          DataTable dt,
          string columnName)
        {
            if (dt == null || dt.Rows.Count == 0)
                return new Dictionary<TKey, TValue>();
            Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
            for (int index = 0; index < dt.Rows.Count; ++index)
            {
                TValue obj = DataSet2Object.DataToObj<TValue>(dt.Rows[index]);
                if ((object)obj != null)
                {
                    TKey key = (TKey)dt.Rows[index][columnName];
                    try
                    {
                        dictionary.Add(key, obj);
                    }
                    catch
                    {
                        dictionary[key] = obj;
                    }
                }
            }
            return dictionary;
        }

        public static DataTable ToDataTable<T>(IList<T> list)
        {
            DataTable dataTable = DataSet2Object.CreateDataTable<T>();
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            if (list == null || list.Count == 0)
                return dataTable;
            foreach (T component in (IEnumerable<T>)list)
            {
                DataRow row = dataTable.NewRow();
                foreach (PropertyDescriptor propertyDescriptor in properties)
                    row[propertyDescriptor.Name] = propertyDescriptor.GetValue((object)component) == null ? (object)DBNull.Value : propertyDescriptor.GetValue((object)component);
                dataTable.Rows.Add(row);
            }
            return dataTable;
        }

        public static DataTable CreateDataTable<T>()
        {
            Type componentType = typeof(T);
            DataTable dataTable = new DataTable(componentType.Name);
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(componentType))
            {
                Type type = property.PropertyType;
                if (type.IsNullableType())
                    type = type.GetUnNullableType();
                dataTable.Columns.Add(property.Name, type);
            }
            return dataTable;
        }

        public static List<T> DataGridViewToList<T>(DataGridView dgv) where T : new()
        {
            List<T> list = new List<T>();

            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow) continue; // 跳过新行

                T obj = new T();
                foreach (var prop in typeof(T).GetProperties())
                {
                    if (dgv.Columns.Contains(prop.Name))
                    {
                        var cellValue = row.Cells[prop.Name].Value;
                        if (cellValue != null)
                        {
                            prop.SetValue(obj, Convert.ChangeType(cellValue, prop.PropertyType));
                        }
                    }
                }
                list.Add(obj);
            }

            return list;
        }
    }
}
