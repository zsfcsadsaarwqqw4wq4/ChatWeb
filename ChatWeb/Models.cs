using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ChatWeb
{
    public class Users
    {
        public int id { get; set; }
        public string name { get; set; }
    }
    public class Msg
    {
        public int id { get; set; }
        public string msg { get; set; }
        public DateTime time { get; set; }
        public Users user { get; set; }
    }

    public static class Global
    {
        public static List<Users> Users = JsonConvert.DeserializeObject<List<Users>>(File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "/user.dat"));
        public static List<Msg> ListMsg = JsonConvert.DeserializeObject<List<Msg>>(File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "/msg.dat"));
        public static List<Msg> TempMsg = new List<Msg>();
        public static int NewMsgID = ListMsg.Count == 0 ? 1 : ListMsg.OrderByDescending(aa => aa.id).FirstOrDefault().id + 1;
    }
}