using ecloning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ecloning.Areas.Admin.Controllers
{
    public class MActivityController : SubRootController
    {
        private ecloningEntities db = new ecloningEntities();
        // GET: Admin/MActivity
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
        [Authorize]
        public ActionResult EnzymeList(int company_id)
        {
            //get the comany
            var company = db.companies.Where(c => c.id == company_id);
            if (company.Count() == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //get company modifying enzyme list
            var enzymes = db.modifying_company.Where(c => c.company_id == company_id);
            List<int> enzymeId = new List<int>();
            if (enzymes.Count() > 0)
            {
                enzymeId = enzymes.Select(e => e.enzyme_id).ToList();
            }
            ViewBag.Count = enzymes.Count();  //show that there is no enzyme
            ViewBag.CompanyId = company_id;
            ViewBag.Company = company.FirstOrDefault().shortName;
            ViewBag.EnzymeId = enzymeId;
            //get the enzymes
            //var enzymeList = db.restri_enzyme.Where(i => enzymeId.Contains(i.id)).ToList();

            //load data to model RestrictionActivity
            List<ModifyingActivity> MActivityList = new List<ModifyingActivity>();

            //first get active buffer
            var buffers = db.buffers.Where(b => b.company_id == company_id && b.show_activity != false).OrderBy(b => b.name);
            ViewBag.BufferCount = buffers.Count(); //don't show buffer and activity
            if (enzymes.Count() > 0)
            {
                foreach (var e in enzymeId)
                {
                    //find the enzyme
                    var enzyme = db.modifying_enzyme.Find(e);

                    var MActivity = new ModifyingActivity();
                    MActivity.company_id = company_id;
                    MActivity.enzyme_id = e;
                    
                    List<Dictionary<int, int>> bufferDict = new List<Dictionary<int, int>>();
                    if (buffers.Count() > 0)
                    {
                        foreach (var b in buffers.ToList())
                        {
                            Dictionary<int, int> Activity = new Dictionary<int, int>();
                            //find activity
                            var activity = db.activity_modifying.Where(a => a.company_id == company_id && a.enzyme_id == e && a.buffer_id == b.id);
                            if (activity.Count() > 0)
                            {
                                Activity.Add(b.id, activity.FirstOrDefault().activity);
                                bufferDict.Add(Activity);
                                MActivity.temprature = activity.FirstOrDefault().temprature;
                            }
                        }
                        MActivity.Activity = bufferDict;
                    }

                    MActivityList.Add(MActivity);
                }
            }

            return View(MActivityList.ToList());



            //return View(enzymeList);
        }

        [Authorize]
        [HttpPost]
        public ActionResult EnzymeList(string type, int company_id, int pk, int value)
        {
            if (type == "activity")
            {
                //find the activity
                var activity = db.activity_modifying.Find(pk);
                if (activity != null)
                {
                    activity.activity = value;
                }
            }
            if (type == "temprature")
            {
                //find the all the activity of this enzyme
                var activities = db.activity_modifying.Where(a => a.enzyme_id == pk && a.company_id == company_id);
                if (activities.Count() > 0)
                {
                    foreach (var a in activities)
                    {
                        a.temprature = value;
                    }
                }
            }
            db.SaveChanges();
            return RedirectToAction("EnzymeList", new { company_id = company_id });
        }

        [Authorize]
        public ActionResult Delete(int? enzyme_id, int? company_id)
        {
            //get the comany
            if (enzyme_id == null || company_id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var company = db.companies.Where(c => c.id == company_id);
            if (company.Count() == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //find the enzyme
            var enzyme = db.modifying_company.Where(e => e.company_id == (int)company_id && e.enzyme_id == (int)enzyme_id);
            if (enzyme.Count() == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                //remove that enzyme
                db.modifying_company.Remove(enzyme.FirstOrDefault());

                //remove all the activity
                var activities = db.activity_modifying.Where(e => e.company_id == (int)company_id && e.enzyme_id == (int)enzyme_id);
                if (activities.Count() > 0)
                {
                    foreach (var a in activities.ToList())
                    {
                        db.activity_modifying.Remove(a);
                    }
                }
                db.SaveChanges();
            }

            return RedirectToAction("EnzymeList", new { company_id = company_id });
        }

        [Authorize]
        public ActionResult AddEnzyme(int company_id)
        {
            //get the comany
            var company = db.companies.Where(c => c.id == company_id);
            if (company.Count() == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //get company restriction enzyme list
            var enzymes = db.modifying_company.Where(c => c.company_id == company_id);
            List<int> enzymeId = new List<int>();
            if (enzymes.Count() > 0)
            {
                enzymeId = enzymes.Select(e => e.enzyme_id).ToList();
            }
            ViewBag.CompanyId = company_id;
            ViewBag.Company = company.FirstOrDefault().shortName;
            //get the enzymes that are not in the company
            var enzymeList = db.modifying_enzyme.Where(i => !enzymeId.Contains(i.id)).ToList();
            return View(enzymeList);
        }
        [Authorize]
        public ActionResult AddToCompany(int? enzyme_id, int? company_id)
        {
            //get the comany
            if (enzyme_id == null || company_id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var company = db.companies.Where(c => c.id == company_id);
            if (company.Count() == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //add to company list
            var enzyme = new modifying_company();
            enzyme.company_id = (int)company_id;
            enzyme.enzyme_id = (int)enzyme_id;
            db.modifying_company.Add(enzyme);


            //create empty activity in activity_restriction table
            //get all buffer id
            var buffers = db.buffers.Where(c => c.company_id == company_id);
            if (buffers.Count() > 0)
            {
                foreach (var b in buffers)
                {
                    var activity = new activity_modifying();
                    activity.enzyme_id = (int)enzyme_id;
                    activity.company_id = (int)company_id;
                    activity.buffer_id = b.id;
                    activity.temprature = 37;
                    activity.activity = 0;
                    db.activity_modifying.Add(activity);
                }
            }
            db.SaveChanges();

            return RedirectToAction("AddEnzyme", new { company_id = company_id });
        }
    }
}