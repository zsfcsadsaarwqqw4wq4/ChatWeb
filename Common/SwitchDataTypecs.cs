using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class SwitchDataTypecs
    {
        /// <summary>
        /// 将Stream转换成byte[]
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static byte[] StreamToBytes(Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes,0,bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }
        /// <summary>
        /// 将byte[]转换成stream
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static Stream BytesToStream(byte[] bytes)
        {
            Stream stream = new MemoryStream(bytes);
            return stream;
        } 
        /// <summary>
        /// 字节流转换成图片
        /// </summary>
        /// <returns></returns>
        public static Image ByteToImg(byte[] bytes)
        {
            try
            {
                MemoryStream ms = new MemoryStream(bytes);
                Image img =Image.FromStream(ms);
                return img;
            }
            catch(Exception ex)
            {
                return null;
            }
        }
        /// <summary>
        /// 图片转换成字节流
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static byte[] ImgToByte(Image img)
        {
            ImageConverter imgconv = new ImageConverter();
            byte[] b = (byte[])imgconv.ConvertTo(img, typeof(byte[]));
            return b;
        }
        /// <summary>
        /// 把图片Url转化成Image对象
        /// </summary>
        /// <param name="imageUrl"></param>
        /// <returns></returns>
        public static Image Url2Img(string imageUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(imageUrl))
                {
                    return null;
                }
                WebRequest webreq = WebRequest.Create(imageUrl);
                WebResponse webres = webreq.GetResponse();
                Stream stream = webres.GetResponseStream();
                Image image;
                image = Image.FromStream(stream);
                stream.Close();

                return image;
            }
            catch (Exception ex)
            {
            }
            return null;
        }
        /// <summary>
        /// byte[] 转换 Bitmap
        /// </summary>
        /// <param name="Bytes"></param>
        /// <returns></returns>
        public static Bitmap BytesToBitmap(byte[] Bytes)
        {
            MemoryStream stream = null;
            try
            {
                stream = new MemoryStream(Bytes);
                return new Bitmap((Image)new Bitmap(stream));
            }
            catch (ArgumentNullException ex)
            {
                throw ex;
            }
            catch (ArgumentException ex)
            {
                throw ex;
            }
            finally
            {
                stream.Close();
            }
        }
        /// <summary>
        /// Bitmap转byte[] 
        /// </summary>
        /// <param name="Bitmap"></param>
        /// <returns></returns>
        public static byte[] BitmapToBytes(Bitmap Bitmap)
        {
            MemoryStream ms = null;
            try
            {
                ms = new MemoryStream();
                Bitmap.Save(ms, Bitmap.RawFormat);
                byte[] byteImage = new Byte[ms.Length];
                byteImage = ms.ToArray();
                return byteImage;
            }
            catch (ArgumentNullException ex)
            {
                throw ex;
            }
            finally
            {
                ms.Close();
            }
        }
        /// <summary>  
        /// 从文件读取 Stream  
        /// </summary>  
        public Stream FileToStream(string fileName)
        {
            // 打开文件  
            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            // 读取文件的 byte[]  
            byte[] bytes = new byte[fileStream.Length];
            fileStream.Read(bytes, 0, bytes.Length);
            fileStream.Close();
            // 把 byte[] 转换成 Stream  
            Stream stream = new MemoryStream(bytes);
            return stream;
        }
    }
}
