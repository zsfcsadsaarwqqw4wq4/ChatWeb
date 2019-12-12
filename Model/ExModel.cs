using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class ExModel
    {
        public class UserModel
        {
            /// <summary>
            /// 主键ID
            /// </summary>
            public int ID { get; set; }
            /// <summary>
            /// 登录账号
            /// </summary>
            public string LoginID { get; set; }
            /// <summary>
            /// 昵称
            /// </summary>
            public string NickeName { get; set; }
            /// <summary>
            /// 登录密码
            /// </summary>
            public string PassWord { get; set; }
            /// <summary>
            /// 个性签名
            /// </summary>
            public string SignaTure { get; set; }
            /// <summary>
            /// 性别
            /// </summary>
            public bool? Sex { get; set; }
            /// <summary>
            /// 生日
            /// </summary>
            public DateTime? Birthday { get; set; }
            /// <summary>
            /// 电话
            /// </summary>
            public string Telephone { get; set; }
            /// <summary>
            /// 真实姓名
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// 邮箱
            /// </summary>
            public string Email { get; set; }
            /// <summary>
            /// 头像
            /// </summary>
            public string HeadPortrait { get; set; }
            /// <summary>
            /// 年龄
            /// </summary>
            public int? Age { get; set; }
            /// <summary>
            /// vip等级
            /// </summary>
            public int? VIP { get; set; }
            /// <summary>
            /// 聊天安全实体
            /// </summary>
            public ChatLevel ChatLevels { get; set; }
            /// <summary>
            /// 虚拟金币
            /// </summary>
            public int? Gold { get; set; }
            /// <summary>
            /// 自定义logo实体
            /// </summary>
            public Logo Logos { get; set; }
            /// <summary>
            /// 职业
            /// </summary>
            public string Vocation { get; set; }
            /// <summary>
            /// 国家实体
            /// </summary>
            public Nation Nations { get; set; }
            /// <summary>
            /// 省份实体
            /// </summary>
            public Province Provinces { get; set; }
            /// <summary>
            /// 城市实体
            /// </summary>
            public City Citys { get; set; }
            /// <summary>
            /// 好友策略实体
            /// </summary>
            public FriendshipPolicy FriendshipPolicys { get; set; }
            /// <summary>
            /// 用户状态实体
            /// </summary>
            public State States { get; set; }
            /// <summary>
            /// 好友策略问题
            /// </summary>
            public string FriendPolicyQuestion { get; set; }
            /// <summary>
            /// 好友策略答案
            /// </summary>
            public string FriendPolicyAnswer { get; set; }
            /// <summary>
            /// 好友策略密码
            /// </summary>
            public string FriendPolicyPassword { get; set; }
        }
        public class Friend_Groups
        {
            /// <summary>
            /// 主键id
            /// </summary>
            public int ID { get; set; }
            /// <summary>
            /// 好友
            /// </summary>
            public User Firend { get; set; }
            /// <summary>
            /// 当前用户
            /// </summary>
            public User User { get; set; }
            /// <summary>
            /// 好友昵称
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// 好友类型
            /// </summary>
            public FriendType FriendType { get; set; }
            /// <summary>
            /// 好友分组
            /// </summary>
            public FriendGroups FriendGroups { get; set; }
            /// <summary>
            /// 状态0:代表不是好友1:代表是好友
            /// </summary>
            public int Status { get; set; }

        }
        public class FriendState
        {
            /// <summary>
            /// 用户
            /// </summary>
            public User user { get; set; }
            /// <summary>
            /// 过期时间
            /// </summary>
            public DateTime endtime { get; set; }
            public int State { get; set; }
        }
        public class ThemeModel
        {
            /// <summary>
            /// 主键ID
            /// </summary>
            public int ID { get; set; }
            /// <summary>
            /// 大主题类型
            /// </summary>
            public int ThemeTypeID { get; set; }
            /// <summary>
            /// 大主题名称
            /// </summary>
            public string TypeName { get; set; }
            /// <summary>
            /// 小主题名称
            /// </summary>
            public string FirstTheme { get; set; }
            /// <summary>
            /// 标题
            /// </summary>
            public string Title { get; set; }
            /// <summary>
            /// 内容
            /// </summary>
            public string Content { get; set; }
            /// <summary>
            /// 发布的图片
            /// </summary>
            public string ThemeImage { get; set; }
            /// <summary>
            /// 回复的内容
            /// </summary>
            public List<Reply> reply { get; set; }
        }
        public class UserMessage
        {
            public int ID { get; set; }
            public string PostMessages { get; set; }  
            public bool? ReciveStatus { get; set; }
            public DateTime Time { get; set; }
            public int? MessagesTypeID { get; set; }
            public User FromUser { get; set; }
            public User ToUser { get; set; }
            public int? Status { get; set; }
            public string GUID { get; set; }
        }
        /// <summary>
        ///推送的附加参数
        /// </summary>
        public class Push
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }
        public class AgentModel
        {
            /// <summary>
            /// 代表当前用的上级代理用户信息
            /// </summary>
            public User ParentAgent { get; set; }
            /// <summary>
            /// 当前用户代理信息
            /// </summary>
            public User Agent { get; set; }
            /// <summary>
            /// 表示当前用户的下级代理用户信息
            /// </summary>
            //public List<User> ChildAgent { get; set; }
            /// <summary>
            /// 表示当前用户的下级的下级代理用户信息
            /// </summary>
            //public List<User> ChildChildAgent { get; set; }
            public List<ChildUser> ChilDren { get; set; }
        }
        public class ChildUser
        {
            public User ChildAgent { get; set; }
            public List<User> ChildChildAgent { get; set; }
        }

    }
}
