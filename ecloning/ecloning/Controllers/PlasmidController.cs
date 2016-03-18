using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ecloning.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System.IO;
using System.Text.RegularExpressions;

namespace ecloning.Controllers
{
    public class PlasmidController : RootController
    {
        private ecloningEntities db = new ecloningEntities();

        // GET: Plasmid
        [Authorize]
        public ActionResult Index()
        {
            //get current login email
            var email = User.Identity.GetUserName();
            //find group shared plasmid
            List<int> sharedPlasmidId = new List<int>();
            IQueryable<int> sharePlasmids = null;
            //get my people_id
            int peopleId = 0;
            var people = db.people.Where(e => e.email == email);
            if(people.Count() == 0)
            {
                //no group plasmid to show
                ViewBag.GroupCount =0;
            }
            else
            {
                peopleId = people.FirstOrDefault().id;
            }
            //get the group id
            List<int> groupId = new List<int>();
            var group_people = db.group_people.Where(p => p.people_id == peopleId);
            if (group_people.Count() == 0)
            {
                //no group plasmid to show
                ViewBag.GroupCount = 0;
            }
            else
            {
                foreach(int i in group_people.Select(g => g.group_id).ToList())
                {
                    groupId.Add(i);
                }
            }
            if (groupId.Count() > 0)
            {

                //get the shared plasmid id
                sharePlasmids = db.group_shared.Where(g => groupId.Contains(g.group_id)).Where(c => c.category == "plasmid").Select(r => r.resource_id);
                if(sharePlasmids.Count() > 0)
                {
                    //show group plasmid
                    ViewBag.GroupCount = groupId.Count();
                    //all the plasmid shared
                    sharedPlasmidId = sharePlasmids.ToList();
                    //get share plasmid for each group
                    int tag = 1;
                    foreach (int g in groupId)
                    {
                        ViewData["groupName" + tag] = db.groups.Find(g).name;
                        var sharedId = db.group_shared.Where(gp => gp.group_id == g).Where(c => c.category == "plasmid").Select(r => r.resource_id);
                        ViewData["sharePlasmid" + tag] = db.plasmids.Include(p => p.person).Where(p => sharedId.Contains(p.id));
                        tag++;
                    }
                }
                else
                {
                    //no group plasmid to show
                    ViewBag.GroupCount = 0;
                }
            }


            //only show my plasmids that are not shared with any group            
            IQueryable<plasmid> plasmids = null;
            if (sharedPlasmidId.Count() > 0)
            {
                plasmids = db.plasmids.Include(p => p.person).Where(p => p.person.email == email).Where(p => !sharedPlasmidId.Contains(p.id));            
            }
            else
            {
                plasmids = db.plasmids.Include(p => p.person).Where(p => p.person.email == email);
            }
            ViewBag.Count = plasmids.Count();
            return View(plasmids.ToList());
        }


        // GET: Plasmid/Create
        [Authorize]
        public ActionResult Create()
        {
            //get current login email
            var email = User.Identity.GetUserName();
            //get people_id
            var researcher = db.people.Where(e => e.email == email);
            int people_id = 0;
            if (researcher.Count() > 0)
            {
                people_id = researcher.FirstOrDefault().id;
            }
            if(people_id == 0)
            {
                ViewBag.people_id = new SelectList(db.people.Select(p => new { id = p.id, name = p.first_name + " " + p.last_name }), "id", "name");
            }
            else
            {
                ViewBag.people_id = new SelectList(db.people.Select(p => new { id = p.id, name = p.first_name + " " + p.last_name }), "id", "name",people_id);
            }
            ViewBag.expression_system = new SelectList(db.dropdownitems.Where(c => c.category == "ExpSystem").OrderBy(g => g.text), "text", "value");
            ViewBag.resistance = new SelectList(db.dropdownitems.Where(c => c.category == "Resistance").OrderBy(g => g.text), "text", "value");
            ViewBag.selection = new SelectList(db.dropdownitems.Where(c => c.category == "SelectMarker").OrderBy(g => g.text), "text", "value");
            ViewBag.insert_species = new SelectList(db.dropdownitems.Where(c => c.category == "InsertSpecies").OrderBy(g => g.text), "text", "value");
            ViewBag.usage = new SelectList(db.dropdownitems.Where(c => c.category == "PlasmidUse").OrderBy(g => g.text), "text", "value");
            ViewBag.plasmid_type = new SelectList(db.dropdownitems.Where(c => c.category == "PlasmidType").OrderBy(g => g.text), "text", "value");
            ViewBag.promotor = new SelectList(db.dropdownitems.Where(c => c.category == "Promotor").OrderBy(g => g.text), "text", "value");
            ViewBag.polyA = new SelectList(db.dropdownitems.Where(c => c.category == "PolyA").OrderBy(g => g.text), "text", "value");
            ViewBag.reporter = new SelectList(db.dropdownitems.Where(c => c.category == "Reporter").OrderBy(g => g.text), "text", "value");
            

            //find all plasmid
            var plasmids = db.plasmids.OrderBy(n => n.name).Select(p => new { id = p.id, name = p.name, usage = p.usage });
            ViewBag.JsonPlasmid = JsonConvert.SerializeObject(plasmids.ToList());

            return View();
        }

