using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace signalR.Controllers
{
    public class homeController : Controller
    {
        // GET: home
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult index(string user_name )
        {
            try
            {
                List<userModel> u = new List<userModel>();

                foreach (string u_name in globalVariables.user_name)
                {
                    if (u_name != user_name)
                    {
                        u.Add(new userModel
                        {
                            user_name = u_name
                        });
                    }
                }
                return Json(u);
            }
            catch (Exception ex)
            {

                return Json(ex.Message);
            }
        }
    }
}