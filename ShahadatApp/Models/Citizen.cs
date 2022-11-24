using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShahadatApp.Models
{
    public class Citizen
    {
        [Key, StringLength(14, MinimumLength = 14), Display(Name = "الرقم القومى")]
        public string Kawmy { get; set; }

        [Required, Display(Name = "الأسم")]
        public string FullName { get; set; }

        [StringLength(13, MinimumLength = 11), Display(Name = "الموبايل")]
        public string Phone { get; set; }
        [Display(Name = "مستخدم واتساب")]
        public bool WhatsAppUser { get; set; }

        [StringLength(4, MinimumLength = 4), Display(Name = "سنة الميلاد")]
        public string Milad { get; set; }

        [ForeignKey("Area"), StringLength(3, MinimumLength = 1), Display(Name = "كود المكتب")]
        public string MktbCode { get; set; }

        [StringLength(8, MinimumLength = 1), Display(Name = "مسلسل")]
        public string Mosalsal { get; set; }

        [Display(Name = "تم استلام المرفقات")]
        public bool MorfkatRecieved { get; set; }

        [Display(Name = "تم ارسال المرفقات")]
        public bool MorfkatSent{ get; set; }
        [Display(Name = "المرفقات")]
        public string MorfakPics { get; set; }
        public string TirnaryNum
        {
            get { return Milad + "/" + MktbCode + "/" + Mosalsal; }
        }
        [DataType(DataType.ImageUrl)]
        public string IDImage { get; set; }
        [DataType(DataType.ImageUrl)]
        public string PersonalPhoto { get; set; }
        public ICollection<Talab> Talabat { get; set; }
        public Area Area { get; set; }
    }
}