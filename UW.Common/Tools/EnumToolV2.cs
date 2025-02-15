using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UW.Common
{
    public static class EnumToolV2
    {
        public static string GetDescription(this Enum enumName)
        {
            string empty = string.Empty;
            DescriptionAttribute[] descriptAttr = enumName.GetType().GetField(enumName.ToString()).GetDescriptAttr();
            return descriptAttr == null || descriptAttr.Length == 0 ? enumName.ToString() : descriptAttr[0].Description;
        }

        public static DescriptionAttribute[] GetDescriptAttr(this FieldInfo fieldInfo)
        {
            return fieldInfo != (FieldInfo)null ? (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false) : (DescriptionAttribute[])null;
        }

        public static T GetEnumName<T>(string description)
        {
            foreach (FieldInfo field in typeof(T).GetFields())
            {
                DescriptionAttribute[] descriptAttr = field.GetDescriptAttr();
                if (descriptAttr != null && descriptAttr.Length != 0)
                {
                    if (descriptAttr[0].Description == description)
                        return (T)field.GetValue((object)null);
                }
                else if (field.Name == description)
                    return (T)field.GetValue((object)null);
            }
            throw new ArgumentException(string.Format("{0} 未能找到对应的枚举.", (object)description), "Description");
        }

        public static ArrayList ToArrayList(this Enum en)
        {
            ArrayList arrayList = new ArrayList();
            foreach (Enum @enum in Enum.GetValues(en.GetType()))
                arrayList.Add((object)new KeyValuePair<Enum, string>(@enum, @enum.GetDescription()));
            return arrayList;
        }

        public static List<string> ToStringList<T>()
        {
            List<string> stringList = new List<string>();
            foreach (object obj in Enum.GetValues(typeof(T)))
                stringList.Add(obj.ToString());
            return stringList;
        }
    }
}
