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

        // GET: Plasmid/Details/5
        public ActionResult Details(int? id)
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
        public ActionResult Create([Bind(Include = "id,name,sequence,expression_system,expression_subsystem,promotor,polyA,resistance,reporter,selection,insert,insert_species,usage,plasmid_type,ref_plasmid,img_fn,addgene,d,people_id,submitted_to_group,shared_with_group,shared_with_people,des")] plasmid plasmid)
        {
            //find all plasmid
            var plasmids = db.plasmids.OrderBy(n => n.name).Select(p => new { id = p.id, name = p.name, usage = p.usage });
            ViewBag.JsonPlasmid = JsonConvert.SerializeObject(plasmids.ToList());

            ViewBag.people_id = new SelectList(db.people.Select(p => new { id = p.id, name = p.first_name + " " + p.last_name }), "id", "name",plasmid.people_id);
            ViewBag.expression_system = new SelectList(db.dropdownitems.Where(c => c.category == "ExpSystem").OrderBy(g => g.text), "text", "value",plasmid.expression_system);
            ViewBag.resistance = new SelectList(db.dropdownitems.Where(c => c.category == "Resistance").OrderBy(g => g.text), "text", "value",plasmid.resistance);
            ViewBag.selection = new SelectList(db.dropdownitems.Where(c => c.category == "SelectMarker").OrderBy(g => g.text), "text", "value",plasmid.selection);
            ViewBag.insert_species = new SelectList(db.dropdownitems.Where(c => c.category == "InsertSpecies").OrderBy(g => g.text), "text", "value",plasmid.insert_species);
            ViewBag.usage = new SelectList(db.dropdownitems.Where(c => c.category == "PlasmidUse").OrderBy(g => g.text), "text", "value",plasmid.usage);
            ViewBag.plasmid_type = new SelectList(db.dropdownitems.Where(c => c.category == "PlasmidType").OrderBy(g => g.text), "text", "value",plasmid.plasmid_type);
            ViewBag.promotor = new SelectList(db.dropdownitems.Where(c => c.category == "Promotor").OrderBy(g => g.text), "text", "value",plasmid.promotor);
            ViewBag.polyA = new SelectList(db.dropdownitems.Where(c => c.category == "PolyA").OrderBy(g => g.text), "text", "value",plasmid.polyA);
            ViewBag.reporter = new SelectList(db.dropdownitems.Where(c => c.category == "Reporter").OrderBy(g => g.text), "text", "value",plasmid.reporter);
            ViewBag.submitted_to_group = new SelectList(db.dropdownitems.Where(c => c.category == "TF").OrderBy(g => g.text), "value", "text", plasmid.submitted_to_group);
            ViewBag.shared_with_group = new SelectList(db.dropdownitems.Where(c => c.category == "TF").OrderBy(g => g.text), "text", "value",plasmid.shared_with_group);
            ViewBag.shared_with_group = new SelectList(db.dropdownitems.Where(c => c.category == "TF").OrderBy(g => g.text), "text", "value",plasmid.shared_with_people);
            if (ModelState.IsValid)
            {
                db.plasmids.Add(plasmid);


                string fileName = null;
                string fileExtension = null;

                //upload img file
                HttpPostedFileBase file = null;
                file = Request.Files["img_fn"];

                if (eCloningSettings.AppHosting() == "Cloud")
                {
                    //upload to azure
                    if (file != null && file.FileName != null && file.ContentLength > 0)
                    {
                        try
                        {
                            fileName = Path.GetFileName(file.FileName);
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
                    var plasmidPath = "~/App_Data/plasmid";
                    if (file != null && file.FileName != null && file.ContentLength > 0)
                    {
                        try
                        {
                            fileName = Path.GetFileName(file.FileName);
                            fileExtension = Path.GetExtension(file.FileName);
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
                plasmid.img_fn = fileName;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(plasmid);
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
            ViewBag.people_id = new SelectList(db.people, "id", "first_name", plasmid.people_id);
            return View(plasmid);
        }

        // POST: Plasmid/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,sequence,expression_system,expression_subsystem,promotor,polyA,resistance,reporter,selection,insert,insert_species,usage,plasmid_type,ref_plasmid,img_fn,addgene,d,people_id,submitted_to_group,shared_with_group,shared_with_people,des")] plasmid plasmid)
        {
            if (ModelState.IsValid)
            {
                db.Entry(plasmid).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.people_id = new SelectList(db.people, "id", "first_name", plasmid.people_id);
            return View(plasmid);
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
