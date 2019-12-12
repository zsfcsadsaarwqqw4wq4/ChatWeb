using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Model.ExModel;

namespace DAL
{
    public class FriendGroupsDAL
    {
        /// <summary>
        /// 获取所有的好友分组信息
        /// </summary>
        /// <returns></returns>
        public List<Friends> GetAll(int userid)
        {
            List<Friends> list = new List<Friends>();
            using (ChatEntities db=new ChatEntities())
            {
                list = db.Friends.Where(a=>a.UserID == userid && a.Status == 1).ToList();
            }
            return list;
        }
        /// <summary>
        /// 查询当前用户的好友分组信息
        /// </summary>
        /// <returns></returns>
        public List<IGrouping<string, Friend_Groups>> GetFriendsGroup(int userid)
        {
            using (ChatEntities db = new ChatEntities())
            {
                List<Friend_Groups> list = (from a in db.Friends
                                      where a.UserID == userid && a.Status == 1
                                      select new Friend_Groups
                                      {
                                          ID = a.ID,
                                          Firend = db.User.FirstOrDefault(aa=>aa.ID==a.FirendID),
                                          User= db.User.FirstOrDefault(aa => aa.ID == a.UserID),
                                          Name=a.Name,
                                          FriendType= db.FriendType.FirstOrDefault(aa => aa.ID == a.FriendTypeID),
                                          FriendGroups= db.FriendGroups.FirstOrDefault(aa => aa.ID == a.FriendGroupsID),
                                          Status=a.Status
                                      }).ToList();
                List<IGrouping<string,Friend_Groups>> res = list.GroupBy(aa => aa.FriendGroups.Name).ToList();
                return res;
            }
        }        
    }
}
