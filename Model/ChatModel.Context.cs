﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Model
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ChatEntities : DbContext
    {
        public ChatEntities()
            : base("name=ChatEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Agent> Agent { get; set; }
        public virtual DbSet<AgentMoney> AgentMoney { get; set; }
        public virtual DbSet<AgentPerModel> AgentPerModel { get; set; }
        public virtual DbSet<ChatLevel> ChatLevel { get; set; }
        public virtual DbSet<City> City { get; set; }
        public virtual DbSet<FriendGroups> FriendGroups { get; set; }
        public virtual DbSet<FriendshipPolicy> FriendshipPolicy { get; set; }
        public virtual DbSet<FriendType> FriendType { get; set; }
        public virtual DbSet<Groups> Groups { get; set; }
        public virtual DbSet<GroupsMSGContent> GroupsMSGContent { get; set; }
        public virtual DbSet<GroupsMSGToUser> GroupsMSGToUser { get; set; }
        public virtual DbSet<GroupsMSGUserToUser> GroupsMSGUserToUser { get; set; }
        public virtual DbSet<GroupsToUser> GroupsToUser { get; set; }
        public virtual DbSet<Invitation> Invitation { get; set; }
        public virtual DbSet<Logo> Logo { get; set; }
        public virtual DbSet<Messages> Messages { get; set; }
        public virtual DbSet<MessagesStillType> MessagesStillType { get; set; }
        public virtual DbSet<MessagesType> MessagesType { get; set; }
        public virtual DbSet<Nation> Nation { get; set; }
        public virtual DbSet<Province> Province { get; set; }
        public virtual DbSet<Reply> Reply { get; set; }
        public virtual DbSet<State> State { get; set; }
        public virtual DbSet<Theme> Theme { get; set; }
        public virtual DbSet<ThemeType> ThemeType { get; set; }
        public virtual DbSet<UserPay> UserPay { get; set; }
        public virtual DbSet<AgentPower> AgentPower { get; set; }
        public virtual DbSet<UID> UID { get; set; }
        public virtual DbSet<Friends> Friends { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<AgentPercent> AgentPercent { get; set; }
    }
}
