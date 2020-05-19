using DAL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Model.ExModel;

namespace BLL
{
    public class UserPayBLL
    {
        UserPayDAL upd = new UserPayDAL();
        /// <summary>
        /// 查询用户的总充值金额
        /// </summary>
        /// <returns></returns>
        public decimal GetAgentMoney(int userid)
        {
            return upd.GetAgentMoney(userid);
        }
    }
}