﻿using ecloning.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ecloning.Controllers
{
    public class BioToolsController : RootController
    {
        private ecloningEntities db = new ecloningEntities();
        // GET: BioTools
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Alignment()
        {
            //get the plasmid seq
            var plasmids = db.plasmids.Where(s => s.sequence != null).Select(p => new { name = p.name, id = p.id, seq = p.sequence, height=1, reference = false });
            ViewBag.Plasmids = JsonConvert.SerializeObject(plasmids.ToList());
            return View();
        }
    }
}