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