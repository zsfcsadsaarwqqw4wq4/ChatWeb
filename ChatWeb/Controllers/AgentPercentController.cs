using BLL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChatWeb.Controllers
{
    public class AgentPercentController : BaseController
    {
        AgentBLL ab=new AgentBLL();
        AgentPowerBLL apb = new AgentPowerBLL();
        AgentPercentBLL agb =new AgentPercentBLL();
        // GET: AgentPercent
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 查询当前用户分红比例，如果没有分红比例代表该用户没有分红的权利
        /// </summary>
        public JsonResult SetAgentPercent(int userid)
        {
            int res= ab.QueryParentAgent(userid);
            if (res != 1)
            {
                Agent agent = ab.GetAgent(res);
                var ap = apb.Get(agent.UserID);
                if (ap.BChildChildMoney)
                {

                }
            }
            else
            {
                resultData.res = 200;
                resultData.msg = "当前用户是最高等级可以自己设置";
                resultData.data = null;
                return Json(resultData);
            }
            return null;
        }
        /// <summary>
        /// 获取到所有的代理分红记录
        /// </summary>
        public JsonResult GetAll()
        {
            RequestUser();
            if (resultData.res == 500)
            {
                return Json(resultData);
            }
            return Json(new { });
        }
    }
}