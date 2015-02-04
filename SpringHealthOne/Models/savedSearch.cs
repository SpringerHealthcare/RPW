using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpringHealthOne.Models
{
    public class savedSearch
    {
        public string searchId { get; set; }
        public string searchName { get; set; }
        public int Page { get; set; }
        public string freetext { get; set; }
    }
}