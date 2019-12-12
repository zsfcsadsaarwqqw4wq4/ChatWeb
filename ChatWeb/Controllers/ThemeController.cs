using BLL;
using Common;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using static Common.EnumHelper;
using static Model.ExModel;

namespace ChatWeb.Controllers
{
    public class ThemeController : BaseController
    {
        ResultData resultdata = new ResultData();
        List<IGrouping<int, ThemeModel>> Themes;
        List<ThemeModel> listtm = new List<ThemeModel>();
        ThemeBLL tb = new ThemeBLL();
        public Enum enums;
        // GET: Theme
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 贴吧主题内容
        /// </summary>
        /// <returns></returns>
        public JsonResult GetTheme()
        {
            try
            {
                RequestUser();
                if (resultData.res == 500)
                {
                    return Json(resultData);
                }
                Themes = tb.GetAll();
                Dictionary<int, List<ThemeModel>> dic = new Dictionary<int, List<ThemeModel>>();
                foreach (var temp in Themes)
                {
                    int key = int.Parse(temp.Key.ToString());
                    listtm = temp.ToList();
                    foreach (var item in listtm)
                    {
                        item.ThemeImage = Constant.files + item.ThemeImage;
                    }
                    dic.Add(key, listtm);
                }
                //List<object> ss = new List<object>();
                //foreach (var tem in dic)
                //{                   
                //    switch (tem.Key)
                //    {
                //        case 1:
                //            enums = TiebaEnum.One;
                //            break;
                //        case 2:
                //            enums = TiebaEnum.two;
                //            break;
                //        case 3:
                //            enums = TiebaEnum.three;
                //            break;
                //        case 4:
                //            enums = TiebaEnum.four;
                //            break;
                //        case 5:
                //            enums = TiebaEnum.five;
                //            break;
                //    }
                //    var datas = new
                //    {
                //        SecodTypeName = EnumHelper.GetEnumDescription(enums),
                //        data=tem
                //    };
                //    ss.Add(datas);
                //};               
                var temps = new
                {
                    type = ThemeEnum.System,
                    Name = EnumHelper.GetEnumDescription(ThemeEnum.System),
                    data = dic.ToArray()
                };
                List<object> result = new List<object>();
                result.Add(temps);
                if (Themes != null)
                {
                    return Json(result.ToArray());
                }
                else
                {
                    resultdata.msg = "未能找到主题";
                }
                return Json(resultdata);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        /// <summary>
        ///  Word主题内容
        /// </summary>
        /// <returns></returns>
        public JsonResult Theme()
        {
            RequestUser();
            if (us.ID != 0)
            {
                string message = string.Empty;
            }
            double sql = 18.46;
            string m = "主题不相同";
            char[] da = m.ToArray();
            double res = Math.Truncate(sql);
            double res1 = Math.Ceiling(sql);
            if (res.Equals(res1))
            {
                string message = "两次值相等";
                ;
            }
            else
            {
                string data = "值不相同,请重新输入";
            }
            Timer t = new Timer(10000);//实例化Timer类，设置间隔时间为10000毫秒;
            t.Elapsed += new ElapsedEventHandler(theout);//到达时间的时候执行事件;
            t.AutoReset = true;//设置是执行一次（false）还是一直执行(true);
            t.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件; 
            return Json(new { });
        }
        /// <summary>
        /// 定时器
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public void theout(object source, System.Timers.ElapsedEventArgs e)
        {
            Response.Write("OK");
        }
        public void GetAll()
        {
            DateTime stime = DateTime.Now;
            DateTime etime = DateTime.Now.AddDays(7);
            List<string> msg = null;
            //获取枚举的值
            int s = Convert.ToInt32(EnumHelper.ThemeEnum.System);
            var name = Enum.GetName(typeof(ThemeEnum), 1);
            var names = Enum.GetNames(typeof(ThemeEnum));
            var stringdata = EnumHelper.GetEnumDescription(ThemeEnum.System);
        }
        //public static async Task TransactionScopeAsync()
        //{
        //    using (var scope=new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        //    {
        //        Transaction.Current.TransactionCompleted += OnTransactionCompleted;
        //    }
        //}
        //public static void OnTransactionCompleted()
        //{
        //}
        /// <summary>
        /// 获取网页的指定标签的值
        /// </summary>
        public static void GetData()
        {
            #region 爬取网页HTML代码
            string Url = "https://www.baidu.com";
            WebClient wc = new WebClient();
            byte[] htmldata = wc.DownloadData(Url);
            string html = Encoding.UTF8.GetString(htmldata);
            #endregion
            #region 用正则匹配自己需要的标签
            Regex reg = new Regex(@"<a></a>");
            MatchCollection result = reg.Matches(html);
            List<string> list = new List<string>();
            foreach (Match temp in result)
            {
                list.Add(temp.Value);
            }
            #endregion
        }
    }
}