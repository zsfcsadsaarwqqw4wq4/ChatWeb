using BLL;
using ChatWeb.App_Start;
using Common;
using Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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
        ThemeBLL tb = new ThemeBLL();
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
        /// 发送消息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SendMsg()
        {
            bool flags = false;
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
            dynamic mock = JsonConvert.DeserializeObject(GetParams("mock").ToString());
            var content = mock.secondary.content;
            var title = mock.first.appBarTitle + "吧";
            users = ub.GetUserById(uid);
            int status = 0;
            object flasedata = null;
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
                loginid=loginid,
                headportrait=Constant.files+us.HeadPortrait,
                uid =uid,
                msg=msg
            };
            var datas = new
            {
                title=title,
                content =content

            };
            if (users.ChatSwitch)
            {
                flasedata = datas;
                Chat.SendMsgToUser(userid, loginid, uid, msg, guid, messagestypeid, isBART, data, flasedata, flags);
            }
            else
            {
                if (us.BurnAfterReading)
                {
                    flags = true;
                    Chat.SendMsgToUser(userid, loginid, uid, msg, guid, messagestypeid, isBART, data, flasedata, flags);
                }
                else
                {
                    Chat.SendMsgToUser(userid, loginid, uid, msg, guid, messagestypeid, isBART, data, flasedata, flags);
                }
            }
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
                string imgbase64 = Request["base64"].ToString();
                if (!string.IsNullOrEmpty(imgbase64))
                {

                }
                Chat.SendPhotoToUser(us.ID, us.LoginID, uid, imgbase64, messagestypeid, guid);
            }
            catch (HttpException ex)
            {
                LogHelper.WriteLog(ex.Message,ex);
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
            string path = string.Empty;
            string paths = string.Empty;
            string img = string.Empty;
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
                string imgbase64 = Request["base64"].ToString();
                bool isBART = us.BurnAfterReading;
                dynamic mock = JsonConvert.DeserializeObject(Request["mock"].ToString());
                var content = mock.secondary.content;
                var title = mock.first.appBarTitle + "吧";
                object flasedata = null;
                int width = Convert.ToInt32(Request["width"].ToString());
                int height = Convert.ToInt32(Request["height"].ToString());
                users = ub.GetUserById(uid);
                //获取图片后缀
                string type = imgbase64.Split(',')[0].Split(';')[0].Split('/')[1];
                string[] str = imgbase64.Split(',');
                byte[] srcBuf = Convert.FromBase64String(str[1]);
                var times = DateTime.Now.ToFileTime().ToString();
                path = Path.Combine(Server.MapPath(string.Format("~/{0}", "Images")), times + '.' + type);
                img = Constant.files + "/" + "Images" + "/" + times + "." + type;

                using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(srcBuf, 0, srcBuf.Length);
                    fs.Close();
                };
                ImageClass imageclass =new ImageClass(path);
                double Percent = 0.8;
                var timess = DateTime.Now.ToFileTime().ToString();
                string thumbImg= Constant.files + "/" + "Images" + "/" + timess + "." + type;
                paths = Path.Combine(Server.MapPath(string.Format("~/{0}", "Images")), timess + '.' + type);
                bool result=imageclass.GetReducedImage(Convert.ToInt32(width * Percent), Convert.ToInt32(height * Percent), paths);
                var datas = new
                {
                    title = title,
                    content = content
                };
                if (users.ChatSwitch)
                {
                    flasedata = datas;
                    if (result)
                    {
                        Chat.SendPhotoToUser(us.ID, us.LoginID, uid, img, messagestypeid, guid, mock, isBART, width, height, flasedata, thumbImg);
                    }
                    else
                    {
                        Chat.SendPhotoToUser(us.ID, us.LoginID, uid, img, messagestypeid, guid, mock, isBART, width, height, flasedata, thumbImg);
                    }
                }
                else
                {
                    if (result)
                    {
                        Chat.SendPhotoToUser(us.ID, us.LoginID, uid, img, messagestypeid, guid, mock, isBART, width, height, flasedata, thumbImg);
                    }
                    else
                    {
                        Chat.SendPhotoToUser(us.ID, us.LoginID, uid, img, messagestypeid, guid, mock, isBART, width, height, flasedata, thumbImg);
                    }
                }
                resultData.res = 200;
                resultData.msg = "发送图片成功";
                resultData.data = img;
                return Json(resultData);
            }
            catch
            {
                resultData.msg = "发送图片失败";
                return Json(resultData);
            }
        }
        public JsonResult NewSendPhoto()
        {
            string path= string.Empty;
            string img = string.Empty;
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
                string imgbase64=Request["base64"].ToString();
                bool isBART = us.BurnAfterReading;
                dynamic mock =JsonConvert.DeserializeObject(Request["mock"].ToString());
                var content = mock.secondary.content;
                var title = mock.first.appBarTitle + "吧";
                object flasedata = null;
                int width =Convert.ToInt32(Request["width"].ToString());
                int height = Convert.ToInt32(Request["height"].ToString());
                users = ub.GetUserById(uid);
                //获取图片后缀
                string type = imgbase64.Split(',')[0].Split(';')[0].Split('/')[1];
                string[] str = imgbase64.Split(',');
                byte[] srcBuf= Convert.FromBase64String(str[1]);
                var times = DateTime.Now.ToFileTime().ToString();
                path = Path.Combine(Server.MapPath(string.Format("~/{0}", "Images")), times + '.'+type);
                img = Constant.files + "/" + "Images" +"/"+ times + "." + type;
                using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(srcBuf, 0, srcBuf.Length);
                    fs.Close();
                };
                var datas = new
                {
                    title = title,
                    content = content

                };
                if (users.ChatSwitch)
                {
                    flasedata = datas;
                    Chat.SendPhotoToUser(us.ID, us.LoginID, uid, img, messagestypeid, guid, mock, isBART, width, height, flasedata);
                }
                else
                {
                    Chat.SendPhotoToUser(us.ID, us.LoginID, uid, img, messagestypeid, guid, mock, isBART, width, height, flasedata);
                }
                resultData.res = 200;
                resultData.msg = "发送图片成功";
                resultData.data = img;
                return Json(resultData);
            }
            catch
            {
                resultData.msg = "发送图片失败";
                return Json(resultData);
            }
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
                    else if (!string.IsNullOrWhiteSpace(list[0].Keys.First()) && !string.IsNullOrWhiteSpace(list[0].Values.First()) && !string.IsNullOrWhiteSpace(list[1].Keys.First()) && !string.IsNullOrWhiteSpace(list[1].Values.First()))
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
            string ID = us.ID.ToString();
            bool data=redis.DeleteString(ID);
            if (!data)
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
                if (string.IsNullOrEmpty(pw))
                {
                    us.ChatSwitch = true;
                    ub.EditUser(us);
                    return Convert.ToInt32(PassWord.two);
                }
                else if (MD5Helper.MD5Encrypt32(pw).Equals(us.PassWord))
                {
                    return Convert.ToInt32(PassWord.One);
                }
                else 
                {
                    us.ChatSwitch = true;
                    ub.EditUser(us);
                    return Convert.ToInt32(PassWord.two);
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
        /// 测试推送方法
        /// </summary>
        public void Pushs()
        {
            string cid = "7b0eb9a4a5e5dbae8d3fda5b4c580ab3";
            //var datas = new
            //{
            //    title= "该账号已在其他地方登录"
            //};
            var datass = new
            {
                title = "该账号已在其他地方登录"
            };
            var PenetrateMsg = new
            {
                type = 0,
                data = datass
            };
            Push.IosPushMessageToSingle(JsonConvert.SerializeObject(PenetrateMsg), cid);
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
        public void SendMsgs()
        {
            int res = int.Parse(Request["res"]);
            int userid = int.Parse(Request["userid"]);
            string loginid = Request["loginid"];
            int uid = int.Parse(Request["uid"]);
            int messagestypeid = int.Parse(Request["messagestypeid"]);
            bool isBART = bool.Parse(Request["isBART"]);
            var data = new
            {
                userid = userid,
                loginid = loginid,
                uid = uid,
            };
            var datas = new
            {
                title = "标题",
                content = "内容"
            };
            for (int i= 0;i<res;i++)
            {
                Chat.SendMsgToUser(userid, loginid, uid, EncodeBase64(i.ToString()), i.ToString(), messagestypeid, isBART, data, datas, false);
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
        /// 获取压缩后的文件流 
        /// </summary>
        /// <returns></returns>
        public Stream ImgCompression()
        {
            var file = Request.Files[0];
            double compressionRatio = 1024 * 1024 / Convert.ToDouble(file.ContentLength);
            compressionRatio = Math.Round(compressionRatio, 2);
            byte[] filebyte = new byte[file.ContentLength];
            file.InputStream.Read(filebyte, 0, file.ContentLength);
            //上传文件的byte数组转为Stream
            MemoryStream ms = new MemoryStream(filebyte);
            Image img = Image.FromStream(ms);
            //按比例计算新的宽高
            int toWidth = Convert.ToInt32(img.Width * compressionRatio);
            int toHeight = Convert.ToInt32(img.Height * compressionRatio);
            //按照新的宽高用画布重新画一张
            Bitmap bitmap = new Bitmap(toWidth, toHeight);
            Graphics g = Graphics.FromImage(bitmap);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.Clear(System.Drawing.Color.Transparent);
            g.DrawImage(img, new System.Drawing.Rectangle(0, 0, toWidth, toHeight), new System.Drawing.Rectangle(0, 0, img.Width, img.Height), System.Drawing.GraphicsUnit.Pixel);
            //将画好的bitmap转成stream（不一定费时stream，byte数组什么都可以）
            var fileStream = new MemoryStream();
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Jpeg);
                byte[] data = new byte[stream.Length];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(data, 0, Convert.ToInt32(stream.Length));
                fileStream = new MemoryStream(data);
            }
            return fileStream;
        }
        public JsonResult GetAll()
        {
            List<string> imgs = new List<string>();
            for (int i = 9; i < 28; i++)
            {
                string url = Constant.files + "/Images/head/" + i + ".jpg";
                imgs.Add(url);
            };
            List<IGrouping<int, ThemeModel>> Themes = tb.GetAll();
            Dictionary<int, List<ThemeModel>> dic = new Dictionary<int, List<ThemeModel>>();
            List<ThemeModel> listtm = new List<ThemeModel>();
            foreach (var temp in Themes)
            {
                int key = int.Parse(temp.Key.ToString());
                listtm = temp.ToList();
                foreach (var item in listtm)
                {
                    item.ThemeImage = Constant.files + item.ThemeImage;
                }
                dic.Add(key, listtm);
            }
            var temps = new
            {
                type = ThemeEnum.System,
                Name = EnumHelper.GetEnumDescription(ThemeEnum.System),
                data = dic.ToArray()
            };
            List<object> result = new List<object>();
            result.Add(temps);
            var datas = new
            {
                avatars= imgs,
                templates= result.ToArray()
            };
            return Json(datas,JsonRequestBehavior.AllowGet);
        }
    }
}