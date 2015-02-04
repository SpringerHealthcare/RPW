using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpringHealthOne.Models
{
    public class requestQuoteList
    {
        public List<requestQuoteObj> requestQuotes {get;set;}

        public requestQuoteList()
        {
            this.requestQuotes = new List<requestQuoteObj>();
        }
    }

    public class requestQuoteObj
    {
        public string articleTitle { get; set; }
        public string articleId { get; set; }
        public string citation { get; set; }
        public string author { get; set; }
        public string pubdate { get; set; }
        public int quantities { get; set; }
        public string destcountry { get; set; }
        public bool englang { get; set; }
        public bool translang { get; set; }
        public string specrequirements { get; set; }

        public bool quotecover { get; set; }
        public bool quotenocover { get; set; }
        public bool quotereprints { get; set; }
        public bool quoteeprint { get; set; }
        
        public requestQuoteObj()
        {
            this.englang = true;
            this.quotecover = true;
            this.quotereprints = true;
        }
    }

}