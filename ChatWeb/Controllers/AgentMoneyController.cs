using BLL;
using Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Model.ExModel;

namespace ChatWeb.Controllers
{
    public class AgentMoneyController : Controller
    {
        private JObject obj;
        AgentBLL ab = new AgentBLL();
        AgentPercentBLL apb = new AgentPercentBLL();
        UserBLL ub = new UserBLL();
        UserPayBLL upb = new UserPayBLL();
        // GET: AgentMoney
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 查询当前用户的代理收益
        /// </summary>
        /// <returns></returns>
        public JsonResult ComputeAgentMoney()
        {
            int userid = int.Parse(Request["userid"].ToString());
            AgentModel agentmodel = new AgentModel();
            agentmodel = ab.GetAgentModel(userid);
            decimal firstAgent = 0;
            decimal secondAgent = 0;
            decimal ThridAgent = 0;
            AgentPercent ap=apb.GetAgentPercent(1);
            foreach (var item in agentmodel.Children)
            {
                //查询用户充值的总金额
                decimal res =upb.GetAgentMoney(item.ChildAgent.ID);
                //计算一级代理收益
                firstAgent = firstAgent + res*decimal.Parse(ap.Percent.ToString());
                if (item.ChildChildAgent!=null)
                {
                    foreach (var temp in item.ChildChildAgent)
                    {
                        //查询用户的充值总金额
                        decimal ress = upb.GetAgentMoney(temp.ID);
                        //计算二级代理收益
                        secondAgent = secondAgent + ress * decimal.Parse(ap.ChildP.ToString());
                        List<Agent> agentlist = ab.GetChildAgent(temp.ID);
                        if (agentlist != null)
                        {
                            foreach (var temps in agentlist)
                            {
                                //查询用户的充值总金额
                                decimal result = upb.GetAgentMoney(temps.UserID);
                                //计算三级代理收益
                                ThridAgent = ThridAgent + result * decimal.Parse(ap.ChildCP.ToString());

                            }
                        }
                    }
                }
            }
            var data = new
            {
                firstAgent= firstAgent,
                secondAgent= secondAgent,
                ThridAgent= ThridAgent
            };
            return Json(data);
        }
        /// <summary>
        /// 计算充值金额
        /// </summary>
        public void ComputedMoney()
        {
            using (StreamReader stream=new StreamReader(Request.InputStream))
            {
                string res=stream.ReadToEnd();
                this.obj = JObject.Parse(res);               
            }
        }
    }
}