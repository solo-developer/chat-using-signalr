using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using System.Net;

namespace signalR.Controllers
{
    public class testController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult index(string user_name)
        {
            try
            {
                List<userModel> u = new List<userModel>();

                foreach (string u_name in globalVariables.user_name)
                {
                    if (u_name != user_name && u_name!=null)
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

        [HttpPost]
        public ActionResult send_files(Models.files model)
        {
            try
            {

            string file_path = "";
            var fileContent = model.ImageFile;
            if (fileContent != null && fileContent.ContentLength > 0)
            {
                string path = System.Web.HttpContext.Current.Server.MapPath("~/shared images/");
                //check for extensions
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".tiff", ".bmp" };
                var extension = Path.GetExtension(fileContent.FileName);
                if (allowedExtensions.Contains(extension))
                {
                        string return_file_path = Path.GetFileNameWithoutExtension(fileContent.FileName) + Guid.NewGuid() + extension;
                    string actual_path = path + return_file_path;
                        
                        //check if file is already present
                        if (!System.IO.File.Exists(fileContent.FileName))
                        {
                            fileContent.SaveAs(actual_path);
                        }
                    file_path = return_file_path;
                }
                    else
                    {
                        return Json(new { success = false, responseText = "Only image files are allowed" });
                    }
            }
            else
            {
                return Json(new { success = false, responseText = "Please upload an image to transfer." });
            }
            chatHub.send_files(model.message_from, model.message_to, file_path);
                var result = new { success = true, responseText = "File send successful", file_path = file_path };
                return Json(result);

            }
            catch (Exception)
            {
                return Json(new { success = false, responseText = "Failed to transfer file to" + model.message_to });
            }
        }
    }
}