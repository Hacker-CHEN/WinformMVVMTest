using System;
using System.Collections;
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
    public static class TypeExtensions
    {
        public static bool IsNullableType(this System.Type type)
        {
            return type != (System.Type)null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static System.Type GetNonNummableType(this System.Type type)
        {
            return type.IsNullableType() ? type.GetGenericArguments()[0] : type;
        }

        public static System.Type GetUnNullableType(this System.Type type)
        {
            return type.IsNullableType() ? new NullableConverter(type).UnderlyingType : type;
        }

        public static string ToDescription(this MemberInfo member, bool inherit = false)
        {
            DescriptionAttribute attribute = member.GetAttribute<DescriptionAttribute>(inherit);
            return attribute != null ? attribute.Description : member.Name;
        }

        public static bool ExistsAttribute<T>(this MemberInfo memberInfo, bool inherit = false) where T : Attribute
        {
            return ((IEnumerable<object>)memberInfo.GetCustomAttributes(typeof(T), inherit)).Any<object>((System.Func<object, bool>)(m => m is T));
        }

        public static T GetAttribute<T>(this MemberInfo memberInfo, bool inherit = false) where T : Attribute
        {
            return ((IEnumerable<object>)memberInfo.GetCustomAttributes(typeof(T), inherit)).FirstOrDefault<object>() as T;
        }

        public static T[] GetAttributes<T>(this MemberInfo memberInfo, bool inherit = false) where T : Attribute
        {
            return memberInfo.GetCustomAttributes(typeof(T), inherit).Cast<T>().ToArray<T>();
        }

        public static bool IsEnumerable(this System.Type type)
        {
            return !(type == typeof(string)) && typeof(IEnumerable).IsAssignableFrom(type);
        }

        public static void CopyTo<T>(this T source, T target, bool copyFixed = false)
        {
            foreach (PropertyInfo property1 in target.GetType().GetProperties())
            {
                PropertyInfo property2 = source.GetType().GetProperty(property1.Name);
                if ((PropertyInfo)null != property2 && property2.CanWrite)
                {
                    object obj = property2.GetValue((object)source, (object[])null);
                    string name = property2.PropertyType.Name;
                    if (name.Equals(typeof(byte[]).Name, StringComparison.OrdinalIgnoreCase) || (name.Equals(typeof(string).Name, StringComparison.OrdinalIgnoreCase) || name.Equals(typeof(int?).Name, StringComparison.OrdinalIgnoreCase) || name.Equals(typeof(int).Name, StringComparison.OrdinalIgnoreCase) || name.Equals(typeof(bool).Name, StringComparison.OrdinalIgnoreCase) || name.Equals(typeof(bool?).Name, StringComparison.OrdinalIgnoreCase) || name.Equals(typeof(DateTime).Name, StringComparison.OrdinalIgnoreCase) || name.Equals(typeof(DateTime?).Name, StringComparison.OrdinalIgnoreCase) || name.Equals(typeof(float).Name, StringComparison.OrdinalIgnoreCase) || name.Equals(typeof(float?).Name, StringComparison.OrdinalIgnoreCase) || name.Equals(typeof(double).Name, StringComparison.OrdinalIgnoreCase) || name.Equals(typeof(double?).Name, StringComparison.OrdinalIgnoreCase) || name.Equals(typeof(double).Name, StringComparison.OrdinalIgnoreCase) || name.Equals(typeof(double?).Name, StringComparison.OrdinalIgnoreCase) || name.Equals(typeof(Decimal).Name, StringComparison.OrdinalIgnoreCase) || name.Equals(typeof(Decimal?).Name, StringComparison.OrdinalIgnoreCase)) && (copyFixed || (!property2.Name.Equals("CREATEBY", StringComparison.OrdinalIgnoreCase) || property2.Name.Equals("CreateBy") && obj != null) && (!property2.Name.Equals("CREATEON", StringComparison.OrdinalIgnoreCase) || property2.Name.Equals("CreateOn") && obj != null) && (!property2.Name.Equals("MODIFYBY", StringComparison.OrdinalIgnoreCase) || property2.Name.Equals("CreateOn") && obj != null) && (!property2.Name.Equals("MODIFYON", StringComparison.OrdinalIgnoreCase) || property2.Name.Equals("CreateOn") && obj != null)))
                        target.GetType().InvokeMember(property1.Name, BindingFlags.SetProperty, (Binder)null, (object)target, new object[1]
                        {
              obj
                        });
                }
            }
            foreach (FieldInfo field1 in target.GetType().GetFields())
            {
                FieldInfo field2 = source.GetType().GetField(field1.Name);
                if ((FieldInfo)null != field2)
                {
                    object obj = field2.GetValue((object)source);
                    if (obj != null)
                        target.GetType().InvokeMember(field1.Name, BindingFlags.SetField, (Binder)null, (object)target, new object[1]
                        {
              obj
                        });
                }
            }
        }

        public static DataTable CreateDataTable<T>(this T source)
        {
            return DataSet2Object.CreateDataTable<T>();
        }

        public static DataTable ToDataTable<T>(this IList<T> list)
        {
            return DataSet2Object.ToDataTable<T>(list);
        }

        public static DataTable TablePage(this DataTable dt, int from, int pagesize)
        {
            DataTable dataTable = dt.Clone();
            foreach (DataRow dataRow in dt.AsEnumerable().Skip<DataRow>(from).Take<DataRow>(pagesize))
                dataTable.Rows.Add(dataRow.ItemArray);
            return dataTable;
        }

        public static DataTable ToDataTable(this DataGridView dataGridView)
        {
            DataTable dataTable = new DataTable();
            for (int index = 0; index < dataGridView.Columns.Count; ++index)
            {
                DataColumn column = new DataColumn(dataGridView.Columns[index].HeaderText.ToString());
                dataTable.Columns.Add(column);
            }
            for (int index1 = 0; index1 < dataGridView.Rows.Count; ++index1)
            {
                DataRow row = dataTable.NewRow();
                for (int index2 = 0; index2 < dataGridView.Columns.Count; ++index2)
                    row[index2] = (object)Convert.ToString(dataGridView.Rows[index1].Cells[index2].Value);
                dataTable.Rows.Add(row);
            }
            return dataTable;
        }
    }
}
