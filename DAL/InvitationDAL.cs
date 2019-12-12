using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class InvitationDAL
    {
        /// <summary>
        /// 查询用户邀请码是否存在，返回值类型bool
        /// </summary>
        /// <param name="iv">邀请码实体</param>
        /// <returns></returns>
        public Invitation GetInvitation(Invitation iv)
        {
            using (ChatEntities db = new ChatEntities())
            {
                bool flag = false;
                Invitation invitation = db.Invitation.FirstOrDefault(i => i.InviteCode == iv.InviteCode);
                return invitation;
            }
        }
        /// <summary>
        /// 根据用户id判断当前登录用户是否已经生成邀请码
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public int GetUserInvitation(int userid)
        {
            using (ChatEntities db = new ChatEntities())
            {
                int flag = 0;
                Invitation invitation = db.Invitation.FirstOrDefault(i => i.UID == userid);
                if(invitation!=null)
                {
                    if (invitation.EndTime > DateTime.Now)
                    {
                        flag = invitation.InviteCode;
                    }
                    else
                    {
                        DeleteInvitation(invitation.InviteCode);
                        return flag;
                    }
                }
                return flag;
            }
        }
        /// <summary>
        /// 添加当前用户生成的邀请码
        /// </summary>
        /// <param name="iv"></param>
        /// <returns></returns>
        public bool CreaeteInvitation(Invitation iv)
        {
            using (ChatEntities db=new ChatEntities())
            {
                db.Invitation.Add(iv);
                return db.SaveChanges() > 0;
            }
        }
        /// <summary>
        /// 获取所有邀请码
        /// </summary>
        /// <returns></returns>
        public List<Invitation> GetAll()
        {
            using (ChatEntities db = new ChatEntities())
            {
                List<Invitation> iv = db.Invitation.ToList();
                return iv;
            }
        }
        /// <summary>
        /// 删除邀请码先查询后删除
        /// </summary>
        /// <param name="invitecode"></param>
        /// <returns></returns>
        public bool DeleteInvitation(int invitecode)
        {
            using (ChatEntities db=new ChatEntities())
            {
                List<Invitation> ivlist=db.Invitation.Where(i=>i.InviteCode==invitecode).ToList();
                bool flag = false;
                foreach(var temp in ivlist)
                {
                    Invitation invitation=db.Invitation.Remove(temp);
                    if (invitation != null)
                    {
                        flag = true;
                        db.SaveChanges();
                    }
                }             
                return flag;
            }
        }
    }
}
