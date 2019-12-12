using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Model.ExModel;

namespace DAL
{
    public class MessagesDAL
    {
        /// <summary>
        /// 添加消息记录
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
        public bool CreateMessages(Messages ms)
        {
            using (ChatEntities db = new ChatEntities())
            {
                db.Messages.Add(ms);
                return db.SaveChanges() > 0;
            }
        }
        /// <summary>
        /// 获取所有的聊天记录
        /// </summary>
        /// <returns></returns>
        public List<Messages> GetMessagesAll()
        {
            using (ChatEntities db = new ChatEntities())
            {
                List<Messages> listmessage=db.Messages.ToList();
                return listmessage;
            }
        }
        /// <summary>
        /// 获取当前用户的聊天记录
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public List<Messages> GetMessages(int userid,DateTime time)
        {
            using (ChatEntities db=new ChatEntities())
            {
                List<Messages> listmessage=db.Messages.Where(m=>m.FromUserID== userid&&m.Time>time).ToList();
                return listmessage;
            }
        }
        /// <summary>
        /// 获取当前用户的聊天记录
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public List<IGrouping<int, UserMessage>> GetMessage(int userid, DateTime time)
        {
            using (ChatEntities db = new ChatEntities())
            {
                var query = (from a in db.Messages
                             where a.ToUserID == userid && a.Time > time
                             select new UserMessage
                             {
                                 ID=a.ID,
                                 PostMessages=a.PostMessages,
                                 ReciveStatus=a.ReciveStatus,
                                 Time=a.Time,
                                 MessagesTypeID=a.MessagesTypeID,
                                 FromUser=db.User.FirstOrDefault(o=>o.ID==a.FromUserID),
                                 ToUser= db.User.FirstOrDefault(o => o.ID == a.ToUserID),
                                 Status=a.Status,
                                 GUID=a.GUID
                             }).ToList();
                List<IGrouping<int, UserMessage>> res = query.GroupBy(o => o.ToUser.ID).ToList();
                return res;
            }
        }
        /// <summary>
        /// 查询当前用户一个星期的聊天记录并分页
        /// </summary>
        /// <param name="pageindex">当前页的索引</param>
        /// <param name="pagesize">每页大小</param>
        /// <returns></returns>
        public List<Messages> PageMessages(int pageindex, int pagesize, int fromuserid)
        {
            using (ChatEntities db = new ChatEntities())
            {
                DateTime time = DateTime.Now.AddDays(-7);
                return db.Messages.Where(m=>m.FromUserID== fromuserid && m.Time>time).Skip((pageindex - 1) * pagesize).Take(pagesize).ToList();
            }
        }  
        /// <summary>
        /// 查询当前用户的消息记录的总条数
        /// </summary>
        /// <param name="fromuserid">用户id</param>
        /// <returns></returns>
        public int PageCount(int fromuserid)
        {
            using (ChatEntities db=new ChatEntities())
            {
                int data=db.Messages.Count(m => m.FromUserID == fromuserid);
                int result = 0;
                if (data!=0)
                {
                    result=data;
                }
                return result;
            }
        } 
        /// <summary>
        /// 修改消息状态
        /// </summary>
        public bool EditMessage(int touserid,DateTime time)
        {
            using (ChatEntities db=new ChatEntities())
            {
                List<Messages> list=db.Messages.Where(m=>m.ToUserID==touserid&&m.Time<time).ToList();
                foreach (var temp in list)
                {
                    temp.Status = 1;
                }                
                return db.SaveChanges()>0;
            }
        }
    }
}
