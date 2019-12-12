using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChatWeb.Controllers
{
    public class UploadController : Controller
    {
        // GET: Upload
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 用户上传文件
        /// </summary>
        /// <returns></returns>        
        public string GetFile()
        {
            string img = string.Empty;
            try
            {
                var file = Request.Files[0];
                string fileName = file.FileName;
                string filepath = Path.Combine(Server.MapPath(string.Format("~/{0}", "Images")), fileName);
                file.SaveAs(filepath);
                img = Constant.files + "/Images/" + fileName;
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
            return img;
        }       
        /// <summary>
        /// 用户上传文件二进制数组
        /// </summary>
        /// <returns></returns>        
        public byte[] GetFiles()
        {
            Stream filestream = Request.Files[0].InputStream;
            byte[] bytes = new byte[filestream.Length];
            filestream.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始 
            filestream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }
    }
}