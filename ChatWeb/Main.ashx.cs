using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatWeb
{
    /// <summary>
    /// Main 的摘要说明
    /// </summary>
    public class Main : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            using (var reader = new System.IO.StreamReader(context.Request.InputStream))
            {
                String xmlData = reader.ReadToEnd();
                context.Response.Write(xmlData);              
            }
        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}