using ecloning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ecloning.Controllers
{
    public class MActivityController : RootController
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
    }
}