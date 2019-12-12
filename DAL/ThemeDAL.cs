using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Model.ExModel;

namespace DAL
{
    public class ThemeDAL
    {
        /// <summary>
        /// 获取主题内容，根据
        /// </summary>
        /// <returns></returns>
        public List<IGrouping<int, ThemeModel>> GetAll()
        {
            using (ChatEntities db = new ChatEntities())
            {
                List <IGrouping<int, ThemeModel> >  tmgroup=new List<IGrouping<int, ThemeModel>>();
                List<ThemeModel> list = new List<ThemeModel>();
                list = (from a in db.Theme
                        select new ThemeModel
                        {
                            ID = a.ID,
                            ThemeTypeID= a.ThemeTypeID,
                            TypeName = db.ThemeType.FirstOrDefault(o => o.ID == a.ThemeTypeID).TypeName,
                            FirstTheme=a.FirstTheme,
                            Title=a.Title,
                            Content=a.Content,
                            ThemeImage=a.ThemeImage,
                            reply=db.Reply.Where(o=>o.ThemeID==a.ID).ToList()
                        }).ToList();
                tmgroup=list.GroupBy(o => o.ThemeTypeID).ToList();
                return tmgroup;
            }
        }
    }
}