        // POST: Plasmid/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,sequence,seq_length,expression_system,expression_subsystem,promotor,polyA,resistance,reporter,selection,insert,insert_species,usage,plasmid_type,ref_plasmid,img_fn,addgene,d,people_id,des")] PlasmidViewModel plasmid)
        {
            //find all plasmid
            var plasmids = db.plasmids.OrderBy(n => n.name).Select(p => new { id = p.id, name = p.name, usage = p.usage });
            ViewBag.JsonPlasmid = JsonConvert.SerializeObject(plasmids.ToList());

            ViewBag.people_id = new SelectList(db.people.Select(p => new { id = p.id, name = p.first_name + " " + p.last_name }), "id", "name",plasmid.people_id);
            ViewBag.expression_system = new SelectList(db.dropdownitems.Where(c => c.category == "ExpSystem").OrderBy(g => g.text), "text", "value",plasmid.expression_system);
            ViewBag.insert_species = new SelectList(db.dropdownitems.Where(c => c.category == "InsertSpecies").OrderBy(g => g.text), "text", "value",plasmid.insert_species);
            ViewBag.plasmid_type = new SelectList(db.dropdownitems.Where(c => c.category == "PlasmidType").OrderBy(g => g.text), "text", "value",plasmid.plasmid_type);
            ViewBag.promotor = new SelectList(db.dropdownitems.Where(c => c.category == "Promotor").OrderBy(g => g.text), "text", "value",plasmid.promotor);
            ViewBag.polyA = new SelectList(db.dropdownitems.Where(c => c.category == "PolyA").OrderBy(g => g.text), "text", "value",plasmid.polyA);
            

            if (ModelState.IsValid)
            {
                plasmid.d = DateTime.Now;
                //process mutiple selection into string

                //db.plasmids.Add(plasmid);
                string Resistance = null;
                string Reporter = null;
                string Selection = null;
                string Usage = null;
                if (plasmid.resistance != null && plasmid.resistance.Count() > 1)
                {
                    Resistance = string.Join(",", plasmid.resistance);
                }
                else if (plasmid.resistance != null && plasmid.resistance.Count() == 1)
                {
                    Resistance = plasmid.resistance[0];
                }
                else
                {
                    Resistance = null;
                }

                if (plasmid.reporter != null && plasmid.reporter.Count() > 1)
                {
                    Reporter = string.Join(",", plasmid.reporter);
                }
                else if (plasmid.reporter != null && plasmid.reporter.Count() == 1)
                {
                    Reporter = plasmid.reporter[0];
                }
                else
                {
                    Reporter = null;
                }

                if (plasmid.selection != null && plasmid.selection.Count() > 1)
                {
                    Selection = string.Join(",", plasmid.selection);
                }
                else if (plasmid.selection != null && plasmid.selection.Count() == 1)
                {
                    Selection = plasmid.selection[0];
                }
                else
                {
                    Selection = null;
                }

                if (plasmid.usage != null && plasmid.usage.Count() > 1)
                {
                    Usage = string.Join(",", plasmid.usage);
                }
                else if (plasmid.usage != null && plasmid.usage.Count() == 1)
                {
                    Usage = plasmid.usage[0];
                }
                else
                {
                    Usage = null;
                }

                //new instance of plasmid model
                var Plasmid = new plasmid();
                Plasmid.name = plasmid.name;
                Plasmid.sequence = Regex.Replace(plasmid.sequence.Trim(), @"[^\u0000-\u007F]", string.Empty);
                Plasmid.seq_length = plasmid.seq_length;
                Plasmid.expression_subsystem = plasmid.expression_subsystem;
                Plasmid.expression_system = plasmid.expression_system;
                Plasmid.promotor = plasmid.promotor;
                Plasmid.polyA = plasmid.polyA;
                Plasmid.resistance = Resistance;
                Plasmid.reporter = Reporter;
                Plasmid.selection = Selection;
                Plasmid.insert = plasmid.insert;
                Plasmid.usage = Usage;
                Plasmid.plasmid_type = plasmid.plasmid_type;
                Plasmid.ref_plasmid = plasmid.ref_plasmid;
                Plasmid.addgene = plasmid.addgene;
                Plasmid.d = plasmid.d;
                Plasmid.people_id = plasmid.people_id;               
                Plasmid.des = plasmid.des;
                Plasmid.insert_species = plasmid.insert_species;


                var timeStamp = DateTime.Now.Millisecond.ToString();
                string fileName = null;

                if (plasmid.sequence != null)
                {
                    //auto generate features
                    var autoFeatures = new PlasmidFeature(plasmid.id, plasmid.sequence);
                }

                //upload img file
                HttpPostedFileBase file = null;
                file = Request.Files["img_fn"];

                if (eCloningSettings.AppHosting == "Cloud")
                {
                    //upload to azure
                    if (file != null && file.FileName != null && file.ContentLength > 0)
                    {
                        try
                        {
                            fileName = timeStamp + Path.GetFileName(file.FileName);
                            AzureBlob azureBlob = new AzureBlob();
                            azureBlob.directoryName = eCloningSettings.plasmidDir;
                            azureBlob.AzureBlobUpload(fileName, file);
                        }
                        catch (Exception)
                        {
                            ModelState.AddModelError("", "File upload failed!");
                            return View(plasmid);
                        }                        
                    }
                }
                else
                {
                    //upload to local plasmid folder
                    var plasmidPath =eCloningSettings.filePath + eCloningSettings.plasmidDir;
                    if (file != null && file.FileName != null && file.ContentLength > 0)
                    {
                        try
                        {
                            fileName = timeStamp + Path.GetFileName(file.FileName);
                            //fileExtension = Path.GetExtension(file.FileName);
                            var path = Path.Combine(Server.MapPath(plasmidPath), fileName);
                            file.SaveAs(path);
                        }
                        catch (Exception)
                        {
                            ModelState.AddModelError("", "File upload failed!");
                            return View(plasmid);
                        }
                    }
                }

                //save fileName to database
                Plasmid.img_fn = fileName;
                db.plasmids.Add(Plasmid);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(plasmid);
        }

        [Authorize]
        public ActionResult Download(string fileName)
        {
            if(eCloningSettings.AppHosting == "Cloud")
            {
                //download from azure
                AzureBlob azureBlob = new AzureBlob();
                azureBlob.directoryName = eCloningSettings.plasmidDir;
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
                    var plasmidPath = eCloningSettings.filePath + eCloningSettings.plasmidDir;
                    var path = Path.Combine(Server.MapPath(plasmidPath), fileName);
                    byte[] fileBytes = System.IO.File.ReadAllBytes(path);
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }
                catch(Exception)
                {
                    return RedirectToAction("FileError");
                }
            }
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpGet]
        public ActionResult FileError()
        {
            return View();
        }
        public ActionResult FileNotFound()
        {
            return View();
        }

