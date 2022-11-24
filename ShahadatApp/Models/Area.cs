using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShahadatApp.Models
{
    public class Area
    {
        [Key, StringLength(3,MinimumLength =1)]
        [Display(Name ="كود المكتب")]
        public string MktbCode { get; set; }

        [Required, MaxLength(128)]
        [Display(Name = "المكتب")]
        public string Name { get; set; }
        [Display(Name = "القسم")]
        public string Qesm { get; set; }
        [Display(Name = "المنطقة")]
        public string Mntqa { get; set; }

        public string MorfkatNum { get; set; }

        public string Location { get; set; }

    }
}