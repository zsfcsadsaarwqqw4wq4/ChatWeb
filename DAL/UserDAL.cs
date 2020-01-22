using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Model.ExModel;

namespace DAL
{
    public class UserDAL
    {
        /// <summary>
        /// 根据账号查询用户信息
        /// </summary>
        /// <param name="loginid">登陆账号</param>
        /// <returns></returns>
        public User GetUser(User us)
        {
            using (ChatEntities db = new ChatEntities())
            {
                User user = db.User.SingleOrDefault(u => u.LoginID==us.LoginID);
                return user;

            }
        }
        //根据ID查询用户信息
        public User GetUserById(int Id)
        {
            using (ChatEntities db = new ChatEntities())
            {
                return db.User.FirstOrDefault(i => i.ID == Id && i.Shape != -1);
            }
        }
        //查询用户是否已被注册
        public bool GetUserIsRegister(string loginid)
        {
            bool flag = false;
            using (ChatEntities db = new ChatEntities())
            {
                User user = db.User.FirstOrDefault(i => i.LoginID == loginid && i.Shape != -1);
                if (user != null)
                {
                    flag = true;
                }
                return flag;
            }
        }
        /// <summary>
        /// 根据ID删除用户,修改状态
        /// </summary>
        /// <param name="user">当前选中的用户</param>
        /// <returns></returns>
        public bool DeleteUser(int id)
        {
            using (ChatEntities db = new ChatEntities())
            {
                User us = db.User.SingleOrDefault(u => u.ID == id);
                db.User.Remove(us);
                return db.SaveChanges() > 0;
            }
        }
        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="us"></param>
        /// <returns></returns>
        public bool CreateUser(User us)
        {
            using (ChatEntities db = new ChatEntities())
            {
                db.User.Add(us);
                return db.SaveChanges()>0;
            }
        }
        /// <summary>
        /// 修改用户信息
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool EditUser(User user)
        {
            using (ChatEntities db = new ChatEntities())
            {
                User us = db.User.SingleOrDefault(u => u.ID == user.ID);
                us.ID = user.ID;
                us.LoginID = user.LoginID;
                us.NickeName = user.NickeName;
                us.PassWord = user.PassWord;
                us.SignaTure = user.SignaTure;
                us.Sex = user.Sex;
                us.Birthday = user.Birthday;
                us.Telephone = user.Telephone;
                us.Name = user.Name;
                us.Email = user.Email;
                us.HeadPortrait = user.HeadPortrait;
                us.Age = user.Age;
                us.VIP = user.VIP;
                us.Level = user.Level;
                us.Gold = user.Gold;
                us.UserLogoID = user.UserLogoID;
                us.Vocation = user.Vocation;
                us.NationID = user.NationID;
                us.ProvinceID = user.ProvinceID;
                us.CityID = user.ProvinceID;
                us.FriendshipPolicyID = user.FriendshipPolicyID;
                us.UserStateID = user.UserStateID;
                us.FriendPolicyQuestion = user.FriendPolicyQuestion;
                us.FriendPolicyAnswer = user.FriendPolicyAnswer;
                us.FriendPolicyPassword = user.FriendPolicyPassword;
                us.PassWords = user.PassWords;
                us.Picture = user.Picture;
                us.PhoneNumber = user.PhoneNumber;
                us.USearchState = user.USearchState;
                us.PSearchState = user.PSearchState;
                us.FirstProblem = user.FirstProblem;
                us.FirstAnswer = user.FirstAnswer;
                us.SecondProblem = user.SecondProblem;
                us.SecondAnswer = user.SecondAnswer;
                us.BurnAfterReading = user.BurnAfterReading;
                us.ChatTimeLimit = user.ChatTimeLimit;
                us.ThemeTypeID = user.ThemeTypeID;
                us.SecondThemeTypeID = user.SecondThemeTypeID;
                us.IsEnterSendMsg = user.IsEnterSendMsg;
                us.ChatSwitch = user.ChatSwitch;
                us.LastLoginAt=user.LastLoginAt;
                return db.SaveChanges() > 0;
            }
        }
        
