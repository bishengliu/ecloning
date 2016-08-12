using ecloning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace ecloning.Areas.Admin.Controllers
{
    public class RActivityController : SubRootController
    {
        private ecloningEntities db = new ecloningEntities();
        // GET: Admin/RActivity
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
            //get company restriction enzyme list
            var enzymes = db.restriction_company.Where(c => c.company_id == company_id);
            List<int> enzymeId = new List<int>();
            if (enzymes.Count() > 0)
            {
                enzymeId = enzymes.Select(e => e.enzyme_id).ToList();
            }
            ViewBag.Count = enzymes.Count();  //show that there is no enzyme
            ViewBag.CompanyId = company_id;
            ViewBag.Company = company.FirstOrDefault().shortName;
            ViewBag.EnzymeId = enzymeId;

            //load data to model RestrictionActivity
            List<RestrictionActivity> RActivityList = new List<RestrictionActivity>();

            //first get active buffer
            var buffers = db.buffers.Where(b => b.company_id == company_id && b.show_activity != false).OrderBy(b => b.name).ToList();
            var bufferId = buffers.Select(i => i.id).ToList();
            var bufferName = buffers.Select(i => i.name).ToList();
            ViewBag.BufferCount = buffers.Count(); //don't show buffer and activity
            ViewBag.BufferId = bufferId;
            ViewBag.bufferName = bufferName;

            var restri = db.restri_enzyme.Where(r => enzymeId.Contains(r.id)).ToList();
            if (enzymes.Count() > 0)
            {
                //first load the activities;
                var activities = db.activity_restriction.Where(a => a.company_id == company_id);
                foreach (var e in restri)
                {
                    var RActivity = new RestrictionActivity();
                    RActivity.company_id = company_id;
                    RActivity.enzyme_id = e.id;
                    RActivity.enzymeName = e.name;
                    RActivity.forward_seq = e.forward_seq;
                    RActivity.forward_cut = e.forward_cut;
                    RActivity.forward_cut2 = e.forward_cut2;
                    RActivity.reverse_cut2 = e.reverse_cut2;
                    RActivity.reverse_cut = e.reverse_cut;
                    RActivity.starActivity = e.staractitivity;
                    RActivity.dam = e.dam;
                    RActivity.dcm = e.dcm;
                    RActivity.cpg = e.cpg;
                    RActivity.inactivity = e.inactivation;

                    List<Activity> bufferActivity = new List<Activity>();
                    //uncoment this if want to show buffer info

                    if (buffers.Count() > 0)
                    {
                        //find activity                           
                        var enzymeActivities = activities.Where(a => a.enzyme_id == e.id).Where(b => bufferId.Contains(b.buffer_id)).OrderBy(b => b.buffer_id).ToList();
                        if (enzymeActivities.Count() > 0)
                        {
                            foreach (var b in enzymeActivities)
                            {
                                Activity Activity = new Activity();
                                Activity.id = b.buffer_id; //buffer id 
                                Activity.bufferName = b.buffer.name;//buffer name
                                Activity.activity = b.activity; //enzyme activity
                                bufferActivity.Add(Activity);
                                RActivity.temprature = b.temprature;
                            }
                        }
                    }

                    RActivity.Activity = bufferActivity;
                    RActivityList.Add(RActivity);
                }
            }
            return View(RActivityList.ToList());
        }

        [Authorize]
        [HttpPost]
        public ActionResult EnzymeList(string type, int company_id, int pk, int value)
        {
            if (type == "activity")
            {
                //find the activity
                var activity = db.activity_restriction.Find(pk);
                if( activity != null)
                {
                    activity.activity = value;
                }
            }
            if (type == "temprature")
            {
                //find the all the activity of this enzyme
                var activities = db.activity_restriction.Where(a => a.enzyme_id == pk && a.company_id == company_id);
                if (activities.Count() > 0)
                {
                    foreach(var a in activities)
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
            var enzyme = db.restriction_company.Where(e => e.company_id == (int)company_id && e.enzyme_id == (int)enzyme_id);
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    if (enzyme.Count() == 0)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    else
                    {
                        //remove that enzyme
                        db.restriction_company.Remove(enzyme.FirstOrDefault());

                        //remove all the activity
                        var activities = db.activity_restriction.Where(e => e.company_id == (int)company_id && e.enzyme_id == (int)enzyme_id);
                        if (activities.Count() > 0)
                        {
                            foreach (var a in activities.ToList())
                            {
                                db.activity_restriction.Remove(a);
                            }
                        }
                        db.SaveChanges();
                    }
                    scope.Complete();
                    return RedirectToAction("EnzymeList", new { company_id = company_id });
                }
                catch (Exception)
                {
                    scope.Dispose();
                    return RedirectToAction("EnzymeList", new { company_id = company_id });
                }
            }

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
            var enzymes = db.restriction_company.Where(c => c.company_id == company_id);
            List<int> enzymeId = new List<int>();
            if (enzymes.Count() > 0)
            {
                enzymeId = enzymes.Select(e => e.enzyme_id).ToList();
            }
            ViewBag.CompanyId = company_id;
            ViewBag.Company = company.FirstOrDefault().shortName;
            //get the enzymes that are not in the company
            var enzymeList = db.restri_enzyme.Where(i => !enzymeId.Contains(i.id)).ToList();
            return View(enzymeList);
        }


        [Authorize]
        public ActionResult AddToCompany(int? enzyme_id, int? company_id)
        {
            //get the comany
            if(enzyme_id == null || company_id== null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var company = db.companies.Where(c => c.id == company_id);
            if (company.Count() == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //start transction
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    //add to company list
                    var enzyme = new restriction_company();
                    enzyme.company_id = (int)company_id;
                    enzyme.enzyme_id = (int)enzyme_id;
                    db.restriction_company.Add(enzyme);

                    //create empty activity in activity_restriction table
                    //get all buffer id
                    var buffers = db.buffers.Where(c => c.company_id == company_id);
                    if (buffers.Count() > 0)
                    {
                        foreach(var b  in buffers)
                        {
                            var activity = new activity_restriction();
                            activity.enzyme_id = (int)enzyme_id;
                            activity.company_id = (int)company_id;
                            activity.buffer_id = b.id;
                            activity.temprature = 37;
                            activity.activity = 0; 
                            db.activity_restriction.Add(activity);
                        }
                    }
                    db.SaveChanges();
                    scope.Complete();
                }
                catch (Exception)
                {
                    scope.Dispose();
                }
            }
            

            return RedirectToAction("AddEnzyme", new { company_id = company_id });
        }


    }
}