using BLL;
using Common;
using Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace ChatWeb.Controllers
{
    public class BaseController : Controller
    {

        #region 字段
        /// <summary>
        /// 成员信息
        /// </summary>
        public User us;
        /// <summary>
        /// 参数列表
        /// </summary>
        public JObject param;
        /// <summary>
        /// 返回信息实体
        /// </summary>
        public  ResultData resultData =new ResultData();
        #endregion
        [HttpPost]
        public virtual JsonResult RequestUser()
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
                if (us == null || authInfo.LoginID != us.LoginID )
                {
                    throw new HttpException("身份验证失败,请重新登录");
                };
                if (us.LastLoginAt>authInfo.Iat)
                {
                    throw new HttpException("身份验证已过期，请重新登录");
                };
                using (StreamReader stream = new StreamReader(Request.InputStream))
                {
                    string json = stream.ReadToEnd();
                    if (!string.IsNullOrEmpty(json))
                    {
                        this.param = JObject.Parse(json);
                    }
                }
            }
            catch (HttpException ex)
            {
                this.resultData.msg = ex.Message;
                return Json(resultData);
            }
            resultData.res = 200;
            return Json(resultData);
        }
        /// <summary>
        /// 获取httprequest的值
        /// </summary>
        /// <param name="key">参数名</param>
        /// <returns>参数值，如果不存在返回空字符串</returns>
        public string GetParams(string key)
        {
            string msg = string.Empty;
            if(string.IsNullOrEmpty(key))
            {
                return msg;
            }
            else
            {
                if (param[key]!=null)
                {
                    return param[key].ToString();
                }
                else
                {
                    return null;
                }
            }
        }
    }
}