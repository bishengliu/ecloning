using ecloning.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace ecloning.Controllers
{
    public class ProtocolController : RootController
    {
        private ecloningEntities db = new ecloningEntities();
        // GET: Protocol
        [Authorize]
        public ActionResult Index()
        {
            //get userId
            var userId = User.Identity.GetUserId();
            var userInfo = new UserInfo(userId);
            ViewBag.PersonId = userInfo.PersonId;
            var groupInfo = new GroupInfo(userInfo.PersonId);
            //get all the peopleId in the group
            var groupPeopleIds = groupInfo.groupPeopleId;

            //pass data to check whether it is allowed to delte and add new version
            //var versions = (from p in db.protocols where groupPeopleIds.Contains((int)p.people_id) group p by new { p.type_id } into g select new { version = g.Count(), typeId = g.Key.type_id }).ToList();
            List<ProtocolVersion> versions = new List<ProtocolVersion>();
            var groups = db.protocols.Where(p => groupPeopleIds.Contains((int)p.people_id)).Where(t => t.type_id != null).GroupBy(t => t.type_id).ToList();
            foreach(var g in groups)
            {
                var latestVersion = g.OrderByDescending(v => v.version).FirstOrDefault();
                var pv = new ProtocolVersion();
                pv.version  = (int)latestVersion.version;
                pv.typeId = latestVersion.type_id;
                versions.Add(pv);
            }
            ViewBag.Versions = versions;
            //show all the protocols in the group
            var protocols = db.protocols.Where(p => groupPeopleIds.Contains((int)p.people_id)).OrderBy(p=>p.name).OrderByDescending(t=>t.type_id).OrderByDescending(v=>v.version);
            return View(protocols.ToList());
        }

        [Authorize]
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include ="id,name,upload,des")] ProtocolViewModel protocol)
        {            
            //get userId
            var userId = User.Identity.GetUserId();
            var userInfo = new UserInfo(userId);
            var groupInfo = new GroupInfo(userInfo.PersonId);
            //get all the peopleId in the group
            var groupPeopleIds = groupInfo.groupPeopleId;
            string fileName = null;
            if (ModelState.IsValid)
            {
                //start transction
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        //upload protocol
                        HttpPostedFileBase file = null;
                        file = Request.Files["pro_fn"];
                        if (eCloningSettings.AppHosting == "Cloud")
                        {
                            //upload to azure
                            if (file != null && file.FileName != null && file.ContentLength > 0)
                            {
                                try
                                {
                                    fileName = "version1" + "$" + Path.GetFileName(file.FileName);
                                    AzureBlob azureBlob = new AzureBlob();
                                    azureBlob.directoryName = eCloningSettings.protocolDir;
                                    azureBlob.AzureBlobUpload(fileName, file);
                                }
                                catch (Exception)
                                {
                                    ModelState.AddModelError("", "File upload failed!");
                                    return View(protocol);
                                }
                            }
                        }
                        else
                        {
                            //upload to local plasmid folder
                            var protocolPath = eCloningSettings.filePath + eCloningSettings.protocolDir;
                            if (file != null && file.FileName != null && file.ContentLength > 0)
                            {
                                try
                                {
                                    fileName = "version1" + "$" + Path.GetFileName(file.FileName);
                                    //fileExtension = Path.GetExtension(file.FileName);
                                    var path = Path.Combine(Server.MapPath(protocolPath), fileName);
                                    file.SaveAs(path);
                                }
                                catch (Exception)
                                {
                                    ModelState.AddModelError("", "File upload failed!");
                                    return View(protocol);
                                }
                            }
                        }


                        //this is a new protocol, version is 1                        
                        var pt = new protocol();
                        pt.upload = fileName;
                        pt.version = 1;
                        pt.versionref = null;
                        pt.name = protocol.name;
                        pt.des = protocol.des;
                        pt.dt = DateTime.Now;
                        pt.people_id = userInfo.PersonId;

                        //get type id
                        int? type_id = 1;
                        var protocols = db.protocols.Where(p => groupPeopleIds.Contains((int)p.people_id));
                        if (protocols.Count() > 0)
                        {
                            type_id = protocols.OrderByDescending(t => t.type_id).FirstOrDefault().type_id + 1;
                        }
                        pt.type_id = type_id;
                        db.protocols.Add(pt);
                        db.SaveChanges();


                        scope.Complete();
                        TempData["msg"] = "Protocol added!";
                        return RedirectToAction("Index", "Protocol");
                    }
                    catch (Exception)
                    {
                        scope.Dispose();
                        //remove the upload file if exists
                        if (eCloningSettings.AppHosting == "Cloud")
                        {
                            //delete from azure
                            AzureBlob azureBlob = new AzureBlob();
                            azureBlob.directoryName = eCloningSettings.protocolDir;
                            azureBlob.AzureBlobDelete(fileName);
                        }
                        else
                        {
                            //delete from local
                            string path = Request.MapPath(eCloningSettings.filePath + eCloningSettings.protocolDir + "/" + fileName);
                            if (System.IO.File.Exists(path))
                            {
                                System.IO.File.Delete(path);
                            }
                        }

                        ModelState.AddModelError("", "Something went wrong, please try again!");
                        return View(protocol);
                    }
                }
                    
            }
                return View(protocol);
        }

        [Authorize]
        [HttpGet]
        public ActionResult NewVersion(int? id, int? version)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            protocol protocol = db.protocols.Find(id);
            if (protocol == null)
            {
                return HttpNotFound();
            }
            ViewBag.LinkedProtocol = protocol;
            return View();
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewVersion(int? linked_id, [Bind(Include = "id,name,upload,des")] ProtocolViewModel protocol)
        {
            if(linked_id == null)
            {
                ModelState.AddModelError("", "Something went wrong, please try again!");
                return View(protocol);
            }
            //get userId
            var userId = User.Identity.GetUserId();
            var userInfo = new UserInfo(userId);
            var groupInfo = new GroupInfo(userInfo.PersonId);
            //get all the peopleId in the group
            var groupPeopleIds = groupInfo.groupPeopleId;
            string fileName = null;
            if (ModelState.IsValid)
            {
                //start transction
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        //find the linked protocol
                        var linkedProtocol = db.protocols.Find(linked_id);
                        var newVersion = linkedProtocol.version + 1;
                        //upload protocol
                        HttpPostedFileBase file = null;
                        file = Request.Files["pro_fn"];
                        if (eCloningSettings.AppHosting == "Cloud")
                        {
                            //upload to azure
                            if (file != null && file.FileName != null && file.ContentLength > 0)
                            {
                                try
                                {
                                    fileName = "version"+ newVersion + "$" + Path.GetFileName(file.FileName);
                                    AzureBlob azureBlob = new AzureBlob();
                                    azureBlob.directoryName = eCloningSettings.protocolDir;
                                    azureBlob.AzureBlobUpload(fileName, file);
                                }
                                catch (Exception)
                                {
                                    ModelState.AddModelError("", "File upload failed!");
                                    return View(protocol);
                                }
                            }
                        }
                        else
                        {
                            //upload to local plasmid folder
                            var protocolPath = eCloningSettings.filePath + eCloningSettings.protocolDir;
                            if (file != null && file.FileName != null && file.ContentLength > 0)
                            {
                                try
                                {
                                    fileName = "version" + newVersion + "$" + Path.GetFileName(file.FileName);
                                    //fileExtension = Path.GetExtension(file.FileName);
                                    var path = Path.Combine(Server.MapPath(protocolPath), fileName);
                                    file.SaveAs(path);
                                }
                                catch (Exception)
                                {
                                    ModelState.AddModelError("", "File upload failed!");
                                    return View(protocol);
                                }
                            }
                        }


                        //this is a new protocol, version is 1                        
                        var pt = new protocol();
                        pt.upload = fileName;
                        pt.version = newVersion;
                        pt.versionref = null;
                        pt.name = protocol.name;
                        pt.des = protocol.des;
                        pt.dt = DateTime.Now;
                        pt.people_id = userInfo.PersonId;
                        pt.type_id = linkedProtocol.type_id;
                        db.protocols.Add(pt);
                        db.SaveChanges();


                        scope.Complete();
                        TempData["msg"] = "Protocol added!";
                        return RedirectToAction("Index", "Protocol");
                    }
                    catch (Exception)
                    {
                        scope.Dispose();
                        //remove the upload file if exists
                        if (eCloningSettings.AppHosting == "Cloud")
                        {
                            //delete from azure
                            AzureBlob azureBlob = new AzureBlob();
                            azureBlob.directoryName = eCloningSettings.protocolDir;
                            azureBlob.AzureBlobDelete(fileName);
                        }
                        else
                        {
                            //delete from local
                            string path = Request.MapPath(eCloningSettings.filePath + eCloningSettings.protocolDir + "/" + fileName);
                            if (System.IO.File.Exists(path))
                            {
                                System.IO.File.Delete(path);
                            }
                        }

                        ModelState.AddModelError("", "Something went wrong, please try again!");
                        return View(protocol);
                    }
                }

            }
            return View(protocol);
        }


        [Authorize]
        public ActionResult Download(string fileName, string actionName)
        {
            if (eCloningSettings.AppHosting == "Cloud")
            {
                //download from azure
                AzureBlob azureBlob = new AzureBlob();
                azureBlob.directoryName = eCloningSettings.protocolDir;
                try
                {
                    if (azureBlob.AzureBlobUri(fileName) == "notFound")
                    {
                        return HttpNotFound();
                    }
                    else
                    {
                        azureBlob.AzureBlobDownload(fileName);
                    }
                }
                catch (Exception)
                {
                    return RedirectToAction("FileError");
                }
            }
            else
            {
                //download from local
                try
                {
                    var protocolPath = eCloningSettings.filePath + eCloningSettings.protocolDir;
                    var path = Path.Combine(Server.MapPath(protocolPath), fileName);
                    byte[] fileBytes = System.IO.File.ReadAllBytes(path);
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }
                catch (Exception)
                {
                    return RedirectToAction("FileError");
                }
            }
            return RedirectToAction(actionName);
        }

        [Authorize]
        [HttpGet]
        public ActionResult FileError()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            protocol protocol = db.protocols.Find(id);
            if (protocol == null)
            {
                return HttpNotFound();
            }
            //only allow the latest version to be deleted
            //get userId
            var userId = User.Identity.GetUserId();
            var userInfo = new UserInfo(userId);
            var groupInfo = new GroupInfo(userInfo.PersonId);
            //get all the peopleId in the group
            var groupPeopleIds = groupInfo.groupPeopleId;
            int? latestVersion = db.protocols.Where(p => groupPeopleIds.Contains((int)p.people_id) && p.type_id==protocol.type_id).OrderByDescending(p=>p.version).FirstOrDefault().version;
            bool isAllowed = protocol.version== latestVersion?true: false;
            ViewBag.isAllowed = isAllowed;
            return View(protocol);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            protocol protocol = db.protocols.Find(id);
            if (protocol == null)
            {
                return HttpNotFound();
            }
            //only allow the latest version to be deleted
            //get userId
            var userId = User.Identity.GetUserId();
            var userInfo = new UserInfo(userId);
            var groupInfo = new GroupInfo(userInfo.PersonId);
            //get all the peopleId in the group
            var groupPeopleIds = groupInfo.groupPeopleId;
            int? latestVersion = db.protocols.Where(p => groupPeopleIds.Contains((int)p.people_id) && p.type_id == protocol.type_id).OrderByDescending(p => p.version).FirstOrDefault().version;
            bool isAllowed = protocol.version == latestVersion ? true : false;
            if (isAllowed)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        db.protocols.Remove(protocol);
                        db.SaveChanges();
                        scope.Complete();
                        //remove the upload file if exists
                        if (eCloningSettings.AppHosting == "Cloud")
                        {
                            //delete from azure
                            AzureBlob azureBlob = new AzureBlob();
                            azureBlob.directoryName = eCloningSettings.protocolDir;
                            azureBlob.AzureBlobDelete(protocol.upload);
                        }
                        else
                        {
                            //delete from local
                            string path = Request.MapPath(eCloningSettings.filePath + eCloningSettings.protocolDir + "/" + protocol.upload);
                            if (System.IO.File.Exists(path))
                            {
                                System.IO.File.Delete(path);
                            }
                        }
                        return RedirectToAction("Index");
                    }
                    catch (Exception)
                    {
                        scope.Dispose();
                        return RedirectToAction("Index");
                    }
                }                
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }

}