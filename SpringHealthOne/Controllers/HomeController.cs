using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using SpringHealthOne.Helpers;
using SpringHealthOne.Models;
using System.Collections;
using System.ServiceModel;
using System.Net.Mail;
using SpringHealthOne.Services;
using SpringHealthOne.SpringWS;

namespace SpringHealthOne.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string drug, string ForMonth = null, string ForYears = null, string ToMonth = null, string ToYears = null, string productType = null, string journalTitle = null, string sort_by = null, string sort_order = null)
        {
            Search model = new SpringHealthOne.Models.Search();

            return View(model);
        }

        public ActionResult AutoComplete(string term)
        {
            SmartService smartService = new SmartService();
            string[] recdrug = smartService.Drugs(term, 10);

            return Json(recdrug, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AutoCompleteJournel(string term)
        {
            SmartService smartService = new SmartService();
            string[] recdrug = smartService.Journals(term, 10);

            return Json(recdrug, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AutoCompleteDevices(string term)
        {
            SmartService smartService = new SmartService();
            string[] recdrug = smartService.Devices(term, 10);

            return Json(recdrug, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AutoCompleteIndications(string term)
        {
            SMARTService smartService = new SMARTService();
            string[] recdrug = smartService.Indications(term, 10, true);

            return Json(recdrug, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AutoCompleteTherapyAreas(string term)
        {
            SmartService smartService = new SmartService();
            string[] recdrug = smartService.TherapyAreas(term, 10);

            return Json(recdrug, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AutoCompleteManufacturers(string term)
        {
            SmartService smartService = new SmartService();
            string[] recdrug = smartService.Manufacturers(term, 10);

            return Json(recdrug, JsonRequestBehavior.AllowGet);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contactus(contactus model)
        {
            //if this field has anything, its a bot so just ignore it.
            if (model.botpot != null)
            {
                return View();
            }

            SmartService smartService = new SmartService();
            if (ModelState.IsValid)
            {
                try
                {
                    ContactUsType cut = new ContactUsType();
                    cut.enquiry = model.enquiry;
                    cut.name = model.name;
                    cut.email = model.email;
                    cut.telephone = model.telephone;
                    cut.company = model.company;
                    cut.country = model.country;

                    string success = smartService.ContactUs(cut);

                    TempData["Message"] = success;
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "";
                    if (ex is FaultException)
                    {
                        TempData["Error"] = "Error Message: " + ex.Message;
                    }
                    if (ex is ArgumentException)
                    {
                        TempData["Error"] = "Error Message: " + ex.Message;
                    }

                    if ((TempData["Error"].ToString()) == "")
                    {
                        TempData["Error"] = ex.Message;
                    }
                }
            }

            return RedirectToAction("../Pages/Details", new { id = 5 });
        }

        public ActionResult BasicSearch(Search model)
        {
            if (String.IsNullOrEmpty(model.freetext) || model.freetext.Trim() == "") //changed from model state as it will always be invalid (Missing drug field)
            {
                TempData["Error"] = "Please enter a search string";
                return View("Index", model);
            }

            //clear any privious search data.
            if (Session[model.freetext] != null)
            {
                Session[model.freetext] = null;
            }

            SmartService smartService = new SmartService();
            int userid = 0;
            SearchResultsList ResultList = new SearchResultsList();
            try
            {

                if (Request.IsAuthenticated)
                { //check if logged in
                    string key, toHash, token;
                    token = smartService.AuthToken();
                    string username = User.Identity.Name;
                    toHash = username.ToUpper() + "|" + token;
                    key = smartService.GetMd5Hash(MD5.Create(), toHash) + "|" + username;


                    if (token == "")
                    { //do something if no token
                        SummaryResultsType searchres = smartService.GetBasicSearchSummaryResults(0, model.freetext, model.ForMonth, model.ForYears, model.ToMonth, model.ToYears, "All", null, "", "DESC");
                        string search_id = searchres.search_id;
                        Session[search_id] = searchres;

                    }
                    else
                    {
                        AccountType Account = smartService.GetAccountDetails(key, username);
                        SummaryResultsType searchres = smartService.GetBasicSearchSummaryResults(Account.userId, model.freetext, model.ForMonth, model.ForYears, model.ToMonth, model.ToYears, "All", null, "", "DESC");
                        string search_id = searchres.search_id;
                        Session[search_id] = searchres;


                        string pagestart = ((1 * 6) - 5).ToString();
                        string pageend = (1 * 6).ToString();
                        int lastpage = 1;

                        FullResultsType fullResults = smartService.GetLoggedInSearchResults(key, search_id, pagestart, pageend);
                        if (fullResults.numResults == 0)
                        { //no results!
                            TempData["noresults"] = "No Summary Results Found";
                            TempData["noresultssearchid"] = search_id;
                        }
                        else
                        {
                            FullResultsType Filterresults = smartService.GetLoggedInSearchResults(key, search_id, "1", fullResults.numResults.ToString());
                            SearchRefineResults SearchRefineResults = new SearchRefineResults(); //initialise search refine results

                            model.resultsText = createSearchString(model);
                            foreach (ArticleType at in fullResults.contentTypeResults)
                            { //do this with a model instead
                                SearchResults SearchResults = new SearchResults();
                                SearchResults.freetext = model.freetext; // need to map drug to this
                                SearchResults.articleId = at.articleId;
                                SearchResults.searchId = search_id;
                                SearchResults.totalresults = fullResults.numResults;
                                SearchResults.currentpage = 1;
                                SearchResults.lastpage = lastpage;
                                SearchResults.nextpage = ((fullResults.numResults / 6) > 6) ? 2 : 1;
                                int modulus = fullResults.numResults % 6;
                                SearchResults.totalpages = (modulus>0) ? (fullResults.numResults / 6) + 1 : (fullResults.numResults / 6);
                                SearchResults.reprintOpprtunityId = at.reprintOpprtunityId;
                                SearchResults.articleTitle = at.articleTitle;
                                SearchResults.authors = at.authors;
                                SearchResults.journalTitle = at.journalTitle;
                                SearchResults.publicationDate = at.publicationDate;
                                SearchResults.citation = at.citation;
                                SearchResults.PDFlink = at.PDFlink;
                                SearchResults.abstractText = at.abstractText;
                                SearchResults.keySentence = at.keySentence;
                                SearchResults.resultsText = model.resultsText;

                                ResultList.SearchResultObjList.Add(SearchResults);

                            }

                            //now that the above result row is built, build filters with the filter results.
                            //first, journals
                            List<string> Journals = new List<string>(); // set jounal list<string> for refined search filter
                            List<string> TheropyArea = new List<string>(); // set jounal list<string> for refined search filter
                            foreach (ArticleType at in Filterresults.contentTypeResults)
                            {
                                Journals.Add(at.journalTitle);
                                TheropyArea.Add(at.therapyArea);
                            }

                            //assign the results - needs cleaning up
                            FilterSearchResults Journalfilter = buildfilteritems(Journals);
                            SearchRefineResults.Journal = Journalfilter.filtername.ToArray();
                            SearchRefineResults.Journalcount = Journalfilter.filtercount.ToArray();

                            FilterSearchResults TheropyAreafilter = buildfilteritems(TheropyArea, true);
                            SearchRefineResults.TheropyArea = TheropyAreafilter.filtername.ToArray();
                            SearchRefineResults.TheropyAreacount = TheropyAreafilter.filtercount.ToArray();

                            //Array positions are hardcoded, this is probably bad.
                            SearchRefineResults.ProductType = new string[4];
                            SearchRefineResults.ProductTypecount = new int[4];
                            foreach (ContentType ct in searchres.contentTypeResults)
                            {
                                if (ct.StringValue == "Anatomical Charts")
                                {
                                    SearchRefineResults.ProductType[2] = "Anatomical Charts";
                                    SearchRefineResults.ProductTypecount[2] = ct.numResults;
                                }

                                if (ct.StringValue == "Books")
                                {
                                    SearchRefineResults.ProductType[1] = "Books";
                                    SearchRefineResults.ProductTypecount[1] = ct.numResults;
                                }

                                if (ct.StringValue == "CME")
                                {
                                    SearchRefineResults.ProductType[3] = "CME";
                                    SearchRefineResults.ProductTypecount[3] = ct.numResults;
                                }

                                if (ct.StringValue == "Journal Articles")
                                {
                                    SearchRefineResults.ProductType[0] = "Journal Articles";
                                    SearchRefineResults.ProductTypecount[0] = ct.numResults;
                                }
                            }

                            ResultList.SearchRefineObjList = SearchRefineResults;
                        }
                    }
                }
                else
                {
                    SummaryResultsType searchres = smartService.GetBasicSearchSummaryResults(0, model.freetext, model.ForMonth, model.ForYears, model.ToMonth, model.ToYears, "All", null, "", "DESC");
                    string search_id = searchres.search_id;
                    Session[search_id] = searchres;

                    LoggedOutSearch loggedoutres = new LoggedOutSearch();
                    loggedoutres.total = searchres.numResults;
                    loggedoutres.searchid = searchres.search_id;
                    loggedoutres.searchtext = model.freetext;
                    foreach (ContentType ct in searchres.contentTypeResults)
                    {
                        if (ct.StringValue == "Anatomical Charts")
                        {
                            loggedoutres.anatomical = ct.numResults;
                        }

                        if (ct.StringValue == "Books")
                        {
                            loggedoutres.books = ct.numResults;
                        }

                        if (ct.StringValue == "CME")
                        {
                            loggedoutres.cme = ct.numResults;
                        }

                        if (ct.StringValue == "Journal Articles")
                        {
                            loggedoutres.journel = ct.numResults;
                        }
                    }

                    try
                    {
                        string key, token;
                        token = smartService.AuthToken();
                        if (token == "")
                            TempData["Error"] = "No Token generated.";
                        else
                        {
                            key = smartService.GetMd5Hash(MD5.Create(), token);
                            FullResultsType results = smartService.GetLoggedOutSearchResults(key, searchres.search_id, "3");
                            if (results.numResults == 0)
                                TempData["Message"] = "No Results Found.";
                            else
                            {
                                foreach (ArticleType at in results.contentTypeResults)
                                {
                                    LoggedOutSearchResults loggedoutresults = new LoggedOutSearchResults();
                                    loggedoutresults.title = at.articleTitle;
                                    loggedoutresults.Authors = at.authors;
                                    loggedoutresults.Journalname = at.journalTitle;
                                    loggedoutresults.Date = at.publicationDate;
                                    loggedoutresults.Citation = at.citation;
                                    loggedoutresults.Abstract = at.abstractText;
                                    loggedoutresults.keysent = at.keySentence;
                                    loggedoutresults.articleId = at.articleId;

                                    loggedoutres.LoggedOutResults.Add(loggedoutresults);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        TempData["Error"] = "";
                        if (ex is FaultException)
                        {
                            TempData["Error"] = "Error Message: " + ex.Message;
                        }
                        if (ex is ArgumentException)
                        {
                            TempData["Error"] = "Error Message: " + ex.Message;
                        }

                        if ((TempData["Error"].ToString()) == "")
                        {
                            TempData["Error"] = ex.Message;
                        }
                    }
                    TempData["searchresults"] = "true";
                    TempData["searchtest"] = model.freetext;
                    return View("LoggedOutSearch", loggedoutres);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "";
                if (ex is FaultException)
                {
                    TempData["Error"] = "Error Message: " + ex.Message;
                }
                if (ex is ArgumentException)
                {
                    TempData["Error"] = "Error Message: " + ex.Message;
                }

                if ((TempData["Error"].ToString()) == "")
                {
                    TempData["Error"] = ex.Message;
                }
            }

            TempData["searchresults"] = "search-page";
            Session["latestresults"] = ResultList;
            return View("Search", ResultList);
        }

        public ActionResult Search(Search model, bool advanced = false, string search_id = "", int Page = 1)
        {
            SmartService smartService = new SmartService();
            SearchResultsList ResultList = new SearchResultsList();
            SummaryResultsType searchres = null;
            int userid = 0;
            bool userIdSpecified = false;
            string key="", toHash, token="";
            try
            {
                token = smartService.AuthToken();

                if (token != "")
                {
                    // Create a validation key based on the current logged in username and authentication token 
                    string username = User.Identity.Name;
                    toHash = username.ToUpper() + "|" + token;
                    key = smartService.GetMd5Hash(MD5.Create(), toHash) + "|" + username;
                    AccountType Account = smartService.GetAccountDetails(key, username);
                    userid = Account.userId;
                    userIdSpecified = true;

                }
            }
            catch{}
            if (search_id == "") // not set? then its a new search!
            {
                if (!ModelState.IsValid)
                {
                    return View("AdvancedSearch", model);
                }

                model.ForMonth = Convert.ToDateTime("01-" + model.ForMonth + "-2011").Month.ToString().PadLeft(2, '0');
                model.ToMonth = Convert.ToDateTime("01-" + model.ToMonth + "-2011").Month.ToString().PadLeft(2, '0');

                if (model.Journal == "All") //if this is set to the custom added string, reset it to null.
                {
                    model.Journal = null;
                }

                if (advanced == true) // true? then this is an advanced search!
                {
                    //if (model.freetext == null && model.drug == null)
                    //{
                    //    TempData["Error"] = "The Drug field is required.";
                    //    return View("AdvancedSearch", model);
                    //}


                    if ((Request.UrlReferrer.PathAndQuery == "/Home/AdvancedSearch")
                        && (String.IsNullOrEmpty(model.drug) || model.drug.Trim() == "")
                        && (String.IsNullOrEmpty(model.Journal) || model.Journal.Trim() == "")
                        && (String.IsNullOrEmpty(model.title) || model.title.Trim() == "")
                        && (String.IsNullOrEmpty(model.device) || model.device.Trim() == "")
                        && (String.IsNullOrEmpty(model.indication) || model.indication.Trim() == "")
                        && (String.IsNullOrEmpty(model.therapy) || model.therapy.Trim() == "")
                        && (String.IsNullOrEmpty(model.manufact) || model.manufact.Trim() == "")
                        && (String.IsNullOrEmpty(model.author) || model.author.Trim() == "")
                        && (String.IsNullOrEmpty(model.doi) || model.doi.Trim() == "")
                        && (String.IsNullOrEmpty(model.pubmed) || model.pubmed.Trim() == "")
                        && (String.IsNullOrEmpty(model.citation) || model.citation.Trim() == "")
                        && (String.IsNullOrEmpty(model.sponsor) || model.sponsor.Trim() == ""))
                    {
                        TempData["Error"] = "You must supply some values in order to perform the search.";
                        return View("AdvancedSearch", model);
                    }
                    if (Session[model.freetext] != null)
                    {

                        Search originalsearch = (Search)Session[model.freetext];

                        if (Request.UrlReferrer.PathAndQuery != "/Home/AdvancedSearch")
                        {


                            //check for product refinements

                            CheckModelRefinements(model, originalsearch);


                        }
                    }
                    try
                    {
                        
                        searchres = smartService.GetAdvancedSearchSummaryResults(userid, model.freetext, model.drug, model.ForMonth, model.ForYears,
                            model.ToMonth, model.ToYears, model.prodtype, model.Journal, model.title, model.device, model.indication,
                            model.therapy, model.manufact, model.author, model.doi, model.pubmed, model.citation, model.sponsor, "", "DESC");

                        model.resultsText = createSearchString(model);

                    }
                    catch (Exception ex) // apparently not throwing the expected fault exception. handling argument exception instead.
                    {
                        TempData["Error"] = "";
                        if (ex is FaultException)
                        {
                            TempData["Error"] = "Error Message: " + ex.Message;
                        }
                        if (ex is ArgumentException)
                        {
                            TempData["Error"] = "Error Message: " + ex.Message;
                        }

                        if ((TempData["Error"].ToString()) == "")
                        {
                            TempData["Error"] = ex.Message;
                        }
                        return View("AdvancedSearch", model);
                    }
                }
                else //otherwise, just run a basic.
                {
                    try
                    {
                        searchres = smartService.GetBasicSearchSummaryResults(userid,  model.drug, model.ForMonth, model.ForYears, model.ToMonth, model.ToYears, "All", null, "", "DESC");
                    }
                    catch (Exception ex) // apparently not throwing the expected fault exception. handling argument exception instead.
                    {
                        TempData["Error"] = "";
                        if (ex is FaultException)
                        {
                            TempData["Error"] = "Error Message: " + ex.Message;
                        }
                        if (ex is ArgumentException)
                        {
                            TempData["Error"] = "Error Message: " + ex.Message;
                        }

                        if ((TempData["Error"].ToString()) == "")
                        {
                            TempData["Error"] = ex.Message;
                        }
                        return View("AdvancedSearch", model);
                    }
                }
                search_id = searchres.search_id.ToString();
                Session[search_id] = searchres;
                if (advanced == true)
                {
                    Session["advanced" + search_id] = model;
                }
            }

            if (Request.IsAuthenticated)
            { //check if logged in
                
                if (token == "")
                { //do something if no token

                }
                else
                {
                    string pagestart = ((Page * 6) - 5).ToString(); ;
                    string pageend = (Page * 6).ToString();


                    int lastpage;
                    if (Page > 1)
                    {
                        lastpage = Page - 1;
                    }
                    else
                    {
                        lastpage = Page;
                    }

                    var tot = Session[search_id] as SummaryResultsType;
                    int total = tot.numResults;

                    if (int.Parse(pageend) > total)
                    {
                        pageend = total.ToString();
                        //int t = total - 5;
                       // pagestart = t.ToString();
                    }

                    FullResultsType fullResults = smartService.GetLoggedInSearchResults(key, search_id, pagestart, pageend);

                    if (fullResults.numResults == 0)
                    { //no results!
                        //no results to we add the total of 0 to the search result list for returning later.
                        // we also add the freetext and drug fields so we can display what was actually searched for.
                        // finally set the search ID for easy re-searching of the data
                        SearchResults SearchResults = new SearchResults();
                        SearchResults.searchId = search_id;
                        SearchResults.freetext = (model.freetext != "") ? model.freetext : model.drug; // need to map drug to this
                        SearchResults.drug = model.drug;
                        SearchResults.totalresults = fullResults.numResults;
                        SearchResults.resultsText = model.resultsText;
                        ResultList.SearchResultObjList.Add(SearchResults);

                        TempData["noresults"] = "Your search for '" + model.freetext + "' returned 0 results.";
                    }
                    else
                    {
                        FullResultsType Filterresults = smartService.GetLoggedInSearchResults(key, search_id, "1", fullResults.numResults.ToString());
                        SearchRefineResults SearchRefineResults = new SearchRefineResults(); //initialise search refine results
                        string text = ""; //if drug is set we use that rather than freetext. need a var to contain either.

                        if (Session[search_id] == null)
                        {
                            searchres = new SummaryResultsType();
                            if (search_id != null || search_id != "")
                            {
                                string savedsearchtoken, savedsearchkey;
                                savedsearchtoken = smartService.AuthToken();
                                savedsearchkey = smartService.GetMd5Hash(MD5.Create(), savedsearchtoken);

                                SavedSearchType saved_search = new SavedSearchType();

                                saved_search = smartService.GetASavedSearch(savedsearchkey, search_id);

                                if (saved_search.searchId == null)
                                {
                                    TempData["Error"] = "Failed to get A Saved Search.";
                                    return RedirectToAction("SavedSearch", "User");
                                }
                                System.DateTime fromdate = Convert.ToDateTime(saved_search.fromDate);
                                System.DateTime todate = Convert.ToDateTime(saved_search.toDate);

                                text = saved_search.drug != null ? saved_search.drug : saved_search.searchTerm;
                                text = text.Replace("\"", "");
                                searchres = smartService.GetBasicSearchSummaryResults(userid, text, fromdate.Month.ToString(), fromdate.Year.ToString(), todate.Month.ToString(), todate.Year.ToString(), "All", null, "", "DESC");
                            }
                        }
                        else
                        {
                            searchres = ((SummaryResultsType)(Session[search_id]));
                        }

                        foreach (ArticleType at in fullResults.contentTypeResults)
                        { //do this with a model instead
                            SearchResults SearchResults = new SearchResults();
                            if (model.drug != null || model.freetext != null)
                            {
                                SearchResults.freetext = (model.freetext != "") ? model.freetext : model.drug; // need to map drug to this
                            }
                            else
                            {
                                SearchResults.freetext = text;
                            }
                            SearchResults.drug = model.drug;
                            SearchResults.articleId = at.articleId;
                            SearchResults.searchId = search_id;
                            SearchResults.totalresults = fullResults.numResults;
                            SearchResults.currentpage = Page;
                            SearchResults.lastpage = lastpage;
                            SearchResults.nextpage = ((fullResults.numResults / 6) > 6) ? (Page < (fullResults.numResults / 6)) ? Page + 1 : Page : 1;
                            int modulus = fullResults.numResults % 6;
                            SearchResults.totalpages = (modulus>0) ? (fullResults.numResults / 6) + 1 : (fullResults.numResults / 6); // HARD CODEDED ALERT!
                            SearchResults.reprintOpprtunityId = at.reprintOpprtunityId;
                            SearchResults.articleTitle = at.articleTitle;
                            SearchResults.authors = at.authors;
                            SearchResults.journalTitle = at.journalTitle;
                            SearchResults.publicationDate = at.publicationDate;
                            SearchResults.citation = at.citation;
                            SearchResults.PDFlink = at.PDFlink;
                            SearchResults.abstractText = at.abstractText;
                            SearchResults.keySentence = at.keySentence;
                            SearchResults.resultsText = model.resultsText;
                            ResultList.SearchResultObjList.Add(SearchResults);
                        }

                        //now that the above result row is built, build filters with the filter results.
                        //first, journals
                        List<string> Journals = new List<string>(); // set jounal list<string> for refined search filter
                        List<string> TheropyArea = new List<string>(); // set jounal list<string> for refined search filter
                        foreach (ArticleType at in Filterresults.contentTypeResults)
                        {
                            Journals.Add(at.journalTitle);
                            TheropyArea.Add(at.therapyArea);
                        }

                        //assign the results - needs cleaning up
                        FilterSearchResults Journalfilter = buildfilteritems(Journals);
                        SearchRefineResults.Journal = Journalfilter.filtername.ToArray();
                        SearchRefineResults.Journalcount = Journalfilter.filtercount.ToArray();

                        FilterSearchResults TheropyAreafilter = buildfilteritems(TheropyArea, true);
                        SearchRefineResults.TheropyArea = TheropyAreafilter.filtername.ToArray();
                        SearchRefineResults.TheropyAreacount = TheropyAreafilter.filtercount.ToArray();

                        //Array positions are hardcoded, this is probably bad.
                        SearchRefineResults.ProductType = new string[4];
                        SearchRefineResults.ProductTypecount = new int[4];


                        //Current if coming from a saved search, the session variable isnt set. the end result is that
                        //no summery results exist. To get summery results, we need to re-request the saved entry 
                        //and generate sumery results from it.

                        //Phil to create a method for handling the request of a saved search by search id in order to 
                        //generate the missing summery results

                        foreach (ContentType ct in searchres.contentTypeResults)
                        {
                            if (ct.StringValue == "Anatomical Charts")
                            {
                                SearchRefineResults.ProductType[2] = "Anatomical Charts";
                                SearchRefineResults.ProductTypecount[2] = ct.numResults;
                            }

                            if (ct.StringValue == "Books")
                            {
                                SearchRefineResults.ProductType[1] = "Books";
                                SearchRefineResults.ProductTypecount[1] = ct.numResults;
                            }

                            if (ct.StringValue == "CME")
                            {
                                SearchRefineResults.ProductType[3] = "CME";
                                SearchRefineResults.ProductTypecount[3] = ct.numResults;
                            }

                            if (ct.StringValue == "Journal Articles")
                            {
                                SearchRefineResults.ProductType[0] = "Journal Articles";
                                SearchRefineResults.ProductTypecount[0] = ct.numResults;
                            }
                        }

                        ResultList.SearchRefineObjList = SearchRefineResults;
                        ResultList.SearchResultObjList[0].resultsText = model.resultsText;
                    }
                }
            }
            else
            {
                LoggedOutSearch loggedoutres = new LoggedOutSearch();
                loggedoutres.total = searchres.numResults;
                loggedoutres.searchid = searchres.search_id;
                foreach (ContentType ct in searchres.contentTypeResults)
                {
                    if (ct.StringValue == "Anatomical Charts")
                    {
                        loggedoutres.anatomical = ct.numResults;
                    }

                    if (ct.StringValue == "Books")
                    {
                        loggedoutres.books = ct.numResults;
                    }

                    if (ct.StringValue == "CME")
                    {
                        loggedoutres.cme = ct.numResults;
                    }

                    if (ct.StringValue == "Journal Articles")
                    {
                        loggedoutres.journel = ct.numResults;
                    }
                }

                return View("LoggedOutSearch", loggedoutres);
            }

            if (advanced == true)
            {
                Session[model.freetext] = model;
            }

            TempData["searchresults"] = "search-page";
            Session["latestresults"] = ResultList;
            return View(ResultList);
        }

        private void CheckModelRefinements(Search model, Search originalsearch)
        {
            if (originalsearch.drug != null && model.drug == null)
            {
                model.drug = originalsearch.drug;
            }

            //date can have a number of differences so handle each one...
            if (originalsearch.ForMonth != null && originalsearch.ForMonth != model.ForMonth)
            {
                model.ForMonth = originalsearch.ForMonth;
            }

            if (originalsearch.ForYears != null && originalsearch.ForYears != model.ForYears)
            {
                model.ForYears = originalsearch.ForYears;
            }

            if (originalsearch.ToMonth != null && originalsearch.ToMonth != model.ToMonth)
            {
                model.ToMonth = originalsearch.ToMonth;
            }

            if (originalsearch.ToYears != null && originalsearch.ToYears != model.ToYears)
            {
                model.ToYears = originalsearch.ToYears;
            }

            if (originalsearch.prodtype != null && model.prodtype == null)
            {
                model.prodtype = originalsearch.prodtype;
            }

            if (originalsearch.Journal != null && originalsearch.Journal != model.Journal)
            {
                model.Journal = originalsearch.Journal;
            }


            if (originalsearch.title != null && originalsearch.title != model.title)
            {
                model.title = originalsearch.title;
            }

            if (originalsearch.device != null && originalsearch.device != model.device)
            {
                model.device = originalsearch.device;
            }

            if (originalsearch.indication != null && originalsearch.indication != model.indication)
            {
                model.indication = originalsearch.indication;
            }
            //check for theropy refinements
            if (originalsearch.therapy != null && originalsearch.therapy != model.therapy)
            {
                model.therapy = originalsearch.therapy;
            }

            if (originalsearch.manufact != null && originalsearch.manufact != model.manufact)
            {
                model.manufact = originalsearch.manufact;
            }



            if (originalsearch.author != null && originalsearch.author != model.author)
            {
                model.author = originalsearch.author;
            }



            if (originalsearch.doi != null && originalsearch.doi != model.doi)
            {
                model.doi = originalsearch.doi;
            }

            if (originalsearch.pubmed != null && originalsearch.doi != model.pubmed)
            {
                model.pubmed = originalsearch.pubmed;
            }

            if (originalsearch.citation != null && originalsearch.citation != model.citation)
            {
                model.citation = originalsearch.citation;
            }

            if (originalsearch.sponsor != null && originalsearch.sponsor != model.sponsor)
            {
                model.sponsor = originalsearch.sponsor;
            }
        }

        private string createSearchString(Search model)
        {

            StringBuilder searchVals = new StringBuilder();

            if (model.drug != null)
            {
                searchVals.Append("Drug = " + model.drug + ",");
            }

            //date can have a number of differences so handle each one...
            if (model.ForMonth != null)
            {

                searchVals.Append("From month = " + model.ForMonth + ",");
            }

            if (model.ForYears != null)
            {

                searchVals.Append("From year = " + model.ForYears + ",");
            }

            if (model.ToMonth != null)
            {

                searchVals.Append("To month = " + model.ToMonth + ",");
            }

            if (model.ToYears != null)
            {

                searchVals.Append("To year = " + model.ToYears + ",");
            }

            if (model.prodtype != null)
            {

                searchVals.Append("Type = " + model.prodtype + ",");
            }

            if (model.Journal != null)
            {

                searchVals.Append("Journal = " + model.Journal + ",");
            }


            if (model.title != null)
            {

                searchVals.Append("Title = " + model.title + ",");
            }

            if (model.device != null)
            {

                searchVals.Append("Device = " + model.device + ",");
            }

            if (model.indication != null)
            {

                searchVals.Append("Indication = " + model.indication + ",");
            }
            //check for theropy refinements
            if (model.therapy != null)
            {

                searchVals.Append("Therapy = " + model.therapy + ",");
            }

            if (model.manufact != null)
            {

                searchVals.Append("Manufacturer = " + model.manufact + ",");
            }


            if (model.author != null)
            {
                model.author = model.author;
                searchVals.Append("Author = " + model.author + ",");
            }


            if (model.doi != null)
            {

                searchVals.Append("DOI = " + model.doi + ",");
            }

            if (model.pubmed != null)
            {

                searchVals.Append("PubMed = " + model.pubmed + ",");
            }

            if (model.citation != null)
            {

                searchVals.Append("Citation = " + model.citation + ",");
            }

            if (model.sponsor != null)
            {

                searchVals.Append("Sponsor = " + model.sponsor + ",");
            }

            return searchVals.ToString().Substring(0, searchVals.ToString().Length - 1);

        }




        //*******************************************************************************************************************
        //assign everything to the search model before passing it back through the search function.
        public void refineresults(string searchid)
        {
            //var ResultList = (SearchResultsList)Session[searchid];
            Search Searchres = new Search();
            Search(Searchres);
        }

        public ActionResult AdvancedSearch(string search_id)
        {
            Search Search = new Search();
            if (Session["advanced" + search_id] != null)
            {
                //if this session exists, load the search data. Typecast is needed here.
                Search = ((Search)(Session["advanced" + search_id]));
            }

            return View("AdvancedSearch", Search);
        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "requestQuote")]
        public ActionResult requestQuote(SearchResultsList model, string[] quote, int roid = 0)
        {
            if (quote == null)
            {
                if (TempData["RequestedQuotes"] != null)
                {
                    quote = (string[])TempData["RequestedQuotes"];
                }
                else
                {
                    TempData["Error"] = "Please select at least 1 result for a quote.";
                    return View("Search", Session["latestresults"]);
                }
            }

            requestQuoteList QuoteList = new requestQuoteList();
            foreach (var article in quote)
            {
                List<string> idname = article.Split(',').ToList<string>();
                requestQuoteObj reqTempObj = new requestQuoteObj();
                reqTempObj.articleId = idname[0];
                reqTempObj.articleTitle = idname[1];
                QuoteList.requestQuotes.Add(reqTempObj);
            }
            return View("RequestQuote", QuoteList);
        }

        [HttpGet]
        public ActionResult requestQuote(int roid = 0)
        {
            if (roid == 0)
            {
                return RedirectToAction("Login", "User");
            }

            return RedirectToAction("Login", "User", new { roid = roid });
        }

        //[Authorize]
        public ActionResult requestQuoteviaURL(int roid = 0)
        {
            if (Request.IsAuthenticated)
            {
                //NOTE::: This was added a while after the origonal quote system was built. Added to allow quotes via URL
                // A lot of blatent duplicate code in here, its dirty but works for now.
                SmartService smartService = new SmartService();
                string key, token;
                token = smartService.AuthToken();
                key = smartService.GetMd5Hash(MD5.Create(), token);

                ArticleType at = smartService.GetArticleDetails(key, roid.ToString());

                requestQuoteList QuoteList = new requestQuoteList();
                requestQuoteObj reqTempObj = new requestQuoteObj();
                reqTempObj.articleId = at.articleId.ToString();
                reqTempObj.articleTitle = at.articleTitle;
                QuoteList.requestQuotes.Add(reqTempObj);

                return View("RequestQuote", QuoteList);
            }
            return RedirectToAction("Login", "User", new { roid = roid });
        }

        public ActionResult submitQuote(requestQuoteList model)
        {
            SmartService smartService = new SmartService();
            try
            {
                if (Request.IsAuthenticated)
                {
                    string key, toHash, token;
                    token = smartService.AuthToken();
                    if (token == "")
                    {

                    }
                    else
                    {
                        string username = User.Identity.Name;
                        toHash = username.ToUpper() + "|" + token;
                        key = smartService.GetMd5Hash(MD5.Create(), toHash) + "|" + username;

                        AccountType Account = smartService.GetAccountDetails(key, username);
                        requestQuoteObj[] requestQuotes = model.requestQuotes.ToArray();

                        if (!ModelState.IsValid)
                        {

                        }

                        QuoteRequestType[] submitquoterequest = new QuoteRequestType[requestQuotes.Count()];
                        var i = 0;
                        foreach (var quote in requestQuotes)
                        {
                            QuoteRequestType qrt = new QuoteRequestType();

                            if (!Request.UrlReferrer.PathAndQuery.ToString().Contains("requestQuote"))
                            {

                                qrt.articleId = Convert.ToInt32(quote.articleId);
                                qrt.articleIdSpecified = true;
                            }
                            else
                            {
                                qrt.pubmedId = int.Parse(quote.articleId);
                                qrt.pubmedIdSpecified = true;


                            }

                            qrt.destinationCountry = quote.destcountry;

                            qrt.quantities = quote.quantities + " copies";

                            qrt.englishLanguage = quote.englang;
                            qrt.englishLanguageSpecified = true;

                            qrt.translated = quote.translang;
                            qrt.translatedSpecified = true;

                            qrt.specialRequirements = quote.specrequirements;

                            qrt.quoteWithCovers = quote.quotecover;
                            qrt.quoteWithCoversSpecified = true;

                            qrt.quoteWithoutCovers = quote.quotenocover;
                            qrt.quoteWithoutCoversSpecified = true;

                            qrt.quoteForReprints = quote.quotereprints;
                            qrt.quoteForReprintsSpecified = true;

                            qrt.quoteForEprints = quote.quoteeprint;
                            qrt.quoteForEprintsSpecified = true;

                            submitquoterequest[i] = qrt;
                            i++;
                        }

                        try
                        {
                            TempData["Message"] = smartService.InsertQuoteRequest(Account.userId, submitquoterequest);
                        }
                        catch (FaultException<ErrorMessage> ex)
                        {
                            Console.WriteLine("Error Message: " + ex.Detail.errorMessage);
                            Console.WriteLine("Error Detail: " + ex.Detail.errorDetails);
                        }

                    }
                }
            }
            catch (Exception ex) // apparently not throwing the expected fault exception. handling argument exception instead.
            {
                TempData["Error"] = "";
                if (ex is FaultException)
                {
                    TempData["Error"] = "Error Message: " + ex.Message;
                }
                if (ex is ArgumentException)
                {
                    TempData["Error"] = "Error Message: " + ex.Message;
                }

                if ((TempData["Error"].ToString()) == "")
                {
                    TempData["Error"] = ex.Message;
                }
                return View("AdvancedSearch", model);
            }
            return View("Index");
        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "submitFave")]
        public ActionResult submitFave(int[] favorite)
        {
            if (favorite == null)
            {
                TempData["Error"] = "Please select at least 1 result to add to your favourites.";
                return View("Search", Session["latestresults"]);
            }
            SmartService smartService = new SmartService();
            string key, toHash, token;
            token = smartService.AuthToken();
            if (token == "")
            {

            }
            else
            {
                string username = User.Identity.Name;
                toHash = username.ToUpper() + "|" + token;
                key = smartService.GetMd5Hash(MD5.Create(), toHash) + "|" + username;

                AccountType Account = smartService.GetAccountDetails(key, username);
                TempData["Message"] = smartService.InsertFavourites(Account.userId, favorite);
            }
            return View("Index");
        }

        public ActionResult pubmed(string search_id, int Page)
        {
            SmartService smartService = new SmartService();
            List<SearchResults> ResultList = new List<SearchResults>();

            string key, toHash, token;
            token = smartService.AuthToken();
            string username = User.Identity.Name;
            toHash = username.ToUpper() + "|" + token;
            key = smartService.GetMd5Hash(MD5.Create(), toHash) + "|" + username;


            if (token == "")
            { //do something if no token

            }
            else
            {
                PubMedSummaryResultsType results = smartService.PubMedSearchSummary(key, search_id);

                if (results.numResults == 0)
                {
                }
                else
                {
                    string pagestart = ((Page * 6) - 5).ToString(); ;
                    string pageend = (Page * 6).ToString();
                    int lastpage;
                    if (Page > 1)
                    {
                        lastpage = Page - 1;
                    }
                    else
                    {
                        lastpage = Page;
                    }
                    PubMedFullResultsType summery = smartService.PubMedSearchResults(key, results, pagestart, pageend);
                    if (results.numResults == 0)
                    { //no results!
                        TempData["noresults"] = "Your PubMed search returned 0 results.";
                    }
                    else
                    {
                        foreach (PubMedArticleType at in summery.contentTypeResults)
                        { //do this with a model instead
                            SearchResults SearchResults = new SearchResults();
                            SearchResults.PMID = at.PMID;
                            SearchResults.searchId = search_id;
                            SearchResults.currentpage = Page;
                            SearchResults.lastpage = lastpage;
                            SearchResults.nextpage = Page + 1;
                            SearchResults.totalresults = results.numResults;
                            SearchResults.totalpages = (((results.numResults / 6) / 6) > 6) ? (results.numResults / 6) + 1 : (results.numResults / 6); // HARD CODEDED ALERT!
                            SearchResults.articleTitle = at.articleTitle;
                            SearchResults.authors = at.authors;
                            SearchResults.journalTitle = at.journalTitle;
                            SearchResults.publicationDate =
                                (at.publicationDate.ToString().Substring(0, 1) == "0") ? "8" + at.publicationDate.ToString().Substring(1) : at.publicationDate;
                            SearchResults.abstractText = at.abstractText;

                            ResultList.Add(SearchResults);
                        }
                    }
                }
            }

            return View(ResultList);
        }

        [HttpPost]
        public ActionResult saveSearch(savedSearch model)
        {
            // TODO: A real app would send some sort of email here

            SmartService smartService = new SmartService();
            string key, toHash, token;
            token = smartService.AuthToken();
            if (token == "")
            {

            }
            else
            {
                string username = User.Identity.Name;
                toHash = username.ToUpper() + "|" + token;
                key = smartService.GetMd5Hash(MD5.Create(), toHash) + "|" + username;

                AccountType Account = smartService.GetAccountDetails(key, username);
                string success = smartService.InsertSavedSearch(model.searchName, Account.userId, model.searchId);
                ViewBag.Status = string.Format("{0} has been saved.", model.searchName);
            }

            // A standard (non-Ajax) HTTP Post came in
            // set TempData and redirect the user back to the Home page
            TempData["Message"] = string.Format("{0} has been saved.", model.searchName);

            //Search(new Search(), false, model.searchId, model.Page);
            return RedirectToAction("Search", new { freetext = model.freetext, advanced = false, search_id = model.searchId, model.Page });
        }

        /**
         * Build the filter list for refine search
         */
        public FilterSearchResults buildfilteritems(List<string> filterlist, bool Split = false)
        {
            //group journals and count each.
            //needs improving
            var filtergroupgroup = filterlist.GroupBy(i => i);
            List<string> filtername = new List<String>();
            List<int> filtercount = new List<int>();

            foreach (var totaljour in filtergroupgroup)
            {
                if (Split == true)
                {
                    List<string> names = totaljour.Key.Split(',').ToList<string>();
                    var i = 1;
                    foreach (var value in names)
                    {
                        string trimedvalue = value.Trim();
                        var index = filtername.IndexOf(trimedvalue);
                        if (index == -1 || filtername.Count() == 0)
                        {
                            filtername.Add(trimedvalue);
                            filtercount.Add(i);
                        }
                        else
                        {
                            filtercount[index] = filtercount[index] + 1;
                        }
                        i++;
                    }
                }
                else
                {
                    filtername.Add(totaljour.Key);
                    filtercount.Add(totaljour.Count());
                }
            }

            FilterSearchResults Flist = new FilterSearchResults(filtername, filtercount);

            return Flist;
        }
    }
}
