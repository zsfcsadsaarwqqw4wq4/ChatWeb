using cn.jpush.api;
using cn.jpush.api.common;
using cn.jpush.api.push.mode;
using cn.jpush.api.push.notification;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using static Model.ExModel;

namespace ChatWeb.App_Start
{
    public static class JPush
    {
        /// <summary>
        /// 应用标识，极光推送名
        /// </summary>
        private static readonly string Appkey = ConfigurationManager.AppSettings["AppKey"];
        /// <summary>
        /// 极光推送的密码
        /// </summary>
        private static readonly string Masert = ConfigurationManager.AppSettings["MasterSecret"];
        /// <summary>
        /// 推送配置
        /// </summary>
        /// <param name="registerid">推送列表(设备别号)</param>
        /// <param name="pushTitle">标题</param>
        /// <param name="pushlist">推送附加参数</param>
        /// <returns></returns>
        private static PushPayload PushObject_RegisterID(HashSet<string> registerid,string pushTitle, List<Push> pushList)
        {
            PushPayload pushPayload = new PushPayload();
            //设置推送平台
            pushPayload.platform = Platform.android_ios();//设置为安卓
            //设置受众
            pushPayload.audience = Audience.s_registrationId(registerid);//设置推送目标
            ////通知获取
            //pushPayload.notification = Notification.android("55555", pushTitle);
            ////通知获取
            //pushPayload.notification = Notification.ios(pushTitle);
            var notification = new Notification().setAlert(pushTitle);//推送消息内容
            notification.AndroidNotification = new AndroidNotification();
            notification.IosNotification = new IosNotification();
            //if (pushList.Count>0)
            //{
            //    foreach (var model in pushList)
            //    {                    
            //        notification.AndroidNotification.AddExtra(model.Key, model.Value);//Android推送消息
            //        notification.IosNotification.AddExtra(model.Key, model.Value);//IOS推送消息
            //        notification.IosNotification.setSound("happy");
            //        pushPayload.notification = notification.Check();
            //    }
            //}
            //else
            //{
            //    pushPayload.notification = Notification.android("这是标题", pushTitle);
            //}
            return pushPayload;
        }
        public static PushPayload PushObject_Android_Tag_AlertWithTitle(string alert, string title, string shebei)
        {
            PushPayload pushPayload = new PushPayload();
            pushPayload.platform = Platform.android();
            pushPayload.audience = Audience.s_tag(shebei);
            pushPayload.notification = Notification.android(alert, title);
            return pushPayload;
        }
        /// <summary>
        /// 推送
        /// </summary>
        /// <param name="registerid">推送列表(设备别号)</param>
        /// <param name="pushTitle">标题</param>
        /// <param name="pushlist">推送附加参数</param>
        /// <returns></returns>
        public static bool  JPushByRegiserID(HashSet<string> registerid, string pushTitle, List<Push> pushList)
        {
            //if (!isAllew)
            //{
            //    return false;
            //}
            //sendNotificationByAlias
            JPushClient client=new JPushClient(Appkey, Masert);//根据app信息生成推送client
            PushPayload payload = PushObject_RegisterID(registerid, pushTitle, pushList);////设置推送的具体参数
            try
            {
                var result = client.SendPush(payload);//推送
                if (int.Parse(result.sendno.ToString())==0)
                {
                    return true;
                }
                return false;
            }
            catch(APIRequestException ex)
            {
                return false;
            }
        }
    }
}