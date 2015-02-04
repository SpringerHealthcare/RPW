using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Microsoft.Web.Mvc;
using Recaptcha;
using SpringHealthOne.SpringWS;

namespace SpringHealthOne.Models
{
    public class reCaptchaReg
    {
        
    }

    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString GenerateCaptcha(this HtmlHelper helper)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(helper.Raw(helper.GenerateCaptcha("captcha", "clean")));

            return MvcHtmlString.Create(builder.ToString());
        }
    }

    public class RegistrationUserModel
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

        public IEnumerable<SelectListItem> DropDownItems { get; set; }

        [Required]
        public string number { get; set; }

        [Required]
        public string password { get; set; }

        [Required]
        public string confirm { get; set; }

        [Required]
        public bool tandc { get; set; }

        public bool sales { get; set; }

        public RegistrationUserModel()
        {
            DropDownItems = new List<SelectListItem>();
        }

        //i couldnt get this working without the call int he constructor, unfortunatly this means i have to duplicate the same for the account page.
        public SelectList getCountry()
        {
            SMARTService smartService = new SMARTService();
            string[] countries = smartService.Countries("", 999, true); //setting to false prevents any response at all
            return new SelectList(countries, "United Kingdom");
        }
    }
}