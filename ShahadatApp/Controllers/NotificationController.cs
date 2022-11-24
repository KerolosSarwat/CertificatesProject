
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShahadatApp.Models;
using ShahadatApp.DAL;
using ShahadatApp.Hubs;

namespace ShahadatApp.Controllers
{
    public class NotificationController : Controller
    {
        // GET: Notification
        private ShahadatAppContext db = new ShahadatAppContext();
        public ActionResult Index(int? id)
        {
            if (id != null)
            {
                bool done = DeleteNotification(id);
                if (done)
                {
                    MyHub.Send($"<a href='/someurl'>تم إرسال إشعار</a>");

                    return Json(new { success = true, message = $"تم إرسال إشعار" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false, message = $"Task #{id} failed" }, JsonRequestBehavior.AllowGet);
            }
            return View();
        }
        public JsonResult GetNotification()
        {
            return Json(NotificaionService.GetNotification(), JsonRequestBehavior.AllowGet);

        }


        public bool DeleteNotification(int? id)
        {
            try
            {
                Notification Notification = db.Notification.Find(id);
                db.Notification.Remove(Notification);
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            
        }
    }
}