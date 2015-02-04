using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using SpringHealthOne.SpringWS;

namespace SpringHealthOne.Models
{
    public class SearchList
    {
        public List<Search> SearchObjList { get; set; }

        public SearchList()
        {
            this.SearchObjList = new List<Search>();
        }
    }

    public class Search
    {

        public string resultsText { get; set; }

        public string freetext { get; set; }
        //removed the required from here
        public string drug { get; set; }

        public string ForMonth { get; set; }
        public string ForYears { get; set; }
        public string ToMonth { get; set; }
        public string ToYears { get; set; }

        public string prodtype { get; set; }
        public string Journal { get; set; }
        public string title { get; set; }
        public string device { get; set; }
        public string indication { get; set; }
        public string therapy { get; set; }
        public string manufact { get; set; }

        public string author { get; set; }
        public string doi { get; set; }
        public string pubmed { get; set; }
        public string citation { get; set; }
        public string sponsor { get; set; }

        public bool advanced { get; set; }

        public Search()
        {
            ForMonth = "01";
            ForYears = "2000";
            ToMonth = getCurrentMonth();
            ToYears = getCurrentYear().ToString();
        }

        public SelectList getMonths(bool selected = false)
        {
            SelectList monthnums;
            if (selected == false)
            {
                monthnums = new SelectList(new[] { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12" });
            }
            else
            {
                monthnums = new SelectList(new[] { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12" }, getCurrentMonth());
            }
            return monthnums;
        }

        public SelectList getYears(bool selected = false)
        {
            int year = getCurrentYear();
            SelectList yearnums;
            List<int> yearlist = new List<int>();
            for (var y = 1970; y <= year; y++)
            {
                yearlist.Add(y);
            }
            if (selected == false)
            {
                yearnums = new SelectList(yearlist);
            }
            else
            {
                yearnums = new SelectList(yearlist, year);
            }
            return yearnums;
        }

        public string getCurrentMonth()
        {
            return DateTime.Now.Month.ToString("d2");
        }

        public int getCurrentYear()
        {
            return Convert.ToInt32(DateTime.Now.Year.ToString());
        }
    }
}