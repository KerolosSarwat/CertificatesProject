using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ShahadatApp.DAL;
using ShahadatApp.Models;
using PagedList;
using System.Windows.Forms;
using ShahadatApp.PrivateClasses;
using Newtonsoft.Json;

namespace ShahadatApp.Controllers
{
    public class HomeController : Controller
    {
        private ShahadatAppContext db = new ShahadatAppContext();
        List<string> servicePostion = new List<string>()
            {
                "", "إستخراج كاشير", "تسجيل بيانات", "مراجعة بيانات", "طباعة الخدمة", "إستلام مندوب", "الخدمة جاهزة للأستلام", "تم تسليم الخدمة"
            };
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact(DateTime? fromDate, DateTime? toDate)
        {
            var talab = db.Talab.Include(t => t.Area).Include(t => t.Citizen).Include(t => t.PrintArea);



            if (fromDate!= null && toDate!= null)
               talab = from t in talab
                       where t.CreateDate >= fromDate && t.CreateDate <= toDate
                       select t;
        
            ViewData["TalabCounter"] = talab.Count();

            List<DataPoint> dataPoints = new List<DataPoint>();
            float counter1 = talab.Where(t => t.TalabStatus.Contains("تم ابلاغه")).Count();
            float counter2 = talab.Where(t => t.TalabStatus.Contains("تم الدفع")).Count();
            int totalCounter = talab.Count();
            double tmEbla8Perc = (int)Math.Round(((counter1 / totalCounter) * 100), 0);
            double tmDaf3Perc = (int)Math.Round(((counter2 / totalCounter) * 100), 0);
            ViewData["TalbEbl8"] = counter1;
            ViewData["TalbDf3"] = counter2;
            dataPoints.Add(new DataPoint("تم ابلاغه ", tmEbla8Perc));
            dataPoints.Add(new DataPoint("تم الدفع ", tmDaf3Perc));
            ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);

            ViewBag.Message = "Your contact page.";
            for (int i = 0; i < 8; i++)
            {
                string postion = servicePostion[i];
                if(i== 0)
                    ViewData["NoPostion"] = talab.Where(t => String.IsNullOrEmpty(t.ServicePostion)).Count();
                else
                    ViewData[servicePostion[i]] = talab.Where(t => t.ServicePostion.Contains(postion)).Count();
            }
            return View(talab);
        }

      
    }
}