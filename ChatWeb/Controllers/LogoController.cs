using BLL;
using Common;
using Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace ChatWeb.Controllers
{
    public class LogoController : Controller
    {
        LogoBLL lb=new LogoBLL();
        // GET: Logo
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 获取所有的Logo
        /// </summary>
        public JsonResult GetLogo()
        {
            List<Logo> list=lb.GetAll();
            List<string> Name = new List<string>();
            List<string> Icon = new List<string>();
            foreach (var temp in list)
            {
                Name.Add(temp.Name);
                Icon.Add(Constant.files+temp.ICO);
            }
            var data = new
            {
                Name = Name,
                ICO = Icon
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}