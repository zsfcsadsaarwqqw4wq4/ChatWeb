using DAL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class InvitationBLL
    {
        InvitationDAL ivd = new InvitationDAL();
        /// <summary>
        /// 查询邀请码是否存在，存在返回True反之False
        /// </summary>
        /// <param name="iv"></param>
        /// <returns></returns>
        public Invitation GetInvitation(Invitation iv)
        {
            return ivd.GetInvitation(iv);
        }
        /// <summary>
        /// 删除邀请码先查询后删除
        /// </summary>
        /// <param name="invitecode">邀请码</param>
        /// <returns></returns>
        public bool DeleteInvitation(int invitecode)
        {
            return ivd.DeleteInvitation(invitecode);
        }
        /// <summary>
        /// 添加当前用户生成的邀请码
        /// </summary>
        /// <param name="iv"></param>
        /// <returns></returns>
        public bool CreaeteInvitation(Invitation iv)
        {
            return ivd.CreaeteInvitation(iv);
        }
        /// <summary>
        /// 获取所有邀请码
        /// </summary>
        /// <returns></returns>
        public List<Invitation> GetAll()
        {
            return ivd.GetAll();
        }
        /// <summary>
        /// 根据用户id判断当前登录用户是否已经生成邀请码
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public int GetUserInvitation(int userid)
        {
            return ivd.GetUserInvitation(userid);
        }
    }
}