        [Authorize]
        // GET: Plasmid/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            plasmid plasmid = db.plasmids.Find(id);
            if (plasmid == null)
            {
                return HttpNotFound();
            }


            var Plasmid = new ecloning.Models.PlasmidViewModel();
            //prepare arrays

            //resistance
            Plasmid.resistance = null;
            if (plasmid.resistance != null && !plasmid.resistance.Contains(","))
            {
                //only have one resistance
                string[] Resistance = new string[1];
                Resistance[0] = plasmid.resistance;
                Plasmid.resistance = Resistance;
            }
            if (plasmid.resistance != null && plasmid.resistance.Contains(","))
            {
                //more than one resistances
                string[] Resistance = plasmid.resistance.Split(',');
                Plasmid.resistance = Resistance;
            }

            //selection
            Plasmid.selection = null;
            if (plasmid.selection != null && !plasmid.selection.Contains(","))
            {
                //only have one selection
                string[] Selection = new string[1];
                Selection[0] = plasmid.selection;
                Plasmid.selection = Selection;
            }

            if (plasmid.selection != null && plasmid.selection.Contains(","))
            {
                //more than one selections
                string[] Selection = plasmid.selection.Split(',');
                Plasmid.selection = Selection;
            }


            //reporter
            Plasmid.reporter = null;
            if (plasmid.reporter != null && !plasmid.reporter.Contains(","))
            {
                //only have one reporter
                string[] Reporter = new string[1];
                Reporter[0] = plasmid.reporter;
                Plasmid.reporter = Reporter;
            }
            if (plasmid.reporter != null && plasmid.reporter.Contains(","))
            {
                //more than one reporters
                string[] Reporter = plasmid.reporter.Split(',');
                Plasmid.reporter = Reporter;
            }

