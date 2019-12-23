using DAL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class LogoBLL
    {
        LogoDAL ld= new LogoDAL();
        public List<Logo> GetAll()
        {
            return ld.GetAll();
        }
    }
}
