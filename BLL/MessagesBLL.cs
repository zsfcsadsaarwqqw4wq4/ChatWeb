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
    public class MessagesBLL
    {
        MessagesDAL md = new MessagesDAL();
        /// <summary>
        /// 添加消息记录
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
        public bool CreateMessages(Messages ms)
        {
            return md.CreateMessages(ms);
        }
        /// <summary>
        /// 查询当前登录用户的聊天消息记录
        /// </summary>
        /// <param name="us"></param>
        /// <returns></returns>
        public List<Messages> GetMessages(int userid,DateTime time)
        {
            return md.GetMessages(userid,time);
        }
        /// <summary>
        /// 用户的聊天消息记录
        /// </summary>
        /// <param name="us"></param>
        /// <returns></returns>
        public List<IGrouping<int, UserMessage>> GetMessage(int userid, DateTime time)
        {
            return md.GetMessage(userid, time);
        }
        /// <summary>
        /// 查询当前用户七天前的聊天记录
        /// </summary>
        /// <param name="pageindex">当前页码</param>
        /// <param name="pagesize">每页显示条数</param>
        /// <param name="fromuserid">当前用户id</param>
        /// <returns></returns>
        public List<Messages> PageMessages(int pageindex, int pagesize, int fromuserid)
        {
            return md.PageMessages(pageindex, pagesize, fromuserid);
        }
        /// <summary>
        /// 查询当前用户的消息记录的总条数
        /// </summary>
        /// <param name="fromuserid">用户id</param>
        /// <returns></returns>
        public int PageCount(int fromuserid)
        {
            return md.PageCount(fromuserid);
        }
        /// <summary>
        /// 修改消息状态
        /// </summary>
        public bool EditMessage(int touserid, DateTime time)
        {
            return md.EditMessage(touserid, time);
        }
    }
}
