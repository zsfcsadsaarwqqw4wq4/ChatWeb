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
        public AgentPercent GetPercent(int uid)
        {
            return apd.GetPercent(uid);
        }
        
    }
}
