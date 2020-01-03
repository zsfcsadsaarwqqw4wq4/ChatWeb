using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class AgentMoneyDAL
    {
        /// <summary>
        /// 获取用户的代理结算信息
        /// </summary>
        /// <param name="uid"></param>
        public AgentMoney GetAgentMoney(int uid)
        {
            using (ChatEntities db=new ChatEntities())
            {
                var data=db.AgentMoney.SingleOrDefault(o => o.UserID == uid);
                if(data != null)
                {
                    return data;
                }
                else
                {
                    string js = "<li>"+data+"</li>";
                    return null;
                }
            }
        }
        /// <summary>
        /// 删除代理结算信息
        /// </summary>
        public void DeleteAgentMoney()
        {
            using (ChatEntities db=new ChatEntities())
            {
                
            }
        }
    }
}
