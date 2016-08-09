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
using System.IO;
using System.Drawing;

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
            //exp shared with a person, not with a group
            var sharedExps = db.exp_share.Where(s => s.people_id == userInfo.PersonId);
            List<int> sharedIds = new List<int>();
            if (sharedExps.Count() > 0)
            {
                sharedIds = sharedExps.Select(r => r.exp_id).ToList();
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
        public ActionResult Details(int? id)
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
            var isMine = false;
            var isShared = false;
            if(exp.people_id == userInfo.PersonId)
            {
                isMine = true;
            }
            //check whether is shared
            var sharedExp = db.exp_share.Where(e => e.exp_id == exp.id);
            if (sharedExp.Count() > 0)
            {
                isShared = true;
            }
            var allowedAction = "Nothing";
            if(isMine && isShared)
            {
                allowedAction = "unShare";
            }
            if (isMine && !isShared)
            {
                allowedAction = "Share";
            }
            ViewBag.AllowedAction = allowedAction;

            //load data into view model
            var data = new ViewExp();
            data.id = exp.id;
            data.des = exp.des;
            data.name = exp.name;
            data.dt = exp.dt;
            var peopleInfo = new PeopleInfo(exp.people_id);
            data.owner = peopleInfo.Name;

            //get steps
            var steps = new List<ExpStep>();
            var expStep = db.exp_step.Where(e => e.exp_id == exp.id).OrderBy(s=>s.step_id).OrderBy(s=> s.sort_id == null? -1: s.sort_id); //need to order by another column
            if (expStep.Count() > 0)
            {
                foreach(var s in expStep)
                {
                    var step = new ExpStep();
                    step.id = s.id;
                    step.name = s.name;
                    step.type_id = s.type_id;
                    step.exp_id = exp.id;
                    step.protocol_id = s.protocol_id;
                    step.des = s.des;
                    step.dt = s.dt;
                    var steppeopleInfo = new PeopleInfo(s.people_id);
                    step.step_owner = steppeopleInfo.Name;

                    //check whether the step has results
                    var results = db.exp_step_result.Where(r => r.exp_step_id == s.step_id);
                    if (results.Count() > 0)
                    {
                        step.hasResult = true;
                    }
                    else
                    {
                        step.hasResult = false;
                    }

                    steps.Add(step);
                }
            }
            data.steps = steps;
            
            return View(data);
        }

        [Authorize]
        [HttpPost]
        public ActionResult SortStep(int? id, string stepId)
        {
            if (id == null)
            {
                return Json(new { result = false });
            }
            experiment exp = db.experiments.Find(id);
            if (exp == null)
            {
                return Json(new { result = false });
            }
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    var IdArray = JsonConvert.DeserializeObject<int[]>(stepId);
                    //need to add a column to sort table
                    for(var i=0; i<IdArray.Length; i++)
                    {
                        var step = db.exp_step.Find(IdArray[i]);
                        step.sort_id = i + 1;
                        db.SaveChanges();
                    }
                    scope.Complete();
                    return Json(new { result = true});
                }
                catch(Exception)
                {
                    scope.Dispose();
                    return Json(new { result = false });
                }
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult StepDetails(int? id)
        {
            //id is the step id
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var step = db.exp_step.Find(id);
            if (step == null)
            {
                return HttpNotFound();
            }
            ViewBag.expId = step.exp_id;
            //get the material
            var material = db.exp_step_material.Where(m => m.exp_id == step.exp_id && m.exp_step_id == step.step_id).FirstOrDefault();
            
            //load data into ViewModel
            var model = new ExpStep();
            model.id = step.id;
            model.name = step.name;
            model.type_id = step.type_id;
            model.exp_id = step.exp_id;
            model.protocol_id = step.protocol_id;
            model.des = step.des;
            model.dt = step.dt;

            model.forward_primer_id = material.forward_primer_id;
            model.reverse_primer_id = material.reverse_primer_id;
            model.probe_id = material.probe_id;
            model.emzyme_id = material.emzyme_id;
            model.plasmid_id = material.plasmid_id;
            model.frag1_id = material.frag1_id;
            model.frag2_id = material.frag2_id;
            model.ligation_method = material.ligation_method;
            model.ligation_direction = material.ligation_direction;
            model.nplasmid_id = material.nplasmid_id;

            //get the plasmid feautures
            dynamic features = null;
            if (model.plasmid_id != null)
            {
                features = db.plasmid_map.Where(p => p.plasmid_id == (int)model.plasmid_id).Where(f => f.feature_id != 4).OrderBy(p => p.plasmid_id).OrderBy(s => s.start).Select(f => new { pId = f.plasmid.id, pName = f.plasmid.name, pSeqCount = f.plasmid.seq_length, show_feature = f.show_feature, end = f.end, feature = f.common_feature != null ? f.common_feature.label : f.feature, type_id = f.feature_id, start = f.start, cut = f.cut, clockwise = f.clockwise == 1 ? true : false }).ToList();
            }
            ViewBag.Features = JsonConvert.SerializeObject(features);

            //get the fragments features
            var fragment1 = new FragmentViewModel();
            if (model.frag1_id != null)
            {
                var frag1 = db.fragments.Find(model.frag1_id);
                //convert enzyme id into enzyme names
                var enzymeIds = new List<int>();
                var enzymes = new List<string>();
                var enzymeIdStringArray = frag1.enzyme_id.Split(',');
                enzymeIds.Add(Int32.Parse(enzymeIdStringArray[0]));
                enzymeIds.Add(Int32.Parse(enzymeIdStringArray[1]));
                var enzyme1 = db.restri_enzyme.Find(enzymeIds[0]).name;
                enzymes.Add(enzyme1);
                var enzyme2 = db.restri_enzyme.Find(enzymeIds[1]).name;
                enzymes.Add(enzyme2);

                //find the features
                IList<fragmentFeatures> featuresArray = new List<fragmentFeatures>();
                var fragFeatures = db.fragment_map.Where(f => f.fragment_id == frag1.id).OrderBy(s => s.start);
                if (fragFeatures.Count() > 0)
                {
                    foreach (var feature in fragFeatures)
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
                fragment1.featureArray = featuresArray;
                fragment1.id = frag1.id;
                fragment1.fName = frag1.name;
                fragment1.plasmid_id = (int)id;
                fragment1.enzymes = enzymes;
                fragment1.f_start = (int)frag1.forward_start;
                fragment1.f_end = (int)frag1.forward_end;
                fragment1.fSeq = frag1.forward_seq;
                //overhangs
                var overhangs = new List<int>();
                overhangs.Add((int)frag1.rc_left_overhand);
                overhangs.Add((int)frag1.rc_right_overhand);
                fragment1.overhangs = overhangs;
                fragment1.cSeq = frag1.rc_seq;

            }
            ViewBag.Fragment1 = JsonConvert.SerializeObject(fragment1);


            //fragment2
            var fragment2 = new FragmentViewModel();
            if (model.frag2_id != null)
            {
                var frag2 = db.fragments.Find(model.frag2_id);
                //convert enzyme id into enzyme names
                var enzymeIds = new List<int>();
                var enzymes = new List<string>();
                var enzymeIdStringArray = frag2.enzyme_id.Split(',');
                enzymeIds.Add(Int32.Parse(enzymeIdStringArray[0]));
                enzymeIds.Add(Int32.Parse(enzymeIdStringArray[1]));
                var enzyme1 = db.restri_enzyme.Find(enzymeIds[0]).name;
                enzymes.Add(enzyme1);
                var enzyme2 = db.restri_enzyme.Find(enzymeIds[1]).name;
                enzymes.Add(enzyme2);

                //find the features
                IList<fragmentFeatures> featuresArray = new List<fragmentFeatures>();
                var fragFeatures = db.fragment_map.Where(f => f.fragment_id == frag2.id).OrderBy(s => s.start);
                if (fragFeatures.Count() > 0)
                {
                    foreach (var feature in fragFeatures)
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
                fragment2.featureArray = featuresArray;
                fragment2.id = frag2.id;
                fragment2.fName = frag2.name;
                fragment2.plasmid_id = (int)id;
                fragment2.enzymes = enzymes;
                fragment2.f_start = (int)frag2.forward_start;
                fragment2.f_end = (int)frag2.forward_end;
                fragment2.fSeq = frag2.forward_seq;
                //overhangs
                var overhangs = new List<int>();
                overhangs.Add((int)frag2.rc_left_overhand);
                overhangs.Add((int)frag2.rc_right_overhand);
                fragment2.overhangs = overhangs;
                fragment2.cSeq = frag2.rc_seq;

            }
            ViewBag.Fragment2 = JsonConvert.SerializeObject(fragment2);

            //parse plasmids generated form ligation
            var nPlasmid = new List<int>();
            var npIdNames = new List<ecloning.Models.pIdName>();
            if (model.nplasmid_id != null && model.nplasmid_id != "")
            {
                if (model.nplasmid_id != null)
                {
                    if (model.nplasmid_id.IndexOf("-") != -1)
                    {
                        var nplasmidId = model.nplasmid_id.Split('-');
                        foreach (var item in nplasmidId)
                        {
                            nPlasmid.Add(Int32.Parse(item));
                        }
                    }
                    else
                    {
                        //only one plasmid
                        var pid = Int32.Parse(model.nplasmid_id);
                        nPlasmid.Add(pid);
                    }
                }
                //convert id and name
                foreach (var p in nPlasmid)
                {
                    var npIdName = new ecloning.Models.pIdName();
                    npIdName.id = p;
                    npIdName.name = npIdName.getName(p);
                    npIdNames.Add(npIdName);
                }
            }            
            ViewBag.npIdNames = npIdNames;

            //get the nplasmid feautures
            dynamic nfeatures = null;
            if (model.nplasmid_id != null)
            {
                nfeatures = db.plasmid_map.Where(p => nPlasmid.Contains(p.plasmid_id)).Where(f => f.feature_id != 4).OrderBy(p => p.plasmid_id).OrderBy(s => s.start).Select(f => new { pId = f.plasmid.id, pName = f.plasmid.name, pSeqCount = f.plasmid.seq_length, show_feature = f.show_feature, end = f.end, feature = f.common_feature != null ? f.common_feature.label : f.feature, type_id = f.feature_id, start = f.start, cut = f.cut, clockwise = f.clockwise == 1 ? true : false }).ToList();
            }
            ViewBag.nFeatures = JsonConvert.SerializeObject(nfeatures);
            ViewBag.nPlasmid = JsonConvert.SerializeObject(nPlasmid);

            //load results
            var res = new List<ExpResult>();
            var stepResults = db.exp_step_result.Where(r => r.exp_id == step.exp_id && r.exp_step_id == step.step_id);
            if (stepResults.Count() > 0)
            {
                model.hasResult = true;
                foreach (var item in stepResults)
                {
                    var exp_res = new ExpResult();
                    exp_res.id = item.id;
                    exp_res.exp_id = step.exp_id;
                    exp_res.exp_step_id = step.step_id;
                    exp_res.result_des = lnbrConvert.br2ln(item.result_des);
                    exp_res.result_dt = item.dt;
                    exp_res.result_id = item.result_id;
                    exp_res.result_upload = item.result_upload;
                    exp_res.type_id = step.type_id;
                    res.Add(exp_res);
                }
            }
            model.results = res;
            return View(model);
        }


        [Authorize]
        [HttpGet]
        public ActionResult AddResult(int? id, int? type_id)
        {
            //id is the step id
            if (id == null || type_id == null || type_id < 1 || type_id > 8)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var step = db.exp_step.Find(id);
            if (step == null)
            {
                return HttpNotFound();
            }
            ViewBag.ExpId = step.exp_id; //exp_id
            ViewBag.StepId = step.step_id; //step id
            ViewBag.TypeId = type_id; //type_id
            ViewBag.Id = id; //step table id
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddResult([Bind(Include = "id,exp_id,exp_step_id,type_id,result_upload,result_des,result_dt")] ExpResult result)
        {
            ViewBag.ExpId = result.exp_id; //exp_id
            ViewBag.StepId = result.exp_step_id; //step id
            ViewBag.TypeId = result.type_id; //type_id
            //get the step
            var step = db.exp_step.Where(s => s.exp_id == result.exp_id && s.step_id == result.exp_step_id).FirstOrDefault();
            ViewBag.Id = step.id;

            if (ModelState.IsValid)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    ///prepare upload
                    //parse exp type name
                    var type = new ecloning.Models.ExpTypes();
                    var timeStamp = DateTime.Now.Millisecond.ToString() + "$" + type.getName(result.type_id) + "%"; //timestap+expType
                    string fileName = null;

                    try
                    {
                        var stepResult = new exp_step_result();
                        stepResult.exp_id = result.exp_id;
                        stepResult.exp_step_id = result.exp_step_id;
                        stepResult.result_des = ecloning.Models.lnbrConvert.ln2br(result.result_des);
                        stepResult.dt = result.result_dt;
                        //get result id
                        int ResultId = 1;
                        var preResults = db.exp_step_result.Where(r => r.exp_id == result.exp_id && r.exp_step_id == result.exp_step_id);
                        if (preResults.Count() > 0)
                        {
                            ResultId = preResults.OrderByDescending(r => r.result_id).FirstOrDefault().result_id + 1;
                        }
                        stepResult.result_id = ResultId;


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
                                    azureBlob.directoryName = eCloningSettings.expDataDir;
                                    azureBlob.AzureBlobUpload(fileName, file);

                                    if (ImageExtensionCheck.IsImage(fileName) == true)
                                    {
                                        //generate the thumbnail and upload azure
                                        //thubnail add 'tb-' before fielName
                                        azureBlob.AzureBlobTBUpload(fileName, "img_fn");
                                    }                                    
                                }
                                catch (Exception)
                                {
                                    ModelState.AddModelError("", "File upload failed!");
                                    //delete from azure
                                    AzureBlob azureBlob = new AzureBlob();
                                    azureBlob.directoryName = eCloningSettings.expDataDir;
                                    azureBlob.AzureBlobDelete(fileName);
                                    azureBlob.AzureBlobDelete("tb-" + fileName);

                                    return View(result);
                                }
                            }
                        }
                        else
                        {
                            //upload to local plasmid folder
                            var expDataPath = eCloningSettings.filePath + eCloningSettings.expDataDir;
                            if (file != null && file.FileName != null && file.ContentLength > 0)
                            {
                                try
                                {
                                    fileName = timeStamp + Path.GetFileName(file.FileName);
                                    //fileExtension = Path.GetExtension(file.FileName);
                                    var path = Path.Combine(Server.MapPath(expDataPath), fileName);
                                    file.SaveAs(path);

                                    if (ImageExtensionCheck.IsImage(fileName) == true)
                                    {
                                        //generate thunmbnail and save tb
                                        //thubnail add 'tb-' before fielName
                                        var tb = new ThumbnailLocal();
                                        tb.SaveLocal(expDataPath, fileName, "img_fn");
                                    }
                                    
                                }
                                catch (Exception)
                                {
                                    ModelState.AddModelError("", "File upload failed!");

                                    //delete from local the original file
                                    string path1 = Request.MapPath(eCloningSettings.filePath + eCloningSettings.expDataDir + "/" + fileName);
                                    if (System.IO.File.Exists(path1))
                                    {
                                        System.IO.File.Delete(path1);
                                    }
                                    //thunbnail
                                    string path2 = Request.MapPath(eCloningSettings.filePath + eCloningSettings.expDataDir + "/tb-" + fileName);
                                    if (System.IO.File.Exists(path2))
                                    {
                                        System.IO.File.Delete(path2);
                                    }

                                    return View(result);
                                }
                            }
                        }

                        stepResult.result_upload = fileName;
                        db.exp_step_result.Add(stepResult);
                        db.SaveChanges();
                        scope.Complete();
                        TempData["msg"] = "Result added!";
                        return RedirectToAction("StepDetails", "Experiment", new { id = step.id });

                    }
                    catch (Exception)
                    {
                        scope.Dispose();
                        TempData["msg"] = "Something went wroing, result NOT added!";
                        //remvoe upload
                        if (eCloningSettings.AppHosting == "Cloud")
                        {
                            //delete from azure
                            AzureBlob azureBlob = new AzureBlob();
                            azureBlob.directoryName = eCloningSettings.expDataDir;
                            azureBlob.AzureBlobDelete(fileName);
                            azureBlob.AzureBlobDelete("tb-"+fileName);
                        }
                        else
                        {
                            //delete from local the original file
                            string path1 = Request.MapPath(eCloningSettings.filePath + eCloningSettings.expDataDir + "/" + fileName);
                            if (System.IO.File.Exists(path1))
                            {
                                System.IO.File.Delete(path1);
                            }
                            //thunbnail
                            string path2 = Request.MapPath(eCloningSettings.filePath + eCloningSettings.expDataDir + "/tb-" + fileName);
                            if (System.IO.File.Exists(path2))
                            {
                                System.IO.File.Delete(path2);
                            }
                        }
                        return RedirectToAction("StepDetails", "Experiment", new { id = step.id });
                    }
                }  
            }
            
            return View(result);
        }


        [Authorize]
        public ActionResult Download(string fileName, string actionName, int id)
        {
            if (eCloningSettings.AppHosting == "Cloud")
            {
                //download from azure
                AzureBlob azureBlob = new AzureBlob();
                azureBlob.directoryName = eCloningSettings.expDataDir;
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
                    var expDataPath = eCloningSettings.filePath + eCloningSettings.expDataDir;
                    var path = Path.Combine(Server.MapPath(expDataPath), fileName);
                    byte[] fileBytes = System.IO.File.ReadAllBytes(path);
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }
                catch (Exception)
                {
                    return RedirectToAction("FileError");
                }
            }
            return RedirectToAction(actionName, "Experiment", new { id = id});
        }

        [Authorize]
        [HttpGet]
        public ActionResult FileError()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult DisplayImage(string fileName, int step_table_id, string des)
        {
            ViewBag.fileName = fileName;
            ViewBag.Id = step_table_id;
            ViewBag.Des = des;
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult DeleteResult(int? id)
        { 
            //id is the result id
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var res = db.exp_step_result.Find(id);
            if (res == null)
            {
                return HttpNotFound();
            }
            //step table id
            var step = db.exp_step.Where(s => s.exp_id == res.exp_id && s.step_id == res.exp_step_id).FirstOrDefault();
            ViewBag.Id = id;
            ViewBag.StepTableId = step.id;
            ViewBag.ExpId = step.exp_id;
            return View(res);
        }

        [Authorize, ActionName("DeleteResult")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteResultConfirmed(int id)
        {
            var res = db.exp_step_result.Find(id);
            try
            {
                db.exp_step_result.Remove(res);
                db.SaveChanges();
                //remvoe upload
                if (eCloningSettings.AppHosting == "Cloud")
                {
                    //delete from azure
                    AzureBlob azureBlob = new AzureBlob();
                    azureBlob.directoryName = eCloningSettings.expDataDir;
                    azureBlob.AzureBlobDelete(res.result_upload);
                    azureBlob.AzureBlobDelete("tb-" + res.result_upload);
                }
                else
                {
                    //delete from local the original file
                    string path1 = Request.MapPath(eCloningSettings.filePath + eCloningSettings.expDataDir + "/" + res.result_upload);
                    if (System.IO.File.Exists(path1))
                    {
                        System.IO.File.Delete(path1);
                    }
                    //thunbnail
                    string path2 = Request.MapPath(eCloningSettings.filePath + eCloningSettings.expDataDir + "/tb-" + res.result_upload);
                    if (System.IO.File.Exists(path2))
                    {
                        System.IO.File.Delete(path2);
                    }
                }
                TempData["msg"] = "Result deleted!";
                var sId = db.exp_step.Where(s => s.exp_id == res.exp_id && s.step_id == res.exp_step_id).FirstOrDefault().id;
                return RedirectToAction("StepDetails", "Experiment", new { id = sId });
            }
            catch (Exception)
            {
                TempData["msg"] = "Result NOT deleted!";
                var sId = db.exp_step.Where(s => s.exp_id == res.exp_id && s.step_id == res.exp_step_id).FirstOrDefault().id;
                return RedirectToAction("StepDetails", "Experiment", new { id = sId });
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult DeleteStep(int? id)
        {
            //id is the step id
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var step = db.exp_step.Find(id);
            if (step == null)
            {
                return HttpNotFound();
            }
            ViewBag.expId = step.exp_id;
            return View(step);
        }

        [Authorize, ActionName("DeleteStep")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteStepConfirmed(int id)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                var step = db.exp_step.Find(id);
                try 
                {                
                    //find material
                    var material = db.exp_step_material.Where(m => m.exp_id == step.exp_id && m.exp_step_id == step.step_id).FirstOrDefault();
                    db.exp_step_material.Remove(material);
                    db.exp_step.Remove(step);
                    db.SaveChanges();
                    scope.Complete();
                    return RedirectToAction("Details", "Experiment", new { id = step.exp_id });
                }
                catch (Exception)
                {
                    scope.Dispose();
                    return RedirectToAction("Details", "Experiment", new { id = step.exp_id });
                }
            }

        }

        [Authorize]
        [HttpGet]
        public ActionResult Edit(int? id)
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
            var view = new ExperimentViewModal();
            view.id = exp.id;
            view.name = exp.name;
            view.des = lnbrConvert.br2ln(exp.des);
            return View(view);
        }
        [Authorize]
        [HttpPost]
        public ActionResult Edit([Bind(Include = "id,name,des")] ExperimentViewModal experiment)
        {
            if (ModelState.IsValid)
            {
                //convert break                
                var exp = db.experiments.Find(experiment.id);
                exp.name = experiment.name;
                exp.des = lnbrConvert.ln2br(experiment.des);
                exp.dt = DateTime.Now;
                db.SaveChanges();
            }
            return View(experiment);
        }


        [Authorize]
        [HttpGet]
        public ActionResult Share(int? id)
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

            //get the group info
            //get current login email
            var userId = User.Identity.GetUserId();
            //get people id
            var userInfo = new UserInfo(userId);
            int peopleId = userInfo.PersonId;

            //get person id and name 
            List<PeopleIdName> pIdNames  = new List<PeopleIdName>();
            var people = db.people.Where(p=>p.id != userInfo.PersonId);
            if (people.Count() > 0)
            {
                foreach(var p in people)
                {
                    PeopleIdName pIdName = new PeopleIdName();
                    pIdName.id = p.id;
                    pIdName.Name = p.first_name + " " + p.last_name;
                    pIdNames.Add(pIdName);
                }
            }
            var listItems = pIdNames.OrderBy(g => g.Name);
            ViewBag.person = new SelectList(listItems, "id", "name");

            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult unShare(int? id)
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

            var shared = db.exp_share.Where(s => s.exp_id == exp.id);
            if (shared.Count() > 0)
            {
                foreach (var item in shared)
                {
                    db.exp_share.Remove(item);                    
                }
                db.SaveChanges();
                TempData["msg"] = "Experiment is not shared anymore!";
                return RedirectToAction("Details", "Experiment", new { id = exp.id });
            }
            else
            {
                TempData["msg"] = "Something went wrong, this experiment is still shared!";
                return RedirectToAction("Details", "Experiment", new { id = exp.id });
            }

        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Share([Bind(Include = "id,exp_id,person")] CollaborationExp colExp)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    foreach(var p in colExp.person.ToList())
                    {
                        var share = new exp_share();
                        share.exp_id = colExp.exp_id;
                        share.people_id = p;
                        share.dt = DateTime.Now;
                        db.exp_share.Add(share);
                    }                   
                    db.SaveChanges();
                    TempData["msg"] = "Experiment Shared!";
                    return RedirectToAction("Details", "Experiment", new { id = colExp.exp_id });

                }
                catch (Exception)
                {
                    TempData["msg"] = "Share experiment failed! please try again";
                    return RedirectToAction("Details", "Experiment", new { id = colExp.exp_id });
                }
            }
            return RedirectToAction("Details", "Experiment", new { id = colExp.exp_id });
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
                exp.des = lnbrConvert.ln2br(experiment.des);
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
                pos = f.fName.IndexOf('[') != -1 ? "["+f.fName.Split('[')[1] : ""
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


            //pass all restic-enzyme
            var restrics = db.restri_enzyme.Select(r => new
            {
                id = r.id,
                name = r.name
            });
            ViewBag.Restrics = JsonConvert.SerializeObject(restrics.ToList());

            //pass cut prototype and cut properties
            string symbol = "<span></span>";
            var enzymeSybol = new RestrictionEnzyme();

            var restric_property = db.restri_enzyme.OrderBy(n => n.name);
            List<RestrictionObject> restrictons = new List<RestrictionObject>();
            foreach (var item in restric_property)
            {
                var rObject = new RestrictionObject();
                rObject.name = item.name;
                rObject.Id = item.id;
                rObject.prototype = (item.forward_cut2 != null && item.reverse_cut2 != null) ? (enzymeSybol.ShowPrototype2(item.forward_seq, item.forward_cut, item.reverse_cut, (int)item.forward_cut2, (int)item.reverse_cut2)) : enzymeSybol.ShowPrototype(item.forward_seq, item.forward_cut, item.reverse_cut);
                rObject.startActivity = item.staractitivity == true ? enzymeSybol.StarActivitySymbol((bool)item.staractitivity) : symbol;
                rObject.heatInactivation = item.inactivation != null ? enzymeSybol.InactivationSymbol((int)item.inactivation) : enzymeSybol.InactivationSymbol(0);
                rObject.dam = item.dam == true ? enzymeSybol.DamSymbol((bool)item.dam) : symbol;
                rObject.dcm = item.dcm == true ? enzymeSybol.DcmSymbol((bool)item.dcm) : symbol;
                rObject.cpg = item.cpg == true ? enzymeSybol.CpGSymbol((bool)item.cpg) : symbol;
                rObject.forward_cut = item.forward_cut;
                rObject.reverse_cut = item.reverse_cut;
                rObject.forward_cut2 = item.forward_cut2;
                rObject.reverse_cut2 = item.reverse_cut2;
                restrictons.Add(rObject);
            }
            ViewBag.restric_property = JsonConvert.SerializeObject(restrictons.ToList());

            ViewBag.ShownStep = -1;
            return View();
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddStep(string StepButton, [Bind(Include = "id,types,name,type_id,exp_id,protocol_id,forward_primer_id,reverse_primer_id,probe_id,emzyme_id,plasmid_id,frag1_id,frag2_id,ligation_method,ligation_direction,des,map1_seq,map2_seq,map3_seq,map4_seq,plasmidName")] ExpStep step)
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
                pos = f.fName.IndexOf('[') != -1 ? "[" + f.fName.Split('[')[1] : ""
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

            //pass all restic-enzyme
            var restrics = db.restri_enzyme.Select(r => new
            {
                id = r.id,
                name = r.name
            });
            ViewBag.Restrics = JsonConvert.SerializeObject(restrics.ToList());

            //pass cut prototype and cut properties
            string symbol = "<span></span>";
            var enzymeSybol = new RestrictionEnzyme();

            var restric_property = db.restri_enzyme.OrderBy(n => n.name);
            List<RestrictionObject> restrictons = new List<RestrictionObject>();
            foreach (var item in restric_property)
            {
                var rObject = new RestrictionObject();
                rObject.name = item.name;
                rObject.Id = item.id;
                rObject.prototype = (item.forward_cut2 != null && item.reverse_cut2 != null) ? (enzymeSybol.ShowPrototype2(item.forward_seq, item.forward_cut, item.reverse_cut, (int)item.forward_cut2, (int)item.reverse_cut2)) : enzymeSybol.ShowPrototype(item.forward_seq, item.forward_cut, item.reverse_cut);
                rObject.startActivity = item.staractitivity == true ? enzymeSybol.StarActivitySymbol((bool)item.staractitivity) : symbol;
                rObject.heatInactivation = item.inactivation != null ? enzymeSybol.InactivationSymbol((int)item.inactivation) : enzymeSybol.InactivationSymbol(0);
                rObject.dam = item.dam == true ? enzymeSybol.DamSymbol((bool)item.dam) : symbol;
                rObject.dcm = item.dcm == true ? enzymeSybol.DcmSymbol((bool)item.dcm) : symbol;
                rObject.cpg = item.cpg == true ? enzymeSybol.CpGSymbol((bool)item.cpg) : symbol;
                rObject.forward_cut = item.forward_cut;
                rObject.reverse_cut = item.reverse_cut;
                rObject.forward_cut2 = item.forward_cut2;
                rObject.reverse_cut2 = item.reverse_cut2;
                restrictons.Add(rObject);
            }
            ViewBag.restric_property = JsonConvert.SerializeObject(restrictons.ToList());



            //add data

            if (ModelState.IsValid)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        string nPlasmids = ""; //should be '-' seperately plasmid id
                        // if the step is ligation
                        if(StepButton == "add6")
                        {
                            //ligation, need to first to save plasmid
                            var method = step.ligation_method; //D, I, B
                            var direction = step.ligation_direction; //CX, AX, BX, XC, XA, XB
                            var plasmidName = step.plasmidName;
                            if(method == "D")
                            {
                                var sub_direction = direction.Substring(0, 1);
                                if (sub_direction == "C")
                                {
                                    var plasmid = new plasmid();
                                    plasmid.name = plasmidName + "_DC";
                                    plasmid.sequence = step.map1_seq;
                                    plasmid.seq_length = step.map1_seq.Length;
                                    plasmid.people_id = userInfo.PersonId;
                                    db.plasmids.Add(plasmid);
                                    db.SaveChanges();
                                    nPlasmids = plasmid.id.ToString();
                                    if (plasmid.sequence != null)
                                    {
                                        //auto generate features
                                        var autoFeatures = new PlasmidFeature(plasmid.id, plasmid.sequence, groupInfo.groupId);
                                    }
                                }
                                else if(sub_direction == "A")
                                {
                                    var plasmid = new plasmid();
                                    plasmid.name = plasmidName + "_DA";
                                    plasmid.sequence = step.map2_seq;
                                    plasmid.seq_length = step.map2_seq.Length;
                                    plasmid.people_id = userInfo.PersonId;
                                    db.plasmids.Add(plasmid);
                                    db.SaveChanges();
                                    nPlasmids = plasmid.id.ToString();
                                    if (plasmid.sequence != null)
                                    {
                                        //auto generate features
                                        var autoFeatures = new PlasmidFeature(plasmid.id, plasmid.sequence, groupInfo.groupId);
                                    }
                                }
                                else
                                {
                                    //B
                                    //C
                                    var plasmid1 = new plasmid();
                                    plasmid1.name = plasmidName + "_DC";
                                    plasmid1.sequence = step.map1_seq;
                                    plasmid1.seq_length = step.map1_seq.Length;
                                    plasmid1.people_id = userInfo.PersonId;
                                    db.plasmids.Add(plasmid1);
                                    db.SaveChanges();
                                    nPlasmids = plasmid1.id.ToString();
                                    if (plasmid1.sequence != null)
                                    {
                                        //auto generate features
                                        var autoFeatures = new PlasmidFeature(plasmid1.id, plasmid1.sequence, groupInfo.groupId);
                                    }
                                    //A
                                    var plasmid = new plasmid();
                                    plasmid.name = plasmidName + "_DA";
                                    plasmid.sequence = step.map2_seq;
                                    plasmid.seq_length = step.map2_seq.Length;
                                    plasmid.people_id = userInfo.PersonId;
                                    db.plasmids.Add(plasmid);
                                    db.SaveChanges();
                                    nPlasmids += "-";
                                    nPlasmids += plasmid.id.ToString();
                                    if (plasmid.sequence != null)
                                    {
                                        //auto generate features
                                        var autoFeatures = new PlasmidFeature(plasmid.id, plasmid.sequence, groupInfo.groupId);
                                    }
                                }
                            }
                            else if(method == "I")
                            {
                                var sub_direction = direction.Substring(1, 1);
                                if (sub_direction == "C")
                                {
                                    var plasmid = new plasmid();
                                    plasmid.name = plasmidName + "_IC";
                                    plasmid.sequence = step.map3_seq;
                                    plasmid.seq_length = step.map3_seq.Length;
                                    plasmid.people_id = userInfo.PersonId;
                                    db.plasmids.Add(plasmid);
                                    db.SaveChanges();
                                    nPlasmids = plasmid.id.ToString();
                                    if (plasmid.sequence != null)
                                    {
                                        //auto generate features
                                        var autoFeatures = new PlasmidFeature(plasmid.id, plasmid.sequence, groupInfo.groupId);
                                    }
                                }
                                else if (sub_direction == "A")
                                {
                                    var plasmid = new plasmid();
                                    plasmid.name = plasmidName + "_IA";
                                    plasmid.sequence = step.map4_seq;
                                    plasmid.seq_length = step.map4_seq.Length;
                                    plasmid.people_id = userInfo.PersonId;
                                    db.plasmids.Add(plasmid);
                                    db.SaveChanges();
                                    nPlasmids = plasmid.id.ToString();
                                    if (plasmid.sequence != null)
                                    {
                                        //auto generate features
                                        var autoFeatures = new PlasmidFeature(plasmid.id, plasmid.sequence, groupInfo.groupId);
                                    }
                                }
                                else
                                {
                                    //B
                                    //C
                                    var plasmid1 = new plasmid();
                                    plasmid1.name = plasmidName + "_IC";
                                    plasmid1.sequence = step.map3_seq;
                                    plasmid1.seq_length = step.map3_seq.Length;
                                    plasmid1.people_id = userInfo.PersonId;
                                    db.plasmids.Add(plasmid1);
                                    db.SaveChanges();
                                    nPlasmids = plasmid1.id.ToString();
                                    if (plasmid1.sequence != null)
                                    {
                                        //auto generate features
                                        var autoFeatures = new PlasmidFeature(plasmid1.id, plasmid1.sequence, groupInfo.groupId);
                                    }
                                    //A
                                    var plasmid = new plasmid();
                                    plasmid.name = plasmidName + "_IA";
                                    plasmid.sequence = step.map4_seq;
                                    plasmid.seq_length = step.map4_seq.Length;
                                    plasmid.people_id = userInfo.PersonId;
                                    db.plasmids.Add(plasmid);
                                    db.SaveChanges();
                                    nPlasmids += "-";
                                    nPlasmids += plasmid.id.ToString();
                                    if (plasmid.sequence != null)
                                    {
                                        //auto generate features
                                        var autoFeatures = new PlasmidFeature(plasmid.id, plasmid.sequence, groupInfo.groupId);
                                    }
                                }
                            }
                            else
                            {
                                //B
                                //D
                                var sub_direction = direction.Substring(0, 1);
                                if (sub_direction == "C")
                                {
                                    var plasmid = new plasmid();
                                    plasmid.name = plasmidName + "_DC";
                                    plasmid.sequence = step.map1_seq;
                                    plasmid.seq_length = step.map1_seq.Length;
                                    plasmid.people_id = userInfo.PersonId;
                                    db.plasmids.Add(plasmid);
                                    db.SaveChanges();
                                    nPlasmids = plasmid.id.ToString();
                                    if (plasmid.sequence != null)
                                    {
                                        //auto generate features
                                        var autoFeatures = new PlasmidFeature(plasmid.id, plasmid.sequence, groupInfo.groupId);
                                    }
                                }
                                else if (sub_direction == "A")
                                {
                                    var plasmid = new plasmid();
                                    plasmid.name = plasmidName + "_DA";
                                    plasmid.sequence = step.map2_seq;
                                    plasmid.seq_length = step.map2_seq.Length;
                                    plasmid.people_id = userInfo.PersonId;
                                    db.plasmids.Add(plasmid);
                                    db.SaveChanges();
                                    nPlasmids = plasmid.id.ToString();
                                    if (plasmid.sequence != null)
                                    {
                                        //auto generate features
                                        var autoFeatures = new PlasmidFeature(plasmid.id, plasmid.sequence, groupInfo.groupId);
                                    }
                                }
                                else
                                {
                                    //B
                                    //C
                                    var plasmid1 = new plasmid();
                                    plasmid1.name = plasmidName + "_DC";
                                    plasmid1.sequence = step.map1_seq;
                                    plasmid1.seq_length = step.map1_seq.Length;
                                    plasmid1.people_id = userInfo.PersonId;
                                    db.plasmids.Add(plasmid1);
                                    db.SaveChanges();
                                    nPlasmids = plasmid1.id.ToString();
                                    if (plasmid1.sequence != null)
                                    {
                                        //auto generate features
                                        var autoFeatures = new PlasmidFeature(plasmid1.id, plasmid1.sequence, groupInfo.groupId);
                                    }
                                    //A
                                    var plasmid = new plasmid();
                                    plasmid.name = plasmidName + "_DA";
                                    plasmid.sequence = step.map2_seq;
                                    plasmid.seq_length = step.map2_seq.Length;
                                    plasmid.people_id = userInfo.PersonId;
                                    db.plasmids.Add(plasmid);
                                    db.SaveChanges();
                                    nPlasmids += "-";
                                    nPlasmids += plasmid.id.ToString();
                                    if (plasmid.sequence != null)
                                    {
                                        //auto generate features
                                        var autoFeatures = new PlasmidFeature(plasmid.id, plasmid.sequence, groupInfo.groupId);
                                    }
                                }

                                //I
                                var sub_direction1 = direction.Substring(1, 1);
                                if (sub_direction1 == "C")
                                {
                                    var plasmid = new plasmid();
                                    plasmid.name = plasmidName + "_IC";
                                    plasmid.sequence = step.map3_seq;
                                    plasmid.seq_length = step.map3_seq.Length;
                                    plasmid.people_id = userInfo.PersonId;
                                    db.plasmids.Add(plasmid);
                                    db.SaveChanges();
                                    nPlasmids += "-";
                                    nPlasmids += plasmid.id.ToString();
                                    if (plasmid.sequence != null)
                                    {
                                        //auto generate features
                                        var autoFeatures = new PlasmidFeature(plasmid.id, plasmid.sequence, groupInfo.groupId);
                                    }
                                }
                                else if (sub_direction1 == "A")
                                {
                                    var plasmid = new plasmid();
                                    plasmid.name = plasmidName + "_IA";
                                    plasmid.sequence = step.map4_seq;
                                    plasmid.seq_length = step.map4_seq.Length;
                                    plasmid.people_id = userInfo.PersonId;
                                    db.plasmids.Add(plasmid);
                                    db.SaveChanges();
                                    nPlasmids += "-";
                                    nPlasmids += plasmid.id.ToString();
                                    if (plasmid.sequence != null)
                                    {
                                        //auto generate features
                                        var autoFeatures = new PlasmidFeature(plasmid.id, plasmid.sequence, groupInfo.groupId);
                                    }
                                }
                                else
                                {
                                    //B
                                    //C
                                    var plasmid1 = new plasmid();
                                    plasmid1.name = plasmidName + "_IC";
                                    plasmid1.sequence = step.map3_seq;
                                    plasmid1.seq_length = step.map3_seq.Length;
                                    plasmid1.people_id = userInfo.PersonId;
                                    db.plasmids.Add(plasmid1);
                                    db.SaveChanges();
                                    nPlasmids += "-";
                                    nPlasmids += plasmid1.id.ToString();
                                    if (plasmid1.sequence != null)
                                    {
                                        //auto generate features
                                        var autoFeatures = new PlasmidFeature(plasmid1.id, plasmid1.sequence, groupInfo.groupId);
                                    }
                                    //A
                                    var plasmid = new plasmid();
                                    plasmid.name = plasmidName + "_IA";
                                    plasmid.sequence = step.map4_seq;
                                    plasmid.seq_length = step.map4_seq.Length;
                                    plasmid.people_id = userInfo.PersonId;
                                    db.plasmids.Add(plasmid);
                                    db.SaveChanges();
                                    nPlasmids += "-";
                                    nPlasmids += plasmid.id.ToString();
                                    if (plasmid.sequence != null)
                                    {
                                        //auto generate features
                                        var autoFeatures = new PlasmidFeature(plasmid.id, plasmid.sequence, groupInfo.groupId);
                                    }
                                }
                            }                            
                        }

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
                        expStep.des =lnbrConvert.ln2br(step.des);
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
                        if(step.ligation_method == "")
                        {
                            material.ligation_method = null;
                        }
                        else
                        {
                            material.ligation_method = step.ligation_method;
                        }
                        if (step.ligation_direction == "")
                        {
                            material.ligation_direction = null;
                        }
                        else
                        {
                            material.ligation_direction = step.ligation_direction;
                        }
                        //new plasmid id, sep by '-'
                        material.nplasmid_id = nPlasmids;
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

        //[Authorize]
        //[HttpGet]
        //public ActionResult DrawPlasmid()
        //{
        //    return View();
        //}
        [Authorize]
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            //id is the experiment id
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var exp = db.experiments.Find(id);
            if (exp == null)
            {
                return HttpNotFound();
            }
            return View(exp);
        }

        [Authorize, ActionName("Delete")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    var exp = db.experiments.Find(id);          
                    db.experiments.Remove(exp);
                    db.SaveChanges();
                    scope.Complete();
                    return RedirectToAction("Index", "Experiment");
                }
                catch (Exception)
                {
                    scope.Dispose();
                    return RedirectToAction("Index", "Experiment");
                }
            }              
        }

        [Authorize]
        [HttpGet]
        public ActionResult RetrievePlasmidMap(int? plasmid_id)
        {
            if (plasmid_id == null)
            {
                
                return Content(JsonConvert.SerializeObject(new { result = "Failed" }));
            }
            plasmid plasmid = db.plasmids.Find(plasmid_id);
            if (plasmid == null || plasmid.sequence == null)
            {
                return Content(JsonConvert.SerializeObject(new { result = "Failed" }));
            }
            //pass json
            //all the methylation
            var methylation = db.methylations.Where(m => m.plasmid_id == plasmid_id);
            var Methylation = methylation.OrderBy(n => n.name).Select(m => new { pId = m.plasmid_id, name = m.name, cut = m.cut, clockwise = m.clockwise, dam_cm = m.dam_complete, dam_ip = m.dam_impaired, dcm_cm = m.dcm_complete, dcm_ip = m.dcm_impaired });
            //ViewBag.Methylation = JsonConvert.SerializeObject(Methylation.ToList());
            //display all enzyme cuts of the current plasmid
            var plasmid_map = db.plasmid_map.OrderBy(s => s.start).Where(p => p.plasmid_id == plasmid_id);
            var Enzymes = plasmid_map.Where(f => f.feature_id == 4).OrderBy(n => n.common_feature != null ? n.common_feature.label : n.feature).OrderBy(s => s.cut).Select(f =>
                new {
                    pId = f.plasmid_id,
                    name = f.common_feature != null ? f.common_feature.label : f.feature,
                    start = f.start,
                    end = f.end,
                    cut = f.cut,
                    clockwise = f.clockwise == 1 ? true : false,
                    methylation = methylation.Where(m => m.name == (f.common_feature != null ? f.common_feature.label : f.feature) && m.cut == f.cut && m.clockwise == f.clockwise).Count() > 0 ? true : false
                });
            //ViewBag.Enzymes = JsonConvert.SerializeObject(Enzymes.ToList());
            //all other features
            //pass json
            var features = plasmid_map.OrderBy(s => s.start).Select(f => new { show_feature = f.show_feature, cut = f.cut, end = f.end, feature = f.common_feature != null ? f.common_feature.label : f.feature, type_id = f.feature_id, common_id = f.common_id, start = f.start, clockwise = f.clockwise == 1 ? true : false });
            //ViewBag.Features = JsonConvert.SerializeObject(features.ToList());
            //pass json for feature viewers
            var fvFeatures = plasmid_map.OrderBy(f => f.feature_id).OrderBy(s => s.start).Select(f => new { x = f.feature_id == 4 ? f.cut : f.start, y = f.feature_id == 4 ? f.cut : f.end, description = f.common_feature != null ? f.common_feature.label : f.feature, id = f.common_feature != null ? f.common_feature.label : f.feature, type_id = f.feature_id, color = "black" });
            //ViewBag.fvFeatures = JsonConvert.SerializeObject(fvFeatures.ToList());
            
            //get the enzyms activities
            var activity = db.activity_restriction.Select(a => new {
                name = a.restri_enzyme.name,
                company = a.company.shortName,
                companyFN = a.company.fullName,
                buffer = a.buffer.name,
                activity = a.activity,
                temprature = a.temprature
            });
            //ViewBag.activity = JsonConvert.SerializeObject(activity.ToList());

            //get the DNA ladders
            var ladder = db.ladders.Where(l => l.ladder_type == "DNA");
            //get json data
            var ladderId = ladder.Select(i => i.id);
            var ladderSize = db.ladder_size.Where(l => ladderId.Contains(l.ladder_id)).OrderBy(l => l.ladder_id).OrderBy(r => r.Rf).Select(l => new {
                id = l.ladder_id,
                size = l.size,
                mass = l.mass,
                Rf = l.Rf,
                name = l.ladder.name
            });
            //ViewBag.ladders= JsonConvert.SerializeObject(ladderSize.ToList());

            //get the previous saved bands
            var bands = db.fragments.Where(p => p.plasmid_id == (int)plasmid_id).Select(n => n.name);
            //ViewBag.SavedBands = JsonConvert.SerializeObject(bands.ToList());

            var data = new
            {
                result = "OK",
                plasmidId = plasmid_id,
                name = plasmid.name,
                Sequence = plasmid.sequence,
                seqCount = plasmid.seq_length,
                saveBands = JsonConvert.SerializeObject(bands.ToList()),
                ladders = JsonConvert.SerializeObject(ladderSize.ToList()),
                activity = JsonConvert.SerializeObject(activity.ToList()),
                fvFeatures = JsonConvert.SerializeObject(fvFeatures.ToList()),
                features = JsonConvert.SerializeObject(features.ToList()),
                enzymes = JsonConvert.SerializeObject(Enzymes.ToList()),
                methylation = JsonConvert.SerializeObject(Methylation.ToList())
            };
            return Content(
                JsonConvert.SerializeObject(data)            
            );
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