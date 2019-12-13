using BLL;
using Common;
using Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using ChatWeb.App_Start;

namespace ChatWeb.Controllers
{
    public class UserController : Controller
    {
        public class ResultUser
        {
            public ResultUser()
            {
                res = 500;
                msg = "请稍后在尝试";
            }
            /// <summary>
            /// 状态码 200 成功 500 失败
            /// </summary>
            public int res { get; set; }
            /// <summary>
            /// 提示信息
            /// </summary>
            public string msg { get; set; }
            /// <summary>
            /// 返回的数据结果
            /// </summary>
            public object data { get; set; }
            /// <summary>
            /// 表示用户登录状态，0:两个密码输入错误 1：登录的是聊天界面 2：工作界面
            /// </summary>
            public int state { get; set; }
        }
        /// <summary>
        /// 参数
        /// </summary>
        private JObject obj;
        // GET: User
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Login()
        {
            Redis redis=new Redis();
            ResultUser resultUser = new ResultUser();
            UserBLL ub=new UserBLL();
            InvitationBLL ib=new InvitationBLL();
            try
            {
                //string loginids = Request["loginid"];
                //string passwords = Request["password"];
                using (StreamReader sr = new StreamReader(Request.InputStream))
                {
                    string json = sr.ReadToEnd();                    
                    if(string.IsNullOrEmpty(json))
                    {
                        resultUser.msg = "没有获取到用户名和密码";
                        return Json(resultUser);
                    }
                    obj = JObject.Parse(json);
                }
                string password=MD5Helper.MD5Encrypt32(obj["password"].ToString());              
                User user = new User()
                {
                    LoginID = obj["loginid"].ToString()
                };
                user = ub.GetUser(user);
                if (user==null)
                {
                    resultUser.msg = "该用户不存在";
                    return Json(resultUser);
                }
                else
                {
                    var result=redis.StringGet(user.LoginID);
                    if (result!=null)
                    {
                        resultUser.msg = "该用户已经登录了";
                        return Json(resultUser);
                    }
                    else if (password.Equals(user.PassWord))
                    {
                        resultUser.res = 200;
                        resultUser.state = 1;
                        resultUser.msg = "用户是登录的私密聊天";
                        resultUser.data = JwtHelper.CreateToken(user);
                        var times = 2*60* 1000;
                        var time=TimeSpan.FromMilliseconds(times);              
                        //用redis保存用户登录的信息
                        redis.StringSet(user.LoginID,user,time);
                    }
                    else if (password.Equals(user.PassWords))
                    {
                        resultUser.res = 200;
                        resultUser.state = 2;
                        resultUser.msg = "用户是登录的工作聊天";
                        resultUser.data = JwtHelper.CreateToken(user);
                    }
                    else
                    {
                        resultUser.msg = "密码错误，请重新输入";
                    };
                }
            }
            catch (HttpException ex)
            {

            }
            return Json(resultUser);
        }
        /// <summary>
        /// 注册
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Register()
        {
            ResultUser resultUser = new ResultUser();
            UserBLL ub = new UserBLL();
            InvitationBLL ib = new InvitationBLL();            
            try
            {
                using (StreamReader sr = new StreamReader(Request.InputStream))
                {
                    string json = sr.ReadToEnd();
                    if (string.IsNullOrEmpty(json))
                    {
                        resultUser.msg = "没有获取到用户名和密码和邀请码";
                        return Json(resultUser);
                    }
                    obj = JObject.Parse(json);
                }
                string loginid=obj["loginid"].ToString();                
                bool msg = ub.GetUserIsRegister(loginid);
                if(msg)
                {
                    resultUser.msg = "该用户名已被注册";
                    return Json(resultUser);
                };
                int InviteCode = int.Parse(obj["invitecode"].ToString());
                Invitation invitation = new Invitation()
                {
                    InviteCode = InviteCode,
                };
                Invitation ivs =ib.GetInvitation(invitation);
                if (ivs == null)
                {
                    resultUser.msg = "该邀请码不存在";
                    return Json(resultUser);
                }
                if (ivs.EndTime<DateTime.Now)
                {
                    resultUser.msg = "邀请码已过期";
                    return Json(resultUser);
                }
                if (ivs!=null)
                {
                    Random random = new Random();
                    int result = random.Next(2, 8);
                    User user = new User();
                    user.LoginID = loginid;                   
                    user.PassWord = MD5Helper.MD5Encrypt32(obj["password"].ToString());
                    user.HeadPortrait = "/Images/head/" + result +".jpg";
                    user.PSearchState = false;
                    user.USearchState = true;
                    user.BurnAfterReading = false;
                    user.ThemeTypeID = 1;
                    user.SecondThemeTypeID = 1;
                    user.IsEnterSendMsg = false;    
                    user.ChatTimeLimit = "0";
                    user.Shape = 1;

                    if (ub.CreateUser(user))
                    {
                        resultUser.res = 200;
                        resultUser.msg = "注册成功"; 
                        resultUser.data = JwtHelper.CreateToken(user);
                        return Json(resultUser);
                    }
                    ib.DeleteInvitation(InviteCode);
                }                  

            }
            catch (HttpException ex)
            {

            }
            return Json(resultUser);
        }   
    }
}