            //usage
            Plasmid.usage = null;
            if (plasmid.usage != null && !plasmid.usage.Contains(","))
            {
                //only have one usage
                string[] Usage = new string[1];
                Usage[0] = plasmid.usage;
                Plasmid.usage = Usage;
            }
            if (plasmid.usage != null && plasmid.usage.Contains(","))
            {
                //more than one usages
                string[] Usage = plasmid.usage.Split(',');
                Plasmid.usage = Usage;
            }


            //pass all rest the data to viewmodal
            Plasmid.name = plasmid.name;
            Plasmid.sequence = plasmid.sequence;
            Plasmid.expression_subsystem = plasmid.expression_subsystem;
            Plasmid.expression_system = plasmid.expression_system;
            Plasmid.promotor = plasmid.promotor;
            Plasmid.polyA = plasmid.polyA;
            Plasmid.insert = plasmid.insert;
            Plasmid.plasmid_type = plasmid.plasmid_type;
            Plasmid.ref_plasmid = plasmid.ref_plasmid;
            Plasmid.addgene = plasmid.addgene;
            Plasmid.d = plasmid.d;
            Plasmid.people_id = plasmid.people_id;
            Plasmid.des = plasmid.des;
            Plasmid.insert_species = plasmid.insert_species;
            Plasmid.img_fn = plasmid.img_fn;
            Plasmid.seq_length = (int)plasmid.seq_length;



            //find all plasmid
            var plasmids = db.plasmids.OrderBy(n => n.name).Select(p => new { id = p.id, name = p.name, usage = p.usage });
            ViewBag.JsonPlasmid = JsonConvert.SerializeObject(plasmids.ToList());

            ViewBag.people_id = new SelectList(db.people.Select(p => new { id = p.id, name = p.first_name + " " + p.last_name }), "id", "name", plasmid.people_id);
            ViewBag.expression_system = new SelectList(db.dropdownitems.Where(c => c.category == "ExpSystem").OrderBy(g => g.text), "text", "value", plasmid.expression_system);
            ViewBag.insert_species = new SelectList(db.dropdownitems.Where(c => c.category == "InsertSpecies").OrderBy(g => g.text), "text", "value", plasmid.insert_species);
            ViewBag.plasmid_type = new SelectList(db.dropdownitems.Where(c => c.category == "PlasmidType").OrderBy(g => g.text), "text", "value", plasmid.plasmid_type);
            ViewBag.promotor = new SelectList(db.dropdownitems.Where(c => c.category == "Promotor").OrderBy(g => g.text), "text", "value", plasmid.promotor);
            ViewBag.polyA = new SelectList(db.dropdownitems.Where(c => c.category == "PolyA").OrderBy(g => g.text), "text", "value", plasmid.polyA);
            return View(Plasmid);
        }

