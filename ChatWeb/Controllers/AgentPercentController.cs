using BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChatWeb.Controllers
{
    public class AgentPercentController : Controller
    {
        AgentBLL ab=new AgentBLL();
        //AgentPowerBLL apb=new AgentPowerBLL();
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
            int res= ab.QueryParentAgent(uid);
            if (res!=0)
            {

            }
            else
            {
                
            }
        }
    }
}