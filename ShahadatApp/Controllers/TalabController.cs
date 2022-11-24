using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ShahadatApp.DAL;
using ShahadatApp.Models;
using PagedList;
using WhatsAppApi;
using ShahadatApp.Hubs;
using System.Web.Script.Serialization;
namespace ShahadatApp.Controllers
{
    public class TalabController : Controller
    {
        private ShahadatAppContext db = new ShahadatAppContext();
        private ShahadatAppContext _db = new ShahadatAppContext();

        //List<string> servicePostion = new List<string>()
        //    {
        //        "", "إستخراج كاشير", "تسجيل بيانات", "مراجعة بيانات", "طباعة الخدمة", "إستلام مندوب", "الخدمة جاهزة للأستلام", "تم تسليم الخدمة"
        //    };
        List<string> servicePostion = new List<string>()
            {
                "",  "طباعة الخدمة","الخدمة جاهزة للأستلام", "تم تسليم الخدمة"
            };
        List<string> Intervals = new List<string>() { "", "اقل من ساعة", "من ساعة إلى 3", "من 3 إلى 6 ساعات", "من 6 إلى 12 ساعة", "من 12 إلى 24 ساعة" };
        List<string> TalabStatus = new List<string>() { "تم ابلاغه بالدفع", "تم ابلاغه فورى", "تم الدفع", "تفاوض","الغاء الطلب" };
        List<string> CertificateType = new List<string>() { "", "مزدوج الجنسية" };


        // GET: Talab
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, string selectOption, int? page, DateTime? fromDate , DateTime? toDate,string talabStatus)
        {

            return HttpNotFound();
            
            ViewBag.CurrentSort = sortOrder;
            ViewBag.CurrentFilter = searchString;
            ViewBag.FromDate = DateTime.Now;
            ViewBag.ToDate = DateTime.Now;
            var OptionList = new List<string>() { "", "طباعة الخدمة", "الخدمة جاهزة للأستلام" };

            //var TalabStatus = new List<string>() { "", "تم ابلاغه بالدفع", "تم ابلاغه بالدفع 2", "تم الدفع" };
            ViewBag.TalabStatus = new SelectList(TalabStatus);
            ViewBag.SelectOption = new SelectList(OptionList);

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            var talab = db.Talab.Include(t => t.Area).Include(t => t.Citizen).Include(t => t.PrintArea);


            if (!String.IsNullOrEmpty(selectOption))
            {
                talab = talab.Where(t => t.ServicePostion.Contains(selectOption));
            }
            if (!String.IsNullOrEmpty(searchString))
            {
                talab = talab.Where(t => t.TalabNum.Contains(searchString) || t.Kawmy.Contains(searchString) || t.Citizen.FullName.Contains(searchString));
            }
            if(fromDate != null && toDate != null)
            {
                talab = talab.Where(t => t.CreateDate >= fromDate && t.CreateDate <= toDate);
            }
            if (!String.IsNullOrEmpty(talabStatus))
            {
                talab = talab.Where(t => t.TalabStatus.Contains(talabStatus));
            }

            talab = talab.OrderByDescending(t => t.CreateDate);
            //int pageSize = 10;
            //int pageNumber = (page ?? 1);

            return View(talab.ToList());
        }