        // POST: Plasmid/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string old_img_name, [Bind(Include = "id,name,sequence,seq_length,expression_system,expression_subsystem,promotor,polyA,resistance,reporter,selection,insert,insert_species,usage,plasmid_type,ref_plasmid,img_fn,addgene,d,people_id,des")] PlasmidViewModel plasmid)
        {
            //find all plasmid
            var plasmids = db.plasmids.OrderBy(n => n.name).Select(p => new { id = p.id, name = p.name, usage = p.usage });
            ViewBag.JsonPlasmid = JsonConvert.SerializeObject(plasmids.ToList());

            ViewBag.people_id = new SelectList(db.people.Select(p => new { id = p.id, name = p.first_name + " " + p.last_name }), "id", "name", plasmid.people_id);
            ViewBag.expression_system = new SelectList(db.dropdownitems.Where(c => c.category == "ExpSystem").OrderBy(g => g.text), "text", "value", plasmid.expression_system);
            ViewBag.insert_species = new SelectList(db.dropdownitems.Where(c => c.category == "InsertSpecies").OrderBy(g => g.text), "text", "value", plasmid.insert_species);
            ViewBag.plasmid_type = new SelectList(db.dropdownitems.Where(c => c.category == "PlasmidType").OrderBy(g => g.text), "text", "value", plasmid.plasmid_type);
            ViewBag.promotor = new SelectList(db.dropdownitems.Where(c => c.category == "Promotor").OrderBy(g => g.text), "text", "value", plasmid.promotor);
            ViewBag.polyA = new SelectList(db.dropdownitems.Where(c => c.category == "PolyA").OrderBy(g => g.text), "text", "value", plasmid.polyA);
           


            if (ModelState.IsValid)
            {
                plasmid.d = DateTime.Now;

                //db.plasmids.Add(plasmid);
                string Resistance = null;
                string Reporter = null;
                string Selection = null;
                string Usage = null;
                if (plasmid.resistance != null && plasmid.resistance.Count() > 1)
                {
                    Resistance = string.Join(",", plasmid.resistance);
                }
                else if (plasmid.resistance != null && plasmid.resistance.Count() == 1)
                {
                    Resistance = plasmid.resistance[0];
                }
                else
                {
                    Resistance = null;
                }

                if (plasmid.reporter != null && plasmid.reporter.Count() > 1)
                {
                    Reporter = string.Join(",", plasmid.reporter);
                }
                else if (plasmid.reporter != null && plasmid.reporter.Count() == 1)
                {
                    Reporter = plasmid.reporter[0];
                }
                else
                {
                    Reporter = null;
                }

                if (plasmid.selection != null && plasmid.selection.Count() > 1)
                {
                    Selection = string.Join(",", plasmid.selection);
                }
                else if (plasmid.selection != null &&  plasmid.selection.Count() == 1)
                {
                    Selection = plasmid.selection[0];
                }
                else
                {
                    Selection = null;
                }

                if (plasmid.usage != null && plasmid.usage.Count() > 1)
                {
                    Usage = string.Join(",", plasmid.usage);
                }
                else if (plasmid.usage != null &&  plasmid.usage.Count() == 1)
                {
                    Usage = plasmid.usage[0];
                }
                else
                {
                    Usage = null;
                }

                //new instance of plasmid model
                var Plasmid = db.plasmids.Find(plasmid.id);
                Plasmid.name = plasmid.name;
                Plasmid.sequence = Regex.Replace(plasmid.sequence.Trim(), @"[^\u0000-\u007F]", string.Empty);
                Plasmid.seq_length = plasmid.seq_length;
                Plasmid.expression_subsystem = plasmid.expression_subsystem;
                Plasmid.expression_system = plasmid.expression_system;
                Plasmid.promotor = plasmid.promotor;
                Plasmid.polyA = plasmid.polyA;
                Plasmid.resistance = Resistance;
                Plasmid.reporter = Reporter;
                Plasmid.selection = Selection;
                Plasmid.insert = plasmid.insert;
                Plasmid.usage = Usage;
                Plasmid.plasmid_type = plasmid.plasmid_type;
                Plasmid.ref_plasmid = plasmid.ref_plasmid;
                Plasmid.addgene = plasmid.addgene;
                Plasmid.d = plasmid.d;
                Plasmid.people_id = plasmid.people_id;   
                Plasmid.des = plasmid.des;
                Plasmid.insert_species = plasmid.insert_species;
                Plasmid.img_fn = plasmid.img_fn;

                if(plasmid.sequence != null)
                {
                    //backup plasmid map
                    var Backup = new BackupMap(plasmid.id);
                    //auto generate features
                    var autoFeatures = new PlasmidFeature(plasmid.id, plasmid.sequence);
                }
                

                //check whether user upload a file
                var timeStamp = DateTime.Now.Millisecond.ToString();
                string fileName = null;

                //upload img file
                HttpPostedFileBase file = null;
                file = Request.Files["img_fn"];

                if (eCloningSettings.AppHosting == "Cloud")
                {
                    //upload to azure
                    if (file != null && file.FileName != null && file.ContentLength > 0)
                    {
                        try
                        {
                            fileName = timeStamp + Path.GetFileName(file.FileName);
                            AzureBlob azureBlob = new AzureBlob();
                            azureBlob.directoryName = eCloningSettings.plasmidDir;
                            azureBlob.AzureBlobUpload(fileName, file);
                        }
                        catch (Exception)
                        {
                            ModelState.AddModelError("", "File upload failed!");
                            return View(plasmid);
                        }
                    }
                }
                else
                {
                    //upload to local plasmid folder
                    var plasmidPath = eCloningSettings.filePath + eCloningSettings.plasmidDir;
                    if (file != null && file.FileName != null && file.ContentLength > 0)
                    {
                        try
                        {
                            fileName = timeStamp + Path.GetFileName(file.FileName);
                            //fileExtension = Path.GetExtension(file.FileName);
                            var path = Path.Combine(Server.MapPath(plasmidPath), fileName);
                            file.SaveAs(path);
                        }
                        catch (Exception)
                        {
                            ModelState.AddModelError("", "File upload failed!");
                            return View(plasmid);
                        }
                    }
                }

                //save fileName to database
                Plasmid.img_fn = fileName;
                db.SaveChanges();
                //delete old image file
                if(file != null && file.FileName != null && file.ContentLength > 0 && old_img_name != null)
                {
                    //remove the old image file
                    if(eCloningSettings.AppHosting == "Cloud")
                    {
                        //delete from azure
                        AzureBlob azureBlob = new AzureBlob();
                        azureBlob.directoryName = eCloningSettings.plasmidDir;
                        azureBlob.AzureBlobDelete(old_img_name);
                    }
                    else
                    {
                        //delete from local
                        string path = Request.MapPath(eCloningSettings.filePath + eCloningSettings.plasmidDir + "/" +old_img_name);
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                    }
                }
                return RedirectToAction("Index");
            }
            return View(plasmid);
        }

