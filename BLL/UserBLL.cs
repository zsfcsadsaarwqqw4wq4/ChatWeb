using DAL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Model.ExModel;

namespace BLL
{
    public class UserBLL
    {
        UserDAL ud = new UserDAL();
        /// <summary>
        /// 根据账户和密码查询用户
        /// </summary>
        /// <param name="us"></param>
        /// <returns></returns>
        public User GetUser(User us)
        {
            return ud.GetUser(us);
        }
        /// <summary>
        /// //根据ID查询用户信息
        /// </summary>
        /// <param name="us"></param>
        /// <returns></returns>
        public User GetUserById(int Id)
        {
            return ud.GetUserById(Id);
        }
        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="us"></param>
        /// <returns></returns>
        public bool CreateUser(User us)
        {
            return ud.CreateUser(us);
        }
        /// <summary>
        /// 查询用户是否已被注册
        /// </summary>
        /// <param name="loginid">登录账号</param>
        /// <returns></returns>
        public bool GetUserIsRegister(string loginid)
        {
            return ud.GetUserIsRegister(loginid);
        }
        /// <summary>
        /// 获取当前用户的全部好友
        /// </summary>
        /// <param name="loginid"></param>
        /// <returns></returns>
        public List<User> GetFriends(int id)
        {
            return ud.GetFriends(id);
        }
        /// <summary>
        /// 根据登录名搜索联系人
        /// </summary>
        public User GetUserName(string loginid)
        {
            return ud.GetUserName(loginid);
        }
        /// <summary>
        /// 根据登录名查询用户
        /// </summary>
        /// <returns></returns>
        public User GetUserNames(string loginid)
        {
            return ud.GetUserNames(loginid);
        }
        /// <summary>
        /// 根据电话号码查询用户
        /// </summary>
        /// <returns></returns>
        public User GetPhoneNumber(string loginid)
        {
            return ud.GetPhoneNumber(loginid);
        }
        /// <summary>
        /// 申请好友
        /// </summary>
        /// <returns></returns>
        public bool AddFirend(int userid, int friendsid, string name, int friendtypeid, int friendgroupsid, int status,DateTime time)
        {
            return ud.AddFirend(userid, friendsid, name, friendtypeid, friendgroupsid, status, time);
        }
        /// <summary>
        /// 通过好友申请
        /// </summary>
        /// <param name="userid">当前用户id</param>
        /// <param name="firendid">朋友id</param>
        /// <returns></returns>
        public bool EditFirends(int userid, int firendid)
        {
            return ud.EditFirends(userid, firendid);
        }
        /// <summary>
        /// 当前用户所有的好友申请
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public List<FriendState> SelectFirend(int userid)
        {
            return ud.SelectFirend(userid);
        }
        /// <summary>
        /// 修改用户信息
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool EditUser(User user)
        {
            return ud.EditUser(user);
        }
        /// <summary>
        /// 设置用户的迷惑密码
        /// </summary>
        /// <returns></returns>
        public bool EditUsers(User user)
        {
            return ud.EditUsers(user);
        }
        /// <summary>
        /// 如果没有迷惑密码，默认1111
        /// </summary>
        public bool UpdatePassword(User user)
        {
            return ud.UpdatePassword(user);
        }
        /// <summary>
        /// 获取用户头像
        /// </summary>
        /// <returns></returns>
        public string GetUserHeadImg(int uid)
        {
            return ud.GetUserHeadImg(uid);
        }
        /// <summary>
        /// 修改用户登陆时间
        /// </summary>
        public void UpdateUser(int userid, DateTime time)
        {
            ud.UpdateUser(userid, time);
        }
        /// <summary>
        /// 查询用户好友请求记录
        /// </summary>
        /// <param name="Userid"></param>
        /// <returns></returns>
        public List<FriendState> QueryFriendRequest(int userid)
        {
            return ud.QueryFriendRequest(userid);
        }
        /// <summary>
        /// 设置用户的迷惑密码
        /// </summary>
        /// <returns></returns>
        public bool EditChatSwitch(int id)
        {
            return ud.EditChatSwitch(id);
        }
    }
}
