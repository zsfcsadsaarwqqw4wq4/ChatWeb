using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Model.ExModel;

namespace BLL
{
    public class ThemeBLL
    {
        ThemeDAL td=new ThemeDAL();
        /// <summary>
        /// 获取主题内容，根据
        /// </summary>
        /// <returns></returns>
        public List<IGrouping<int, ThemeModel>> GetAll()
        {
            return td.GetAll();
        }
    }
}