        [Authorize]
        [HttpGet]
        public ActionResult Search()
        {
            ViewBag.Count = -2; //initial status

            ViewBag.expression_system = new SelectList(db.dropdownitems.Where(c => c.category == "ExpSystem").OrderBy(g => g.text), "text", "value");
            ViewBag.resistance1 = new SelectList(db.dropdownitems.Where(c => c.category == "Resistance").OrderBy(g => g.text), "text", "value");
            ViewBag.resistance2 = new SelectList(db.dropdownitems.Where(c => c.category == "Resistance").OrderBy(g => g.text), "text", "value");
            ViewBag.selection1 = new SelectList(db.dropdownitems.Where(c => c.category == "SelectMarker").OrderBy(g => g.text), "text", "value");
            ViewBag.selection2 = new SelectList(db.dropdownitems.Where(c => c.category == "SelectMarker").OrderBy(g => g.text), "text", "value");
            ViewBag.usage1 = new SelectList(db.dropdownitems.Where(c => c.category == "PlasmidUse").OrderBy(g => g.text), "text", "value");
            ViewBag.usage2 = new SelectList(db.dropdownitems.Where(c => c.category == "PlasmidUse").OrderBy(g => g.text), "text", "value");
            ViewBag.promotor = new SelectList(db.dropdownitems.Where(c => c.category == "Promotor").OrderBy(g => g.text), "text", "value");
            ViewBag.polyA = new SelectList(db.dropdownitems.Where(c => c.category == "PolyA").OrderBy(g => g.text), "text", "value");
            ViewBag.reporter = new SelectList(db.dropdownitems.Where(c => c.category == "Reporter").OrderBy(g => g.text), "text", "value");

            ViewBag.name = "";
            ViewBag.insert = "";
            ViewBag.sharedPlasmidCount = 0;
            return View();
        }
        [Authorize]
        [HttpPost]
        public ActionResult Search(string name, string expression_system, string promotor, string polyA, string resistance1, string resistance2, string reporter, string selection1, string selection2, string insert, string usage1, string usage2)
        {
            ViewBag.Count = -2; //initial status

            ViewBag.expression_system = new SelectList(db.dropdownitems.Where(c => c.category == "ExpSystem").OrderBy(g => g.text), "text", "value", expression_system);
            ViewBag.resistance1 = new SelectList(db.dropdownitems.Where(c => c.category == "Resistance").OrderBy(g => g.text), "text", "value");
            ViewBag.resistance2 = new SelectList(db.dropdownitems.Where(c => c.category == "Resistance").OrderBy(g => g.text), "text", "value");
            ViewBag.selection1 = new SelectList(db.dropdownitems.Where(c => c.category == "SelectMarker").OrderBy(g => g.text), "text", "value");
            ViewBag.selection2 = new SelectList(db.dropdownitems.Where(c => c.category == "SelectMarker").OrderBy(g => g.text), "text", "value");
            ViewBag.usage1 = new SelectList(db.dropdownitems.Where(c => c.category == "PlasmidUse").OrderBy(g => g.text), "text", "value");
            ViewBag.usage2 = new SelectList(db.dropdownitems.Where(c => c.category == "PlasmidUse").OrderBy(g => g.text), "text", "value");
            ViewBag.promotor = new SelectList(db.dropdownitems.Where(c => c.category == "Promotor").OrderBy(g => g.text), "text", "value", promotor);
            ViewBag.polyA = new SelectList(db.dropdownitems.Where(c => c.category == "PolyA").OrderBy(g => g.text), "text", "value", polyA);
            ViewBag.reporter = new SelectList(db.dropdownitems.Where(c => c.category == "Reporter").OrderBy(g => g.text), "text", "value", reporter);

            ViewBag.name = name;
            ViewBag.insert = insert;

            //validate user input
            name = string.IsNullOrWhiteSpace(name) ? null : name;
            expression_system = string.IsNullOrWhiteSpace(expression_system) ? null : expression_system;
            promotor= string.IsNullOrWhiteSpace(promotor) ? null : promotor;
            polyA = string.IsNullOrWhiteSpace(polyA) ? null : polyA;
            insert = string.IsNullOrWhiteSpace(insert) ? null : insert;
            resistance1 = string.IsNullOrWhiteSpace(resistance1) ? null : resistance1;
            resistance2 = string.IsNullOrWhiteSpace(resistance2) ? null : resistance2;
            selection1 = string.IsNullOrWhiteSpace(selection1) ? null : selection1;
            selection2 = string.IsNullOrWhiteSpace(selection2) ? null : selection2;
            usage1 = string.IsNullOrWhiteSpace(usage1) ? null : usage1;
            usage2 = string.IsNullOrWhiteSpace(usage2) ? null : usage2;
            reporter = string.IsNullOrWhiteSpace(reporter) ? null : reporter;
            if (name == null && expression_system == null && promotor == null && polyA == null && insert == null && resistance1== null && resistance2 == null && reporter== null && selection1== null && usage1 == null && selection2 == null && usage2 == null)
            {
                //user didn't type anything before cliking search
                ViewBag.Count = -1;
                ViewBag.sharedPlasmidCount = 0;
                return View();
            }


            var plasmids = db.plasmids.Include(p => p.person)
                .Where(p=>(name==null? true : p.name.Contains(name)))
                .Where(p => (expression_system == null ? true : p.expression_system.Contains(expression_system)))
                .Where(p => (promotor == null ? true : p.promotor.Contains(promotor)))
                .Where(p => (polyA == null ? true : p.polyA.Contains(polyA)))
                .Where(p => (insert == null ? true : p.insert.Contains(insert)))
                .Where(p => (reporter == null ? true : p.reporter.Contains(reporter)))
                .Where(p => (resistance1 == null ? true : p.resistance.Contains(resistance1)))
                .Where(p => (resistance2 == null ? true : p.resistance.Contains(resistance2)))
                .Where(p => (selection1 == null ? true : p.resistance.Contains(selection1)))
                .Where(p => (selection2 == null ? true : p.resistance.Contains(selection2)))
                .Where(p => (usage1 == null ? true : p.resistance.Contains(usage1)))
                .Where(p => (usage2 == null ? true : p.resistance.Contains(usage2)));

            ViewBag.Count = plasmids.Count();

            //get all the shared plasmid id
            //get current login email
            var userId = User.Identity.GetUserId();
            //get people id
            var userInfo = new UserInfo(userId);
            int peopleId = userInfo.PersonId;

            //get groupId
            var groupInfo = new GroupInfo(peopleId);
            IList<int> groupId = groupInfo.groupId;

            //get group shared plasmid ids
            var shared_plasmid = db.group_shared.Where(c => c.category == "plasmid").Where(g => groupId.Contains(g.group_id));
            ViewBag.sharedPlasmidCount = 0;
            if (shared_plasmid.Count() > 0)
            {
                List<int> sharedPlasmidId = shared_plasmid.Select(p => p.resource_id).ToList();
                ViewBag.sharedPlasmidCount = 1;
                ViewBag.sharedPlasmidId = sharedPlasmidId;
            }

            return View(plasmids.ToList());
        }

