using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Model;

namespace ChatWeb.SingalR
{
    [HubName("chatHub")]
    public class ChatHub : Hub
    {
        /// <summary>
        /// 静态用户列表
        /// </summary>
        private IList<string> userList = UserInfo.userList;
        /// <summary>
        /// 用户的connectionID与用户名对照表
        /// </summary>
        private readonly static Dictionary<string, string> _connections = new Dictionary<string, string>();
        public void Hello()
        {
            Clients.All.hello();
        }
        /// <summary>
        /// 给指定好友发送消息
        /// </summary>
        /// <param name="loginid1">发起者</param>
        /// <param name="loginid2">接收者</param>
        /// <param name="message">发送的消息</param>
        public void Send(string loginid1,string loginid2,string message)
        {
            string time=DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //将当前用户发送的消息和时间进行拼接
            message = string.Format("{0},时间{1}", message, time);
            //向所有客户端推送消息
            Clients.Client(_connections[loginid2]).addSomeMessage(loginid1, message);
        }
        /// <summary>
        /// 用户上线函数
        /// </summary>
        /// <param name="name"></param>
        public void SendLogin(string loginid)
        {
            if (!userList.Contains(loginid))
            {
                userList.Add(loginid);
                //这里便是将用户id和姓名联系起来
                _connections.Add(loginid, Context.ConnectionId);
            }
            else
            {
                //每次登陆id会发生变化           
                _connections[loginid] = Context.ConnectionId;
            }
            //新用户上线，服务器广播该用户名
            Clients.All.loginUser(userList);
        }
        public class UserInfo
        {
            public static IList<string> userList = new List<string>();
        }
    }
}