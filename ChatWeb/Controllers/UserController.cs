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
using System.Text.RegularExpressions;

namespace ChatWeb.Controllers
{
    public class UserController : Controller
    {
        Redis redis = new Redis();
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
                string loginid = obj["loginid"].ToString();
                Regex r1 = new Regex(@"^[a-zA-Z-0-9]{3,16}");
                User user = new User();
                if (r1.IsMatch(loginid))
                {
                    user.LoginID = loginid;
                }
                else
                {
                    resultUser.msg = "登录名格式不对";
                    return Json(resultUser);
                };
                string token = obj["token"].ToString();
                if (string.IsNullOrEmpty(token))
                {
                    resultUser.msg = "登录失败";
                    return Json(resultUser);
                }
                user = ub.GetUser(user);
                if (user==null)
                {
                    resultUser.msg = "该用户不存在";
                    return Json(resultUser);
                }
                else
                {
                    DateTime time = DateTime.Now;
                    bool value = false;
                    if (string.IsNullOrEmpty(user.PassWords))
                    {
                        user.PassWords = MD5Helper.MD5Encrypt32("1111");
                        if (ub.UpdatePassword(user))
                        {
                            value = true;
                        }                       
                    }
                    var result=redis.StringGet(user.LoginID);
                    if (result!=null)
                    {
                        //如果该账户已经登录则通过第三方推送将消息通知给上一个用户
                        var users = ub.GetUserName(user.LoginID);
                        var datas = redis.StringGet(users.ID.ToString());
                        if (datas != null)
                        {
                            string device = datas["device"].ToString();
                            string tokens = datas["token"].ToString();
                            object PenetrateMsg = null;
                            if ("2".Equals(device))
                            {
                                Push.APNsPushToSingle("", "该账号已在其他地方登录", tokens, PenetrateMsg);
                            }
                            else if ("1".Equals(device))
                            {
                                Push.PushMessageToSingle("该账号已在其他地方登录", "", tokens);
                            }
                            else
                            {
                                resultUser.msg = "当前登录的是其他设备";
                                return Json(resultUser);
                            }
                        }
                    }
                    else if (password.Equals(user.PassWord))
                    {
                        if (value)
                        {
                            resultUser.res = 205;
                            resultUser.state = 1;
                            resultUser.msg = "由于您未设置迷惑密码系统帮您设置了迷惑密码为1111";                           
                            resultUser.data = JwtHelper.CreateToken(user, time);
                            ub.UpdateUser(user.ID, time);
                            //用redis保存用户登录的信息
                            redis.StringSet(user.LoginID, user);
                        }
                        int res = CheckAgent();
                        var datas = new
                        {
                            device = res,
                            token = token
                        };
                        redis.StringSet(user.ID.ToString(), datas);
                        resultUser.res = 200;
                        resultUser.state = 1;
                        resultUser.msg = "用户是登录的私密聊天";                        
                        resultUser.data = JwtHelper.CreateToken(user, time);
                        ub.UpdateUser(user.ID, time);
                        //用redis保存用户登录的信息
                        redis.StringSet(user.LoginID,user);
                    }
                    else if (password.Equals(user.PassWords))
                    {
                        resultUser.res = 200;
                        resultUser.state = 2;
                        resultUser.msg = "用户是登录的工作聊天";
                        ub.UpdateUser(user.ID, time);
                        resultUser.data = JwtHelper.CreateToken(user, time);
                    }
                    else
                    {
                        resultUser.res = 500;
                        resultUser.msg = "密码错误，请重新输入";

                    };
                }
            }
            catch (HttpException ex)
            {
                LogHelper.WriteLog(ex.Message.ToString(), ex);
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
                string token = obj["token"].ToString();
                if (string.IsNullOrEmpty(token))
                {
                    resultUser.msg = "注册失败";
                    return Json(resultUser);
                }
                if (ivs!=null)
                {
                    DateTime time = DateTime.Now;
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
                    user.ChatSwitch = false;
                    user.LastLoginAt = time;
                    if (ub.CreateUser(user))
                    {
                        var data= ub.GetUserName(loginid);
                        int res=CheckAgent();
                        var datas = new
                        {
                            device = res,
                            token=token
                        };
                        redis.StringSet(data.ID.ToString(), datas);
                        ib.DeleteInvitation(InviteCode);
                        resultUser.res = 200;
                        resultUser.msg = "注册成功"; 
                        resultUser.data = JwtHelper.CreateToken(user,time);
                        return Json(resultUser);
                    }
                }                  

            }
            catch (HttpException ex)
            {
                LogHelper.WriteLog(ex.Message.ToString(),ex);
            }
            return Json(resultUser);
        }
        /// <summary>
        /// 1代表当前登录用户是安卓设备，2代表是苹果设备,0代表其他设备 
        /// </summary>
        /// <returns></returns>
        public int CheckAgent()
        {
            string Agent = Request.UserAgent;
            string[] keywords = { "Android", "iPhone" };
            if (Agent.Contains("Android"))
            {
                return 1;
            }
            if (Agent.Contains("iPhone"))
            {
                return 2;
            }
            return 0;
        }
    }
}