using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ecloning.Models
{
    public class ImageExtensionCheck
    {
        public static bool IsImage(string filename)
        {
            if (filename.IndexOf('.') != -1)
            {
                var arrayFile = filename.Split('.');
                var extension = arrayFile[arrayFile.Length - 1];
                if (extension.ToLower() == "jpg")
                {
                    return true;
                }
                else if (extension.ToLower() == "jpeg")
                {
                    return true;
                }
                else if (extension.ToLower() == "png")
                {
                    return true;
                }
                else if (extension.ToLower() == "tif")
                {
                    return true;
                }
                else if (extension.ToLower() == "tiff")
                {
                    return true;
                }
                else if (extension.ToLower() == "bmp")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}