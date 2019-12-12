using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class RoleDAL
    {
        /// <summary>
        /// 查看当前用户所拥有的角色
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public User_Role GetRole(int id)
        {
            using (ChatEntities db=new ChatEntities())
            {
                User_Role ur = db.User_Role.FirstOrDefault(r => r.UserID == id);
                return ur;
            }
        }
    }
}
