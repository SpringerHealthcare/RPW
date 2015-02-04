using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpringHealthOne.Models
{
    public class FilterSearchResults
    {
        public List<String> filtername { get; set; }
        public List<int> filtercount { get; set; }

        public FilterSearchResults(List<String> filtername, List<int> filtercount)
        {   
            List<int> sortedcount = filtercount.OrderByDescending(p => p).ToList();
            List<string> sortedname = new List<string>();

            foreach (var count in sortedcount)
            {
                var countkey = filtercount.IndexOf(count);
                sortedname.Add(filtername[countkey]);
                filtername.Remove(filtername[countkey]); //remove to prevent pulling the same values
                filtercount.Remove(filtercount[countkey]); //remove to prevent index out of range
            }

            this.filtername = sortedname;
            this.filtercount = sortedcount;

            //this.filtername = filtername;
            //this.filtercount = filtercount;
        }
    }
}