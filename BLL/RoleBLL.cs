using DAL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class RoleBLL
    {
        RoleDAL rd=new RoleDAL();
        /// <summary>
        /// 查看当前用户所拥有的角色
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public User_Role GetRole(int id)
        {
            return rd.GetRole(id);
        }
        
    }
}
