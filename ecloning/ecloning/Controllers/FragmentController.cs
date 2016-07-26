using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ecloning.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;

namespace ecloning.Controllers
{
    public class FragmentController : RootController
    {
        private ecloningEntities db = new ecloningEntities();
        // GET: Fragment
        public ActionResult Index(int? id, string tag)
        {
            //id is the plasmid_id
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            plasmid plasmid = db.plasmids.Find(id);
            if (plasmid == null)
            {
                return HttpNotFound();
            }
            var sequence = plasmid.sequence;
            //get usr info
            var userId = User.Identity.GetUserId();
            var userInfo = new UserInfo(userId);

            IList<FragmentViewModel> fragments = new List<FragmentViewModel>();
            IList<int> fragmentIds = new List<int>();
            //get the fragment of the current user
            var savedFragments = db.fragments.Where(f => f.plasmid_id == (int)id && f.people_id==userInfo.PersonId);
            if (savedFragments.Count() > 0)
            {
                foreach(var item in savedFragments)
                {
                    //convert enzyme id into enzyme names
                    var enzymeIds = new List<int>();
                    var enzymes = new List<string>();
                    var enzymeIdStringArray = item.enzyme_id.Split(',');
                    enzymeIds.Add(Int32.Parse(enzymeIdStringArray[0]));
                    enzymeIds.Add(Int32.Parse(enzymeIdStringArray[1]));
                    var enzyme1 = db.restri_enzyme.Find(enzymeIds[0]).name;
                    enzymes.Add(enzyme1);
                    var enzyme2 = db.restri_enzyme.Find(enzymeIds[1]).name;
                    enzymes.Add(enzyme2);

                    //find the features
                    IList<fragmentFeatures> featuresArray = new List<fragmentFeatures>();
                    var features = db.fragment_map.Where(f => f.fragment_id == item.id).OrderBy(s=>s.start);
                    if (features.Count() > 0)
                    {
                        foreach(var feature in features)
                        {
                            var fObj = new fragmentFeatures();
                            fObj.clockwise = feature.clockwise==1? true : false;
                            fObj.cut = feature.cut;
                            fObj.common_id = feature.common_id;
                            fObj.end = feature.end;
                            fObj.feature = feature.feature;
                            fObj.show_feature = feature.show_feature;
                            fObj.start = feature.start;
                            fObj.type_id = feature.feature_id;
                            featuresArray.Add(fObj);
                        }
                    }

                    var fragment = new FragmentViewModel();
                    fragment.featureArray = featuresArray;
                    fragment.id = item.id;
                    fragment.fName = item.name;
                    fragment.plasmid_id = (int)id;
                    fragment.enzymes = enzymes;
                    fragment.f_start = (int)item.forward_start;
                    fragment.f_end = (int)item.forward_end;
                    fragment.fSeq = item.forward_seq;
                    //overhangs
                    var overhangs = new List<int>();
                    overhangs.Add((int)item.rc_left_overhand);
                    overhangs.Add((int)item.rc_right_overhand);
                    fragment.overhangs = overhangs;
                    fragment.cSeq = item.rc_seq;
                    fragments.Add(fragment); //add fragments
                    fragmentIds.Add(item.id); //add fragment ids
                }
            }
            ViewBag.plasmidName = plasmid.name;
            ViewBag.Id = id;
            ViewBag.Tag = tag;
            ViewBag.FragmentIds = fragmentIds;
            ViewBag.Sequence = sequence;
            ViewBag.Fragments = JsonConvert.SerializeObject(fragments.ToList());
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult Delete(int? id, int? plasmid_id, string tag)
        {
            if(id== null || plasmid_id== null)
            {
                return HttpNotFound();
            }
            else
            {
                //remove the fragment features
                var fMap = db.fragment_map.Where(f => f.fragment_id == id);
                if (fMap.Count() > 0)
                {
                    foreach(var map in fMap)
                    {
                        db.fragment_map.Remove(map);
                    }
                }
                //remove the fragment
                var fragment = db.fragments.Find(id);
                db.fragments.Remove(fragment);
                db.SaveChanges();
                return RedirectToAction("Index", "Fragment", new { id = plasmid_id, tag = tag });
            }
        }

        //for manually added linearized vectors
        [Authorize]
        [HttpGet]
        public ActionResult Fragment()
        {
            //get my people_id
            var userId = User.Identity.GetUserId();
            var userInfo = new UserInfo(userId);
            var groupInfo = new GroupInfo(userInfo.PersonId);

            int peopleId = userInfo.PersonId;
            List<int> groupId = groupInfo.groupId;


            //get group shared vectors
            //get the admin group ids (appAdmin, InstAdmin)
            List<int> adminId = new List<int>();
            List<string> adminNames = new List<string>();
            adminNames.Add("appAdmin");
            adminNames.Add("Institute Admin");
            var adminInfo = new AdminInfo();
            adminId = adminInfo.AdminId(adminNames);

            List<int> shareIds = new List<int>();
            if (groupId.Count() > 0)
            {
                var share = db.group_shared.Where(g => groupId.Contains(g.group_id) || adminId.Contains(g.group_id)).Where(c => c.category == "fragment").OrderByDescending(r => r.resource_id);
                shareIds = share.Select(r => r.resource_id).ToList();
            }

            //get my vectors and ally my own plasmid derived plasmid
            var myFragments = db.fragments.Where(f => f.people_id == userInfo.PersonId).Where(f => !shareIds.Contains(f.id));
            List<int> myfragId = myFragments.Select(f => f.id).ToList();

            //get the combined ids for get the fragment map
            List<int> combinedId = shareIds.Concat(myfragId).ToList();

            IList<FragmentViewModel> fragments = new List<FragmentViewModel>();
            var allFragments = db.fragments.Where(f => combinedId.Contains(f.id)).OrderByDescending(f => f.name).ToList();
            if (allFragments.Count() > 0)
            {
                foreach (var item in allFragments)
                {

                    //convert enzyme id into enzyme names
                    var enzymeIds = new List<int>();
                    var enzymes = new List<string>();
                    if (item.enzyme_id != null)
                    {
                        var enzymeIdStringArray = item.enzyme_id.Split(',');
                        enzymeIds.Add(Int32.Parse(enzymeIdStringArray[0]));
                        enzymeIds.Add(Int32.Parse(enzymeIdStringArray[1]));
                        var enzyme1 = db.restri_enzyme.Find(enzymeIds[0]).name;
                        enzymes.Add(enzyme1);
                        var enzyme2 = db.restri_enzyme.Find(enzymeIds[1]).name;
                        enzymes.Add(enzyme2);
                    }

                    //find the features
                    IList<fragmentFeatures> featuresArray = new List<fragmentFeatures>();
                    var features = db.fragment_map.Where(f => f.fragment_id == item.id).Where(f=>f.feature_id != 4).OrderBy(s => s.start);
                    if (features.Count() > 0)
                    {
                        foreach (var feature in features)
                        {
                            var fObj = new fragmentFeatures();
                            fObj.clockwise = feature.clockwise == 1 ? true : false;
                            fObj.cut = feature.cut;
                            fObj.common_id = feature.common_id;
                            fObj.end = feature.end;
                            fObj.feature = feature.feature;
                            fObj.show_feature = feature.show_feature;
                            fObj.start = feature.start;
                            fObj.type_id = feature.feature_id;
                            featuresArray.Add(fObj);
                        }
                    }
                    //get plasmid name
                    var plasmid = db.plasmids.Find((int)item.plasmid_id);

                    var fragment = new FragmentViewModel();
                    fragment.featureArray = featuresArray;
                    fragment.id = item.id;
                    fragment.fName = item.plasmid_id != null ? plasmid.name: item.name;
                    fragment.plasmid_id = (int)item.plasmid_id;
                    fragment.enzymes = enzymes;
                    fragment.f_start = (int)item.forward_start;
                    fragment.f_end = (int)item.forward_end;
                    fragment.fSeq = item.forward_seq;
                    //overhangs
                    var overhangs = new List<int>();
                    overhangs.Add((int)item.rc_left_overhand);
                    overhangs.Add((int)item.rc_right_overhand);
                    fragment.overhangs = overhangs;
                    fragment.cSeq = item.rc_seq;
                    fragments.Add(fragment); //add fragments

                }
            }
            ViewBag.shareIds = shareIds;
            ViewBag.FragmentIds = combinedId;
            ViewBag.Fragments = JsonConvert.SerializeObject(fragments.ToList());

            return View();
        }

        //add linearized vectors
        [Authorize]
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "fName,fSeq,cSeq,left_overhang")] FragmentViewModel fragment)
        {
            var userId = User.Identity.GetUserId();
            var userInfo = new UserInfo(userId);
            int peopleId = userInfo.PersonId;
            var groupInfo = new GroupInfo(userInfo.PersonId);
            //non zero validation
            if (fragment.left_overhang == 0)
            {
                TempData["error"] = "Left Overhang cann't be zero!";
                return View(fragment);
            }
            //check the name
            var fragments = db.fragments.Where(f => f.name == fragment.fName && f.people_id == peopleId);
            if (fragments.Count() > 0)
            {
                TempData["error"] = "Vector \"" + fragment.fName + "\" already exists!";
                return View(fragment);
            }
            if (ModelState.IsValid)
            {
                //cal the right overhang
                int right_overhang = -999;
                if (fragment.left_overhang == 0)
                {
                    right_overhang = fragment.cSeq.Length - fragment.fSeq.Length;
                }
                else if (fragment.left_overhang < 0)
                {
                    right_overhang = fragment.cSeq.Length - Math.Abs(fragment.left_overhang) - fragment.fSeq.Length;
                }
                else
                {
                    right_overhang = fragment.cSeq.Length + Math.Abs(fragment.left_overhang) - fragment.fSeq.Length;
                }
                var f = new fragment();
                f.name = fragment.fName;
                f.parantal = false;
                f.forward_start = 1;
                f.forward_end = fragment.fSeq.Length;
                f.rc_seq = fragment.cSeq;
                f.rc_left_overhand = fragment.left_overhang;
                f.people_id = peopleId;
                f.dt = DateTime.Now;
                f.rc_right_overhand = right_overhang;
                db.fragments.Add(f);
                db.SaveChanges();


                if (f.forward_seq != null)
                {
                    //auto generate features
                    var autoFeatures = new VectorFeature(f.id, f.forward_seq, groupInfo.groupId);
                }

                return RedirectToAction("Fragment", "Fragment");
            }
            return View(fragment);
        }

        //edit linearized vectors
        [Authorize]
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            //id is the fragment id
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var fragment = db.fragments.Find(id);
            if (fragment == null)
            {
                return HttpNotFound();
            }
            //load data
            var f = new FragmentViewModel();
            f.id = fragment.id;
            f.fName = fragment.name;
            f.fSeq = fragment.forward_seq;
            f.cSeq = fragment.rc_seq;
            f.left_overhang = (int)fragment.rc_left_overhand;
            return View(f);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,fName,fSeq,cSeq,left_overhang")] FragmentViewModel fragment)
        {
            //people id
            var userId = User.Identity.GetUserId();
            var userInfo = new UserInfo(userId);
            int peopleId = userInfo.PersonId;
            var groupInfo = new GroupInfo(userInfo.PersonId);
            //non zero validation
            if (fragment.left_overhang == 0)
            {
                TempData["error"] = "Left Overhang cann't be zero!";
                return View(fragment);
            }
            //check the name
            var fragments = db.fragments.Where(f=>f.id != fragment.id).Where(f => f.name == fragment.fName && f.people_id==peopleId);
            if (fragments.Count() > 0)
            {
                TempData["error"] = "Vector \"" + fragment.fName + "\" already exists!";
                return View(fragment);
            }
            if (ModelState.IsValid)
            {
                //find the fragment from database
                var f = db.fragments.Find(fragment.id);
                
                //cal the right overhang
                int right_overhang = -999;
                if (fragment.left_overhang == 0)
                {
                    right_overhang = fragment.cSeq.Length - fragment.fSeq.Length;
                }
                else if (fragment.left_overhang < 0)
                {
                    right_overhang = fragment.cSeq.Length - Math.Abs(fragment.left_overhang) - fragment.fSeq.Length;
                }
                else
                {
                    right_overhang = fragment.cSeq.Length + Math.Abs(fragment.left_overhang) - fragment.fSeq.Length;
                }
                f.name = fragment.fName;
                f.parantal = false;
                f.forward_start = 1;
                f.forward_end = fragment.fSeq.Length;
                f.rc_seq = fragment.cSeq;
                f.rc_left_overhand = fragment.left_overhang;
                f.people_id = peopleId;
                f.dt = DateTime.Now;
                f.rc_right_overhand = right_overhang;
                db.SaveChanges();

                if (f.forward_seq != null)
                {
                    //auto generate features
                    var autoFeatures = new VectorFeature(f.id, f.forward_seq, groupInfo.groupId);
                }

                return RedirectToAction("Fragment", "Fragment");
            }
            return View(fragment);
        }


        //share vectors
        [Authorize]
        [HttpGet]
        public ActionResult Share(int? id)
        {
            //check the existence of vector id
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var vector = db.fragments.Where(f => f.id == id);
            if (vector.Count() == 0)
            {
                return HttpNotFound();
            }

            //get the group info
            //get userId
            var userId = User.Identity.GetUserId();
            var userInfo = new UserInfo(userId);
            var groupInfo = new GroupInfo(userInfo.PersonId);
            //check whether it has already been shared
            var isShared = db.group_shared.Where(r => r.category == "fragment" && r.resource_id == id && r.group_id == groupInfo.groupId.FirstOrDefault());
            if (isShared.Count() > 0)
            {
                return RedirectToAction("Fragment", "Fragment");
            }

            //share the bundle
            var share = new group_shared();
            share.category = "fragment";
            share.group_id = groupInfo.groupId.FirstOrDefault();
            share.resource_id = (int)id;
            share.sratus = "submitted";
            db.group_shared.Add(share);
            db.SaveChanges();

            return RedirectToAction("Fragment", "Fragment");
        }


        [Authorize]
        [HttpGet]
        public ActionResult unShare(int? id)
        {
            //check the existence of fragment id
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var pBundle = db.fragments.Where(f => f.id == id);
            if (pBundle.Count() == 0)
            {
                return HttpNotFound();
            }
            //find the share
            var share = db.group_shared.Where(s => s.category == "fragment" && s.resource_id == id);
            foreach (var item in share)
            {
                db.group_shared.Remove(item);
            }
            db.SaveChanges();
            return RedirectToAction("Fragment", "Fragment");
        }

        //delete linearized vectors
        [Authorize]
        [HttpGet]
        public ActionResult DVector(int? id)
        {
            //id is the fragment id
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var fragment = db.fragments.Find(id);
            if (fragment == null)
            {
                return HttpNotFound();
            }

            db.fragments.Remove(fragment);

            //detele all the reffered features in fragment_map
            var maps = db.fragment_map.Where(i => i.fragment_id == id);
            if (maps.Count() > 0)
            {
                foreach (var m in maps)
                {
                    db.fragment_map.Remove(m);
                }
            }
            db.SaveChanges();
            return RedirectToAction("Fragment", "Fragment");
            
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