using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SpringHealthOne.Models
{
    public class contactus
    {
        [Required]
        public string enquiry { get; set;}
        [Required]
        public string name { get; set; }
        [Required]
        public string email { get; set; }

        public string telephone { get; set; }
        public string company { get; set; }
        public string country { get; set; }
        public string botpot { get; set; }
    }
}