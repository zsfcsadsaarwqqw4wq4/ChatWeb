using BLL;
using Common;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using static Model.ExModel;

namespace ChatWeb.Controllers
{
    public class AgentController : BaseController
    {
        AgentBLL ab=new AgentBLL();
        ResultData resultdata = new ResultData();
        // GET: Agent
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 获取当前登录用户的代理关系
        /// </summary>
        /// <returns></returns>
        public JsonResult GetAgent()
        {
            RequestUser();
            if (resultData.res == 500)
            {
                return Json(resultData);
            }
            int uid = us.ID;    
            Agent agent=ab.GetAgent(uid);
            if(agent.Level!=0)
            {
            };
            if (agent != null)
            {
                resultdata.res = 200;
                resultdata.msg = "查询成功";
                resultdata.data = agent;
            }
            return Json(resultdata);
        }   
        /// <summary>
        /// 设置用户代理信息
        /// </summary>
        public JsonResult GetAgents()
        {
            int userid=int.Parse(Request["userid"].ToString());
            AgentModel agentmodel=new AgentModel();
            agentmodel=ab.GetAgentModel(userid);        
            if (agentmodel != null)
            {
                resultdata.res = 200;
                resultdata.msg = "查询成功";
                resultdata.data = agentmodel;
            }
            return Json(resultdata,JsonRequestBehavior.AllowGet);
        }  
        /// <summary>
        /// 代理统计数量
        /// </summary>
        /// <returns></returns>
        public JsonResult AgentCount()
        {
            int userid = int.Parse(Request["userid"].ToString());
            var result = ab.AgentCount(userid);
            if (result != null)
            {
                resultdata.res = 200;
                resultdata.msg = "查询成功";
                resultdata.data = result;
            }
            return Json(resultdata, JsonRequestBehavior.AllowGet);
        }
    }
}