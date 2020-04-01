using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// 图片处理类
    /// 1.生成略缩图或按照比例改变图片的大小和画质
    /// </summary>
    public class ImageClass
    {
        public Image ResourceImage;
        public string ErrMessage;
        /// <summary>
        /// 构造函数
        /// </summary>
        public ImageClass(string ImageFileName)
        {
            ResourceImage = Image.FromFile(ImageFileName);
            ErrMessage = "";
        }
        public bool ThumbnailCallback()
        {
            return false;
        }
        /// <summary>
        /// 生成缩略图重载方法，返回缩略图的Image对象
        /// </summary>
        /// <param name="Width">缩略图的宽度</param>
        /// <param name="Height">缩略图的高度</param>
        /// <returns></returns>
        public Image GetReducedImage(int Width,int Height)
        {
            try
            {
                Image.GetThumbnailImageAbort callb = new Image.GetThumbnailImageAbort(ThumbnailCallback);
                ResourceImage = ResourceImage.GetThumbnailImage(Width, Height, callb, IntPtr.Zero);
                return ResourceImage;
            }
            catch(Exception ex)
            {
                ErrMessage = ex.Message;
                return null;
            }
        }
        /// <summary>
        /// 生成缩略图重载方法，将缩略图文件保存到指定的路径
        /// </summary>
        /// <param name="Width">缩略图的宽度</param>
        /// <param name="Height">缩略图的高度</param>
        /// <param name="targetFilePath"></param>
        /// <returns></returns>
        public bool GetReducedImage(int Width, int Height, string targetFilePath)
        {
            try
            {
                Image.GetThumbnailImageAbort callb = new Image.GetThumbnailImageAbort(ThumbnailCallback);
                ResourceImage = ResourceImage.GetThumbnailImage(Width, Height, callb, IntPtr.Zero);
                ResourceImage.Save(targetFilePath);
                ResourceImage.Dispose();
                return true;
            }
            catch(Exception ex)
            {
                ErrMessage = ex.Message;
                return false;
            }
        }

    }
}
