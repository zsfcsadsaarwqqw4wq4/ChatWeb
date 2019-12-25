using BLL;
using ChatWeb.App_Start;
using Common;
using Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Timers;
using System.Web;
using System.Web.Mvc;
using static Model.ExModel;

namespace ChatWeb.Controllers
{
    public class FriendsController : BaseController
    {        
        public List<User> userlist;
        UserBLL ub = new UserBLL();
        User user = new User();
        FriendGroupsBLL fgb = new FriendGroupsBLL();
        // GET: Friends
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 同意添加好友
        /// </summary>
        /// <returns></returns>
        public JsonResult AddFriend()
        {
            RequestUser();
            if (resultData.res == 500)
            {
                return Json(resultData);
            }
            int userid = us.ID;
            string loginid = us.LoginID;
            int friendsid = int.Parse(GetParams("friendsid"));
            string name = GetParams("name");
            int friendtypeid = int.Parse(GetParams("friendtypeid"));
            int friendgroupsid = int.Parse(GetParams("friendgroupsid"));
            int status = 0;
            DateTime time = DateTime.Now.AddDays(7);           
            bool flag = ub.AddFirend(userid, friendsid, name, friendtypeid, friendgroupsid, status, time);
            if (!flag)
            {
                resultData.res = 500;
                resultData.msg = "同意添加好友失败";
            }
            bool s=ub.EditFirends(userid, friendsid);
            if (!s)
            {
                resultData.res = 500;
                resultData.msg = "同意添加好友失败";
            }
            int messagestypeid = 1;
            string msg = "您的好友申请已通过";
            Chat.SendMsgSystem(userid, loginid, friendsid, msg, messagestypeid);            
            resultData.msg = "同意添加好友成功";         
            return Json(resultData);
        }
        /// <summary>
        /// 搜索联系人
        /// </summary>
        /// <returns></returns>
        public JsonResult SelectFirend()
        {
            RequestUser();
            if (resultData.res == 500)
            {
                return Json(resultData);
            }           
            string loginid = GetParams("loginid");
            Regex r1=new Regex(@"^[1]+[3,5,6,7,8,9]+\d{9}$");
            if (r1.IsMatch(loginid))
            {
                user = ub.GetPhoneNumber(loginid);
            };
            Regex r2 = new Regex(@"[A-Za-z0-9\u4e00-\u9fa5]");
            if (r2.IsMatch(loginid))
            {   
                user = ub.GetUserName(loginid);
            }
            if (user != null)
            {
                if (user.ID == us.ID)
                {
                    resultData.res = 500;
                    resultData.msg = "不能添加自己为好友";
                    return Json(resultData);
                };
                var data = fgb.GetAll(us.ID);
                foreach (var temp in data)
                {
                    if(temp.FirendID.Equals(user.ID))
                    {
                        resultData.res = 500;
                        resultData.msg = "已添加该好友";
                        return Json(resultData);
                    }
                }
                user.HeadPortrait = Constant.files + user.HeadPortrait;
                resultData.msg = "成功找到联系人";
                resultData.data = user;
                return Json(resultData);
            }
            else
            {
                resultData.res = 500;
                resultData.msg = "该联系人不存在";
                return Json(resultData);
            }
        }
        