        /// <summary>
        /// 设置用户的迷惑密码
        /// </summary>
        /// <returns></returns>
        public bool EditUsers(User user)
        {
            using (ChatEntities db = new ChatEntities())
            {
                User us = db.User.SingleOrDefault(u => u.ID == user.ID);
                us.PassWords = user.PassWords;
                us.FirstProblem = user.FirstProblem;
                us.FirstAnswer = user.FirstAnswer;
                us.SecondProblem = user.SecondProblem;
                us.SecondAnswer = user.SecondAnswer;
                return db.SaveChanges() > 0;
            }
        }
        /// <summary>
        /// 修改用户登陆时间
        /// </summary>
        public void UpdateUser(int userid,DateTime time)
        {
            using (ChatEntities db=new ChatEntities())
            {
                User user=db.User.SingleOrDefault(u=>u.ID==userid);
                user.LastLoginAt = time;
                db.SaveChanges();
            }
        }
        /// <summary>
        /// 获取所有用户关联信息
        /// </summary>
        /// <returns></returns>
        public List<UserModel> SelectUser()
        {
            using (ChatEntities db = new ChatEntities())
            {
                List<UserModel> umList = new List<UserModel>();
                umList = (from a in db.User
                          where (a.Shape != -1)
                          select new UserModel
                          {
                              ID = a.ID,
                              LoginID = a.LoginID,
                              NickeName = a.NickeName,
                              PassWord = a.PassWord,
                              SignaTure = a.SignaTure,
                              Sex = a.Sex,
                              Birthday = a.Birthday,
                              Telephone = a.Telephone,
                              Name = a.Name,
                              Email = a.Email,
                              HeadPortrait = a.HeadPortrait,
                              Age = a.Age,
                              VIP = a.VIP,
                              ChatLevels = db.ChatLevel.FirstOrDefault(cl => cl.ID == a.Level),
                              Gold = a.Gold,
                              Logos = db.Logo.FirstOrDefault(lg => lg.ID == a.UserLogoID),
                              Vocation = a.Vocation,
                              Nations = db.Nation.FirstOrDefault(n => n.ID == a.NationID),
                              Provinces = db.Province.FirstOrDefault(p => p.ID == a.ProvinceID),
                              Citys = db.City.FirstOrDefault(c => c.ID == a.CityID),
                              FriendshipPolicys = db.FriendshipPolicy.FirstOrDefault(fp => fp.ID == a.FriendshipPolicyID),
                              States = db.State.FirstOrDefault(fp => fp.ID == a.UserStateID),
                              FriendPolicyQuestion = a.FriendPolicyQuestion,
                              FriendPolicyAnswer = a.FriendPolicyAnswer,
                              FriendPolicyPassword = a.FriendPolicyPassword
                          }).ToList();
                return umList;
            }
        }
        /// <summary>
        /// 获取当前用户的全部好友列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<User> GetFriends(int id)
        {
            using (ChatEntities db = new ChatEntities())
            {
                List<Friends> us = db.Friends.Where(f => f.UserID == id && f.Status == 1).ToList();
                List<User> userlist = new List<User>();
                foreach (var temp in us)
                {
                    User user = new User();
                    user = db.User.FirstOrDefault(u => u.ID == temp.FirendID);
                    userlist.Add(user);
                }
                return userlist;
            }
        }
        /// <summary>
        /// 根据登录名搜索联系人
        /// </summary>
        /// <returns></returns>
        public User GetUserName(string loginid)
        {
            using (ChatEntities db = new ChatEntities())
            {
                return db.User.FirstOrDefault(u => u.LoginID == loginid&&u.USearchState==true);
            }
        }
        /// <summary>
        /// 根据登录名查询用户
        /// </summary>
        /// <returns></returns>
        public User GetUserNames(string loginid)
        {
            using (ChatEntities db = new ChatEntities())
            {
                return db.User.FirstOrDefault(u => u.LoginID == loginid);
            }
        }
        /// <summary>
        /// 根据电话号码查询用户
        /// </summary>
        /// <returns></returns>
        public User GetPhoneNumber(string loginid)
        {
            using (ChatEntities db = new ChatEntities())
            {
                return db.User.FirstOrDefault(u => u.PhoneNumber == loginid && u.PSearchState == true);
            }
        }
        /// <summary>
        /// 将通过申请的好友状态改变
        /// </summary>
        /// <returns></returns>
        public bool EditFirends(int userid, int firendid)
        {
            using (ChatEntities db = new ChatEntities())
            {
                List<Friends> friendlist = db.Friends.Where(f => f.UserID == userid && f.FirendID == firendid || f.UserID == firendid && f.FirendID == userid&&f.Status==0&&f.OverdueTime>DateTime.Now).ToList();
                List<IGrouping<int, Friends>> res = friendlist.GroupBy(aa => aa.UserID).ToList();
                foreach (var temp in res)
                {
                    Friends fd=temp.ToList().LastOrDefault();
                    fd.Status = 1;
                }
                return db.SaveChanges() > 0;
            }
        }
        /// <summary>
        /// 先将好友申请添加到好有实体中
        /// </summary>
        /// <param name="firendid"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool AddFirend(int userid, int friendsid, string name, int friendtypeid, int friendgroupsid, int status,DateTime time)
        {
            using (ChatEntities db = new ChatEntities())
            {
                Friends friend = new Friends();
                friend.UserID = userid;
                friend.Status = status;
                friend.FirendID = friendsid;
                friend.Name = name;
                friend.FriendTypeID = friendtypeid;
                friend.FriendGroupsID = friendgroupsid;
                friend.Status = status;
                friend.OverdueTime = time;
                db.Friends.Add(friend);
                return db.SaveChanges() > 0;
            }
        }
        /// <summary>
        /// 当前用户所有的好友申请
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public List<FriendState> SelectFirend(int userid)
        {
            using (ChatEntities db = new ChatEntities())
            {
                List<Friends> us = db.Friends.Where(f => f.FirendID == userid && f.Status == 0).ToList();
                List<IGrouping<int, Friends>> res = us.GroupBy(aa => aa.UserID).ToList();                 
                List<FriendState> userlist = new List<FriendState>();
                foreach (var temp in res)
                {
                    FriendState friendstate=new FriendState();
                    friendstate.user = db.User.FirstOrDefault(u => u.ID == temp.Key);
                    var data = temp.ToList();
                    friendstate.endtime = data.Last(u => u.UserID == temp.Key && u.Status == 0).OverdueTime;
                    userlist.Add(friendstate);
                }
                return userlist;
            }
        }
        /// <summary>
        /// 获取用户头像
        /// </summary>
        /// <returns></returns>
        public string GetUserHeadImg(int uid)
        {
            using (ChatEntities db=new ChatEntities())
            {
                User us= db.User.SingleOrDefault(o=>o.ID==uid);
                if (us!=null)
                {
                    return us.HeadPortrait;
                }
                else
                {
                    return null;
                }           
            }
        }
        /// <summary>
        /// 如果没有迷惑密码，默认1111
        /// </summary>
        public bool UpdatePassword(User user)
        {
            using (ChatEntities db = new ChatEntities())
            {
                User us = db.User.SingleOrDefault(u => u.ID == user.ID);
                us.PassWords = user.PassWords;
                return db.SaveChanges() > 0;
            }
        }
    }
}
