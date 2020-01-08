using BLL;
using ChatWeb.App_Start;
using Common;
using Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using static Common.EnumHelper;
using static Model.ExModel;

namespace ChatWeb.Controllers
{
    public class DefaultController :BaseController
    {
        MessagesBLL mb = new MessagesBLL();
        InvitationBLL ib = new InvitationBLL();
        UserBLL ub=new UserBLL();
        ResultData resultdata = new ResultData();
        User users = new User();
        Redis redis = new Redis();
        /// <summary>
        /// 参数
        /// </summary>
        private JObject obj;
        /// <summary>
        /// 用户登录后获取当前用户的信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Index()
        {
            RequestUser();
            if (resultData.res == 500)
            {
                return Json(resultData);
            };
            var data = new
            {
                FirstProblem = us.FirstProblem,
                SecondProblem = us.SecondProblem
            };
            List<string> imgs = new List<string>();
            for (int i=9;i<28;i++)
            {
                string url = Constant.files+ "/Images/head/" + i + ".jpg";
                imgs.Add(url);
            };
            var temp = new
            {
                ID = us.ID,
                LoginID = us.LoginID,               
                HeadPortrait = Constant.files + us.HeadPortrait,
                PSearchState = us.PSearchState,
                USearchState=us.USearchState,
                BurnAfterReading=us.BurnAfterReading,
                ChatTimeLimit=us.ChatTimeLimit,
                PassWord=us.PassWord,
                Passwords=us.PassWords,
                ThemeTypeID=us.ThemeTypeID,
                IsEnterSendMsg=us.IsEnterSendMsg,
                SecondThemeTypeID=us.SecondThemeTypeID,
                ChatSwitch=us.ChatSwitch,
                Problem =data,
                AvatarSet = imgs
            };
            resultdata.res = 200;
            resultdata.msg = "身份验证成功";
            resultdata.data = temp;
            return Json(resultdata);
        }
        /// <summary>
        /// 1代表当前登录用户是安卓设备，2代表是苹果设备,0代表既不是苹果也不是安卓
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
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SendMsg()
        {
            RequestUser();
            if (resultData.res == 500)
            {
                return Json(resultData);
            }
            int userid = us.ID;
            string loginid = us.LoginID;
            int uid = int.Parse(GetParams("uid"));
            string msg = GetParams("msg");
            DateTime time = DateTime.Now;            
            int messagestypeid=int.Parse(GetParams("messagestypeid"));
            bool isBART = us.BurnAfterReading;
            string guid = GetParams("guid");
            int status = 0;
            bool recivestatus = true;
            Messages msgs=new Messages();
            msgs.FromUserID = userid;
            msgs.ToUserID = uid;
            msgs.Status = status;
            msgs.PostMessages = msg;
            msgs.ReciveStatus = recivestatus;
            msgs.Time = time;
            msgs.MessagesTypeID = messagestypeid;
            msgs.GUID = guid;
            bool flag= mb.CreateMessages(msgs);          
            if (flag)
            {
                resultdata.res = 200;
                resultdata.msg = "发送成功";
            }
            var data = new
            {
                userid= userid,
                uid=uid,
                msg=msg
            };
            Chat.SendMsgToUser(userid, loginid, uid, msg, guid, messagestypeid, isBART,data);
            return Json(resultdata);
        }
        /// <summary>
        /// 通知发送者消息已读
        /// </summary>
        /// <returns></returns>
        public JsonResult ReceiveMsg()
        {
            RequestUser();
            if (resultData.res == 500)
            {
                return Json(resultData);
            }
            int userid = us.ID;
            int uid = int.Parse(GetParams("uid"));
            string guid = GetParams("guid");
            int messagestypeid = 3;
            Chat.SendMsg(userid, uid, guid, messagestypeid);
            return Json(new { });
        }       