        /// <summary>
        /// 设置是否以电话号码进行搜索
        /// </summary>
        /// <returns></returns>
        public JsonResult SetSearchPhoneNumber()
        {
            RequestUser();
            string msg = "";
            if (resultData.res == 500)
            {
                return Json(resultData);
            }
            bool psearchstate = bool.Parse(GetParams("psearchstate"));
            us.PSearchState = psearchstate;
            if (!ub.EditUser(us))
            {
                resultData.msg = "设置失败";
                return Json(resultData);
            }
            else
            {
                resultData.res = 200;               
            }
            return Json(resultData);
        }   
        /// <summary>
        /// 设置是否以用户名进行搜索
        /// </summary>
        /// <returns></returns>
        public JsonResult SetSearchUser()
        {
            RequestUser();
            if (resultData.res == 500)
            {
                return Json(resultData);
            }
            bool usearchstate = bool.Parse(GetParams("usearchstate"));
            us.USearchState = usearchstate;
            if (!ub.EditUser(us))
            {
                resultData.msg = "设置失败";
                return Json(resultData);
            }
            else
            {
                resultData.res = 200;
            }
            return Json(resultData);
        }
        /// <summary>
        /// 查询好友列表
        /// </summary>
        /// <returns></returns>
        public JsonResult ListFirend()
        {
            RequestUser();
            if (resultData.res == 500)
            {
                return Json(resultData);
            }
            int userid = us.ID;
            userlist = ub.GetFriends(userid);
            if (userlist != null)
            {
                foreach (var user in userlist)
                {
                    user.HeadPortrait = Constant.files + user.HeadPortrait;
                }
            }
            if (userlist != null)
            {
                resultData.msg = "查询成功";
                resultData.data = userlist;
            }
            return Json(resultData);
        }     
        /// <summary>
        /// 提交好友申请
        /// </summary>
        /// <returns></returns>
        public JsonResult RequestFriends()
        {
            RequestUser();
            if (resultData.res == 500)
            {
                return Json(resultData);
            }
            int status = 0;
            int userid = us.ID;
            int friendsid=int.Parse(GetParams("friendsid"));            
            string name = GetParams("name");
            int friendtypeid = int.Parse(GetParams("friendtypeid"));
            int friendgroupsid = int.Parse(GetParams("friendgroupsid"));
            DateTime time = DateTime.Now.AddDays(7);
            bool flag=ub.AddFirend(userid, friendsid, name, friendtypeid, friendgroupsid, status, time);           
            if (flag)
            {
                resultData.msg = "成功提交好友申请，请耐心等待";
            }            
            return Json(resultData);
        }            
        /// <summary>
        /// 加载好友请求列表
        /// </summary>
        /// <returns></returns>
        public JsonResult RequestListFriend()
        {
            RequestUser();
            if (resultData.res == 500)
            {
                return Json(resultData);
            }
            int userid = us.ID;
            List<FriendState> userlist=ub.SelectFirend(userid);
            if (userlist.Count!=0)
            {
                DateTime time = DateTime.Now;
                foreach (var temp in userlist)
                {
                    int state = 0;
                    if (temp.endtime > time)
                    {
                        state = 1;
                    }
                    temp.user.HeadPortrait = Constant.files + temp.user.HeadPortrait;         
                    temp.State=state;
                }                
                resultData.msg = "已成功找到当前用户的好友申请列表";
                resultData.data = userlist;
            }  
            else
            {
                resultData.res = 500;
                resultData.msg = "当前用户没有好友申请";
            }
            return Json(resultData);
        }
        /// <summary>
        /// 删除过期的用户请求数据,每个月月底删除一次
        /// </summary>
        //public void Delete()
        //{
        //    DateTime time = DateTime.Now;
        //    int year = time.Year;
        //    bool isleapyear = false;
        //    if(year%4==0&&year%100!=0||year%400==0)
        //    {
        //        isleapyear = true;
        //    };
        //    if(isleapyear)
        //    {
        //    }
        //    //TimeSpan time = new TimeSpan();
        //    //Timer timer=new Timer(1000*60*60*24);
        //}
        /// <summary>
        /// 以二维码方式添加好友
        /// </summary>
        /// <returns></returns>
        //public JsonResult AddQrcode()
        //{
        //    RequestUser();
        //    int userid = us.ID;
        //    int friendsid = int.Parse(GetParams("friendsid"));
        //    string name = GetParams("name");
        //    int friendtypeid = int.Parse(GetParams("friendtypeid"));
        //    int friendgroupsid = int.Parse(GetParams("friendgroupsid"));
        //    int status = 1;
        //    return Json(new { });
        //}
    }
}