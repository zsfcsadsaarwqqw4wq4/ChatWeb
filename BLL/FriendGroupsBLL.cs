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
    public class FriendGroupsBLL
    {
        FriendGroupsDAL fgd= new FriendGroupsDAL();
        /// <summary>
        /// 获取所有的好友分组信息
        /// </summary>
        /// <returns></returns>
        public List<Friends> GetAll(int userid)
        {
            return fgd.GetAll(userid);
        }
        /// <summary>
        /// 查询当前用户的好友分组信息
        /// </summary>
        /// <returns></returns>
        public List<IGrouping<string, Friend_Groups>> GetFriendsGroup(int userid)
        {
            return fgd.GetFriendsGroup(userid);
        }
    }
}
