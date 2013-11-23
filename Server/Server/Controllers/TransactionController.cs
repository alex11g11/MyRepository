using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Web;
using System.Web.Mvc;
using Server.Models;
using Server.Others;

namespace Server.Controllers
{
    public class TransactionController : Controller
    {
        //
        // GET: /Transaction/

        public ActionResult Index()
        {
            return View();
        }

    
    }
}
