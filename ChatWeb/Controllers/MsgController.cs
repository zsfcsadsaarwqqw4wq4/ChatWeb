using BLL;
using Common;
using Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using static Model.ExModel;

namespace ChatWeb.Controllers
{
    public class MsgController : BaseController
    {
        ResultData resultdata = new ResultData();
        MessagesBLL mb = new MessagesBLL();
        // GET: Msg
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 查询当前用户聊天记录并分页
        /// </summary>
        /// <returns></returns>
        public JsonResult PageMessages()
        {
            RequestUser();
            if (resultData.res == 500)
            {
                return Json(resultData);
            }
            int pageindex = int.Parse(GetParams("pageindex"));
            int pagesize = int.Parse(GetParams("pagesize"));
            int fromuserid = us.ID;            
            List<Messages> list = mb.PageMessages(pageindex, pagesize, fromuserid);            
            int total = mb.PageCount(fromuserid);
            return Json(new { });
        }
        /// <summary>
        /// 获取当前用的历史聊天记录,七天以前
        /// </summary>
        /// <returns></returns>
        public JsonResult HistoryMsg()
        {
            RequestUser();
            if (resultData.res == 500)
            {
                return Json(resultData);
            }
            int userid = us.ID;            
            DateTime starttime = DateTime.Now.AddDays(-7);
            #region 编码
            //string encode = string.Empty;
            //byte[] bytes= Encoding.UTF8.GetBytes(mssages);
            //encode = Convert.ToBase64String(bytes);
            #endregion
            #region 解码
            //string decode = string.Empty;
            //byte[] bytess=Convert.FromBase64String(mssages);            
            //Encoding.UTF8.GetString(bytess);
            #endregion
            List<IGrouping<int, UserMessage>> listmsg = mb.GetMessage(userid, starttime);
            if (listmsg.Count > 0)
            {
                foreach (var temp in listmsg)
                {
                    var key = temp.Key;
                    var value = temp.ToList();
                }
            }
            //List<UserMessage> listmsg=mb.GetMessage(userid,starttime);
            //List<object> result = new List<object>();
            //if (listmsg.Count > 0)
            //{
            //    foreach (var temp in listmsg)
            //    {
            //        string Time = temp.Time.ToString("yyyy-MM-dd HH:mm:ss");
            //        temp.FromUser.HeadPortrait = Constant.files + temp.FromUser.HeadPortrait;
            //        temp.ToUser.HeadPortrait = Constant.files + temp.ToUser.HeadPortrait;
            //        var data = new
            //        {
            //            ID = temp.ID,
            //            PostMessages = temp.PostMessages,
            //            ReciveStatus = temp.ReciveStatus,
            //            Time = Time,
            //            MessagesTypeID = temp.MessagesTypeID,
            //            FromUser = temp.FromUser,
            //            ToUser = temp.ToUser,
            //            Status = temp.Status,
            //            GUID = temp.GUID
            //        };
            //        result.Add(data);
            //    }
            //    resultData.res = 200;
            //    resultData.msg = "已找到当前用户的消息记录";
            //    resultData.data = result;
            //}
            else
            {
                resultData.msg = "当前用户没有聊天记录";
            }
            return Json(resultData);
        }       
    }
}