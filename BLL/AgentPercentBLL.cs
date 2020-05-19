using DAL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class AgentPercentBLL
    {
        AgentPercentDAL apd=new AgentPercentDAL();
        /// <summary>
        /// 根据用户代理等级查询代理分红比例
        /// </summary>
        /// <param name="AgentLevel"></param>
        /// <returns></returns>
        public AgentPercent GetAgentPercent(int modelid)
        {
            return apd.GetAgentPercent(modelid);
        }
    }
}
