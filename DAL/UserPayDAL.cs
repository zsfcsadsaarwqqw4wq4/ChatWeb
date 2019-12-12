using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    /// <summary>
    /// 用户充值类
    /// </summary>
    public class UserPayDAL
    {
        /// <summary>
        /// 获取当前用户的充值信息
        /// </summary>
        /// <param name="userpayid">充值id</param>
        /// <returns></returns>
        public List<UserPay> GetUserPay(int userpayid)
        {
            using (ChatEntities db=new ChatEntities())
            {
                return  db.UserPay.Where(u=>u.UserID==userpayid).ToList();
;           }
        }
        /// <summary>
        /// 删除当前用户的消费记录
        /// </summary>
        /// <param name="up"></param>
        /// <returns></returns>
        public bool DeleteUserPay(UserPay up)
        {
            using (ChatEntities db = new ChatEntities())
            {
                UserPay ups = db.UserPay.Find(up.ID);
                db.UserPay.Remove(ups);
                return db.SaveChanges() > 0;
            }
        }
        /// <summary>
        /// 修改用户的一条消费记录
        /// </summary>
        /// <param name="up"></param>
        /// <returns></returns>
        public bool UpdateUserPay(UserPay up)
        {
            using (ChatEntities db=new ChatEntities())
            {
                UserPay userpay = db.UserPay.SingleOrDefault(u => u.ID == up.ID);
                userpay.ID = up.ID;
                userpay.UserID = up.UserID;
                userpay.PayMoney = up.PayMoney;
                userpay.PayTime = up.PayTime;
                userpay.Type = up.Type;
                return db.SaveChanges() > 0;
            }
        }
        /// <summary>
        /// 添加一条充值记录
        /// </summary>
        /// <param name="up"></param>
        /// <returns></returns>
        public bool AddUserPay(UserPay up)
        {
            using (ChatEntities db=new ChatEntities())
            {
                db.UserPay.Add(up);
                return db.SaveChanges() > 0;              
            }
        }

    }
}
