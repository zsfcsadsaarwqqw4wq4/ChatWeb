using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class AgentPercentDAL
    {
        /// <summary>
        /// 根据用户代理等级查询代理分红比例
        /// </summary>
        /// <param name="AgentLevel"></param>
        /// <returns></returns>
        public AgentPercent GetAgentPercent(int modelid)
        {
            using (ChatEntities db=new ChatEntities())
            {
                return db.AgentPercent.SingleOrDefault(o => o.ModelID == modelid);
            }
        }
    }
}
