using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.ProtocolBuffers;
using com.gexin.rp.sdk.dto;
using com.igetui.api.openservice;
using com.igetui.api.openservice.igetui;
using com.igetui.api.openservice.igetui.template;
using com.igetui.api.openservice.payload;
using System.Net;
using System.Web;

namespace ChatWeb
{
    /// <summary>
    /// Push 的摘要说明
    /// </summary>
    public class Push : IHttpHandler
    {
        //http的域名
        private static String HOST = "http://sdk.open.api.igexin.com/apiex.htm";
        //定义常量, appId、appKey、masterSecret 采用本文档 "第二步 获取访问凭证 "中获得的应用配置
        private static String APPID = "L9oAbDqZcr8DazmwiGlOt4";
        private static String APPKEY = "wKppBiwovrAOERqpNIJ4n6";
        private static String MASTERSECRET = "ya67y9uHXV6erZ08rhmn92";
        private static String CLIENTID = "f5cedb30883d33aa524a89400f90f298";
        private static String token = "6926fa659909111950b08b6f53fb2f7d2e4ba1a27ab70e0f4618a3d0d80577e3";
        //private static String CLIENTID1 = "7018ef85b40466ffcefaf41ba17c7e21";
        //private static String CLIENTID2 = "f5cedb30883d33aa524a89400f90f298";
        //private static String GroupName = "app推送";
        //private static String Badge = "50";
        //private static String TASKID = "OSA-0903_bWHwhpFPEC7i5nZwHmc6d";
        //private static String ALIAS = "请输入别名";
        //private static string PN = "13550347892";
        public void ProcessRequest(HttpContext context)
        {
            //toList接口每个用户状态返回是否开启，可选
            Console.OutputEncoding = Encoding.GetEncoding(936);
            Environment.SetEnvironmentVariable("needDetails", "true");
            //下为消息推送的四种方式，单独使用时，请注释掉另外三种方法
            //APNsPushToSingle("这是标题", "这是内容", token);
            //对单个用户的推送
            //PushMessageToSingle();

            ////对指定列表用户推送
            //PushMessageToList();

            ////对指定应用群推送
            //pushMessageToApp();

            ////批量单推
            //singleBatchDemo();
        }
        public static void PushMessageToSingle(string content, string data,string cid)
        {

            IGtPush push = new IGtPush(HOST, APPKEY, MASTERSECRET);
            //消息模版：TransmissionTemplate:透传模板
            var template = NotificationTemplateDemo(content, data);
            // 单推消息模型

            SingleMessage message = new SingleMessage();
            message.IsOffline = false;                         // 用户当前不在线时，是否离线存储,可选
            message.OfflineExpireTime = 1000 * 3600 * 12;            // 离线有效时间，单位为毫秒，可选
            message.Data = template;
            //判断是否客户端是否wifi环境下推送，2为4G/3G/2G，1为在WIFI环境下，0为不限制环境
            message.PushNetWorkType = 0;
            com.igetui.api.openservice.igetui.Target target = new com.igetui.api.openservice.igetui.Target();
            target.appId = APPID;
            target.clientId = cid;
            //target.alias = ALIAS;
            try
            {
                push.pushMessageToSingle(message, target);
            }
            catch (RequestException e)
            {
                String requestId = e.RequestId;
                //发送失败后的重发
                push.pushMessageToSingle(message, target, requestId);
            }
        }

