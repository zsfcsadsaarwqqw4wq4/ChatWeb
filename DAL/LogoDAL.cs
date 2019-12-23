using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class LogoDAL
    {
        /// <summary>
        /// 获取所有的Logo
        /// </summary>
        /// <returns></returns>
        public List<Logo> GetAll()
        {
            using (ChatEntities db=new ChatEntities())
            {
               return db.Logo.ToList();
            }
        }
    }
}
