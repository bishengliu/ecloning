using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using ecloning.Models;
using System.Net;
using Newtonsoft.Json;
using System.Transactions;

namespace ecloning.Controllers
{
    public class ExperimentController : Controller
    {
        private ecloningEntities db = new ecloningEntities();
        // GET: Experiment
        [Authorize]
        [HttpGet]
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var userInfo = new UserInfo(userId);
            var groupInfo = new GroupInfo(userInfo.PersonId);
            ViewBag.PersonId = userInfo.PersonId;
            //show shared experiment
            var sharedExps= db.group_shared.Where(p => p.category == "experiment").Where(g => groupInfo.groupId.Contains(g.group_id)).OrderByDescending(r=>r.resource_id);
            List<int> sharedIds = new List<int>();
            if (sharedExps.Count() > 0)
            {
                sharedIds = sharedExps.Select(r => r.resource_id).ToList();
            }
            ViewBag.SharedId = sharedIds;
            //show my experiment
            var myExpIds = new List<int>();
            var myExps = db.experiments.Where(e => e.people_id == userInfo.PersonId).Where(e => !sharedIds.Contains(e.id)).OrderByDescending(e => e.id);
            if (myExps.Count() > 0)
            {
                myExpIds = myExps.Select(e => e.id).ToList();
            }
            //combined id
            List<int> combinedIds = myExpIds.Concat(sharedIds).ToList();            
            //get all exps
            var exps = db.experiments.Where(e => combinedIds.Contains(e.id)).ToList();
            return View(exps);
        }


