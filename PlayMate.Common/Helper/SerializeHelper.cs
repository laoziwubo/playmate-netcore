using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace PlayMate.Common.Helper
{
    public class SerializeHelper
    {
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static byte[] Serialize(object item)
        {
            var jsonString = JsonConvert.SerializeObject(item);

            return Encoding.UTF8.GetBytes(jsonString);
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T Deserialize<T>(byte[] value)
        {
            if (value == null)
            {
                return default(T);
            }
            var jsonString = Encoding.UTF8.GetString(value);
            return JsonConvert.DeserializeObject<T>(jsonString);
        }
    }
}
