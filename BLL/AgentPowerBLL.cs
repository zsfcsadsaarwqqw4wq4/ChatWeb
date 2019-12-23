using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public  class AgentPowerBLL
    {
        AgentPowerDAL apd=new AgentPowerDAL();
        /// <summary>
        /// 查询用户是否有分红权限
        /// </summary>
        /// <param name="uid"></param>
        public void GetAgentPower(int uid)
        {
            apd.GetAgentPower(uid);
        }
    }
}
