using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ecloning.Models
{
    public static class ImageFormat
    {
        public static System.Drawing.Imaging.ImageFormat GetImageFormat(this System.Drawing.Image img)
        {
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Jpeg))
                return System.Drawing.Imaging.ImageFormat.Jpeg;
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Bmp))
                return System.Drawing.Imaging.ImageFormat.Bmp;
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Png))
                return System.Drawing.Imaging.ImageFormat.Png;
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Emf))
                return System.Drawing.Imaging.ImageFormat.Emf;
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Exif))
                return System.Drawing.Imaging.ImageFormat.Exif;
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Gif))
                return System.Drawing.Imaging.ImageFormat.Gif;
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Icon))
                return System.Drawing.Imaging.ImageFormat.Icon;
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.MemoryBmp))
                return System.Drawing.Imaging.ImageFormat.MemoryBmp;
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Tiff))
                return System.Drawing.Imaging.ImageFormat.Tiff;
            else
                return System.Drawing.Imaging.ImageFormat.Wmf;
        }

        public static System.Drawing.Imaging.ImageFormat GetImageFormatFromFile(string filename)
        {
            var arrayFile = filename.Split('.');
            var extension = arrayFile[arrayFile.Length - 1];
            if (extension.ToLower() == "jpg")
            {
                return System.Drawing.Imaging.ImageFormat.Jpeg;
            }
            else if (extension.ToLower() == "jpeg")
            {
                return System.Drawing.Imaging.ImageFormat.Jpeg;
            }
            else if (extension.ToLower() == "png")
            {
                return System.Drawing.Imaging.ImageFormat.Png;
            }
            else if (extension.ToLower() == "tif")
            {
                return System.Drawing.Imaging.ImageFormat.Tiff;
            }
            else if (extension.ToLower() == "tiff")
            {
                return System.Drawing.Imaging.ImageFormat.Tiff;
            }
            else
            {
                return System.Drawing.Imaging.ImageFormat.Bmp;
            }

        }

    }
}