using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class AgentPowerDAL
    {
        /// <summary>
        /// 查询当前用户是否有权限能设置下线分红
        /// </summary>
        /// <param name="uid"></param>
        public void GetAgentPower(int uid)
        {
            using (ChatEntities db=new ChatEntities())
            {
            }
        }       
        /// <summary>
        /// 获取当前用户的代理权限
        /// </summary>
        public AgentPower Get(int uid)
        {
            using (ChatEntities db = new ChatEntities())
            {
                return db.AgentPower.FirstOrDefault(o => o.UserID == uid);                
            }
        }
        /// <summary>
        /// 是否能够设置下线分红
        /// </summary>
        /// <returns></returns>
        public bool AddAgentPower(int uid)
        {
            using (ChatEntities db = new ChatEntities())
            {
                bool result = false;
                var agentpower = db.AgentPower.FirstOrDefault(o => o.UserID == uid);
                if (agentpower.BChild)
                {
                    if(agentpower.BChildMoney)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
                return result;
            }
        }
    }
}
