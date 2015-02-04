using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Web.Mvc;
using SpringHealthOne.SpringWS;

namespace SpringHealthOne.Models
{
    public class MyAccount
    {

        [Required]
        public string firstname { get; set; }

        [Required]
        public string lastname { get; set; }

        [Required]
        [EmailAddress]
        public string email { get; set; }

        [Required]
        public string organization { get; set; }

        [Required]
        public string jobtitle { get; set; }

        public string address { get; set; }

        [Required]
        public string country { get; set; }

        public string number { get; set; }

        public string password { get; set; }

        public string aoitext { get; set; }

        public bool tandc { get; set; }

        public bool sales { get; set; }

        public string addterm { get; set; }

        public string linkedterm { get; set; }

        public List<UserTermData> data { get; set; }

        public IEnumerable<SelectListItem> DropDownItems { get; set; }

        public MyAccount()
        {
            DropDownItems = new List<SelectListItem>();
            data = new List<UserTermData>();
        }

        public SelectList getCountry(string selectedcountry = "")
        {
            SMARTService smartService = new SMARTService();
            string[] countries = smartService.Countries("", 999, true); //setting to false prevents any response at all
            return new SelectList(countries, selectedcountry);
        }
    }

    public class UserTermData
    {
        public string term { get; set; }
        public string qualifier { get; set; }
        public bool remove { get; set; }

        public UserTermData()
        {
            this.remove = false;
        }
    }
}