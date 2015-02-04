using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace SpringHealthOne.Models
{

    public class SearchResultsList
    {
        public List<SearchResults> SearchResultObjList { get; set; }
        public SearchRefineResults SearchRefineObjList { get; set; }

        public SearchResultsList()
        {
            this.SearchResultObjList = new List<SearchResults>();
            this.SearchRefineObjList = new SearchRefineResults();
        }
    }

    public class SearchResults
    {
        public string resultsText;
        public string freetext;
        public string drug;
        public int articleId;
        public string PMID;
        public string searchId;
        public int totalresults;
        public int currentpage;
        public int nextpage;
        public int totalpages;
        public int lastpage;
        public int reprintOpprtunityId;
        public string articleTitle;
        public string authors;
        public string journalTitle;
        public string publicationDate;
        public string citation;
        public string PDFlink;
        public string abstractText;
        public string keySentence;

        // This will convert the passed XYZ object to JSON string
        public static string Serialize(SearchResultsList xyz)
        {
            var serializer = new JavaScriptSerializer();
            return serializer.Serialize(xyz);
        }

        // This will convert the passed JSON string back to XYZ object
        public static SearchResultsList Deserialize(string data)
        {
            var serializer = new JavaScriptSerializer();
            return serializer.Deserialize<SearchResultsList>(data);
        }
    }

    public class SearchRefineResults
    {
        public string[] ProductType;
        public int[] ProductTypecount;
        public string[] Journal;
        public int[] Journalcount;
        public string[] TheropyArea;
        public int[] TheropyAreacount;
    }
}