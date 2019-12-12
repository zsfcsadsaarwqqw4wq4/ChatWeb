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
        RoleBLL rb = new RoleBLL();
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
        public JsonResult SetAgent()
        {
            RequestUser();
            if (resultData.res == 500)
            {
                return Json(resultData);
            }
            int uid = us.ID;
            AgentModel agentmodel=new AgentModel();
            agentmodel=ab.GetAgentModel(uid);
            if (agentmodel != null)
            {
                resultdata.res = 200;
                resultdata.msg = "查询成功";
                resultdata.data = agentmodel;
            }
            return Json(resultdata);
        }        
    }
}