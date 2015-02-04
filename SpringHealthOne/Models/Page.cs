using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Globalization;
using SpringHealthOne.SpringWS;
using System.Configuration;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpringHealthOne.Models
{
    public class Page 
    {
        public int PageID { get; set; }
        [Required]
        public string SystemName { get; set; }
        [Required]
        public string Title { get; set; }

        public String PageHeading { get; set; }
        [AllowHtml]
        public string PageBodyColumn1 { get; set; }
        [AllowHtml]
        public string PageBodyColumn2 { get; set; }
        [AllowHtml]
        public string PageBodyColumn3 { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string Keywords { get; set; }
        public bool Published { get; set; }
    }
}