        /// <summary>
        /// 发送图片
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SendPhoto()
        {
            try
            {
                string token = Request.Headers["token"];
                if (string.IsNullOrEmpty(token))
                {
                    throw new HttpException("身份验证失败");
                }
                AuthInfo authInfo = JwtHelper.GetJwtDecode(token);
                //判断token正确性
                if (authInfo == null)
                {
                    throw new HttpException("身份验证失败");
                }
                //判断身份是否过期
                DateTime dt = Convert.ToDateTime(authInfo.Exp);
                if (dt < DateTime.Now)
                {
                    throw new HttpException("身份过期,请重新登录");
                }
                UserBLL ud = new UserBLL();
                this.us = ud.GetUserById(authInfo.ID);
                //验证身份信息是否正确
                if (us == null || authInfo.LoginID != us.LoginID)
                {
                    throw new HttpException("身份验证失败,请重新登录");
                };
                int messagestypeid = 2;
                int uid = int.Parse(Request["uid"].ToString());
                string guid = Request["guid"].ToString();
                string imgbase64 = Request["img"].ToString();            
                //imgbase64=imgbase64.Replace("%","").Replace(" ","+").Replace(",","");
                Chat.SendPhotoToUser(us.ID, us.LoginID, uid, imgbase64, messagestypeid, guid);
            }
            catch (HttpException ex)
            {
                this.resultData.msg = ex.Message;
                return Json(resultData);
            }
            resultData.res = 200;
            resultData.msg = "发送图片成功";
            return Json(resultData);
        }
        /// <summary>
        /// 发送图片
        /// </summary>
        public JsonResult SendPhotos()
        {
            try
            {
                string token = Request.Headers["token"];
                if (string.IsNullOrEmpty(token))
                {
                    throw new HttpException("身份验证失败");
                }
                AuthInfo authInfo = JwtHelper.GetJwtDecode(token);
                //判断token正确性
                if (authInfo == null)
                {
                    throw new HttpException("身份验证失败");
                }
                //判断身份是否过期
                DateTime dt = Convert.ToDateTime(authInfo.Exp);
                if (dt < DateTime.Now)
                {
                    throw new HttpException("身份过期,请重新登录");
                }
                UserBLL ud = new UserBLL();
                this.us = ud.GetUserById(authInfo.ID);
                //验证身份信息是否正确
                if (us == null || authInfo.LoginID != us.LoginID)
                {
                    throw new HttpException("身份验证失败,请重新登录");
                };
                int messagestypeid = 2;
                int uid = int.Parse(Request["uid"].ToString());
                string guid = Request["guid"].ToString();
                string width =Request["width"].ToString();
                string height = Request["height"].ToString();
                var res = Request.Files[0];
                var filetype = res.ContentType.Split('/')[0];
                if ("image".Equals(filetype))
                {
                    string ext = res.FileName.Split('.')[1];
                    Stream filestream = res.InputStream;
                    var bytes = SwitchDataTypecs.StreamToBytes(filestream);
                    //var image=SwitchDataTypecs.BytesToBitmap(bytes);
                    string img = Convert.ToBase64String(bytes);
                    string data = "data:" + res.ContentType;
                    string base64 = ";base64,";
                    img = data + base64 + img;
                    Chat.SendPhotoToUsers(us.ID, us.LoginID, uid, img, messagestypeid, guid, ext, width, height);
                }
                else
                {
                    resultData.msg = "上传的不是图片";
                    return Json(resultData);
                }
            }
            catch (HttpException ex)
            {
                this.resultData.msg = ex.Message;
                return Json(resultData);
            }
            resultData.res = 200;
            resultData.msg = "发送图片成功";
            return Json(resultData);
        }
        /// <summary>
        /// 将图片数据转换为Base64字符串
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ToBase64()
        {
            //var base64Img = Convert.ToBase64String(System.IO.File.ReadAllBytes("图片路径"));
            var file=new FileInfo(Request.Files[0].FileName);
            var s=file.Extension;
        }
        /// <summary>
        /// 用户生成邀请码
        /// </summary>
        /// <returns></returns>
        public int CreaeteInvitation()
        {
            RequestUser();
            int userid = us.ID;
            int s=ib.GetUserInvitation(userid);
            if (s != 0)
            {
                return s;
            }
            List<int> list = new List<int>();
            for (int j=1000;j<9999;j++)
            {
                list.Add(j);
            }
            List<Invitation> listiv=ib.GetAll();
            foreach (var temp in listiv)
            {
                list.Remove(temp.InviteCode);
            }
            Random rm = new Random();
            int i = rm.Next(list.Count); //随机数最大值不能超过list的总数
            DateTime endtime=DateTime.Now.AddDays(7);
            int result=list[i];
            Invitation invitation=new Invitation();
            invitation.UID = userid;
            invitation.InviteCode = result;
            invitation.EndTime = endtime;
            bool flag= ib.CreaeteInvitation(invitation);
            return result;
        }
        /// <summary>
        /// 设置用户的迷惑密码，和安全问题
        /// </summary>
        /// <returns></returns>
        public JsonResult SetConfusingPwd()
        {
            RequestUser();
            if (resultData.res == 500)
            {
                return Json(resultData);
            }
            string passwords = GetParams("cpwd");
            passwords = MD5Helper.MD5Encrypt32(passwords);
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            list = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(GetParams("ques"));
            if (!string.IsNullOrWhiteSpace(list[0].Keys.First()) || !string.IsNullOrWhiteSpace(list[0].Values.First()) || !string.IsNullOrWhiteSpace(list[1].Keys.First()) || !string.IsNullOrWhiteSpace(list[1].Values.First()))
            {
                try
                {
                    us.PassWords = passwords;
                    if (!string.IsNullOrWhiteSpace(list[0].Keys.First()) && !string.IsNullOrWhiteSpace(list[0].Values.First()))
                    {
                        string firstproblem = list[0].Keys.First();
                        string firstanswer = list[0].Values.First();
                        us.FirstProblem = firstproblem;
                        us.FirstAnswer = firstanswer;
                    }
                    else if (!string.IsNullOrWhiteSpace(list[1].Keys.First()) && !string.IsNullOrWhiteSpace(list[1].Values.First()))
                    {
                        string secondproblem = list[1].Keys.First();
                        string secondanswer = list[1].Values.First();
                        us.SecondProblem = secondproblem;
                        us.SecondAnswer = secondanswer;
                    }
                    else if(!string.IsNullOrWhiteSpace(list[0].Keys.First()) && !string.IsNullOrWhiteSpace(list[0].Values.First()) && !string.IsNullOrWhiteSpace(list[1].Keys.First()) && !string.IsNullOrWhiteSpace(list[1].Values.First()))
                    {
                        string firstproblem = list[0].Keys.First();
                        string firstanswer = list[0].Values.First();
                        string secondproblem = list[1].Keys.First();
                        string secondanswer = list[1].Values.First();
                        us.FirstProblem = firstproblem;
                        us.FirstAnswer = firstanswer;
                        us.SecondProblem = secondproblem;
                        us.SecondAnswer = secondanswer;
                    }
                    else
                    {
                        resultdata.msg = "不能只填答案或问题";
                        return Json(resultdata);
                    };
                    if (ub.EditUser(us))
                    {
                        resultdata.res = 200;
                        resultdata.msg = "设置成功";
                    }
                    else
                    {
                        resultdata.msg = "设置失败";
                    }
                    return Json(resultdata);
                }
                catch 
                {
                    string message = "数据格式异常请检查";
                    resultdata.msg = message;
                    return Json(resultdata);
                }                
            }
            else
            {
                us.PassWords = passwords;
                if (ub.EditUser(us))
                {
                    resultdata.res = 200;
                    resultdata.msg = "设置成功";
                }
                else
                {
                    resultdata.msg = "设置失败";
                }
                return Json(resultdata);
            }
        }
        /// <summary>
        /// 设置阅后即焚状态
        /// </summary>
        /// <returns></returns>
        public JsonResult SetBAR()
        {
            RequestUser();
            if (resultData.res == 500)
            {
                return Json(resultData);
            }
            bool barState = bool.Parse(GetParams("isbart"));            
            us.BurnAfterReading = barState;           
            if (!ub.EditUser(us))
            {
                resultdata.msg = "设置失败";
                return Json(resultdata);    
            }
            else
            {
                resultdata.res = 200;
            }
            return Json(resultdata);
        }
        /// <summary>
        /// 设置聊天失效
        /// </summary>
        /// <returns></returns>
        public JsonResult ChatTimeLimit()
        {
            RequestUser();
            if (resultData.res == 500)
            {
                return Json(resultData);
            }
            string chattimelimit = GetParams("chattimelimit");
            us.ChatTimeLimit = chattimelimit;
            if (!ub.EditUser(us))
            {
                resultdata.msg = "设置失败";
                return Json(resultdata);
            }
            else
            {
                resultdata.res = 200;
            }
            return Json(resultdata);
        }
        /// <summary>
        /// 设置是换行还是发送消息
        /// </summary>
        /// <returns></returns>
        public JsonResult SetIsEnterSendMsg()
        {
            RequestUser();
            if (resultData.res == 500)
            {
                return Json(resultData);
            }
            bool isentersendmsg =bool.Parse(GetParams("isentersendmsg"));
            us.IsEnterSendMsg = isentersendmsg;
            if (!ub.EditUser(us))
            {
                resultdata.msg = "设置失败";
                return Json(resultdata);
            }
            else
            {
                resultdata.res = 200;
                resultdata.msg = "设置成功";
            }
            return Json(resultdata);
        }
        /// <summary>
        ///  获取当前用户安全问题
        /// </summary>
        /// <returns></returns>
        public JsonResult GetProblem()
        {
            try
            {
                using (StreamReader sr = new StreamReader(Request.InputStream))
                {
                    string json = sr.ReadToEnd();
                    if (string.IsNullOrEmpty(json))
                    {
                        resultdata.msg = "没有获取到类型和账户名";
                        return Json(resultdata);
                    }
                    var res = JsonConvert.DeserializeObject(json);
                    obj = JObject.Parse(json);
                    int data = int.Parse(obj["type"].ToString());
                    if (data == 0)
                    {
                        resultdata.res = 200;
                        resultdata.msg = "客服电话";
                        resultdata.data = Constant.PhoneNumber;
                        return Json(resultdata);
                    }
                    if (data == 1)
                    {
                        string loginid = obj["loginid"].ToString();
                        users = ub.GetUserNames(loginid);
                        if (users!=null)
                        {
                            List<string> list = new List<string>();
                            if (!string.IsNullOrWhiteSpace(users.FirstProblem)&& string.IsNullOrWhiteSpace(users.SecondProblem))
                            {
                                list.Add(users.FirstProblem);
                            }
                            else if (!string.IsNullOrWhiteSpace(users.SecondProblem)&& string.IsNullOrWhiteSpace(users.FirstProblem))
                            {
                                list.Add(users.SecondProblem);
                            }
                            else if(!string.IsNullOrWhiteSpace(users.SecondProblem) && !string.IsNullOrWhiteSpace(users.FirstProblem))
                            {
                                list.Add(users.FirstProblem);
                                list.Add(users.SecondProblem);
                            }
                            else
                            {
                                resultdata.msg = "此用户未设置安全问题，请联系客服";
                                return Json(resultdata);
                            }
                            resultdata.res = 200;
                            resultdata.msg = "成功找到用户的安全问题";
                            resultdata.data = list.ToArray();
                            Session["User"] = users;
                            return Json(resultdata);
                        }
                        else
                        {
                            resultdata.msg = "该用户不存在";     
                            return Json(resultdata);
                        }
                    }
                }
            }
            catch(Exception ex) 
            {

            }
            return Json(new { });
        }
        /// <summary>
        /// 验证用户输入的安全问题答案是否正确
        /// </summary>
        public JsonResult ValidationAnswer()
        {
            try
            {
                using (StreamReader sr = new StreamReader(Request.InputStream))
                {                    
                    string json = sr.ReadToEnd();
                    if (string.IsNullOrEmpty(json))
                    {
                        resultdata.msg = "没有获取到类型和账户名";
                        return Json(resultdata);
                    }
                    List<string> datas = JsonConvert.DeserializeObject<List<string>>(json);                    
                    users = Session["User"] as User;
                    if (!users.FirstAnswer.Equals(datas[0]))
                    {
                        resultdata.msg = "答案错误，请检查";
                        return Json(resultdata);
                    }
                    if (!users.SecondAnswer.Equals(datas[1]))
                    {
                        resultdata.msg = "答案错误，请检查";
                        return Json(resultdata);
                    }
                    resultdata.res = 200;
                    resultdata.msg = "验证成功";
                    resultdata.data = "/Default/EditPassWord";
                    return Json(resultdata);
                }
            }
            catch(Exception ex)
            {

            }
            return Json(new { });
        }
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <returns></returns>
        public JsonResult EditPassWord()
        {
            try
            {
                using (StreamReader sr = new StreamReader(Request.InputStream))
                {
                    string json = sr.ReadToEnd();
                    if (string.IsNullOrEmpty(json))
                    {
                        resultdata.msg = "没有获取到类型和账户名";
                        return Json(resultdata);
                    }
                    obj = JObject.Parse(json);
                    users = Session["User"] as User;
                    string password =MD5Helper.MD5Encrypt32(obj["password"].ToString());
                    users.PassWord = password;
                    if (ub.EditUser(users))
                    {
                        resultdata.res = 200;
                        resultdata.msg = "密码修改成功,请登录";
                    }
                    else
                    {
                        resultdata.msg = "密码相同，请勿重复提交";
                    }
                    return Json(resultdata);
                }
            }
            catch
            {

            }
            return Json(new { });
        }
        /// <summary>
        /// 设置主题
        /// </summary>
        /// <returns></returns>
        public JsonResult SetTheme()
        {
            RequestUser();
            if (resultData.res == 500)
            {
                return Json(resultData);
            }
            int themetypeid = int.Parse(GetParams("themetypeid"));
            int secondthemetypeid = int.Parse(GetParams("secondthemetypeid"));
            us.ThemeTypeID = themetypeid;
            us.SecondThemeTypeID = secondthemetypeid;
            if (!ub.EditUser(us))
            {
                resultdata.msg = "设置失败";
                return Json(resultdata);
            }
            else
            {
                resultdata.res = 200;
                resultdata.msg = "设置成功";
            }
            return Json(resultdata);
        }
        /// <summary>
        /// 设置聊天模式
        /// </summary>
        public JsonResult SetChatSwitch()
        {
            RequestUser();
            if (resultData.res == 500)
            {
                return Json(resultData);
            }
            bool chatswitch=bool.Parse(GetParams("chatswitch"));
            us.ChatSwitch = chatswitch;
            if (!ub.EditUser(us))
            {
                resultdata.msg = "设置失败";
                return Json(resultdata);
            }
            else
            {
                resultdata.res = 200;
                resultdata.msg = "设置成功";
            }
            return Json(resultdata);
        }
        /// <summary>
        /// 退出登录
        /// </summary>
        /// <returns></returns>
        public JsonResult LogOut()
        {
            RequestUser();
            if (resultData.res == 500)
            {
                return Json(resultData);
            }
            bool result=redis.DeleteString(us.LoginID);
            if (!result)
            {
                resultData.msg = "退出失败";
                return Json(resultData);
            }
            resultData.res = 200;
            resultData.msg = "退出成功";
            return Json(resultData);
        }
        /// <summary>
        /// 判断用户输入的是哪种密码
        /// </summary>
        public int IsPassWord()
        {
            RequestUser();
            if (resultData.res==500)
            {
                return 500;
            }
            string pw= GetParams("passwords");
            if (us.ChatSwitch==true)
            {
                if (MD5Helper.MD5Encrypt32(pw).Equals(us.PassWords.ToUpper()))
                {
                    return Convert.ToInt32(PassWord.One);
                }
                else if (MD5Helper.MD5Encrypt32(pw).Equals(us.PassWord))
                {
                    us.ChatSwitch = false;
                    ub.EditUser(us);
                    return Convert.ToInt32(PassWord.three);
                }
                else
                {
                    return Convert.ToInt32(PassWord.four);
                }
            }
            else
            {
                if (MD5Helper.MD5Encrypt32(pw).Equals(us.PassWord))
                {
                    return Convert.ToInt32(PassWord.One);
                }
                else if(MD5Helper.MD5Encrypt32(pw).Equals(us.PassWords.ToUpper()))
                {
                    us.ChatSwitch = true;
                    ub.EditUser(us);
                    return Convert.ToInt32(PassWord.two);
                }
                else
                {
                    return Convert.ToInt32(PassWord.four);
                }
            }
        }
        /// <summary>
        /// 当用户登录后完善用户信息接口
        /// </summary>
        /// <returns></returns>
        public JsonResult AddUserMsg()
        {
            RequestUser();
            if (resultData.res==500)
            {
                return Json(resultData);
            }
            string nickename=GetParams("nickename");
            string signature=GetParams("signature");
            string sex = GetParams("sex");
            string birthday=GetParams("birthday");
            string name = GetParams("name");
            string email = GetParams("email");
            int age = int.Parse(GetParams("age"));         
;           int vip = int.Parse(GetParams("vip"));
            int level= int.Parse(GetParams("level"));
            if (!string.IsNullOrWhiteSpace(nickename))
            {
                HashSet<string> data = new HashSet<string>();
            }
            else
            {       
                string ss=Enum.GetName(typeof(Enum),EnumHelper.ThemeEnum.System);
            }
            return Json(new { });
        }
        /// <summary>
        /// 当用户退出app时，删除账号和设备关联
        /// </summary>
        public void RemoveUserIdAndToken()
        {
            RequestUser();
            string ID = us.ID.ToString();
            redis.DeleteString(ID);
        }
        /// <summary>
        /// 测试推送方法
        /// </summary>
        public void Push()
        {
            HashSet<string> hash = new HashSet<string>();
            string data = "190e35f7e02ab9a5b1b";
            hash.Add(data);
            List<Push> list = new List<Push>();
            string msg = "新消息";
            JPush.JPushByRegiserID(hash, msg, list);
            var ary= Encoding.UTF8.GetBytes(msg);
            string val= Encoding.UTF8.GetString(ary);
        }
        /// <summary>
        /// 测试
        /// </summary>
        public void SetString()
        {
            Redis redis=new Redis();
            string key = "loginid";
            string password = "admin";
            User users=new User();
            users.LoginID = "hehehe";
            users.Name = "小张";
            bool res=redis.StringSet<User>("user", users);
        }
        /// <summary>
        /// base64编码
        /// </summary>
        /// <param name="code_type"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public  string EncodeBase64(string code)
        {
            string encode = "";
            byte[] bytes = Encoding.Unicode.GetBytes(code);
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
        public  string DecodeBase64(string code_type, string code)
        {
            string decode = "";
            byte[] bytes = Convert.FromBase64String(code);
            try
            {
                decode = Encoding.GetEncoding(code_type).GetString(bytes);
            }
            catch
            {
                decode = code;
            }
            return decode;
        }
    }
}