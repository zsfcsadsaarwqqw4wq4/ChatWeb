using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChatWeb.Controllers
{
    public class AgentPercentController : Controller
    {
        // GET: AgentPercent
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 设置用户代理分红接口
        /// </summary>
        public void SetAgentPercent(int uid)
        {
            
        }
    }
}