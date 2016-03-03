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
        public ActionResult Index()
        {
            var plasmids = db.plasmids.Include(p => p.person);
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
            ViewBag.submitted_to_group = new SelectList(db.dropdownitems.Where(c => c.category == "TF").OrderBy(g => g.text), "value", "text", "false");
            ViewBag.shared_with_group = new SelectList(db.dropdownitems.Where(c => c.category == "TF").OrderBy(g => g.text), "text", "value");
            ViewBag.shared_with_group = new SelectList(db.dropdownitems.Where(c => c.category == "TF").OrderBy(g => g.text), "text", "value");

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
        public ActionResult Create([Bind(Include = "id,name,sequence,expression_system,expression_subsystem,promotor,polyA,resistance,reporter,selection,insert,insert_species,usage,plasmid_type,ref_plasmid,img_fn,addgene,d,people_id,submitted_to_group,shared_with_group,shared_with_people,des")] PlasmidViewModel plasmid)
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
            ViewBag.submitted_to_group = new SelectList(db.dropdownitems.Where(c => c.category == "TF").OrderBy(g => g.text), "value", "text", plasmid.submitted_to_group);
            ViewBag.shared_with_group = new SelectList(db.dropdownitems.Where(c => c.category == "TF").OrderBy(g => g.text), "text", "value",plasmid.shared_with_group);
            ViewBag.shared_with_group = new SelectList(db.dropdownitems.Where(c => c.category == "TF").OrderBy(g => g.text), "text", "value",plasmid.shared_with_people);

            //ViewBag.resistance = new SelectList(db.dropdownitems.Where(c => c.category == "Resistance").OrderBy(g => g.text), "text", "value", plasmid.resistance);
            //ViewBag.selection = new SelectList(db.dropdownitems.Where(c => c.category == "SelectMarker").OrderBy(g => g.text), "text", "value", plasmid.selection);
            //ViewBag.usage = new SelectList(db.dropdownitems.Where(c => c.category == "PlasmidUse").OrderBy(g => g.text), "text", "value", plasmid.usage);
            //ViewBag.reporter = new SelectList(db.dropdownitems.Where(c => c.category == "Reporter").OrderBy(g => g.text), "text", "value", plasmid.reporter);

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
                Plasmid.submitted_to_group = plasmid.submitted_to_group;
                Plasmid.shared_with_group = plasmid.shared_with_group;
                Plasmid.shared_with_people = plasmid.shared_with_people;
                Plasmid.des = plasmid.des;
                Plasmid.insert_species = plasmid.insert_species;


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
            Plasmid.submitted_to_group = plasmid.submitted_to_group;
            Plasmid.shared_with_group = plasmid.shared_with_group;
            Plasmid.shared_with_people = plasmid.shared_with_people;
            Plasmid.des = plasmid.des;
            Plasmid.insert_species = plasmid.insert_species;
            Plasmid.img_fn = plasmid.img_fn;



            //find all plasmid
            var plasmids = db.plasmids.OrderBy(n => n.name).Select(p => new { id = p.id, name = p.name, usage = p.usage });
            ViewBag.JsonPlasmid = JsonConvert.SerializeObject(plasmids.ToList());

            ViewBag.people_id = new SelectList(db.people.Select(p => new { id = p.id, name = p.first_name + " " + p.last_name }), "id", "name", plasmid.people_id);
            ViewBag.expression_system = new SelectList(db.dropdownitems.Where(c => c.category == "ExpSystem").OrderBy(g => g.text), "text", "value", plasmid.expression_system);
            ViewBag.insert_species = new SelectList(db.dropdownitems.Where(c => c.category == "InsertSpecies").OrderBy(g => g.text), "text", "value", plasmid.insert_species);
            ViewBag.plasmid_type = new SelectList(db.dropdownitems.Where(c => c.category == "PlasmidType").OrderBy(g => g.text), "text", "value", plasmid.plasmid_type);
            ViewBag.promotor = new SelectList(db.dropdownitems.Where(c => c.category == "Promotor").OrderBy(g => g.text), "text", "value", plasmid.promotor);
            ViewBag.polyA = new SelectList(db.dropdownitems.Where(c => c.category == "PolyA").OrderBy(g => g.text), "text", "value", plasmid.polyA);
            ViewBag.submitted_to_group = new SelectList(db.dropdownitems.Where(c => c.category == "TF").OrderBy(g => g.text), "value", "text", plasmid.submitted_to_group);
            ViewBag.shared_with_group = new SelectList(db.dropdownitems.Where(c => c.category == "TF").OrderBy(g => g.text), "text", "value", plasmid.shared_with_group);
            ViewBag.shared_with_group = new SelectList(db.dropdownitems.Where(c => c.category == "TF").OrderBy(g => g.text), "text", "value", plasmid.shared_with_people);
            return View(Plasmid);
        }

        // POST: Plasmid/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string old_img_name, [Bind(Include = "id,name,sequence,expression_system,expression_subsystem,promotor,polyA,resistance,reporter,selection,insert,insert_species,usage,plasmid_type,ref_plasmid,img_fn,addgene,d,people_id,submitted_to_group,shared_with_group,shared_with_people,des")] PlasmidViewModel plasmid)
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
            ViewBag.submitted_to_group = new SelectList(db.dropdownitems.Where(c => c.category == "TF").OrderBy(g => g.text), "value", "text", plasmid.submitted_to_group);
            ViewBag.shared_with_group = new SelectList(db.dropdownitems.Where(c => c.category == "TF").OrderBy(g => g.text), "text", "value", plasmid.shared_with_group);
            ViewBag.shared_with_group = new SelectList(db.dropdownitems.Where(c => c.category == "TF").OrderBy(g => g.text), "text", "value", plasmid.shared_with_people);



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
                Plasmid.submitted_to_group = plasmid.submitted_to_group;
                Plasmid.shared_with_group = plasmid.shared_with_group;
                Plasmid.shared_with_people = plasmid.shared_with_people;
                Plasmid.des = plasmid.des;
                Plasmid.insert_species = plasmid.insert_species;
                Plasmid.img_fn = plasmid.img_fn;


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
            ViewBag.expression_system = new SelectList(db.dropdownitems.Where(c => c.category == "ExpSystem").OrderBy(g => g.text), "text", "value");
            ViewBag.resistance = new SelectList(db.dropdownitems.Where(c => c.category == "Resistance").OrderBy(g => g.text), "text", "value");
            ViewBag.selection = new SelectList(db.dropdownitems.Where(c => c.category == "SelectMarker").OrderBy(g => g.text), "text", "value");
            ViewBag.usage = new SelectList(db.dropdownitems.Where(c => c.category == "PlasmidUse").OrderBy(g => g.text), "text", "value");
            ViewBag.promotor = new SelectList(db.dropdownitems.Where(c => c.category == "Promotor").OrderBy(g => g.text), "text", "value");
            ViewBag.polyA = new SelectList(db.dropdownitems.Where(c => c.category == "PolyA").OrderBy(g => g.text), "text", "value");
            ViewBag.reporter = new SelectList(db.dropdownitems.Where(c => c.category == "Reporter").OrderBy(g => g.text), "text", "value");
            return View();
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
