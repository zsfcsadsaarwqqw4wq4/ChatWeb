
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Web.WebSockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using Common;
using System.Threading.Tasks;
using BLL;
using ChatWeb.App_Start;

namespace ChatWeb
{
    /// <summary>
    /// 离线消息类
    /// </summary>
    public class MessageInfo
    {        
        public MessageInfo(ArraySegment<byte> _MsgContent)
        {
            MsgContent = _MsgContent;
        }
        /// <summary>
        /// 发送的消息内容
        /// </summary>
        public ArraySegment<byte> MsgContent { get; set; }
    }
    public class Chat : IHttpHandler
    {
        private static Dictionary<int, WebSocket> CONNECT_POOL = new Dictionary<int, WebSocket>(); //用户连接池
        private static Dictionary<int, WebSocket> CONNECT_TMP_POOL = new Dictionary<int, WebSocket>(); //临时连接池      
        private static Dictionary<int, List<MessageInfo>> MESSAGE_POOL = new Dictionary<int, List<MessageInfo>>();//离线消息池 
        public void ProcessRequest(HttpContext context)
        {
            //检查 查询是否是WebSocket请求
            if (HttpContext.Current.IsWebSocketRequest)
            {
                //如果是，我们附加异步处理程序
                context.AcceptWebSocketRequest(WebSocketRequestHandler);
            }
        }
        //异步请求处理程序
        public async Task WebSocketRequestHandler(AspNetWebSocketContext webSocketContext)
        {
            try
            {
                //获取当前的WebSocket对象
                WebSocket webSocket = webSocketContext.WebSocket;
                //我们定义一个常数，它将表示接收到的数据的大小。 它是由我们建立的，我们可以设定任何值。 我们知道在这种情况下，发送的数据的大小非常小。
                const int maxMessageSize = 2048;
                Dictionary<string, string> linkRes = new Dictionary<string, string>(); //链接结果
                //received bits的缓冲区
                var receivedDataBuffer = new ArraySegment<byte>(new byte[maxMessageSize]);
                var cancellationToken = new CancellationToken();
                string token = webSocketContext.QueryString["token"].ToString();
                AuthInfo authInfo = JwtHelper.GetJwtDecode(token);
                int uid_main = authInfo.ID;
                #region 用户添加连接池
                //第一次open时，添加到连接池中
                if (!CONNECT_POOL.ContainsKey(uid_main))
                {
                    try
                    {
                        CONNECT_POOL.Add(uid_main, webSocket); //不存在，添加
                    }
                    catch
                    {
                        try
                        {
                            if (webSocket != CONNECT_POOL[uid_main] && webSocket != null) //当前对象不一致，更新
                            {
                                CONNECT_POOL[uid_main] = webSocket;
                            }
                        }
                        catch { }
                    }                 
                }
                else
                {
                    try
                    {
                        if (webSocket != CONNECT_POOL[uid_main] && webSocket != null) //当前对象不一致，更新
                        {
                            CONNECT_POOL[uid_main] = webSocket;
                        }
                    }
                    catch { }
                }
                #endregion
                #region 离线消息处理
                if (MESSAGE_POOL.ContainsKey(uid_main))
                {
                    List<MessageInfo> msgs = MESSAGE_POOL[uid_main];
                    foreach (MessageInfo item in msgs)
                    {
                        await webSocket.SendAsync(item.MsgContent, WebSocketMessageType.Text, true, cancellationToken);                       
                    }   
                    MESSAGE_POOL.Remove(uid_main);//移除离线消息
                }
                #endregion
                while (webSocket.State == WebSocketState.Open)
                {
                    //读取数据
                    WebSocketReceiveResult webSocketReceiveResult = await webSocket.ReceiveAsync(receivedDataBuffer, cancellationToken);
                    //如果输入帧为取消帧，发送close命令
                    if (webSocketReceiveResult.MessageType == WebSocketMessageType.Close)
                    {
                        if (CONNECT_POOL.ContainsKey(uid_main) && webSocket == CONNECT_POOL[uid_main])
                        {
                            CONNECT_POOL.Remove(uid_main);//删除连接池
                            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, cancellationToken);
                            break;
                        }
                    }
                    else
                    {
                        //发过来的消息
                        string receiveToken = Encoding.UTF8.GetString(receivedDataBuffer.Array, 0, webSocketReceiveResult.Count);                        
                        try
                        {                           
                            authInfo = JwtHelper.GetJwtDecode(receiveToken);
                        }
                        catch
                        {
                            continue;
                        }
                        Dictionary<string, string> res = new Dictionary<string, string>();
                        
                        res.Add("res", "100");
                        #region 发回结果
                        //心跳接收成功，返回给当前目标用户
                        CONNECT_TMP_POOL = new Dictionary<int, WebSocket>(CONNECT_POOL);                                                                   
                        try
                        {
                            if (CONNECT_POOL.ContainsKey(authInfo.ID))
                            {
                                WebSocket destSocket = CONNECT_TMP_POOL[authInfo.ID]; //目的客户端
                                if (destSocket != null && destSocket.State == WebSocketState.Open)
                                {
                                    byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(res));
                                    ArraySegment<byte> buffer = new ArraySegment<byte>(bytes);
                                    await destSocket.SendAsync(buffer, WebSocketMessageType.Text, true, new CancellationToken());
                                }
                            }
                        }
                        catch { }
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Tools.WriteLog(AppDomain.CurrentDomain.BaseDirectory + "/Log/ALL.txt", "[" + DateTime.Now.ToString("HH:mm:ss") + "] " + ex.Message + "\r\n");
            }
        }
        /// <summary>
        /// 发送消息给指定用户
        /// </summary>
        /// <param name="uid">接收者id</param>
        /// <param name="msg">发送的消息</param>
        public static void SendMsgToUser(int userid, string loginid,int uid, string msg,string guid,int messagestypeid,bool isBART,object res,dynamic flasedata,bool flag)
        {
            //发送成功，返回给所有目标用户
            CONNECT_TMP_POOL = new Dictionary<int, WebSocket>(CONNECT_POOL);
            try
            {
                WebSocket destSocket = CONNECT_TMP_POOL[uid]; //目的客户端
                if (destSocket != null && destSocket.State == WebSocketState.Open)
                {
                    DateTime time = DateTime.Now;
                    UserBLL ub = new UserBLL();
                    string result = ub.GetUserHeadImg(userid);
                    var temp = new
                    {
                        userid=userid,
                        time = time,
                        msg= msg,
                        loginid=loginid,
                        uid=uid,
                        guid= guid,
                        userheadportrait = Constant.files + result,
                        messagestypeid = messagestypeid,
                        isBART = isBART
                    };
                    string data=JsonConvert.SerializeObject(temp);
                    byte[] bytes = Encoding.UTF8.GetBytes(data);
                    ArraySegment<byte> buffer = new ArraySegment<byte>(bytes);
                    destSocket.SendAsync(buffer, WebSocketMessageType.Text, true, new CancellationToken());                    
                }
            }
            catch(Exception ex)
            {
                UserBLL ub = new UserBLL();
                var users=ub.GetUserById(userid);
                string msgs = string.Empty;
                Redis redis = new Redis();
                var datas = redis.StringGet(uid.ToString());
                string token = string.Empty;
                string device = string.Empty;
                if (datas != null)
                {
                    token = datas["token"].ToString();
                    device = datas["device"].ToString();
                }
                if (Convert.ToBoolean(users.ChatSwitch))
                {
                    msgs = "私聊消息";
                }
                else
                {
                    msgs = DecodeBase64(msg);                    
                }
                if ("1".Equals(device))
                {
                    if (flasedata != null)
                    {
                        string title = flasedata.title;
                        string content = flasedata.content;
                        Push.PushMessageToSingle(title,content, JsonConvert.SerializeObject(res), token);
                    }
                    else
                    {
                        if (flag)
                        {
                            Push.PushMessageToSingle(loginid, "阅后即焚", JsonConvert.SerializeObject(res), token);
                        }
                        else
                        {
                            Push.PushMessageToSingle(loginid, msgs, JsonConvert.SerializeObject(res), token);
                        }
                    }
                }
                if ("2".Equals(device))
                {
                    if (flasedata != null)
                    {
                        string title = flasedata.title;
                        string content = flasedata.content;
                        Push.APNsPushToSingle(title, content, token, res);
                    }
                    else
                    {
                        if (flag)
                        {
                            Push.APNsPushToSingle(loginid, "阅后即焚", token, res);
                        }
                        else
                        {
                            Push.APNsPushToSingle(loginid, msgs, token, res);
                        }
                    }
                }
                DateTime time = DateTime.Now;                
                string result=ub.GetUserHeadImg(userid);
                var temp = new
                {
                    userid = userid,
                    time = time,
                    msg = msg,
                    loginid = loginid,
                    uid = uid,
                    userheadportrait=Constant.files+ result,
                    guid=guid,
                    messagestypeid = EnumHelper.Message.four,
                    isBART = isBART
                };
                string data = JsonConvert.SerializeObject(temp);
                byte[] bytes = Encoding.UTF8.GetBytes(data);
                ArraySegment<byte> buffer = new ArraySegment<byte>(bytes);
                try
                {
                    MESSAGE_POOL.Add(uid, new List<MessageInfo>());
                    MESSAGE_POOL[uid].Add(new MessageInfo(buffer));
                }
                catch
                {
                    MESSAGE_POOL[uid].Add(new MessageInfo(buffer));//添加离线消息
                }
            }
        }
        /// <summary>
        /// 发送系统消息给指定用户
        /// </summary>
        /// <param name="userid">当前登录用户id</param>
        /// <param name="loginid">当前用户的用户名</param>
        /// <param name="uid">接收者id</param>
        /// <param name="msg">发送的消息</param>
        /// <param name="messagestypeid">消息类型1:系统消息2:聊天消息</param>
        public static void SendMsgSystem(int userid, string loginid, int uid, object msg, int messagestypeid)
        {
            //发送成功，返回给所有目标用户
            CONNECT_TMP_POOL = new Dictionary<int, WebSocket>(CONNECT_POOL);
            try
            {
                WebSocket destSocket = CONNECT_TMP_POOL[uid]; //目的客户端
                if (destSocket != null && destSocket.State == WebSocketState.Open)
                {   
                    DateTime time = DateTime.Now;
                    UserBLL ub = new UserBLL();
                    string result = ub.GetUserHeadImg(userid);
                    var temp = new
                    {
                        userid = userid,
                        time = time,
                        msg = msg,
                        loginid = loginid,
                        uid = uid,
                        userheadportrait = Constant.files + result,
                        messagestypeid = messagestypeid
                    };
                    string data = JsonConvert.SerializeObject(temp);
                    byte[] bytes = Encoding.UTF8.GetBytes(data);
                    ArraySegment<byte> buffer = new ArraySegment<byte>(bytes);
                    destSocket.SendAsync(buffer, WebSocketMessageType.Text, true, new CancellationToken());
                }
            }
            catch(Exception ex) {
                
            }
        }
        /// <summary>
        /// 将已经读的消息guid发送给用户
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="uid"></param>
        /// <param name="guid"></param>
        public static void SendMsg(int userid, int uid, string guid, int messagestypeid)
        {
            //发送成功，返回给所有目标用户
            CONNECT_TMP_POOL = new Dictionary<int, WebSocket>(CONNECT_POOL);

            try
            {
                WebSocket destSocket = CONNECT_TMP_POOL[uid]; //目的客户端
                if (destSocket != null && destSocket.State == WebSocketState.Open)
                {
                    DateTime time = DateTime.Now;
                    UserBLL ub = new UserBLL();
                    string result = ub.GetUserHeadImg(userid);
                    var temp = new
                    {
                        userid = userid,
                        time = time,
                        uid = uid,
                        guid = guid,
                        userheadportrait = Constant.files + result,
                        messagestypeid = messagestypeid
                    };
                    string data = JsonConvert.SerializeObject(temp);
                    byte[] bytes = Encoding.UTF8.GetBytes(data);
                    ArraySegment<byte> buffer = new ArraySegment<byte>(bytes);
                    destSocket.SendAsync(buffer, WebSocketMessageType.Text, true, new CancellationToken());
                }
            }
            catch
            {
                DateTime time = DateTime.Now;
                UserBLL ub = new UserBLL();
                string result = ub.GetUserHeadImg(userid);
                var temp = new
                {
                    userid = userid,
                    time = time,
                    uid = uid,
                    guid = guid,
                    userheadportrait = Constant.files + result,
                    messagestypeid = messagestypeid
                };
                string data = JsonConvert.SerializeObject(temp);
                byte[] bytes = Encoding.UTF8.GetBytes(data);
                ArraySegment<byte> buffer = new ArraySegment<byte>(bytes);
                try
                {
                    MESSAGE_POOL.Add(uid, new List<MessageInfo>());
                    MESSAGE_POOL[uid].Add(new MessageInfo(buffer));
                }
                catch
                {
                    MESSAGE_POOL[uid].Add(new MessageInfo(buffer));//添加离线消息
                }

            }
        }
        /// <summary>
        /// 发送图片个指定用户
        /// </summary>
        /// <param name="userid">当前登录用户id</param>
        /// <param name="loginid">当前用户用户名</param>
        /// <param name="uid">接收者用户id</param>
        /// <param name="img">图片链接</param>
        public static void SendPhotoToUser(int userid, string loginid, int uid, string Url, int messagestypeid, string guid)
        {
            //发送成功，返回给所有目标用户
            CONNECT_TMP_POOL = new Dictionary<int, WebSocket>(CONNECT_POOL);
            try
            {
                WebSocket destSocket = CONNECT_TMP_POOL[uid]; //目的客户端
                if (destSocket != null && destSocket.State == WebSocketState.Open)
                {
                    DateTime time = DateTime.Now;
                    UserBLL ub = new UserBLL();
                    string result = ub.GetUserHeadImg(userid);
                    var temp = new
                    {
                        userid = userid,
                        time = time,
                        Url = Url,
                        loginid = loginid,
                        uid = uid,
                        guid = guid,
                        userheadportrait = Constant.files + result,
                        messagestypeid = messagestypeid,
                    };
                    string data = JsonConvert.SerializeObject(temp);
                    byte[] bytes = Encoding.UTF8.GetBytes(data);
                    ArraySegment<byte> buffer = new ArraySegment<byte>(bytes);
                    destSocket.SendAsync(buffer, WebSocketMessageType.Text, true, new CancellationToken());
                }
            }
            catch
            {

                DateTime time = DateTime.Now;
                UserBLL ub = new UserBLL();
                string result = ub.GetUserHeadImg(userid);
                var temp = new
                {
                    userid = userid,
                    time = time,
                    Url = Url,
                    loginid = loginid,
                    uid = uid,
                    guid = guid,
                    userheadportrait = Constant.files + result,
                    messagestypeid = messagestypeid
                };
                string data = JsonConvert.SerializeObject(temp);
                byte[] bytes = Encoding.UTF8.GetBytes(data);
                ArraySegment<byte> buffer = new ArraySegment<byte>(bytes);
                try
                {
                    MESSAGE_POOL.Add(uid, new List<MessageInfo>());
                    MESSAGE_POOL[uid].Add(new MessageInfo(buffer));
                }
                catch
                {
                    MESSAGE_POOL[uid].Add(new MessageInfo(buffer));//添加离线消息
                }
            }
        }
        /// <summary>
        /// 发送图片个指定用户
        /// </summary>
        /// <param name="userid">当前登录用户id</param>
        /// <param name="loginid">当前用户用户名</param>
        /// <param name="uid">接收者用户id</param>
        /// <param name="img">图片链接</param>
        public static void SendPhotoToUser(int userid, string loginid, int uid, string img, int messagestypeid, string guid,object mock,bool isBART,int width, int height, dynamic flasedata)
        {
            //发送成功，返回给所有目标用户
            CONNECT_TMP_POOL = new Dictionary<int, WebSocket>(CONNECT_POOL);
            try
            {
                WebSocket destSocket = CONNECT_TMP_POOL[uid]; //目的客户端
                if (destSocket != null && destSocket.State == WebSocketState.Open)
                {
                    DateTime time = DateTime.Now;
                    UserBLL ub = new UserBLL();
                    string result = ub.GetUserHeadImg(userid);
                    var temp = new
                    {
                        userid = userid,
                        time = time,
                        img = img,
                        loginid = loginid,
                        uid = uid,
                        guid = guid,
                        userheadportrait = Constant.files + result,
                        messagestypeid = messagestypeid,
                        mock=mock,
                        isBART= isBART,
                        width,
                        height
                    };
                    string data = JsonConvert.SerializeObject(temp);
                    byte[] bytes = Encoding.UTF8.GetBytes(data);
                    ArraySegment<byte> buffer = new ArraySegment<byte>(bytes);
                    destSocket.SendAsync(buffer, WebSocketMessageType.Text, true, new CancellationToken());
                }
            }
            catch {
                UserBLL ub = new UserBLL();
                Redis redis = new Redis();
                var datas = redis.StringGet(uid.ToString());
                string token = string.Empty;
                string device = string.Empty;
                if (datas != null)
                {
                    token = datas["token"].ToString();
                    device = datas["device"].ToString();
                }
                if ("1".Equals(device))
                {
                    var res = new
                    {
                        userid = userid,
                        uid = uid,
                        img = img
                    };
                    if (flasedata != null)
                    {
                        Push.PushMessageToSingle(flasedata.title, "这是一条回复[图片]吧", JsonConvert.SerializeObject(res), token);
                    }
                    else
                    {
                        Push.PushMessageToSingle(loginid, "[图片]", JsonConvert.SerializeObject(res), token);
                    }
                }
                if ("2".Equals(device))
                {
                    var res = new
                    {
                        userid = userid,
                        uid = uid,
                        img = img
                    };
                    if (flasedata != null)
                    {
                        string title = flasedata.title;
                        string content = flasedata.content;
                        Push.APNsPushToSingle(title, "这是一条回复[图片]吧", token, res);
                    }
                    else
                    {
                        Push.APNsPushToSingle(loginid, "[图片]", token, res);
                    }
                }
                DateTime time = DateTime.Now;
                string result = ub.GetUserHeadImg(userid);
                var temp = new
                {
                    userid = userid,
                    time = time,
                    img = img,
                    loginid = loginid,
                    uid = uid,
                    guid = guid,
                    userheadportrait = Constant.files + result,
                    messagestypeid = messagestypeid,
                    mock = mock,
                    isBART = isBART,
                    width,
                    height
                };
                string data = JsonConvert.SerializeObject(temp);                    
                byte[] bytes = Encoding.UTF8.GetBytes(data);
                ArraySegment<byte> buffer = new ArraySegment<byte>(bytes);
                try
                {
                    MESSAGE_POOL.Add(uid, new List<MessageInfo>());
                    MESSAGE_POOL[uid].Add(new MessageInfo(buffer));
                }
                catch
                {
                    MESSAGE_POOL[uid].Add(new MessageInfo(buffer));//添加离线消息
                }
            }
        }
        public static void SendPhotoToUser(int userid, string loginid, int uid, string img, int messagestypeid, string guid, object mock, bool isBART, int width, int height, dynamic flasedata,string thumbImg)
        {
            //发送成功，返回给所有目标用户
            CONNECT_TMP_POOL = new Dictionary<int, WebSocket>(CONNECT_POOL);
            try
            {
                WebSocket destSocket = CONNECT_TMP_POOL[uid]; //目的客户端
                if (destSocket != null && destSocket.State == WebSocketState.Open)
                {
                    DateTime time = DateTime.Now;
                    UserBLL ub = new UserBLL();
                    string result = ub.GetUserHeadImg(userid);
                    var temp = new
                    {
                        userid = userid,
                        time = time,
                        img = img,
                        loginid = loginid,
                        uid = uid,
                        guid = guid,
                        userheadportrait = Constant.files + result,
                        messagestypeid = messagestypeid,
                        mock = mock,
                        isBART = isBART,
                        width,
                        height,
                        thumbImg
                    };
                    string data = JsonConvert.SerializeObject(temp);
                    byte[] bytes = Encoding.UTF8.GetBytes(data);
                    ArraySegment<byte> buffer = new ArraySegment<byte>(bytes);
                    destSocket.SendAsync(buffer, WebSocketMessageType.Text, true, new CancellationToken());
                }
            }
            catch
            {
                UserBLL ub = new UserBLL();
                Redis redis = new Redis();
                var datas = redis.StringGet(uid.ToString());
                string token = string.Empty;
                string device = string.Empty;
                if (datas != null)
                {
                    token = datas["token"].ToString();
                    device = datas["device"].ToString();
                }
                //if ("1".Equals(device))
                //{
                //    if (flasedata != null)
                //    {
                //        Push.PushMessageToSingle(flasedata.content, JsonConvert.SerializeObject(res), token);
                //    }
                //    else
                //    {
                //        Push.PushMessageToSingle(msgs, JsonConvert.SerializeObject(res), token);
                //    }
                //}
                if ("2".Equals(device))
                {
                    var res = new
                    {
                        userid = userid,
                        uid = uid,
                        img = img
                    };
                    if (flasedata != null)
                    {
                        string title = flasedata.title;
                        string content = flasedata.content;
                        Push.APNsPushToSingle(title, "这是一条回复[图片]吧", token, res);
                    }
                    else
                    {
                        Push.APNsPushToSingle(loginid, "[图片]", token, res);
                    }
                }
                DateTime time = DateTime.Now;
                string result = ub.GetUserHeadImg(userid);
                var temp = new
                {
                    userid = userid,
                    time = time,
                    img = img,
                    loginid = loginid,
                    uid = uid,
                    guid = guid,
                    userheadportrait = Constant.files + result,
                    messagestypeid = messagestypeid,
                    mock = mock,
                    isBART = isBART,
                    width,
                    height,
                    thumbImg
                };
                string data = JsonConvert.SerializeObject(temp);
                byte[] bytes = Encoding.UTF8.GetBytes(data);
                ArraySegment<byte> buffer = new ArraySegment<byte>(bytes);
                try
                {
                    MESSAGE_POOL.Add(uid, new List<MessageInfo>());
                    MESSAGE_POOL[uid].Add(new MessageInfo(buffer));
                }
                catch
                {
                    MESSAGE_POOL[uid].Add(new MessageInfo(buffer));//添加离线消息
                }
            }
        }
        /// <summary>
        /// 发送图片个指定用户
        /// </summary>
        /// <param name="userid">当前登录用户id</param>
        /// <param name="loginid">当前用户用户名</param>
        /// <param name="uid">接收者用户id</param>
        /// <param name="img">图片链接</param>
        public static void SendPhotoToUsers(int userid, string loginid, int uid, string img, int messagestypeid,string guid,string ext,string width,string height,object imgURLs)
        {
            //发送成功，返回给所有目标用户
            CONNECT_TMP_POOL = new Dictionary<int, WebSocket>(CONNECT_POOL);
            try
            {                
                WebSocket destSocket = CONNECT_TMP_POOL[uid]; //目的客户端
                if (destSocket != null && destSocket.State == WebSocketState.Open)
                {
                    DateTime time = DateTime.Now;
                    UserBLL ub = new UserBLL();
                    string result = ub.GetUserHeadImg(userid);
                    var temp = new
                    {
                        userid = userid,
                        time = time,
                        img = img,
                        loginid = loginid,
                        uid = uid,
                        guid=guid,
                        ext=ext,
                        width=width,
                        height = height,
                        messagestypeid = messagestypeid,
                        userheadportrait = Constant.files + result,
                        imgURLs = imgURLs
                    };
                    string data = JsonConvert.SerializeObject(temp);
                    byte[] bytes = Encoding.UTF8.GetBytes(data);
                    ArraySegment<byte> buffer = new ArraySegment<byte>(bytes);
                    destSocket.SendAsync(buffer, WebSocketMessageType.Text, true, new CancellationToken());
                }
            }
            catch {
                Redis redis = new Redis();
                var datas = redis.StringGet(uid.ToString());
                string token = datas["token"].ToString();
                string device = datas["device"].ToString();                
                string msg = "图片";
                if ("1".Equals(device))
                {

                };
                if ("2".Equals(device))
                {
                    Push.APNsPushToSingle("", msg, token, null);
                }
                DateTime time = DateTime.Now;
                UserBLL ub = new UserBLL();
                string result = ub.GetUserHeadImg(userid);
                var temp = new
                {
                    userid = userid,
                    time = time,
                    img = img,
                    loginid = loginid,
                    uid = uid,
                    guid = guid,
                    ext = ext,
                    width = width,
                    height = height,
                    messagestypeid = messagestypeid,
                    userheadportrait = Constant.files + result,
                    imgURLs = imgURLs
                };
                string data = JsonConvert.SerializeObject(temp);
                byte[] bytes = Encoding.UTF8.GetBytes(data);
                ArraySegment<byte> buffer = new ArraySegment<byte>(bytes);
                try
                {
                    MESSAGE_POOL.Add(uid, new List<MessageInfo>());
                    MESSAGE_POOL[uid].Add(new MessageInfo(buffer));
                }
                catch
                {
                    MESSAGE_POOL[uid].Add(new MessageInfo(buffer));//添加离线消息
                }
            }
        }
        /// <summary>
        /// 通知添加好友接口
        /// </summary>
        /// <param name="id"></param>
        /// <param name="loginid"></param>
        /// <param name="headportrait"></param>
        public static void AddNewFriends(object us,int messagestypeid,int State,int friendsid)
        {
            //发送成功，返回给所有目标用户
            CONNECT_TMP_POOL = new Dictionary<int, WebSocket>(CONNECT_POOL);
            try
            {
                WebSocket destSocket = CONNECT_TMP_POOL[friendsid]; //目的客户端
                if (destSocket != null && destSocket.State == WebSocketState.Open)
                {
                    DateTime time = DateTime.Now;
                    var temp = new
                    {
                        user = us,
                        messagestypeid = messagestypeid,
                        State = State
                    };
                    string data = JsonConvert.SerializeObject(temp);
                    byte[] bytes = Encoding.UTF8.GetBytes(data);
                    ArraySegment<byte> buffer = new ArraySegment<byte>(bytes);
                    destSocket.SendAsync(buffer, WebSocketMessageType.Text, true, new CancellationToken());
                }
            }
            catch
            {
                var temp = new
                {
                    user = us,
                    messagestypeid = messagestypeid,
                    State = State
                };
                string data = JsonConvert.SerializeObject(temp);
                byte[] bytes = Encoding.UTF8.GetBytes(data);
                ArraySegment<byte> buffer = new ArraySegment<byte>(bytes);
                try
                {
                    MESSAGE_POOL.Add(friendsid, new List<MessageInfo>());
                    MESSAGE_POOL[friendsid].Add(new MessageInfo(buffer));
                }
                catch
                {
                    MESSAGE_POOL[friendsid].Add(new MessageInfo(buffer));//添加离线消息
                }
            }
        }
        /// <summary>
        /// base64编码
        /// </summary>
        /// <param name="code_type"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string EncodeBase64(string code)
        {
            string encode = "";
            byte[] bytes = Encoding.UTF8.GetBytes(code);
            try
            {
                encode = Convert.ToBase64String(bytes);
            }
            catch
            {
                encode = code;
            }
            return encode;
        }

        /// <summary>
        /// base64解码
        /// </summary>
        /// <param name="code_type"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string DecodeBase64(string code)
        {
            string decode = "";
            byte[] bytes = Convert.FromBase64String(code);
            try
            {
                decode = Encoding.UTF8.GetString(bytes);
            }
            catch
            {
                decode = code;
            }
            return decode;
        }
        public bool IsReusable { get { return false; } }
    }
}