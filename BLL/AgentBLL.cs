﻿using DAL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Model.ExModel;

namespace BLL
{
    public class AgentBLL
    {
        AgentDAL ad = new AgentDAL();
        /// <summary>
        /// 获取代理关系
        /// </summary>
        /// <returns></returns>
        public Agent GetAgent(int userid)
        {
            return ad.GetAgent(userid);
        }
        /// <summary>
        /// 删除代理关系
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public bool DeleteAgent(Agent a)
        {
            return ad.DeleteAgent(a);
        }
        /// <summary>
        /// 获取当前用户的代理关系
        /// </summary>
        /// <returns></returns>
        public AgentModel GetAgentModel(int userid)
        {
           return ad.GetAgentModel(userid);
        }
        /// <summary>
        /// 获取当前登录用户的父级id
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public int QueryParentAgent(int uid)
        {
            return ad.QueryParentAgent(uid);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public object AgentCount(int userid)
        {
            return ad.AgentCount(userid);
        }
        /// <summary>
        /// 查询用户代理等级
        /// </summary>
        /// <param name="userid">用户id</param>
        /// <returns></returns>
        public int AgentLevel(int userid)
        {
            return ad.AgentLevel(userid);
        }
        /// <summary>
        /// 查询当前用户的一级子代理
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public List<Agent> GetChildAgent(int userid)
        {
            return ad.GetChildAgent(userid);
        }
    }
}
