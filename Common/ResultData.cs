using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class ResultData
    {
        public ResultData()
        {
            res = 500;
            msg = "请稍后在尝试";
        }
        /// <summary>
        /// 状态码 200 成功 500 失败
        /// </summary>
        public int res { get; set; }
        /// <summary>
        /// 提示信息
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 返回的数据结果
        /// </summary>
        public object data{ get; set; }
    }
}
