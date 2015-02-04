using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpringHealthOne.Models
{
    public class LoggedOutSearch
    {
        public int total;
        public string searchid;
        public string searchtext;
        public int journel;
        public int books;
        public int anatomical;
        public int cme;

        public List<LoggedOutSearchResults> LoggedOutResults;
        public LoggedOutSearch()
        {
            this.LoggedOutResults = new List<LoggedOutSearchResults>();
        }
    }

    public class LoggedOutSearchResults
    {
        public string title;
        public string Authors;
        public string Journalname;
        public string Date;
        public string Citation;
        public string Abstract;
        public string keysent;
        public int articleId;
    }
}