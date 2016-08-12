using ecloning.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ecloning.Controllers
{
    public class RActivityController : RootController
    {
        private ecloningEntities db = new ecloningEntities();

        // GET: RActivity
        [Authorize]
        public ActionResult Index()
        {
            //get current login email
            var email = User.Identity.GetUserName();
            int peopleId = 0;
            var people = db.people.Where(e => e.email == email);
            peopleId = people.FirstOrDefault().id;

            //get the group id
            List<int> groupId = new List<int>();
            var group_people = db.group_people.Where(p => p.people_id == peopleId);
            foreach (int i in group_people.Select(g => g.group_id).ToList())
            {
                groupId.Add(i);
            }
            //get the list enzyme list by company
            //get the favorite enzymes
            List<EnzymeByCompany> enzymeByCompanies = new List<EnzymeByCompany>();

            //get company id list
            var companies = db.companies;
            if (companies.Count() > 0)
            {
                foreach (var c in companies)
                {
                    EnzymeByCompany enzymeByCompany = new EnzymeByCompany();

                    //get the enzyme id in common_restriction
                    var enzymes = db.common_restriction.Where(e => e.company_id == c.id).Where(g => groupId.Contains(g.group_id));
                    if (enzymes.Count() > 0)
                    {
                        enzymeByCompany.company_id = c.id;
                        enzymeByCompany.enzymeId = enzymes.Select(e => e.enzyme_id).ToList();
                        enzymeByCompanies.Add(enzymeByCompany);
                    }
                }
            }
            ViewBag.Count = enzymeByCompanies.Count();
            return View(enzymeByCompanies.ToList());
        }

        [Authorize]
        public ActionResult Company()
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
            var enzymes = db.restriction_company.Where(c => c.company_id == company_id).ToList();
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
                        var enzymeActivities = activities.Where(a => a.enzyme_id == e.id).Where(b=>bufferId.Contains(b.buffer_id)).OrderBy(b=>b.buffer_id).ToList();
                        if (enzymeActivities.Count() > 0)
                        {
                            foreach (var b in enzymeActivities)
                            {
                                Activity Activity = new Activity();
                                Activity.id = b.buffer_id; //buffer id 
                                Activity.bufferName =b.buffer.name;//buffer name
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

            var userId = User.Identity.GetUserId();
            var userInfo = new UserInfo(userId);
            var groupInfo = new GroupInfo(userInfo.PersonId);
            ViewBag.GroupId = groupInfo.groupId;

            return View(RActivityList.ToList());
        }

        [Authorize]
        public ActionResult AddToFavorite(int? company_id, int? enzyme_id)
        {
            if(company_id == null || enzyme_id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //get group info
            //get current login email
            var email = User.Identity.GetUserName();
            int peopleId = 0;
            var people = db.people.Where(e => e.email == email);
            peopleId = people.FirstOrDefault().id;

            //get the group id
            List<int> groupId = new List<int>();
            var group_people = db.group_people.Where(p => p.people_id == peopleId);
            foreach (int i in group_people.Select(g => g.group_id).ToList())
            {
                groupId.Add(i);
            }

            foreach(var g in groupId)
            {
                var fav = new common_restriction();
                fav.company_id = (int)company_id;
                fav.enzyme_id = (int)enzyme_id;
                fav.group_id = g;
                db.common_restriction.Add(fav);
            }
            db.SaveChanges();
            return RedirectToAction("EnzymeList", new { company_id = company_id });
        }

        [Authorize]
        public ActionResult RemoveFromFavorite(int? company_id, int? enzyme_id)
        {
            if (company_id == null || enzyme_id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //get group info
            //get current login email
            var email = User.Identity.GetUserName();
            int peopleId = 0;
            var people = db.people.Where(e => e.email == email);
            peopleId = people.FirstOrDefault().id;

            //get the group id
            List<int> groupId = new List<int>();
            var group_people = db.group_people.Where(p => p.people_id == peopleId);
            foreach (int i in group_people.Select(g => g.group_id).ToList())
            {
                groupId.Add(i);
            }

            foreach (var g in groupId)
            {

                var fav = db.common_restriction.Where(f=>f.enzyme_id == enzyme_id && f.company_id ==company_id&& f.group_id==g);
                if (fav.Count() >0)
                {
                    db.common_restriction.Remove(fav.FirstOrDefault());
                }
            }
            db.SaveChanges();
            return RedirectToAction("EnzymeList", new { company_id = company_id });
        }

        //show the favorite enzymes for groups
        //[Authorize]
        //public ActionResult FavoriteEnzyme()
        //{
        //    //get current login email
        //    var email = User.Identity.GetUserName();
        //    int peopleId = 0;
        //    var people = db.people.Where(e => e.email == email);
        //    peopleId = people.FirstOrDefault().id;

        //    //get the group id
        //    List<int> groupId = new List<int>();
        //    var group_people = db.group_people.Where(p => p.people_id == peopleId);
        //    foreach (int i in group_people.Select(g => g.group_id).ToList())
        //    {
        //        groupId.Add(i);
        //    }
        //    //get the list enzyme list by company
        //    //get the favorite enzymes
        //    List<EnzymeByCompany> enzymeByCompanies = new List<EnzymeByCompany>();

        //        //get company id list
        //        var companies = db.companies;
        //        if (companies.Count() > 0)
        //        {
        //            foreach (var c in companies)
        //            {
        //                EnzymeByCompany enzymeByCompany = new EnzymeByCompany();

        //                //get the enzyme id in common_restriction
        //                var enzymes = db.common_restriction.Where(e => e.company_id == c.id).Where(g => groupId.Contains(g.group_id));
        //                if (enzymes.Count() > 0)
        //                {
        //                    enzymeByCompany.company_id = c.id;
        //                    enzymeByCompany.enzymeId = enzymes.Select(e => e.enzyme_id).ToList();
        //                    enzymeByCompanies.Add(enzymeByCompany);
        //                }
        //            }
        //        }
        //    ViewBag.Count = enzymeByCompanies.Count();
        //    return View(enzymeByCompanies.ToList());
        //}

        [Authorize]
        public ActionResult EnzymeInfo(int? company_id, int? enzyme_id)
        {
            //get the comany
            var company = db.companies.Where(c => c.id == company_id);
            if (company.Count() == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var enzyme = db.restri_enzyme.Find(enzyme_id);
            if (enzyme == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.CompanyId = company_id;
            ViewBag.Company = company.FirstOrDefault().shortName;
            ViewBag.EnzymeId = enzyme_id;
            
            //first get active buffer
            var buffers = db.buffers.Where(b => b.company_id == company_id && b.show_activity != false).OrderBy(b => b.name);
            ViewBag.BufferCount = buffers.Count(); //don't show buffer and activity

            //find the enzyme
            var RActivity = new RestrictionActivity();
            RActivity.company_id = (int)company_id;
            RActivity.enzyme_id = (int)enzyme_id;
            RActivity.starActivity = enzyme.staractitivity;
            RActivity.dam = enzyme.dam;
            RActivity.dcm = enzyme.dcm;
            RActivity.cpg = enzyme.cpg;
            RActivity.inactivity = enzyme.inactivation;

            List<Activity> bufferActivity = new List<Activity>();
            if (buffers.Count() > 0)
            {
                foreach (var b in buffers.ToList())
                {
                    Activity Activity = new Activity();
                    //find activity
                    var activity = db.activity_restriction.Where(a => a.company_id == company_id && a.enzyme_id == enzyme_id && a.buffer_id == b.id);
                    if (activity.Count() > 0)
                    {
                        Activity.id = b.id; //buffer id 
                        Activity.bufferName = b.name;//buffer name
                        Activity.activity = activity.FirstOrDefault().activity; //enzyme activity
                        bufferActivity.Add(Activity);
                        RActivity.temprature = activity.FirstOrDefault().temprature;
                    }
                }
                RActivity.Activity = bufferActivity;
            }
            
            ViewBag.Enzyme = enzyme;
            ViewBag.Buffers = buffers.ToList();
            return View(RActivity);
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