using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Formatting = Newtonsoft.Json.Formatting;

namespace UW.Common
{
    public static class ObjectExtensions
    {
        public static string ToJsonString(this object value, string timeFormat = "yyyy-MM-dd HH:mm:ss", bool clearEnter = true, bool nullToEmpty = true)
        {
            if (string.IsNullOrEmpty(timeFormat))
                return JsonConvert.SerializeObject(value);
            Formatting formatting = Formatting.None;
            if (!clearEnter)
                formatting = Formatting.Indented;
            if (nullToEmpty)
            {
                JsonSerializerSettings settings = new JsonSerializerSettings()
                {
                    ContractResolver = (IContractResolver)new ObjectExtensions.NullToEmptyStringResolver()
                };
                settings.Converters.Add((JsonConverter)ObjectExtensions.GetTimeConverter(timeFormat));
                return JsonConvert.SerializeObject(value, formatting, settings);
            }
            return JsonConvert.SerializeObject(value, formatting, (JsonConverter)ObjectExtensions.GetTimeConverter(timeFormat));
        }

        public static string FormatJsonString(this string str)
        {
            JsonSerializer jsonSerializer = new JsonSerializer();
            JsonTextReader reader = new JsonTextReader((TextReader)new StringReader(str));
            object obj = jsonSerializer.Deserialize((JsonReader)reader);
            if (obj == null)
                return str;
            StringWriter stringWriter = new StringWriter();
            JsonTextWriter jsonTextWriter1 = new JsonTextWriter((TextWriter)stringWriter);
            jsonTextWriter1.Formatting = Formatting.Indented;
            jsonTextWriter1.Indentation = 1;
            jsonTextWriter1.IndentChar = '\t';
            JsonTextWriter jsonTextWriter2 = jsonTextWriter1;
            jsonSerializer.Serialize((JsonWriter)jsonTextWriter2, obj);
            return stringWriter.ToString();
        }

        private static IsoDateTimeConverter GetTimeConverter(string timeFormat)
        {
            IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
            string str = string.Empty;
            foreach (char ch in timeFormat)
                str = "YMDHSFymdhsf ".IndexOf(ch) >= 0 ? str + ch.ToString() : str + string.Format("'{0}'", (object)ch);
            timeConverter.DateTimeFormat = str;
            return timeConverter;
        }

        public static T FromJsonString<T>(this string value, string timeFormat = "yyyy-MM-dd HH:mm:ss")
        {
            try
            {
                if (string.IsNullOrEmpty(timeFormat))
                    return JsonConvert.DeserializeObject<T>(value);
                return JsonConvert.DeserializeObject<T>(value, (JsonConverter)ObjectExtensions.GetTimeConverter(timeFormat));
            }
            catch (Exception ex)
            {
            }
            return default(T);
        }

        public static object FromJsonString(this string value, Type type, string timeFormat = "yyyy-MM-dd HH:mm:ss")
        {
            try
            {
                if (string.IsNullOrEmpty(timeFormat))
                    return JsonConvert.DeserializeObject(value, type);
                return JsonConvert.DeserializeObject(value, type, (JsonConverter)ObjectExtensions.GetTimeConverter(timeFormat));
            }
            catch (Exception ex)
            {
            }
            return (object)null;
        }

        public static string JsonStringParse(this string value, string objectKey)
        {
            if (string.IsNullOrEmpty(objectKey))
                return string.Empty;
            try
            {
                JObject jobject = JObject.Parse(value);
                if (jobject != null)
                    return jobject[objectKey].ToString();
            }
            catch (Exception ex)
            {
            }
            return string.Empty;
        }

        public static object CastTo(this object value, Type conversionType)
        {
            if (value == null)
                return (object)null;
            if (conversionType.IsNullableType())
                conversionType = conversionType.GetUnNullableType();
            return conversionType.IsEnum ? Enum.Parse(conversionType, value.ToString()) : Convert.ChangeType(value, conversionType);
        }

        public static T CastTo<T>(this object value) => (T)value.CastTo(typeof(T));

        public static T CastTo<T>(this object value, T defaultValue)
        {
            T obj;
            try
            {
                obj = value.CastTo<T>();
            }
            catch (Exception ex)
            {
                obj = defaultValue;
            }
            return obj;
        }

        public static string ToBase64String(this byte[] value) => Convert.ToBase64String(value);

        public static byte[] FromBase64String4Byte(this string value)
        {
            return Convert.FromBase64String(value);
        }

        public static bool IsBetween<T>(
          this IComparable<T> value,
          T start,
          T end,
          bool leftEqual = false,
          bool rightEqual = false)
          where T : IComparable
        {
            if ((leftEqual ? (value.CompareTo(start) >= 0 ? 1 : 0) : (value.CompareTo(start) > 0 ? 1 : 0)) == 0)
                return false;
            return !rightEqual ? value.CompareTo(end) < 0 : value.CompareTo(end) <= 0;
        }