        //通知透传模板动作内容
        public static NotificationTemplate NotificationTemplateDemo(string content,string data)
        {
            NotificationTemplate template = new NotificationTemplate();
            template.AppId = APPID;
            template.AppKey = APPKEY;
            //通知栏标题
            template.Title = "";
            //通知栏内容     
            template.Text = content;
            //通知栏显示本地图片
            template.Logo = "";
            //通知栏显示网络图标
            template.LogoURL = "";
            //应用启动类型，1：强制应用启动  2：等待应用启动
            template.TransmissionType = 1;
            //透传内容  
            template.TransmissionContent = data;
            //接收到消息是否响铃，true：响铃 false：不响铃   
            template.IsRing = true;
            //接收到消息是否震动，true：震动 false：不震动   
            template.IsVibrate = true;
            //接收到消息是否可清除，true：可清除 false：不可清除    
            template.IsClearable = true;
            //设置通知定时展示时间，结束时间与开始时间相差需大于6分钟，消息推送后，客户端将在指定时间差内展示消息（误差6分钟）
            //string firsttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //double time = 10;
            //string lasttime = DateTime.Now.AddMinutes(time).ToString("yyyy-MM-dd HH:mm:ss");            
            //String begin = firsttime;
            //String end = lasttime;
            //template.setDuration(begin, end);

            return template;
        }
        /// <summary>
        /// ios端个推透传内容
        /// </summary>
        /// <param name="content"></param>
        /// <param name="data"></param>
        /// <param name="cid"></param>
        public static void IosPushMessageToSingle(string data, string cid)
        {
            IGtPush push = new IGtPush(HOST, APPKEY, MASTERSECRET);
            //消息模版：TransmissionTemplate:透传模板
            var template = TransmissionTemplateDemo(data);
            // 单推消息模型
            SingleMessage message = new SingleMessage();
            message.IsOffline = false;                         // 用户当前不在线时，是否离线存储,可选
            message.OfflineExpireTime = 1000 * 3600 * 12;            // 离线有效时间，单位为毫秒，可选
            message.Data = template;
            //判断是否客户端是否wifi环境下推送，2为4G/3G/2G，1为在WIFI环境下，0为不限制环境
            message.PushNetWorkType = 0;
            com.igetui.api.openservice.igetui.Target target = new com.igetui.api.openservice.igetui.Target();
            target.appId = APPID;
            target.clientId = cid;
            //target.alias = ALIAS;
            try
            {
                push.pushMessageToSingle(message, target);
            }
            catch (RequestException e)
            {
                String requestId = e.RequestId;
                //发送失败后的重发
                push.pushMessageToSingle(message, target, requestId);
            }
        }
        //透传模板动作内容
        public static TransmissionTemplate TransmissionTemplateDemo(string data)
        {
            TransmissionTemplate template = new TransmissionTemplate();
            template.AppId = APPID;
            template.AppKey = APPKEY;
            //应用启动类型，1：强制应用启动 2：等待应用启动
            template.TransmissionType = 1;
            //透传内容  
            template.TransmissionContent = data;
            return template;
        }

        /// <summary>
        /// ios个推离线通知
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="token"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string APNsPushToSingle(string title, string content, string token,object data)
        {
            APNTemplate template = new APNTemplate();
            APNPayload apnpayload = new APNPayload();
            DictionaryAlertMsg alertMsg = new DictionaryAlertMsg();
            alertMsg.Body = content;         //通知文本消息字符串
            alertMsg.ActionLocKey = "";
            alertMsg.LocKey = "";
            alertMsg.addLocArg("");
            alertMsg.LaunchImage = "";//指定启动界面图片名
            //IOS8.2支持字段
            alertMsg.Title = title;     //通知标题
            alertMsg.TitleLocKey = "";
            alertMsg.addTitleLocArg("");
            apnpayload.AlertMsg = alertMsg;
            //apnpayload.Badge = 1;//应用icon上显示的数字
            apnpayload.ContentAvailable = 1;//推送直接带有透传数据
            apnpayload.Category = "";
            apnpayload.Sound = "";//通知铃声文件名
            apnpayload.addCustomMsg("data", data);//增加自定义的数据           
            template.setAPNInfo(apnpayload);
            IGtPush push = new IGtPush(HOST, APPKEY, MASTERSECRET);
            /*单个用户推送接口*/
            SingleMessage Singlemessage = new SingleMessage();
            Singlemessage.Data = template;
            String pushResult = push.pushAPNMessageToSingle(APPID, token, Singlemessage);
            Console.Out.WriteLine(pushResult);
            return pushResult;
        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}