        [Authorize]
        [HttpGet]
        public ActionResult Create()
        {
            //generate list
            //ViewBag.types = new SelectList(db.exp_type.OrderBy(g => g.name), "id", "name");
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,des")] ExperimentViewModal experiment)
        {
            //get userId
            var userId = User.Identity.GetUserId();
            var userInfo = new UserInfo(userId);
            if (ModelState.IsValid)
            {
                var exp = new experiment();
                exp.name = experiment.name;
                exp.des = experiment.des;
                exp.people_id = userInfo.PersonId;
                exp.dt = DateTime.Now;
                db.experiments.Add(exp);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(experiment);
        }


        [Authorize]
        [HttpGet]
        public ActionResult AddStep(int? id)
        {
            //id is the experiment id
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            experiment exp = db.experiments.Find(id);
            if (exp == null)
            {
                return HttpNotFound();
            }
            ViewBag.expId = id;

            var userId = User.Identity.GetUserId();
            var userInfo = new UserInfo(userId);
            var groupInfo = new GroupInfo(userInfo.PersonId);

            //get all the peopleId in the group
            var groupPeopleIds = groupInfo.groupPeopleId;

            //generate exp type list
            List<ExpType> types = new List<ExpType>();
            var type1 = new ExpType();
            type1.id = 1;
            type1.Name = "Restriction Enzyme Digestion";
            types.Add(type1);
            var type2 = new ExpType();
            type2.id = 2;
            type2.Name = "Plasmid Transformation";
            types.Add(type2);
            var type3 = new ExpType();
            type3.id = 3;
            type3.Name = "Plasmid Miniprep";
            types.Add(type3);
            var type4 = new ExpType();
            type4.id = 4;
            type4.Name = "Fragment Gel Extraction";
            types.Add(type4);
            var type5 = new ExpType();
            type5.id = 5;
            type5.Name = "PCR";
            types.Add(type5);
            var type6 = new ExpType();
            type6.id = 6;
            type6.Name = "Ligation";
            types.Add(type6);
            var type7 = new ExpType();
            type7.id = 7;
            type7.Name = "Pick Colonies";
            types.Add(type7);
            var type8 = new ExpType();
            type8.id = 8;
            type8.Name = "Plasmid Maxiprep";
            types.Add(type8);
            
            var listItems = types.OrderBy(g => g.Name);
            ViewBag.types = new SelectList(listItems, "id", "name");

            //pass plasmid json data
            //get shared plasmids
            var sharedPlasmidId = db.group_shared.Where(c => c.category == "plasmid").Where(g => groupInfo.groupId.Contains(g.group_id)).Select(p => p.resource_id).ToList();
            //my plasmidId
            var myPlasmidId = db.plasmids.Where(p => p.people_id == userInfo.PersonId && !sharedPlasmidId.Contains(p.id)).Select(p => p.id).ToList();
            //get combinedId
            var comPlasmidId = sharedPlasmidId.Concat(myPlasmidId).ToList();
            //pass protocol json data
            var plasmids = db.plasmids.Where(p => comPlasmidId.Contains(p.id)).Select(p => new
            {
                id = p.id,
                name = p.name
            });
            ViewBag.Plasmids = JsonConvert.SerializeObject(plasmids.ToList());
            //pass protocol id
            var protocols = db.protocols.Where(p => groupPeopleIds.Contains((int)p.people_id)).Select(p => new
            {
                id = p.id,
                name = p.name,
                version = p.version
            });
            ViewBag.Protocols = JsonConvert.SerializeObject(protocols.ToList());

            //pass fragments
            //get group shared vectors
            //get the admin group ids (appAdmin, InstAdmin)
            List<int> adminId = new List<int>();
            List<string> adminNames = new List<string>();
            adminNames.Add("appAdmin");
            adminNames.Add("Institute Admin");
            var adminInfo = new AdminInfo();
            adminId = adminInfo.AdminId(adminNames);

            var sharedFragementId = db.group_shared.Where(g => groupInfo.groupId.Contains(g.group_id) || adminId.Contains(g.group_id)).Where(c => c.category == "fragment").Select(r => r.resource_id).ToList();
            //get the fragment of the current user
            var myFragmentId = db.fragments.Where(f => f.people_id == userInfo.PersonId && !sharedFragementId.Contains(f.id)).Select(f => f.id).ToList();
            //combnined
            var comFragmentId = sharedFragementId.Concat(myFragmentId).ToList();

            //for drawing maps of fragment
            IList<FragmentViewModel> fragments = new List<FragmentViewModel>();
            var allFragments = db.fragments.Where(f => comFragmentId.Contains(f.id)).OrderByDescending(f => f.name).ToList();
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
                    var features = db.fragment_map.Where(f => f.fragment_id == item.id).Where(f => f.feature_id != 4).OrderBy(s => s.start);
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
                    var fragment = new FragmentViewModel();
                    string decodeName = "";
                    if (item.plasmid_id!= null)
                    {
                        decodeName = db.plasmids.Find((int)item.plasmid_id).name + " [" + enzymes[0]+"/"+ item.name.Split('-')[2] + "," + enzymes[1] + "/" + item.name.Split('-')[3] + "]";
                    }
                    fragment.featureArray = featuresArray;
                    fragment.id = item.id;
                    fragment.fName = item.plasmid_id != null ? decodeName : item.name;
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
            ViewBag.FragmentMap = JsonConvert.SerializeObject(fragments.ToList());
            //for token input
            var Fragments = fragments.Select(f => new {
                id = f.id,
                name = f.fName.Split('[')[0],
                pos = "["+f.fName.Split('[')[1]
            });
            ViewBag.Fragments = JsonConvert.SerializeObject(Fragments.ToList());

            //pass primers
            var primers = db.primers.Where(p => groupPeopleIds.Contains((int)p.people_id)).Select(p=> new {
                id = p.id,
                name = p.name
            });
            ViewBag.Primers = JsonConvert.SerializeObject(primers.ToList());
            //pass probe
            var probes = db.probes.Where(p => groupPeopleIds.Contains((int)p.people_id)).Select(p => new {
                id = p.id,
                name = p.name
            });
            ViewBag.Probes = JsonConvert.SerializeObject(probes.ToList());

            ViewBag.ShownStep = -1;
            return View();
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddStep(string StepButton, [Bind(Include = "id,types,name,type_id,exp_id,protocol_id,forward_primer_id,reverse_primer_id,probe_id,emzyme_id,plasmid_id,frag1_id,frag2_id, ligation_direction,des")] ExpStep step)
        {
            if (StepButton == "add1")
            {
                ViewBag.ShownStep = 1;
            }
            if (StepButton == "add2")
            {
                ViewBag.ShownStep = 2;
            }
            if (StepButton == "add3")
            {
                ViewBag.ShownStep = 3;
            }
            if (StepButton == "add4")
            {
                ViewBag.ShownStep = 4;
            }
            if (StepButton == "add5")
            {
                ViewBag.ShownStep = 5;
            }
            if (StepButton == "add6")
            {
                ViewBag.ShownStep = 6;
            }
            if (StepButton == "add7")
            {
                ViewBag.ShownStep = 7;
            }
            if (StepButton == "add8")
            {
                ViewBag.ShownStep = 8;
            }


            ViewBag.expId = step.exp_id;
            var userId = User.Identity.GetUserId();
            var userInfo = new UserInfo(userId);
            var groupInfo = new GroupInfo(userInfo.PersonId);

            //get all the peopleId in the group
            var groupPeopleIds = groupInfo.groupPeopleId;

            //generate exp type list
            List<ExpType> types = new List<ExpType>();
            var type1 = new ExpType();
            type1.id = 1;
            type1.Name = "Restriction Enzyme Digestion";
            types.Add(type1);
            var type2 = new ExpType();
            type2.id = 2;
            type2.Name = "Plasmid Transformation";
            types.Add(type2);
            var type3 = new ExpType();
            type3.id = 3;
            type3.Name = "Plasmid Miniprep";
            types.Add(type3);
            var type4 = new ExpType();
            type4.id = 4;
            type4.Name = "Fragment Gel Extraction";
            types.Add(type4);
            var type5 = new ExpType();
            type5.id = 5;
            type5.Name = "PCR";
            types.Add(type5);
            var type6 = new ExpType();
            type6.id = 6;
            type6.Name = "Ligation";
            types.Add(type6);
            var type7 = new ExpType();
            type7.id = 7;
            type7.Name = "Pick Colonies";
            types.Add(type7);
            var type8 = new ExpType();
            type8.id = 8;
            type8.Name = "Plasmid Maxiprep";
            types.Add(type8);

            var listItems = types.OrderBy(g => g.Name);
            ViewBag.types = new SelectList(listItems, "id", "name");

            //pass plasmid json data
            //get shared plasmids
            var sharedPlasmidId = db.group_shared.Where(c => c.category == "plasmid").Where(g => groupInfo.groupId.Contains(g.group_id)).Select(p => p.resource_id).ToList();
            //my plasmidId
            var myPlasmidId = db.plasmids.Where(p => p.people_id == userInfo.PersonId && !sharedPlasmidId.Contains(p.id)).Select(p => p.id).ToList();
            //get combinedId
            var comPlasmidId = sharedPlasmidId.Concat(myPlasmidId).ToList();
            //pass protocol json data
            var plasmids = db.plasmids.Where(p => comPlasmidId.Contains(p.id)).Select(p => new
            {
                id = p.id,
                name = p.name
            });
            ViewBag.Plasmids = JsonConvert.SerializeObject(plasmids.ToList());
            //pass protocol id
            var protocols = db.protocols.Where(p => groupPeopleIds.Contains((int)p.people_id)).Select(p => new
            {
                id = p.id,
                name = p.name,
                version = p.version
            });
            ViewBag.Protocols = JsonConvert.SerializeObject(protocols.ToList());


            //pass fragments
            //get group shared vectors
            //get the admin group ids (appAdmin, InstAdmin)
            List<int> adminId = new List<int>();
            List<string> adminNames = new List<string>();
            adminNames.Add("appAdmin");
            adminNames.Add("Institute Admin");
            var adminInfo = new AdminInfo();
            adminId = adminInfo.AdminId(adminNames);

            var sharedFragementId = db.group_shared.Where(g => groupInfo.groupId.Contains(g.group_id) || adminId.Contains(g.group_id)).Where(c => c.category == "fragment").Select(r => r.resource_id).ToList();
            //get the fragment of the current user
            var myFragmentId = db.fragments.Where(f => f.people_id == userInfo.PersonId && !sharedFragementId.Contains(f.id)).Select(f => f.id).ToList();
            //combnined
            var comFragmentId = sharedFragementId.Concat(myFragmentId).ToList();

            //for drawing maps of fragment
            IList<FragmentViewModel> fragments = new List<FragmentViewModel>();
            var allFragments = db.fragments.Where(f => comFragmentId.Contains(f.id)).OrderByDescending(f => f.name).ToList();
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
                    var features = db.fragment_map.Where(f => f.fragment_id == item.id).Where(f => f.feature_id != 4).OrderBy(s => s.start);
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
                    var fragment = new FragmentViewModel();
                    string decodeName = "";
                    if (item.plasmid_id != null)
                    {
                        decodeName = db.plasmids.Find((int)item.plasmid_id).name + " [" + enzymes[0] + "/" + item.name.Split('-')[2] + "," + enzymes[1] + "/" + item.name.Split('-')[3] + "]";
                    }
                    fragment.featureArray = featuresArray;
                    fragment.id = item.id;
                    fragment.fName = item.plasmid_id != null ? decodeName : item.name;
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
            ViewBag.FragmentMap = JsonConvert.SerializeObject(fragments.ToList());
            //for token input
            var Fragments = fragments.Select(f => new {
                id = f.id,
                name = f.fName.Split('[')[0],
                pos = "[" + f.fName.Split('[')[1]
            });
            ViewBag.Fragments = JsonConvert.SerializeObject(Fragments.ToList());

            //pass primers
            var primers = db.primers.Where(p => groupPeopleIds.Contains((int)p.people_id)).Select(p => new {
                id = p.id,
                name = p.name
            });
            ViewBag.Primers = JsonConvert.SerializeObject(primers.ToList());
            //pass probe
            var probes = db.probes.Where(p => groupPeopleIds.Contains((int)p.people_id)).Select(p => new {
                id = p.id,
                name = p.name
            });
            ViewBag.Probes = JsonConvert.SerializeObject(probes.ToList());











            //add data

            if (ModelState.IsValid)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        //get the new step_d 
                        var newStepId = 1;
                        var latestStep = db.exp_step.Where(s => s.exp_id == step.exp_id);
                        if (latestStep.Count() > 0)
                        {
                            newStepId = latestStep.OrderByDescending(s => s.step_id).FirstOrDefault().step_id + 1;
                        }
                        var expStep = new exp_step();
                        expStep.name = step.name;
                        expStep.exp_id = step.exp_id;
                        expStep.type_id = step.type_id;
                        if (step.protocol_id == 0)
                        {
                            expStep.protocol_id = null;
                        }
                        else
                        {
                            expStep.protocol_id = step.protocol_id;
                        }
                        expStep.des = step.des;
                        expStep.dt = DateTime.Now;
                        expStep.people_id = userInfo.PersonId;
                        expStep.step_id = newStepId;
                        db.exp_step.Add(expStep);
                        var material = new exp_step_material();
                        material.exp_id = step.exp_id;
                        material.exp_step_id = newStepId;
                        if(step.forward_primer_id == 0)
                        {
                            material.forward_primer_id = null;
                        }
                        else
                        {
                            material.forward_primer_id = step.forward_primer_id;
                        }
                        if (step.reverse_primer_id == 0)
                        {
                            material.reverse_primer_id = null;
                        }
                        else
                        {
                            material.reverse_primer_id = step.reverse_primer_id;
                        }
                        if (step.probe_id == 0)
                        {
                            material.probe_id = null;
                        }
                        else
                        {
                            material.probe_id = step.probe_id;
                        }
                        if (step.emzyme_id == "")
                        {
                            material.emzyme_id = "";
                        }
                        else
                        {
                            material.emzyme_id = step.emzyme_id;
                        }
                        if (step.plasmid_id==0)
                        {
                            material.plasmid_id = null;
                        }
                        else
                        {
                            material.plasmid_id = step.plasmid_id;
                        }
                        if (step.frag1_id == 0)
                        {
                            material.frag1_id = null;
                        }
                        else
                        {
                            material.frag1_id = step.frag1_id;
                        }
                        if (step.frag2_id == 0)
                        {
                            material.frag2_id = null;
                        }
                        else
                        {
                            material.frag2_id = step.frag2_id;
                        }
                        material.dt = DateTime.Now;
                        db.exp_step_material.Add(material);
                        //need to add ligaiton direction data
                        db.SaveChanges();
                        scope.Complete();
                        TempData["msg"] = "Experiment Step added added!";
                        return RedirectToAction("Details", "Experiment", new { id = step.exp_id});
                    }
                    catch (Exception)
                    {
                        scope.Dispose();
                        ModelState.AddModelError("", "Something went wrong, please try again!");
                        return View(step);
                    }
                }
            }

            return View();
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