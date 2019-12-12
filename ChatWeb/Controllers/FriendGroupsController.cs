using BLL;
using Common;
using Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Model.ExModel;

namespace ChatWeb.Controllers
{
    public class FriendGroupsController : BaseController
    {
        List<IGrouping<string, Friend_Groups>> Infos;
        List<Friend_Groups> listfg = new List<Friend_Groups>();
        FriendGroupsBLL fgb = new FriendGroupsBLL();
        ResultData resultdata = new ResultData();
        // GET: FriendGroups
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 查询所有分组信息
        /// </summary>
        /// <returns></returns>
        //public JsonResult GetFriendGroupsAll()
        //{
        //    RequestUser();
        //    List<FriendGroups> list = fgb.GetAll();
        //    if (list != null)
        //    {
        //        resultdata.res = 200;
        //        resultdata.msg = "成功找到好友分组信息";
        //        resultdata.data = list;
        //    }
        //    return Json(resultdata);
        //}       
        /// <summary>
        /// 获取当前登录用户的好友分组
        /// </summary>
        /// <returns></returns>
        public JsonResult GetFriendGroupsMsg()
        {
            RequestUser();
            int userid = us.ID;
            Infos=fgb.GetFriendsGroup(userid);
            Dictionary<string, List<Friend_Groups>> dic = new Dictionary<string, List<Friend_Groups>>();
            foreach (var temp in Infos)
            {
                string key = temp.Key.ToString();
                listfg=temp.ToList();
                foreach (var value in listfg)
                {
                    value.Firend.HeadPortrait = Constant.files + value.Firend.HeadPortrait;
                    value.User.HeadPortrait = Constant.files + value.User.HeadPortrait;
                }
                dic.Add(key, listfg);               
            };       
            if (Infos != null)
            {
                resultdata.res = 200;
                resultdata.msg = "成功找到当前好友分组信息";
                resultdata.data = dic;
            }
            return Json(resultdata);
        }
    }
}