        [Authorize]
        [HttpGet]
        public ActionResult SharePlasmid(int? id)
        {
            //id is the plasmid table id
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //find the plasmid
            var plasmid = db.plasmids.Find(id);
            if(plasmid == null)
            {
                return HttpNotFound();
            }

            //get the group info
            //get current login email
            var userId = User.Identity.GetUserId();
            //get people id
            var userInfo = new UserInfo(userId);
            int peopleId = userInfo.PersonId;

            //get groupId
            var groupInfo = new GroupInfo(peopleId);
            Dictionary<int, string> groupIdName = groupInfo.groupIdName;
            ViewBag.groupIdName = groupIdName;
            ViewBag.Id = id;
            return View();
        }


        [Authorize]
        [HttpPost]
        public ActionResult SharePlasmid(string[] group, int plasmid_table_id)
        {
            if (group.Count() == 0)
            {
                TempData["msg"] = "You must select at least one group to share your plasmid!";
            }
            if (plasmid_table_id == 0)
            {
                TempData["msg"] = "Something went wrong, please try again!";
            }
            else
            {
                var plasmid = db.plasmids.Find(plasmid_table_id);
                if (plasmid == null)
                {
                    TempData["msg"] = "Something went wrong, please try again!";
                    return RedirectToAction("Index");
                }
                else
                {
                    //share the plasmid
                    //parse group id sting into int
                    foreach(string i in group)
                    {
                        //parse to int
                        int groupId = Int32.Parse(i);
                        var share = new group_shared();
                        share.group_id = groupId;
                        share.resource_id = plasmid.id;
                        share.category = "plasmid";
                        db.group_shared.Add(share);
                    }
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index");
        }

        // GET: Plasmid/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            plasmid plasmid = db.plasmids.Find(id);
            if (plasmid == null)
            {
                return HttpNotFound();
            }
            return View(plasmid);
        }

        // POST: Plasmid/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            plasmid plasmid = db.plasmids.Find(id);
            db.plasmids.Remove(plasmid);
            //delete plasmid_map
            var plasmid_map = db.plasmid_map.Where(p => p.plasmid_id == id);
            if (plasmid_map.Count() > 0)
            {
                foreach(var i in plasmid_map.ToList())
                {
                    db.plasmid_map.Remove(i);
                }
            }
            //delete plasmid_map_backup
            var plasmid_map_backup = db.plasmid_map_backup.Where(p => p.plasmid_id == id);
            if (plasmid_map_backup.Count() > 0)
            {
                foreach (var i in plasmid_map_backup.ToList())
                {
                    db.plasmid_map_backup.Remove(i);
                }
            }
            db.SaveChanges();
            //remove any uploaded file
            if(plasmid.img_fn != null)
            {
                if (eCloningSettings.AppHosting == "Cloud")
                {
                    //delete from azure
                    AzureBlob azureBlob = new AzureBlob();
                    azureBlob.directoryName = eCloningSettings.plasmidDir;
                    azureBlob.AzureBlobDelete(plasmid.img_fn);
                }
                else
                {
                    //delete from local
                    string path = Request.MapPath(eCloningSettings.filePath + eCloningSettings.plasmidDir + "/" + plasmid.img_fn);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }
            }
            return RedirectToAction("Index");
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
