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
        /// 查询当前用户的分红比例
        /// </summary>
        /// <param name="uid"></param>
        public AgentPercent GetPercent(int uid)
        {
            using (ChatEntities db=new ChatEntities())
            {
                return db.AgentPercent.FirstOrDefault(o=>o.UserID==uid);
            }
        }
    }
}
