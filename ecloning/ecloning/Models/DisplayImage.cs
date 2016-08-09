using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ecloning.Models
{
    public class DisplayImage
    {
        public MemoryStream loadImage(string fileName)
        {
            MemoryStream memoryStream = new MemoryStream();
            //azure
            var azureBlob = new AzureBlob();
            if (ImageExtensionCheck.IsImage(fileName) == true)
            {
                if (eCloningSettings.AppHosting == "Cloud")
                {
                    //cloud
                    if (azureBlob.AzureBlobUri("tb-" + fileName) != "notFound")
                    {
                        memoryStream = azureBlob.AzureBlobDownloadToStream("tb-" + fileName);
                    }
                    else
                    {
                        //if the tb img not found, fall back to load original image
                        //check the exisitence of the orignal image
                        if (azureBlob.AzureBlobUri(fileName) != "notFound")
                        {
                            //put in the mStream
                            memoryStream = azureBlob.AzureBlobDownloadToStream(fileName);
                        }
                    }
                }
                else
                {
                    //local or hybrid
                    string path1 = System.Web.HttpContext.Current.Request.MapPath(ecloning.Models.eCloningSettings.filePath + ecloning.Models.eCloningSettings.expDataDir + "/tb-" + fileName);
                    string path2 = System.Web.HttpContext.Current.Request.MapPath(ecloning.Models.eCloningSettings.filePath + ecloning.Models.eCloningSettings.expDataDir + "/" + fileName);
                    if (System.IO.File.Exists(path1))
                    {
                        //tb
                        var file = System.IO.File.OpenRead(path1);
                        file.CopyTo(memoryStream);
                    }
                    else
                    {
                        //original img
                        var file = System.IO.File.OpenRead(path2);
                        file.CopyTo(memoryStream);
                    }
                }
            }
            return memoryStream;
        }
    }
}