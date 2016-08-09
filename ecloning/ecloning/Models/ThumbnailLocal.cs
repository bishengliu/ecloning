using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace ecloning.Models
{
    public class ThumbnailLocal
    {
        public void SaveLocal(string savePath, string fileName, string KeyName)
        {
            HttpPostedFile file = System.Web.HttpContext.Current.Request.Files[KeyName];
            if (file.ContentLength != 0)
            {
                //copy file into memStream

                Image thumbNail = null;
                using (Image image = Image.FromStream(file.InputStream, true, false))
                {
                    //var ratio = image.Height / image.Width;
                    //var ratio = 1;
                    using (thumbNail = image.GetThumbnailImage(100, 100, () => false, IntPtr.Zero))
                    {
                        //get image format
                        System.Drawing.Imaging.ImageFormat format = ImageFormat.GetImageFormatFromFile(fileName);
                        fileName = "/tb-" + fileName;
                        //var path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(savePath), fileName);
                        var path = System.Web.HttpContext.Current.Server.MapPath(savePath + "/" + fileName);
                        thumbNail.Save(path, format);
                    }
                }
            }
        }
    }
}
