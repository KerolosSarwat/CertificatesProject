using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using ShahadatApp.DAL;

namespace ShahadatApp.Models
{
    public class Talab
    {

        [Key, Display(Name = "رقم الطلب")]
        public string TalabNum { get; set; }

        [Display(Name = "الرقم القومى")]
        [ForeignKey("Citizen")]
        public string Kawmy { get; set; }

        [Display(Name = "حالة الطلب")]
        public string TalabStatus { get; set; }

        [ForeignKey("PrintArea"), Display(Name = "مكان الطباعة")]
        public string PrintAreaID { get; set; }

        [ForeignKey("Area"), Display(Name = "مكان الأستلام")]
        public string RecievedAreaID { get; set; }

        [Display(Name = "موقف خدمة الشهادة")]
        public string ServicePostion { get; set; }
        [Display(Name = "موقف خدمة الكارنيه")]
        public string CardServicePostion { get; set; }

        [Display(Name = "نوع الخدمة")]
        public string ServiceType { get; set; }

        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true), Display(Name = "تاريخ طباعة الشهادة")]
        public DateTime PrintDate { get; set; }
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true), Display(Name = "تاريخ طباعة الكارنيه")]
        public DateTime? CardPrintDate { get; set; }

        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }
        public DateTime CreateDate { get; set; }
        [Display(Name = "رسالة تم إبلاغه")]
        public bool TmEbla8Msg { get; set; }
        [Display(Name = "رسالة تم الدفع")]
        public bool TmDaf3Msg { get; set; }
        [Display(Name ="رسالة التفاوض")]
        public bool isNegotiated { get; set; }
        public bool ServiceReadyMsg { get; set; }
        public bool CardServiceReadyMsg { get; set; }
        public bool isCanceled { get; set; }
        public Citizen Citizen { get; set; }
        public Area Area { get; set; }
        public Area PrintArea { get; set; }
        [Display(Name ="نوع الشهادة")]
        public string CertificateType { get; set; }

        public void SendNotification(string id)
        {
            ShahadatAppContext db = new ShahadatAppContext();
            var talab = db.Talab.Include("Citizen").Where(t => t.TalabNum == id).FirstOrDefault();
            if (talab != null)
            {
                Notification notification = new Notification();
                notification.Status = "Active";
                notification.Message = "السيد/ " + talab.Citizen.FullName + "يمكنك الان سداد الرسوم عن طريق ماكينة فورى\n برقم الطلب " + talab.TalabNum;
                db.Notification.Add(notification);
                db.SaveChanges();

            }
            //return RedirectToAction("TalabatByArea");
            //return new EmptyResult();

        }
    }
        [DataContract]
    public class DataPoint
    {
        public DataPoint(string label, double y)
        {
            this.Label = label;
            this.Y = y;
        }

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "label")]
        public string Label = "";

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "y")]
        public Nullable<double> Y = null;
    }
}