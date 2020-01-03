using Newtonsoft.Json;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace ChatWeb.App_Start
{
    public class Redis
    {
        /// <summary>
        /// 获取Redis连接ip地址
        /// </summary>
        private static string RedisPath = ConfigurationManager.AppSettings["RedisPath"];
        /// <summary>
        /// 获取需要连接的端口
        /// </summary>
        private static int RedisPort = Convert.ToInt32(ConfigurationManager.AppSettings["RedisPort"]);
        /// <summary>
        /// 获取某个key的过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long endtime(string key)
        {
            using (RedisClient redisclient = new RedisClient(RedisPath, RedisPort, "123456"))
            {
                long entime=redisclient.PTtl(key);
                return entime;
            }
        }
        /// <summary>
        /// 保存一个对象，该对象会被序列化并设置过期时间,Set方法会自动将对象序列化成一个string类型的json字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool StringSet<T>(string key,T value, TimeSpan time)
        {
            //创建Redis连接对象
            using (RedisClient redisclient = new RedisClient(RedisPath, RedisPort, "123456"))
            {
                //存放string类型数据到内存中
                bool res = redisclient.Set(key, value, time);
                return res;
            }
        }
        /// <summary>
        /// 保存一个对象，该对象会被序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool StringSet<T>(string key, T value)
        {
            //创建Redis连接对象
            using (RedisClient redisclient = new RedisClient(RedisPath, RedisPort, "123456"))
            {
                //var jsonSetting=new JsonSerializerSettings(){ NullValueHandling = NullValueHandling.Ignore };
                //string data=JsonConvert.SerializeObject(value,Formatting.None, jsonSetting);
                //data=data.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "");
                //存放string类型数据到内存中
                bool res = redisclient.Set(key, value);
                return res;
            }
        }
        /// <summary>
        /// 获取一个object对象，该对象是通过反序列化得到
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public IDictionary<string,object> StringGet(string key)
        {
            using (RedisClient redisclient = new RedisClient(RedisPath, RedisPort, "123456"))
            {
                var data=redisclient.GetValue(key);
                if(data!=null)
                {                   
                    return JsonConvert.DeserializeObject<IDictionary<string,object>>(data);
                }
                return null;
            }
        }
        /// <summary>
        /// 获取key，返回byte格式
        /// </summary>
        /// <returns></returns>
        public byte[] GetValueByte(string key)
        {
            using (RedisClient redisclient = new RedisClient(RedisPath, RedisPort, "123456"))
            {
                byte[] value = redisclient.Get(key);
                return value;
            }
        }
        /// <summary>
        /// 存储string类型数据有过期时间
        /// </summary>
        public bool SetString(string key,string value,TimeSpan time)
        {
            //创建Redis连接对象
            using (RedisClient redisclient = new RedisClient(RedisPath, RedisPort, "123456"))
            {               
                //存放string类型数据到内存中
                bool res = redisclient.Set<string>(key, value, time);
                return res;
            }
        }
        /// <summary>
        /// 存储string类型数据无过期时间
        /// </summary>
        public bool SetString(string key, string value)
        {
            //创建Redis连接对象
            using (RedisClient redisclient = new RedisClient(RedisPath, RedisPort, "123456"))
            {
                //存放string类型数据到内存中
                bool res = redisclient.Set<string>(key, value);
                return res;
            }
        }
        /// <summary>
        /// 存储int类型数据无过期时间
        /// </summary>
        public bool SetInt(string key, int value)
        {
            //创建Redis连接对象
            using (RedisClient redisclient = new RedisClient(RedisPath, RedisPort, "123456"))
            {
                //存放string类型数据到内存中
                bool res = redisclient.Set<int>(key, value);
                return res;
            }
        }
        /// <summary>
        /// 根据key得到value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetString(string key)
        {
            //创建Redis连接对象
            using (RedisClient redisclient = new RedisClient(RedisPath, RedisPort, "123456"))
            {

                //存放string类型数据到内存中
                string res = redisclient.Get<string>(key);
                return res;
            }
        }
        /// <summary>
        /// 根据key删除string类型的值
        /// </summary>
        /// <param name="key"></param>
        public bool DeleteString(string key)
        {
            using (RedisClient redisclient = new RedisClient(RedisPath, RedisPort, "123456"))
            {
                bool res = redisclient.Remove(key);
                return res;
            }
        }
        /// <summary>
        /// 存储List类型数据指定key值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetList(string key, List<string> value)
        {
            using (RedisClient redisclient = new RedisClient(RedisPath, RedisPort, "123456"))
            {
                //value.ForEach(x => redisclient.AddItemToList(key, x));
                redisclient.AddRangeToList(key, value);
            }
        }
    }
}