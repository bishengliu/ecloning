using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.IO;

//for azure storage
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;


//connection string is located in the web.config
using System.Configuration;

namespace ecloning.Models
{
    public class FileTransfer
    {
        public HttpPostedFileBase file { get; set; }
        public string filePath { get; set; }
        string input_name { get; set; }

        public void Upload(HttpPostedFileBase file, string path)
        {
            //file must be pasted as Request.Files["xxx"];
            //file must be checked for file type;
            this.file = file;
            filePath = path;

            if (file != null && file.FileName != null && file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var savePath = Path.Combine(HttpContext.Current.Server.MapPath(path), fileName);
                file.SaveAs(path);
            }
        }


        //public FileResult Download(string path, string filename)
        //{
        //    byte[] fileBytes = System.IO.File.ReadAllBytes(path + "/" + filename);
        //    string fileName = filename;
        //    return Controller.File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        //}

    }
}