        // GET: Talab/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Talab talab = db.Talab.Find(id);
            if (talab == null)
            {
                return HttpNotFound();
            }
            return View(talab);
        }

        List<Area> SortPrintArea(List<Area> listOfPrintArea)
        {
            List<Area> SortedArea = new List<Area>();
            SortedArea.Add(listOfPrintArea.Where(a => a.Name.Contains("إدارة")).SingleOrDefault());
            SortedArea.Add(listOfPrintArea.Where(a => a.Name.Contains("منطقة تجنيد وتعبئة القاهرة")).SingleOrDefault());
            SortedArea.Add(listOfPrintArea.Where(a => a.Name.Contains("منطقة تجنيد وتعبئة الإسكندرية")).SingleOrDefault());
            SortedArea.Add(listOfPrintArea.Where(a => a.Name.Contains("منطقة تجنيد وتعبئة المنصورة")).SingleOrDefault());
            SortedArea.Add(listOfPrintArea.Where(a => a.Name.Contains("منطقة تجنيد وتعبئة أسيوط")).SingleOrDefault());
            SortedArea.Add(listOfPrintArea.Where(a => a.Name.Contains("منطقة تجنيد وتعبئة الجيزة")).SingleOrDefault());
            SortedArea.Add(listOfPrintArea.Where(a => a.Name.Contains("منطقة تجنيد وتعبئة طنطا")).SingleOrDefault());
            SortedArea.Add(listOfPrintArea.Where(a => a.Name.Contains("منطقة تجنيد وتعبئة المنيا")).SingleOrDefault());
            SortedArea.Add(listOfPrintArea.Where(a => a.Name.Contains("منطقة تجنيد وتعبئة الزقازيق")).SingleOrDefault());
            SortedArea.Add(listOfPrintArea.Where(a => a.Name.Contains("منطقة تجنيد وتعبئة قنا")).SingleOrDefault());

            return SortedArea;
        }

        List<Area> SortRecievedArea(List<Area> listOfRecievedArea)
        {
            List<Area> SortedArea = new List<Area>();
            SortedArea.Add(listOfRecievedArea.Where(a => a.Name.Contains("إدارة")).SingleOrDefault());
            SortedArea.AddRange(listOfRecievedArea.Where(a => a.Qesm.Contains("القاهرة") && !a.Name.Contains("إدارة")));
            SortedArea.AddRange(listOfRecievedArea.Where(a => a.Qesm.Contains("القليوبية")));
            SortedArea.AddRange(listOfRecievedArea.Where(a => a.Qesm.Contains("حلوان")));
            SortedArea.AddRange(listOfRecievedArea.Where(a => a.Qesm.Contains("الاسكندرية")));
            SortedArea.AddRange(listOfRecievedArea.Where(a => a.Qesm.Contains("المنصورة")));
            SortedArea.AddRange(listOfRecievedArea.Where(a => a.Qesm.Contains("أسيوط")));
            SortedArea.AddRange(listOfRecievedArea.Where(a => a.Qesm.Contains("الجيزة")));
            SortedArea.AddRange(listOfRecievedArea.Where(a => a.Qesm.Contains("طنطا")));
            SortedArea.AddRange(listOfRecievedArea.Where(a => a.Qesm.Contains("المنيا")));
            SortedArea.AddRange(listOfRecievedArea.Where(a => a.Qesm.Contains("الزقازيق")));
            SortedArea.AddRange(listOfRecievedArea.Where(a => a.Qesm.Contains("قنا")));
            return SortedArea;
        }

        // GET: Talab/Create
        public ActionResult Create()
        {
            List<Area> PrintArea = db.Area.Where(area => area.Name.Contains("منطقة") || area.Name.Contains("إدارة")).ToList();


            List<Area> SortedArea = SortPrintArea(PrintArea);

            var ReceivedArea = (from area in db.Area
                               where area.Name.Contains("إدارة") || area.Name.Contains("منطقة") || area.Name.Contains("قسم تجنيد")
                               || area.MktbCode == "237" || area.MktbCode == "152" || area.MktbCode == "78" || area.MktbCode == "236" || area.MktbCode == "251" || area.MktbCode == "262" || area.MktbCode == "221" || area.MktbCode == "163" || area.MktbCode == "44" || area.MktbCode == "401"
                               select area).ToList();
            //var ReceivedArea = (from area in db.Area
            //                   select area).ToList();

            List<Area> NewReceivedArea = SortRecievedArea(ReceivedArea);
            ViewBag.TalabStatus = new SelectList(TalabStatus);
            ViewBag.CertificateType = new SelectList(CertificateType);
            ViewBag.ServicePostion = new SelectList(servicePostion,"", new List<string> { "تم تسليم الخدمة", "الخدمة جاهزة للأستلام" });
            ViewBag.CardServicePostion = new SelectList(servicePostion,"", new List<string> { "تم تسليم الخدمة", "الخدمة جاهزة للأستلام" });
            ViewBag.ServiceType = new SelectList(new List<string>() { "","شهادة", "كارنيه", "شهادة و كارنيه" });
            ViewBag.RecievedAreaID = new SelectList(NewReceivedArea, "MktbCode", "Name");
            ViewBag.Kawmy = new SelectList(db.Citizen, "Kawmy", "FullName");
            ViewBag.PrintAreaID = new SelectList(SortedArea, "MktbCode", "Name");
            return View();
        }

        // POST: Talab/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TalabNum,Kawmy,TalabStatus,PrintAreaID,RecievedAreaID,ServicePostion,CardServicePostion,ServiceType,PrintDate,CardPrintDate,Notes,CertificateType,Citizen")] Talab talab)
        {
           
            talab.CreateDate = DateTime.Now;
            talab.Kawmy = talab.Citizen.Kawmy;

            if (ModelState.IsValid)
            {
                talab.Citizen.Phone = talab.Citizen.Phone.StartsWith("01") ? "+2" + talab.Citizen.Phone : talab.Citizen.Phone;
                var citizen = db.Citizen.Find(talab.Citizen.Kawmy);

                if (citizen != null)
                    talab.Citizen = null;

                db.Talab.Add(talab);
                db.SaveChanges();
                return RedirectToAction("TalabatByArea");
            }
            ViewBag.TalabStatus = new SelectList(new List<string>() { "تم ابلاغه بالدفع", "تم الدفع" });
            ViewBag.ServicePostion = new SelectList(servicePostion,"", new List<string> { "تم تسليم الخدمة", "الخدمة جاهزة للأستلام" });
            
            ViewBag.ServiceType = new SelectList(new List<string>() { "شهادة", "كارنيه", "شهادة و كارنيه" });
            ViewBag.CardServicePostion = new SelectList(servicePostion, "", new List<string> { "تم تسليم الخدمة", "الخدمة جاهزة للأستلام" });
            
            ViewBag.Kawmy = new SelectList(db.Citizen, "Kawmy", "FullName", talab.Kawmy);
            
            return Redirect("Create");
            
        }

        // GET: Talab/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Talab talab = db.Talab.Include(t => t.Area).Include(t => t.Citizen).Include(t => t.PrintArea).Where(t=> t.TalabNum == id).FirstOrDefault();
            if (talab == null)
            {
                return HttpNotFound();
            }
            var PrintArea = (from area in db.Area
                              where area.Name.Contains("منطقة") || area.Name.Contains("إدارة")
                            select area).ToList();

            PrintArea = SortPrintArea(PrintArea);

            var ReceivedArea = (from area in db.Area
                               where area.Name.Contains("إدارة") || area.Name.Contains("منطقة") || area.Name.Contains("قسم تجنيد") || area.MktbCode == "237" || area.MktbCode == "152" || area.MktbCode == "78" || area.MktbCode == "236" || area.MktbCode == "251" || area.MktbCode == "262" || area.MktbCode == "221" || area.MktbCode == "163" || area.MktbCode == "44" || area.MktbCode == "401"
                               select area).ToList();

            ReceivedArea = SortRecievedArea(ReceivedArea);

            ViewBag.TalabStatus = new SelectList(TalabStatus, talab.TalabStatus);
            ViewBag.CertificateType = new SelectList(CertificateType, talab.CertificateType);
            ViewBag.ServicePostion = new SelectList(servicePostion, talab.ServicePostion,new List<string> { "تم تسليم الخدمة", "الخدمة جاهزة للأستلام" });
            ViewBag.CardServicePostion = new SelectList(servicePostion, talab.CardServicePostion, new List<string> { "تم تسليم الخدمة", "الخدمة جاهزة للأستلام" });
            ViewBag.ServiceType = new SelectList(new List<string>() { "شهادة", "كارنيه", "شهادة و كارنيه" }, talab.ServiceType);
            ViewBag.RecievedAreaID = new SelectList(ReceivedArea, "MktbCode", "Name", talab.RecievedAreaID);
            ViewBag.Kawmy = new SelectList(db.Citizen, "Kawmy", "FullName", talab.Kawmy);
            ViewBag.PrintAreaID = new SelectList(PrintArea, "MktbCode", "Name", talab.PrintAreaID);
            talab.Citizen.Phone = talab.Citizen.Phone.Remove(0, 2);
            return View(talab);
        }

        public void AutoRecievedAreas(string PrintAreaID)
        {
            var Areas = from area in db.Area
                        select area;

            var PrintArea = from area in Areas
                            where area.Name.Contains("منطقة") || area.Name.Contains("إدارة")
                            select area;

            var recievedAreas = from a in Areas
                                where a.Mntqa == (from area in Areas
                                                  where area.MktbCode == PrintAreaID
                                                  select area.Mntqa).FirstOrDefault() && (a.Name.Contains("قسم") || a.Name.Contains("منطقة"))
                                select a;

            if (PrintAreaID == "477")//منطقة القاهرة
                recievedAreas = from a in Areas
                                where a.Mntqa == (from area in Areas
                                                  where area.MktbCode == PrintAreaID
                                                  select area.Mntqa).FirstOrDefault() && (a.Name.Contains("إدارة") || a.Name.Contains("منطقة") || a.Name.Contains("قسم تجنيد")
                               || a.MktbCode == "237" || a.MktbCode == "152" || a.MktbCode == "78" || a.MktbCode == "236" || a.MktbCode == "251" || a.MktbCode == "262" || a.MktbCode == "221" || a.MktbCode == "163" || a.MktbCode == "44" || a.MktbCode == "401")
                                select a;
            if (PrintAreaID == "472")
                recievedAreas = from area in Areas
                                where area.Name.Contains("إدارة") || area.Name.Contains("منطقة") || area.Name.Contains("قسم تجنيد")
                           || area.MktbCode == "237" || area.MktbCode == "152" || area.MktbCode == "78" || area.MktbCode == "236" || area.MktbCode == "251" || area.MktbCode == "262" || area.MktbCode == "221" || area.MktbCode == "163" || area.MktbCode == "44" || area.MktbCode == "401"
                                select area;
            ViewBag.RecievedAreaID = new SelectList(recievedAreas, "MktbCode", "Name");
        }
        // POST: Talab/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TalabNum,Kawmy,TalabStatus,PrintAreaID,RecievedAreaID,ServicePostion,CardServicePostion,ServiceType,PrintDate,CardPrintDate,Notes,CreateDate,TmEbla8Msg,TmDaf3Msg,isNegotiated,CertificateType,Citizen")] Talab talab, string previousUrl)
        {
            if (ModelState.IsValid)
            {
                //talab.Citizen = citizen;
                Talab talaB = db.Talab.Find(talab.TalabNum);
                if (talaB == null)
                {
                    //Update Talab
                    Talab oldTalab = db.Talab.Where(t => t.Kawmy == talab.Kawmy).First();
                    DeleteConfirmed(oldTalab.TalabNum);
                    talab.CreateDate = DateTime.Today;
                    talab.TmEbla8Msg = false; talab.TmDaf3Msg = false; talab.isCanceled = false; talab.isNegotiated = false;
                    Create(talab);
                    return Redirect(previousUrl);
                }
                else
                {
                    talab.Citizen.Phone = talab.Citizen.Phone.StartsWith("01") ? "+2" + talab.Citizen.Phone : talab.Citizen.Phone;
                    
                    db.Entry(talab.Citizen).State = EntityState.Modified;
                    db.Entry(talaB).CurrentValues.SetValues(talab);
                    
                    db.SaveChanges();
                    return Redirect(previousUrl);
                }
                
            }

            ViewBag.TalabStatus = new SelectList(new List<string>() { "تم ابلاغه بالدفع", "تم ابلاغه بالدفع 2", "تم الدفع" }, talab.TalabStatus);
            ViewBag.RecievedAreaID = new SelectList(db.Area, "MktbCode", "Name", talab.RecievedAreaID);
            ViewBag.Kawmy = new SelectList(db.Citizen, "Kawmy", "FullName", talab.Kawmy);
            ViewBag.PrintAreaID = new SelectList(db.Area, "MktbCode", "Name", talab.PrintAreaID);
            ViewBag.ServicePostion = new SelectList(servicePostion, talab.ServicePostion, new List<string> { "تم تسليم الخدمة", "الخدمة جاهزة للأستلام" });
            ViewBag.ServiceType = new SelectList(new List<string>() { "شهادة", "كارنيه", "شهادة و كارنيه" }, talab.ServiceType);
            return View();
        }

        // GET: Talab/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Talab talab = db.Talab.Find(id);
            if (talab == null)
            {
                return HttpNotFound();
            }
            return View(talab);
        }

        // POST: Talab/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Talab talab = db.Talab.Find(id);
            db.Talab.Remove(talab);
            db.SaveChanges();
            return View();
            //return RedirectToAction("TalabatByArea");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        
        public ActionResult TalabatByArea(string id,string talabStatus, string searchString, string printArea, DateTime? fromDate, DateTime? toDate, string selectOption, int? page)
        {
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            ViewBag.TalabStatus = new SelectList(TalabStatus);

            UpdateCanceledTalabat();

            var OptionList = new List<string>() { "", "طباعة الخدمة", "الخدمة جاهزة للأستلام" };
            ViewBag.SelectOption = new SelectList(OptionList);

            var PrintArea = from area in db.Area
                            where area.Name.Contains("منطقة")
                            select area;
            
            ViewBag.PrintArea = new SelectList(PrintArea, "MktbCode", "Name");
            if (searchString != null)
                page = 1;

            ViewBag.SelectedArea = printArea;
            ViewBag.CurrentFilter = searchString;

            var talab = db.Talab.Include(t => t.Area).Include(t => t.Citizen).Include(c => c.PrintArea);

            if (!String.IsNullOrEmpty(id))
            {
                bool done = SendNotification(id);
                if (done)
                {
                    MyHub.Send($"<a href='/someurl'>تم إرسال إشعار</a>");

                    return Json(new { success = true, message = $"تم إرسال إشعار" }, JsonRequestBehavior.AllowGet);
                } 
                return Json(new { success = false, message = $"Task #{id} failed" }, JsonRequestBehavior.AllowGet);
            }
            if (!String.IsNullOrEmpty(printArea) && !printArea.Contains("الكل"))
            {
                var SelectedArea = (from area in db.Area
                                    where area.MktbCode.Contains(printArea)
                                    select area.Mntqa).First();

                talab = from area in db.Area
                            join citizen in db.Citizen on area.MktbCode equals citizen.MktbCode
                            join tlb in db.Talab on citizen.Kawmy equals tlb.Kawmy
                            where area.Mntqa.Contains(SelectedArea)
                            orderby tlb.CreateDate descending
                            select tlb;

                talab = talab.Include(t => t.Area).Include(t => t.Citizen).Include(c => c.PrintArea);
            }
            if (!String.IsNullOrEmpty(searchString))
            {
                if (searchString.StartsWith("+2"))
                {
                    searchString = searchString.Replace(" ",String.Empty);
                    talab = talab.Where(t => t.Citizen.Phone.Contains(searchString));
                }
                else
                    talab = talab.Where(t => t.TalabNum.Contains(searchString) || t.Kawmy.Contains(searchString) || t.Citizen.FullName.Contains(searchString) || t.Citizen.Phone.Contains(searchString) || (t.Citizen.Milad + " " + t.Citizen.MktbCode + " " + t.Citizen.Mosalsal).Contains(searchString));
            }

            if (!String.IsNullOrEmpty(selectOption))
                talab = talab.Where(t => t.ServicePostion.Contains(selectOption));

            if (fromDate != null && toDate != null)
            {
                ViewBag.FromDate = Convert.ToDateTime(fromDate).ToString("yyyy-MM-dd");
                ViewBag.ToDate = Convert.ToDateTime(toDate).ToString("yyyy-MM-dd");
                talab = talab.Where(t => t.CreateDate >= fromDate && t.CreateDate <= toDate);
            }
            if (!String.IsNullOrEmpty(talabStatus))
            {
                ViewBag.TalabStatus = new SelectList(TalabStatus, talabStatus);
                talab = talab.Where(t => t.TalabStatus.Contains(talabStatus));
            }

            talab = talab.OrderByDescending(t => t.CreateDate);
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            ViewBag.GreenCount = talab.Where(item => !item.TalabStatus.Contains("تم الدفع")).ToList()
                .Where(item => DateTime.Now.AddHours(-1) <= item.CreateDate).Count();

            ViewBag.BlueCount = talab.Where(item => !item.TalabStatus.Contains("تم الدفع")).ToList()
                .Where(item => DateTime.Now.AddHours(-1) > item.CreateDate && DateTime.Now.AddHours(-3) <= item.CreateDate).Count();

            ViewBag.LightCount = talab.Where(item => !item.TalabStatus.Contains("تم الدفع")).ToList()
                .Where(item => DateTime.Now.AddHours(-3) > item.CreateDate && DateTime.Now.AddHours(-6) <= item.CreateDate).Count();

            ViewBag.YellowCount = talab.Where(item => !item.TalabStatus.Contains("تم الدفع")).ToList()
                .Where(item => DateTime.Now.AddHours(-6) > item.CreateDate && DateTime.Now.AddHours(-12) <= item.CreateDate).Count();

            ViewBag.RedCount = talab.Where(item => !item.TalabStatus.Contains("تم الدفع")).ToList()
                .Where(item => DateTime.Now.AddHours(-12) > item.CreateDate && DateTime.Now.AddHours(-24) <= item.CreateDate).Count();

            ViewBag.AllCount = talab.Where(item => !item.TalabStatus.Contains("تم الدفع")).ToList().Count();
            
            return View(talab.ToPagedList(pageNumber, pageSize));
            //return View(talab.ToList());
        }

        public ActionResult MarkMorfkat(string kawmy, string url)
        {
            if (kawmy == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var talab = db.Talab.Include(c => c.Citizen).Where(t => t.Citizen.Kawmy == kawmy).FirstOrDefault();
            if (talab == null)
            {
                return HttpNotFound();
            }
            talab.Citizen.MorfkatRecieved = true;
            db.Entry(talab).State = EntityState.Modified;
            db.SaveChanges();
            
            return Redirect(url);
        }

        public JsonResult AutoSelectRecievedArea(string printArea)
        {

            var RecievedArea = from recieved in db.Area
                               where recieved.Mntqa == printArea
                               select recieved.Name;
            ViewBag.RecievedAreaID = new SelectList(RecievedArea);
            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(RecievedArea);
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        
        public ActionResult TmEbla8()
        {
            
            ViewBag.Intervals = new SelectList(Intervals);
            var PrintArea = from area in db.Area
                            where area.Name.Contains("منطقة")
                            select area;

            ViewBag.SelectedArea = new SelectList(PrintArea, "MktbCode", "Name");

            var talab = db.Talab.Include(t => t.Area).Include(t => t.Citizen).Include(c => c.PrintArea).Where(t => t.TalabStatus.Contains("تم ابلاغه بالدفع")&& t.TmEbla8Msg == false);
            talab = talab.Where(t => t.CreateDate >= DateTime.Today && t.CreateDate <= DateTime.Today);
            talab = talab.OrderByDescending(t => t.CreateDate);
            return View(talab.ToList());
        }
        [HttpPost]
        public ActionResult TmEbla8(DateTime? fromDate, DateTime? toDate, string interval, string selectedArea)
        {
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            ViewBag.Intervals = new SelectList(Intervals, interval);

            var PrintArea = from area in db.Area
                            where area.Name.Contains("منطقة")
                            select area;
            
            ViewBag.SelectedArea = new SelectList(PrintArea, "MktbCode", "Name", selectedArea);

            var talabat = db.Talab.Include(t => t.Area).Include(t => t.Citizen).Include(c => c.PrintArea).Where(t => t.TalabStatus.Contains("تم ابلاغه بالدفع") && t.TmEbla8Msg == false);

            if (fromDate != null && toDate != null)
                talabat = talabat.Where(t => t.CreateDate >= fromDate && t.CreateDate <= toDate);

            if (!String.IsNullOrEmpty(selectedArea) && !selectedArea.Contains("الكل"))
            {
                var SelectedArea = (from area in db.Area
                                    where area.MktbCode.Contains(selectedArea)
                                    select area.Mntqa).First();

                talabat = from area in db.Area
                        join citizen in db.Citizen on area.MktbCode equals citizen.MktbCode
                        join tlb in talabat on citizen.Kawmy equals tlb.Kawmy
                        where area.Mntqa.Contains(SelectedArea)
                        orderby tlb.CreateDate descending
                        select tlb;

                talabat = talabat.Include(t => t.Area).Include(t => t.Citizen).Include(c => c.PrintArea);
            }
            if (!String.IsNullOrEmpty(interval))
            {
                DateTime FromDate,ToDate;
                if (ViewBag.Intervals.SelectedValue == Intervals[1])
                {
                    FromDate = DateTime.Now.AddHours(-1);
                    talabat = talabat.Where(t => FromDate <= t.CreateDate);
                }

                else if (ViewBag.Intervals.SelectedValue == Intervals[2])
                {
                    FromDate = DateTime.Now.AddHours(-3);
                    ToDate = DateTime.Now.AddHours(-1);
                    talabat = talabat.Where(t => ToDate > t.CreateDate && FromDate <= t.CreateDate);
                }
                else if (ViewBag.Intervals.SelectedValue == Intervals[3])
                {
                    FromDate= DateTime.Now.AddHours(-6);
                    ToDate = DateTime.Now.AddHours(-3);
                    talabat = talabat.Where(t => ToDate > t.CreateDate && FromDate <= t.CreateDate);
                }

                else if (ViewBag.Intervals.SelectedValue == Intervals[4])
                {
                    FromDate = DateTime.Now.AddHours(-12);
                    ToDate = DateTime.Now.AddHours(-6);
                    talabat = talabat.Where(t => ToDate > t.CreateDate && FromDate <= t.CreateDate);
                }
                else if (ViewBag.Intervals.SelectedValue == Intervals[5])
                {
                    FromDate = DateTime.Now.AddHours(-24);
                    ToDate = DateTime.Now.AddHours(-12);
                    talabat = talabat.Where(t => ToDate > t.CreateDate && FromDate <= t.CreateDate);
                }
            }
            

            talabat = talabat.OrderByDescending(t => t.CreateDate);
            return View(talabat.ToList());
        }

        public ActionResult UpdateTmEbla8(DateTime? fromDate, DateTime? toDate)
        {
            if (fromDate != null && toDate != null)
            {
                var Talabat = db.Talab.Where(t => t.CreateDate >= fromDate && t.CreateDate <= toDate && t.TalabStatus.Contains("تم ابلاغه بالدفع") && t.TmEbla8Msg == false);

                foreach (Talab talab in Talabat)
                {
                    talab.TmEbla8Msg = true;
                    db.Entry(talab).State = EntityState.Modified;
                }
                db.SaveChanges();
            }

            return Redirect("TmEbla8");
        }

        public ActionResult AreaReports(DateTime? fromDate, DateTime? toDate, string interval, string SelectedArea)
        {
            var PrintArea = from area in db.Area
                            where area.Name.Contains("منطقة")
                            select area;

            ViewBag.SelectedArea = new SelectList(PrintArea, "Mntqa", "Name");

            var talab = db.Talab.Include(t => t.Area).Include(t => t.Citizen).Where(t => t.ServicePostion == "طباعة الخدمة" && t.ServiceType.Contains("كارنيه"));

            if (fromDate != null && toDate != null)
                talab = talab.Where(t => t.CreateDate >= fromDate && t.CreateDate <= toDate);

            if (!String.IsNullOrEmpty(SelectedArea) && !SelectedArea.Contains("الكل"))
                talab = talab.Where(t => t.Area.Mntqa == SelectedArea);

            talab = talab.OrderByDescending(t => t.Area.Name);
            return View(talab.ToList());
        }


        public ActionResult Morfkat(DateTime? fromDate, DateTime? toDate, string talabStatus)
        {
            var talab = db.Talab.Include(c => c.Citizen).Where(t => t.Citizen.MorfkatRecieved == false && t.ServiceType.Contains("كارنيه"));
            ViewBag.TalabStatus = new SelectList(TalabStatus);
            if (!String.IsNullOrEmpty(talabStatus))
            {
                ViewBag.TalabStatus = new SelectList(TalabStatus, talabStatus);
                talab = talab.Where(t => t.TalabStatus == talabStatus);
            }
            if (fromDate != null && toDate != null)
                talab = talab.Where(t => t.CreateDate >= fromDate && t.CreateDate <= toDate);
            return View(talab.ToList());
        }
        [HttpPost]
        public ActionResult TmDaf3(DateTime? fromDate, DateTime? toDate)
        {
            //Clipboard.SetText();
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            var talab = db.Talab.Include(t => t.Area).Include(t => t.Citizen).Include(c => c.PrintArea).Where(t => t.TalabStatus.Contains("تم الدفع"));

            if (fromDate != null && toDate != null)
                talab = talab.Where(t => t.CreateDate >= fromDate && t.CreateDate <= toDate && t.TmDaf3Msg == false);

            return View(talab.ToList());
        }

        public ActionResult UpdateTmDaf3(DateTime? fromDate, DateTime? toDate)
        {
            if(fromDate != null && toDate != null)
            {
                var Talabat = db.Talab.Where(t => t.CreateDate >= fromDate && t.CreateDate <= toDate && t.TalabStatus.Contains("تم الدفع") && t.TmDaf3Msg == false);

                foreach (Talab talab in Talabat)
                {
                    talab.TmDaf3Msg = true;
                    db.Entry(talab).State = EntityState.Modified;
                }
                db.SaveChanges();
            }

            return Redirect("TmDaf3");
        }

        public ActionResult Negotiation(DateTime? fromDate, DateTime? toDate)
        {
            //Clipboard.SetText();
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            var talab = db.Talab.Include(t => t.Area).Include(t => t.Citizen).Include(c => c.PrintArea).Where(t => t.TalabStatus.Contains("تفاوض") && t.isNegotiated == false);

            if (fromDate != null && toDate != null)
                talab = talab.Where(t => t.CreateDate >= fromDate && t.CreateDate <= toDate && t.TmDaf3Msg == false);

            return View(talab.ToList());
        }
        public ActionResult UpdateNegotiation(DateTime? fromDate, DateTime? toDate)
        {
            //Clipboard.SetText();
            if (fromDate != null && toDate != null)
            {
                var Talabat = db.Talab.Include(t => t.Area).Include(t => t.Citizen).Include(c => c.PrintArea).Where(t => t.TalabStatus.Contains("تفاوض") && t.isNegotiated == false);
                foreach (Talab talab in Talabat)
                {
                    talab.isNegotiated = true;
                    db.Entry(talab).State = EntityState.Modified;
                }
                db.SaveChanges();
            }


            return Redirect("Negotiation");
        }

        public ActionResult ServiceReady(DateTime? fromDate, DateTime? toDate)
        {
            var res = new List<Talab>();

            if (fromDate != null && toDate != null)
            {

                ViewBag.FromDate = fromDate;
                ViewBag.ToDate = toDate;

                res.AddRange(getShahadaList(fromDate, toDate));
                res.AddRange(getCardList(fromDate, toDate));
            }
            else
            {
                UpdateServicePostion(DateTime.Today, DateTime.Today);

                res.AddRange(getShahadaList(DateTime.Today, DateTime.Today));
                res.AddRange(getCardList(DateTime.Today, DateTime.Today));
            }
            return View(res);
        }

        //[HttpPost]
        public ActionResult UpdateServicePostion(DateTime fromDate, DateTime toDate)
        {
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;

            if (fromDate != null && toDate != null)
            {

                // in case of shahada bs.. 
                var talabat = db.Talab.Include(t => t.Area).Include(t => t.Citizen).Include(c => c.PrintArea).Where(t => t.PrintDate >= fromDate && t.PrintDate <= toDate && t.ServicePostion == "طباعة الخدمة");


                if (talabat != null)
                {
                    foreach (var talab in talabat)
                    {
                        if (talab.Area.Name.Contains("منطقة") || talab.Area.Name.Contains("إدارة"))
                        {
                            talab.ServicePostion = "الخدمة جاهزة للأستلام";
                        }

                        else if (talab.Area.Name.Contains("مكتب") || talab.Area.Name.Contains("قسم"))
                        {
                            talab.ServicePostion = "الخدمة جاهزة للأستلام";
                          //  talab.PrintDate = talab.PrintDate.AddDays(1);
                        }

                        db.Entry(talab).State = EntityState.Modified;


                    }
                    db.SaveChanges();
                }

                // in case of Karneeh bs.. 
                talabat = db.Talab.Include(t => t.Area).Include(t => t.Citizen).Include(c => c.PrintArea).Where(t => t.CardPrintDate >= fromDate && t.CardPrintDate <= toDate && t.CardServicePostion == "طباعة الخدمة");
                if (talabat != null)
                {
                    foreach (var talab in talabat)
                    {
                        if (talab.Area.Name.Contains("إدارة"))
                        {
                            talab.CardServicePostion = "الخدمة جاهزة للأستلام";
                        }

                        else if (talab.Area.Name.Contains("مكتب") || talab.Area.Name.Contains("قسم") || talab.Area.Name.Contains("منطقة"))
                        {
                            talab.CardServicePostion = "الخدمة جاهزة للأستلام";
                          //  talab.CardPrintDate = talab.CardPrintDate.Value.AddDays(5);
                        }

                        db.Entry(talab).State = EntityState.Modified;


                    }
                    db.SaveChanges();
                }

            }
            return Redirect("ServiceReady");
        }

        public ActionResult ServiceDone(DateTime? fromDate, DateTime? toDate)
        {
            if (fromDate != null && toDate != null)
            {
                /*  var Talabat = db.Talab.Where(t => t.PrintDate >= fromDate && t.PrintDate <= toDate && t.ServicePostion == "الخدمة جاهزة للأستلام" && t.ServiceReadyMsg == false);
                  */
                var TalabatNums = getShahadaList(fromDate, toDate).Select(t => t.TalabNum);
                var Talabat = db.Talab.Where(t => TalabatNums.Contains(t.TalabNum));

                foreach (Talab talab in Talabat)
                {
                    talab.ServicePostion = "تم تسليم الخدمة";
                    talab.ServiceReadyMsg = true;
                    db.Entry(talab).State = EntityState.Modified;
                }

                /*  Talabat = db.Talab.Where(t => t.CardPrintDate >= fromDate && t.CardPrintDate <= toDate && t.CardServicePostion == "الخدمة جاهزة للأستلام" && t.CardServiceReadyMsg == false);
                */

                TalabatNums = getCardList(fromDate, toDate).Select(t => t.TalabNum);
                Talabat = db.Talab.Where(t => TalabatNums.Contains(t.TalabNum));

                foreach (Talab talab in Talabat)
                {
                    talab.CardServicePostion = "تم تسليم الخدمة";
                    talab.CardServiceReadyMsg = true;
                    db.Entry(talab).State = EntityState.Modified;
                }
                db.SaveChanges();
            }
            return Redirect("ServiceReady");
        }

        void UpdateCanceledTalabat()
        {
            DateTime yesterday = DateTime.Today.AddDays(-1);

            var talabat = db.Talab.Where(t => yesterday < t.CreateDate && (t.TalabStatus == "تم ابلاغه بالدفع" || t.TalabStatus == "تم ابلاغه فورى")).ToList();
            talabat = talabat.Where( t=> t.CreateDate.AddDays(1) < DateTime.Now).ToList();
            foreach (Talab talab in talabat)
            {
                talab.TalabStatus = TalabStatus[4];
                db.Entry(talab).State = EntityState.Modified;
            }
            db.SaveChanges();
        }
        //[NonAction]

        //[HttpPost]
        public bool SendNotification(string id)
        {
            var talab = db.Talab.Include(t => t.Citizen).Include(a => a.Area).Where(t => t.TalabNum == id).FirstOrDefault();
            if (talab != null && talab.TalabStatus.Contains("تم ابلاغه"))
            {
                Notification notification = new Notification();
                notification.Status = "Active";
                notification.Message = "السيد/  " + talab.Citizen.FullName + "\n" + "يمكنك الان سداد الرسوم عن طريق ماكينة *فورى*" + "\n" +"لأستخراج الخدمة من: "+ talab.Area.Name+ "\n" + "كود الخدمة: 73602" + "\n" + "رقم الطلب: " + talab.TalabNum + "\n" + "الرقم القومى: " + talab.Citizen.Kawmy + "\n";

                if (String.IsNullOrEmpty(talab.CertificateType))
                {
                    if (talab.ServiceType == "كارنيه")
                        notification.Message += " رسوم الكارنيه : 185 جنيه ";
                    else if (talab.ServiceType == "شهادة و كارنيه")
                        notification.Message += " رسوم الشهادة + الكارنيه : 370 جنيه ";
                    else
                        notification.Message += " رسوم الشهادة الورقية : 185 جنيهاً ";
                }
                else if(talab.CertificateType == "مزدوج الجنسية")
                {
                    if (talab.ServiceType == "كارنيه")
                        notification.Message += " رسوم الكارنيه : 585 جنيه ";
                    else if (talab.ServiceType == "شهادة و كارنيه")
                        notification.Message += " رسوم الشهادة + الكارنيه : 1170 جنيه ";
                    else
                        notification.Message += " رسوم الشهادة الورقية : 585 جنيهاً ";
                }
                notification.Message += "\n" + "إدارة التجنيد و التعبئة" + "\n" + "المركز الإلكترونى لخدمات التجنيد";
                notification.Message += "\t" + talab.Citizen.Phone.Remove(0, 2);
                db.Notification.Add(notification);
                db.SaveChanges();
                return true;
            }
            return false;
            //return new EmptyResult();
        }

        public List<Talab> getShahadaList(DateTime? fromDate, DateTime? toDate)
        {
            List<Talab> talab = db.Talab.Include(a => a.Area).Include(c => c.Citizen).Where(t => (t.ServicePostion.Contains("الخدمة جاهزة للأستلام") && t.ServiceReadyMsg == false)).ToList();

            var ShahadaTalabatV1 = talab.Where(t => t.Area != null && (t.Area.Name.Contains("منطقة") || t.Area.Name.Contains("إدارة"))
                                 && t.PrintDate >= fromDate && t.PrintDate <= toDate).ToList();
            ShahadaTalabatV1.ForEach(t => t.ServiceType = "شهادة");

            var ShahadaTalabatV2 = talab.Where(t => t.Area != null && (t.Area.Name.Contains("قسم") || t.Area.Name.Contains("مكتب"))
                                   && t.PrintDate.AddDays(1) >= fromDate && t.PrintDate.AddDays(1) <= toDate).ToList();
            ShahadaTalabatV2.ForEach(t => t.ServiceType = "شهادة");

            ShahadaTalabatV1.AddRange(ShahadaTalabatV2);

            return ShahadaTalabatV1;
        }

        public List<Talab> getCardList(DateTime? fromDate, DateTime? toDate)
        {
            List<Talab> talab = _db.Talab.Include(a => a.Area).Include(c => c.Citizen).Where(t => (t.CardServicePostion.Contains("الخدمة جاهزة للأستلام") && t.CardServiceReadyMsg == false)).ToList();

            var CardTalabatV1 = talab.Where(t => t.Area != null && t.Area.Name.Contains("إدارة") && t.CardPrintDate != null
                                   && t.CardPrintDate.Value >= fromDate && t.CardPrintDate.Value <= toDate).ToList();
            CardTalabatV1.ForEach(t => t.ServiceType = "كارنيه");


            var CardTalabatV2 = talab.Where(t => t.Area != null && (t.Area.Name.Contains("قسم") || t.Area.Name.Contains("مكتب") || t.Area.Name.Contains("منطقة")) && t.CardPrintDate != null && t.CardPrintDate.Value.AddDays(5) >= fromDate && t.CardPrintDate.Value.AddDays(5) <= toDate).ToList();
            CardTalabatV2.ForEach(t => t.ServiceType = "كارنيه");

            CardTalabatV1.AddRange(CardTalabatV2);

            return CardTalabatV1;
        }

        public ActionResult OrdersReport(DateTime? fromDate, DateTime? toDate)
        {
            var Talabat = db.Talab.Include(c => c.Citizen).Include(c => c.Citizen.Area).Include(a => a.Area);

            if (fromDate != null && toDate != null)
            {
                ViewBag.FromDate = Convert.ToDateTime(fromDate).ToString("yyyy-MM-dd");
                ViewBag.ToDate = Convert.ToDateTime(toDate).ToString("yyyy-MM-dd");
                Talabat = Talabat.Where(t => t.CreateDate >= fromDate && t.CreateDate <= toDate);
            }
            return View(Talabat.ToList());
            
        }
        // get all talabat that is paid and has no service position or card service position from 2 days ago
        public ActionResult PaidWithoutPosition(DateTime? fromDate, DateTime? toDate, string printArea, int? page)
        {
            var talabat = db.Talab.Include(c => c.Citizen).Include(C => C.Citizen.Area).Include(a => a.Area).Where(t => t.TalabStatus == "تم الدفع" && (t.ServiceType == "شهادة" && string.IsNullOrEmpty(t.ServicePostion) ||
            (t.ServiceType == "كارنيه" && string.IsNullOrEmpty(t.CardServicePostion)) ||
            (t.ServiceType == "شهادة و كارنيه" && (string.IsNullOrEmpty(t.ServicePostion) || string.IsNullOrEmpty(t.CardServicePostion))))).ToList();

            if (fromDate != null && toDate != null)
            {
                ViewBag.FromDate = Convert.ToDateTime(fromDate).ToString("yyyy-MM-dd");
                ViewBag.ToDate = Convert.ToDateTime(toDate).ToString("yyyy-MM-dd");
                talabat = talabat.Where(t => t.CreateDate >= fromDate && t.CreateDate <= toDate).ToList();
            }
            if (!String.IsNullOrEmpty(printArea) && !printArea.Contains("الكل"))
            {
                ViewBag.SelectedArea = printArea;
                talabat = talabat.Where(T => T.Citizen.Area != null && T.Citizen.Area.Mntqa == printArea).ToList();
            }
                var PrintArea = db.Area.GroupBy(a => a.Mntqa).Select(t => t.Key).ToList();
                PrintArea.RemoveAt(0);  // إدارة التجنيد @ index 0
            talabat = talabat.OrderBy(t => t.ServiceType).ToList();
            ViewBag.PrintArea = new SelectList(PrintArea, printArea);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(talabat.ToPagedList(pageNumber, pageSize));
            //return View(talabat);
        }
        
        public JsonResult RecievedAreas(string areaName)
        {
            var areas = db.Area.Where(a => a.Mntqa.Contains(areaName)).Select(a => new { a.MktbCode, a.Name }).OrderBy(a => a.Name).ToList();

            if (areaName.Contains("إدارة"))
                areas = db.Area.Where(a => a.Name.Contains("إدارة التجنيد و التعبئة") || a.Name.Contains("مكتب") || a.Name.Contains("قسم") || a.Name.Contains("منطقة")).Select(a => new { a.MktbCode, a.Name }).OrderBy(a => a.Name).ToList();
            
            return Json(areas, JsonRequestBehavior.AllowGet);
        }


    }

}


