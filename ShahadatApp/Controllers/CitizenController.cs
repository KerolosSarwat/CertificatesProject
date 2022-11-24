using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.IO;
using System.Web;
using System.Web.Mvc;
using ShahadatApp.DAL;
using ShahadatApp.Models;
using PagedList;
using System.Threading.Tasks;

namespace ShahadatApp.Controllers
{
    public class CitizenController : Controller
    {
        private ShahadatAppContext db = new ShahadatAppContext();

        // GET: Citizen
        public ActionResult Index(string searchString, int? page)
        {
            ViewBag.CurrentFilter = searchString;
            var citizen = db.Citizen.Include(c => c.Area);
            

            if(!String.IsNullOrEmpty(searchString))
                citizen = citizen.Where(c => c.Kawmy.Contains(searchString));
            citizen = citizen.OrderBy(c => c.MktbCode);

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(citizen.ToPagedList(pageNumber, pageSize));
            //return View(citizen.ToList());
        }

        // GET: Citizen/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Citizen citizen = db.Citizen.Find(id);
            if (citizen == null)
            {
                return HttpNotFound();
            }
            return View(citizen);
        }

        // GET: Citizen/Create
        public ActionResult Create()
        {
            ViewBag.MktbCode = new SelectList(db.Area, "MktbCode", "Name");
            return View();
        }

        // POST: Citizen/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Kawmy,FullName,Phone,WhatsAppUser,Milad,MktbCode,Mosalsal,IDImage,PersonalPhoto")] Citizen citizen)
        {
            if (ModelState.IsValid)
            {
                db.Citizen.Add(citizen);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MktbCode = new SelectList(db.Area, "MktbCode", "Name", citizen.MktbCode);
            return View(citizen);
        }

        // GET: Citizen/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Citizen citizen = db.Citizen.Find(id);
            if (citizen == null)
            {
                return HttpNotFound();
            }
            ViewBag.MktbCode = new SelectList(db.Area, "MktbCode", "Name", citizen.MktbCode);
            return View(citizen);
        }

        // POST: Citizen/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Kawmy,FullName,Phone,WhatsAppUser,Milad,MktbCode,Mosalsal,IDImage,PersonalPhoto")] Citizen citizen)
        {
            if (ModelState.IsValid)
            {
                db.Entry(citizen).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MktbCode = new SelectList(db.Area, "MktbCode", "Name", citizen.MktbCode);
            return View(citizen);
        }

        // GET: Citizen/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Citizen citizen = db.Citizen.Find(id);
            if (citizen == null)
            {
                return HttpNotFound();
            }
            return View(citizen);
        }

        // POST: Citizen/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Citizen citizen = db.Citizen.Find(id);
            var Talabat = db.Talab.Where(t => t.Kawmy == citizen.Kawmy);
            if(Talabat != null)
                foreach (Talab talab in Talabat)
                    db.Talab.Remove(talab);

            db.Citizen.Remove(citizen);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public ActionResult MorfkatNotSent(bool isCopy=false)
        {
            var talabat = db.Talab.Include(c => c.Citizen).Where(t => t.Citizen.MorfkatRecieved == true && t.Citizen.MorfkatSent == false && t.ServiceType.Contains("كارنيه"));

            List<Citizen> Citizens = new List<Citizen>();
            foreach (var t in talabat)
            {
                Citizens.Add(t.Citizen);
            }
            Citizens = Citizens.Distinct().ToList();

            if(isCopy)
            {
                string sourceFile = @"D:\morfat\";
                string destinationFile = @"F:\M\";
                foreach (var item in Citizens)
                {
                    try
                    {


                        System.IO.File.Copy(sourceFile + item.Kawmy + ".jpg", destinationFile + item.Kawmy + ".jpg", true);
                        System.IO.File.Copy(sourceFile + item.Kawmy + "_1.jpg", destinationFile + item.Kawmy + "_1.jpg", true);


                    }
                    catch (IOException iox)
                    {
                        //return Json(iox.Message, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json("تم النسخ بنجاح", JsonRequestBehavior.AllowGet);
            }
            //CopyMorfkat(Citizens.Select(c => c.Kawmy).ToList());

            return View(Citizens.ToList());
        }
        public ActionResult CopyMorfkat(List<string> Kawmys)
        {
            string sourceFile = @"D:\morfat\";
            string destinationFile = @"F:\M\";
            try
            {
                foreach (var item in Kawmys)
                {

                    System.IO.File.Copy(sourceFile + item + ".jpg", destinationFile + item + ".jpg", true);
                    System.IO.File.Copy(sourceFile + item + "_1.jpg", destinationFile + item + "_1.jpg", true);

                }
            }
            catch (IOException)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

    }
}
