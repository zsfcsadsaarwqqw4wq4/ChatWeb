using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Model.ExModel;

namespace DAL
{
    /// <summary>
    /// 代理关系类
    /// </summary>
    public class AgentDAL
    {
        /// <summary>
        /// 得到当前用户的代理关系
        /// </summary>
        /// <param name="userid">用户id</param>
        /// <returns></returns>
        public Agent GetAgent(int userid)
        {
            using (ChatEntities db=new ChatEntities())
            {
               return db.Agent.SingleOrDefault(ag => ag.UserID == userid);
            }
        }
        /// <summary>
        /// 查询当前用户的一级子代理
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public List<Agent> GetChildAgent(int userid)
        {
            using (ChatEntities db=new ChatEntities())
            {
                return db.Agent.Where(ag => ag.ParentID == userid).ToList();
            }
        }
        /// <summary>
        /// 删除当前代理关系
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public bool DeleteAgent(Agent a)
        {
            using (ChatEntities db=new ChatEntities())
            {
                Agent agent= db.Agent.FirstOrDefault(ag => ag.ID == a.ID);
                bool flag = false;
                if (db.Agent.Remove(agent)!=null)
                {
                    flag = true;
                }
                return flag;
            }
        }
        /// <summary>
        /// 查询当前用户的父级代理
        /// </summary>
        public int QueryParentAgent(int uid)
        {
            using (ChatEntities db=new ChatEntities())
            {
                var data=db.Agent.FirstOrDefault(o=>o.UserID==uid);
                if(data!=null)
                {
                    int id = Convert.ToInt32(data.ParentID);
                    return id;
                }
                return 1;
            }
        }
        /// <summary>
        /// 修改代理关系
        /// </summary>
        /// <param name="a"></param>
        public bool UpdateAgent(Agent a)
        {
            using (ChatEntities db=new ChatEntities())
            {
                Agent agent= db.Agent.FirstOrDefault(ag => ag.ID == a.ID);
                agent.ID = a.ID;
                agent.ParentID = a.ParentID;
                agent.UserID = a.UserID;
                agent.Status = a.Status;
                agent.Percent = a.Percent;
                agent.Level = a.Level;
                return db.SaveChanges() > 0;
            }
        }
        /// <summary>
        /// 添加一条代理关系
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public bool AddAgent(Agent a)
        {
            using (ChatEntities db=new ChatEntities())
            {
                db.Agent.Add(a);
                return db.SaveChanges()>0;
            }
        }
        /// <summary>
        /// 统计当前用户的下级或下下级人数
        /// </summary>
        public void GetAgents()
        {
            using (ChatEntities db=new ChatEntities())
            {
                
            }
        }
        /// <summary>
        /// 获取当前用户的代理关系
        /// </summary>
        /// <returns></returns>
        //public AgentModel GetAgentModel(int userid)
        //{
        //    using (ChatEntities db = new ChatEntities())
        //    {
        //        AgentModel agentmodel = new AgentModel();
        //        agentmodel.Agent=db.User.FirstOrDefault(o => o.ID == userid);
        //        var agent= db.Agent.FirstOrDefault(o => o.UserID == userid);
        //        if (agent.Level==0)
        //        {
        //            return agentmodel;
        //        }
        //        else
        //        {
        //            int parentid=db.Agent.FirstOrDefault(o => o.UserID == agent.ParentID).UserID;
        //            agentmodel.ParentAgent=db.User.FirstOrDefault(o => o.ID == parentid);
        //            var childAgent=db.Agent.Where(o => o.ParentID == userid).ToList();
        //            List<User> childAgentlist=new List<User>();
        //            List<User> childchildAgentlist = new List<User>();
        //            List<object> datas = new List<object>();
        //            if (childAgent.Count>0)
        //            {
        //                foreach (var temp in childAgent)
        //                {
        //                    User childuser=new User();
        //                    childuser=db.User.FirstOrDefault(o => o.ID == temp.UserID);
        //                    childAgentlist.Add(childuser);
        //                    var childchildAgent = db.Agent.Where(o => o.ParentID == temp.UserID).ToList();                            
        //                    if(childchildAgent.Count>0)
        //                    {
        //                        foreach (var temps in childchildAgent)
        //                        {
        //                            User childchilduser = new User();
        //                            childchilduser = db.User.FirstOrDefault(o => o.ID == temps.UserID);
        //                            childchildAgentlist.Add(childchilduser);
        //                        }
        //                    }
        //                }
        //                agentmodel.ChildAgent = childAgentlist;
        //                agentmodel.ChildChildAgent = childchildAgentlist;
        //            }

        //        }
        //        return agentmodel;
        //    }
        //}
        /// <summary>
        /// 获取当前用户的代理关系
        /// </summary>
        /// <returns></returns>
        public AgentModel GetAgentModel(int userid)
        {
            using (ChatEntities db = new ChatEntities())
            {
                AgentModel agentmodel = new AgentModel();
                agentmodel.Agent = db.User.FirstOrDefault(o => o.ID == userid);
                var agent = db.Agent.FirstOrDefault(o => o.UserID == userid);
                if (agent.Level == 0)
                {
                    return agentmodel;
                }
                else
                {
                    int parentid = db.Agent.FirstOrDefault(o => o.UserID == agent.ParentID).UserID;
                    agentmodel.ParentAgent = db.User.FirstOrDefault(o => o.ID == parentid);
                    var childAgent = db.Agent.Where(o => o.ParentID == userid).ToList();                   
                    List<ChildUser> childuserlist = new List<ChildUser>();
                    if (childAgent.Count > 0)
                    {
                        foreach (var temp in childAgent)
                        {
                            ChildUser childuser=new ChildUser();
                            childuser.ChildAgent = db.User.FirstOrDefault(o => o.ID == temp.UserID);
                            var childchildAgent = db.Agent.Where(o => o.ParentID == temp.UserID).ToList();
                            if (childchildAgent.Count > 0)
                            {
                                List<User> ulist = new List<User>();
                                foreach (var temps in childchildAgent)
                                {
                                    User childchilduser = new User();
                                    childchilduser = db.User.FirstOrDefault(o => o.ID == temps.UserID);
                                    ulist.Add(childchilduser);
                                }
                                childuser.ChildChildAgent = ulist;
                            }
                            childuserlist.Add(childuser);
                        }
                        agentmodel.Children= childuserlist;
                    }
                

                }
                return agentmodel;
            }
        }
        /// <summary>
        /// 统计代理总数
        /// </summary>
        /// <returns></returns>
        public object AgentCount(int userid)
        {
            using (ChatEntities db=new ChatEntities())
            {
                List<Agent> firstagent=db.Agent.Where(o => o.ParentID == userid).ToList();
                int firstcount = firstagent.Count;                
                int secondcount = 0;
                foreach(var item in firstagent)
                {
                    int res = db.Agent.Where(o => o.ParentID == item.UserID).ToList().Count;
                    secondcount = secondcount + res;
                }
                var datas = new
                {
                    firstcount= firstcount,
                    secondcount= secondcount
                };
                return datas;
            }
        }
        /// <summary>
        /// 查询用户代理等级
        /// </summary>
        /// <param name="userid">用户id</param>
        /// <returns></returns>
        public int AgentLevel(int userid)
        {
            using (ChatEntities db=new ChatEntities())
            {
                var data=db.Agent.SingleOrDefault(o => o.UserID == userid);
                if (data!=null)
                {
                    int res =Convert.ToInt32(data.Level.ToString());
                    return res;
                }
                return 0;
            }
        }
    }
}
