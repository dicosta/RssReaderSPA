using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RssReader.Web.Controllers
{
    public class PartialsController : Controller
    {
        public ActionResult News()
        {
            return View();
        }

        public ActionResult Suscriptions()
        {
            return View();
        }
    }
}