        public static void SetPropertyValue<T>(this T obj, string name, object value)
        {
            foreach (PropertyInfo property in typeof(T).GetProperties())
            {
                if (property.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    property.SetValue((object)obj, value, (object[])null);
                    break;
                }
            }
        }

        public static object GetPropertyValue<T>(this T obj, string name)
        {
            foreach (PropertyInfo property in typeof(T).GetProperties())
            {
                if (property.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    return property.GetValue((object)obj, (object[])null);
            }
            return (object)null;
        }

        public static T Clone<T>(this T obj) where T : new()
        {
            if ((object)obj == null)
                return default(T);
            Type type = typeof(T);
            T obj1 = new T();
            foreach (PropertyInfo property in type.GetProperties())
            {
                if (!property.IsSpecialName && property.CanWrite)
                {
                    object obj2 = property.GetValue((object)obj, (object[])null);
                    if (obj2 != null)
                        property.SetValue((object)obj1, obj2, (object[])null);
                }
            }
            return obj1;
        }

        public static bool Equals<TempClass>(
          this TempClass obj1,
          TempClass obj2,
          BindingFlags bindingFlags = BindingFlags.Default)
        {
            Type type1 = obj1.GetType();
            Type type2 = obj2.GetType();
            PropertyInfo[] properties1 = type1.GetProperties(bindingFlags);
            PropertyInfo[] properties2 = type2.GetProperties(bindingFlags);
            bool flag = true;
            for (int index = 0; index < properties1.Length; ++index)
            {
                string name = properties1[index].DeclaringType.Name;
                if (properties1[index].GetValue((object)obj1, (object[])null).ToString() != properties2[index].GetValue((object)obj2, (object[])null).ToString())
                {
                    flag = false;
                    break;
                }
            }
            return flag;
        }

        public static bool IsEqual(Type dataType, object oldObj, object newObj)
        {
            if (oldObj == null && newObj == null)
                return true;
            if (dataType == typeof(int))
                return (int)oldObj == (int)newObj;
            if (dataType == typeof(Decimal))
                return (Decimal)oldObj == (Decimal)newObj;
            if (dataType == typeof(double))
                return (double)oldObj == (double)newObj;
            if (dataType == typeof(Guid))
                return (Guid)oldObj == (Guid)newObj;
            return dataType == typeof(DateTime) ? (DateTime)oldObj == (DateTime)newObj : oldObj.Equals(newObj);
        }

        public static string ObjectToJSON(this object obj)
        {
            JavaScriptSerializer scriptSerializer = new JavaScriptSerializer();
            try
            {
                return scriptSerializer.Serialize(obj);
            }
            catch (Exception ex)
            {
                throw new Exception("Newtonsoft.Json.JsonConvert.SerializeObject(): " + ex.Message);
            }
        }

        public static T JSONToObject<T>(this string jsonText)
        {
            JavaScriptSerializer scriptSerializer = new JavaScriptSerializer();
            try
            {
                return scriptSerializer.Deserialize<T>(jsonText);
            }
            catch (Exception ex)
            {
                throw new Exception("Newtonsoft.Json.JsonConvert.DeserializeObject(): " + ex.Message);
            }
        }

        private class NullToEmptyStringResolver : DefaultContractResolver
        {
            protected override IList<JsonProperty> CreateProperties(
              Type type,
              MemberSerialization memberSerialization)
            {
                return (IList<JsonProperty>)((IEnumerable<PropertyInfo>)type.GetProperties()).Select<PropertyInfo, JsonProperty>((Func<PropertyInfo, JsonProperty>)(p => new JsonProperty()
                {
                    ValueProvider = (IValueProvider)new ObjectExtensions.NullToEmptyStringValueProvider(p)
                })).ToList<JsonProperty>();
            }
        }

        private class NullToEmptyStringValueProvider : IValueProvider
        {
            private PropertyInfo _MemberInfo;

            public NullToEmptyStringValueProvider(PropertyInfo memberInfo)
            {
                this._MemberInfo = memberInfo;
            }

            public object GetValue(object target)
            {
                object obj = this._MemberInfo.GetValue(target, (object[])null);
                if (this._MemberInfo.PropertyType == typeof(string) && obj == null)
                    obj = (object)"";
                return obj;
            }

            public void SetValue(object target, object value)
            {
                this._MemberInfo.SetValue(target, value, (object[])null);
            }
        }

        public static T FromObject<T>(this string jsonStr)
        {
            JavaScriptSerializer scriptSerializer = new JavaScriptSerializer();
            try
            {
                return scriptSerializer.Deserialize<T>(jsonStr);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
