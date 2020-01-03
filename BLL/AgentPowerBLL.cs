using DAL;
using Model;
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
        /// <summary>
        /// 获取当前用户的代理权限
        /// </summary>
        /// <param name="uid"></param>
        public AgentPower Get(int uid)
        {
            return apd.Get(uid);
        }
        /// <summary>
        /// 判断当前用户是否拥有设置下线分红的权限
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public bool AddAgentPower(int uid)
        {
           return apd.AddAgentPower(uid);
        }            
    }
}
