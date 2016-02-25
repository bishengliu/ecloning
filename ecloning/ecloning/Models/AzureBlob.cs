using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


using System.Data;
using System.Data.Entity;

using System.Net;

using System.Web.Mvc;
using ecloning.Models;


//for azure storage
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;


//connection string is located in the web.config
using System.Configuration;

using System.IO;
using System.Web.UI.WebControls;

namespace ecloning.Models
{
    public class AzureBlob
    {


        //public string blobName { get; set; }
        public string directoryName { get; set; }


        public void AzureBlobUpload(string FileName, HttpPostedFileBase file)
        {
        //for Azure storage
        // Retrieve storage account from connection string.
            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString;
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

        //A CloudBlobClient type allows you to retrieve objects that represent containers and blobs stored within the Blob Storage Service. 
        //The following code creates a CloudBlobClient object using the storage account object we retrieved above:
        // Create the blob client.
        CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

        // Retrieve a reference to a container.
        directoryName = this.directoryName;
        CloudBlobContainer container = blobClient.GetContainerReference(directoryName);

        // Create the container if it doesn't already exist.
        container.CreateIfNotExists();

        // Retrieve reference to a blob named "myblob".
        CloudBlockBlob blockBlob = container.GetBlockBlobReference(FileName);

        if (file != null && file.FileName != null && file.ContentLength > 0)
            {
                blockBlob.Properties.ContentType = file.ContentType;
                blockBlob.UploadFromStream(file.InputStream);
            }
        }


        public List<string> AzureBloblist()
        {
            //for Azure storage
            // Retrieve storage account from connection string.
            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString;
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            //A CloudBlobClient type allows you to retrieve objects that represent containers and blobs stored within the Blob Storage Service. 
            //The following code creates a CloudBlobClient object using the storage account object we retrieved above:
            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container.
            directoryName = this.directoryName;
            CloudBlobContainer container = blobClient.GetContainerReference(directoryName);

            // Create the container if it doesn't already exist.
            container.CreateIfNotExists();

            //name list
            var nameeList = new List<string>();
            // Loop over items within the container
            foreach (IListBlobItem item in container.ListBlobs(null, false))
            {
                if (item.GetType() == typeof(CloudBlockBlob)){
                    CloudBlockBlob blob = (CloudBlockBlob)item;
                    var name = blob.Name;
                    nameeList.Add(blob.Name);

                }
            }
            return nameeList;

        }

        public string AzureBlobUri(string FileName)
        {
            //for Azure storage
            // Retrieve storage account from connection string.
            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString;
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            //A CloudBlobClient type allows you to retrieve objects that represent containers and blobs stored within the Blob Storage Service. 
            //The following code creates a CloudBlobClient object using the storage account object we retrieved above:
            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container.
            directoryName = this.directoryName;
            CloudBlobContainer container = blobClient.GetContainerReference(directoryName);


            //Create the container if it doesn't already exist.
            container.CreateIfNotExists();



            // Retrieve reference to a blob named "myblob".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(FileName);
            try
            {
                blockBlob.FetchAttributes();
                return blockBlob.Uri.ToString();
            }
            catch
            {
                return "notFound";
            }
            //output the URI.
            //return blockBlob.Uri.ToString();


        }

        public void AzureBlobDownload(string FileName)
        {
            //for Azure storage
            // Retrieve storage account from connection string.
            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString;
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            //A CloudBlobClient type allows you to retrieve objects that represent containers and blobs stored within the Blob Storage Service. 
            //The following code creates a CloudBlobClient object using the storage account object we retrieved above:
            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container.
            directoryName = this.directoryName;
            CloudBlobContainer container = blobClient.GetContainerReference(directoryName);

            // Create the container if it doesn't already exist.
            container.CreateIfNotExists();

            // Retrieve reference to a blob named "myblob".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(FileName);
            
            //download
            using (var memoryStream = new MemoryStream())
            {
                memoryStream.Flush();
                memoryStream.Seek(0, SeekOrigin.Begin);
                if (blockBlob != null)
               blockBlob.DownloadToStream(memoryStream);

                memoryStream.Seek(0, SeekOrigin.Begin);
                byte[] byteArray = memoryStream.ToArray();
               
                memoryStream.Close();
                
                System.Web.HttpContext.Current.Response.Clear(); 
                System.Web.HttpContext.Current.Response.ContentType = blockBlob.Properties.ContentType;
                System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "Attachment; filename=" + FileName.ToString());
                System.Web.HttpContext.Current.Response.AddHeader("Content-Length", blockBlob.Properties.Length.ToString());
                //memoryStream.Seek(0, SeekOrigin.Begin);
                System.Web.HttpContext.Current.Response.BinaryWrite(byteArray);
                //memoryStream.Seek(0, SeekOrigin.Begin);
                System.Web.HttpContext.Current.Response.Flush();
                System.Web.HttpContext.Current.Response.End();

         
            }
        }


        public MemoryStream AzureBlobDownloadToStream(string FileName)
        {
            //for Azure storage
            // Retrieve storage account from connection string.
            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString;
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            //A CloudBlobClient type allows you to retrieve objects that represent containers and blobs stored within the Blob Storage Service. 
            //The following code creates a CloudBlobClient object using the storage account object we retrieved above:
            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container.
            directoryName = this.directoryName;
            CloudBlobContainer container = blobClient.GetContainerReference(directoryName);


            // Retrieve reference to a blob named "myblob".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(FileName);

            // Create the container if it doesn't already exist.
            container.CreateIfNotExists();

            //download
            var memoryStream = new MemoryStream();
            
                memoryStream.Flush();
                memoryStream.Seek(0, SeekOrigin.Begin);
                if (blockBlob != null)
                    blockBlob.DownloadToStream(memoryStream);

                memoryStream.Seek(0, SeekOrigin.Begin);
                return memoryStream;
            
        }


        public string AzureBlobDownloadType(string FileName)
        {
            //for Azure storage
            // Retrieve storage account from connection string.
            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString;
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            //A CloudBlobClient type allows you to retrieve objects that represent containers and blobs stored within the Blob Storage Service. 
            //The following code creates a CloudBlobClient object using the storage account object we retrieved above:
            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container.
            directoryName = this.directoryName;
            CloudBlobContainer container = blobClient.GetContainerReference(directoryName);

            // Create the container if it doesn't already exist.
            container.CreateIfNotExists();

            // Retrieve reference to a blob named "myblob".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(FileName);

            var nameArray = blockBlob.Name.Split('.');
            var fileType = nameArray[nameArray.Length - 1];
            return fileType;
            
        }

        public void AzureBlobDelete(string FileName)
        {
            //for Azure storage
            // Retrieve storage account from connection string.
            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString;
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            //A CloudBlobClient type allows you to retrieve objects that represent containers and blobs stored within the Blob Storage Service. 
            //The following code creates a CloudBlobClient object using the storage account object we retrieved above:
            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container.
            directoryName = this.directoryName;
            CloudBlobContainer container = blobClient.GetContainerReference(directoryName);

            // Create the container if it doesn't already exist.
            container.CreateIfNotExists();

            // Retrieve reference to a blob named "myblob".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(FileName);
            var Uri = AzureBlobUri(FileName);
            if (Uri != "notFound")
            {
                //delete
                blockBlob.Delete();
            }


        }
    }
}